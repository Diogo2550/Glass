using Newtonsoft.Json;
using System;

using Glass.Core.Enums;

namespace Glass.Models {
    class Appointment {

        [JsonProperty]
        public ushort Id { get; private set; }
        [JsonProperty]
        public DateTime AppointmentDate { get; private set; }
        [JsonProperty]
        public AppointmentType AppointmentType { get; private set; }

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
