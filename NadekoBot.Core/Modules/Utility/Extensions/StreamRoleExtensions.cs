﻿using Microsoft.EntityFrameworkCore;
using Kotocorn.Core.Services.Database.Models;
using Kotocorn.Core.Services.Database.Repositories;

namespace Kotocorn.Modules.Utility.Extensions
{
    public static class StreamRoleExtensions
    {
        /// <summary>
        /// Gets full stream role settings for the guild with the specified id.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="guildId">Id of the guild to get stream role settings for.</param>
        /// <returns></returns>
        public static StreamRoleSettings GetStreamRoleSettings(this IGuildConfigRepository gc, ulong guildId)
        {
            var conf = gc.For(guildId, set => set.Include(y => y.StreamRole)
                .Include(y => y.StreamRole.Whitelist)
                .Include(y => y.StreamRole.Blacklist));

            if (conf.StreamRole == null)
                conf.StreamRole = new StreamRoleSettings();

            return conf.StreamRole;
        }
    }
}
