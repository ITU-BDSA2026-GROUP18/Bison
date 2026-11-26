using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;
using Microsoft.EntityFrameworkCore;

public class PostRepository : IPostRepository
{
	private readonly BisonDbContext _context;

	public const int MaxImageBytes = 5 * 1024 * 1024;

	private static readonly HashSet<string> AllowedImageTypes = ["image/png", "image/jpeg"];

	public PostRepository(BisonDbContext context)
	{
		_context = context;
	}

	public List<Observation> GetObservations(int page, int pageSize)
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			orderby Observation.PubDate descending
			select Observation
		)
			.Include(c => c.Author)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);
		result = query.ToList();
		return result;
	}

	public List<Observation> GetObservations()
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			orderby Observation.PubDate descending
			select Observation
		)
			.Include(c => c.Author)
			.Include(c => c.Taxon);
		result = query.ToList();
		return result;
	}

	public List<Observation> GetObservationsByAuthor(string username, int page, int pageSize)
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			where Observation.Author.Name == username
			orderby Observation.PubDate descending
			select Observation
		)
			.Include(c => c.Author)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);
		result = query.ToList();
		return result;
	}

	public Observation? GetObservationById(int observationId)
	{
		Console.WriteLine($"Searching for observation with ID: {observationId}");
		var query = (
			from Observation in _context.Observations
			where Observation.PostId == observationId
			select Observation
		).Include(c => c.Author);
		return query.ToList().FirstOrDefault();
	}

	public List<Comment> GetCommentsForObservation(int observationId)
	{
		var result = new List<Comment>();
		var query = (
			from Comment in _context.Comments
			where Comment.ObservationId == observationId
			select Comment
		)
			.Include(c => c.Author)
			.Include(c => c.Observation);

		result = [.. query];
		return result;
	}

	public List<Comment> GetCommentsByAuthor(string username, int page, int pageSize)
	{
		var result = new List<Comment>();
		var query = (
			from Comment in _context.Comments
			where Comment.Author.Name == username
			select Comment
		)
			.Include(c => c.Author)
			.Include(c => c.Observation)
			.OrderByDescending(c => c.PubDate)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);

		result = [.. query];
		return result;
	}

	public List<Proposal> GetProposalsForObservation(int observationId)
	{
		var result = new List<Proposal>();
		var query = (
			from Proposal in _context.Proposals
			where Proposal.ObservationId == observationId
			select Proposal
		)
			.Include(c => c.Author)
			.Include(c => c.Taxon);

		result = [.. query];
		return result;
	}

	public List<Proposal> GetProposalsByAuthor(string username, int page, int pageSize)
	{
		var result = new List<Proposal>();
		var query = (
			from Proposal in _context.Proposals
			where Proposal.Author.Name == username
			select Proposal
		)
			.Include(c => c.Author)
			.Include(c => c.Taxon)
			.OrderByDescending(c => c.PubDate)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);

		result = [.. query];
		return result;
	}

	public Taxon GetTaxonById(int taxonId)
	{
		var taxon = (from Taxon in _context.Taxons where Taxon.TaxonId == taxonId select Taxon);

		return taxon.FirstOrDefault();
	}

	public Comment AddCommentToObservation(int observationId, string authorId, CommentInput input)
	{
		var observation =
			GetObservationById(observationId)
			?? throw new ArgumentException($"Observation with ID {observationId} not found.");

		if (
			input.ParentCommentId is int parentId
			&& !_context.Comments.Any(c => c.PostId == parentId && c.ObservationId == observationId)
		)
		{
			throw new ArgumentException(
				$"Comment {parentId} not found on observation {observationId}."
			);
		}

		var comment = new Comment
		{
			ObservationId = observation.PostId,
			ParentCommentId = input.ParentCommentId,
			AuthorId = authorId,
			Text = input.Content,
			PubDate = DateTime.UtcNow,
		};

		_context.Comments.Add(comment);
		_context.SaveChanges();
		return comment;
	}

	public Task<int> ReassignPostsByUserId(string userId)
	{
		return _context
			.Posts.Where(post => post.AuthorId == userId)
			.ExecuteUpdateAsync(update =>
				update.SetProperty(post => post.AuthorId, DeletedUserid.deletedUserId)
			);
	}

	public Observation AddObservation(
		string authorId,
		string text,
		int taxonId,
		byte[]? image = null,
		string? imageContentType = null
	)
	{
		if (string.IsNullOrWhiteSpace(authorId))
			throw new ArgumentException("Author is required.", nameof(authorId));

		if (string.IsNullOrWhiteSpace(text))
			throw new ArgumentException("Text must not be empty.", nameof(text));

		if (!_context.Users.Any(u => u.Id == authorId))
			throw new ArgumentException($"User with ID {authorId} not found.", nameof(authorId));

		if (!_context.Taxons.Any(t => t.TaxonId == taxonId))
			throw new ArgumentException($"Taxon with ID {taxonId} not found.", nameof(taxonId));

		if (image is not null)
		{
			if (image.Length == 0)
				throw new ArgumentException("Image must not be empty.", nameof(image));
			if (image.Length > MaxImageBytes)
				throw new ArgumentException("Image is too large.", nameof(image));
			if (imageContentType is null || !AllowedImageTypes.Contains(imageContentType))
				throw new ArgumentException("Unsupported image type.", nameof(imageContentType));
		}

		var observation = new Observation
		{
			AuthorId = authorId,
			Text = text.Trim(),
			TaxonId = taxonId,
			PubDate = DateTime.UtcNow,
			Image = image,
			ImageContentType = image is null ? null : imageContentType,
		};

		_context.Observations.Add(observation);
		_context.SaveChanges();
		return observation;
	}
}
