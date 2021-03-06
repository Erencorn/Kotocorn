﻿using Discord;
using Discord.Commands;
using Kotocorn.Common;
using Kotocorn.Common.Attributes;
using System;
using System.Threading.Tasks;

namespace Kotocorn.Modules.Utility
{
    public partial class Utility
    {
        public class BotConfigCommands : KotocornSubmodule
        {
            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task BotConfigEdit()
            {
                var names = Enum.GetNames(typeof(BotConfigEditType));
                await ReplyAsync(string.Join(", ", names)).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task BotConfigEdit(BotConfigEditType type, [Remainder]string newValue = null)
            {
                if (string.IsNullOrWhiteSpace(newValue))
                    newValue = null;

                var success = _bc.Edit(type, newValue);

                if (!success)
                    await ReplyErrorLocalized("bot_config_edit_fail", Format.Bold(type.ToString()), Format.Bold(newValue ?? "NULL")).ConfigureAwait(false);
                else
                    await ReplyConfirmLocalized("bot_config_edit_success", Format.Bold(type.ToString()), Format.Bold(newValue ?? "NULL")).ConfigureAwait(false);
            }
        }
    }
}
