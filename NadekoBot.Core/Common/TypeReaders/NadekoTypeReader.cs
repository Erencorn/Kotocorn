using Discord.Commands;
using Discord.WebSocket;

namespace Kotocorn.Core.Common.TypeReaders
{
    public abstract class KotocornTypeReader<T> : TypeReader
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmds;

        private KotocornTypeReader() { }
        public KotocornTypeReader(DiscordSocketClient client, CommandService cmds)
        {
            _client = client;
            _cmds = cmds;
        }
    }
}
