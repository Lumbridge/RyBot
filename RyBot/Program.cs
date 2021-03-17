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
using RyBot.Helpers;
using Serilog;

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
            if (string.IsNullOrEmpty(token) || token.Length != 59)
            {
                throw new Exception("Error, bot token must be set properly in appsettings.json.");
            }

            // setup a discord bot client
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            // easter egg chat responses
            discord.MessageCreated += async (s, e) => await BotChatResponses(s, e);

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
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration { StringPrefixes = stringPrefixes, Services = services.BuildServiceProvider() });

            // register commands
            commands.RegisterCommands<MainModule>();

            Console.WriteLine($"Found {commands.RegisteredCommands.Count} commands to register.");

            foreach (var command in commands.RegisteredCommands)
            {
                Console.WriteLine($"Registering command: {command.Key}");
            }

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

        private static async Task BotChatResponses(DiscordClient client, MessageCreateEventArgs e)
        {
            ulong tesaId = ulong.Parse(config.GetSection("TesaId").Value);
            ulong botId = ulong.Parse(config.GetSection("BotId").Value);

            // tesa id: 282389799711539201
            // ryan id: 142350124964773888
            // bot id: 819629279419564043
            
            if (e.Author.Id != botId)
            {
                if (e.Author.Id == tesaId)
                {
                    Console.WriteLine("tesa sent a message rolling for response...");
                    RandomHelper.OneInNChance(1, 500, async () => {
                        Console.WriteLine("Rolled a 1/100! responding to tesa! <3");
                        var responses = config.GetSection("TesaResponses").GetChildren().Select(x => x.Value).ToList();
                        var response = responses[RandomHelper.RandomNumber(0, responses.Count - 1)];
                        await e.Message.RespondAsync(new DiscordMessageBuilder().WithReply(e.Message.Id, true).WithContent(response));
                    }, null);
                }
                
                var messageContent = e.Message.Content.ToLowerInvariant();

                // someone @ing the bot
                if (e.Message.MentionedUsers.Any(x => x.Id == 819629279419564043))
                {
                    var insults = config.GetSection("CommonBotInsults").GetChildren().Select(x => x.Value).ToList();
                    foreach (var insult in insults)
                    {
                        if (messageContent.Contains(insult))
                        {
                            var responses = config.GetSection("BotInsultResponses").GetChildren().Select(x => x.Value).ToList();
                            var response = responses[RandomHelper.RandomNumber(0, responses.Count - 1)];
                            await e.Message.RespondAsync(new DiscordMessageBuilder().WithReply(e.Message.Id, true).WithContent(response));
                        }
                    }
                }
                else
                {
                    if (messageContent.Contains("xd"))
                    {
                        RandomHelper.OneInNChance(1, 3, async () => {
                            await e.Message.RespondAsync(new DiscordMessageBuilder().WithReply(e.Message.Id, true).WithContent("😆"));
                        }, null);
                    }
                    else if (messageContent.Contains("@met"))
                    {
                        RandomHelper.OneInNChance(1, 3, async () => {
                            await e.Message.RespondAsync(new DiscordMessageBuilder().WithContent("@met 😢"));
                        }, null);
                    }
                    else
                    {
                        RandomHelper.OneInNChance(1, 1000, async () => {
                            var responses = config.GetSection("RandomResponses").GetChildren().Select(x => x.Value).ToList();
                            var response = responses[RandomHelper.RandomNumber(0, responses.Count - 1)];
                            await e.Message.RespondAsync(new DiscordMessageBuilder().WithReply(e.Message.Id, true).WithContent(response));
                        }, null);
                    }
                }

                RandomHelper.OneInNChance(1, 1000000, async () => {
                    await e.Message.RespondAsync(new DiscordMessageBuilder().WithReply(e.Message.Id, true).WithContent("You just rolled 1 in a million (no meme) :O"));
                }, null);
            }
        }
    }
}
