﻿using Discord.Commands;
using Discord;
using System.Linq;
using System.Threading.Tasks;
using Kotocorn.Core.Services;
using System.Collections.Generic;
using Kotocorn.Core.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using Kotocorn.Common.Attributes;
using Kotocorn.Extensions;
using Kotocorn.Modules.Searches.Services;
using Kotocorn.Modules.Searches.Common;
using System.Text.RegularExpressions;
using System;
using Discord.WebSocket;

namespace Kotocorn.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class StreamNotificationCommands : KotocornSubmodule<StreamNotificationService>
        {
            private readonly DbService _db;

            public StreamNotificationCommands(DbService db)
            {
                _db = db;
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public Task Smashcast([Remainder] string username) =>
                smashcastRegex.IsMatch(username)
                ? StreamAdd(username)
                : TrackStream((ITextChannel)Context.Channel,
                    username,
                    FollowedStream.FType.Smashcast);

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public Task Twitch([Remainder] string username) =>
                twitchRegex.IsMatch(username)
                ? StreamAdd(username)
                : TrackStream((ITextChannel)Context.Channel,
                    username,
                    FollowedStream.FType.Twitch);

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public Task Picarto([Remainder] string username) =>
                picartoRegex.IsMatch(username)
                ? StreamAdd(username)
                : TrackStream((ITextChannel)Context.Channel,
                    username,
                    FollowedStream.FType.Picarto);

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public Task Mixer([Remainder] string username) =>
                mixerRegex.IsMatch(username)
                ? StreamAdd(username)
                : TrackStream((ITextChannel)Context.Channel,
                    username,
                    FollowedStream.FType.Mixer);

            private static readonly Regex twitchRegex = new Regex(@"twitch.tv/(?<name>.+)/?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static readonly Regex mixerRegex = new Regex(@"mixer.com/(?<name>.+)/?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static readonly Regex smashcastRegex = new Regex(@"smashcast.tv/(?<name>.+)/?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static readonly Regex picartoRegex = new Regex(@"picarto.tv/(?<name>.+)/?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            private static readonly Dictionary<FollowedStream.FType, Regex> typesWithRegex = new Dictionary<FollowedStream.FType, Regex>()
            {
                { FollowedStream.FType.Mixer, mixerRegex },
                { FollowedStream.FType.Picarto, picartoRegex },
                { FollowedStream.FType.Smashcast, smashcastRegex },
                { FollowedStream.FType.Twitch, twitchRegex },
            };

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task StreamAdd(string link)
            {
                var streamRegexes = new(Func<string, Task> Func, Regex Regex)[]
                {
                    (Twitch, twitchRegex),
                    (Mixer, mixerRegex),
                    (Smashcast, smashcastRegex),
                    (Picarto, picartoRegex)
                };

                foreach (var s in streamRegexes)
                {
                    var m = s.Regex.Match(link);
                    if (m.Captures.Count != 0)
                    {
                        await s.Func(m.Groups["name"].ToString());
                        return;
                    }
                }

                await ReplyErrorLocalized("stream_not_exist").ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task StreamRemove(string link)
            {
                var streamRegexes = new(Func<string, Task> Func, Regex Regex)[]
                {
                    ((u) => StreamRemove(FollowedStream.FType.Twitch, u), twitchRegex),
                    ((u) => StreamRemove(FollowedStream.FType.Mixer, u), mixerRegex),
                    ((u) => StreamRemove(FollowedStream.FType.Smashcast, u), smashcastRegex),
                    ((u) => StreamRemove(FollowedStream.FType.Picarto, u), picartoRegex),
                };

                foreach (var s in streamRegexes)
                {
                    var m = s.Regex.Match(link);
                    if (m.Captures.Count != 0)
                    {
                        await s.Func(m.Groups["name"].ToString());
                        return;
                    }
                }

                await ReplyErrorLocalized("stream_not_exist").ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task ListStreams()
            {
                IEnumerable<FollowedStream> streams;
                using (var uow = _db.UnitOfWork)
                {
                    var all = uow.GuildConfigs
                                 .For(Context.Guild.Id,
                                      set => set.Include(gc => gc.FollowedStreams))
                                 .FollowedStreams;

                    var toRemove = all.Where(x => ((SocketGuild)Context.Guild).GetTextChannel(x.ChannelId) == null);
                    streams = all.Except(toRemove);
                    if (toRemove.Any())
                    {
                        foreach (var r in toRemove)
                        {
                            _service.UntrackStream(r);
                        }
                        uow._context.RemoveRange(toRemove);
                        uow.Complete();
                    }
                }

                if (!streams.Any())
                {
                    await ReplyErrorLocalized("streams_none").ConfigureAwait(false);
                    return;
                }

                var text = string.Join("\n", await Task.WhenAll(streams.Select(async snc =>
                {
                    var ch = await Context.Guild.GetTextChannelAsync(snc.ChannelId);
                    return string.Format("{0}'s stream on {1} channel. 【{2}】",
                        Format.Code(snc.Username),
                        Format.Bold(ch?.Name ?? "deleted-channel"),
                        Format.Code(snc.Type.ToString()));
                })));

                await Context.Channel.SendConfirmAsync(GetText("streams_following", streams.Count()) + "\n\n" + text)
                    .ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task StreamOffline()
            {
                var newValue = _service.ToggleStreamOffline(Context.Guild.Id);
                if (newValue)
                {
                    await ReplyConfirmLocalized("stream_off_enabled").ConfigureAwait(false);
                }
                else
                {
                    await ReplyConfirmLocalized("stream_off_disabled").ConfigureAwait(false);
                }
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task StreamMessage(string url, [Remainder] string message)
            {
                if (!GetNameAndType(url, out var info))
                {
                    await ReplyErrorLocalized("stream_not_exist").ConfigureAwait(false);
                    return;
                }
                if(!_service.SetStreamMessage(Context.Guild.Id, info.Value.Item1, info.Value.Item2, message))
                {
                    await ReplyConfirmLocalized("stream_not_following").ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    await ReplyConfirmLocalized("stream_message_reset", url).ConfigureAwait(false);
                }
                else
                {
                    await ReplyConfirmLocalized("stream_message_set", url).ConfigureAwait(false);
                }
            }
            //todo default message

            private bool GetNameAndType(string url, out (string, FollowedStream.FType)? nameAndType)
            {
                nameAndType = null;
                foreach (var kvp in typesWithRegex)
                {
                    var m = kvp.Value.Match(url);
                    if (m.Captures.Count > 0)
                    {
                        nameAndType = (m.Groups["name"].ToString(), kvp.Key);
                        return true;
                    }
                }
                return false;
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task StreamRemove(FollowedStream.FType type, [Remainder] string username)
            {
                username = username.ToLowerInvariant().Trim();

                var fs = new FollowedStream()
                {
                    GuildId = Context.Guild.Id,
                    ChannelId = Context.Channel.Id,
                    Username = username,
                    Type = type
                };

                bool removed;
                using (var uow = _db.UnitOfWork)
                {
                    var config = uow.GuildConfigs.For(Context.Guild.Id, set => set.Include(gc => gc.FollowedStreams));
                    removed = config.FollowedStreams.Remove(fs);
                    if (removed)
                        await uow.CompleteAsync().ConfigureAwait(false);
                }
                _service.UntrackStream(fs);
                if (!removed)
                {
                    await ReplyErrorLocalized("stream_no").ConfigureAwait(false);
                    return;
                }

                await ReplyConfirmLocalized("stream_removed",
                    Format.Code(username),
                    type).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task CheckStream(FollowedStream.FType platform, [Remainder] string username)
            {
                var stream = username?.Trim();
                if (string.IsNullOrWhiteSpace(stream))
                    return;
                try
                {
                    var streamStatus = await _service.GetStreamStatus(platform, username);
                    if (streamStatus == null)
                    {
                        await ReplyErrorLocalized("no_channel_found").ConfigureAwait(false);
                        return;
                    }
                    if (streamStatus.Live)
                    {
                        await ReplyConfirmLocalized("streamer_online",
                                Format.Bold(username),
                                Format.Bold(streamStatus.Viewers.ToString()))
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await ReplyConfirmLocalized("streamer_offline",
                            username).ConfigureAwait(false);
                    }
                }
                catch
                {
                    await ReplyErrorLocalized("no_channel_found").ConfigureAwait(false);
                }
            }

            private async Task TrackStream(ITextChannel channel, string username, FollowedStream.FType type)
            {
                username = username.ToLowerInvariant().Trim();
                var fs = new FollowedStream
                {
                    GuildId = channel.Guild.Id,
                    ChannelId = channel.Id,
                    Username = username,
                    Type = type,
                };

                IStreamResponse status;
                try
                {
                    status = await _service.GetStreamStatus(fs.Type, fs.Username).ConfigureAwait(false);
                }
                catch
                {
                    await ReplyErrorLocalized("stream_not_exist").ConfigureAwait(false);
                    return;
                }

                if (status == null)
                {
                    await ReplyErrorLocalized("stream_not_exist").ConfigureAwait(false);
                    return;
                }

                using (var uow = _db.UnitOfWork)
                {
                    uow.GuildConfigs.For(channel.Guild.Id, set => set.Include(gc => gc.FollowedStreams))
                                    .FollowedStreams
                                    .Add(fs);
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                _service.TrackStream(fs);
                await channel.EmbedAsync(_service.GetEmbed(fs, status, Context.Guild.Id), GetText("stream_tracked")).ConfigureAwait(false);
            }
        }
    }
}
