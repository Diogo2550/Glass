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
        public AppointmentType AppointmentType { get; private set; }
        public List<Patient> Patient { get; set; }
        public List<Room> Room { get; set; }

        public Appointment() {
            Patient = new List<Patient>();
            Room = new List<Room>();
        }

        public void SetId(ushort id) {
            Id = id;
        }

        public void SetAppointmentDate(DateTime appointmentDate) {
            AppointmentDate = appointmentDate;
        }

        public void SetAppointmentType(AppointmentType appointmentType) {
            AppointmentType = appointmentType;
        }

        public void SetAppointmentType(string appointmentType) {
            AppointmentType = (AppointmentType)Enum.Parse(AppointmentType.GetType(), appointmentType);
        }

    }
}
