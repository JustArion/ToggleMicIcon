using MelonLoader;
using System;
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

        internal const string Version = "1.0.9";
    }
    internal sealed class ToggleMicIconClass : MelonMod 
    {
        private static MelonPreferences_Entry<bool> ToggleMic;
        private static HudVoiceIndicator cachedVoiceIndicator;
        private static HudVoiceIndicator HudVoiceIndicator => cachedVoiceIndicator ??= Object.FindObjectOfType<HudVoiceIndicator>();

        private static Transform cachedVoiceDot;
        private static Transform VoiceDot => cachedVoiceDot ??= HudVoiceIndicator.transform.Find("VoiceDot");
        
        private static Transform cachedVoiceDotDisabled;
        private static Transform VoiceDotDisabled => cachedVoiceDotDisabled ??= HudVoiceIndicator.transform.Find("VoiceDotDisabled");
        
        
        public override void OnApplicationStart()
        {
            ToggleMic = MelonPreferences.CreateCategory("ToggleMicIcon", "Toggle Mic Icon")
                .CreateEntry("DisableMic", false, "Disable Microphone Icon");
            
            ToggleMic.OnValueChanged += (_, b1) => { ToggleMethod(b1);};
            
            MelonLogger.Msg("Settings can be configured in UserData\\modprefs.ini or through 'UI Expansion Kit'");
            
            if (MelonHandler.Mods.All(mod => mod.Info.Name != "UI Expansion Kit")) UIXAdvert();
        }
        
        
        private static void UIXAdvert()
        {
            MelonLogger.Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            MelonLogger.Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            MelonLogger.Warning("or alternatively under the #finished-mods section in the VRChat Modding Group Discord.");
        }
        private static bool _UIManagerStarted;

        private static void VRChat_OnUiManagerInit()
        {
            _UIManagerStarted = true;
            ToggleMethod(ToggleMic.Value);
            MelonLogger.Msg("Sucessfully Initialized!");
        }

        
        [Credit("DDAkebono", "https://github.com/ddakebono/BTKSAImmersiveHud/blob/8b5968a7cf35398217ad14b86b316dc93fb705fe/BTKSAImmersiveHud.cs#L52")]
        [Modified]
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_ScenesLoaded is null or > 2) return;
            _ScenesLoaded++;
            if (_ScenesLoaded != 2) return;
            VRChat_OnUiManagerInit();
            _ScenesLoaded = null;
        }
        private static byte? _ScenesLoaded = 0;
        
        // public override void OnPreferencesSaved() => ToggleMethod();

        private static void ToggleMethod(bool hide)
        {
            if (!_UIManagerStarted) return;
            try
            {
                if (hide) // We don't want to cache initial scales as other mods in the future might initially scale it to zero on Init.
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
