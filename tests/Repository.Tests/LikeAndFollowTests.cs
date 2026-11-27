using Bison.Razor.DBObjects;

namespace Repository.Tests;

public class LikeTests : IDisposable
{
	private readonly Fixture _f = new();

	public void Dispose() => _f.Dispose();

	[Fact]
	public void NewObservation_HasZeroLikes()
	{
		var obs = _f.Repo.AddObservation("u1", "bison", 1);
		Assert.Equal(0, _f.Repo.GetObservationById(obs.PostId)!.Likes);
	}

	[Fact]
	public void ToggleLike_LikesThenUnlikes()
	{
		var obs = _f.Repo.AddObservation("u1", "bison", 1);

		var liked = _f.Repo.ToggleLike(obs.PostId, "u2");
		Assert.True(liked.Liked);
		Assert.Equal(1, liked.Likes);
		Assert.Equal(1, _f.Repo.GetObservationById(obs.PostId)!.Likes);
		Assert.Contains(obs.PostId, _f.Repo.GetLikedObservationIds("u2", [obs.PostId]));

		var unliked = _f.Repo.ToggleLike(obs.PostId, "u2");
		Assert.False(unliked.Liked);
		Assert.Equal(0, unliked.Likes);
		Assert.Empty(_f.Repo.GetLikedObservationIds("u2", [obs.PostId]));
	}

	[Fact]
	public void Likes_FromDifferentUsersAreCounted()
	{
		var obs = _f.Repo.AddObservation("u1", "bison", 1);
		_f.Repo.ToggleLike(obs.PostId, "u1");
		var result = _f.Repo.ToggleLike(obs.PostId, "u2");

		Assert.Equal(2, result.Likes);
		Assert.Equal(2, _f.Repo.GetObservations(1, 10).Single().Likes);
	}

	[Fact]
	public void ToggleLike_UnknownObservation_Throws() =>
		Assert.Throws<ArgumentException>(() => _f.Repo.ToggleLike(999, "u1"));

	[Fact]
	public void ToggleLike_UnknownUser_Throws()
	{
		var obs = _f.Repo.AddObservation("u1", "bison", 1);
		Assert.Throws<ArgumentException>(() => _f.Repo.ToggleLike(obs.PostId, "nobody"));
	}

	[Fact]
	public async Task RemoveLikesByUserId_UpdatesCounts()
	{
		var obs = _f.Repo.AddObservation("u1", "bison", 1);
		_f.Repo.ToggleLike(obs.PostId, "u1");
		_f.Repo.ToggleLike(obs.PostId, "u2");

		await _f.Repo.RemoveLikesByUserId("u2");

		Assert.Equal(1, _f.Repo.GetObservationById(obs.PostId)!.Likes);
		Assert.Empty(_f.Repo.GetLikedObservationIds("u2", [obs.PostId]));
	}

	[Fact]
	public void Service_ReportsLikedByCurrentUser()
	{
		var service = new ObservationService(_f.Repo);
		var obs = _f.Repo.AddObservation("u1", "bison", 1);
		service.ToggleLike(obs.PostId, "u2");

		var asBob = service.GetObservations(1, currentUserId: "u2").Single();
		var asAlice = service.GetObservations(1, currentUserId: "u1").Single();
		var signedOut = service.GetObservations(1).Single();

		Assert.True(asBob.LikedByCurrentUser);
		Assert.False(asAlice.LikedByCurrentUser);
		Assert.False(signedOut.LikedByCurrentUser);
		Assert.Equal(1, signedOut.Likes);
	}
}

public class FollowTests : IDisposable
{
	private readonly Fixture _f = new();
	private readonly UserRepository _users;

	public FollowTests()
	{
		_f.Context.Users.Add(
			new User
			{
				Id = "u3",
				Name = "carol",
				UserName = "carol",
				Email = "c@x.test",
			}
		);
		_f.Context.SaveChanges();
		_users = new UserRepository(_f.Context);
	}

	public void Dispose() => _f.Dispose();

	[Fact]
	public void ToggleFollow_FollowsThenUnfollows()
	{
		Assert.True(_users.ToggleFollow("u1", "u2"));
		Assert.True(_users.IsFollowing("u1", "u2"));
		Assert.False(_users.IsFollowing("u2", "u1"));

		Assert.False(_users.ToggleFollow("u1", "u2"));
		Assert.False(_users.IsFollowing("u1", "u2"));
	}

	[Fact]
	public void FollowingYourself_Throws()
	{
		Assert.Throws<ArgumentException>(() => _users.ToggleFollow("u1", "u1"));
		Assert.False(_users.IsFollowing("u1", "u1"));
	}

	[Fact]
	public void FollowingUnknownUser_Throws() =>
		Assert.Throws<ArgumentException>(() => _users.ToggleFollow("u1", "nobody"));

	[Fact]
	public void FollowingFeed_OnlyContainsFollowedAuthors()
	{
		var fromBob = _f.Repo.AddObservation("u2", "bob's bison", 1);
		var fromCarol = _f.Repo.AddObservation("u3", "carol's bison", 1);
		var fromAlice = _f.Repo.AddObservation("u1", "alice's bison", 1);
		_users.ToggleFollow("u1", "u2");

		var feed = _f.Repo.GetObservationsByFollowedAuthors("u1", 1, 10);

		Assert.Equal([fromBob.PostId], feed.Select(o => o.PostId).ToArray());
		Assert.Empty(_f.Repo.GetObservationsByFollowedAuthors("u3", 1, 10));
	}

	[Fact]
	public void Profile_ShowsFollowStateAndHidesButtonOnOwnProfile()
	{
		var service = new UserService(_users);
		service.ToggleFollow("u1", "bob");

		var bobAsAlice = service.GetProfile("bob", "u1")!;
		var carolAsAlice = service.GetProfile("carol", "u1")!;
		var aliceAsAlice = service.GetProfile("alice", "u1")!;
		var bobSignedOut = service.GetProfile("bob", null)!;

		Assert.True(bobAsAlice.IsFollowing);
		Assert.True(bobAsAlice.ShowFollowButton);
		Assert.False(carolAsAlice.IsFollowing);
		Assert.False(aliceAsAlice.ShowFollowButton);
		Assert.True(bobSignedOut.ShowFollowButton);
		Assert.False(bobSignedOut.IsFollowing);
		Assert.Null(service.GetProfile("nobody", "u1"));
	}
}
