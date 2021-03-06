﻿using Kotocorn.Common.Attributes;
using Kotocorn.Core.Modules.Gambling.Services;
using System.Threading.Tasks;
using Discord;
using Kotocorn.Extensions;
using System.Linq;
using Discord.Commands;
using Kotocorn.Core.Modules.Gambling.Common;
using Kotocorn.Core.Common;

namespace Kotocorn.Modules.Gambling
{
    public partial class Gambling
    {
        public class CurrencyRaffleCommands : GamblingSubmodule<CurrencyRaffleService>
        {
            public enum Mixed { Mixed }

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [Priority(0)]
            public Task RaffleCur(Mixed _, ShmartNumber amount) =>
                RaffleCur(amount, true);

            [KotocornCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [Priority(1)]
            public async Task RaffleCur(ShmartNumber amount, bool mixed = false)
            {
                if (!await CheckBetMandatory(amount))
                    return;
                async Task OnEnded(IUser arg, long won)
                {
                    await Context.Channel.SendConfirmAsync(GetText("rafflecur_ended", _bc.BotConfig.CurrencyName, Format.Bold(arg.ToString()), won + _bc.BotConfig.CurrencySign));
                }
                var res = await _service.JoinOrCreateGame(Context.Channel.Id,
                    Context.User, amount, mixed, OnEnded)
                        .ConfigureAwait(false);

                if (res.Item1 != null)
                {
                    await Context.Channel.SendConfirmAsync(GetText("rafflecur", res.Item1.GameType.ToString()),
                        string.Join("\n", res.Item1.Users.Select(x => $"{x.DiscordUser} ({x.Amount})")),
                        footer: GetText("rafflecur_joined", Context.User.ToString())).ConfigureAwait(false);
                }
                else
                {
                    if (res.Item2 == CurrencyRaffleService.JoinErrorType.AlreadyJoinedOrInvalidAmount)
                        await ReplyErrorLocalized("rafflecur_already_joined").ConfigureAwait(false);
                    else if (res.Item2 == CurrencyRaffleService.JoinErrorType.NotEnoughCurrency)
                        await ReplyErrorLocalized("not_enough", _bc.BotConfig.CurrencySign).ConfigureAwait(false);
                }
            }
        }
    }
}
