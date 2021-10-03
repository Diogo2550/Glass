# GLASS - API
#### DOCUMENTAÇÃO
### **Introdução:**
Serão descritos, abaixo, todas as informações necessárias para a completa utilização de nossa API. Dentre essas informações, não estarão contidas a nível de implementação, porém, estará explicada de forma clara, e simples, os possíveis caminhos e tipos de mensagens enviadas/recebidas.

<br>

> Todas as informações presentes foram retiradas de nosso projeto UML, tanto do diagrama de caso de uso quanto do diagrama de classes.

<br>

## Modelo de retorno:
Todos os retornos de nossa API utilizarão da seguinte estrutura de respostas:
| nome                     | tipo       | descrição
|--------------------------|------------|------------------------
| code                     | int        | status code da resposta
| success                  | bool       | informa se houve sucesso na requisição
| data?                    | any (JSON) | informações da resposta
| error? (se houver falha) | string     | mensagem de erro da requisição.
| method (se WebSocket)    | string     | nome do evento de resposta

**onde:**<br>
data = objetos da resposta (descrito em cada endpoint).

**Obs:** é possível que seja respondido um código 200 e haja erro na requisição, por isso, fique sempre atento ao "success".

<br>

## Modelo de envio:
Os envios de dados para a nossa API deverão seguir a seguinte estrutura:

| nome   | tipo   | descrição
|--------|--------|--------------------
| method | string | método utilizado
| token  | string | token de usuário
| outros | any    | informações necessárias

<br>

---
## End-Points:
A estrutura de envio de dados à API será denominada no formato:

    URL - tipo:
	    Ação/evento:
		    Envio:
		    
            Retorno:

Abaixo serão listados todos os end-points de forma detalhada.

### HTTP

<details>
    <summary>/user/login - HTTP:</summary>
    POST:
        Envio:
            cpf: string;
            password: string;

    Retorno:
        se sucesso:
            code: 200,
            data: {
                token: string
            }
            
    se o login falha por causa do cliente (ex: senha/login errado):
        code: 200,
        error: string
</details>

<details>
    <summary>/user/signup - HTTP:</summary>
	POST:
		Envio:
			Employee

    Retorno:
        se sucesso:
            code: 200;
            data: {
                message: string;
            }
</details>

<details>
    <summary>/appointment/make - HTTP:</summary>
    POST:
        Envio:
            roomId: int;
            professionalId: int;
            patientId: int;
            appointment: {
                appointmentType: string;
                appointmentDate: DateTime;
            }

    Retorno:
        se sucesso:
            HTTP:
                code: int;
                data: string;
                success: bool;
            WebSocket:
                code: int;
                method: 'ADD_APPOINTMENT',
                success: bool,
                data: {
                    appointment: Appointment
                }
</details>

<details>
    <summary>/appointment/cancel - HTTP:</summary>
    POST:
        Envio:
            appointmentId: int;

    Retorno:
        se sucesso:
            HTTP:
                code: int;
                data: string;
                success: bool;
            WebSocket:
                code: 201;
                method: 'ADD_APPOINTMENT',
                success: true,
                data: {
                    appointment: Appointment;
                }
</details>


<br><br>

### /schedule - WebSocket:
**Obs:** caso haja erro (parâmetro “success”), será enviada uma mensagem - parâmetro ‘error’ - informando o erro. Caso contrário, será enviada uma notificação a todos os usuários com as alterações.<br>
**Obs2:** os retornos marcados como “ALL” serão enviados a todos os usuários conectados se, e somente se, a solicitação for bem sucedida. Caso contrário será enviado um erro ao remetente da mensagem.


<details>
    <summary>onOpen:</summary>
  
    Retorno:
        method = “OPEN”
        code: int;
            success: bool;
            error?: string;
            data?: {
                prefessionals: Array&lt;Professional&gt;;
            }
</details>
<br>
<details>
    <summary>GET</summary>

<details>
    <summary>GET_ALL:</summary>
    
    Envio:
        method = “GET_ALL”;
        token: string;
        employeeId: int;
        month: int; (janeiro = 0)
        year?: int;
        componentId?: string;

    Retorno:
        method = “GET_ALL”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            appointments: Array<Appointment>;
            schedules: Array<Schedule>;
            eventualSchedules: Array<Schedule>;
        }
</details>

<details>
    <summary>GET_SCHEDULES:</summary>
    
    Envio:
        method = “GET_SCHEDULES”;
        token: string;
        employeeId: int;
        componentId?: string;

    Retorno:
        method = “GET_SCHEDULES”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            schedules: Array<Schedule>;
        }
