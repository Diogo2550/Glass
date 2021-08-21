using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Core.Util {
    class TimeSpanConverter : JsonConverter<TimeSpan> {

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer) {
            Console.WriteLine();
            Console.WriteLine(reader.Value);
            Console.WriteLine();
            long ticks = (long)reader.Value;

            TimeSpan time = new TimeSpan(ticks);
            return time;
        }

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer) {
            writer.WriteValue(value.Ticks);
        }

    }
}
