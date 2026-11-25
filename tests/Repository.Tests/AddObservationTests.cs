using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repository.Tests;

public sealed class Fixture : IDisposable
{
	private readonly SqliteConnection _connection = new("DataSource=:memory:");
	public BisonDbContext Context { get; }
	public PostRepository Repo { get; }

	public Fixture()
	{
		_connection.Open();
		var options = new DbContextOptionsBuilder<BisonDbContext>().UseSqlite(_connection).Options;
		Context = new BisonDbContext(options);
		Context.Database.EnsureCreated();
		Context.Users.Add(
			new User
			{
				Id = "u1",
				Name = "alice",
				UserName = "alice",
				Email = "a@x.test",
			}
		);
		Context.Users.Add(
			new User
			{
				Id = "u2",
				Name = "bob",
				UserName = "bob",
				Email = "b@x.test",
			}
		);
		Context.Taxons.Add(new Taxon { TaxonId = 1, DwcTaxonId = "t1" });
		Context.SaveChanges();
		Repo = new PostRepository(Context);
	}

	public void Dispose()
	{
		Context.Dispose();
		_connection.Dispose();
	}
}

public class AddObservationTests : IDisposable
{
	private readonly Fixture _f = new();
	private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3];

	public void Dispose() => _f.Dispose();

	[Fact]
	public void ValidInput_IsPersistedWithAuthorTextTaxonAndDate()
	{
		var before = DateTime.UtcNow.AddSeconds(-2);
		var obs = _f.Repo.AddObservation("u1", "  A bison  ", 1);

		Assert.True(obs.PostId > 0);
		var stored = _f.Repo.GetObservationById(obs.PostId);
		Assert.NotNull(stored);
		Assert.Equal("u1", stored.AuthorId);
		Assert.Equal("alice", stored.Author.Name);
		Assert.Equal("A bison", stored.Text);
		Assert.Equal(1, stored.TaxonId);
		Assert.True(stored.PubDate >= before);
		Assert.Null(stored.Image);
		Assert.Null(stored.ImageUrl);
	}

	[Fact]
	public void NewObservation_AppearsInPublicListAndAuthorList_NotOthers()
	{
		var obs = _f.Repo.AddObservation("u1", "mine", 1);

		Assert.Contains(_f.Repo.GetObservations(), o => o.PostId == obs.PostId);
		Assert.Contains(_f.Repo.GetObservations(1, 10), o => o.PostId == obs.PostId);
		Assert.Contains(
			_f.Repo.GetObservationsByAuthor("alice", 1, 10),
			o => o.PostId == obs.PostId
		);
		Assert.DoesNotContain(
			_f.Repo.GetObservationsByAuthor("bob", 1, 10),
			o => o.PostId == obs.PostId
		);
	}

	[Fact]
	public void Observations_AreReturnedNewestFirst()
	{
		var older = _f.Repo.AddObservation("u1", "older", 1);
		var newer = _f.Repo.AddObservation("u1", "newer", 1);
		older.PubDate = DateTime.UtcNow.AddDays(-1);
		newer.PubDate = DateTime.UtcNow;
		_f.Context.SaveChanges();

		var list = _f.Repo.GetObservations(1, 10);
		Assert.Equal(["newer", "older"], list.Select(o => o.Text).ToArray());
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void BlankText_Throws(string text) =>
		Assert.Throws<ArgumentException>(() => _f.Repo.AddObservation("u1", text, 1));

	[Fact]
	public void UnknownTaxon_Throws() =>
		Assert.Throws<ArgumentException>(() => _f.Repo.AddObservation("u1", "x", 999));

	[Fact]
	public void UnknownUser_Throws() =>
		Assert.Throws<ArgumentException>(() => _f.Repo.AddObservation("nobody", "x", 1));

	[Fact]
	public void Failure_DoesNotPersistAnything()
	{
		Assert.Throws<ArgumentException>(() => _f.Repo.AddObservation("u1", "x", 999));
		Assert.Empty(_f.Repo.GetObservations());
	}

	[Fact]
	public void Image_IsStoredWithContentType()
	{
		var obs = _f.Repo.AddObservation("u1", "pic", 1, Png, "image/png");

		var stored = _f.Repo.GetObservationById(obs.PostId)!;
		Assert.Equal(Png, stored.Image);
		Assert.Equal("image/png", stored.ImageContentType);
		Assert.Equal($"/ob/{stored.PostId}/image", stored.ImageUrl);
	}

	[Fact]
	public void EmptyImage_Throws() =>
		Assert.Throws<ArgumentException>(() =>
			_f.Repo.AddObservation("u1", "x", 1, [], "image/png")
		);

	[Fact]
	public void OversizedImage_Throws() =>
		Assert.Throws<ArgumentException>(() =>
			_f.Repo.AddObservation(
				"u1",
				"x",
				1,
				new byte[PostRepository.MaxImageBytes + 1],
				"image/png"
			)
		);

	[Theory]
	[InlineData("text/html")]
	[InlineData("image/svg+xml")]
	[InlineData(null)]
	public void UnsupportedImageType_Throws(string? type) =>
		Assert.Throws<ArgumentException>(() => _f.Repo.AddObservation("u1", "x", 1, Png, type));
}

