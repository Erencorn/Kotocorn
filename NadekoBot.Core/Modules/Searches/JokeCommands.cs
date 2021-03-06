﻿using Discord.Commands;
using Kotocorn.Extensions;
using Kotocorn.Modules.Searches.Services;
using System.Linq;
using System.Threading.Tasks;
using Kotocorn.Common;
using Kotocorn.Common.Attributes;

namespace Kotocorn.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class JokeCommands : KotocornSubmodule<SearchesService>
        {

            [KotocornCommand, Usage, Description, Aliases]
            public async Task Yomama()
            {
                await Context.Channel.SendConfirmAsync(await _service.GetYomamaJoke()).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            public async Task Randjoke()
            {
                var jokeInfo = await _service.GetRandomJoke();
                await Context.Channel.SendConfirmAsync("", jokeInfo.Text, footer: jokeInfo.BaseUri).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            public async Task ChuckNorris()
            {
                await Context.Channel.SendConfirmAsync(await _service.GetChuckNorrisJoke()).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            public async Task WowJoke()
            {
                if (!_service.WowJokes.Any())
                {
                    await ReplyErrorLocalized("jokes_not_loaded").ConfigureAwait(false);
                    return;
                }
                var joke = _service.WowJokes[new KotocornRandom().Next(0, _service.WowJokes.Count)];
                await Context.Channel.SendConfirmAsync(joke.Question, joke.Answer).ConfigureAwait(false);
            }

            [KotocornCommand, Usage, Description, Aliases]
            public async Task MagicItem()
            {
                if (!_service.WowJokes.Any())
                {
                    await ReplyErrorLocalized("magicitems_not_loaded").ConfigureAwait(false);
                    return;
                }
                var item = _service.MagicItems[new KotocornRandom().Next(0, _service.MagicItems.Count)];

                await Context.Channel.SendConfirmAsync("✨" + item.Name, item.Description).ConfigureAwait(false);
            }
        }
    }
}
