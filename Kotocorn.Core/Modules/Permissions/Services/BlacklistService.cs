using System.Linq;
using System.Threading.Tasks;
using Discord;
using Kotocorn.Common.Collections;
using Kotocorn.Common.ModuleBehaviors;
using Kotocorn.Core.Services;
using Kotocorn.Core.Services.Database.Models;

namespace Kotocorn.Modules.Permissions.Services
{
    public class BlacklistService : IEarlyBlocker, INService
    {
        public ConcurrentHashSet<ulong> BlacklistedUsers { get; }
        public ConcurrentHashSet<ulong> BlacklistedGuilds { get; }
        public ConcurrentHashSet<ulong> BlacklistedChannels { get; }

        public BlacklistService(IBotConfigProvider bc)
        {
            var blacklist = bc.BotConfig.Blacklist;
            BlacklistedUsers = new ConcurrentHashSet<ulong>(blacklist.Where(bi => bi.Type == BlacklistType.User).Select(c => c.ItemId));
            BlacklistedGuilds = new ConcurrentHashSet<ulong>(blacklist.Where(bi => bi.Type == BlacklistType.Server).Select(c => c.ItemId));
            BlacklistedChannels = new ConcurrentHashSet<ulong>(blacklist.Where(bi => bi.Type == BlacklistType.Channel).Select(c => c.ItemId));
        }

        public Task<bool> TryBlockEarly(IGuild guild, IUserMessage usrMsg)
            => Task.FromResult((guild != null && BlacklistedGuilds.Contains(guild.Id)) ||
                BlacklistedChannels.Contains(usrMsg.Channel.Id) ||
                BlacklistedUsers.Contains(usrMsg.Author.Id));
    }
}
