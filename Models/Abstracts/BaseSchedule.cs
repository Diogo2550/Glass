using Glass.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glass.Models.Abstracts {
    abstract class BaseSchedule {

        [JsonProperty]
        public ushort Id { get; protected set; }
        [JsonProperty]
        public TimeSpan StartTime { get; protected set; }
        [JsonProperty]
        public TimeSpan EndTime { get; protected set; }
        [JsonProperty]
        public AppointmentFrequency Frequency { get; protected set; }
        public Employee employee;

        public BaseSchedule() {
            employee = new Professional();
        }

        public void SetId(ushort id) {
            Id = id;
        }

        public void SetStartTime(TimeSpan startTime) {
            StartTime = startTime;
        }

        public void SetEndTime(TimeSpan endTime) {
            EndTime = endTime;
        }

        public void SetFrequency(AppointmentFrequency frequency) {
            Frequency = frequency;
        }

        public void SetFrequency(ushort frequency) {
            Frequency = (AppointmentFrequency)frequency;
        }

    }
}
