using Glass.Core.Database;
using Glass.Core.Enums;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Glass.Controllers.WebSocket {
    class ScheduleController : WebSocketBehavior {

        private Context context;

        public ScheduleController(Context context) {
            this.context = context;
        }

        protected override void OnMessage(MessageEventArgs e) {
            Console.WriteLine("Mensagem recebida de um cliente! Vamos respondê-lo com todo amor do mundo <3");
            // Processar mensagem
            JObject request = JObject.Parse(e.Data);
            string method = request.Value<string>("method");
            int employeeId = request.Value<int>("employee_id");
            int month = request.Value<int>("month");
            if(method == "GET") {
                string response = GetEmployeeSchedule(employeeId, month);
                Send(response);
            }
        }

        protected override void OnOpen() {
            Console.WriteLine("Um cliente acabou de se conectar. Estamos ficando famosos!");
            // Mandar carga inicial
            List<Professional> professionals = new List<Professional>();
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

            var x = new { 
                code = 200,
                sucess = true, 
                data = professionals 
            };
            Send(JsonConvert.SerializeObject(x));
        }

        protected override void OnError(ErrorEventArgs e) {
            Console.WriteLine("");
            Console.WriteLine("Um cliente apresentou um erro: {0}", e.Message);
            Console.WriteLine("");
        }

        protected override void OnClose(CloseEventArgs e) {
            Console.WriteLine("Um cliente acabou de se desconectar. Adeus...");
            Console.WriteLine("Usuários ainda ativos: {0}", base.Sessions.Count);
        }

        private string GetEmployeeSchedule(int employeeId, int month) {
            List<EventualSchedule> eventualSchedules = new List<EventualSchedule>();
            List<Appointment> appointments = new List<Appointment>();
            List<Schedule> schedules = new List<Schedule>();

            DateTime dateStart = new DateTime(DateTime.Now.Year, month, 1);
            DateTime dateEnd = dateStart.AddMonths(1);

            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT id,dayOfWeek,startTime,endTime,frequency FROM Schedule WHERE employeeId=@employeeId";
                command.Parameters.AddWithValue("@employeeId", employeeId);

                using(var reader = command.ExecuteReader()) {
                    while(reader.HasRows) {
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

                command.CommandText = @"SELECT id,eventualDate,startTime,endTime,frequency,eventualState FROM EventualSchedule WHERE employeeId=@employeeId AND eventualDate BETWEEN @monthStart AND @monthEnd";
                command.Parameters.AddWithValue("monthStart", dateStart);
                command.Parameters.AddWithValue("monthEnd", dateEnd);
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

                command.CommandText = "SELECT id,appointmentDate,appointmentType FROM Appointment WHERE employeeId=@employeeId AND appointmentDate BETWEEN @monthStart AND @monthEnd";
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

            var response = new {
                code = 200,
                success = true,
                data = new {
                    schedules = schedules,
                    eventual_schedules = eventualSchedules,
                    appointments = appointments,
                }
            };

            return JsonConvert.SerializeObject(response);
        }

    }
}
