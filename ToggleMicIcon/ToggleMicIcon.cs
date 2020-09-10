using System;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ToggleMicIcon
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/ToggleMicIcon/releases";
        public const string Name = "ToggleMicIcon";

        public const string Version = "1.0.1";
    }
    public class ToggleMicIconClass : MelonMod 
    {
        //Was going to do some cool volume things but someone wanted it faster so just doing .SetActive XD
        public static bool ToggleMic;
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("ToggleMicIcon", "Toggle Mic Icon");
            MelonPrefs.RegisterBool("ToggleMicIcon", "DisableMic", false, "Disable Microphone Icon");
            ToggleMic = MelonPrefs.GetBool("ToggleMicIcon", "DisableMic");
            MelonLogger.Log("Settings can be configured in UserData\\modprefs.ini or through UIExpansionKit");
        }

        public override void VRChat_OnUiManagerInit()
        {
            ToggleMethod(ToggleMic);
        }
        public override void OnModSettingsApplied()
        {
            ToggleMic = MelonPrefs.GetBool("ToggleMicIcon", "DisableMic");
            ToggleMethod(ToggleMic);
        }
        public static void ToggleMethod(bool value)
        {
            try
            {
                GameObject MicIcon = GameObject.Find("/UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent").gameObject;
                MicIcon.SetActive(!value);
            }
            catch (Exception e)
            { MelonLogger.LogError("ToggleMethod Error: " + e); }


        }
    }
}
