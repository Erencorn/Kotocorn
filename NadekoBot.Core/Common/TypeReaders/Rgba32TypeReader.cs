﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using ImageSharp;

namespace Kotocorn.Core.Common.TypeReaders
{
    public class Rgba32TypeReader : KotocornTypeReader<Rgba32>
    {
        public Rgba32TypeReader(DiscordSocketClient client, CommandService cmds) : base(client, cmds)
        {
        }

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            await Task.Yield();

            input = input.Replace("#", "");
            try
            {
                return TypeReaderResult.FromSuccess(Rgba32.FromHex(input));
            }
            catch
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "Parameter is not a valid color hex.");
            }
        }
    }
}
