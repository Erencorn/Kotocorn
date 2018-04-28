using System.Runtime.CompilerServices;
using Discord.Commands;
using Kotocorn.Core.Services.Impl;

namespace Kotocorn.Common.Attributes
{
    public class Description : SummaryAttribute
    {
        public Description([CallerMemberName] string memberName="") : base(Localization.LoadCommand(memberName.ToLowerInvariant()).Desc)
        {

        }
    }
}