</details>

<details>
    <summary>GET_ADDITIONALS:</summary>

    Envio:
        method = “GET_ADDITIONALS”;
        token: string;
        employeeId: int;
        month: int; (janeiro = 0)
        year?: int;
        componentId?: string;

    Retorno:
        method = “GET_ADDITIONALS”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            appointments: Array<Appointment>;
            eventualSchedules: Array<Schedule>;
        }
</details>

<details>
    <summary>GET_PATIENT:</summary>

    Envio:
        method = “GET_PATIENT”;
        token: string;
        patientId: int;
        componentId?: string;

    Retorno:
        method = “GET_PATIENT”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            patient: Patient;
        }
</details>

<details>
    <summary>GET_ALL_PATIENTS:</summary>

    Envio:
        method = “GET_ALL_PATIENTS”;
        componentId?: string;

    Retorno:
        method = “GET_ALL_PATIENTS”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            patients: Array<Patient>;
        }
</details>

<details>
    <summary>GET_ALL_ROOMS:</summary>

    Envio:
        method = "GET_ALL_ROOMS";

    Retorno:
        method = "GET_ALL_ROOMS";
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            rooms: Array<Room>
        }
</details>
</details>
<br>
<details>
    <summary>ADD</summary>
<details>
    <summary>ADD_SCHEDULE:</summary>

    Envio:
        method = “ADD_SCHEDULE”;
        token: string;
        schedule: Schedule;
        employeeId: int;
        componentId?: string;

    Retorno - ALL:
        method = “ADD_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: int;
            schedule: Schedule;
        }
</details>

<details>
    <summary>ADD_EVENTUAL_SCHEDULE:</summary>
    
    Envio:
        method = “ADD_EVENTUAL_SCHEDULE”;
        token: string;
        eventualSchedule: EventualSchedule;
        employeeId: int;
        componentId?: string;

    Retorno - ALL:
        method = “ADD_EVENTUAL_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: int;
            eventualSchedule: EventualSchedule;
        }
</details>

<details>
    <summary>ADD_EMPLOYEE:</summary>

    Envio:
        method = “ADD_EMPLOYEE”;
        token: string;
        employee: Employee;
        componentId?: string;

    Retorno - ALL:
        method = “ADD_EMPLOYEE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employee: Employee;
		}
</details>

<details>
	<summary>ADD_PATIENT:</summary>

    Envio:
        method = “ADD_PATIENT”;
        token: string;
        patient: Patient;
        componentId?: string;

    Retorno - ALL:
        method = “ADD_PATIENT”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            patient: Patient;
        }
        Obs: no momento o patient não deverá ser enviado com Records.
</details>

<details>
    <summary>ADD_ROOM:</summary>

    Envio:
        method = "ADD_ROOM";
        Room: Room;
        componentId?: string;

    Retorno - ALL:
        method = “ADD_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            room: Room
        }
</details>
</details>
<br>
<details>
    <summary>DELETE</summary>
<details>
    <summary>DELETE_SCHEDULE:</summary>

    Envio:
        method = “DELETE_SCHEDULE”
        token: string;
        scheduleId: int;
        componentId?: string;

    Retorno - ALL:
        method = “DELETE_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            scheduleId: int
        }
</details>
        
<details>
    <summary>DELETE_EVENTUAL_SCHEDULE:</summary>

    Envio:
        method = “DELETE_EVENTUAL_SCHEDULE”
        token: string;
        eventualScheduleId: int;
        componentId?: string;

    Retorno - ALL:
        method = “DELETE_EVENTUAL_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            eventualScheduleId: int
        }
</details>

<details>
    <summary>DELETE_EMPLOYEE:</summary>

    Envio:
        method = “DELETE_EMPLOYEE”
        token: string;
        employeeId: int;
        componentId?: string;

    Retorno - ALL:
        method = “DELETE_EMPLOYEE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: int
        }
</details>

<details>
    <summary>DELETE_PATIENT:</summary>

    Envio:
        method = “DELETE_PATIENT”
        token: string;
        patientId: int;
        componentId?: string;

    Retorno - ALL:
        method = “DELETE_PATIENT”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            patientId: int
        }
</details>

<details>
    <summary>DELETE_ROOM:</summary>

    Envio:
        method = “DELETE_ROOM”
        token: string;
        roomId: int;
        componentId?: string;

    Retorno - ALL:
        method = “DELETE_ROOM”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            roomId: int
        }
</details>
</details>
<br>
<details>
    <summary>UPDATE</summary>
