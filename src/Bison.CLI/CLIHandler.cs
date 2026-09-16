using System.CommandLine;

class CLIHandler
{
	public CLIHandler(string[] args)
	{
		RootCommand rootCommand = new("Bison.CLI");

		var readCommand = new Command("--read", "Prints out entire contents of CSV to the console");

		readCommand.SetAction(parseResult => Program.read());
		readCommand.Aliases.Add("-r");

		// terrible, terrible name. root path should be set by separate (optional) parameter, 
		// exists for now to deal with testing.
		var readPathTarg = new Argument<string>("RootPath"); 
		var readPathCommand = new Command(
			"--ReadWithRootPath",
			"Prints out comments to Observation in the CSV database based on Manually set Root Path"
		)
		{
			readPathTarg,
		};
		readPathCommand.Aliases.Add("-rwrp");

		readPathCommand.SetAction(parseResult => Program.read(parseResult.GetValue(readPathTarg)!));

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
		var idTarg = new Argument<string>("Id"); //TODO: change this to long

		var commCommand = new Command(
				"--comment", 
				"Add comment to observation based on id"
		)
		{
			commTarg,
			idTarg,
		};
		commCommand.Aliases.Add("-c");

		commCommand.SetAction(parseResult =>
			Program.comment(
				parseResult.GetValue(commTarg)!,
				Int64.Parse(parseResult.GetValue(idTarg)!)
			)
		);

		rootCommand.Subcommands.Add(readPathCommand);
		rootCommand.Subcommands.Add(discCommand);
		rootCommand.Subcommands.Add(readCommand);
		rootCommand.Subcommands.Add(obsCommand);
		rootCommand.Subcommands.Add(commCommand);
		rootCommand.Subcommands.Add(locCommand);

		rootCommand.Parse(args).Invoke();
	}
}
