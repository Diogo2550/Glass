using MySqlConnector;
using System;
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
            } catch (MySqlException) {
                throw new Exception("Erro ao inicar o MySQL. Verifique se as informações no Config.json estão corretas.");
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
