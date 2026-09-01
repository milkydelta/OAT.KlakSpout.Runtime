// KlakSpout - Spout video frame sharing plugin for Unity
// https://github.com/keijiro/KlakSpout

using UnityEngine;
using System.Runtime.InteropServices;

namespace OnAirTap.Klak.Spout
{
    static class PluginEntry
    {
        internal enum Event { Update, Dispose }

        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR_OSX

        internal static bool IsAvailable {
            get {
                return SystemInfo.graphicsDeviceType ==
                    UnityEngine.Rendering.GraphicsDeviceType.Direct3D11;
            }
        }

        [DllImport("OAT_KlakSpout_1")]
        internal static extern System.IntPtr GetRenderEventFunc();

        [DllImport("OAT_KlakSpout_1")]
        internal static extern System.IntPtr CreateSender(string name, int width, int height);

        [DllImport("OAT_KlakSpout_1")]
        internal static extern System.IntPtr CreateReceiver(string name);

        [DllImport("OAT_KlakSpout_1")]
        internal static extern System.IntPtr GetTexturePointer(System.IntPtr ptr);

        [DllImport("OAT_KlakSpout_1")]
        internal static extern int GetTextureWidth(System.IntPtr ptr);

        [DllImport("OAT_KlakSpout_1")]
        internal static extern int GetTextureHeight(System.IntPtr ptr);

        [DllImport("OAT_KlakSpout_1")] [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CheckValid(System.IntPtr ptr);

        [DllImport("OAT_KlakSpout_1")]
        internal static extern int ScanSharedObjects();

        [DllImport("OAT_KlakSpout_1")]
        internal static extern System.IntPtr GetSharedObjectName(int index);

        internal static string GetSharedObjectNameString(int index)
        {
            var ptr = GetSharedObjectName(index);
            return ptr != System.IntPtr.Zero ? Marshal.PtrToStringAnsi(ptr) : null;
        }

        #else

        internal static bool IsAvailable { get { return false; } }

        internal static System.IntPtr GetRenderEventFunc()
        { return System.IntPtr.Zero; }

        internal static System.IntPtr CreateSender(string name, int width, int height)
        { return System.IntPtr.Zero; }

        internal static System.IntPtr CreateReceiver(string name)
        { return System.IntPtr.Zero; }

        internal static System.IntPtr GetTexturePointer(System.IntPtr ptr)
        { return System.IntPtr.Zero; }

        internal static int GetTextureWidth(System.IntPtr ptr)
        { return 0; }

        internal static int GetTextureHeight(System.IntPtr ptr)
        { return 0; }

        internal static bool CheckValid(System.IntPtr ptr)
        { return false; }

        internal static int ScanSharedObjects()
        { return 0; }

        internal static string GetSharedObjectNameString(int index)
        { return null; }

        #endif
    }
}
