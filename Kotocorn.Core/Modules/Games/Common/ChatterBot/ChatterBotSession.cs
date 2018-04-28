﻿using System.Net.Http;
using System.Threading.Tasks;
using Kotocorn.Common;
using Kotocorn.Extensions;
using Newtonsoft.Json;

namespace Kotocorn.Modules.Games.Common.ChatterBot
{
    public class ChatterBotSession : IChatterBotSession
    {
        private static NadekoRandom Rng { get; } = new NadekoRandom();

        private readonly string _chatterBotId;
        private int _botId = 6;

        public ChatterBotSession()
        {
            _chatterBotId = Rng.Next(0, 1000000).ToString().ToBase64();
        }

        private string apiEndpoint => "http://api.program-o.com/v2/chatbot/" +
                                      $"?bot_id={_botId}&" +
                                      "say={0}&" +
                                      $"convo_id=Kotocorn_{_chatterBotId}&" +
                                      "format=json";

        public async Task<string> Think(string message)
        {
            using (var http = new HttpClient())
            {
                var res = await http.GetStringAsync(string.Format(apiEndpoint, message)).ConfigureAwait(false);
                var cbr = JsonConvert.DeserializeObject<ChatterBotResponse>(res);
                return cbr.BotSay.Replace("<br/>", "\n");
            }
        }
    }
}