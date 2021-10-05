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
            } catch (MySqlException) {
                throw new ExternalException("Erro ao inicar o MySQL. Verifique se as informações no Config.json estão corretas e tenha certeza de que o processo do mysql foi iniciado.");
            }
        }

        public MySqlConnection GetConnection() {
            return connection;
        }

        public MySqlCommand GetCommand() {
            if (connection.State == System.Data.ConnectionState.Closed) {
                throw new Exception("Tentativa de acessar o banco sem iniciar a conexão.");
            }
            return connection.CreateCommand();
        }

    }
}
