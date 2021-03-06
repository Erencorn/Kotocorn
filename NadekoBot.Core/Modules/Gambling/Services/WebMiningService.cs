﻿#if GLOBAL_Kotocorn
using Kotocorn.Core.Modules.Gambling.Common;
using Kotocorn.Core.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kotocorn.Core.Modules.Gambling.Services
{
    public class WebMiningService : INService
    {
        private readonly Logger _log;
        private readonly IBotCredentials _creds;
        private readonly HttpClient _http;
        private readonly DbService _db;
        private readonly CurrencyService _cs;
        private readonly Task _reqTask;

        public WebMiningService(Kotocorn Kotocorn,IBotCredentials creds, DbService db, CurrencyService cs)
        {
            _log = LogManager.GetCurrentClassLogger();
            _creds = creds;
            _http = new HttpClient();
            _db = db;
            _cs = cs;

            if (!string.IsNullOrWhiteSpace(_creds.MiningProxyCreds))
            {
                var byteArray = Encoding.ASCII.GetBytes(_creds.MiningProxyCreds);
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            if (Kotocorn.Client.ShardId == 0)
                _reqTask = RequestAsync();
        }

        private async Task RequestAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(60)).ConfigureAwait(false);
                await PayoutRewards();
            }
        }

        private async Task PayoutRewards()
        {
            if (string.IsNullOrWhiteSpace(_creds.MiningProxyUrl))
                return;

            try
            {
                _log.Info("Paying out mining rewards.");
                var res = await _http.GetStringAsync(_creds.MiningProxyUrl).ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<Payout[]>(res);
                if (data.Length == 0)
                {
                    _log.Info("No payouts sent out.");
                    return;
                }

                var filtered = data.Where(x => x.Amount > 0 && ulong.TryParse(x.User, out _)).ToArray();
                await _cs.AddBulkAsync(filtered.Select(x => ulong.Parse(x.User)), 
                    filtered.Select(x => "Mining payout"), 
                    filtered.Select(x => (long)x.Amount), true);
            }
            catch (Exception ex)
            {
                _log.Warn(ex);
            }
        }

        //public async Task<decimal> GetMoneroMarketPrice()
        //{
        //    if (DateTime.UtcNow - lastMoneroPriceUpdate > TimeSpan.FromHours(1))
        //    {
        //        try
        //        {
        //            var res = await _http.GetStringAsync("https://min-api.cryptocompare.com/data/price?fsym=XMR&tsyms=USD");
        //            var obj = new { Usd = 300.00m };
        //            var price = JsonConvert.DeserializeAnonymousType(res, obj);
        //            _moneroMarketPrice = price.Usd * 100;
        //        }
        //        catch (Exception ex)
        //        {
        //            _log.Warn(ex);
        //        }
        //    }

        //    return _moneroMarketPrice;
        //}
    }
}
#endif