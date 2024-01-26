using System.ComponentModel;

namespace Appointments.App.Models.Enum
{
    public enum AppointmentTypeEnum
    {
        [Description("Endodoncia")]
        Endodoncia,
        [Description("Ortodoncia")]
        Ortodoncia,
        [Description("Profilaxis")]
        Profilaxis,
        [Description("PPR")]
        PPR,
        [Description("PT")]
        PT,
        [Description("Perno")]
        Perno,
        [Description("Corona")]
        Corona,
        [Description("Incrustación")]
        Incrustacion,
        [Description("Exodoncia")]
        Exodoncia,
        [Description("Terceros")]
        Terceros,
        [Description("Raspado")]
        Raspado,
        [Description("Alargamiento")]
        Alargamiento,
        [Description("Restauración")]
        Restauracion,
        [Description("Restauración Interproximal")]
        RestauracionInterproximal,
        [Description("Diagnóstico")]
        Diagnostico,
        [Description("Blanqueamiento")]
        Blanqueamiento,
    }

    public enum UserTypeEnum
    {
        Doctor,
        Paciente
    }

    public enum AppointmentDurationEnum
    {
        [Description("15 minutos")]
        QuinceMinutos = 15,
        [Description("30 minutos")]
        TreintaMinutos = 30,
        [Description("45 minutos")]
        CuarentaYCincoMinutos = 45,
        [Description("1 hora")]
        UnaHora = 60,
        [Description("1h15 minutos")]
        UnaHoraYQuinceMinutos = 75,
        [Description("1h30 minutos")]
        UnaHoraYMedia = 90,
        [Description("2 horas")]
        DosHoras = 120,
        [Description("3 horas")]
        TresHoras = 180,
        [Description("4 horas")]
        CuatroHoras = 240,
        [Description("5 horas")]
        CincoHoras = 300,
        [Description(" 6 horas")]
        SeisHoras = 360
    }
}
