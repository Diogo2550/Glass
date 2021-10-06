using Glass.Core.Enums;
using Glass.Core.Exceptions;
using Glass.Core.Util;
using Glass.Core.WebSocket.Builders;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Controllers.WebSocket {
    partial class ScheduleController {

        #region GET
        public void GET_ALL(JObject request, WebSocketResponseBuilder response) {
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

        public void GET_SCHEDULES(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");

            var data = new {
                schedules = repository.GetSchedulesFromEmployee(employeeId)
            };

            response.SetData(data);
            Send(response.GetResponse());
        }

        public void GET_ADDITIONALS(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");
            ushort month = request.Value<ushort>("month");
            int year = request.Value<int>("year");

            year = year == 0 ? (ushort)DateTime.Now.Year : year;

            var data = new {
                eventualSchedules = repository.GetMonthlyEventualSchedulesFromEmployee(employeeId, month, year),
                appointments = repository.GetMonthlyAppointmentsFromEmployee(employeeId, month, year)
            };

            response.SetData(data);
            Send(response.GetResponse());
        }

        public void GET_PATIENT(JObject request, WebSocketResponseBuilder response) {
            ushort patientId = request.Value<ushort>("patientId");

            if(patientId == 0) {
                response.SetError("Error: era esperado a presença de um patientId");
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
        
        public void GET_ALL_PATIENTS(JObject request, WebSocketResponseBuilder response) {
            var data = new {
                patients = repository.GetAllPatients()
            };

            response.SetData(data);
            Sessions.Broadcast(response.GetResponse());
        }

        public void GET_ALL_ROOMS(JObject request, WebSocketResponseBuilder response) {            
            var data = new {
                rooms = repository.GetAllRooms()
            };

            response.SetData(data);
            Send(response.GetResponse());
        }
        #endregion

        #region ADD
        public void ADD_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
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

        public void ADD_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");
            EventualSchedule eventualSchedule = null;
            try {
                eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            int insertedId = repository.AddEventualScheduleToEmployee(employeeId, eventualSchedule);
            if (insertedId == -1) {
                response.SetError("Falha ao inserir cronograma eventual");
                return;
            }

            if (eventualSchedule.EventualState == EventualState.BlockedByAdmin || eventualSchedule.EventualState == EventualState.BlockedByProfessional) {
                var appointments = repository.GetAppointmentsOnDay(eventualSchedule.EventualDate);
                appointments.ForEach(x => {
                    // Sem tempo de fazer direito D:
                    // Deveria ser pego direto do banco de dados filtrado pelo id
                    if (x.Professional.Id != employeeId) {
                        return;
                    }
                    DateTime start = new DateTime(x.AppointmentDate.Year, x.AppointmentDate.Month, x.AppointmentDate.Day);
                    DateTime end = new DateTime(x.AppointmentDate.Year, x.AppointmentDate.Month, x.AppointmentDate.Day);
                    start = start.Add(eventualSchedule.StartTime);
                    end = end.Add(eventualSchedule.EndTime);

                    if (x.AppointmentDate >= start && x.AppointmentDate <= end) {
                        repository.DeleteFrom(x.Id, "Appointment");
                    }
                });
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

        public void ADD_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
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

        public void ADD_PATIENT(JObject request, WebSocketResponseBuilder response) {
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

        public void ADD_ROOM(JObject request, WebSocketResponseBuilder response) {
            Room room = null;
            string roomName = request.SelectToken("room").Value<string>("name");
            if(String.IsNullOrEmpty(roomName)) {
                response.SetError("O parâmetro roomName é obrigatório!");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            room = new Room();
            room.SetName(roomName);

            int insertedId = repository.AddRoom(room);
            if (insertedId == -1) {
                response.SetError("Falha ao inserir funcionário ao banco de dados");
                response.SetStatusCode(400);
                return;
            }

            room.SetId((ushort)insertedId);
            var data = new {
                room = room
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }
        #endregion

        #region DELETE
        public void DELETE_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort scheduleId = request.Value<ushort>("scheduleId");

            Schedule schedule = repository.GetScheduleById(scheduleId);

            bool deleted = repository.DeleteFrom(scheduleId, "Schedule");
            var data = new {
                schedule = schedule
            };

            SendDefaultResponse(deleted, response, data, "Falha ao deleter o cronograma.");
        }

        public void DELETE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            ushort eventualScheduleId = request.Value<ushort>("eventualScheduleId");

            EventualSchedule eventualSchedule = repository.GetEventualScheduleById(eventualScheduleId);

            bool deleted = repository.DeleteFrom(eventualScheduleId, "EventualSchedule");
            var data = new {
                eventualSchedule = eventualSchedule
            };

            SendDefaultResponse(deleted, response, data, "Falha ao deleter o cronograma eventual.");
        }

        public void DELETE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            ushort employeeId = request.Value<ushort>("employeeId");

            Employee employee = repository.GetEmployeeById(employeeId);

            bool deleted = repository.DeleteFrom(employeeId, "Employee");
            var data = new {
                employee = employee
            };

            SendDefaultResponse(deleted, response, data, "Falha ao deleter o funcionário.");
        }

        public void DELETE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            ushort patientId = request.Value<ushort>("patientId");

            Patient patient = repository.GetPatientById(patientId);

            bool deleted = repository.DeleteFrom(patientId, "Patient");
            var data = new {
                patient = patient
            };

            SendDefaultResponse(deleted, response, data, "Falha ao deleter o paciente.");
        }

        public void DELETE_ROOM(JObject request, WebSocketResponseBuilder response) {
            ushort roomId = request.Value<ushort>("roomId");

            Room room = repository.GetRoomById(roomId);

            bool deleted = repository.DeleteFrom(roomId, "Room");
            var data = new {
                room = room
            };

            SendDefaultResponse(deleted, response, data, "Falha ao deleter o quarto.");
        }
        #endregion

        #region UPDATE
        public void UPDATE_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            Schedule schedule = null;
            ushort employeeId = request.Value<ushort>("employeeId");

            try {
                schedule = RequestObjectFactory.BuildSchedule(request.SelectToken("schedule"));
            } catch(InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            bool updated = repository.UpdateSchedule(schedule);
            var data = new {
                employeeId = employeeId,
                schedule = schedule
            };

            SendDefaultResponse(updated, response, data, "Falha ao atualizar o cronograma.");
        }

        public void UPDATE_EVENTUAL_SCHEDULE(JObject request, WebSocketResponseBuilder response) {
            EventualSchedule eventualSchedule = null;
            ushort employeeId = request.Value<ushort>("employeeId");

            try {
                eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            bool updated = repository.UpdateEventualSchedule(eventualSchedule);
            var data = new {
                employeeId = employeeId,
                eventualSchedule = eventualSchedule
            };

            SendDefaultResponse(updated, response, data, "Falha ao atualizar o cronograma eventual.");
        }

        public void UPDATE_EMPLOYEE(JObject request, WebSocketResponseBuilder response) {
            Employee employee = null;

            try {
                employee = RequestObjectFactory.BuildEmployee(request.SelectToken("employee"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            bool updated = repository.UpdateEmployee(employee);
            var data = new {
                employee = employee
            };

            SendDefaultResponse(updated, response, data, "Falha ao atualizar o funcionário.");
        }

        public void UPDATE_APPOINTMENT(JObject request, WebSocketResponseBuilder response) {
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

            bool updated = repository.UpdateAppointment(appointment);
            var data = new {
                employeeId = employeeId,
                appointment = appointment
            };

            SendDefaultResponse(updated, response, data, "Falha ao atualizar a consulta.");
        }

        public void UPDATE_PATIENT(JObject request, WebSocketResponseBuilder response) {
            Patient patient = null;
            try {
                patient = RequestObjectFactory.BuildPatient(request.SelectToken("patient"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            bool updated = repository.UpdatePatient(patient);
            var data = new {
                patient = patient
            };

            SendDefaultResponse(updated, response, data, "Falha ao atualizar o paciente.");
        }

        public void UPDATE_ROOM(JObject request, WebSocketResponseBuilder response) {
            Room room = null;
            ushort roomId = request.SelectToken("room").Value<ushort>("id");
            string roomName = request.SelectToken("room").Value<string>("name");
            if(roomId == 0) {
                response.SetError("O parâmetro roomId é obrigatório.");
                response.SetStatusCode(400);
                Send(response.GetResponse());
                return;
            }

            room.SetId(roomId);
            var data = new {
                room = room
            };
            
            bool updated = repository.UpdateRoom(room);
            SendDefaultResponse(updated, response, data, "Falha ao atulizar o quarto.");
        }
        #endregion

        #region Especial
        public void BLOCK_DAY_TO_ALL(JObject request, WebSocketResponseBuilder response) {
            EventualSchedule eventualSchedule = null;
            try {
                eventualSchedule = RequestObjectFactory.BuildEventualSchedule(request.SelectToken("eventualSchedule"));
            } catch (InvalidRequestArgument ex) {
                Send(ex.response.GetResponse());
                return;
            }

            eventualSchedule.SetStartTime(TimeSpan.Parse("00:00:00"));
            eventualSchedule.SetEndTime(TimeSpan.Parse("23:59:59"));
            repository.DeleteEventualScheduleByDate(eventualSchedule.EventualDate);

            int insertedId = -1;
            var employees = repository.GetAllEmployees();
            employees.ForEach(x => {
                insertedId = repository.AddEventualScheduleToEmployee(x.Id, eventualSchedule);
            });
            if (insertedId == -1) {
                response.SetError("Falha ao inserir cronograma eventual");
                return;
            }

            eventualSchedule.SetId((ushort)insertedId);
            var data = new {
                eventualSchedule = eventualSchedule
            };

            response.SetData(data);
            response.SetMethod("ADD_EVENTUAL_SCHEDULE");
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }

        public void UNLOCK_DAY_TO_ALL(JObject request, WebSocketResponseBuilder response) {
            string date = request.SelectToken("date").Value<string>();
            DateTime dateToUnlock = DateTime.Parse(date);

            var eventualSchedule = repository.GetDayBlockedToAll(dateToUnlock);
            eventualSchedule.SetId(0);

            bool deleted = repository.DeleteEventualScheduleByDate(dateToUnlock);
            if (!deleted) {
                response.SetError("Falha ao inserir cronograma eventual");
                return;
            }

            var data = new {
                eventualSchedule = eventualSchedule
            };

            response.SetData(data);
            response.SetStatusCode(201);
            Sessions.Broadcast(response.GetResponse());
        }
        #endregion

        // MÉTODOS UTILITÁRIOS
        private void SendDefaultResponse(bool success, WebSocketResponseBuilder response, object data, string errorMessage = "Error") {
            if (success) {
                response.SetData(data);
                Sessions.Broadcast(response.GetResponse());
            } else {
                response.SetError(errorMessage);
                response.SetStatusCode(400);

                Send(response.GetResponse());
            }
        }
    }
}
