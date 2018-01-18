using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMapViewer : MonoBehaviour, IDragHandler {

	float momentumThreshold = 20;

	public void OnDrag (PointerEventData e) {
		if (e.dragging) {
			if (Mathf.Abs (e.delta.x) < momentumThreshold) {
				RectTransform rectT = gameObject.GetComponent<RectTransform> ();
				rectT.localPosition += new Vector3 (e.delta.x, 0, 0);

			}
		}

	}

}