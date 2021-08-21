using Glass.Core.Exceptions;
using Glass.Models.Abstracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glass.Core.Util {
    static class RequestObjectFactory {
    
        public static Schedule BuildSchedule(JToken scheduleObject) {
            Schedule schedule = new Schedule();

            var id = scheduleObject.Value<string>("id");
            var startTime = scheduleObject.Value<string>("startTime");
            var endTime = scheduleObject.Value<string>("endTime");
            var frequency = scheduleObject.Value<string>("frequency");
            var dayOfWeek = scheduleObject.Value<string>("dayOfWeek");

            try {
                if (RegexValidator.IsNumber(startTime))
                    schedule.SetStartTime(new TimeSpan(long.Parse(startTime)));
                else
                    schedule.SetStartTime(TimeSpan.Parse(startTime));

                if (RegexValidator.IsNumber(endTime))
                    schedule.SetEndTime(new TimeSpan(long.Parse(endTime)));
                else
                    schedule.SetEndTime(TimeSpan.Parse(endTime));

                if(RegexValidator.IsNumber(id))
                    schedule.SetId(ushort.Parse(id));

                schedule.SetFrequency(ushort.Parse(frequency));
                schedule.SetDayOfWeek(ushort.Parse(dayOfWeek));
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return schedule;
        }
        
    }
}
