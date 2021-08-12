using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models {
    class Record {

        [JsonProperty]
        public ushort Id { get; private set; }
        [JsonProperty]
        public string Allergies { get; private set; }
        [JsonProperty]
        public string Annotations { get; private set; }

        public void SetId(ushort id) {
            Id = id;
        }

        public void SetAllergies(string allergies) {
            Allergies = allergies;
        }

        public void SetAnnotations(string annotations) {
            Annotations = annotations;
        }

    }
}
