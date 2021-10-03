using Glass.Core.Database;
using Glass.Core.HTTP.Interfaces;
using Glass.Core.Repository;
using Glass.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebSocketSharp.Server;

namespace Glass.Core.HTTP.Builders {
    
    static class HTTPRouterBuilder {

        public static IHTTPRouter BuildHTTPRouter(HttpRequestEventArgs e) {
            string controllerName = GetControllerName(e);
            Type controllerType = Type.GetType($"Glass.Controllers.HTTP.{controllerName}");

            if(controllerType == null) {
                return null;
            }

            IHTTPRouter controller = (IHTTPRouter)Activator.CreateInstance(controllerType, new object[] { e });
            return controller;
        }

        public static IHTTPRouter BuildHTTPRouter(HttpRequestEventArgs e, Context c, HttpServer httpServer) {
            string controllerName = GetControllerName(e);
            Type controllerType = Type.GetType($"Glass.Controllers.HTTP.{controllerName}");

            if (controllerType == null) {
                return null;
            }

            IHTTPRouter controller = (IHTTPRouter)Activator.CreateInstance(controllerType, new object[] { e, c });
            if(controller is HTTPRouter) {
                HTTPRouter router = controller as HTTPRouter;

                if(!String.IsNullOrEmpty(router.WebSocketClass)) {
                    WebSocketServiceHost host = httpServer.WebSocketServices.Hosts.ToList().Find((x) => x.Type.Name == router.WebSocketClass);
                    if(host == null) {
                        throw new Exception($"Classe WebSocket {{{router.WebSocketClass}}} inexistente");
                    }
                    router.SetWebSocket(host);
                }
            }
            return controller;
        }

        private static string GetControllerName(HttpRequestEventArgs e) {
            string name = e.Request.Url.Segments[1].Split('/')[0];

            return $"{StringManipulation.ToUpperFirstLetter(name)}Controller";
        }

    }

}
