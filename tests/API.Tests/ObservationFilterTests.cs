public class ObservationFilterTests
{
	[Fact]
	public void FilterBy_EmptyInput_ReturnsEmptyList()
	{
		var root = new Taxon { TaxonId = 1 };

		var result = ObservationFilter.FilterBy(root, Array.Empty<Observation>());

		Assert.Empty(result);
	}

	[Fact]
	public void FilterBy_ReturnsMatchingObservationsInInputOrder()
	{
		var root = new Taxon { TaxonId = 1 };
		var first = CreateObservation(1, 1);
		var excluded = CreateObservation(2, 2);
		var last = CreateObservation(3, 1);

		var result = ObservationFilter.FilterBy(root, [first, excluded, last]);

		Assert.Equal([first, last], result);
	}

	[Fact]
	public void GetObservationsByTaxon_FiltersThenPaginatesResults()
	{
		var root = new Taxon { TaxonId = 1 };
		var observations = new List<Observation>
		{
			CreateObservation(1, 1),
			CreateObservation(2, 1),
			CreateObservation(3, 1),
			CreateObservation(4, 1),
			CreateObservation(5, 2),
		};
		var repository = new FakePostRepository(root, observations);
		var service = new ObservationService(repository);

		var result = service.GetObservationsByTaxon(root.TaxonId, page: 2, pageSize: 2);

		Assert.Equal([3, 4], result.Select(observation => observation.Id));
		Assert.Equal(1, repository.GetAllObservationsCallCount);
	}

	private static Observation CreateObservation(int postId, int taxonId)
	{
		return new Observation
		{
			PostId = postId,
			Taxon = new Taxon { TaxonId = taxonId },
			Author = new User
			{
				UserId = 1,
				Username = $"user-{postId}",
				Email = $"user-{postId}@example.test",
			},
			Text = $"observation-{postId}",
			PubDate = DateTime.UnixEpoch.AddDays(postId),
		};
	}

	private sealed class FakePostRepository(Taxon root, List<Observation> observations)
		: IPostRepository
	{
		public int GetAllObservationsCallCount { get; private set; }

		public List<Observation> GetObservations(int page, int pageSize) =>
			throw new NotSupportedException();

		public List<Observation> GetObservations()
		{
			GetAllObservationsCallCount++;
			return observations;
		}

		public List<Observation> GetObservationsByAuthor(string username, int page, int pageSize) =>
			throw new NotSupportedException();

		public Observation? GetObservationById(int observationId) =>
			throw new NotSupportedException();

		public List<Comment> GetCommentsForObservation(int observationId) =>
			throw new NotSupportedException();

		public List<Proposal> GetProposalsForObservation(int observationId) =>
			throw new NotSupportedException();

		public Taxon GetTaxonById(int taxonId) => taxonId == root.TaxonId ? root : null!;
	}
}
