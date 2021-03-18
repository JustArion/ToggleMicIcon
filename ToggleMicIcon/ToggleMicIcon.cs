using MelonLoader;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace ToggleMicIcon
{
    internal sealed class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/ToggleMicIcon/releases";
        internal const string Name = "ToggleMicIcon";

        internal const string Version = "1.0.4";
    }
    internal sealed class ToggleMicIconClass : MelonMod 
    {
        private static bool ToggleMic;
        private static HudVoiceIndicator HudVoiceIndicator;

        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("ToggleMicIcon", "Toggle Mic Icon");
            MelonPreferences.CreateEntry("ToggleMicIcon", "DisableMic", false, "Disable Microphone Icon");
            ToggleMic = MelonPreferences.GetEntryValue<bool>("ToggleMicIcon", "DisableMic");
            MelonLogger.Msg("Settings can be configured in UserData\\modprefs.ini or through 'UI Expansion Kit'");
            
            if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit")) return;
            MelonLogger.Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            MelonLogger.Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            MelonLogger.Warning("or alternatively under the #finished-mods-✅ section in the VRChat Modding Group Discord.");
        }

        private static bool UIManagerStarted;
        public override void VRChat_OnUiManagerInit()
        {
            UIManagerStarted = true;
            HudVoiceIndicator = Object.FindObjectOfType<HudVoiceIndicator>();
            ToggleMethod(ToggleMic);
        }

        public override void OnPreferencesSaved()
        {
            if (!UIManagerStarted) return;
            ToggleMic = MelonPreferences.GetEntryValue<bool>("ToggleMicIcon", "DisableMic");
            ToggleMethod(ToggleMic);
        }

        private static void ToggleMethod(bool value)
        {
            try
            { 
                HudVoiceIndicator.enabled = !value;
                if (!value) return;
                HudVoiceIndicator.field_Private_Image_0.enabled = false; //Credits to Psychloor
                HudVoiceIndicator.field_Private_Image_1.enabled = false;
            }
            catch (Exception e)
            { MelonLogger.Error("ToggleMethod Error: " + e); }


        }
    }
}
