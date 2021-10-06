using System;
using System.Text;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using MySqlConnector;
using Glass.Models;
using Glass.Core.Database.Builders;
using Glass.Core.HTTP;
using Glass.Core.Database;
using System.Security.Cryptography;
using Glass.Core.Util;

namespace Glass.Controllers.HTTP {
    
    class UserController : HTTPRouter {

        public UserController(HttpRequestEventArgs e, Context c) : base(e, c) { }

        public void Login() {
            JObject body = request.GetBodyJson();

            string cpf = body.Value<string>("cpf");
            string password = body.Value<string>("password");

            using (var conn = new MySqlConnection(context.GetConnectionString())) {
                conn.Open();
                using (var command = conn.CreateCommand()) {
                    command.CommandText = "SELECT password,id,name,admin FROM Employee WHERE cpf=@cpf LIMIT 1";
                    command.Parameters.AddWithValue("@cpf", cpf);

                    using (var reader = command.ExecuteReader()) {
                        string userPassword = null, name = null;
                        bool isAdmin = false;
                        int id = 0;
                        if (reader.HasRows) {
                            reader.Read();
                            userPassword = reader.GetFieldValue<string>(0);
                            id = reader.GetFieldValue<int>(1);
                            name = reader.GetFieldValue<string>(2);
                            isAdmin = reader.GetFieldValue<bool>(3);

                            reader.NextResult();
                        }

                        var hashBuilder = MD5.Create();
                        byte[] hash = hashBuilder.ComputeHash(Encoding.UTF8.GetBytes(password));

                        StringBuilder sBuilder = new StringBuilder();
                        for (int i = 0; i < hash.Length; i++) {
                            sBuilder.Append(hash[i].ToString("x2"));
                        }

                        if (userPassword == null || userPassword != sBuilder.ToString()) {
                            response.SetError("Usuário ou senha inválidos!");
                            response.Reply();
                            return;
                        }

                        var data = new {
                            message = "Usuário logado com sucesso!",
                            token = JWT.Create(id, name, isAdmin)
                        };
                        response.SetData(data);
                        response.Reply();
                    }
                }
            }
        }

        public void Signup() {
            JObject body = request.GetBodyJson();
            Employee employee = null;

            bool admin = request.GetValue<bool>("admin");
            if(admin) employee = body.ToObject<Admin>();
            else      employee = body.ToObject<Professional>();

            MySQLErrorBuilder errorBuilder = new MySQLErrorBuilder();
            if (employee.Name.IsNullOrEmpty()) errorBuilder.AddProperty("Nome", null);
            if (employee.CPF.IsNullOrEmpty()) errorBuilder.AddProperty("CPF", null);
            if (!employee.Birthday.HasValue) errorBuilder.AddProperty("Dt. Nascimento", null);
            if (employee.Password.IsNullOrEmpty()) errorBuilder.AddProperty("Senha", null);

            if(errorBuilder.GetPropertyAmount() > 0) {
                response.SetError(errorBuilder.GetEmptyPropertiesError());

                response.Reply();
                return;
            }

            using (var conn = new MySqlConnection(context.GetConnectionString())) {
                conn.Open();
                using (var command = conn.CreateCommand()) {
                    command.CommandText = "INSERT INTO employee VALUES(DEFAULT, @name, @cpf, @rg, @birthday, @phone, MD5(@password), @admin)";
                    command.Parameters.AddWithValue("@name", employee.Name);
                    command.Parameters.AddWithValue("@cpf", employee.CPF);
                    command.Parameters.AddWithValue("@rg", employee.RG);
                    command.Parameters.AddWithValue("@phone", employee.Phone);
                    command.Parameters.AddWithValue("@birthday", employee.Birthday);
                    command.Parameters.AddWithValue("@password", employee.Password);
                    command.Parameters.AddWithValue("@admin", employee.IsAdmin());

                    try {
                        command.ExecuteNonQuery();
                    } catch (MySqlException ex) {
                        if (ex.Number == 1062) {
                            response.SetError("Não foi possível cadastrar o usuário. CPF já existente em nossa base de dados.");
                        } else {
                            response.SetStatusCode(500);
                            response.SetError(ex.Message);
                        }
                    }
                }
                context.CloseConnection();
                response.Reply();
            }
        }

    }

}
