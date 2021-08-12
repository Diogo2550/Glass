using MySqlConnector;
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

            connection = new MySqlConnection(builder.ConnectionString);
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
