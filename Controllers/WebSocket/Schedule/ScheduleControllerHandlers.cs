using Glass.Core.Exceptions;
using Glass.Core.Util;
using Glass.Core.WebSocket.Builders;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Controllers.WebSocket {
    partial class ScheduleController {

        private void GET_ALL(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");
            ushort month = request.Value<ushort>("month");
            int year = request.Value<int>("year");

            year = year == 0 ? (ushort)DateTime.Now.Year : year;

            var data = new {
                schedules = repository.GetSchedulesFromEmployee(employeeId),
                eventualSchedules = repository.GetMonthlyEventualSchedulesFromEmployee(employeeId, month, year),
                appointments = repository.GetMonthlyAppointmentsFromEmployee(employeeId, month, year)
            };

            response.SetData(data);
            Send(response.GetResponse());
        }

        private void GET_PATIENT(JObject request, WebSocketResponseBuilder response) {
            ushort patientId = request.Value<ushort>("patientId");

            if(patientId == 0) {
                response.SetError("Por favor, insira um id de paciênte válido!");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            var data = new {
                patient = repository.GetPatientById(patientId)
            };

            response.SetData(data);
            Sessions.Broadcast(response.GetResponse());
        }

        private void ADD_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");
            Schedule schedule;
            try {
                schedule = RequestObjectFactory.BuildSchedule(request.SelectToken("schedule"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddScheduleToEmployee(employeeId, schedule);
            if (insertedId == -1) {
                response.SetError("Falha ao inserir cronograma ao banco de dados.");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            schedule.SetId((ushort)insertedId);
            var data = new {
                employeeId = employeeId,
                schedule = schedule
            };

            response.SetStatusCode(201);
            response.SetData(data);
            Sessions.Broadcast(response.GetResponse());
        }

        private void ADD_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");
            EventualSchedule eventualSchedule = null;
            try {
                eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddEventualScheduleToEmployee(employeeId, eventualSchedule);
            if (insertedId == -1) {
                response.SetError("Falha ao inserir cronograma eventual");
                return;
            }

            eventualSchedule.SetId((ushort)insertedId);
            var data = new {
                employeeId = employeeId,
                eventualSchedule = eventualSchedule
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        private void ADD_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            Employee employee = null;
            try {
                employee = RequestObjectFactory.BuildEmployee(request.SelectToken("employee"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddEmployee(employee);
            if (insertedId == -1) {
                response.SetError("Falha ao inserir funcionário ao banco de dados");
                response.SetStatusCode(400);
                return;
            }

            employee.SetId((ushort)insertedId);
            var data = new {
                employee = employee
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        private void ADD_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            ushort roomId = request.Value<ushort>("roomId");
            ushort professionalId = request.Value<ushort>("professionalId");
            ushort patientId = request.Value<ushort>("patientId");

            Appointment appointment = null;
            try {
                appointment = RequestObjectFactory.BuildAppointment(request.SelectToken("appointment"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddAppointmentToEmployee(professionalId, roomId, patientId, appointment);
            if (insertedId == -1) {
                response.SetError("Falha ao adicionar consulta para o funcionário.");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            appointment.SetId((ushort)insertedId);
            var data = new {
                appointment = appointment
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        private void ADD_PATIENT(JObject request, WebSocketResponseBuilder response) {
            Patient patient = null;
            try {
                patient = RequestObjectFactory.BuildPatient(request.SelectToken("patient"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddPatient(patient);
            if (insertedId == -1) {
                response.SetError("Falha ao adicionar paciente ao banco de dados.");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            patient.SetId((ushort)insertedId);
            var data = new {
                patient = patient
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        private void DELETE_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort scheduleId = request.Value<ushort>("scheduleId");

            bool deleted = repository.DeleteSchedule(scheduleId);
            if (deleted) {
                var data = new {
                    scheduleId = scheduleId
                };

                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao deletar o cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void DELETE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort eventualScheduleId = request.Value<ushort>("eventualScheduleId");

            bool deleted = repository.DeleteEventualSchedule(eventualScheduleId);
            if(deleted) {
                var data = new {
                    eventualScheduleId = eventualScheduleId
                };

                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao deletar o cronograma eventual.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void DELETE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");

            bool deleted = repository.DeleteEmployee(employeeId);
            if (deleted) {
                var data = new {
                    employeeId = employeeId
                };

                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao deletar o funcionário.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void DELETE_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            ushort appointmentId = request.Value<ushort>("appointmentId");

            bool deleted = repository.DeleteAppointment(appointmentId);
            if (deleted) {
                var data = new {
                    appointmentId = appointmentId
                };

                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao deletar a consulta.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void DELETE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            ushort patientId = request.Value<ushort>("patientId");

            bool deleted = repository.DeletePatient(patientId);
            if (deleted) {
                var data = new {
                    patientId = patientId
                };

                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao deletar o paciente.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void UPDATE_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            Schedule schedule = null;
            ushort employeeId = request.Value<ushort>("employeeId");

            try {
                schedule = RequestObjectFactory.BuildSchedule(request.SelectToken("schedule"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if (repository.UpdateSchedule(schedule)) {
                var data = new {
                    employeeId = employeeId,
                    schedule = schedule
                };
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void UPDATE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            EventualSchedule eventualSchedule = null;
            ushort employeeId = request.Value<ushort>("employeeId");

            try {
                eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if (repository.UpdateEventualSchedule(eventualSchedule)) {
                var data = new {
                    employeeId = employeeId,
                    eventualSchedule = eventualSchedule
                };
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void UPDATE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            Employee employee = null;

            try {
                employee = RequestObjectFactory.BuildEmployee(request.SelectToken("employee"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if (repository.UpdateEmployee(employee)) {
                var data = new {
                    employee = employee
                };
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void UPDATE_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            Appointment appointment = null;
            ushort employeeId = request.Value<ushort>("employeeId");
            ushort roomId = request.Value<ushort>("roomId");
            ushort patientId = request.Value<ushort>("patientId");

            try {
                appointment = RequestObjectFactory.BuildAppointment(request.SelectToken("appointment"));
                appointment.Room.SetId(roomId);
                appointment.Professional.SetId(employeeId);
                appointment.Patient.SetId(patientId);
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if (repository.UpdateAppointment(appointment)) {
                var data = new {
                    employeeId = employeeId,
                    appointment = appointment
                };
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

        private void UPDATE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            Patient patient = null;
            try {
                patient = RequestObjectFactory.BuildPatient(request.SelectToken("patient"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if (repository.UpdatePatient(patient)) {
                var data = new {
                    patient = patient
                };
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }

    }
}
