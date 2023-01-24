using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace RyBot.Commands
{
    class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("test", "Does something")]
        public async Task test(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Success!"));
        }
    }
}
