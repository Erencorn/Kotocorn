using Kotocorn.Core.Modules.Gambling.Common.Blackjack;
using Kotocorn.Core.Services;
using System.Collections.Concurrent;

namespace Kotocorn.Core.Modules.Gambling.Services
{
    public class BlackJackService : INService
    {
        public ConcurrentDictionary<ulong, Blackjack> Games { get; } = new ConcurrentDictionary<ulong, Blackjack>();
    }
}
