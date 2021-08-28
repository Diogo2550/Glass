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
                Password = password
            };

            try {
                connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();
            } catch (MySqlException) {
                throw new ExternalException("Erro ao inicar o MySQL. Verifique se as informações no Config.json estão corretas e tenha certeza de que o processo do mysql foi iniciado.");
            }
        }

        public MySqlCommand GetCommand() {
            if (connection.State == System.Data.ConnectionState.Closed) {
                connection.Open();
            }
            return connection.CreateCommand();
        }

        public async Task<MySqlCommand> GetCommandAsync() {
            if (connection.State == System.Data.ConnectionState.Closed) {
                await connection.OpenAsync();
            }
            return connection.CreateCommand();
        }

    }
}
