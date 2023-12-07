using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

using InkboundModEnabler.Util;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

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
			public static Transform global_volume_label;



			private static Transform fov_transform;
			private static Transform fov_label;
			private static Slider fov_slider;
			private static float current_fov;

			private static GameObject camera_unit_follow;


			[HarmonyPostfix]
			public static void PostFix(ApplicationBinding __instance)
			{

				if ( !SettingsScreenFound )
				{

					if (__instance.name == "SettingsScreen")
					{
						// log.LogInfo($"SettingsScreen Found!");
						SettingsScreenFound = true;

						// get slider component

						global_volume_slider = __instance.transform.Find("Absolute BG/Inner/Volume Settings Page/Global Volume/Inner/Slider");
						global_volume_label = __instance.transform.Find("Absolute BG/Inner/Volume Settings Page/Global Volume/Inner/Volume Label");
						// log.LogInfo($"Slider & Label Captured: {global_volume_slider}");

					}

				}

				if( !WorldUIFound )
				{
					if ( __instance.name == "WorldUI")
					{
						// log.LogInfo($"WorldUI Found!");
						WorldUIFound = true;

						// create an instance of the global_volume_slider
						fov_transform = GameObject.Instantiate(global_volume_slider);
						fov_label = GameObject.Instantiate(global_volume_label);

						// set parent
						fov_transform.parent = __instance.transform.Find("WorldDecorationScreen");
						fov_label.parent = __instance.transform.Find("WorldDecorationScreen");

						// reposition to the right of the vestiges
						fov_transform.localPosition = new Vector3(600.0f, 25.0f, 0.0f);
						fov_label.localPosition = new Vector3(875.0f, 30.0f, 0.0f);

						fov_slider = fov_transform.GetComponent<Slider>();

						fov_transform.name = "FOV Slider";
						fov_label.name = "FOV Label";

						fov_slider.minValue = 5;
						fov_slider.maxValue = 90;
						fov_slider.value = 20; // Default is 20
						current_fov = 20;

						fov_label.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("FOV: " + System.Math.Round(current_fov, 1).ToString()) ;
						// fov_label.GetComponentInChildren<TMP_Text>().minWidth = 200;

						fov_slider.onValueChanged.AddListener(delegate { FOVChange(); });

						camera_unit_follow = SceneManager.GetSceneByName("World").GetRootGameObjects().First(gameObject => gameObject.name.Equals("CmvCamera_UnitFollow"));

					}
				}
				// log.LogInfo($"PostFix ApplicationBinding: {__instance.name}");

			}

			public static void FOVChange()
			{
				if ( fov_slider.value != current_fov )
				{
					current_fov = fov_slider.value;

					Cinemachine.CinemachineVirtualCamera virtual_camera = camera_unit_follow.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();

					virtual_camera.m_Lens.FieldOfView = current_fov;

					fov_label.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "FOV: " + System.Math.Round(current_fov, 1).ToString();

				}
			}



		}


	}



}