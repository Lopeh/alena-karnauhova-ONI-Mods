using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using static Localization;

namespace Utils
{
    public static class LocalizationPatch
    {
        public static string GetModPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static void Translate(Type root)
        {
            // Basic intended way to register strings, keeps namespace
            RegisterForTranslation(root);

            // Load user created translation files
            LoadStrings();

            // Register strings without namespace
            // because we already loaded user transltions, custom languages will overwrite these
            LocString.CreateLocStringKeys(root, null);

            // Creates template for users to edit
            GenerateStringsTemplate(root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
        }

        private static void LoadStrings()
        {
            string path = Path.Combine(GetModPath(), "translations", GetLocale()?.Code + ".po");
            if (File.Exists(path))
                OverloadStrings(LoadStringsFile(path, false));
        }
    }
}
