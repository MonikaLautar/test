using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ATMInfoAPI.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static ATMInfoAPI.ATMGetListResponse;
using static ATMInfoAPI.ATMGetStatusResponse;

namespace ATMInfoAPI.Controllers
{
    [ApiController]
    [Route("v1")]
    public class ATMInfoController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ATMInfoController> _logger;

        string _url;
        string _username;
        string _password;

        public ATMInfoController(IConfiguration config, ILogger<ATMInfoController> logger)
        {
            _config = config;
            _logger = logger;

            _url = _config["BORESTWS:Url"];
            _username = _config["BORESTWS:Username"];
            _password = _config["BORESTWS:Password"];
        }

        [HttpGet]
        [Route("ATMListMock")]
        public IEnumerable<ATM> Get(string termId)
        {
            Random rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new ATM
            {
                TermId = termId,
                BankId = rng.Next().ToString(),
                Location = "Poslovalnica banke",
                Address = "Cankarjeva ulica 2",
                City = "Velenje",
                PostCode = "1000",
                Printer = false,
                Deposit = false,
                Availability = true,
                Cards = "DECPVR",
                SpecialPayments = true,
                BNA = true,
                TransactionReceipt = true
            })
            .ToArray();
        }

        [HttpGet]
        [Route("ATMList")]
        public IEnumerable<ATM> GetATMListBO()
        {
            string response = GetResponseString("GetATMList");
            ATMGetListResponse data = BRest.DeserializeResponse<ATMGetListResponse>(response);
            return data != null ? data.atmList : null;
        }
        
        [HttpGet]
        [Route("ATMStatus")]
        public IEnumerable<ATMStatus> GetATMStatusBO()
        {
            string response = GetResponseString("GetATMStatus");
            ATMGetStatusResponse data = BRest.DeserializeResponse<ATMGetStatusResponse>(response);
            return data != null ? data.atmStatusList : null;
        }

        private string GetResponseString(string method)
        {
            string response = null;
            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var client = BRest.GetHttpClient())
                {
                    BRest.SetBasicAuth(client, _username, _password);
                    response = BRest.HttpGet(client, $"{_url}/{method}");
                }
            }
            return response;
        }
    }
}