public class ObservationServiceAddTests : IDisposable
{
	private readonly Fixture _f = new();
	private ObservationService Service => new(_f.Repo);

	public void Dispose() => _f.Dispose();

	private static IFormFile File(byte[] bytes, string name = "a.png") =>
		new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Image", name);

	[Fact]
	public async Task TextOnly_ReturnsIdAndAppearsInTimelines()
	{
		var id = await Service.AddObservationAsync(
			"u1",
			new ObservationInput { Text = "hi", TaxonId = 1 }
		);

		Assert.Contains(Service.GetObservationsFromAuthor("alice", 1), o => o.Id == id);
		Assert.Contains(Service.GetObservations(1), o => o.Id == id);
	}

	[Fact]
	public async Task ValidPng_IsStoredWithDetectedType()
	{
		byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 9];
		var id = await Service.AddObservationAsync(
			"u1",
			new ObservationInput
			{
				Text = "hi",
				TaxonId = 1,
				Image = File(png),
			}
		);

		var stored = _f.Repo.GetObservationById(id)!;
		Assert.Equal("image/png", stored.ImageContentType);
		Assert.Equal(png, stored.Image);
	}

	[Fact]
	public async Task DetectedType_OverridesClientName()
	{
		byte[] jpg = [0xFF, 0xD8, 0xFF, 0xE0, 1];
		var id = await Service.AddObservationAsync(
			"u1",
			new ObservationInput
			{
				Text = "hi",
				TaxonId = 1,
				Image = File(jpg, "x.png"),
			}
		);
		Assert.Equal("image/jpeg", _f.Repo.GetObservationById(id)!.ImageContentType);
	}

	[Fact]
	public async Task NonImageBytes_AreRejected()
	{
		var html = System.Text.Encoding.UTF8.GetBytes("<html><script>alert(1)</script></html>");
		await Assert.ThrowsAsync<ArgumentException>(() =>
			Service.AddObservationAsync(
				"u1",
				new ObservationInput
				{
					Text = "hi",
					TaxonId = 1,
					Image = File(html),
				}
			)
		);
		Assert.Empty(_f.Repo.GetObservations());
	}

	[Fact]
	public async Task OversizedFile_IsRejected()
	{
		var big = new byte[PostRepository.MaxImageBytes + 1];
		big[0] = 0x89;
		await Assert.ThrowsAsync<ArgumentException>(() =>
			Service.AddObservationAsync(
				"u1",
				new ObservationInput
				{
					Text = "hi",
					TaxonId = 1,
					Image = File(big),
				}
			)
		);
	}
}

public class ImageSnifferTests
{
	[Fact]
	public void Png() =>
		Assert.Equal(
			"image/png",
			ImageSniffer.Detect([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0])
		);

	[Fact]
	public void Jpeg() => Assert.Equal("image/jpeg", ImageSniffer.Detect([0xFF, 0xD8, 0xFF, 0xE1]));

	[Fact]
	public void Empty_ReturnsNull() => Assert.Null(ImageSniffer.Detect([]));

	[Fact]
	public void TooShort_ReturnsNull() => Assert.Null(ImageSniffer.Detect([0x89, 0x50]));

	[Fact]
	public void Html_ReturnsNull() =>
		Assert.Null(ImageSniffer.Detect(System.Text.Encoding.UTF8.GetBytes("<html></html>")));
}
