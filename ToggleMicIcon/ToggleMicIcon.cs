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

        internal const string Version = "1.0.7";
    }
    internal sealed class ToggleMicIconClass : MelonMod 
    {
        private static MelonPreferences_Entry<bool> ToggleMic;
        private static HudVoiceIndicator cachedVoiceIndicator;
        private static HudVoiceIndicator HudVoiceIndicator => cachedVoiceIndicator ??= Object.FindObjectOfType<HudVoiceIndicator>();
        private static Transform VoiceDot => HudVoiceIndicator.gameObject.transform.Find("VoiceDot");
        private static Transform VoiceDotDisabled => HudVoiceIndicator.gameObject.transform.Find("VoiceDotDisabled");

        public override void OnApplicationStart()
        {
            var cat = MelonPreferences.CreateCategory("ToggleMicIcon", "Toggle Mic Icon");
            ToggleMic = (MelonPreferences_Entry<bool>) cat.CreateEntry("DisableMic", false, "Disable Microphone Icon");
            
            MelonLogger.Msg("Settings can be configured in UserData\\modprefs.ini or through 'UI Expansion Kit'");

            UIManager();

        }

        private void UIManager()
        {
            if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit" && VersionCheck(mod.Info.Version, "0.3.0")))
            {
                UIExpansionKit.API.ExpansionKitApi.OnUiManagerInit += () => { MelonCoroutines.Start(VRCUiManagerCoroutine()); };
                MelonLogger.Msg("Utilizing UI Expansion Kit Event.");
            }
            else if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit"))
            {
                MelonCoroutines.Start(VRCUiManagerCoroutine());
            }
            else
            {
                UIXAdvert();
                MelonCoroutines.Start(VRCUiManagerCoroutine());
            }
        }
        private void UIXAdvert()
        {
            MelonLogger.Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            MelonLogger.Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            MelonLogger.Warning("or alternatively under the #finished-mods section in the VRChat Modding Group Discord.");
        }
        private static bool m_UIManagerStarted;

        private static bool VersionCheck(string modVersion, string greaterOrEqual)
        {
            if (Version.TryParse(modVersion, out var owo) && Version.TryParse(greaterOrEqual, out var uwu)) return uwu.CompareTo(owo) <= 0;
            return false;
        }

        private void UiManagerInit()
        {
            m_UIManagerStarted = true;
            ToggleMethod();
            MelonLogger.Msg("Sucessfully Initialized!");
        }
        
        private byte? CheckCount = 0;
        private IEnumerator VRCUiManagerCoroutine()
        {
            MelonLogger.Msg("Starting VRCUiManagerCoroutine.");
            for (;;)
            {
                while (Object.FindObjectOfType<HudVoiceIndicator>() == null) yield return new WaitForSeconds(1);
                if (m_UIManagerStarted) yield break;
                m_UIManagerStarted = true;
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

        public override void OnPreferencesSaved() => ToggleMethod();

        private static void ToggleMethod()
        {
            try
            { 
                if (!m_UIManagerStarted) return;

                if (ToggleMic.Value) // We don't want to cache initial scales as other mods in the future might initially scale it to zero on Init.
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
