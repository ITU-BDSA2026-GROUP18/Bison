using System.CommandLine;

class CLIHandler
{
    public CLIHandler(string[] args)
    {
        RootCommand rootCommand = new("Bison.CLI");

        var readCommand = new Command("--read", 
                    "Prints out entire contents of CSV to the console");

        readCommand.SetAction(parseResult => Program.read());
        readCommand.Aliases.Add("-r");

        var obsTarg = new Argument<string>("Description");
        var obsCommand = new Command("--observe", "Add observation to the CSV database")
        {
            obsTarg
        };
        obsCommand.Aliases.Add("-o");
        
        obsCommand.SetAction(parseResult => Program.observe(parseResult.GetValue(obsTarg)!));

        rootCommand.Subcommands.Add(readCommand);
        rootCommand.Subcommands.Add(obsCommand);
        rootCommand.Parse(args).Invoke();
    }

}