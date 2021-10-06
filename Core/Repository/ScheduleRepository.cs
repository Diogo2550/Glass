using Glass.Core.Database;
using Glass.Core.Enums;
using Glass.Models;
using Glass.Models.Abstracts;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Core.Repository {
    class ScheduleRepository {

        private Context context;

        public ScheduleRepository(Context context) {
            this.context = context;
        }

        #region GET
        public Patient GetPatientById(ushort patientId) {
            Patient patient = new Patient();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Patient WHERE id=@id";
					command.Parameters.AddWithValue("@id", patientId);

					using (var reader = command.ExecuteReader()) {
						if(reader.HasRows) {
							reader.Read();

							patient.SetId(reader.GetUInt16("id"));
							patient.SetCPF(GetStringSafe(reader, "cpf"));
							patient.SetBirthday(reader.GetDateTime("birthday"));
							patient.SetName(GetStringSafe(reader, "fullName"));
							patient.SetPhone(GetStringSafe(reader, "phone"));
							patient.SetRG(GetStringSafe(reader, "rg"));
						}
					}
				}
            }

            return patient;
        }

        public List<Patient> GetAllPatients() {
            var patients = new List<Patient>();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Patient";

					using (var reader = command.ExecuteReader()) {
						while (reader.HasRows) {
							while (reader.Read()) {
								Patient patient = new Patient();

								patient.SetId(reader.GetUInt16("id"));
								patient.SetCPF(GetStringSafe(reader, "cpf"));
								patient.SetBirthday(reader.GetDateTime("birthday"));
								patient.SetName(GetStringSafe(reader, "fullName"));
								patient.SetPhone(GetStringSafe(reader, "phone"));
								patient.SetRG(GetStringSafe(reader, "rg"));

								patients.Add(patient);
							}

							reader.NextResult();
						}
					}
				}
            }

            return patients;
        }

        public Schedule GetScheduleById(ushort scheduleId) {
            Schedule schedule = new Schedule();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Schedule WHERE id=@id";
					command.Parameters.AddWithValue("@id", scheduleId);

					using (var reader = command.ExecuteReader()) {
						if (reader.HasRows) {
							reader.Read();

							schedule.SetId(reader.GetUInt16("id"));
							schedule.SetDayOfWeek(reader.GetUInt16("dayOfWeek"));
							schedule.SetStartTime(reader.GetTimeSpan("startTime"));
							schedule.SetFrequency(reader.GetUInt16("frequency"));
							schedule.SetEndTime(reader.GetTimeSpan("endTime"));
							schedule.employee.SetId(reader.GetUInt16("employeeId"));
						}
					}
				}
            }

            return schedule;
        }

        public List<Schedule> GetSchedulesFromEmployee(ushort employeeId) {
            var schedules = new List<Schedule>();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
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
            }
            return schedules;
        }

        public EventualSchedule GetEventualScheduleById(ushort eventualScheduleId) {
            EventualSchedule schedule = new EventualSchedule();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM EventualSchedule WHERE id=@id";
					command.Parameters.AddWithValue("@id", eventualScheduleId);

					using (var reader = command.ExecuteReader()) {
						if (reader.HasRows) {
							reader.Read();

							schedule.SetId(reader.GetUInt16("id"));
							schedule.SetEventualDate(reader.GetDateTime("eventualDate"));
							schedule.SetStartTime(reader.GetTimeSpan("startTime"));
							schedule.SetEndTime(reader.GetTimeSpan("endTime"));
							schedule.SetFrequency(reader.GetUInt16("frequency"));
							schedule.SetEventualState(reader.GetUInt16("eventualState"));
							schedule.employee.SetId(reader.GetUInt16("employeeId"));
						}
					}
				}
            }

            return schedule;
        }

		public EventualSchedule GetDayBlockedToAll(DateTime date) {
			EventualSchedule schedule = new EventualSchedule();

			using (var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM EventualSchedule WHERE eventualDate BETWEEN @dateStart AND @dateEnd LIMIT 1";
					command.Parameters.AddWithValue("@dateStart", date.ToString("yyyy-MM-dd 00:00:00"));
					command.Parameters.AddWithValue("@dateEnd", date.ToString("yyyy-MM-dd 23:59:59"));

					using (var reader = command.ExecuteReader()) {
						if (reader.HasRows) {
							reader.Read();

							schedule.SetId(reader.GetUInt16("id"));
							schedule.SetEventualDate(reader.GetDateTime("eventualDate"));
							schedule.SetStartTime(reader.GetTimeSpan("startTime"));
							schedule.SetEndTime(reader.GetTimeSpan("endTime"));
							schedule.SetFrequency(reader.GetUInt16("frequency"));
							schedule.SetEventualState(reader.GetUInt16("eventualState"));
							schedule.employee.SetId(reader.GetUInt16("employeeId"));
						}
					}
				}
			}

			return schedule;
		}


		public List<EventualSchedule> GetMonthlyEventualSchedulesFromEmployee(ushort employeeId, ushort month, int year) {
            var eventualSchedules = new List<EventualSchedule>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = (startDate.AddMonths(1)).AddDays(-1);
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
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
            }
            return eventualSchedules;
        }

        public List<Appointment> GetMonthlyAppointmentsFromEmployee(ushort employeeId, ushort month, int year) {
            var appointments = new List<Appointment>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = (startDate.AddMonths(1)).AddDays(-1);
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
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
								a.SetAppointmentType(GetStringSafe(reader, 2));
								a.Patient.SetId(reader.GetUInt16(3));
								a.Patient.SetName(GetStringSafe(reader, 4));
								a.Room.SetId(reader.GetUInt16(5));
								a.Room.SetName(GetStringSafe(reader, 6));

								appointments.Add(a);
							}
							reader.NextResult();
						}
					}
				}
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsOnDay(DateTime day) {
            var appointments = new List<Appointment>();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = $"SELECT * FROM Appointment WHERE appointmentDate BETWEEN '{day.ToString("yyyy-MM-dd 00:00:00")}' AND '{day.ToString("yyyy-MM-dd 23:59:59")}'";

					using(var reader = command.ExecuteReader()) {
						while (reader.HasRows) {
							while (reader.Read()) {
								Appointment a = new Appointment();
								a.SetId(reader.GetUInt16("id"));
								a.SetAppointmentDate(reader.GetDateTime("appointmentDate"));
								a.SetAppointmentType(GetStringSafe(reader, "appointmentType"));
								a.Patient.SetId(reader.GetUInt16("patientId"));
								a.Room.SetId(reader.GetUInt16("roomId"));
								a.Professional.SetId(reader.GetUInt16("employeeId"));

								appointments.Add(a);
							}
							reader.NextResult();
						}
					}
				}
            }
            return appointments;
        }

		public List<Employee> GetAllEmployees() {
			var employees = new List<Employee>();

			using (var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = $"SELECT id, birthday, name, admin, phone FROM Employee";

					using (var reader = command.ExecuteReader()) {
						while (reader.HasRows) {
							while (reader.Read()) {
								Employee employee;
								if (reader.GetUInt16("admin") == 1)
									employee = new Admin();
								else
									employee = new Professional();

								employee.SetId(reader.GetUInt16("id"));
								employee.SetBirthday(reader.GetDateTime("birthday"));
								employee.SetName(GetStringSafe(reader, "name"));
								employee.SetPhone(GetStringSafe(reader, "phone"));

								employees.Add(employee);
							}
							reader.NextResult();
						}
					}
				}
			}

			return employees;
		}

		public Employee GetEmployeeById(ushort id) {
            Employee employee = null;

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Employee WHERE id=@id";
					command.Parameters.AddWithValue("@id", id);

					using (var reader = command.ExecuteReader()) {
						if (reader.HasRows) {
							reader.Read();

							if (reader.GetUInt16("admin") == 1)
								employee = new Admin();
							else
								employee = new Professional();

							employee.SetId(reader.GetUInt16("id"));
							employee.SetName(GetStringSafe(reader, "name"));
							employee.SetPhone(GetStringSafe(reader, "phone"));
							employee.SetBirthday(reader.GetDateTime("birthday"));
						}
					}
				}
            }

            return employee;
        }

        public List<Professional> GetAllProfessionals() {
            var professionals = new List<Professional>();
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT id, name FROM Employee WHERE admin=0";

					using (var reader = command.ExecuteReader()) {
						while (reader.HasRows) {
							reader.Read();

							Professional p = new Professional();
							p.SetName(GetStringSafe(reader, 1));
							p.SetId(reader.GetUInt16(0));

							professionals.Add(p);
							reader.NextResult();
						}
					}
				}
            }
            return professionals;
        }

        public Room GetRoomById(ushort id) {
            Room room = new Room();

            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Room WHERE id=@id";
					command.Parameters.AddWithValue("@id", id);

					using (var reader = command.ExecuteReader()) {
						if (reader.HasRows) {
							reader.Read();

							room.SetId(reader.GetUInt16("id"));
							room.SetName(GetStringSafe(reader, "name"));
						}
					}
				}
            }

            return room;
        }

        public List<Room> GetAllRooms() {
            List<Room> rooms = new List<Room>();
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "SELECT * FROM Room";

					using (var reader = command.ExecuteReader()) {
						while(reader.HasRows) {
							while(reader.Read()) {
								Room room = new Room();

								room.SetId(reader.GetUInt16("id"));
								room.SetName(GetStringSafe(reader, "name"));

								rooms.Add(room);
							}

							reader.NextResult();
						}
					}
				}
            }
            return rooms;
        }
        #endregion

        #region ADD
        public int AddScheduleToEmployee(ushort employeeId, Schedule schedule) {
            int insertedId;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using(var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO Schedule VALUES (DEFAULT, @day, @start, @end, @frequency, @employeeId)";
					command.Parameters.AddWithValue("@day", schedule.DayOfWeek);
					command.Parameters.AddWithValue("@start", schedule.StartTime);
					command.Parameters.AddWithValue("@end", schedule.EndTime);
					command.Parameters.AddWithValue("@frequency", schedule.Frequency);
					command.Parameters.AddWithValue("@employeeId", employeeId);

					int rows = command.ExecuteNonQuery();
					schedule.SetId((ushort)command.LastInsertedId);

					insertedId = (rows > 0) ? (ushort)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }

        public int AddEventualScheduleToEmployee(ushort employeeId, EventualSchedule eventualSchedule) {
            int insertedId = 0;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO EventualSchedule VALUES(DEFAULT, @eventualDate, @start, @end, @freq, @state, @employeeId)";
					command.Parameters.AddWithValue("@eventualDate", eventualSchedule.EventualDate.ToString("yyyy-MM-dd HH:mm:ss"));
					command.Parameters.AddWithValue("@start", eventualSchedule.StartTime);
					command.Parameters.AddWithValue("@end", eventualSchedule.EndTime);
					command.Parameters.AddWithValue("@freq", eventualSchedule.Frequency);
					command.Parameters.AddWithValue("@state", eventualSchedule.EventualState);
					command.Parameters.AddWithValue("@employeeId", employeeId);

					int rows = command.ExecuteNonQuery();
					insertedId = (rows > 0) ? (short)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }

        public int AddAppointmentToEmployee(ushort employeeId, ushort roomId, ushort patientId, Appointment appointment) {
            int insertedId;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO Appointment VALUES(DEFAULT, @date, @type, @employee, @patient, @room)";
					command.Parameters.AddWithValue("@date", appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
					command.Parameters.AddWithValue("@type", appointment.AppointmentType);
					command.Parameters.AddWithValue("@employee", employeeId);
					command.Parameters.AddWithValue("@patient", patientId);
					command.Parameters.AddWithValue("@room", roomId);

					int rows = command.ExecuteNonQuery();

					insertedId = (rows > 0) ? (short)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }

        public int AddEmployee(Employee employee) {
            int insertedId;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO Employee VALUES(DEFAULT, @name, @cpf, @rg, @birth, @phone, @password, @admin)";
					command.Parameters.AddWithValue("@name", employee.Name);
					command.Parameters.AddWithValue("@cpf", employee.CPF);
					command.Parameters.AddWithValue("@rg", employee.RG);
					if(employee.Birthday.HasValue)
						command.Parameters.AddWithValue("@birth", employee.Birthday.Value.ToString("yyyy-MM-dd HH:mm:ss"));
					command.Parameters.AddWithValue("@phone", employee.Phone);
					command.Parameters.AddWithValue("@password", employee.Password);
					command.Parameters.AddWithValue("@admin", employee.IsAdmin());

					int rows = command.ExecuteNonQuery();
					
					insertedId = (rows > 0) ? (short)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }

        public int AddPatient(Patient patient) {
            int insertedId;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO Patient VALUES(DEFAULT, @name, @birth, @cpf, @rg, @phone)";
					command.Parameters.AddWithValue("@name", patient.Name);
					if(patient.Birthday.HasValue)
						command.Parameters.AddWithValue("@birth", patient.Birthday.Value.ToString("yyyy-MM-dd HH:mm:ss"));
					command.Parameters.AddWithValue("@cpf", patient.CPF);
					command.Parameters.AddWithValue("@rg", patient.RG);
					command.Parameters.AddWithValue("@phone", patient.Phone);

					int rows = command.ExecuteNonQuery();

					insertedId = (rows > 0) ? (short)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }

        public int AddRoom(Room room) {
            int insertedId;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "INSERT INTO Room VALUES(DEFAULT, @name)";
					command.Parameters.AddWithValue("@name", room.Name);

					int rows = command.ExecuteNonQuery();
					insertedId = (rows > 0) ? (short)command.LastInsertedId : -1;
				}
            }
            return insertedId;
        }
        #endregion

        #region UPDATE
        public bool UpdateSchedule(Schedule schedule) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE Schedule SET dayOfWeek=@day, startTime=@start, endTime=@end, frequency=@freq WHERE id=@id";
					command.Parameters.AddWithValue("@day", schedule.DayOfWeek);
					command.Parameters.AddWithValue("@start", schedule.StartTime);
					command.Parameters.AddWithValue("@end", schedule.EndTime);
					command.Parameters.AddWithValue("@freq", schedule.Frequency);
					command.Parameters.AddWithValue("@id", schedule.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }

        public bool UpdateEventualSchedule(EventualSchedule eventualSchedule) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE EventualSchedule SET eventualState=@eState, eventualDate=@eDate, startTime=@start, endTime=@end, frequency=@freq WHERE id=@id";
					command.Parameters.AddWithValue("@eState", eventualSchedule.EventualState);
					command.Parameters.AddWithValue("@eDate", eventualSchedule.EventualDate);
					command.Parameters.AddWithValue("@start", eventualSchedule.StartTime);
					command.Parameters.AddWithValue("@end", eventualSchedule.EndTime);
					command.Parameters.AddWithValue("@freq", eventualSchedule.Frequency);
					command.Parameters.AddWithValue("@id", eventualSchedule.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }

        public bool UpdateEmployee(Employee employee) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE Employee SET name=@name, cpf=@cpf, rg=@rg, birthday=@birth, phone=@phone WHERE id=@id";
					command.Parameters.AddWithValue("@name", employee.Name);
					command.Parameters.AddWithValue("@cpf", employee.CPF);
					command.Parameters.AddWithValue("@rg", employee.RG);
					command.Parameters.AddWithValue("@birth", employee.Birthday);
					command.Parameters.AddWithValue("@phone", employee.Phone);
					command.Parameters.AddWithValue("@id", employee.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }

        public bool UpdateAppointment(Appointment appointment) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE Appointment SET appointmentType=@aType, appointmentDate=@aDate, employeeId=@eId, patientId=@pId, roomId=@rId WHERE id=@id";
					command.Parameters.AddWithValue("@aType", appointment.AppointmentType);
					command.Parameters.AddWithValue("@aDate", appointment.AppointmentDate);
					command.Parameters.AddWithValue("@eId", appointment.Professional.Id);
					command.Parameters.AddWithValue("@pId", appointment.Patient.Id);
					command.Parameters.AddWithValue("@rId", appointment.Room.Id);
					command.Parameters.AddWithValue("@id", appointment.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }

        public bool UpdatePatient(Patient patient) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE Patient SET fullName=@name, cpf=@cpf, rg=@rg, birthday=@birth, phone=@phone WHERE id=@id";
					command.Parameters.AddWithValue("@name", patient.Name);
					command.Parameters.AddWithValue("@cpf", patient.CPF);
					command.Parameters.AddWithValue("@rg", patient.RG);
					command.Parameters.AddWithValue("@birth", patient.Birthday);
					command.Parameters.AddWithValue("@phone", patient.Phone);
					command.Parameters.AddWithValue("@id", patient.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }

        public bool UpdateRoom(Room room) {
            bool updated;
            using(var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = "UPDATE Room SET name=@name WHERE id=@id";
					command.Parameters.AddWithValue("@name", room.Name);
					command.Parameters.AddWithValue("@id", room.Id);

					int rows = command.ExecuteNonQuery();

					updated = (rows > 0) ? true : false;
				}
            }
            return updated;
        }
		#endregion

		#region DELETE
		public bool DeleteFrom(ushort id, string table) {
			bool deleted;
			using (var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = $"DELETE FROM {table} WHERE id=@id";
					command.Parameters.AddWithValue("@id", id);

					int rows = command.ExecuteNonQuery();

					deleted = (rows > 0) ? true : false;
				}
			}
			return deleted;
		}

		public bool DeleteEventualScheduleByDate(DateTime date) {
			bool deleted;
			using (var mysql = new MySqlConnection(context.GetConnectionString())) {
				mysql.Open();
				using (var command = mysql.CreateCommand()) {
					command.CommandText = $"DELETE FROM EventualSchedule WHERE eventualDate BETWEEN @dateStart AND @dateEnd";
					command.Parameters.AddWithValue("@dateStart", date.ToString("yyyy-MM-dd 00:00:00"));
					command.Parameters.AddWithValue("@dateEnd", date.ToString("yyyy-MM-dd 23:59:59"));

					int rows = command.ExecuteNonQuery();

					deleted = (rows > 0) ? true : false;
				}
			}
			return deleted;
		}
		#endregion

		// MÉTODOS PARA FACILITAR AS BUSCAS
		private static string GetStringSafe(IDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            else
                return null;
        }

        private static string GetStringSafe(IDataReader reader, string indexName) {
            return GetStringSafe(reader, reader.GetOrdinal(indexName));
        }

    }
}
