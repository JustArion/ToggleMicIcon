using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToggleMicIcon
{
    internal static class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/ToggleMicIcon/releases";
        internal const string Name = "ToggleMicIcon";

        internal const string Version = "1.0.6";
    }
    internal sealed class ToggleMicIconClass : MelonMod 
    {
        private static bool ToggleMic;
        private static HudVoiceIndicator HudVoiceIndicator;
        private static Transform VoiceDot => HudVoiceIndicator.gameObject.transform.Find("VoiceDot");
        private static Transform VoiceDotDisabled => HudVoiceIndicator.gameObject.transform.Find("VoiceDotDisabled");

        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("ToggleMicIcon", "Toggle Mic Icon");
            MelonPreferences.CreateEntry("ToggleMicIcon", "DisableMic", false, "Disable Microphone Icon");
            ToggleMic = MelonPreferences.GetEntryValue<bool>("ToggleMicIcon", "DisableMic");
            MelonLogger.Msg("Settings can be configured in UserData\\modprefs.ini or through 'UI Expansion Kit'");

            if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit" && VersionCheck(mod.Info.Version, "0.2.6")))
            {
                UIExpansionKit.API.ExpansionKitApi.OnUiManagerInit += () => { MelonCoroutines.Start(VRCUiManagerCoroutine()); };
            }
            else if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit"))
            {
                MelonCoroutines.Start(VRCUiManagerCoroutine());
                return;
            }
            UIXAdvert();
            MelonCoroutines.Start(VRCUiManagerCoroutine());

        }
        private void UIXAdvert()
        {
            MelonLogger.Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            MelonLogger.Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            MelonLogger.Warning("or alternatively under the #finished-mods section in the VRChat Modding Group Discord.");
        }
        private static bool m_UIManagerStarted;

        private bool VersionCheck(string modVersion, string greaterOrEqual)
        {
            if (modVersion == greaterOrEqual) return true;
            if (Version.TryParse(modVersion, out var owo) && Version.TryParse(greaterOrEqual, out var uwu)) return owo.CompareTo(uwu) < 0;
            return false;
        }

        private void UiManagerInit()
        {
            if (typeof(VRCUiManager).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(VRCUiManager)) != null)
            {
                m_UIManagerStarted = true;
                ToggleMethod(ToggleMic);
                MelonLogger.Msg("Sucessfully Initialized!");
            }
            if (StartedOnce) return;
            StartedOnce = true;
            MelonCoroutines.Start(VRCUiManagerCoroutine());
        }

        private bool StartedOnce;
        private byte? CheckCount = 0;
        private IEnumerator VRCUiManagerCoroutine()
        {
            for (;;)
            {
                while (typeof(VRCUiManager).GetProperties().FirstOrDefault(p=> p.PropertyType == typeof(VRCUiManager)) == null) yield return null;
                m_UIManagerStarted = true;
                HudVoiceIndicator = Object.FindObjectOfType<HudVoiceIndicator>();
                if (HudVoiceIndicator == null)
                {
                    CheckCount++;
                    if (CheckCount >= 30)
                    {
                        MelonLogger.Warning("Mic Icon Indicator Was Not Found");
                        yield break;
                    }
                    yield return new WaitForSeconds(1);
                    continue;
                }
                UiManagerInit();
                CheckCount = null;
                yield break;
            }
        }

        public override void OnPreferencesSaved()
        {
            if (!m_UIManagerStarted) return;
            ToggleMic = MelonPreferences.GetEntryValue<bool>("ToggleMicIcon", "DisableMic");
            ToggleMethod(ToggleMic);
        }

        private static void ToggleMethod(bool value)
        {
            try
            { 
                // HudVoiceIndicator.enabled = !value;
                // if (!value) return;
                // HudVoiceIndicator.field_Private_Image_0.enabled = false; //Credits to Psychloor
                // HudVoiceIndicator.field_Private_Image_1.enabled = false;

                if (value) // We don't want to cache initial scales as other mods in the future might initially scale it to zero on Init.
                {
                    VoiceDot.localScale = Vector3.zero;
                    VoiceDotDisabled.localScale = Vector3.zero;
                    return;
                }
                VoiceDot.localScale = Vector3.one;
                VoiceDotDisabled.localScale = Vector3.one;

            }
            catch (Exception e)
            { MelonLogger.Error("ToggleMethod Error: " + e); }


        }
    }
}
