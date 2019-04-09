using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace office365.PORTAL.Core
{
    [Cmdlet("Connect", "PORTALService")]
    public class ConnectPORTALService : PSCmdlet
    {
        [Parameter(Position = 0, 
                    Mandatory = true, 
                    ParameterSetName = "ConnectPORTALService01")]
        public PSCredential credential { get; set; }

        [Parameter(Position = 0, 
                    Mandatory = true, 
                    ParameterSetName = "ConnectPORTALService02")]
        public String AdGraphAccessToken { get; set; }

         [Parameter(Position = 0, 
                    Mandatory = true, 
                    ParameterSetName = "ConnectPORTALService03")]
        public String MsGraphAccessToken { get; set; }

       [Parameter( Mandatory = false, 
                    ParameterSetName = "ConnectPORTALService01")]
        [Parameter( Mandatory = false, 
                    ParameterSetName = "ConnectPORTALService02")]
        [Parameter( Mandatory = false, 
                    ParameterSetName = "ConnectPORTALService03")]
        public AzureEnvironment AzureEnvironment { get; set; }

        public WellKnownOpenIDConfiguration configuration;

        private String endpoint;
        protected override void ProcessRecord()
        {


        }

        protected override void EndProcessing()
        {

            configuration = JsonConvert.DeserializeObject<WellKnownOpenIDConfiguration>(getAzureRegions());

            switch (AzureEnvironment)
            {
                case AzureEnvironment.AzureCloud:
                    {
                        endpoint = configuration.metadata[0].preferred_network;
                        break;
                    }
                case AzureEnvironment.AzureChinaCloud:
                    {
                        endpoint = configuration.metadata[1].preferred_network;
                        break;
                    }
                case AzureEnvironment.AzureGermanyCloud:
                    {
                        endpoint = configuration.metadata[2].preferred_network;
                        break;
                    }
                case AzureEnvironment.USGovernment:
                    {
                        endpoint = configuration.metadata[3].preferred_network;
                        break;
                    }
                default:
                    {
                        endpoint = configuration.metadata[0].preferred_network;
                        break;
                    }
            }

            HttpResponseMessage r;
            String result = getGrantTypePasswordAuthentication(endpoint, out r);

            //
            // needs to be reworked using events
            //    
            if (r.IsSuccessStatusCode)
            {
                GlobalAuthToken.Auth = JsonConvert.DeserializeObject<BearerToken>(result);

                WriteObject(GlobalAuthToken.Auth);
                WriteVerbose("IsSuccessStatusCode -eq $true: Authentication successful.");
            }
            else
            {
                AuthError AE = JsonConvert.DeserializeObject<AuthError>(result);
                if (AE.error.Equals("interaction_required"))
                {
                    // implementation required
                    WriteVerbose("AE.error.Equals(interaction_required). Trying authentication using GrantType code.");
                    GlobalAuthToken.Auth = getGrantTypeCodeAuthentication(endpoint);
                    if (GlobalAuthToken.Auth.access_token.Length > 10) {
                         WriteObject(GlobalAuthToken.Auth);
                         WriteVerbose("Authentication successful.");       
                    }
                }
                else
                {
                    WriteObject(AE);
                    WriteVerbose("Authentication failed."); 
                }

            }
        }

        private String getAzureRegions()
        {
            HttpClient client = new HttpClient();
            String discoverURL = "https://login.microsoftonline.com/common/discovery/instance?api-version=1.1&authorization_endpoint=https://login.microsoftonline.com/common/oauth2/authorize";

            String usernameUrlEncoded = WebUtility.UrlEncode(credential.UserName);
            String password = new System.Net.NetworkCredential(string.Empty, credential.Password).Password;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation PORTAL Rest API Client");

            var streamTask = client.GetStringAsync(discoverURL);
            streamTask.Wait();
            return streamTask.Result;
        }

        private String getGrantTypePasswordAuthentication(string endpoint,
                                                        out HttpResponseMessage r)
        {
            WriteVerbose("getGrantTypePasswordAuthentication: running getGrantTypePasswordAuthentication - fully functional. ");    
            HttpClient client = new HttpClient();
            String GranttypePassword = "password";
            String tokenEndpointUrl = "https://" + endpoint + "/common/oauth2/token";
            String clientid = "1b730954-1685-4b74-9bfd-dac224a7b894";
            String result;
            Task<string> p;
            Dictionary<string, string> tokenEndpointGranttypePasswordDict = new Dictionary<string, string>();
            tokenEndpointGranttypePasswordDict.Add("resource", "https://graph.microsoft.com");
            tokenEndpointGranttypePasswordDict.Add("grant_type", GranttypePassword);
            tokenEndpointGranttypePasswordDict.Add("client_id", clientid);
            tokenEndpointGranttypePasswordDict.Add("username", credential.UserName);
            tokenEndpointGranttypePasswordDict.Add("password", (new System.Net.NetworkCredential(string.Empty, credential.Password)).Password);
            tokenEndpointGranttypePasswordDict.Add("scope", "openid");

            Task<HttpResponseMessage> response = client.PostAsync(tokenEndpointUrl,
                            new FormUrlEncodedContent(tokenEndpointGranttypePasswordDict));
            response.Wait();
            r = response.Result;

            p = r.Content.ReadAsStringAsync();
            p.Wait();
            result = p.Result;

            return result;
        }

        private BearerToken getGrantTypeCodeAuthentication(string endpoint)
        {
            // code authentication is currently not possible as we do not have a browser 
            // support for the auth rendering. instead, we use "devicecode" similar to what 
            // is implemented in the Azure az module.
            // I didn't find an official documentation for that procedure on Microsofts
            // website
            UserCodeObject deviceCode = getDeviceCode(endpoint);
           
            BearerToken bt = null;
            bt = getDeviceCodeAuthentication(endpoint, deviceCode);
            return bt;
        }

        private BearerToken getDeviceCodeAuthentication(string endpoint, UserCodeObject deviceCode)
        {
            BearerToken bt = null;
            int graceTime = Int32.Parse(deviceCode.expires_in);
            bool deviceauthenticated = false;
            while ((graceTime > 0) && (deviceauthenticated == false))
            {
                graceTime = graceTime - Int32.Parse(deviceCode.interval);
                WriteWarning(deviceCode.message);

                HttpClient client = new HttpClient();
                String tokenEndpointUrl = "https://" + endpoint + "/Common/oauth2/token";
                String clientid = "1b730954-1685-4b74-9bfd-dac224a7b894";
                String Granttype = "device_code";
                String result;
                Task<string> p;
                Dictionary<string, string> tokenEndpointGranttypeDict = new Dictionary<string, string>();
                tokenEndpointGranttypeDict.Add("resource", "https://graph.microsoft.com");
                tokenEndpointGranttypeDict.Add("grant_type", Granttype);
                tokenEndpointGranttypeDict.Add("client_id", clientid);
                tokenEndpointGranttypeDict.Add("code", deviceCode.device_code);

                Task<HttpResponseMessage> response = client.PostAsync(tokenEndpointUrl,
                            new FormUrlEncodedContent(tokenEndpointGranttypeDict));
                response.Wait();
                var r = response.Result;
                p = r.Content.ReadAsStringAsync();
                p.Wait();
                result = p.Result;
                if (r.IsSuccessStatusCode)
                {
                    bt = JsonConvert.DeserializeObject<BearerToken>(result);
                    WriteVerbose("authentication successfull");
                    deviceauthenticated = true;
                }
                else
                {
                    AuthError AE = JsonConvert.DeserializeObject<AuthError>(result);
                
                    WriteVerbose("authen1tication failed - waiting " + graceTime);
                    String badVerificationCode = "bad_verification_code";
                    if (AE.error.Equals(badVerificationCode))
                    {
                        deviceCode = getDeviceCode(endpoint);
                    }                   
                    System.Threading.Thread.Sleep(Int32.Parse(deviceCode.interval) * 1000);
                }
            }
            return bt;
        }

        private UserCodeObject getDeviceCode(string endpoint)
        {
            HttpClient client = new HttpClient();
            UserCodeObject deviceCode;
            Task<string> p;

            String resource = "https://graph.microsoft.com";
            String authorizationEndpointUrl = "https://" + endpoint + "/common/oauth2/devicecode";
            String tokenEndpointUrl = "https://" + endpoint + "/common/oauth2/token";
            String clientid = "1b730954-1685-4b74-9bfd-dac224a7b894";
            string requestid = Guid.NewGuid().ToString();

            WriteVerbose("generating requestID " + requestid);

            String request = authorizationEndpointUrl + "?"
                                + "resource=" + resource + "&"
                                + "client_id=" + clientid + "&"
                                + "client-request-id=" + requestid + "&"
                                + "x-client-SKU=PCL.CoreCLR&"
                                + "x-client-Ver=3.19.2.6005";
            WriteVerbose("getGrantTypeCodeAuthentication: Request: " + request);

            try {
                deviceCode = JsonConvert.DeserializeObject<UserCodeObject>(
                    newGetRequest(client, request)
                );
                WriteVerbose("getDeviceCode: device code received.");
            }
            catch (Exception e)
            {
                WriteVerbose("getDeviceCode: " + e.Message);
                throw new Exception();
            }

            return deviceCode;
        }

        private String newGetRequest(HttpClient client, string request)
        {
            Task<string> p;
            Task<HttpResponseMessage> response = client.GetAsync(request);
            response.Wait();
            var r = response.Result;
            p = r.Content.ReadAsStringAsync();
            p.Wait();
            return p.Result;
        }
    }
}

