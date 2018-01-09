using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Header : MonoBehaviour {
	[SerializeField]
	Button SkinsBackBtn, TrackPickerBackBtn, SettingsBtn;

	[SerializeField]
	Text CurrencyCount;

	[SerializeField]
	UI_Home HomeUI;

	[SerializeField]
	UI_MapSelector MapSelectorUI;

	[SerializeField]
	UI_Skins SkinsUI;

}