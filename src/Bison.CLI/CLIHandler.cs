using System.CommandLine;

class CLIHandler
{
	public CLIHandler(string[] args)
	{
		RootCommand rootCommand = new("Bison.CLI");

		Option<string> pathOption = new("--path", "-p")
		{
			Description = "Set root path containing \"data\" folder",
			Recursive = true,
			Arity = ArgumentArity.ExactlyOne,
		};

		var readCommand = new Command("--read", "Prints out entire contents of CSV to the console");

		readCommand.SetAction(parseResult => Program.read(parseResult.GetValue(pathOption))); //TODO: Add path option to rest of relevant Actions
		readCommand.Aliases.Add("-r");

#if !WEBSERVER
		var discTarg = new Argument<long>("ObservationId");
#endif
		var discCommand = new Command(
			"--discussion",
			"Prints out comments to Observation in the CSV database"
		)
		{
#if !WEBSERVER
			discTarg,
#endif
		};
		discCommand.Aliases.Add("-d");

		discCommand.SetAction(parseResult =>
#if !WEBSERVER
			Program.discussion(parseResult.GetValue(discTarg)!)
#else
			Program.discussion()
#endif
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
		locCommand.SetAction(parseResult => Program.location(parseResult.GetValue(locTarg)!));

#if !WEBSERVER
		var obsTarg = new Argument<string>("Description");
		var locationTarg = new Argument<string>("Location");
#endif
		var obsCommand = new Command("--observe", "Add observation to the CSV database")
		{
#if !WEBSERVER
			obsTarg,
			locationTarg,
#endif
		};
		obsCommand.Aliases.Add("-o");

		obsCommand.SetAction(parseResult =>
#if !WEBSERVER
			Program.observe(parseResult.GetValue(obsTarg)!, parseResult.GetValue(locationTarg)!)
#else
			Program.observe()
#endif
		);

#if !WEBSERVER
		var commTarg = new Argument<string>("Comment");
		var idTarg = new Argument<long>("Id"); //TODO: change this to long
#endif
		var commCommand = new Command("--comment", "Add comment to observation based on id")
		{
#if !WEBSERVER
			commTarg,
			idTarg,
#endif
		};
		commCommand.Aliases.Add("-c");

		commCommand.SetAction(parseResult =>
#if !WEBSERVER
			Program.comment(parseResult.GetValue(commTarg)!, parseResult.GetValue(idTarg)!)
#else
			Program.comment()
#endif
		);

		//rootCommand.Subcommands.Add(pathCommand);

		rootCommand.Options.Add(pathOption);
		//rootCommand.SetAction(parseResult => Program.setEnvPath(parseResult.GetValue(pathOption)!));

		rootCommand.Subcommands.Add(discCommand);
		rootCommand.Subcommands.Add(readCommand);
		rootCommand.Subcommands.Add(obsCommand);
		rootCommand.Subcommands.Add(commCommand);
		rootCommand.Subcommands.Add(locCommand);

		rootCommand.Parse(args).Invoke();
	}
}
