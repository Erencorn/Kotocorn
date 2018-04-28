using System.Collections.Generic;
using Kotocorn.Core.Services.Database.Models;

namespace Kotocorn.Core.Services.Database.Repositories
{
    public interface ICustomReactionRepository : IRepository<CustomReaction>
    {
        CustomReaction[] GetGlobalAndFor(IEnumerable<long> ids);
        CustomReaction[] For(ulong id);
    }
}
