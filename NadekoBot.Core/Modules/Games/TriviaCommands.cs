﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kotocorn.Extensions;
using Kotocorn.Core.Services;
using System.Threading.Tasks;
using Kotocorn.Common.Attributes;
using Kotocorn.Modules.Games.Common.Trivia;
using Kotocorn.Modules.Games.Services;
using Kotocorn.Core.Common;
using Kotocorn.Core.Modules.Games.Common.Trivia;

namespace Kotocorn.Modules.Games
{
    public partial class Games
    {
        [Group]
        public class TriviaCommands : KotocornSubmodule<GamesService>
        {
            private readonly IDataCache _cache;
            private readonly ICurrencyService _cs;
            private readonly DiscordSocketClient _client;

            public TriviaCommands(DiscordSocketClient client, IDataCache cache, ICurrencyService cs)
            {
                _cache = cache;
                _cs = cs;
                _client = client;
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [Priority(0)]
            [KotocornOptions(typeof(TriviaOptions))]
            public Task Trivia(params string[] args)
                => InternalTrivia(args);

            public async Task InternalTrivia(params string[] args)
            {
                var channel = (ITextChannel)Context.Channel;

                var (opts, _) = OptionsParser.Default.ParseFrom(new TriviaOptions(), args);

                if (_bc.BotConfig.MinimumTriviaWinReq > 0 && _bc.BotConfig.MinimumTriviaWinReq > opts.WinRequirement)
                {
                    return;
                }
                var trivia = new TriviaGame(_strings, _client, _bc, _cache, _cs, channel.Guild, channel, opts);
                if (_service.RunningTrivias.TryAdd(channel.Guild.Id, trivia))
                {
                    try
                    {
                        await trivia.StartGame().ConfigureAwait(false);
                    }
                    finally
                    {
                        _service.RunningTrivias.TryRemove(channel.Guild.Id, out trivia);
                        await trivia.EnsureStopped().ConfigureAwait(false);
                    }
                    return;
                }

                await Context.Channel.SendErrorAsync(GetText("trivia_already_running") + "\n" + trivia.CurrentQuestion)
                    .ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Tl()
            {
                var channel = (ITextChannel)Context.Channel;

                if (_service.RunningTrivias.TryGetValue(channel.Guild.Id, out TriviaGame trivia))
                {
                    await channel.SendConfirmAsync(GetText("leaderboard"), trivia.GetLeaderboard()).ConfigureAwait(false);
                    return;
                }

                await ReplyErrorLocalized("trivia_none").ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Tq()
            {
                var channel = (ITextChannel)Context.Channel;

                if (_service.RunningTrivias.TryGetValue(channel.Guild.Id, out TriviaGame trivia))
                {
                    await trivia.StopGame().ConfigureAwait(false);
                    return;
                }

                await ReplyErrorLocalized("trivia_none").ConfigureAwait(false);
            }
        }
    }
}