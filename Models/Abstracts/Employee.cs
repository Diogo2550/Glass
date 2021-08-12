using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models
{
    abstract class Employee : Person
    {
        
        [JsonProperty]
        protected bool admin = false;

        public bool IsAdmin() { return admin; }

        public string GetToken() {
            return (CPF + RG);
        }

        public bool VerifyToken(string token) {
            if(token == GetToken()) {
                return true;
            }
            return false;
        }


        public override string ToString()
        {
            return String.Format("Employee:\n{0} {1} {2}", Id, Name, CPF);
        }

    }
}
