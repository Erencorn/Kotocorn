using System;
using Discord.Commands;

namespace Kotocorn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed class KotocornModuleAttribute : GroupAttribute
    {
        public KotocornModuleAttribute(string moduleName) : base(moduleName)
        {
        }
    }
}

