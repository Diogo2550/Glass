using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Errors:
// 100 - Respostas informativas
// 200 - Respostas de sucesso
// 300 - 
// 400 - Erros do cliente
// 500 - Erros do servidor

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
        
        public void SetComponentId(string componentId) {
            replyMessage["componentId"] = JToken.FromObject(componentId);
        }

        public void SetData(object data) {
            replyMessage["data"] = JToken.FromObject(data, new JsonSerializer() {
                ContractResolver = new DefaultContractResolver() {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
        }

        public string GetResponse() {
            return replyMessage.ToString();
        }

    }
}
