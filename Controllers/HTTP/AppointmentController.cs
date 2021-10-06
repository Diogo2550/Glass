using Glass.Core.Database;
using Glass.Core.Exceptions;
using Glass.Core.HTTP;
using Glass.Core.HTTP.Builders;
using Glass.Core.Util;
using Glass.Core.WebSocket.Builders;
using Glass.Models;
using MySqlConnector;
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

            if(!Authenticate(response)) {
                response.Reply();
                return;
            }

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

            int affectedRows;
            using (var conn = new MySqlConnection(context.GetConnectionString())) {
                conn.Open();
                using (var command = conn.CreateCommand()) {
                    command.CommandText = "INSERT INTO Appointment VALUES(DEFAULT, @date, @type, @employee, @patient, @room)";
                    command.Parameters.AddWithValue("@date", appointment.AppointmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@type", appointment.AppointmentType.ToString());
                    command.Parameters.AddWithValue("@employee", appointment.Professional.Id);
                    command.Parameters.AddWithValue("@patient", appointment.Patient.Id);
                    command.Parameters.AddWithValue("@room", appointment.Room.Id);

                    affectedRows = command.ExecuteNonQuery();
                    appointment.SetId((ushort)command.LastInsertedId);
                }
            }

            Appointment returnedAppointment = new Appointment();
            using (var conn = new MySqlConnection(context.GetConnectionString())) {
                conn.Open();
                using (var command = conn.CreateCommand()) {
                    StringBuilder b = new StringBuilder();
                    b.Append("SELECT a.id, a.appointmentDate, a.appointmentType, e.id, e.name, e.phone, r.id, r.name, p.id, p.fullName, p.phone FROM Appointment a");
                    b.Append(" INNER JOIN patient p ON p.id=a.patientId");
                    b.Append(" INNER JOIN employee e ON e.id=a.patientId");
                    b.Append(" INNER JOIN room r ON r.id=a.patientId");
                    b.Append(" WHERE a.id=@id");

                    command.CommandText = b.ToString();
                    command.Parameters.AddWithValue("@id", appointment.Id);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            appointment.SetId(reader.GetUInt16(0));
                            appointment.SetAppointmentDate(reader.GetDateTime(1));
                            appointment.SetAppointmentType(reader.GetString(2));
                            appointment.Professional.SetId(reader.GetUInt16(3));
                            appointment.Professional.SetName(reader.GetString(4));
                            appointment.Professional.SetPhone(reader.GetString(5));
                            appointment.Room.SetId(reader.GetUInt16(6));
                            appointment.Room.SetName(reader.GetString(7));
                            appointment.Patient.SetId(reader.GetUInt16(8));
                            appointment.Patient.SetName(reader.GetString(9));
                            appointment.Patient.SetPhone(reader.GetString(10));
                        }
                    }
                }
            }

            response.SetData("Consulta marcada com sucesso!");
            response.SetStatusCode(201);
            response.Reply();

            var data = new {
                appointment = appointment,
            };

            WebSocketResponseBuilder wsBuilder = new WebSocketResponseBuilder();
            wsBuilder.SetMethod("ADD_APPOINTMENT");
            wsBuilder.SetData(data);
            websocket.Sessions.Broadcast(wsBuilder.GetResponse());
        }

        public void Cancel() {
            JObject body = request.GetBodyJson();

            if (!Authenticate(response)) {
                response.Reply();
                return;
            }

            ushort appointmentId = body.Value<ushort>("appointmentId");
            if(appointmentId == 0) {
                response.SetError("O campo appointmentId é obrigatório.");
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            Appointment appointment = new Appointment();
            int affectedRows;
            using (var conn = new MySqlConnection(context.GetConnectionString())) {
                conn.Open();
                using (var command = conn.CreateCommand()) {
                    StringBuilder b = new StringBuilder();
                    b.Append("SELECT a.id, a.appointmentDate, a.appointmentType, e.id, e.name, e.phone, r.id, r.name, p.id, p.fullName, p.phone FROM Appointment a");
                    b.Append(" INNER JOIN patient p ON p.id=a.patientId");
                    b.Append(" INNER JOIN employee e ON e.id=a.patientId");
                    b.Append(" INNER JOIN room r ON r.id=a.patientId");
                    b.Append(" WHERE a.id=@id");

                    command.CommandText = b.ToString();
                    command.Parameters.AddWithValue("@id", appointmentId);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            appointment.SetId(reader.GetUInt16(0));
                            appointment.SetAppointmentDate(reader.GetDateTime(1));
                            appointment.SetAppointmentType(reader.GetString(2));
                            appointment.Professional.SetId(reader.GetUInt16(3));
                            appointment.Professional.SetName(reader.GetString(4));
                            appointment.Professional.SetPhone(reader.GetString(5));
                            appointment.Room.SetId(reader.GetUInt16(6));
                            appointment.Room.SetName(reader.GetString(7));
                            appointment.Patient.SetId(reader.GetUInt16(8));
                            appointment.Patient.SetName(reader.GetString(9));
                            appointment.Patient.SetPhone(reader.GetString(10));
                        }
                    }
                }

                using (var command = context.GetCommand()) {
                    command.CommandText = "DELETE FROM Appointment WHERE id=@id";
                    command.Parameters.AddWithValue("@id", appointmentId);

                    affectedRows = command.ExecuteNonQuery();
                }
            }

            if (affectedRows == 0) {
                response.SetError($"Não existem uma consulta com o id {appointmentId}");
                response.SetStatusCode(400);
                response.Reply();
                return;
            }

            response.SetData("Consulta cancelada com sucesso!");
            response.SetStatusCode(200);
            response.Reply();

            var data = new {
                appointment = appointment,
            };

            WebSocketResponseBuilder wsBuilder = new WebSocketResponseBuilder();
            wsBuilder.SetData(data);
            wsBuilder.SetMethod("DELETE_APPOINTMENT");
            websocket.Sessions.Broadcast(wsBuilder.GetResponse());
        }

        private bool Authenticate(HTTPResponseBuilder responseBuilder) {
            var body = request.GetBodyJson();
            var token = body.Value<string>("token");

            if (!JWT.Validate(token)) {
                responseBuilder.SetError("Falha ao verificar sessão. Token inválido.");
                responseBuilder.SetStatusCode(401);

                return false;
            }
            return true;
        }

    }
}
