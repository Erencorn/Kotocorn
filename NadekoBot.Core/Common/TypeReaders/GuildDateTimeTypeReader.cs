﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Kotocorn.Modules.Administration.Services;
using Kotocorn.Core.Common.TypeReaders;
using Discord.WebSocket;

namespace Kotocorn.Common.TypeReaders
{
    public class GuildDateTimeTypeReader : KotocornTypeReader<GuildDateTime>
    {
        public GuildDateTimeTypeReader(DiscordSocketClient client, CommandService cmds) : base(client, cmds)
        {
        }

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var gdt = Parse(services, context.Guild.Id, input);
            if(gdt == null)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input string is in an incorrect format."));

            return Task.FromResult(TypeReaderResult.FromSuccess(gdt));
        }

        public static GuildDateTime Parse(IServiceProvider services, ulong guildId, string input)
        {
            var _gts = (GuildTimezoneService)services.GetService(typeof(GuildTimezoneService));
            if (!DateTime.TryParse(input, out var dt))
                return null;

            var tz = _gts.GetTimeZoneOrUtc(guildId);

            return new GuildDateTime(tz, dt);
        }
    }

    public class GuildDateTime
    {
        public TimeZoneInfo Timezone { get; }
        public DateTime CurrentGuildTime { get; }
        public DateTime InputTime { get; }
        public DateTime InputTimeUtc { get; }

        private GuildDateTime() { }

        public GuildDateTime(TimeZoneInfo guildTimezone, DateTime inputTime)
        {
            var now = DateTime.UtcNow;
            Timezone = guildTimezone;
            CurrentGuildTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Utc, Timezone);
            InputTime = inputTime;
            InputTimeUtc = TimeZoneInfo.ConvertTime(inputTime, Timezone, TimeZoneInfo.Utc);
        }
    }
}
