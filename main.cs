using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;

internal class main
{
    private CancellationTokenSource _cts {
        get; set;
    }

    private IConfigurationRoot _config;

    private DiscordClient _discord;
    private CommandsNextModule _commands;
    private InteractivityModule _interactivity;

    static async Task Main (string[] args) => await new main().InitBot(args)
    async Task InitBot (string[] args) {
        try
        {
            Console.WriteLine("[info] Welcome to my bot!")
            _cts = new CancellationTokenSource();

            //load the config file
            Console.WriteLine("[info] Loading config file...")
            _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(config.json, optional: false, reloadOnChange: true)
            .Build();

            //create the DShapPlus client
            Console.WriteLine("[info] Creating discord client...")
            _discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config.GetValue < string > ("discord:token"),
                    TokenType = TokenType.bot
                });

            //create the interactivity module
            _interactivity = _discord.UseInteractivity(new InteractivityConfiguration() {
                PaginationBehaviour = TimeoutBehaviour.Delete, //what to do when pagination request times out
                PaginationTimeout = TimeSpan.FromSeconds(30), //how long to wait before timing out
                Timeout = TimeSpan.FromSeconds(30) //default time to wait for interactive commands
            });

            //build dependencies and create the commands module
            var deps = BuildDeps();
            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefix = _config.GetValue < string > ("discord:CommandPrefix") //load the bot's command prefix
                    Dependencies = deps //pass the dependencies
                });

            // TODO: Add command Loading

            RunAsync(args).Wait();
        }
        catch (Exception ex) {
            //this will catch any exceptions that occur during the operation/setup of the bot

            Console.Error.WriteLine(ex.ToString());
        }
    }

    async Task RunAsync(string[] args) {
        //connect to Discord's service
        Console.WriteLine("Connecting..")
        await _discord.ConnectAsync();
        Console.WriteLine("Connected!")

        //keep the bot running until the cancelation token requests we stop
        while (!_cts.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromMinutes(1));
    }
}