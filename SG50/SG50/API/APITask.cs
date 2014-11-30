using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Android.Content;
using Java.IO;

namespace SG50
{
    class APITask
    {
        public String API = "";
        public static String API_TEMPLATE = "http://sg50-private-api.herokuapp.com";
        public delegate void SuccessDelegate(IRestResponse response);
        public delegate void ErrorDelegate(IRestResponse response);

        public APITask(String API)
        {
            this.API = API;
        }

        public IRestResponse Call(APIArgs args, Method method = Method.POST)
        {
            var client = new RestClient(API_TEMPLATE);
            return client.Execute(GetRequest(args, method));
        }

        public void CallAsync(APIArgs args, SuccessDelegate OnSuccess, ErrorDelegate OnError, Method method = Method.POST)
        {
            var client = new RestClient(API_TEMPLATE);
            var request = GetRequest(args, method);
            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException != null)
                {
                    if (OnError != null)
                    {
                        OnError(response);
                    }
                }
                else
                {
                    if (OnSuccess != null)
                    {
                        OnSuccess(response);
                    }
                }
            });
        }

        private RestRequest GetRequest(APIArgs args, Method method)
        {
            var request = new RestRequest(API, method);

            foreach (String key in args.Headers.Keys)
            {
                request.AddHeader(key, args.Headers[key].ToString());
            }

           string accesstoken = MainActivity.GetAccessToken();

            if (accesstoken != null)
            {
                request.AddHeader("X-Accesstoken", accesstoken);
            }

            request.AddParameter("key", Guid.NewGuid().ToString().Replace("-", ""), ParameterType.QueryString);

            foreach (String key in args.Parameters.Keys)
            {
                request.AddParameter(key, args.Parameters[key], ParameterType.GetOrPost);
            }

            foreach (String key in args.Files.Keys)
            {
                File file = new File(args.Files[key]);
                byte[] data = new byte[(int)file.Length()];
                try
                {
                    new FileInputStream(file).Read(data);
                    request.AddFile(key, data, file.Name);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error: " + e.Message + " | " + e.StackTrace);
                }

            }

            return request;
        }
    }
}