using Kotocorn.Modules.Music.Common.SongResolver.Strategies;
using Kotocorn.Core.Services.Database.Models;
using System.Threading.Tasks;

namespace Kotocorn.Modules.Music.Common.SongResolver
{
    public interface ISongResolverFactory
    {
        Task<IResolveStrategy> GetResolveStrategy(string query, MusicType? musicType);
    }
}
