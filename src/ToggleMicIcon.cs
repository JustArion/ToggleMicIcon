using MelonLoader;
using System;
using System.Linq;
using ToggleMicIcon;
using UnityEngine;
using static ToggleMicIcon.BuildInfo;

[assembly: MelonInfo(typeof(Start), Name, ToggleMicIcon.BuildInfo.Version, Author, DownloadLink)]
[assembly: MelonColor(ConsoleColor.DarkCyan)]
[assembly: MelonGame("VRChat", "VRChat")]

namespace ToggleMicIcon
{
    internal static class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/ToggleMicIcon/releases";
        internal const string Name = "ToggleMicIcon";
        internal const string Version = "1.1.0";
    }
    internal sealed class Start : MelonMod 
    {
        private static MelonPreferences_Entry<bool> ToggleMic;
        private static GameObject _VoiceDotParentCached;
        private static GameObject VoiceDotParent => _VoiceDotParentCached ??= Resources.FindObjectsOfTypeAll<GameObject>()
            .First(go => go.name is "VoiceDotParent" && go.transform.GetChild(0) is {name: "VoiceDot"} && go.transform.GetChild(1) is {name: "VoiceDotDisabled"});
        
        //No need to Cache this, its not called often.
        private static Transform VoiceDot => VoiceDotParent.transform.Find("VoiceDot");
        private static Transform VoiceDotDisabled => VoiceDotParent.transform.Find("VoiceDotDisabled");

        public override void OnApplicationStart()
        {
            var cat = MelonPreferences.CreateCategory("ToggleMicIcon", "Toggle Mic Icon");
            ToggleMic = cat.CreateEntry("DisableMic", false, "Disable Microphone Icon");
            
            MelonLogger.Msg("Settings can be configured in UserData\\modprefs.ini or through 'UI Expansion Kit'");
            
            if (MelonHandler.Mods.All(mod => mod.Info.Name != "UI Expansion Kit")) UIXAdvert();
        }
        
        private static void UIXAdvert()
        {
            MelonLogger.Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            MelonLogger.Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            MelonLogger.Warning("or alternatively under the #finished-mods section in the VRChat Modding Group Discord.");
        }
        private static bool m_UIManagerStarted;
        private static void VRChat_OnUiManagerInit()
        {
            m_UIManagerStarted = true;
            ToggleMethod();
            MelonLogger.Msg("Sucessfully Initialized!");
        }

        
        [Credit("DDAkebono", "https://github.com/ddakebono/BTKSAImmersiveHud/blob/8b5968a7cf35398217ad14b86b316dc93fb705fe/BTKSAImmersiveHud.cs#L52")]
        [Modified]
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (scenesLoaded is null or > 2) return;
            scenesLoaded++;
            if (scenesLoaded != 2) return;
            VRChat_OnUiManagerInit();
            scenesLoaded = null;
        }
        private static byte? scenesLoaded = 0;
        
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