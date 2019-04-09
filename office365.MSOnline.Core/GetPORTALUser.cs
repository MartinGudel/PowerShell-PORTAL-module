using System;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace office365.PORTAL.Core
{
    [Cmdlet("Get", "PORTALUser")]
    public class GetPORTALUser : PSCmdlet
    {


        protected override void EndProcessing()
        {
            String userEndpoint = "https://graph.microsoft.com/v1.0/users";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent",
                                ".NET Foundation MSONLINE Rest API Client");
            client.DefaultRequestHeaders.Add("Authorization",
                                        "Bearer " + GlobalAuthToken.Auth.access_token);

            WriteVerbose("Autorization header: Bearer " + GlobalAuthToken.Auth.access_token);

            var streamTask = client.GetStringAsync(userEndpoint);
            streamTask.Wait();
            String t = streamTask.Result;

            WriteObject(JsonConvert.DeserializeObject<PORTALUserObjectList>(t));

            // this.WriteObject($"not implemented yet. :(");
            base.EndProcessing();
        }
    }

}

