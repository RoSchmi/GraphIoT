﻿using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Clients.Withings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/withings")]
    public class WithingsController : Controller
    {
        private readonly WithingsClient _withingsClient;
        public WithingsController(WithingsClient withingsClient)
        {
            _withingsClient = withingsClient;
        }

        // GET: api/withings/login
        [HttpGet("login")]
        public void Login()
        {
            var scopes = new string[] { "user.info", "user.metrics", "user.activity" };

            Response.Redirect("https://account.withings.com/oauth2_user/authorize2?response_type=code&client_id=" + _withingsClient.ClientId +
                "&state=f84b57ec&scope=" + string.Join(",", scopes) + "&redirect_uri=https://your.domain/smarthome/api/withings/callback");
        }

        // POST: api/withings/callback
        [HttpPost("callback")]
        [HttpGet("callback")]
        public async Task<IActionResult> PostCallback(string code, string state)
        {
            if (state != "f84b57ec")
                return StatusCode((int)HttpStatusCode.Forbidden);

            var authorization_code = code;
            var refresh_token = await _withingsClient.AuthenticateLogin(authorization_code);

            async Task sendTelegramMessage(string message) => await (new HttpClient()).GetStringAsync("https://api.telegram.org/***REMOVED***/sendMessage?chat_id=***REMOVED***&text=" + WebUtility.UrlEncode(message));
            await sendTelegramMessage("Refresh token: " + refresh_token);

            return StatusCode((int)HttpStatusCode.OK);
        }
        
        // HEAD: api/withings/callback
        [HttpHead("callback")]
        public IActionResult HeadCallback()
        {
            return StatusCode((int)HttpStatusCode.OK);
        }

        // GET: api/withings/measures
        [HttpGet("measures")]
        public async Task<JsonResult> Measures()
        {
            var measures = await _withingsClient.GetMeasures(WithingsClient.MeasureType.Weight);

            return Json(new
            {
                measures = measures.OrderBy(x => x.Key).TakeLast(37).Select(x => Math.Round(((decimal)x.Value) / 1000, 2).ToString() + " kg")
            });
        }

        // GET: api/withings/devices
        [HttpGet("devices")]
        public async Task<JsonResult> Devices()
        {
            var devices = await _withingsClient.GetDevices();

            return Json(new
            {
                devices = devices.Select(x => new
                {
                    model = x.Item1,
                    battery = x.Item2
                })
            });
        }

        // GET: api/withings/lametric
        [HttpGet("lametric")]
        public async Task<JsonResult> LaMetric()
        {
            var measures = (await _withingsClient.GetMeasures(WithingsClient.MeasureType.Weight)).OrderBy(x => x.Key).ToList();

            var minMeasure = measures.TakeLast(37).Select(x => x.Value).Min();
            var chartMeasures = measures.TakeLast(37).Select(x => (int)(Math.Round(((decimal)(x.Value - minMeasure)) / 1000, 1) * 10));
            var lastMeasure = Math.Round(((decimal)measures.Last().Value) / 1000, 1);
            var prevLastMeasure = Math.Round(((decimal)measures.SkipLast(1).Last().Value) / 1000, 1);

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = "Philip",
                        icon = "i173"
                    },
                    new
                    {
                        text = $"{lastMeasure} kg",
                        icon = "i173"
                    },
                    new
                    {
                        index = 2,
                        chartData = chartMeasures
                    },
                    new
                    {
                        text = (prevLastMeasure < lastMeasure ? "+" : "-") + $" {Math.Abs(prevLastMeasure - lastMeasure)} kg",
                        icon = prevLastMeasure < lastMeasure ? "i4103" : "i402"
                    }
                }
            });
        }
    }
}
