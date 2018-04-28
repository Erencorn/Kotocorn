using Microsoft.EntityFrameworkCore;
using Kotocorn.Core.Services.Database.Models;
using System;
using System.Linq;

namespace Kotocorn.Core.Services.Database.Repositories
{
    public interface IBotConfigRepository : IRepository<BotConfig>
    {
        BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null);
    }
}
