using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

public class CmdArgs
{
    public static CmdArgs? Instance { get; set; }

    [Option('f', "file", Required = false, HelpText = "Input File to use, usualy main.masm or some *.masm file")]
    public string? FileName { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Prints all messages to standard output. ")]
    public bool Verbose { get; set; }

    [Option('V', "veryverbose", Required = false, HelpText = "Prints all messages to standard output. including internal debugging infomation that may or may not swear at you lol ")]
    public bool VeryVerbose { get; set; }

    [Option('s', "start-server", Required = false, HelpText = "Starts the ASP.NET web server that hosts our documentation because i (charlie) am lazy to update the website after every new beta release")]
    public bool StartServer { get; set; }

    // since filename is not required, most of the time people will just ./masm.exe main.masm
    // so we will just set the filename to main.masm if it is not set
    public static CmdArgs Parse(string[] args)
    {
        var cmdArgs = new CmdArgs();
        if (args.Length == 0)
        {
            cmdArgs.FileName = "main.masm";
        }
        else
        {
            Parser.Default.ParseArguments<CmdArgs>(args)
                .WithParsed(parsedArgs =>
                {
                    cmdArgs = parsedArgs;
                });
        }
        return cmdArgs;
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
