using Glass.Core.WebSocket.Builders;
using Glass.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Controllers.WebSocket {
    partial class ScheduleController {

        private string GetEmployeeSchedule(ushort employeeId, ushort month, ushort year, WebSocketResponseBuilder response) {
            year = year == 0 ? (ushort)DateTime.Now.Year : year;

            var data = new {
                schedules = repository.GetSchedulesFromEmployee(employeeId),
                eventualSchedules = repository.GetMonthlyEventualSchedulesFromEmployee(employeeId, month, year),
                appointments = repository.GetMonthlyAppointmentsFromEmployee(employeeId, month, year)
            };

            response.SetData(data);
            return response.GetResponse();
        }

        private string AddEmployeeSchedule(ushort employeeId, Schedule schedule, WebSocketResponseBuilder response) {
            if (!repository.AddScheduleToEmployee(employeeId, schedule)) {
                response.SetError("Erro ao adicionar calendário.");
                return null;
            }

            var data = new {
                employeeId = employeeId,
                schedule = schedule
            };

            response.SetStatusCode(201);
            response.SetData(data);
            return response.GetResponse();
        }

        private string DeleteSchedule(ushort scheduleId, WebSocketResponseBuilder response) {
            bool deleted = repository.DeleteSchedule(scheduleId);

            if (deleted) {
                var data = new {
                    scheduleId = scheduleId
                };

                response.SetData(data);
            } else {
                response.SetError("Falha ao deletar o cronograma");
                response.SetStatusCode(200);
            }

            return response.GetResponse();
        }

        private string UpdateSchedule(ushort employeeId, Schedule schedule, WebSocketResponseBuilder response) {
            if (repository.UpdateSchedule(schedule)) {
                var data = new {
                    employeeId = employeeId,
                    schedule = schedule
                };
                response.SetData(data);
            } else {
                response.SetError("Falha ao atualizar cronograma.");
                response.SetStatusCode(200);
            }

            return response.GetResponse();
        }

    }
}
