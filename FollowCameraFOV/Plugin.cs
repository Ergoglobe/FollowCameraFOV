using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

using InkboundModEnabler.Util;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;

namespace FollowCameraFOV
{
	[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[CosmeticPlugin]
	public class FollowCameraFOVPlugin : BaseUnityPlugin
	{
		public const string PLUGIN_GUID = "FollowCameraFOV";
		public const string PLUGIN_NAME = "Follow Camera FOV";
		public const string PLUGIN_VERSION = "0.0.1";
		public static readonly Harmony HarmonyInstance = new Harmony(PLUGIN_GUID);
		internal static ManualLogSource log;

		public static Transform global_volume_slider;


		private void Awake()
		{
			// Plugin startup logic
			log = Logger;
			HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
		}



		[HarmonyPatch(typeof(ApplicationBinding), nameof(ApplicationBinding.RegisterResource))]
		public static class AppBindingHook
		{

			public static bool SettingsScreenFound = false;
			public static bool WorldUIFound = false;
			public static Transform global_volume_slider;

			[HarmonyPostfix]
			public static void PostFix(ApplicationBinding __instance)
			{

				if ( !SettingsScreenFound )
				{

					if (__instance.name == "SettingsScreen")
					{
						log.LogInfo($"SettingsScreen Found!");
						SettingsScreenFound = true;

						// get slider component

						global_volume_slider = __instance.transform.Find("Absolute BG/Inner/Volume Settings Page/Global Volume/Inner/Slider");

						log.LogInfo($"Slider Captured: {global_volume_slider}");

					}

				}

				if( !WorldUIFound )
				{
					if ( __instance.name == "WorldUI")
					{
						log.LogInfo($"WorldUI Found!");
						WorldUIFound = true;

						global_volume_slider.parent = __instance.transform.Find("WorldDecorationScreen");
						global_volume_slider.localPosition = new Vector3(600.0f, 25.0f, 0.0f);

						InventoryHandScreenHook.Set_global_volume_slider(global_volume_slider);
					}
				}
				log.LogInfo($"PostFix ApplicationBinding: {__instance.name}");

			}
		}


		[HarmonyPatch(typeof(ShinyShoe.ClientApp), nameof(ShinyShoe.ClientApp.Initialize))]
		public static class ClientAppHook
		{
			[HarmonyPostfix]
			public static void PostFixInit(ClientApp __instance)
			{
				// this.FOVSlider = GameObject.Instantiate( )
				// log.LogInfo($"PostFix ClientApp: {__instance._screenSystem.OpenScreen}");



			}
		}



		[HarmonyPatch(typeof(ShinyShoe.InventoryHandScreen), nameof(ShinyShoe.InventoryHandScreen.OnRegisterVisual))]
		public static class InventoryHandScreenHook
		{

			private static Transform fov_slider;
			public static void Set_global_volume_slider( Transform global_volume_slider )
			{
				fov_slider = global_volume_slider;
			}

			[HarmonyPostfix]
			public static void Postfix(InventoryHandScreen __instance)
			{
				// this.FOVSlider = GameObject.Instantiate( )
				log.LogInfo($"Postfix InventoryHandScreen: {__instance.GetVisualResourceName()}");

				Transform FOVSlider = GameObject.Instantiate(fov_slider);
				FOVSlider.name = "FOV Slider";

				
				
			}
		}


	}



}