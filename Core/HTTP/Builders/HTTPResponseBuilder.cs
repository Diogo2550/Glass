using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Glass.Core.HTTP.Builders {
    class HTTPResponseBuilder {

        private HttpListenerResponse response;
        private JObject replyMessage;

        public HTTPResponseBuilder(HttpListenerResponse response) {
            this.response = response;

            replyMessage = new JObject();
            replyMessage["success"] = JToken.FromObject(true);
            replyMessage["code"] = JToken.FromObject(200);
        }

        public HttpListenerResponse GetListenerResponse() {
            return response;
        }

        public void SetStatusCode(int code) {
            replyMessage["code"] = JToken.FromObject(code);
        }

        public void SetError(string message) {
            replyMessage["success"] = JToken.FromObject(false);
            replyMessage["error"] = JToken.FromObject(message);
        }

        public void SetData(object data) {
            replyMessage["data"] = JToken.FromObject(data);
        }

        public void Reply() {
            byte[] buffer = Encoding.UTF8.GetBytes(replyMessage.ToString());
            
            response.StatusCode = (int)replyMessage["code"];
            response.ContentLength64 = buffer.Length;

            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void ReplyAsync() {
            byte[] buffer = Encoding.UTF8.GetBytes(replyMessage.ToString());

            response.StatusCode = (int)replyMessage["code"];
            response.ContentLength64 = buffer.Length;

            response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

    }
}
