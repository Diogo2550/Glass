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
                //patient = repository.GetPatientById(patientId)
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

            if (!repository.AddScheduleToEmployee(employeeId, schedule)) {
                response.SetError("Falha ao inserir cronograma ao banco de dados.");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

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
                //eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            if(!repository.AddEventualScheduleToEmployee(employeeId, eventualSchedule)) {
                response.SetError("Falha ao inserir cronograma eventual");
                return;
            }

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
                //employee = RequestObjectFactory.BuildEmployee(request.SelectToken("employee"));
            } catch(InvalidRequestArgument ex) {
                Send(response.GetResponse());
                return;
            }

            //if(!repository.AddEmployee(employee)) {
            //    response.SetError("Falha ao inserir funcionário ao banco de dados");
            //    response.SetStatusCode(400);
            //    return;
            //}

            var data = new {
                employee = employee
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        // Método não implementado
        private void ADD_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            ushort roomId = request.Value<ushort>("roomId");
            ushort professionalId = request.Value<ushort>("professionalId");
            ushort patientId = request.Value<ushort>("patientId");
        }

        // Método não implementado
        private void ADD_PATIENT(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        private void DELETE_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort scheduleId = request.Value<ushort>("scheduleId");

            bool deleted = repository.DeleteSchedule(scheduleId);

            if (deleted) {
                var data = new {
                    scheduleId = scheduleId
                };

                response.SetData(data);
            } else {
                response.SetError("Falha ao deletar o cronograma");
                response.SetStatusCode(200);

                Send(response.GetResponse());
                return;
            }

            Sessions.Broadcast(response.GetResponse());
        }

        // Método não implementado
        private void DELETE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void DELETE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void DELETE_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void DELETE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
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
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(200);

                Send(response.GetResponse());
                return;
            }

            Sessions.Broadcast(response.GetResponse());
        }

        // Método não implementado
        private void UPDATE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void UPDATE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void UPDATE_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

        // Método não implementado
        private void UPDATE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            response.SetError("Método ainda não implementado");
        }

    }
}
