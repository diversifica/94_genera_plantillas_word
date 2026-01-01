using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using TemplateGen.Core.Services;

namespace TemplateGen.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var profileOption = new Option<FileInfo>(
            name: "--profile",
            description: "Path to the JSON profile file.")
        {
            IsRequired = true
        };

        var outputOption = new Option<DirectoryInfo>(
            name: "--output",
            description: "Directory for output artifacts.")
        {
            IsRequired = false
        };

        var strictOption = new Option<bool>(
            name: "--strict",
            description: "Enable strict mode (fail on warnings).",
            getDefaultValue: () => false);

        var rootCommand = new RootCommand("TemplateGen - OpenXML Word Template Generator")
        {
            profileOption,
            outputOption,
            strictOption
        };

        rootCommand.SetHandler(async (profileFile, outputDir, strict) =>
        {
            await RunAsync(profileFile, outputDir, strict);
        }, profileOption, outputOption, strictOption);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task RunAsync(FileInfo profileFile, DirectoryInfo? outputDir, bool strict)
    {
        try
        {
            Console.WriteLine($"TemplateGen (Phase 1)");
            Console.WriteLine($"Loading profile: {profileFile.FullName}");

            var loader = new ProfileLoader();
            var profile = await loader.LoadAsync(profileFile.FullName);

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Profile Loaded Successfully");
            Console.WriteLine($"Schema Version : {profile.SchemaVersion}");
            Console.WriteLine($"Client ID      : {profile.Metadata.ClientId}");
            Console.WriteLine($"Client Name    : {profile.Metadata.ClientName}");
            Console.WriteLine($"Version        : {profile.Metadata.ProfileVersion}");
            Console.WriteLine("--------------------------------------------------");

            if (outputDir != null)
            {
                if (!outputDir.Exists) outputDir.Create();
                Console.WriteLine($"Output directory prepared: {outputDir.FullName}");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }
}
