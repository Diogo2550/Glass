using Glass.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Core.Util {
    static class JWT {

        private static string type = "JWT";
        private static string algorithm = "HS256";
        private static int daysToExpire = 2;

        /** Cria um JWT a partir de um dado _payload_.
         * @param array $payload Um assoc contendo os dados a serem adicionados ao JWT.
         * @return string Um JWT completo.
         */
        public static string Create(int id, string name, bool isAdmin = false) {
            var payload = new Dictionary<string, object>();
            payload.Add("id", id);
            payload.Add("name", name);
            payload.Add("isAdmin", isAdmin);
            payload.Add("exp", DateTime.Now.Second + (daysToExpire * 24 * 60 * 60));

            var header = new Dictionary<string, object>();
            header.Add("alg", algorithm);
            header.Add("typ", type);

            var buildedHeader = Base64Encode(JsonSerialize(header));
            var buildedPayload = Base64Encode(JsonSerialize(payload));
            var signature = Base64Encode(BuildSignature(buildedHeader, buildedPayload));

            return BuildToken(buildedHeader, buildedPayload, signature);
        }

        /** Pega um dado a partir de um JWT. Funciona apenas para chaves em primeira instancia. */
        public static string GetTokenData(string token, string keyData) {
            var payload = token.Split('.')[1];
            var jsonString = Base64Decode(payload);

            var json = JObject.Parse(jsonString);

            return json.Value<string>(keyData);
        }

        /** Valida um JWT, informando se ele é válido ou se foi manipulado por entidades externas. */
        public static bool Validate(string token) {
            string[] tokenParts = token.Split('.');
            if(token.Length < 3) return false;

            string header = tokenParts[0];
            string payload = tokenParts[1];
            string trustedSignature = tokenParts[2];

            string signature = Base64Encode(BuildSignature(header, payload));

            if(trustedSignature == signature)
                return true;
            return false;
        }

        /** Verifica se um dado token está expirado ou se ainda é válido. */
        public static bool IsExpired(string token) {
            string validity = GetTokenData(token, "exp");
            if(validity == null) 
                throw new Exception("Foi tentado verificar a validade de um token sem o parâmetro exp.");
            return (DateTime.Now.Second > int.Parse(validity));
        }

        private static string JsonSerialize(Dictionary<string, object> header) {
            return JsonConvert.SerializeObject(header);
        }

        private static string BuildSignature(string header, string payload) {
            HashAlgorithm hasher = new HMACSHA256(Encoding.UTF8.GetBytes(Program.config.SelectToken("JWT").Value<string>("secret")));
            string valueToHash = $"{header}.{payload}";
            var signature = hasher.ComputeHash(Encoding.UTF8.GetBytes(valueToHash));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < signature.Length; i++) {
                sBuilder.Append(signature[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static string BuildToken(string header, string payload, string signature) {
            return $"{header}.{payload}.{signature}";
        }

        private static string Base64Encode(string value) {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            string base64string = Convert.ToBase64String(bytes);

            return base64string;
        }

        private static string Base64Decode(string value) {
            byte[] bytes = Convert.FromBase64String(value);

            return Encoding.UTF8.GetString(bytes);
        }

    }
}
