using System.Collections.Generic;
using Kotocorn.Core.Services.Database.Models;

namespace Kotocorn.Core.Services.Database.Repositories
{
    public interface ICurrencyTransactionsRepository : IRepository<CurrencyTransaction>
    {
        List<CurrencyTransaction> GetPageFor(ulong userId, int page);
    }
}
