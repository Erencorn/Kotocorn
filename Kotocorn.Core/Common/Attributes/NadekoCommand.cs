using System.Runtime.CompilerServices;
using Discord.Commands;
using Kotocorn.Core.Services.Impl;

namespace Kotocorn.Common.Attributes
{
    public class NadekoCommand : CommandAttribute
    {
        public NadekoCommand([CallerMemberName] string memberName="") : base(Localization.LoadCommand(memberName.ToLowerInvariant()).Cmd.Split(' ')[0])
        {

        }
    }
}
