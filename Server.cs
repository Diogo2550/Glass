using System;
using System.Text;

using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using Glass.Controllers.WebSocket;
using Glass.Core.Database;
using Glass.Core.HTTP.Interfaces;
using Glass.Core.HTTP.Builders;
using Glass.Core.Repository;

namespace Glass {

    class Server {

        private HttpServer httpServer;
        private Context context;

        public Server(string url, int port, Context context) {
            string serverUrl = $"{url}:{port}/";

            httpServer = new HttpServer(serverUrl);
            this.context = context;
        }

        public void Start() {
            httpServer.OnPost += RequestHandler;
            httpServer.OnGet += RequestHandler;
            httpServer.OnDelete += RequestHandler;
            httpServer.OnPut += RequestHandler;

            httpServer.AddWebSocketService<ScheduleController>("/schedule", () => new ScheduleController(new ScheduleRepository(context)));

            httpServer.Start();
        }

        private void RequestHandler(object sender, HttpRequestEventArgs e) {
            Console.WriteLine("Requisição recebida...");
            e.Response.AddHeader("Access-Control-Allow-Origin", "*");
            e.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            e.Response.Headers.Add("ContentType", "application/json;charset=UTF-8");

            IHTTPRouter router = HTTPRouterBuilder.BuildHTTPRouter(e, context);
            if(router == null) {
                Console.WriteLine("A URL requisitada não existe");
                var response = new HTTPResponseBuilder(e.Response);
                response.SetStatusCode(404);
                response.SetError("A URL requisitada não existe");

                response.Reply();
                return;
            }

            router.Open();
        }

    }

}
