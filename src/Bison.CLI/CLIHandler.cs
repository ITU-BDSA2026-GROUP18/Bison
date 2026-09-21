using System.CommandLine;
using SimpleDB;

class CLIHandler
{
	Microsoft.AspNetCore.Builder.WebApplication? app;

	private void startWebServer()
	{
		Microsoft.AspNetCore.Builder.WebApplicationBuilder builder = WebApplication.CreateBuilder();
		app = builder.Build();
		Console.WriteLine("------- WEB SERVER BUILD -------");
		CSVDatabase<Observation>.getInstance().start(app);
	}

	private void stopWebServer()
	{
		// work in progress
	}

	public CLIHandler(string[] args)
	{
		RootCommand rootCommand = new("Bison.CLI");
		Option<string> pathOption = new("--path", "-p")
		{
			Description = "Set root path containing \"data\" folder",
			Recursive = true,
			Arity = ArgumentArity.ExactlyOne,
		};

		var startCommand = new Command("--start", "starts the db endpoints");
		startCommand.SetAction(ParseResult => startWebServer());

		var stopCommand = new Command("--stop", "stops the db webserver, and closes endpoints");
		stopCommand.SetAction(ParseResult => stopWebServer());

		var readCommand = new Command("--read", "Prints out entire contents of CSV to the console");
		readCommand.SetAction(async parseResult =>
			await Program.read(parseResult.GetValue(pathOption))
		); //TODO: Add path option to rest of relevant Actions
		readCommand.Aliases.Add("-r");

		var discTarg = new Argument<long>("ObservationId");

		var discCommand = new Command(
			"--discussion",
			"Prints out comments to an Observation in the CSV database"
		)
		{
			discTarg,
		};
		discCommand.Aliases.Add("-d");

		discCommand.SetAction(async parseResult =>
			await Program.discussion(parseResult.GetValue(discTarg)!)
		);

		var propsTarg = new Argument<long>("ObservationId");

		var propsCommand = new Command(
			"--proposals",
			"Prints out taxon proposals to an Observation in the CSV database"
		)
		{
			propsTarg,
		};
		propsCommand.Aliases.Add("-ps");

		propsCommand.SetAction(async parseResult =>
			await Program.proposals(parseResult.GetValue(propsTarg)!)
		);

		var locTarg = new Argument<string>("Location");
		var locCommand = new Command(
			"--location",
			"Prints out observations based on location in the CSV database"
		)
		{
			locTarg,
		};
		locCommand.Aliases.Add("-l");
		locCommand.SetAction(async parseResult =>
			await Program.location(parseResult.GetValue(locTarg)!)
		);

		var obsTarg = new Argument<string>("Description");
		var locationTarg = new Argument<string>("Location");

		var obsCommand = new Command("--observe", "Add observation to the CSV database")
		{
			obsTarg,
			locationTarg,
		};
		obsCommand.Aliases.Add("-o");

		obsCommand.SetAction(async parseResult =>
			await Program.observe(
				parseResult.GetValue(obsTarg)!,
				parseResult.GetValue(locationTarg)!
			)
		);

		var commTarg = new Argument<string>("Comment");
		var commParentIdTarg = new Argument<long>("Id");

		var commCommand = new Command("--comment", "Add comment to observation based on id")
		{
			commTarg,
			commParentIdTarg,
		};
		commCommand.Aliases.Add("-c");

		commCommand.SetAction(async parseResult =>
			await Program.comment(
				parseResult.GetValue(commTarg)!,
				parseResult.GetValue(commParentIdTarg)!
			)
		);

		var propTarg = new Argument<string>("TaxonId");
		var propParentIdTarg = new Argument<long>("Id");

		var propCommand = new Command("--propose", "Add taxon proposal to observation based on id")
		{
			propTarg,
			propParentIdTarg,
		};
		propCommand.Aliases.Add("-pr");

		propCommand.SetAction(async parseResult =>
			await Program.propose(
				parseResult.GetValue(propTarg)!,
				parseResult.GetValue(propParentIdTarg)!
			)
		);

		//rootCommand.Subcommands.Add(pathCommand);

		rootCommand.Options.Add(pathOption);
		//rootCommand.SetAction(parseResult => Program.setEnvPath(parseResult.GetValue(pathOption)!));

		rootCommand.Subcommands.Add(stopCommand);
		rootCommand.Subcommands.Add(startCommand);
		rootCommand.Subcommands.Add(discCommand);
		rootCommand.Subcommands.Add(readCommand);
		rootCommand.Subcommands.Add(obsCommand);
		rootCommand.Subcommands.Add(commCommand);
		rootCommand.Subcommands.Add(locCommand);
		rootCommand.Subcommands.Add(propCommand);
		rootCommand.Subcommands.Add(propsCommand);

		rootCommand.Parse(args).Invoke();
	}
}
