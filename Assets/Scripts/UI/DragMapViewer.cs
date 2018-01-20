using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMapViewer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	float maxDrag = 0.4f;
	float dragStart = 0;
	float dragStop = 0;
	float momentumThreshold = 20;
	float momentumTimeThresh = 0.5f;
	float velocity = 0f;
	int selectedTrackIndex {
		get {
			return GetSelectedTrackIndex ();
		}
	}

	float individualMapWidth;
	RectTransform rectT;
	int trackQty;

	void Start () {
		rectT = gameObject.GetComponent<RectTransform> ();
		individualMapWidth = rectT.GetChild (0).gameObject.GetComponent<RectTransform> ().rect.width;
		trackQty = User.SavedTracks.Count + 1;

	}
	float contPos {
		get {
			return rectT.localPosition.x;
		}
	}

	public void OnBeginDrag (PointerEventData e) {
		dragStart = Time.unscaledTime;
		//StopCoroutine ("Momentum");
		StopAllCoroutines ();
	}

	public void OnDrag (PointerEventData e) {
		if (e.dragging) {
			velocity = e.delta.x;
			//if dragged beyond last track
			if (contPos / individualMapWidth < -trackQty + 1) {
				velocity = velocity * (-1 + (((contPos / individualMapWidth) * 2) + ((trackQty) * 2)));
			}
			rectT.localPosition += new Vector3 (velocity / 2, 0, 0);

		}
	}

	public void OnEndDrag (PointerEventData e) {
		dragStop = Time.unscaledTime;
		lastVelAtZeroDrag = velocity;
		if (dragStop - dragStart < momentumTimeThresh && (selectedTrackIndex != 0 && selectedTrackIndex != trackQty - 1)) {
			StartCoroutine ("SteppedMove");
		}
		//if were on last element and we swiped back into the list, move normally
		if (dragStop - dragStart < momentumTimeThresh && (selectedTrackIndex == 0 && velocity < 0)) {
			StartCoroutine ("SteppedMove");
		}
		if (dragStop - dragStart < momentumTimeThresh && (selectedTrackIndex == trackQty - 1 && velocity >= 0)) {
			StartCoroutine ("SteppedMove");
		}
		if ((selectedTrackIndex == trackQty - 1 && velocity < 0) || (selectedTrackIndex == 0 && velocity >= 0)) {
			StartCoroutine ("bounceBack");
		}

	}

	float releaseVel;
	bool bounced;
	IEnumerator bounceBack () {
		releaseVel = velocity;
		bounced = false;
		while (true) {
			float locByCurrentIndex = (Mathf.Abs (contPos) % individualMapWidth) / individualMapWidth;
			if (!bounced && Mathf.Abs (velocity) > 0 && selectedTrackIndex == trackQty - 1) {
				velocity *= (-1 + (((contPos / individualMapWidth) * 2) + ((trackQty) * 2)));
				rectT.localPosition += new Vector3 (velocity, 0, 0);
			}

			if (!bounced && Mathf.Abs (velocity) > 0 && selectedTrackIndex == 0) {
				Debug.Log ("inside routing");
				velocity *= (1 - (contPos * 2) / individualMapWidth);
				rectT.localPosition += new Vector3 (velocity, 0, 0);
			}

			if (!bounced && Mathf.Abs (velocity) < 0.01f) {
				bounced = true;
				velocity = -releaseVel;
			}

			if (bounced && locByCurrentIndex > 0.05f && selectedTrackIndex == 0) {
				velocity *= locByCurrentIndex * 15;
			}

			yield return new WaitForEndOfFrame ();
		}

	}

	void bounceToLast () {

	}

	float lastVelAtZeroDrag;
	int selectedIndexLastIteration;
	bool toggle = false;

	IEnumerator SteppedMove () {
		toggle = false;
		selectedIndexLastIteration = GetSelectedTrackIndex ();
		while (true) {
			//float dragFactor = 1 - maxDrag;
			float dragFactor = 1 - (maxDrag * (Mathf.Pow (getDistToNearestMap () * 2, 2)));
			float speedUpFactor = 1f + (maxDrag * (0.8f * Mathf.Pow (getDistToNearestMap () * 2, 2)));

			if (selectedIndexLastIteration != GetSelectedTrackIndex ()) {
				lastVelAtZeroDrag = velocity;
			}
			selectedIndexLastIteration = GetSelectedTrackIndex ();
			if (!toggle && Mathf.Abs (velocity) < 2f && Mathf.Abs (contPos) % individualMapWidth > 2) {
				toggle = true;
				velocity = lastVelAtZeroDrag * 2;
			}

			if (!toggle && Mathf.Abs (velocity) >= 0.01f) {
				rectT.localPosition += new Vector3 (velocity, 0, 0);
				velocity *= dragFactor;
			}

			//slow down as a function of current location relative to nearest forward map;
			float locByCurrentIndex = (Mathf.Abs (contPos) % individualMapWidth) / individualMapWidth;
			if (toggle && locByCurrentIndex >= 0f && velocity > 0 && selectedTrackIndex != 0) {
				rectT.localPosition += new Vector3 (locByCurrentIndex * 15, 0, 0);
			}
			if (toggle && locByCurrentIndex >= 0f && velocity < 0 && selectedTrackIndex != trackQty) {
				rectT.localPosition -= new Vector3 (locByCurrentIndex * 15, 0, 0);

			}

			if (selectedTrackIndex == trackQty && locByCurrentIndex > 0.5) {
				rectT.localPosition += new Vector3 (locByCurrentIndex * 35, 0, 0);

			}

			if (toggle && locByCurrentIndex <= 0.05f || locByCurrentIndex >= 0.95f) {
				StopAllCoroutines ();
			}

			yield return new WaitForEndOfFrame ();

			// float posInOscillation = Mathf.Abs (contPos % individualMapWidth) / individualMapWidth;
			// if (velocity > 0 && posInOscillation > 0.7f) {
			// 	velocity /= speedUpFactor;
			// }
			// if (velocity > 0 && posInOscillation < 0.3f) {

			// }

		}
	}
	//States: Drag stopped and not

	float getDistToNearestMap () {
		float calc = Mathf.Abs (contPos % individualMapWidth) / individualMapWidth;
		if (calc <= 0.5f) {
			return calc;

		} else {
			return calc - 0.5f;
		}
	}

	void MoveContainer (float maxDrag) {
		//set initial velocity
		float settleFactor = 0.7f;
		//modulates exponentially as a function of remainder;
		float dragFactor = 1 - (maxDrag * (1.2f * Mathf.Exp ((getDistToNearestMap () * 2))));
		float nearestMap = (float) selectedTrackIndex;

		if (contPos > 0) {
			velocity = velocity * (1 - (contPos * 2) / individualMapWidth);
		}
		//snap to first map
		if (contPos > 0 && velocity == 0) {

		}
		//	if (contPos > 0 && velocity <= 0.000001f) {
		// 	velocity = -lastVelAtZeroDrag * settleFactor;
		// 	lastVelAtZeroDrag *= -settleFactor;
		// }

		if (contPos / individualMapWidth < -trackQty + 1) {
			velocity = velocity * (-1 + (((contPos / individualMapWidth) * 2) + ((trackQty) * 2)));
		}
		//exit condition
		if (-contPos % individualMapWidth < 1 && velocity == 0) {
			StopCoroutine ("Momentum");
		}
		rectT.localPosition += new Vector3 (velocity, 0, 0);
		velocity *= dragFactor;
	}

	int GetSelectedTrackIndex () {
		if (-contPos / individualMapWidth > trackQty) {
			return trackQty;

		} else if (-contPos / individualMapWidth < 0) {
			return 0;
		} else {
			return Mathf.RoundToInt (-contPos / individualMapWidth);
		}
	}

	IEnumerator Momentum () {
		while (true) {
			MoveContainer (maxDrag);
			yield return new WaitForSecondsRealtime (0.02f);
		}
	}
}