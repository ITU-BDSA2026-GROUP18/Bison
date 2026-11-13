using System.Globalization;
using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;
using Microsoft.VisualBasic;

public record ObservationViewModel(string Author, string Message, string Timestamp, int Id = 0);

public record CommentViewModel(
	int Id,
	string Author,
	string Message,
	string Timestamp,
	int? ParentCommentId = null,
	List<CommentViewModel>? Replies = null
);

public record ProposalViewModel(
	string Author,
	string Message,
	string DwcTaxonId,
	string VernacularName,
	string Timestamp
);

public interface IObservationService
{
	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32);
	public List<ObservationViewModel> GetObservationsFromAuthor(
		string author,
		int page,
		int pageSize = 32
	);
	public ObservationViewModel GetObservationById(int observationId);
	public List<CommentViewModel> GetCommentsForObservation(
		int observationId,
		bool onlyRootComments
	);
	public List<ProposalViewModel> GetProposalsForObservation(int observationId);
	public List<ObservationViewModel> GetObservationsByTaxon(
		int taxonId,
		int page,
		int pageSize = 32
	);

	public int AddCommentToObservation(int observationId, string authorId, CommentInput input);
}

public class ObservationService : IObservationService
{
	// These would normally be loaded from a database for example
	private readonly IPostRepository _repository;

	public ObservationService(IPostRepository repository)
	{
		_repository = repository;
	}

	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _repository.GetObservations(page, pageSize))
		{
			result.Add(ToViewModel(observation));
		}
		return result;
	}

	public List<ObservationViewModel> GetObservationsFromAuthor(
		string author,
		int page,
		int pageSize = 32
	)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _repository.GetObservationsByAuthor(author, page, pageSize))
		{
			result.Add(ToViewModel(observation));
		}

		return result;
	}

	public ObservationViewModel? GetObservationById(int observationId)
	{
		var observation = _repository.GetObservationById(observationId);
		if (observation is not null)
			return ToViewModel(observation);
		return null;
	}

	public List<CommentViewModel> GetCommentsForObservation(
		int observationId,
		bool onlyRootComments
	)
	{
		var result = new List<CommentViewModel>();

		foreach (var comment in _repository.GetCommentsForObservation(observationId))
		{
			if (onlyRootComments && comment.ParentCommentId.HasValue)
			{
				continue;
			}
			result.Add(ToViewModel(comment));
		}

		return result;
	}

	public List<ProposalViewModel> GetProposalsForObservation(int observationId)
	{
		var result = new List<ProposalViewModel>();

		foreach (var proposal in _repository.GetProposalsForObservation(observationId))
		{
			result.Add(ToViewModel(proposal));
		}

		return result;
	}

	public List<ObservationViewModel> GetObservationsByTaxon(
		int taxonId,
		int page,
		int pageSize = 32
	)
	{
		Taxon taxon;
		if (taxonId < 0 || (taxon = _repository.GetTaxonById(taxonId)) == null)
		{
			return GetObservations(page, pageSize);
		}

		var result = new List<ObservationViewModel>();

		foreach (
			var observation in ObservationFilter
				.FilterBy(taxon, _repository.GetObservations())
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
		)
		{
			result.Add(ToViewModel(observation));
		}

		return result;
	}

	public int AddCommentToObservation(int observationId, string authorId, CommentInput input)
	{
		Comment comment = _repository.AddCommentToObservation(observationId, authorId, input);
		return comment.PostId;
	}

	private static ObservationViewModel ToViewModel(Observation o)
	{
		return new ObservationViewModel(
			o.Author.Name,
			o.Text,
			o.PubDate.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture),
			o.PostId
		);
	}

	private static CommentViewModel ToViewModel(Comment c)
	{
		return new CommentViewModel(
			c.PostId,
			c.Author.Name,
			c.Text,
			c.PubDate.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture),
			c.ParentCommentId,
			c.Replies.OrderBy(r => r.PubDate).Select(ToViewModel).ToList()
		);
	}

	private static ProposalViewModel ToViewModel(Proposal p)
	{
		return new ProposalViewModel(
			p.Author.Name,
			p.Text,
			p.Taxon.DwcTaxonId,
			p.Taxon.VernacularName,
			p.PubDate.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
		);
	}
}

public class UnixTime
{
	public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds(unixTimeStamp);
		return dateTime.ToString("MM/dd/yy H:mm:ss");
	}
}