<details>
    <summary>UPDATE_SCHEDULE:</summary>

    Envio:
        method = “UPDATE_SCHEDULE”;
        token: string;
        componentId?: string;
        employeeId: int;
        schedule: Schedule;

    Retorno - ALL:
        method = “UPDATE_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: employeeId;
            schedule: Schedule;
        }
</details>
        
<details>
    <summary>UPDATE_EVENTUAL_SCHEDULE:</summary>

    Envio:
        method = “UPDATE_EVENTUAL_SCHEDULE”;
        token: string;
        componentId?: string;
        eventualSchedule: eventualSchedule;
        employeeId: int

    Retorno - ALL:
        method = “UPDATE_EVENTUAL_SCHEDULE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: int;
            eventualSchedule: eventualSchedule;
        }
</details>

<details>
    <summary>UPDATE_EMPLOYEE:</summary>

    Envio:
        method = “UPDATE_EMPLOYEE”;
        token: string;
        employee: Employee;
        componentId?: string;

    Retorno - ALL:
        method = “UPDATE_EMPLOYEE”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employee: Employee;
        }
</details>

<details>
    <summary>UPDATE_APPOINTMENT:</summary>

    Envio:
        method = “UPDATE_APPOINTMENT”;
        token: string;
        employeeId: int;
        roomId: int;
        patientId: int;
        appointment: Appointment;
        componentId?: string;

    Retorno - ALL:
        method = “UPDATE_APPOINTMENT”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            employeeId: int;
            appointment: Appointment;
        }
        Obs: método com problemas para atualizar as Foreign Key
</details>

<details>
    <summary>UPDATE_PATIENT:</summary>

    Envio:
        method = “UPDATE_PATIENT”;
        token: string;
        patient: Patient;
        componentId?: string;

    Retorno - ALL:
        method = “UPDATE_PATIENT”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            patient: Patient;
        }
</details>

<details>
    <summary>UPDATE_ROOM:</summary>

    Envio:
        method = “UPDATE_ROOM”;
        token: string;
        room: Room;
        componentId?: string;

    Retorno - ALL:
        method = “UPDATE_ROOM”;
        code: int;
        success: bool;
        componentId?: string;
        error?: string;
        data?: {
            room: Room;
        }
</details>
</details>
<br>

---
## Tipos de dados:

### 3.1. Objetos:
#### **Employee:**
| name | type
|------|-------
| id | int
| name |string
| cpf |string
| rg | string
| birthday | DateTime
| password |string
| admin | bool
| phone | string

<br>

#### **Appointment:**
| name | type
|------|-------
| id | int
| professional | Professional
| appointmentType | AppointmentType
| patient | Patient
| room | Room
| appointmentDate | DateTime

<br>

#### **Schedule:**
| name | type
|------|-------
| id | int
| startTime | TimeSpan
| endTime | TimeSpan
| frequency | Frequency
| dayOfWeek | DayOfWeek

<Br>

#### **EventualSchedule:**
| name | type
|------|-------
| id | int
| startTime | TimeSpan
| endTime | TimeSpan
| frequency | Frequency
| eventualDate | DateTime
| eventualState | EventualState

<br>

#### **Patient:**
| name | type
|------|-------
| id | int
| name | string
| cpf | string
| rg | string
| phone | string
| birthday | DateTime
| record | Record

<br>

#### **Room: **
| name | type
|------|-------
| id | int
| name | string

<br>

#### **Record:**
| name | type
|------|-------
| id | int
| allergies | string
| annotations | string

<br><br>

### 3.2. Tipos:

#### **TimeSpan:**
Um valor representando hora. Podem ser eles: ticks OU string.
| type | exemplo
|------|-------
| Tick: inteiro representando a hora atual. | ex: 360000000000 = 10:00:00
| String: uma string em formato “hh:mm:ss”. | ex: 12:50:23

<br><br>

### 3.3. ENUMS:

#### **AppointmentType - enum (a pensar):**
| name | value
|------|-------
| Default | 1

<br>

#### **Frequency:**
| name | value
|------|------
| TenMinute | 1
| FifteenMinute | 2
| ThirtyMinute | 3
| OneHour | 4
| TwoHour | 5
| ThreeHour | 6
| FiveHour | 7

<br>

#### **DayOfWeek:**
| name | value
|------|-------
| Sunday | 0
| Monday | 1
| Tuesday | 2
| Wednesday | 3
| Thursday | 4
| Friday | 5
| Saturday | 6

<br>

#### **EventualState:**
| name | value
|------|-------
| Blocked | 0
| Opened | 1