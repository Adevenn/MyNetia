namespace MyNetia.Model
{
    public static class UserSettings
    {
        private static Ini ini = new Ini(FileManager.CONFIG_PATH);
        public static string theme
        {
            get => ini.Read("THEME", "UserSettings");
            set => ini.Write("THEME", value, "UserSettings");
        }
        public static string serverIP
        {
            get => ini.Read("SERVER_IP", "UserSettings");
            set => ini.Write("SERVER_IP", value, "UserSettings");
        }
        public static string port
        {
            get => ini.Read("PORT", "UserSettings");
            set => ini.Write("PORT", value, "UserSettings");
        }
        public static string database
        {
            get => ini.Read("DATABASE", "UserSettings");
            set => ini.Write("DATABASE", value, "UserSettings");
        }
        public static string userName
        {
            get => ini.Read("USER_NAME", "UserSettings");
            set => ini.Write("USER_NAME", value, "UserSettings");
        }
        public static string password;
    }
}
