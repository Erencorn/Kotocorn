﻿using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Kotocorn.Common.Attributes;
using Kotocorn.Modules.Permissions.Services;

namespace Kotocorn.Modules.Permissions
{
    public partial class Permissions
    {
        [Group]
        public class ResetPermissionsCommands : NadekoSubmodule
        {
            private readonly ResetPermissionsService _service;

            public ResetPermissionsCommands(ResetPermissionsService service)
            {
                _service = service;
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task ResetPermissions()
            {
                await _service.ResetPermissions(Context.Guild.Id).ConfigureAwait(false);
                await ReplyConfirmLocalized("perms_reset").ConfigureAwait(false);
            }

            [NadekoCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task ResetGlobalPermissions()
            {
                await _service.ResetGlobalPermissions().ConfigureAwait(false);
                await ReplyConfirmLocalized("global_perms_reset").ConfigureAwait(false);
            }
        }
    }
}