using System;

namespace Kotocorn.Modules.Searches.Common.Exceptions
{
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string message) : base($"Stream '{message}' not found.")
        {
        }
    }
}
