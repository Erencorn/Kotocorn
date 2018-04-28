using Kotocorn.Core.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kotocorn
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            if (args.Length == 2 
                && int.TryParse(args[0], out int shardId) 
                && int.TryParse(args[1], out int parentProcessId))
            {
                return new Kotocorn(shardId, parentProcessId)
                    .RunAndBlockAsync(args);
            }
            else
            {
#if DEBUG
                var _ = new Kotocorn(0, Process.GetCurrentProcess().Id)
                       .RunAsync(args);
#endif
                return new ShardsCoordinator()
                    .RunAndBlockAsync();
            }
        }
    }
}
