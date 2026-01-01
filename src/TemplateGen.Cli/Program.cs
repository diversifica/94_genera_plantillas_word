using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using TemplateGen.Core.Services;
using TemplateGen.Core.Models;


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

        var contentOption = new Option<FileInfo?>(
            name: "--content",
            description: "Path to the JSON content file.")
        {
            IsRequired = false
        };

        var rootCommand = new RootCommand("TemplateGen - OpenXML Word Template Generator")
        {
            profileOption,
            outputOption,
            strictOption,
            contentOption
        };

        rootCommand.SetHandler(async (profileFile, outputDir, strict, contentFile) =>
        {
            await RunAsync(profileFile, outputDir, strict, contentFile);
        }, profileOption, outputOption, strictOption, contentOption);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task RunAsync(FileInfo profileFile, DirectoryInfo? outputDir, bool strict, FileInfo? contentFile)
    {
        // Configure logging
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var wordGenLogger = loggerFactory.CreateLogger<WordGeneratorService>();
        var contentGenLogger = loggerFactory.CreateLogger<ContentGeneratorService>();

        try
        {
            Console.WriteLine($"TemplateGen (Phase 8)");
            Console.WriteLine($"Loading profile: {profileFile.FullName}");

            // Locate schema (POC: assumes running from repo root or adjacent folders)
            // Ideally we'd bundle this or allow configuration.
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas", "template-profile.schema.json");
            // Fix for running via dotnet run from root
            if (!File.Exists(schemaPath))
            {
                var repoRootSchema = Path.Combine(Directory.GetCurrentDirectory(), "schemas", "template-profile.schema.json");
                if (File.Exists(repoRootSchema)) schemaPath = repoRootSchema;
            }

            SchemaValidator? validator = null;
            if (File.Exists(schemaPath))
            {
               Console.WriteLine($"Schema found: {schemaPath}");
               validator = new SchemaValidator(schemaPath);
            }
            else
            {
                Console.WriteLine("WARNING: Schema file not found. Validation skipped.");
            }

            var loader = new ProfileLoader(validator);
            var (profile, validationErrors) = await loader.LoadWithValidationAsync(profileFile.FullName);
            
            var hasErrors = validationErrors.Any();

            if (hasErrors)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Validation Issues Found:");
                foreach (var error in validationErrors)
                {
                    Console.WriteLine($" - [{error.Severity}] {error.JsonPath}: {error.Message}");
                }
                Console.ResetColor();
            }

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Profile Loaded Successfully");
            Console.WriteLine($"Schema Version : {profile.SchemaVersion}");
            Console.WriteLine($"Client ID      : {profile.Metadata.ClientId}");
            Console.WriteLine($"Client Name    : {profile.Metadata.ClientName}");
            Console.WriteLine($"Version        : {profile.Metadata.ProfileVersion}");
            Console.WriteLine("--------------------------------------------------");

            DocumentContent? content = null;
            if (contentFile != null && contentFile.Exists)
            {
                 Console.WriteLine($"Loading content: {contentFile.FullName}");
                 // Simple deserialization for MVP
                 var contentJson = await File.ReadAllTextAsync(contentFile.FullName);
                 content = System.Text.Json.JsonSerializer.Deserialize<DocumentContent>(contentJson);
            }

            if (outputDir != null)
            {
                if (!outputDir.Exists) outputDir.Create();
                Console.WriteLine($"Output directory prepared: {outputDir.FullName}");
                
                // Generate report.json
                var reportPath = Path.Combine(outputDir.FullName, "report.json");
                var report = new 
                {
                    GeneratedAt = DateTime.UtcNow,
                    Profile = profileFile.Name,
                    IsValid = !hasErrors,
                    ValidationErrors = validationErrors
                };
                
                await File.WriteAllTextAsync(reportPath, System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine($"Report generated: {reportPath}");

                if (!hasErrors)
                {
                    Console.WriteLine("Generating Word Template...");
                    var generator = new WordGeneratorService();
                    var docPath = Path.Combine(outputDir.FullName, "GeneratedTemplate.dotx");
                    generator.Generate(profile, docPath, content);
                    Console.WriteLine($"Template created successfully: {docPath}");
                }
            }

            if (strict && hasErrors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Strict mode enabled: Exiting due to validation errors.");
                Console.ResetColor();
                Environment.Exit(1);
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
