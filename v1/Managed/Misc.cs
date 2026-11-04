namespace OnAirTap.Spout
{
    public static class Misc
    {
        public static string pluginFilename => "OAT_KlakSpout_1.dll";
        public static System.Action<string> RawLog = a => UnityEngine.Debug.Log(a);
        internal static void Log(string msg) { RawLog("[OAT-KlakSpout-101]: " + msg); }
    }
}
