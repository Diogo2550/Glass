using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Errors:
// 100 - Erros do cliente
// 200
// 300
// 400
// 500

namespace Glass.Core.WebSocket.Builders {
    class WebSocketResponseBuilder {

        public bool Success { get; private set; }
        private JObject replyMessage;

        public WebSocketResponseBuilder() {
            replyMessage = new JObject();

            Success = true;
            replyMessage["success"] = JToken.FromObject(true);
            replyMessage["code"] = JToken.FromObject(200);
        }

        public void SetStatusCode(int code) {
            replyMessage["code"] = JToken.FromObject(code);
        }

        public void SetError(string message) {
            Success = false;
            replyMessage["success"] = JToken.FromObject(false);
            replyMessage["error"] = JToken.FromObject(message);
        }

        public void SetMethod(string method) {
            replyMessage["method"] = JToken.FromObject(method);
        }

        public void SetData(object data) {
            replyMessage["data"] = JToken.FromObject(data);
        }

        public string GetResponse() {
            if (replyMessage == null) {
                throw new Exception("Tentativa de responder uma mensagem inválida encontrada.");
            }
            return replyMessage.ToString();
        }

    }
}
