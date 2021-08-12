using Glass.Core.Database;
using Glass.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Core.Repository {
    class ScheduleRepository {

        private Context context;

        public ScheduleRepository(Context context) {
            this.context = context;
        }

        public List<Schedule> GetSchedulesFromEmployee(ushort employeeId, ushort month) {
            var schedules = new List<Schedule>();

            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT id,dayOfWeek,startTime,endTime,frequency FROM Schedule WHERE employeeId=@employeeId";
                command.Parameters.AddWithValue("@employeeId", employeeId);

                using (var reader = command.ExecuteReader()) {
                    while (reader.HasRows) {
                        while (reader.Read()) {
                            Schedule s = new Schedule();
                            s.SetId(reader.GetUInt16(0));
                            s.SetDayOfWeek(reader.GetUInt16(1));
                            s.SetStartTime(reader.GetTimeSpan(2));
                            s.SetEndTime(reader.GetTimeSpan(3));
                            s.SetFrequency(reader.GetUInt16(4));

                            schedules.Add(s);
                        }

                        reader.NextResult();
                    }
                }
            }
            
            return schedules;
        }
         
    }
}
