using MySqlConnector;
using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Glass.Core.Database {
    public class Context {
                
        private MySqlConnection connection;
        private MySqlConnectionStringBuilder builder;

        public Context(string host, string database, string user, string password) {
            builder = new MySqlConnectionStringBuilder {
                Server = host,
                Database = database,
                UserID = user,
                Password = password,
                AllowZeroDateTime = true,
            };

            try {
                connection = new MySqlConnection(builder.ConnectionString);
                connection.Dispose();
            } catch (MySqlException) {
                throw new ExternalException("Erro ao inicar o MySQL. Verifique se as informações no Config.json estão corretas e tenha certeza de que o processo do mysql foi iniciado.");
            }
        }

        public string GetConnectionString() {
            return builder.ConnectionString;
        }

    }
}
