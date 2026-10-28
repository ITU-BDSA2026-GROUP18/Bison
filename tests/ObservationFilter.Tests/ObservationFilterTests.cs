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
		var other = new Taxon { TaxonId = 2 };

		var first = CreateObservation(1, root);
		var excluded = CreateObservation(2, other);
		var last = CreateObservation(3, root);

		var result = ObservationFilter.FilterBy(root, [first, excluded, last]);

		Assert.Equal([first, last], result);
	}

	[Fact]
	public void FilterBy_ReturnsObservationsForSubTaxons()
	{
		var root = new Taxon { TaxonId = 1 };
		var child = new Taxon { TaxonId = 2, ParentTaxon = root };
		var grandchild = new Taxon { TaxonId = 3, ParentTaxon = child };
		var other = new Taxon { TaxonId = 4 };
		var first = CreateObservation(1, root);
		var second = CreateObservation(2, child);
		var third = CreateObservation(3, grandchild);
		var excluded = CreateObservation(4, other);

		var result = ObservationFilter.FilterBy(root, [first, second, third, excluded]);

		Assert.Equal([first, second, third], result);
	}

	[Fact]
	public void isSubtaxon_ReturnsTrueForSubtaxons()
	{
		var root = new Taxon { TaxonId = 1 };
		var child = new Taxon { TaxonId = 2, ParentTaxon = root };
		var grandchild = new Taxon { TaxonId = 3, ParentTaxon = child };
		var random = new Taxon { TaxonId = 4 };

		Assert.True(grandchild.isSubtaxon(root));
		Assert.True(child.isSubtaxon(root));
		Assert.True(root.isSubtaxon(root));
		Assert.False(random.isSubtaxon(root));
	}

	[Fact]
	public void GetObservationsByTaxon_FiltersThenPaginatesResults()
	{
		var root = new Taxon { TaxonId = 1 };
		var observations = new List<Observation>
		{
			CreateObservation(1, root),
			CreateObservation(2, root),
			CreateObservation(3, root),
			CreateObservation(4, root),
			CreateObservation(5, root),
		};
		var repository = new FakePostRepository(root, observations);
		var service = new ObservationService(repository);

		var result = service.GetObservationsByTaxon(root.TaxonId, page: 2, pageSize: 2);

		Assert.Equal([3, 4], result.Select(observation => observation.Id));
		Assert.Equal(1, repository.GetAllObservationsCallCount);
	}

	private static Observation CreateObservation(int postId, Taxon taxon)
	{
		return new Observation
		{
			PostId = postId,
			Taxon = taxon,
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
