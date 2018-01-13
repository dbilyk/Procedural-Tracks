using UnityEngine;

public class ToggleSelf : MonoBehaviour {
	void Off (GameObject self) {
		self.SetActive (false);
	}

	void On (GameObject self) {
		self.SetActive (true);
	}
}