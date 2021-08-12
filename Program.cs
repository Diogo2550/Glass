using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace Glass {
    class Program {

        static readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        static JObject config;

        static void Main(string[] args) {
            GetConfigData();

            Server server = new Server(config);
            server.Start();
            
            Console.ReadLine();
        }

        static void GetConfigData() {
            if(!File.Exists($"{path}/Config.json")) {
                throw new FileNotFoundException("Arquivo \"Config.json\" não existe");
            }

            FileStream file = new FileStream($"{path}/Config.json", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);

            string json = "";
            while(!reader.EndOfStream) {
                json += reader.ReadLine();
            }

            config = JObject.Parse(json);
        }

    }
}
