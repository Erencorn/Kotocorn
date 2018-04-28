using System.Threading.Tasks;

namespace Kotocorn.Modules.Music.Common.SongResolver.Strategies
{
    public interface IResolveStrategy
    {
        Task<SongInfo> ResolveSong(string query);
    }
}
