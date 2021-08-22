using System;
using Newtonsoft.Json;

namespace Glass.Models {
    class Person {

        [JsonProperty]
        public ushort Id { get; protected set; }
        [JsonProperty]
        public string Name { get; protected set; }
        [JsonProperty]
        public string CPF { get; protected set; }
        [JsonProperty]
        public string RG { get; protected set; }
        [JsonProperty]
        public string Phone { get; protected set; }
        [JsonProperty]
        public DateTime? Birthday { get; protected set; }

        public int GetAge() {
            if (Birthday.HasValue) {
                throw new ArgumentNullException("Não é possível saber a idade do funcionário pois o mesmo não possui data de nascimento!");
            }

            return (DateTime.Now.Subtract(TimeSpan.FromTicks(Birthday.Value.Ticks)).Year - 1);
        }

        public void SetId(ushort id) {
            Id = id;
        }

        public void SetName(string name) {
            Name = name;
        }

        public void SetCPF(string cpf) {
            CPF = cpf;
        }

        public void SetRG(string rg) {
            RG = rg;
        }

        public void SetPhone(string phone) {
            Phone = phone;
        }

        public void SetBirthday(DateTime birthday) {
            Birthday = birthday;
        }

    }
}
