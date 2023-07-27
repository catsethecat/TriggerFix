using IPA;
using IPALogger = IPA.Logging.Logger;

using HarmonyLib;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using UnityEngine.XR;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace TriggerFix
{
    public class Config
    {
        public static Config Instance { get; set; }
    }

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        static InputDevice idL;
        static InputDevice idR;
        
        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config)
        {
            Instance = this;
            Log = logger;
            Config.Instance = config.Generated<Config>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Plugin.Log.Info("meow");
            idL = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
            idR = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            Harmony harmony = new Harmony("Catse.BeatSaber.TriggerFix");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

        [HarmonyPatch]
        internal class patches
        {

            [HarmonyPatch(typeof(VRControllersInputManager), "TriggerValue")]
            static void Postfix(XRNode node, ref float __result)
            {
                if(node == XRNode.LeftHand)
                {
                    idL.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float triggerValue);
                    if (triggerValue > 0.5f)
                        __result = 1.0f;
                }
                else
                {
                    idR.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float triggerValue);
                    if (triggerValue > 0.5f)
                        __result = 1.0f;
                }
            }

        }

    }

    
}
