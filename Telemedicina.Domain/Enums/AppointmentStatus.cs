using System;

namespace Telemedicina.Domain.Enums;

public enum AppointmentStatus
{
    Agendado = 0,
    EmEspera = 1,
    Atendido = 2,
    Faltou = 3,
    Cancelado = 4
}
