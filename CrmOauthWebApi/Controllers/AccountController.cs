using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmOauthWebApi.Controllers
{
    public class AccountController : ApiController
    {
        private static AuthenticationResult _AuthenticationResult;
        private static JObject _jAccounts;

        // GET: api/Account
        public JObject Get()
        {
            // TODO Substitute your correct CRM root service address, 
            string resource = "https://demogold.crm.dynamics.com";

            // TODO Substitute your app registration values that can be obtained after you
            // register the app in Active Directory on the Microsoft Azure portal.
            string clientId = "c447dac5-f9bc-4ca2-bc0f-4d8c649583f9";
            string redirectUrl = "https://localhost:44300/";


            // Authenticate the registered application with Azure Active Directory.
            AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/common", false);
            _AuthenticationResult = authContext.AcquireToken(resource, clientId, new Uri(redirectUrl));

            //using this page: https://github.com/jlattimer/CrmWebApiCSharp/blob/master/CrmWebApiCSharp/Program.cs
            Task.WaitAll(Task.Run(async () => await GetAccounts()));

            return _jAccounts;
        }

        private static async Task GetAccounts()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _AuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync("https://demogold.crm.dynamics.com/api/data/v8.0/accounts?$select=name&$top=3");

                _jAccounts = JObject.Parse(response.Content.ReadAsStringAsync().Result); 
            }
        }

        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
    }
}
