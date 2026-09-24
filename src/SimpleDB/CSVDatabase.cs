namespace SimpleDB;

using System.Globalization;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

/*public record Observationn(
	long Id,
	string Author,
	string Description,
	long Timestamp,
	string Location
);

public record Comment(long ParentId, string Author, string Description, long Timestamp);

public record Proposal(long ParentId, string Author, string TaxonId, long Timestamp);

//Most relevant fields from joined.csv added. More exist.
public record Taxon
{
	[Name("dwc:taxonID")]
	public required string TaxonId;

	[Name("dwc:parentNameUsageID")]
	public string? ParentId;

	[Name("dwc:taxonRank")]
	public string? TaxonRank;

	[Name("dwc:scientificName")]
	public string? ScientificName;

	[Name("dwc:vernacularName")]
	public string? VernacularName;
}

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
	public static string ObserveDatabasePath { get; set; } = "./data/bison_observe_cli_db.csv";

	public static string CommentDatabasePath { get; set; } = "./data/bison_comment_cli_db.csv";

	public static string ProposalDatabasePath { get; set; } = "./data/bison_proposal_cli_db.csv";

	public static string TaxonDatabasePath { get; set; } = "./data/taxon.csv";

	private string dbpath = "/data/bison_observe_cli_db.csv";

	public StreamReader Reader()
	{
		if (dbpath == TaxonDatabasePath)
		{
			var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
			var reader = embeddedProvider.GetFileInfo("./data/taxon.csv").CreateReadStream();
			return new StreamReader(reader);
		}
		return new StreamReader(dbpath);
	}

	private WebApplication? app;

	private CSVDatabase() { }

	private static CSVDatabase<T> instance = new();

	public static CSVDatabase<T> getInstance()
	{
		return instance;
	}

	public void Configure(WebApplication app_, bool testing)
	{
		app = app_;

		setReadEndpoints();
		setStoreEndpoints();

		if (!testing)
			app.Run();
	}

	private IEnumerable<T> internalRead(int? limit = null)
	{
		using var reader = Reader();
		var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
		var records = csv.GetRecords<T>().ToList();
		return records;
	}

	private void internalStore(T record)
	{
		using var writer = new StreamWriter(dbpath, append: true);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

		csv.NextRecord();
		csv.WriteRecord(record);
	}

	private static IEnumerable<Comment> getMatchingId(long Id)
	{
		List<Comment> filteredRecords = new List<Comment>();

		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		var records = database.internalRead();

		foreach (var record in records)
		{
			if (record.ParentId == Id)
			{
				filteredRecords.Add(record);
			}
		}
		return filteredRecords;
	}

	private static bool doesIdExist(long Id)
	{
		var csvData = CSVDatabase<Observation>.getInstance();
		csvData.setPath(ObserveDatabasePath);
		var csvRec = csvData.internalRead();
		foreach (var existingRecord in csvRec)
		{
			if (existingRecord.Id == Id)
				return true;
		}

		return false;
	}

	public void setReadEndpoints()
	{
		var observationsDB = CSVDatabase<Observation>.getInstance();
		var commentsDB = CSVDatabase<Comment>.getInstance();
		var proposalsDB = CSVDatabase<Proposal>.getInstance();
		observationsDB.setPath(ObserveDatabasePath);
		commentsDB.setPath(CommentDatabasePath);
		proposalsDB.setPath(ProposalDatabasePath);
		var commentRec = commentsDB.internalRead(); // its never used?

		app.MapGet("/observations", () => observationsDB.internalRead());

		app.MapGet(
			"/comments/{netId}",
			(long netId) =>
			{
				var list = commentsDB.internalRead();
				var res = new List<Comment>();
				foreach (Comment c in list)
				{
					if (c.ParentId == netId)
					{
						res.Add(c);
					}
				}
				return res;
			}
		);

		app.MapGet(
			"/proposals/{netId}",
			(long netId) =>
			{
				var list = proposalsDB.internalRead();
				var res = new List<Proposal>();
				foreach (Proposal p in list)
				{
					if (p.ParentId == netId)
					{
						res.Add(p);
					}
				}
				return res;
			}
		);
	}

	public void setStoreEndpoints()
	{
		var observationsDB = CSVDatabase<Observation>.getInstance();
		var commentsDB = CSVDatabase<Comment>.getInstance();
		var proposalsDB = CSVDatabase<Proposal>.getInstance();
		observationsDB.setPath(ObserveDatabasePath);
		commentsDB.setPath(CommentDatabasePath);
		proposalsDB.setPath(ProposalDatabasePath);

		app.MapPost(
			"/observation",
			(Observation netObservation) =>
			{
				observationsDB.internalStore(netObservation);
			}
		);

		app.MapPost(
			"/comment",
			(Comment netComment) =>
			{
				if (doesIdExist(netComment.ParentId))
				{
					commentsDB.setPath(CommentDatabasePath);
					commentsDB.internalStore(netComment);
					return Results.Created($"created {netComment}", netComment);
				}
				else
				{
					return Results.BadRequest(netComment);
				}
			}
		);

		app.MapPost(
			"/proposal",
			(Proposal netProposal) =>
			{
				if (doesIdExist(netProposal.ParentId))
				{
					proposalsDB.setPath(ProposalDatabasePath);
					proposalsDB.internalStore(netProposal);
					return Results.Created($"created {netProposal}", netProposal);
				}
				else
				{
					return Results.BadRequest(netProposal);
				}
			}
		);
	}

	public IEnumerable<T> read()
	{
		return internalRead();
	}

	public void store(Observation r)
	{
		using var writer = new StreamWriter(dbpath, append: true);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

		csv.NextRecord();
		csv.WriteRecord(r);
	}

	//returns the first taxon that matches the given taxon. If no match is found null is returned.
	public Taxon? findTaxon(Taxon t)
	{
		var database = CSVDatabase<Taxon>.getInstance();
		database.setPath(TaxonDatabasePath);
		var records = database.internalRead();
		foreach (var rec in records)
		{
			if (rec.TaxonId == t.TaxonId || rec.VernacularName == t.VernacularName)
				return rec;
		}
		return null;
	}

	//retrieves the direct parent of a taxon. If no parent exist null is returned. Hence the taxon is the root
	public Taxon? getTaxonParent(Taxon t)
	{
		return findTaxon(new Taxon { TaxonId = t.ParentId });
	}

	public Taxon? findTaxonByVenicularName(string name)
	{
		return findTaxon(new Taxon { TaxonId = "", VernacularName = name });
	}

	//returns a list of all the direct children of a taxon.
	public IEnumerable<Taxon> getTaxonChildren(Taxon t)
	{
		string Id = t.TaxonId;
		List<Taxon> children = new List<Taxon>();

		var database = CSVDatabase<Taxon>.getInstance();
		database.setPath(TaxonDatabasePath);
		var records = database.internalRead();

		foreach (var record in records)
		{
			if (record.ParentId == Id)
			{
				children.Add(record);
			}
		}
		return children;
	}

	public void storeNoAppend(Observation r)
	{
		using var writer = new StreamWriter(dbpath);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		csv.WriteHeader<T>();
		csv.NextRecord();
		csv.WriteRecord(r);
	}

	public void setPath(string path)
	{
		dbpath = path;
	}
}*/
