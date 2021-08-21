using Glass.Core.Database;
using Glass.Models;
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

        public List<Schedule> GetSchedulesFromEmployee(ushort employeeId) {
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

        public List<EventualSchedule> GetMonthlyEventualSchedulesFromEmployee(ushort employeeId, ushort month, int year) {
            var eventualSchedules = new List<EventualSchedule>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = (startDate.AddMonths(1)).AddDays(-1);
            using (var command = context.GetCommand()) {
                command.CommandText = @"SELECT id,eventualDate,startTime,endTime,frequency,eventualState FROM EventualSchedule WHERE employeeId=@employeeId AND eventualDate BETWEEN @monthStart AND @monthEnd";
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("monthStart", startDate);
                command.Parameters.AddWithValue("monthEnd", endDate);
                using (var reader = command.ExecuteReader()) {
                    while (reader.HasRows) {
                        while (reader.Read()) {
                            EventualSchedule es = new EventualSchedule();
                            es.SetId(reader.GetUInt16(0));
                            es.SetEventualDate(reader.GetDateTime(1));
                            es.SetStartTime(reader.GetTimeSpan(2));
                            es.SetEndTime(reader.GetTimeSpan(3));
                            es.SetFrequency(reader.GetUInt16(4));
                            es.SetEventualState(reader.GetUInt16(5));

                            eventualSchedules.Add(es);
                        }
                        reader.NextResult();
                    }
                }
            }

            return eventualSchedules;
        }

        public List<Appointment> GetMonthlyAppointmentsFromEmployee(ushort employeeId, ushort month, int year) {
            var appointments = new List<Appointment>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = (startDate.AddMonths(1)).AddDays(-1);
            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT id,appointmentDate,appointmentType FROM Appointment WHERE employeeId=@employeeId AND appointmentDate BETWEEN @monthStart AND @monthEnd";
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("monthStart", startDate);
                command.Parameters.AddWithValue("monthEnd", endDate);
                using (var reader = command.ExecuteReader()) {
                    while (reader.HasRows) {
                        while (reader.Read()) {
                            Appointment a = new Appointment();
                            a.SetId(reader.GetUInt16(0));
                            a.SetAppointmentDate(reader.GetDateTime(1));
                            a.SetAppointmentType(reader.GetString(2));

                            appointments.Add(a);
                        }
                        reader.NextResult();
                    }
                }
            }

            return appointments;
        }

        public List<Professional> GetAllProfessionals() {
            var professionals = new List<Professional>();
            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT id, name FROM Employee WHERE admin=0";

                using (var reader = command.ExecuteReader()) {
                    while (reader.HasRows) {
                        reader.Read();

                        Professional p = new Professional();
                        p.SetName(reader.GetString(1));
                        p.SetId(reader.GetUInt16(0));

                        professionals.Add(p);
                        reader.NextResult();
                    }
                }
            }

            return professionals;
        }
        
        public bool AddScheduleToEmployee(ushort employeeId, Schedule schedule) {
            using(var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Schedule VALUES (DEFAULT, @day, @start, @end, @frequency, @employeeId)";
                command.Parameters.AddWithValue("@day", schedule.DayOfWeek);
                command.Parameters.AddWithValue("@start", schedule.StartTime);
                command.Parameters.AddWithValue("@end", schedule.EndTime);
                command.Parameters.AddWithValue("@frequency", schedule.Frequency);
                command.Parameters.AddWithValue("@employeeId", employeeId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool AddEventualScheduleToEmployee(ushort employeeId, EventualSchedule eventualSchedule) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO EventualSchedule VALUES(DEFAULT, @eventualDate, @start, @end, @freq, @state, @employeeId)";
                command.Parameters.AddWithValue("@eventualDate", eventualSchedule.EventualState);
                command.Parameters.AddWithValue("@start", eventualSchedule.StartTime);
                command.Parameters.AddWithValue("@end", eventualSchedule.EndTime);
                command.Parameters.AddWithValue("@freq", eventualSchedule.Frequency);
                command.Parameters.AddWithValue("@state", eventualSchedule.EventualState);
                command.Parameters.AddWithValue("@employeeId", employeeId);

                int rows = command.ExecuteNonQuery();
                
                return (rows > 0) ? true : false;
            }
        }

        public bool AddAppointmentToEmployee(ushort employeeId, ushort roomId, ushort patientId, Appointment appointment) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Appointment VALUES(DEFAULT, @date, @type, @employee, @patient, @room)";
                command.Parameters.AddWithValue("@date", appointment.AppointmentDate);
                command.Parameters.AddWithValue("@type", appointment.AppointmentType);
                command.Parameters.AddWithValue("@employee", employeeId);
                command.Parameters.AddWithValue("@patient", patientId);
                command.Parameters.AddWithValue("@room", roomId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }
    
        public bool DeleteSchedule(ushort scheduleId) {
            using(var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Schedule WHERE id=@id";
                command.Parameters.AddWithValue("@id", scheduleId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool UpdateSchedule(Schedule schedule) {
            using (var command = context.GetCommand()) {
                command.CommandText = "UPDATE Schedule SET dayOfWeek=@day, startTime=@start, endTime=@end, frequency=@freq WHERE id=@id";
                command.Parameters.AddWithValue("@day", schedule.DayOfWeek);
                command.Parameters.AddWithValue("@start", schedule.StartTime);
                command.Parameters.AddWithValue("@end", schedule.EndTime);
                command.Parameters.AddWithValue("@freq", schedule.Frequency);
                command.Parameters.AddWithValue("@id", schedule.Id);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

    }
}
