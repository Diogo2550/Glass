using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Text;
using Glass.Core.Database;
using MySqlConnector;
using System.Runtime.InteropServices;

namespace Glass {
    class Program {

        public static JObject config;
        static readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        static void Main(string[] args) {
            try {
                GetConfigData();
            } catch (FileNotFoundException ex) {
                Console.WriteLine(ex.Message);
                return;
            }

            string url = config.SelectToken("connection.url").Value<string>();
            int port = config.SelectToken("connection.port").Value<int>();

            string host = config.SelectToken("database.host").Value<string>();
            string user = config.SelectToken("database.user").Value<string>();
            string database = config.SelectToken("database.database").Value<string>();
            string password = config.SelectToken("database.password").Value<string>();

            Context context = null;
            try {
                context = new Context(host, database, user, password);
            } catch (ExternalException e) {
                Console.WriteLine(e.Message);
                return;
            }

            Server server = new Server(url, port, context);
            server.Start();

            Console.WriteLine("Servidor iniciado! Aguardando requisições...");
            Console.ReadLine();
        }

        static void GetConfigData() {
            string filePath = $"{path}/Config.json";
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("Arquivo \"Config.json\" não existe. Crie um arquivo de configuração adequado para utilizar a aplicação.");
            }
            
            using(FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                using (StreamReader reader = new StreamReader(file)) {
                    StringBuilder builder = new StringBuilder();
                    while (!reader.EndOfStream) {
                        builder.Append(reader.ReadLine());
                    }

                    config = JObject.Parse(builder.ToString());
                }
            }
        }

    }
}
