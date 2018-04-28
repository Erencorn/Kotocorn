using Kotocorn.Common;
using Kotocorn.Core.Services.Database.Models;

namespace Kotocorn.Core.Services
{
    public interface IBotConfigProvider
    {
        BotConfig BotConfig { get; }
        void Reload();
        bool Edit(BotConfigEditType type, string newValue);
    }
}
