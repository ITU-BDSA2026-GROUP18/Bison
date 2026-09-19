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
		CSVDatabase<Comment>.getInstance().start(app);
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
		readCommand.SetAction(parseResult => Program.read(parseResult.GetValue(pathOption))); //TODO: Add path option to rest of relevant Actions
		readCommand.Aliases.Add("-r");

		var discTarg = new Argument<long>("ObservationId");

		var discCommand = new Command(
			"--discussion",
			"Prints out comments to Observation in the CSV database"
		)
		{
			discTarg,
		};
		discCommand.Aliases.Add("-d");

		discCommand.SetAction(parseResult => Program.discussion(parseResult.GetValue(discTarg)!));

		var locTarg = new Argument<string>("Location");
		var locCommand = new Command(
			"--location",
			"Prints out observations based on location in the CSV database"
		)
		{
			locTarg,
		};
		locCommand.Aliases.Add("-l");
		locCommand.SetAction(parseResult => Program.location(parseResult.GetValue(locTarg)!));

		var obsTarg = new Argument<string>("Description");
		var locationTarg = new Argument<string>("Location");

		var obsCommand = new Command("--observe", "Add observation to the CSV database")
		{
			obsTarg,
			locationTarg,
		};
		obsCommand.Aliases.Add("-o");

		obsCommand.SetAction(parseResult =>
			Program.observe(parseResult.GetValue(obsTarg)!, parseResult.GetValue(locationTarg)!)
		);

		var commTarg = new Argument<string>("Comment");
		var idTarg = new Argument<long>("Id"); //TODO: change this to long

		var commCommand = new Command("--comment", "Add comment to observation based on id")
		{
			commTarg,
			idTarg,
		};
		commCommand.Aliases.Add("-c");

		commCommand.SetAction(parseResult =>
			Program.comment(parseResult.GetValue(commTarg)!, parseResult.GetValue(idTarg)!)
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

		rootCommand.Parse(args).Invoke();
	}
}
