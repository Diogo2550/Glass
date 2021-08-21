using Glass.Core.Exceptions;
using Glass.Core.Repository;
using Glass.Core.Util;
using Glass.Core.WebSocket.Builders;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Glass.Controllers.WebSocket {
    partial class ScheduleController : WebSocketBehavior {

        private ScheduleRepository repository;

        public ScheduleController(ScheduleRepository repo) {
            repository = repo;
        }

        protected override void OnOpen() {
            Console.WriteLine("Um cliente acabou de se conectar. Estamos ficando famosos!");
            List<Professional> professionals = new List<Professional>();

            WebSocketResponseBuilder response = new WebSocketResponseBuilder();
            var data = new {
                professionals = repository.GetAllProfessionals()
            };
            response.SetData(data);

            Send(response.GetResponse());
        }

        protected override void OnMessage(MessageEventArgs e) {
            Console.WriteLine("Mensagem recebida de um cliente! Vamos respondê-lo com todo amor do mundo <3");
            // Processar mensagem
            JObject request = JObject.Parse(e.Data);
            ushort employeeId = request.Value<ushort>("employeeId");
            string method = request.Value<string>("method");
            string token = request.Value<string>("token");

            WebSocketResponseBuilder responseBuilder = new WebSocketResponseBuilder();
            responseBuilder.SetMethod(method);

            string responseString;
            switch (method) {
                case "GET_ALL":
                    ushort month = request.Value<ushort>("month");
                    ushort year = request.Value<ushort>("year");

                    responseString = GetEmployeeSchedule(employeeId, month, year, responseBuilder);

                    Send(responseString);
                    break;
                case "ADD_SCHEDULE":
                    Schedule addSchedule = null;
                    try {
                        addSchedule = RequestObjectFactory.BuildSchedule(request.SelectToken("schedule"));
                    } catch (InvalidRequestArgument ex) {
                        ex.response.SetMethod(method);
                        Send(ex.response.GetResponse());
                        return;
                    }

                    responseString = AddEmployeeSchedule(employeeId, addSchedule, responseBuilder);
                    if (responseBuilder.Success) {
                        Sessions.Broadcast(responseString);
                    } else {
                        Send(responseString);
                    }

                    break;
                case "DELETE_SCHEDULE":
                    ushort scheduleId = request.Value<ushort>("scheduleId");

                    responseString = DeleteSchedule(scheduleId, responseBuilder);
                    if(responseBuilder.Success) {
                        Sessions.Broadcast(responseString);
                    } else {
                        Send(responseString);
                    }

                    break;
                case "UPDATE_SCHEDULE":
                    Schedule updateSchedule = null;
                    try {
                        updateSchedule = RequestObjectFactory.BuildSchedule(request.SelectToken("schedule"));
                    } catch(InvalidRequestArgument ex) {
                        ex.response.SetMethod(method);
                        Send(ex.response.GetResponse());
                        return;
                    }

                    responseString = UpdateSchedule(employeeId, updateSchedule, responseBuilder);
                    if(responseBuilder.Success) {
                        Sessions.Broadcast(responseString);
                    } else {
                        Send(responseString);
                    }

                    break;
                default:
                    Send("Método WebSocket inválido.");
                    break;
            }
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

    }
}
