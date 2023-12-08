namespace Appointments.App.Models.Enum
{
    public enum AppointmentType
    {
        Descanso,
        Consulta,
        Extraccion,
        Endodoncia,
        Ortodoncia
    }

    public enum UserType
    {
        Doctor,
        Paciente
    }

    public enum AppointmentDuration
    {
        QuinceMinutos = 15,
        TreintaMinutos = 30,
        CuarentaYCincoMinutos = 45,
        UnaHora = 60,
        UnaHoraYQuinceMinutos = 75,
        UnaHoraYMedia = 90,
        DosHoras = 120
    }
}
