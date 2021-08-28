using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

using WebSocketSharp.Server;
using WebSocketSharp.Net;

using Glass.Core.HTTP.Interfaces;
using Glass.Core.Database;
using Glass.Core.Util;
using Glass.Core.HTTP.Builders;
using System;

namespace Glass.Core.HTTP {
    abstract class HTTPRouter : IHTTPRouter {

        protected HTTPRequestInfo request;
        protected HTTPResponseBuilder response;
        protected Context context;


        public HTTPRouter(HttpRequestEventArgs e, Context context) {
            request = new HTTPRequestInfo(e.Request);
            response = new HTTPResponseBuilder(e.Response);

            this.context = context;
        }

        public void Open() {
            string methodName = request.Method;
            methodName = StringManipulation.ToUpperFirstLetter(methodName);
            
            try {
                GetType()
                    .InvokeMember(methodName, BindingFlags.InvokeMethod, null, this, null);
            } catch(MissingMethodException) {
                response.SetStatusCode(404);
                response.SetError($"Não existe o método {methodName} na rota inserida.");
                response.Reply();
            }
        }

        public virtual bool AcceptGet() { return true; }
        public virtual bool AcceptPost() { return true; }
        public virtual bool AcceptDelete() { return true; }
        public virtual bool AcceptPut() { return true; }

    }
}
