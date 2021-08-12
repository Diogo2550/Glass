using Glass.Core.Database;
using Glass.Core.HTTP.Interfaces;
using Glass.Core.Util;
using System;
using System.Collections.Generic;
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

        public static IHTTPRouter BuildHTTPRouter(HttpRequestEventArgs e, Context context) {
            string controllerName = GetControllerName(e);
            Type controllerType = Type.GetType($"Glass.Controllers.HTTP.{controllerName}");

            if (controllerType == null) {
                return null;
            }

            IHTTPRouter controller = (IHTTPRouter)Activator.CreateInstance(controllerType, new object[] { e, context });
            return controller;
        }

        private static string GetControllerName(HttpRequestEventArgs e) {
            string name = e.Request.Url.Segments[1].Split('/')[0];

            return $"{StringManipulation.ToUpperFirstLetter(name)}Controller";
        }

    }

}
