﻿using Kotocorn.Modules.Music.Common;
using Kotocorn.Core.Services.Database.Models;
using Kotocorn.Core.Services.Impl;
using System;
using System.Threading.Tasks;

namespace Kotocorn.Modules.Music.Extensions
{
    public static class Extensions
    {
        public static Task<SongInfo> GetSongInfo(this SoundCloudVideo svideo) =>
            Task.FromResult(new SongInfo
            {
                Title = svideo.FullName,
                Provider = "SoundCloud",
                Uri = () => svideo.StreamLink(),
                ProviderType = MusicType.Soundcloud,
                Query = svideo.TrackLink,
                Thumbnail = svideo.artwork_url,
                TotalTime = TimeSpan.FromMilliseconds(svideo.Duration)
            });
    }
}
