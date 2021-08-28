using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace Glass.Core.HTTP {
    class HTTPRequestInfo {

        public string SenderAddress { get; private set; }
        public string Uri { get; private set; }
        public string Path { get; private set; }
        /**
         * O protocolo utilizado na requisição
         * example http
         */
        public string Scheme { get; private set; }
        public string RawUrl { get; private set; }
        public string HttpMethod { get; private set; }
        public string Controller { get; private set; }
        public string Method { get; private set; }

        private HttpListenerRequest request;
        private JObject body;

        public HTTPRequestInfo(HttpListenerRequest e) {
            request = e;
            SenderAddress = request.UrlReferrer.AbsoluteUri;
            Scheme = request.Url.Scheme;
            Uri = request.Url.AbsoluteUri;
            HttpMethod = request.HttpMethod;
            Path = request.Url.AbsolutePath;
            Controller = Path.Split('/')[1];
            Method = Path.Split('/')[2];

            body = GetRequestBody();
        }

        public HttpListenerRequest GetListenerRequest() {
            return request;
        }

        public string GetQueryValue(string key) {
            return request.QueryString.GetValues(key)[0];
        }
        
        public JObject GetBodyJson() {
            return body;
        }

        public T GetValue<T>(string propertyName) {
            return body.Value<T>(propertyName);
        }

        private JObject GetRequestBody() {
            if (!request.HasEntityBody) return null;
            Stream body = request.InputStream;
            StreamReader reader = new StreamReader(body);

            string jsonText = reader.ReadToEnd();
            return JObject.Parse(jsonText);
        }

    }
}
