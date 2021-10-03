using Newtonsoft.Json;
using System;

using Glass.Core.Enums;
using System.Collections.Generic;

namespace Glass.Models {
    class Appointment {

        [JsonProperty]
        public ushort Id { get; private set; }
        [JsonProperty]
        public DateTime AppointmentDate { get; private set; }
        [JsonProperty]
        public string AppointmentType { get; private set; }
        public Patient Patient { get; set; }
        public Room Room { get; set; }
        public Professional Professional { get; set; }

        public Appointment() {
            Room = new Room();
            Professional = new Professional();
            Patient = new Patient();
        }

        public void SetId(ushort id) {
            Id = id;
        }

        public void SetAppointmentDate(DateTime appointmentDate) {
            AppointmentDate = appointmentDate;
        }

        public void SetAppointmentType(string appointmentType) {
            AppointmentType = appointmentType;
        }

    }
}
