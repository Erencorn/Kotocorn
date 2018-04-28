using Kotocorn.Core.Common.Pokemon;
using Kotocorn.Modules.Games.Common.Trivia;
using System.Collections.Generic;

namespace Kotocorn.Core.Services
{
    public interface ILocalDataCache
    {
        IReadOnlyDictionary<string, SearchPokemon> Pokemons { get; }
        IReadOnlyDictionary<string, SearchPokemonAbility> PokemonAbilities { get; }
        TriviaQuestion[] TriviaQuestions { get; }
        IReadOnlyDictionary<int, string> PokemonMap { get; }
    }
}
