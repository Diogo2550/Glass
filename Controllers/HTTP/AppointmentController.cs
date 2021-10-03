using Glass.Core.Database;
using Glass.Core.Exceptions;
using Glass.Core.HTTP;
using Glass.Core.Util;
using Glass.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace Glass.Controllers.HTTP {
    class AppointmentController : HTTPRouter {

        public AppointmentController(HttpRequestEventArgs e, Context c) : base(e, c) {
            base.WebSocketClass = "ScheduleController";
        }

        public void Make() {
            JObject body = request.GetBodyJson();

            ushort roomId = body.Value<ushort>("roomId");
            ushort professionalId = body.Value<ushort>("professionalId");
            ushort patientId = body.Value<ushort>("patientId");

            Appointment appointment = null;
            try {
                appointment = RequestObjectFactory.BuildAppointment(body.SelectToken("appointment"));
            } catch (InvalidRequestArgument ex) {
                response.SetError(ex.Message);
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            appointment.Room.SetId(roomId);
            appointment.Professional.SetId(professionalId);
            appointment.Patient.SetId(patientId);

            int insertedId;
            using (var command = context.GetCommand()) {
                command.CommandText = "INSERT INTO Appointment VALUES(DEFAULT, @date, @type, @employee, @patient, @room)";
                command.Parameters.AddWithValue("@date", appointment.AppointmentDate.ToString("yyyy-MM-dd HH-mm-ss"));
                command.Parameters.AddWithValue("@type", appointment.AppointmentType.ToString());
                command.Parameters.AddWithValue("@employee", appointment.Professional.Id);
                command.Parameters.AddWithValue("@patient", appointment.Patient.Id);
                command.Parameters.AddWithValue("@room", appointment.Room.Id);

                insertedId = command.ExecuteNonQuery();
                appointment.SetId((ushort)command.LastInsertedId);
            }

            if (insertedId == -1) {
                response.SetError("Falha ao adicionar consulta para o funcionário.");
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            response.SetData("Consulta marcada com sucesso!");
            response.SetStatusCode(201);
            response.Reply();

            var data = new {
                method = "ADD_APPOINTMENT",
                success = true,
                code = 200,
                data = new {
                    appointment = appointment,
                }
            };
            websocket.Sessions.Broadcast(JObject.FromObject(data, new JsonSerializer() {
                ContractResolver = new DefaultContractResolver() {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }).ToString());
        }

        public void Cancel() {
            JObject body = request.GetBodyJson();

            ushort appointmentId = body.Value<ushort>("appointmentId");
            if(appointmentId == 0) {
                response.SetError("O campo appointmentId é obrigatório.");
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            Appointment appointment = new Appointment();
            using (var command = context.GetCommand()) {
                command.CommandText = "SELECT * FROM Appointment WHERE id=@id";
                command.Parameters.AddWithValue("@id", appointmentId);

                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        appointment.SetId(reader.GetUInt16("id"));
                        appointment.SetAppointmentDate(reader.GetDateTime("appointmentDate"));
                        appointment.SetAppointmentType(reader.GetString("appointmentType"));
                    }
                }
            }

            int deletedId;
            using (var command = context.GetCommand()) {
                command.CommandText = "DELETE FROM Appointment WHERE id=@id";
                command.Parameters.AddWithValue("@id", appointmentId);

                deletedId = command.ExecuteNonQuery();
            }

            if (deletedId == -1) {
                response.SetError("Falha ao cancelar a consulta.");
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            response.SetData("Consulta cancelada com sucesso!");
            response.SetStatusCode(200);
            response.Reply();

            var data = new {
                method = "DELETE_APPOINTMENT",
                success = true,
                code = 200,
                data = new {
                    appointment = appointment,
                }
            };
            websocket.Sessions.Broadcast(JObject.FromObject(data, new JsonSerializer() {
                ContractResolver = new DefaultContractResolver() {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }).ToString());
        }

    }
}
