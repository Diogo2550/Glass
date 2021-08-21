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

        private JObject config;
        private HttpServer server;
        private Context context;

        public Server(JObject config) {
            this.config = config;
            
            InitServer();
        }

        public void Start() {
            server.OnPost += RequestHandler;
            server.OnGet += RequestHandler;
            server.OnDelete += RequestHandler;
            server.OnPut += RequestHandler;

            server.AddWebSocketService<ScheduleController>("/schedule", () => new ScheduleController(new ScheduleRepository(context)));
            
            server.Start();
            Console.WriteLine("Servidor iniciado! Aguardando requisições...");
        }

        private void InitServer() {
            string url = config.SelectToken("connection.url").Value<string>();
            int port = config.SelectToken("connection.port").Value<int>();

            string host = config.SelectToken("database.host").Value<string>();
            string user = config.SelectToken("database.user").Value<string>();
            string database = config.SelectToken("database.database").Value<string>();
            string password = config.SelectToken("database.password").Value<string>();

            server = new HttpServer($"{url}:{port}/");
            context = new Context(host, database, user, password);
        }

        private void RequestHandler(object sender, HttpRequestEventArgs e) {
            Console.WriteLine("Requisição recebida...");
            e.Response.AddHeader("Access-Control-Allow-Origin", "*");
            e.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            e.Response.Headers.Add("ContentType", "application/json;charset=UTF-8");

            IHTTPRouter router = HTTPRouterBuilder.BuildHTTPRouter(e, context);
            if(router == null) {
                Console.WriteLine("URL Inválida inserida");
                var response = new HTTPResponseBuilder(e.Response);
                response.SetStatusCode(404);
                response.SetError("Url inválida");

                response.Reply();
                return;
            }

            router.Open();
        }

    }

}
