using Glass.Core.WebSocket.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Core.Exceptions {
    class InvalidRequestArgument : Exception {

        public WebSocketResponseBuilder response;
        public override string Message => "Parâmetros recebidos inválidos.";

        public InvalidRequestArgument() {
            response = new WebSocketResponseBuilder();
            response.SetStatusCode(101);
            response.SetError(Message);
        }

    }
}
