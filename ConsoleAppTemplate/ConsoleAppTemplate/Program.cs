﻿using Abraham.ProgramSettingsManager;
using Abraham.Scheduler;
using NLog.Web;
using CommandLine;

namespace ConsoleAppTemplate;

/// <summary>
/// Console App Template
/// 
/// This is a simple but useful template for console apps, using 
/// - a configuration file (hjson or json)
/// - nlog logger, with daily log rotation
/// - a scheduler that is able to start a method on a regular basis
/// 
/// AUTHOR
/// Written by Oliver Abraham, mail@oliver-abraham.de
/// 
/// INSTALLATION
/// See the README.md
/// 
/// SOURCE CODE
/// https://www.github.com/OliverAbraham/Templates
/// 
/// 
/// </summary>
public class Program
{
    public const string VERSION="2022-10-07";

    #region ------------- Fields --------------------------------------------------------------
    private static CommandLineOptions _commandLineOptions;
    private static ProgramSettingsManager<Configuration> _programSettingsManager;
    private static Configuration _config;
    private static NLog.Logger _logger;
    private static Scheduler _scheduler;
    #endregion



    #region ------------- Command line options ------------------------------------------------
    class CommandLineOptions
    {
        [Option('c', "config", Default = "appsettings.hjson", Required = false, HelpText = 
            """
            Configuration file (full path and filename).
            If you don't specify this option, the program will expect your configuration file 
            named 'appsettings.hjson' in your program folder.
            You can specify a different location.
            You can use Variables for special folders, like %APPDATA%.
            Please refer to the documentation of my nuget package https://github.com/OliverAbraham/Abraham.ProgramSettingsManager
            """)]
        public string ConfigurationFile { get; set; } = "";

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }

    #endregion


    #region ------------- Init ----------------------------------------------------------------
    public static void Main(string[] args)
    {
        ParseCommandLineArguments();
        ReadConfiguration();
        ValidateConfiguration();
        InitLogger();
        PrintGreeting();
        LogConfiguration();
        HealthChecks();
        StartScheduler();

        Run();

        StopScheduler();
    }
    #endregion



    #region ------------- Health checks -------------------------------------------------------
    private static void HealthChecks()
    {
    }
    #endregion



    #region ------------- Configuration -------------------------------------------------------
    private static void ParseCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();
        CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed   <CommandLineOptions>(options => { _commandLineOptions = options; })
            .WithNotParsed<CommandLineOptions>(errors  => { Console.WriteLine(errors.ToString()); });
    }

    private static void ReadConfiguration()
    {
        // ATTENTION: When loading fails, you probably forgot to set the properties of appsettings.hjson to "copy if newer"!
        // ATTENTION: or you have an error in your json file
        _programSettingsManager = new ProgramSettingsManager<Configuration>()
            .UseFullPathAndFilename(_commandLineOptions.ConfigurationFile)
            //.UsePathRelativeToSpecialFolder(_commandLineOptions.ConfigurationFile)
            .Load();
        _config = _programSettingsManager.Data;
        Console.WriteLine($"Loaded configuration file '{_programSettingsManager.ConfigFilename}'");
    }

    private static void ValidateConfiguration()
    {
        // ATTENTION: When validating fails, you missed to enter a value for a property in your json file
        _programSettingsManager.Validate();
    }

    private static void SaveConfiguration()
    {
        _programSettingsManager.Save(_programSettingsManager.Data);
    }
    #endregion



    #region ------------- Logging -------------------------------------------------------------
    private static void InitLogger()
    {
        try
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing our logger. {ex.ToString()}");
            throw;  // ATTENTION: When you come here, you probably forgot to set the properties of nlog.config to "copy if newer"!
        }
    }

    /// <summary>
    /// To generate text like this, use https://onlineasciitools.com/convert-text-to-ascii-art
    /// </summary>
    private static void PrintGreeting()
    {
        _logger.Debug("");
        _logger.Debug("");
        _logger.Debug("");
        _logger.Debug(@"-----------------------------------------------------------------------------------------");
        _logger.Debug(@"   __  __          _____                      _                             			 ");
        _logger.Debug(@"  |  \/  |        / ____|                    | |          /\                			 ");
        _logger.Debug(@"  | \  / |_   _  | |     ___  _ __  ___  ___ | | ___     /  \   _ __  _ __  			 ");
        _logger.Debug(@"  | |\/| | | | | | |    / _ \| '_ \/ __|/ _ \| |/ _ \   / /\ \ | '_ \| '_ \ 			 ");
        _logger.Debug(@"  | |  | | |_| | | |___| (_) | | | \__ \ (_) | |  __/  / ____ \| |_) | |_) |			 ");
        _logger.Debug(@"  |_|  |_|\__, |  \_____\___/|_| |_|___/\___/|_|\___| /_/    \_\ .__/| .__/ 			 ");
        _logger.Debug(@"           __/ |                                               | |   | |    			 ");
        _logger.Debug(@"          |___/                                                |_|   |_|    			 ");
        _logger.Debug(@"                                                                                       	 ");
        _logger.Info ($"MyConsoleApp started, Version {VERSION}                                                  ");
        _logger.Debug(@"-----------------------------------------------------------------------------------------");
    }

    private static void LogConfiguration()
    {
        _logger.Debug($"");
        _logger.Debug($"");
        _logger.Debug($"");
        _logger.Debug($"------------ 0 Configuration -------------------------------------------");
        _logger.Debug($"Loaded from file               : {_programSettingsManager.ConfigFilename}");
        _programSettingsManager.Data.LogOptions(_logger);
        _logger.Debug("");
    }
    #endregion



    #region ------------- Periodic actions ----------------------------------------------------
    private static void StartScheduler()
    {
        // set the interval to 2 seconds
        _scheduler = new Scheduler()
            .UseAction(() => PeriodicJob())
            .UseIntervalSeconds(2)
            .Start();
    }

    private static void StopScheduler()
    {
        _scheduler.Stop();
    }

    private static void PeriodicJob()
    {
        Console.WriteLine($"Scheduler action every 2 seconds!");
    }
    #endregion



    #region ------------- Domain logic --------------------------------------------------------
    private static void Run()
    {
        Console.WriteLine("this is where the music plays");
        Console.WriteLine("Press any key to end the program");
        Console.ReadKey();
    }
    #endregion
}
