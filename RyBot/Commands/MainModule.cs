using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.LanguageTranslator.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RyBot.Helpers;
using RyBot.Models;

namespace RyBot.Commands
{
    class MainModule : BaseCommandModule
    {
        public IConfigurationRoot config { private get; set; }
        public ILogger<MainModule> logger { private get; set; }

        [Command("urban"), Description("Search for a term on urban dictionary.")]
        public async Task UrbanCommand(CommandContext ctx, [RemainingText][Description("The term you want to search for.")] string term)
        {
            var baseUrl = config["UrbanDictionaryApiEndpoint"];

            var url = $"{baseUrl}{WebUtility.UrlEncode(term)}";

            var results = await JsonHelper.GetJsonData<UrbanDictionaryModel>(url);

            var pages = new List<Page>();
            for (var i = 0; i < results.UrbanResultList.Count; i++)
            {
                var result = results.UrbanResultList[i];
                pages.Add(new Page("",
                    new DiscordEmbedBuilder(new DiscordEmbedBuilder())
                        .WithDescription(result.Definition)
                        .WithFooter($"Page {i + 1}/{results.UrbanResultList.Count} | Likes: {result.ThumbsUp} | Dislikes: {result.ThumbsDown} | Posted on: {result.WrittenOn:g}")));
            }

            if (results.UrbanResultList.Count == 0)
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = $"No results found for \"{term}\"."
                    })
                    .SendAsync(ctx.Channel);
            }
            else
            {
                await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
            }
        }

        [Command("avatar"), Aliases("myavatar", "myav"), Description("View your personal avatar.")]
        public async Task AvatarCommand(CommandContext ctx)
        {
            var user = ctx.Member;
            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder
                {
                    Title = $"{user.Username}#{user.Discriminator}'s Avatar",
                    Description = $"`ID: {user.Id}`",
                    ImageUrl = user.AvatarUrl
                })
                .SendAsync(ctx.Channel);
        }

        [Command("avatar"), Aliases("av"), Description("View the mentioned user's avatar.")]
        public async Task AvatarCommand(CommandContext ctx, [Description("The user you want to see the avatar for.")] DiscordUser user)
        {
            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder
                {
                    Title = $"{user.Username}#{user.Discriminator}'s Avatar",
                    Description = $"`ID: {user.Id}`",
                    ImageUrl = user.AvatarUrl
                })
                .SendAsync(ctx.Channel);
        }

        [Command("image"), Aliases("img"), Description("Searches for images matching the term entered.")]
        public async Task ImageCommand(CommandContext ctx, [RemainingText][Description("The image you want to search for.")] string term)
        {
            var apiKey = config["GoogleCustomSearchApiKey"];
            var contextCode = config["GoogleCustomSearchEngineContextCode"];
            
            var imageSearchResult = await ScrapeHelper.GetImageSearchResult(term, apiKey, contextCode);
            var imageUrls = imageSearchResult.SearchResults;
            var searchTitle = imageSearchResult.Queries.Request.First().Title;
            var counter = imageSearchResult.SearchResults.Count;
            while (counter < 30 && counter % 10 == 0)
            {
                imageSearchResult = await ScrapeHelper.GetImageSearchResult(term, apiKey, contextCode, counter + 1);
                if (imageSearchResult.SearchResults.Count > 0) {
                    imageUrls.AddRange(imageSearchResult.SearchResults);
                }
                counter += imageSearchResult.SearchResults.Count;
            }

            if (imageUrls == null)
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = "An error occurred when performing image search."
                    })
                    .SendAsync(ctx.Channel);
            }
            else
            {
                var pages = new List<Page>();
                for (var i = 0; i < imageUrls.Count; i++)
                {
                    var result = imageUrls[i];
                    pages.Add(new Page("",
                        new DiscordEmbedBuilder(new DiscordEmbedBuilder())
                            .WithTitle(searchTitle)
                            .WithImageUrl(result.Link)
                            .WithFooter($"Page {i + 1}/{imageUrls.Count}")));
                }

                if (imageUrls.Count == 0)
                {
                    await new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder
                        {
                            Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                            Title = $"No results found for \"{term}\"."
                        })
                        .SendAsync(ctx.Channel);
                }
                else
                {
                    await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
                }
            }
        }

        [Command("translate"), Aliases("tr"), Description("Translate a term from one language to another")]
        public async Task TranslateCommand(CommandContext ctx, [Description("The language code that you want to translate the term to.")] string languageCode, [RemainingText][Description("The term to translate.")] string termToTranslate)
        {
            var apiUrl = config["IbmWatsonApiUrl"];
            var apiKey = config["IbmWatsonApiKey"];

            var authenticator = new IamAuthenticator(apiKey);

            var languageTranslator = new LanguageTranslatorService("2018-05-01", authenticator);
            languageTranslator.SetServiceUrl(apiUrl);

            var identifyResult = languageTranslator.Identify(termToTranslate);

            if (identifyResult.Result.Languages.Count == 0)
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = $"Error: Unable to determine source language."
                    })
                    .SendAsync(ctx.Channel);
            }

            var translationResult = languageTranslator.Translate(
                text: new List<string> { termToTranslate },
                source: identifyResult.Result.Languages.First().Language,
                target: languageCode
            );

            if (translationResult.Result.Translations.Count == 0)
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = $"Error: Unable to translate the requested term from {identifyResult.Result.Languages.First().Language} -> {languageCode}."
                    })
                    .SendAsync(ctx.Channel);
            }

            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder
                {
                    Color = new Optional<DiscordColor>(new DiscordColor(0, 255, 0)),
                    Title = $"Translation Result ({identifyResult.Result.Languages.First().Language} -> {languageCode})",
                    Description = $"{termToTranslate} -> {translationResult.Result.Translations.First()._Translation}"
                })
                .SendAsync(ctx.Channel);
        }

        [Command("source"), Aliases("sauce", "src"), Description("Searches for anime that a screenshot of a scene is from.")]
        public async Task SourceCommand(CommandContext ctx, [Description("Image URL to use in the anime search.")] string imageUrl)
        {
            try
            {
                var url = $"https://trace.moe/api/search?url={WebUtility.UrlEncode(imageUrl)}";

                var results = await JsonHelper.GetJsonData<TraceMoeApiResponseModel>(url);

                var result = results.Docs[0];
                var anilist_id = result.AnilistId;
                var filename = result.Filename;
                var at = result.At;
                var tokenthumb = result.Tokenthumb;
                
                var previewUrl = $"https://media.trace.moe/video/{anilist_id}/{Uri.EscapeUriString(filename)}?t={at}&token={tokenthumb}";

                var preview = await new HttpClient().GetStreamAsync(previewUrl);

                await new DiscordMessageBuilder()
                    .WithFile("preview_" + filename, preview)
                    .WithEmbed(new DiscordEmbedBuilder()
                        .WithColor(new DiscordColor(0, 255, 0))
                        .WithTitle("Sauce Result")
                        .WithUrl(previewUrl)
                        .WithDescription($"English Title: {result.TitleEnglish}\n" +
                                         $"Season: {result.Season}\n" +
                                         $"Episode: {result.Episode}\n" +
                                         $"Similarity: {result.Similarity:P}\n" +
                                         $"MAL ID: {result.MalId}\n"))
                    .SendAsync(ctx.Channel);
            }
            catch
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = "No results found or image recognition service unavailable."
                    })
                    .SendAsync(ctx.Channel);
            }
        }

        [Command("enlarge"), Aliases("e"), Description("Enlarges an emote")]
        public async Task EnlargeCommand(CommandContext ctx, [RemainingText] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder
                    {
                        Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                        Title = $"Error: No emotes found (usage: .e :emote:)."
                    })
                    .SendAsync(ctx.Channel);
            }
            else
            {
                //<:emotename:emote id>
                //https://cdn.discordapp.com/emojis/782662653931683850.png

                var emotes = StringHelper.ParseEmotesFromMessage(message);

                if (emotes.Count == 0)
                {
                    await new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder
                        {
                            Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0)),
                            Title = $"Error: No emotes found (usage: .e :emote:)."
                        })
                        .SendAsync(ctx.Channel);
                }

                if (emotes.Count == 1)
                {
                    await ctx.Channel.SendMessageAsync(
                        new DiscordEmbedBuilder(new DiscordEmbedBuilder())
                            .WithTitle(emotes[0].Item1)
                            .WithImageUrl(emotes[0].Item2));
                }
                else
                {
                    var pages = new List<Page>();
                    for (var i = 0; i < emotes.Count; i++)
                    {
                        pages.Add(new Page("",
                            new DiscordEmbedBuilder(new DiscordEmbedBuilder())
                                .WithTitle(emotes[i].Item1)
                                .WithImageUrl(emotes[i].Item2)
                                .WithFooter($"Page {i + 1}/{emotes.Count}")));
                    }

                    await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
                }
            }
        }

        [Command("ping"), Aliases("p"), Description("Gets delay in ms between the user and the bot.")]
        public async Task PingCommand(CommandContext ctx)
        {
            var then = ctx.Message.CreationTimestamp.TimeOfDay;
            var now = DateTime.UtcNow.TimeOfDay;
            var pingTime = (int)now.Subtract(then).TotalMilliseconds;
            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder
                {
                    Title = $"...pong ({pingTime}ms)",
                    Color = new Optional<DiscordColor>(new DiscordColor(0, 255, 0))
                })
                .SendAsync(ctx.Channel);
        }

        [Command("version"), Aliases("v"), Description("Returns version information for the bot.")]
        public async Task VersionCommand(CommandContext ctx)
        {
            await new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder
                {
                    Color = new Optional<DiscordColor>(new DiscordColor(0, 255, 0)),
                    Title = $"This shard is running RyBot Version {GetType().Assembly.GetName().Version}."
                })
                .SendAsync(ctx.Channel);
        }
    }
}
