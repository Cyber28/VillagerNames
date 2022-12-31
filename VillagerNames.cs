using BepInEx;
using BepInEx.Configuration;
using BerryLoaderNS;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace VillagerNamesMod
{
	[BepInPlugin("VillagerNames", "Villager Names", "1.0.0")]
	[BepInDependency("BerryLoader")]
	public class VillagerNames : BaseUnityPlugin
	{
		public static BepInEx.Logging.ManualLogSource L;
		public static Harmony Harmony;
		public static GameCard HoveredCard;
		public static FieldInfo NameOverrideField = typeof(CardData).GetField("nameOverride", BindingFlags.Instance | BindingFlags.NonPublic);

		private void Awake()
		{
			L = Logger;
			Harmony = new Harmony("VillagerNames");
			Harmony.PatchAll(typeof(Patches));
		}
	}

	public static class Patches
	{
		[HarmonyPatch(typeof(CardData), "GetExtraCardData", new Type[] { })]
		[HarmonyPostfix]
		public static void AddMoreExtraData(CardData __instance, ref List<ExtraCardData> __result)
		{
			if (__instance.MyCardType != CardType.Humans) return;
			__result.Add(new ExtraCardData("vn_name", (string)VillagerNames.NameOverrideField.GetValue(__instance)));
			return;
		}

		[HarmonyPatch(typeof(CardData), "SetExtraCardData", new Type[] { typeof(List<ExtraCardData>) })]
		[HarmonyPrefix]
		public static void SetMoreExtraData(CardData __instance, ref List<ExtraCardData> extraData)
		{
			if (__instance.MyCardType != CardType.Humans) return;
			var vnName = extraData.Find((ExtraCardData x) => x.AttributeId == "vn_name");
			if (vnName != null)
			{
				VillagerNames.NameOverrideField.SetValue(__instance, vnName.StringValue);
				extraData.Remove(vnName);
			}
		}

		[HarmonyPatch(typeof(WorldManager), "Update")]
		[HarmonyPostfix]
		public static void CardHoverCheck()
		{
			if (WorldManager.instance.HoveredCard == null || WorldManager.instance.HoveredCard.CardData.MyCardType != CardType.Humans) return;
			VillagerNames.HoveredCard = WorldManager.instance.HoveredCard;
			if (InputController.instance.GetKeyDown(Key.N))
			{
				ModalScreen.instance.Clear();
				ModalScreen.instance.SetTexts("Name your villager", "");
				// getting the text input in a modal SUCKED, tysm lopidav for helping and figuring it out before me
				GameObject stuff = UnityEngine.Object.Instantiate(DebugScreen.instance.CardRect.gameObject, ModalScreen.instance.ButtonParent);
				var csf = GameCanvas.instance.Modal.GetChild(1).gameObject.GetComponent<ContentSizeFitter>();
				csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
				stuff.transform.GetChild(1).GetComponent<ScrollRect>().vertical = false; // :^)

				RectTransform component2 = GameCanvas.instance.Modal.GetChild(1).gameObject.GetComponent<RectTransform>();
				if (component2 != null)
				{
					component2.sizeDelta = new Vector2(450f, 260f);
				}

				RectTransform textInput = stuff.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
				textInput.sizeDelta = new Vector2(0f, 500f);
				textInput.anchoredPosition = new Vector2(0f, -300f);

				Transform content = stuff.transform.GetChild(1).GetChild(0).GetChild(0);
				foreach (RectTransform item in content)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}

				stuff.SetActive(true);

				TMP_InputField component4 = stuff.transform.GetChild(0).GetComponent<TMP_InputField>();
				component4.text = "";
				ModalScreen.instance.ButtonParent.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
				((TextMeshProUGUI)component4.placeholder).text = "Name";

				// default modal size 600 100 // y *should* expand

				CustomButton renameButton = UnityEngine.Object.Instantiate(ModalScreen.instance.ButtonPrefab);
				renameButton.transform.SetParent(stuff.transform.GetChild(1).GetChild(0).GetChild(0));
				renameButton.transform.localPosition = Vector3.zero;
				renameButton.transform.localScale = Vector3.one;
				renameButton.transform.localRotation = Quaternion.identity;
				renameButton.TextMeshPro.text = "Okay";
				renameButton.Clicked += delegate
				{
					VillagerNames.NameOverrideField.SetValue(VillagerNames.HoveredCard.CardData, component4.text);
					component2.sizeDelta = new Vector2(600, 100);
					csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
					GameCanvas.instance.CloseModal();
				};

				CustomButton cancelButton = UnityEngine.Object.Instantiate(ModalScreen.instance.ButtonPrefab);
				cancelButton.transform.SetParent(stuff.transform.GetChild(1).GetChild(0).GetChild(0));
				cancelButton.transform.localPosition = Vector3.zero;
				cancelButton.transform.localScale = Vector3.one;
				cancelButton.transform.localRotation = Quaternion.identity;
				cancelButton.TextMeshPro.text = "Cancel";
				cancelButton.Clicked += delegate
				{
					VillagerNames.L.LogInfo("cancel");
					component2.sizeDelta = new Vector2(600, 100);
					csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
					GameCanvas.instance.CloseModal();
				};

				GameCanvas.instance.OpenModal();
			}
		}
	}
}