using MelonLoader;
using System;
using Object = UnityEngine.Object;

namespace ToggleMicIcon
{
    public sealed class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/ToggleMicIcon/releases";
        public const string Name = "ToggleMicIcon";

        public const string Version = "1.0.2";
    }
    public sealed class ToggleMicIconClass : MelonMod 
    {
        private static bool ToggleMic;
        private static HudVoiceIndicator HudVoiceIndicator;
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("ToggleMicIcon", "Toggle Mic Icon");
            MelonPrefs.RegisterBool("ToggleMicIcon", "DisableMic", false, "Disable Microphone Icon");
            ToggleMic = MelonPrefs.GetBool("ToggleMicIcon", "DisableMic");
            MelonLogger.Log("Settings can be configured in UserData\\modprefs.ini or through UIExpansionKit");
        }

        public override void VRChat_OnUiManagerInit()
        {
            HudVoiceIndicator = Object.FindObjectOfType<HudVoiceIndicator>();
            ToggleMethod(ToggleMic);
        }
        public override void OnModSettingsApplied()
        {
            ToggleMic = MelonPrefs.GetBool("ToggleMicIcon", "DisableMic");
            ToggleMethod(ToggleMic);
        }
        private void ToggleMethod(bool value)
        {
            try
            { 
                HudVoiceIndicator.enabled = !value;
                if (!value) return;
                HudVoiceIndicator.field_Private_Image_0.enabled = false; //Credits to Psychloor
                HudVoiceIndicator.field_Private_Image_1.enabled = false;
            }
            catch (Exception e)
            { MelonLogger.LogError("ToggleMethod Error: " + e); }


        }
    }
}
