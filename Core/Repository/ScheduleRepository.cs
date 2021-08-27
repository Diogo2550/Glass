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

        public Patient GetPatientById(ushort patientId) {
            Patient patient = new Patient();

            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT * FROM Patient WHERE id=@id";
                command.Parameters.AddWithValue("@id", patientId);

                using (var reader = command.ExecuteReader()) {
                    if(reader.HasRows) {
                        reader.Read();

                        patient.SetId(reader.GetUInt16("id"));
                        patient.SetCPF(reader.GetString("cpf"));
                        patient.SetBirthday(reader.GetDateTime("birthday"));
                        patient.SetName(reader.GetString("fullName"));
                        patient.SetPhone(reader.GetString("phone"));
                        patient.SetRG(reader.GetString("rg"));
                    }
                }
            }

            return patient;
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
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT a.id, a.appointmentDate, a.appointmentType, p.id, p.fullName, r.id, r.name");
                builder.Append(" FROM Appointment as a");
                builder.Append(" INNER JOIN Patient p ON p.id = a.patientId");
                builder.Append(" INNER JOIN Room r ON r.id = a.roomId");
                builder.Append(" WHERE employeeId=@employeeId AND appointmentDate BETWEEN @monthStart AND @monthEnd");
                
                command.CommandText = builder.ToString();

                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("@monthStart", startDate);
                command.Parameters.AddWithValue("@monthEnd", endDate);
                using (var reader = command.ExecuteReader()) {
                    while (reader.HasRows) {
                        while (reader.Read()) {
                            Appointment a = new Appointment();
                            a.SetId(reader.GetUInt16(0));
                            a.SetAppointmentDate(reader.GetDateTime(1));
                            a.SetAppointmentType(reader.GetString(2));
                            a.Patient.SetId(reader.GetUInt16(3));
                            a.Patient.SetName(reader.GetString(4));
                            a.Room.SetId(reader.GetUInt16(5));
                            a.Room.SetName(reader.GetString(6));

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
        
        public int AddScheduleToEmployee(ushort employeeId, Schedule schedule) {
            using(var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Schedule VALUES (DEFAULT, @day, @start, @end, @frequency, @employeeId)";
                command.Parameters.AddWithValue("@day", schedule.DayOfWeek);
                command.Parameters.AddWithValue("@start", schedule.StartTime);
                command.Parameters.AddWithValue("@end", schedule.EndTime);
                command.Parameters.AddWithValue("@frequency", schedule.Frequency);
                command.Parameters.AddWithValue("@employeeId", employeeId);

                int rows = command.ExecuteNonQuery();
                schedule.SetId((ushort)command.LastInsertedId);

                return (rows > 0) ? (ushort)command.LastInsertedId : -1;
            }
        }

        public int AddEventualScheduleToEmployee(ushort employeeId, EventualSchedule eventualSchedule) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO EventualSchedule VALUES(DEFAULT, @eventualDate, @start, @end, @freq, @state, @employeeId)";
                command.Parameters.AddWithValue("@eventualDate", eventualSchedule.EventualState);
                command.Parameters.AddWithValue("@start", eventualSchedule.StartTime);
                command.Parameters.AddWithValue("@end", eventualSchedule.EndTime);
                command.Parameters.AddWithValue("@freq", eventualSchedule.Frequency);
                command.Parameters.AddWithValue("@state", eventualSchedule.EventualState);
                command.Parameters.AddWithValue("@employeeId", employeeId);

                int rows = command.ExecuteNonQuery();
                
                return (rows > 0) ? (short)command.LastInsertedId : -1;
            }
        }

        public int AddAppointmentToEmployee(ushort employeeId, ushort roomId, ushort patientId, Appointment appointment) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Appointment VALUES(DEFAULT, @date, @type, @employee, @patient, @room)";
                command.Parameters.AddWithValue("@date", appointment.AppointmentDate);
                command.Parameters.AddWithValue("@type", appointment.AppointmentType.ToString());
                command.Parameters.AddWithValue("@employee", employeeId);
                command.Parameters.AddWithValue("@patient", patientId);
                command.Parameters.AddWithValue("@room", roomId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? (short)command.LastInsertedId : -1;
            }
        }

        public int AddEmployee(Employee employee) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Employee VALUES(DEFAULT, @name, @cpf, @rg, @birth, @phone, @password, @admin)";
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@cpf", employee.CPF);
                command.Parameters.AddWithValue("@rg", employee.RG);
                command.Parameters.AddWithValue("@birth", employee.Birthday);
                command.Parameters.AddWithValue("@phone", employee.Phone);
                command.Parameters.AddWithValue("@password", employee.Password);
                command.Parameters.AddWithValue("@admin", employee.IsAdmin());

                int rows = command.ExecuteNonQuery();
                
                return (rows > 0) ? (short)command.LastInsertedId : -1;
            }
        }

        public int AddPatient(Patient patient) {
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Patient VALUES(DEFAULT, @name, @birth, @cpf, @rg, @phone)";
                command.Parameters.AddWithValue("@name", patient.Name);
                command.Parameters.AddWithValue("@birth", patient.Birthday);
                command.Parameters.AddWithValue("@cpf", patient.CPF);
                command.Parameters.AddWithValue("@rg", patient.RG);
                command.Parameters.AddWithValue("@phone", patient.Phone);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? (short)command.LastInsertedId : -1;
            }
        }

        public bool DeleteSchedule(ushort scheduleId) {
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Schedule WHERE id=@id";
                command.Parameters.AddWithValue("@id", scheduleId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool DeleteEventualSchedule(ushort eventualScheduleId) {
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM EventualSchedule WHERE id=@id";
                command.Parameters.AddWithValue("@id", eventualScheduleId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool DeleteEmployee(ushort employeeId) {
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Employee WHERE id=@id";
                command.Parameters.AddWithValue("@id", employeeId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool DeleteAppointment(ushort appointmentId) {
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Appointment WHERE id=@id";
                command.Parameters.AddWithValue("@id", appointmentId);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool DeletePatient(ushort patientId) {
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Patient WHERE id=@id";
                command.Parameters.AddWithValue("@id", patientId);

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

        public bool UpdateEventualSchedule(EventualSchedule eventualSchedule) {
            using (var command = context.GetCommand()) {
                command.CommandText = "UPDATE EventualSchedule SET eventualState=@eState, eventualDate=@eDate, startTime=@start, endTime=@end, frequency=@freq WHERE id=@id";
                command.Parameters.AddWithValue("@eState", eventualSchedule.EventualState);
                command.Parameters.AddWithValue("@eDate", eventualSchedule.EventualDate);
                command.Parameters.AddWithValue("@start", eventualSchedule.StartTime);
                command.Parameters.AddWithValue("@end", eventualSchedule.EndTime);
                command.Parameters.AddWithValue("@freq", eventualSchedule.Frequency);
                command.Parameters.AddWithValue("@id", eventualSchedule.Id);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool UpdateEmployee(Employee employee) {
            using (var command = context.GetCommand()) {
                command.CommandText = "UPDATE Employee SET name=@name, cpf=@cpf, rg=@rg, birthday=@birth, phone=@phone WHERE id=@id";
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@cpf", employee.CPF);
                command.Parameters.AddWithValue("@rg", employee.RG);
                command.Parameters.AddWithValue("@birth", employee.Birthday);
                command.Parameters.AddWithValue("@phone", employee.Phone);
                command.Parameters.AddWithValue("@id", employee.Id);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool UpdateAppointment(Appointment appointment) {
            using (var command = context.GetCommand()) {
                command.CommandText = "UPDATE Appointment SET appointmentType=@aType, appointmentDate=@aDate, employeeId=@eId, patientId=@pId, roomId=@rId WHERE id=@id";
                command.Parameters.AddWithValue("@aType", appointment.AppointmentType);
                command.Parameters.AddWithValue("@aDate", appointment.AppointmentDate);
                command.Parameters.AddWithValue("@eId", appointment.Professional.Id);
                command.Parameters.AddWithValue("@pId", appointment.Patient.Id);
                command.Parameters.AddWithValue("@rId", appointment.Room.Id);
                command.Parameters.AddWithValue("@id", appointment.Id);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

        public bool UpdatePatient(Patient patient) {
            using (var command = context.GetCommand()) {
                command.CommandText = "UPDATE Patient SET fullName=@name, cpf=@cpf, rg=@rg, birthday=@birth, phone=@phone WHERE id=@id";
                command.Parameters.AddWithValue("@name", patient.Name);
                command.Parameters.AddWithValue("@cpf", patient.CPF);
                command.Parameters.AddWithValue("@rg", patient.RG);
                command.Parameters.AddWithValue("@birth", patient.Birthday);
                command.Parameters.AddWithValue("@phone", patient.Phone);
                command.Parameters.AddWithValue("@id", patient.Id);

                int rows = command.ExecuteNonQuery();

                return (rows > 0) ? true : false;
            }
        }

    }
}
