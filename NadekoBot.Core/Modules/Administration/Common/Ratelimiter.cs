using System.Collections.Concurrent;

namespace Kotocorn.Modules.Administration.Common
{
    public class Slowmoder
    {
        public ConcurrentDictionary<ulong, uint> Users { get; set; } = new ConcurrentDictionary<ulong, uint>();
        public uint MaxMessages { get; set; }
        public int PerSeconds { get; set; }
    }
}
