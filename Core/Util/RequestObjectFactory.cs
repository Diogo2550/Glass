using Glass.Core.Exceptions;
using Glass.Models;
using Glass.Models.Abstracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glass.Core.Util {
    static class RequestObjectFactory {
    
        public static Schedule BuildSchedule(JToken scheduleObject) {
            Schedule schedule = new Schedule();

            var id = scheduleObject.Value<string>("id");
            var startTime = scheduleObject.Value<string>("startTime");
            var endTime = scheduleObject.Value<string>("endTime");
            var frequency = scheduleObject.Value<string>("frequency");
            var dayOfWeek = scheduleObject.Value<string>("dayOfWeek");

            try {
                if (RegexValidator.IsNumber(startTime))
                    schedule.SetStartTime(new TimeSpan(long.Parse(startTime)));
                else
                    schedule.SetStartTime(TimeSpan.Parse(startTime));

                if (RegexValidator.IsNumber(endTime))
                    schedule.SetEndTime(new TimeSpan(long.Parse(endTime)));
                else
                    schedule.SetEndTime(TimeSpan.Parse(endTime));

                if(id != null && RegexValidator.IsNumber(id))
                    schedule.SetId(ushort.Parse(id));

                schedule.SetFrequency(ushort.Parse(frequency));
                schedule.SetDayOfWeek(ushort.Parse(dayOfWeek));
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return schedule;
        }

        public static EventualSchedule BuildEventualSchedule(JToken eventualScheduleObject) {
            EventualSchedule eventualSchedule = new EventualSchedule();

            var id = eventualScheduleObject.Value<string>("id");
            var startTime = eventualScheduleObject.Value<string>("startTime");
            var endTime = eventualScheduleObject.Value<string>("endTime");
            var frequency = eventualScheduleObject.Value<string>("frequency");
            var eventualDate = eventualScheduleObject.Value<string>("eventualDate");
            var eventualState = eventualScheduleObject.Value<string>("eventualState");

            try {
                if (id != null && RegexValidator.IsNumber(id))
                    eventualSchedule.SetId(ushort.Parse(id));

                if (RegexValidator.IsNumber(startTime))
                    eventualSchedule.SetStartTime(new TimeSpan(long.Parse(startTime)));
                else
                    eventualSchedule.SetStartTime(TimeSpan.Parse(startTime));

                if (RegexValidator.IsNumber(endTime))
                    eventualSchedule.SetEndTime(new TimeSpan(long.Parse(endTime)));
                else
                    eventualSchedule.SetEndTime(TimeSpan.Parse(endTime));

                eventualSchedule.SetFrequency(ushort.Parse(frequency));
                eventualSchedule.SetEventualDate(DateTime.Parse(eventualDate));
                eventualSchedule.SetEventualState(eventualState);
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return eventualSchedule;
        }
        
        public static Patient BuildPatient(JToken patientObject) {
            Patient patient = new Patient();

            var id = patientObject.Value<string>("id");
            var cpf = patientObject.Value<string>("cpf");
            var phone = patientObject.Value<string>("phone");
            var rg = patientObject.Value<string>("rg");
            var name = patientObject.Value<string>("fullName");
            var birthday = patientObject.Value<string>("birthday");

            try {
                if(id != null && RegexValidator.IsNumber(id))
                    patient.SetId(ushort.Parse(id));
                
                patient.SetName(name);
                patient.SetCPF(cpf);
                patient.SetPhone(phone);
                patient.SetRG(rg);
                patient.SetBirthday(DateTime.Parse(birthday));
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return patient;
        }

        public static Employee BuildEmployee(JToken employeeObject) {
            Employee employee = null;

            var id = employeeObject.Value<string>("id");
            var name = employeeObject.Value<string>("name");
            var birthday = employeeObject.Value<string>("birthday");
            var cpf = employeeObject.Value<string>("cpf");
            var rg = employeeObject.Value<string>("rg");
            var phone = employeeObject.Value<string>("phone");
            var password = employeeObject.Value<string>("password");
            var admin = employeeObject.Value<string>("admin");

            try {
                if (admin != null && admin.ToLower() == "true")
                    employee = new Admin();
                else
                    employee = new Professional();

                if(id != null && RegexValidator.IsNumber(id))
                    employee.SetId(ushort.Parse(id));

                employee.SetName(name);
                employee.SetCPF(cpf);
                employee.SetRG(rg);
                employee.SetPhone(phone);
                employee.SetPassword(password);
                employee.SetBirthday(DateTime.Parse(birthday));
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return employee;
        }
    
        public static Appointment BuildAppointment(JToken appointmentObject) {
            Appointment appointment = new Appointment();

            var id = appointmentObject.Value<string>("id");
            var appointmentType = appointmentObject.Value<string>("appointmentType");
            var appointmentDate = appointmentObject.Value<string>("appointmentDate");

            // TODO - Caso use o sistema de usar os objetos filhos. Preenche-los.
            try {
                if(id != null && RegexValidator.IsNumber(id))
                    appointment.SetId(ushort.Parse(id));

                appointment.SetAppointmentType(appointmentType);
                appointment.SetAppointmentDate(DateTime.Parse(appointmentDate));
            } catch (Exception) {
                throw new InvalidRequestArgument();
            }

            return appointment;
        }

    }
}
