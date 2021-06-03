using System;
using System.Collections.Generic;
using System.Text;

namespace Notesieve
{
    static class NotesieveSettings
    {
        public static int opacity = 100;
        public static string keyHide = "F6";
        public static string keyScreenShot = "PrintScreen";
        public static bool autoLogin = false;

        public static bool isAdmin = false;
        public static bool autoStart = false;

        public static void SetSettings(int opacity, string keyHide, string keyScreenShot, bool autoLogin)
        {
            NotesieveSettings.opacity = opacity;
            NotesieveSettings.keyHide = keyHide;
            NotesieveSettings.keyScreenShot = keyScreenShot;
            NotesieveSettings.autoLogin = autoLogin;
        }
    }
}
