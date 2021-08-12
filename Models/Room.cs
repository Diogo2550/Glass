using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models {
    class Room {

        [JsonProperty]
        public ushort Id { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }
    
        public void SetId(ushort id) {
            Id = id;
        }

        public void SetName(string name) {
            Name = name;
        }

    }
}
