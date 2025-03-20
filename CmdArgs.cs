using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

public class CmdArgs
{
    public static CmdArgs? Instance { get; set; }

    // Explicitly add a parameterless constructor for the parser
    public CmdArgs() { }

    [Option('f', "file", Required = false, HelpText = "Input File to use, usualy main.masm or some *.masm file")]
    public string? FileName { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Prints all messages to standard output. ")]
    public bool Verbose { get; set; }

    [Option('V', "veryverbose", Required = false, HelpText = "Prints all messages to standard output. including internal debugging infomation that may or may not swear at you lol ")]
    public bool VeryVerbose { get; set; }

    [Option('s', "start-server", Required = false, HelpText = "Starts the web server that hosts our documentation because i (charlie) am lazy to update the website after every new beta release")]
    public bool StartServer { get; set; }

    [Option('a', "use-arrays", Required = false, HelpText = "Uses built-in language arrays instead of memory-mapped files for memory operations. May improve performance or compatibility on some systems.")]
    public bool UseBuiltInArrays { get; set; }

    // since filename is not required, most of the time people will just ./masm.exe main.masm
    // so we will just set the filename to main.masm if it is not set
    public static CmdArgs Parse(string[] args)
    {
        var result = new CmdArgs();
        
        // Handle the case where the first argument might be the filename without a flag
        if (args.Length == 1 && !args[0].StartsWith("-"))
        {
            result.FileName = args[0];
            return result;
        }
        else if (args.Length == 0)
        {
            result.FileName = "main.masm";
            return result;
        }
        
        // Use the parser result directly
        Parser.Default.ParseArguments<CmdArgs>(args)
            .WithParsed(parsed => {
                result = parsed;
                // If filename is still null after parsing, set default
                if (string.IsNullOrEmpty(result.FileName))
                {
                    result.FileName = "main.masm";
                }
            })
            .WithNotParsed(_ => {
                // Keep default values on parsing error
                result.FileName = "main.masm";
            });
            
        return result;
    }

    public static CmdArgs GetInstance()
    {
        if (Instance == null)
        {
            Instance = new CmdArgs();
        }
        return Instance;
    }
}
