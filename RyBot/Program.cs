using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RyBot.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using RyBot.Helpers;
using Serilog;
using System.Reflection;

namespace RyBot
{
    class Program
    {
        public static IConfigurationRoot config;

        static void Main(string[] args)
        {
            // pass the config into the main program
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            // ###################################
            // Setup dependency injection services
            // ###################################

            var services = GetServiceConfiguration();

            // ########################
            // Setup discord bot client
            // ########################

            // get api token from config file
            var token = config["DiscordBotApiToken"];

            // check for valid token
            if (string.IsNullOrEmpty(token) || token.Length != 70)
            {
                throw new Exception("Error, bot token must be set properly in appsettings.json.");
            }

            // setup a discord bot client
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            // setup command interactivity
            discord.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                Timeout = TimeSpan.FromSeconds(60)
            });

            // setup string prefixes i.e. what the bot will look for when triggering a chat command (.example /example ::example)
            var stringPrefixes = config.GetSection("CommandPrefixes").GetChildren().Select(x => x.Value).ToArray();

            foreach (var stringPrefix in stringPrefixes)
            {
                Console.WriteLine($"Registering string prefix \"{stringPrefix}\".");
            }

            // get command modules
            var commands = discord.UseCommandsNext(
                new CommandsNextConfiguration
                {
                    StringPrefixes = stringPrefixes,
                    Services = services.BuildServiceProvider()
                });

            // register commands
            commands.RegisterCommands<MainModule>();

            Console.WriteLine($"Found {commands.RegisteredCommands.Count} commands to register.");

            foreach (var command in commands.RegisteredCommands)
            {
                Console.WriteLine($"Registering command: {command.Key}");
            }

            var slashCommands = discord.UseSlashCommands();

            slashCommands.RegisterCommands<SlashCommands>(1067228243834445855);

            // register slash commands
            Console.WriteLine($"Found {slashCommands.RegisteredCommands.Count} slash commands to register.");

            // connect to the discord gateway
            await discord.ConnectAsync();

            // keep the thread running
            await Task.Delay(-1);
        }

        private static ServiceCollection GetServiceConfiguration()
        {
            var serviceCollection = new ServiceCollection();

            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            // Build configuration
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(config);

            return serviceCollection;
        }
    }
}
