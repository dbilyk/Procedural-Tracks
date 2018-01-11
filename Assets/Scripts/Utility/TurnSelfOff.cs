using UnityEngine;

public class TurnSelfOff : MonoBehaviour {
	void Off (GameObject self) {
		self.SetActive (false);
	}

	void On (GameObject self) {
		self.SetActive (true);
	}
}