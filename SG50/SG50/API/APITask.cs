using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace SG50
{
    class APITask
    {
        public String API = "";
        public static String API_TEMPLATE = "http://sg50-private-api.herokuapp.com/api/";

        public APITask(String API)
        {
            this.API = API;
        }

        public class APIEventHandlerArgs : EventArgs
        {
            public String ErrorMessage = "";
            public String Result = "";
        }

        public IRestResponse Call(APIArgs args)
        {
            var client = new RestClient(API_TEMPLATE);
            return client.Execute(GetRequest(args));
        }

        public Task<IRestResponse> CallAsync(APIArgs args)
        {
            var client = new RestClient(API_TEMPLATE);

            var request = GetRequest(args);

            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException != null)
                    tcs.TrySetException(response.ErrorException);
                else
                    tcs.TrySetResult(response);
            });

            return tcs.Task;
        }

        private RestRequest GetRequest(APIArgs args)
        {
            var request = new RestRequest(String.Format("{0}/?{1}", API, Guid.NewGuid().ToString().Replace("-", "")), Method.POST);



            foreach (String key in args.Headers.Keys)
            {
                request.AddHeader(key, args.Headers[key].ToString());
            }

            if (true)
            {
                request.AddParameter("accesstoken", "f3f82d07dad109d74a1229f8a79c49e7");
            }

            foreach (String key in args.Parameters.Keys)
            {
                request.AddParameter(key, args.Parameters[key]);
            }

            return request;
        }
    }
}