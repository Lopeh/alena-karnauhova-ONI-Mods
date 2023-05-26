using System;
using System.IO;
using System.Reflection;
using static Localization;

namespace Utils
{
    public static class LocalizationUtils
    {
        public static void Translate(Type root)
        {
            // Basic intended way to register strings, keeps namespace
            RegisterForTranslation(root);

            // Load user created translation files
            string path = LoadStrings();

            // Register strings without namespace
            // because we already loaded user transltions, custom languages will overwrite these
            LocString.CreateLocStringKeys(root, null);

            // Creates template for users to edit
            //GenerateStringsTemplate(root, Path.Combine(path, "strings_templates"));
        }

        private static string LoadStrings()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string locFile = Path.Combine(modPath, "translations", GetLocale()?.Code + ".po");
            if (File.Exists(locFile))
                OverloadStrings(LoadStringsFile(locFile, false));
            return modPath;
        }
    }
}
