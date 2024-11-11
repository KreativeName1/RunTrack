using System.ComponentModel;

namespace RunTrack
{
    public enum Geschlecht
    {
        [Description("Männlich")]
        Maennlich,
        [Description("Weiblich")]
        Weiblich,
        [Description("Divers")]
        Divers
    }

    public static class GeschlechtHelper
    {
        public static string GetDescription(Geschlecht geschlecht)
        {
            var type = geschlecht.GetType();
            var memInfo = type.GetMember(geschlecht.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}