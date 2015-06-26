using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace StylusEditorClassifier
{
    public enum VsTheme
    {
        Unknown = 0,
        Light,
        Dark,
        Blue
    }

    public class ThemeUtil
    {
        private static readonly IDictionary<String, VsTheme> Themes = new Dictionary<String, VsTheme>()
        {
            {"de3dbbcd-f642-433c-8353-8f1df4370aba", VsTheme.Light},
            {"1ded0138-47ce-435e-84ef-9ec1f439b749", VsTheme.Dark},
            {"a4d6a176-b948-4b29-8c66-53c97a1ed7d0", VsTheme.Blue},
            {"", VsTheme.Blue}
        };

        public static VsTheme GetCurrentTheme()
        {
            string themeId = GetThemeId();
            if (string.IsNullOrWhiteSpace(themeId) == false)
            {
                VsTheme theme;
                if (Themes.TryGetValue(themeId, out theme))
                {
                    return theme;
                }
            }

            return VsTheme.Unknown;
        }

        public static String GetThemeId()
        {
            const string CategoryName = "General";
            const string ThemePropertyName = "CurrentTheme";
            string keyName = string.Format(@"Software\Microsoft\VisualStudio\12.0\{0}", CategoryName);

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    return (string) key.GetValue(ThemePropertyName, string.Empty);
                }
            }

            return null;
        }
    }
}
