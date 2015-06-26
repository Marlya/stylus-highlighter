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
		private static readonly IDictionary<Guid, VsTheme> Themes = new Dictionary<Guid, VsTheme>()
        {
            { new Guid("de3dbbcd-f642-433c-8353-8f1df4370aba"), VsTheme.Light},
            { new Guid("1ded0138-47ce-435e-84ef-9ec1f439b749"), VsTheme.Dark},
            { new Guid("a4d6a176-b948-4b29-8c66-53c97a1ed7d0"), VsTheme.Blue}
        };

        public static VsTheme GetCurrentTheme()
        {
            Guid themeId = GetThemeId();
            VsTheme theme;
	        if (Themes.TryGetValue(themeId, out theme))
	        {
		        return theme;
	        }
		    return VsTheme.Unknown;
        }

        public static Guid GetThemeId()
        {
	        const string keyName = @"Software\Microsoft\VisualStudio\12.0\General";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            {
	            if (key == null)
	            {
		            return Guid.Empty;
	            }

	            string rawValue = (string) key.GetValue("CurrentTheme", null);
				return Guid.Parse(rawValue);
            }
        }
    }
}
