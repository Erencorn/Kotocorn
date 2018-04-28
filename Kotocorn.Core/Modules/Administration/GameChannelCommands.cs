using Discord;
using Discord.Commands;
using Kotocorn.Core.Services;
using System.Threading.Tasks;
using Kotocorn.Common.Attributes;
using Kotocorn.Modules.Administration.Services;

namespace Kotocorn.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class GameChannelCommands : NadekoSubmodule<GameVoiceChannelService>
        {
            private readonly DbService _db;

            public GameChannelCommands(DbService db)
            {
                _db = db;
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.Administrator)]
            [RequireBotPermission(GuildPermission.MoveMembers)]
            public async Task GameVoiceChannel()
            {
                var vch = ((IGuildUser)Context.User).VoiceChannel;

                if (vch == null)
                {
                    await ReplyErrorLocalized("not_in_voice").ConfigureAwait(false);
                    return;
                }
                var id = _service.ToggleGameVoiceChannel(Context.Guild.Id, vch.Id);

                if (id == null)
                {
                    await ReplyConfirmLocalized("gvc_disabled").ConfigureAwait(false);
                }
                else
                {
                    _service.GameVoiceChannels.Add(vch.Id);
                    await ReplyConfirmLocalized("gvc_enabled", Format.Bold(vch.Name)).ConfigureAwait(false);
                }
            }
        }
    }
}
