﻿using Glass.Core.Exceptions;
using Glass.Core.Repository;
using Glass.Core.Util;
using Glass.Core.WebSocket.Builders;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Glass.Controllers.WebSocket {
    partial class ScheduleController : WebSocketBehavior {

        private readonly string[] acceptedMethods = new string[] { "GET_ALL", "ADD_SCHEDULE", "DELETE_SCHEDULE", "UPDATE_SCHEDULE" };
        private ScheduleRepository repository;

        public ScheduleController(ScheduleRepository repo) {
            repository = repo;
        }

        protected override void OnOpen() {
            Console.WriteLine("Um cliente acabou de se conectar. Estamos ficando famosos!");
            WebSocketResponseBuilder response = new WebSocketResponseBuilder();

            var data = new {
                professionals = repository.GetAllProfessionals()
            };
            response.SetData(data);

            Send(response.GetResponse());
        }

        protected override void OnMessage(MessageEventArgs e) {
            Console.WriteLine("Mensagem recebida de um cliente! Vamos respondê-lo com todo amor do mundo <3");
            WebSocketResponseBuilder responseBuilder = new WebSocketResponseBuilder();
            
            JObject request = JObject.Parse(e.Data);
            string method = request.Value<string>("method");

            // Verificar existencia do método
            if(!acceptedMethods.Contains(method)) {
                responseBuilder.SetError("Médoto inserido não encontrado.");
                responseBuilder.SetStatusCode(404);

                Send(responseBuilder.GetResponse());
                return;
            }

            // Processar mensagem
            MethodBase handler = this.GetType().GetMethod(method, BindingFlags.NonPublic|BindingFlags.Instance);
            handler.Invoke(this, new object[] { request, responseBuilder });
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
