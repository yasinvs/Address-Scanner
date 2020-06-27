using MadMilkman.Ini;

namespace AddressScanner
{
    internal class SettingsClass
    {
        internal static IniOptions iniOptions = new IniOptions() { EncryptionPassword = "SettingsPasswordProtected" };
        internal static IniFile iniFile = new IniFile(iniOptions);

        internal static string txtPath;
        internal static int count = 0;

        internal static int timeout = 2000;
        internal static bool autoupdate = true;
    }
}