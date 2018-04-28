using Discord;
using Discord.Commands;
using Kotocorn.Core.Services;
using Kotocorn.Core.Services.Database.Models;
using System.Threading.Tasks;
using Kotocorn.Common.Attributes;
using Kotocorn.Common.Collections;
using Kotocorn.Modules.Permissions.Services;
using Kotocorn.Common.TypeReaders;

namespace Kotocorn.Modules.Permissions
{
    public partial class Permissions
    {
        [Group]
        public class BlacklistCommands : KotocornSubmodule<BlacklistService>
        {
            private readonly DbService _db;
            private readonly IBotCredentials _creds;

            private ConcurrentHashSet<ulong> BlacklistedUsers => _service.BlacklistedUsers;
            private ConcurrentHashSet<ulong> BlacklistedGuilds => _service.BlacklistedGuilds;
            private ConcurrentHashSet<ulong> BlacklistedChannels => _service.BlacklistedChannels;

            public BlacklistCommands(DbService db, IBotCredentials creds)
            {
                _db = db;
                _creds = creds;
            }

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public Task UserBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.User);

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public Task UserBlacklist(AddRemove action, IUser usr)
                => Blacklist(action, usr.Id, BlacklistType.User);

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ChannelBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.Channel);

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ServerBlacklist(AddRemove action, ulong id)
                => Blacklist(action, id, BlacklistType.Server);

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public Task ServerBlacklist(AddRemove action, IGuild guild)
                => Blacklist(action, guild.Id, BlacklistType.Server);

            private async Task Blacklist(AddRemove action, ulong id, BlacklistType type)
            {
                if(action == AddRemove.Add && _creds.OwnerIds.Contains(id))
                    return;

                using (var uow = _db.UnitOfWork)
                {
                    if (action == AddRemove.Add)
                    {
                        var item = new BlacklistItem { ItemId = id, Type = type };
                        uow.BotConfig.GetOrCreate().Blacklist.Add(item);
                        if (type == BlacklistType.Server)
                        {
                            BlacklistedGuilds.Add(id);
                        }
                        else if (type == BlacklistType.Channel)
                        {
                            BlacklistedChannels.Add(id);
                        }
                        else if (type == BlacklistType.User)
                        {
                            BlacklistedUsers.Add(id);
                        }                        
                    }
                    else
                    {
                        uow.BotConfig.GetOrCreate().Blacklist.RemoveWhere(bi => bi.ItemId == id && bi.Type == type);
                        if (type == BlacklistType.Server)
                        {
                            BlacklistedGuilds.TryRemove(id);
                        }
                        else if (type == BlacklistType.Channel)
                        {
                            BlacklistedChannels.TryRemove(id);
                        }
                        else if (type == BlacklistType.User)
                        {
                            BlacklistedUsers.TryRemove(id);
                        }
                    }
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                if(action == AddRemove.Add)
                    await ReplyConfirmLocalized("blacklisted", Format.Code(type.ToString()), Format.Code(id.ToString())).ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("unblacklisted", Format.Code(type.ToString()), Format.Code(id.ToString())).ConfigureAwait(false);
            }
        }
    }
}
