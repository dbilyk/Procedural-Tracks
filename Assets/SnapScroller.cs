using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScroller : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	enum motion {
		left,
		right
	}

	//the specific prefab used to render each track
	[SerializeField]
	GameObject ScrollItemPfab;

	//the actual rectTforms
	RectTransform ScrollContainerTform;
	RectTransform ScrollPrefabTform;

	//width of individual map module
	float mapWidth;
	motion moving;

	float dragStartT;
	float dragStopT;
	public float timeMomentumThresh = 0.5f;
	public float dragFactor = 0.95f;
	public float startLerpVelocity = 2f;

	public delegate void NewTargetTrack (int savedTrackIndex);
	public event NewTargetTrack OnNewTargetTrack;

	//get or set current scroll position in POSITIVE value
	float scrollPosition {
		get { return -ScrollContainerTform.localPosition.x; }
		set { ScrollContainerTform.localPosition = new Vector3 (-value, 0, 0); }
	}

	private int _lastTargetTrack;
	//closest track module index
	public int targetTrackIndex {
		get {
			int rawIndex = Mathf.RoundToInt (scrollPosition / mapWidth);
			int result;
			if (rawIndex <= 0) {
				result = 0;
			} else if (rawIndex >= trackCount) {
				result = trackCount;
			} else {
				result = rawIndex;
			}
			//call our event if the target track changed;
			// if (_lastTargetTrack != rawIndex) {
			// 	if (OnNewTargetTrack != null) OnNewTargetTrack (result);
			// 	_lastTargetTrack = result;

			// }
			return result;
		}
	}

	//value between -.5 and .5 signifying whci side of target index im at.
	float centerOffset {
		get {
			return (scrollPosition - (targetTrackIndex * mapWidth)) / mapWidth;
		}
	}

	int trackCount {
		get {
			return User.SavedTracks.Count;
		}
	}

	bool hitRight {
		get {
			if (targetTrackIndex == trackCount) {
				return true;
			} else {
				return false;
			}
		}
	}

	float velocity;

	void Start () {
		ScrollContainerTform = gameObject.GetComponent<RectTransform> ();
		ScrollPrefabTform = ScrollItemPfab.GetComponent<RectTransform> ();
		mapWidth = ScrollPrefabTform.rect.width;
	}

	public void OnBeginDrag (PointerEventData e) {
		StopAllCoroutines ();
		dragStartT = Time.unscaledTime;
		velocity = 0;
	}

	IEnumerator dragRoutine;
	public void OnDrag (PointerEventData e) {
		dragRoutine = drag (e);
		StopAllCoroutines ();
		StartCoroutine (dragRoutine);
	}

	List<float> velocitiesThisDrag = new List<float> ();
	IEnumerator drag (PointerEventData e) {
		while (true) {
			yield return new WaitForEndOfFrame ();
			velocity = -e.delta.x;
			velocitiesThisDrag.Add (velocity);
			scrollPosition += velocity;

		}
	}

	public void OnEndDrag (PointerEventData e) {
		StopAllCoroutines ();
		velocity = velocitiesThisDrag.Sum (x => x) / velocitiesThisDrag.Count;
		velocitiesThisDrag.Clear ();
		dragStopT = Time.unscaledTime;
		bool momentum = (dragStopT - dragStartT < timeMomentumThresh) ? true : false;
		moving = (velocity <= 0) ? motion.left : motion.right;
		Debug.Log (velocity);
		if (momentum) {
			StartCoroutine ("Momentum");
		} else {
			StartCoroutine ("NoMomentum");
		}
	}

	IEnumerator NoMomentum () {
		while (true) {
			lerpToSelectedIndex (targetTrackIndex);

			if (scrollPosition % (targetTrackIndex * mapWidth) < 0.1f) {
				StopAllCoroutines ();
			}

			yield return new WaitForEndOfFrame ();
		}
	}

	bool lerpStart;
	bool lerpEnd;
	int startingTargetIndex;
	IEnumerator Momentum () {
		lerpStart = false;
		lerpEnd = false;
		startingTargetIndex = targetTrackIndex;
		while (true) {
			if (Mathf.Abs (velocity) > startLerpVelocity) {
				if ((targetTrackIndex == 0 && centerOffset < 0) || (targetTrackIndex == trackCount && centerOffset > 0)) {
					velocity /= 1.5f;
					scrollPosition += velocity;
				} else {
					scrollPosition += velocity;
					velocity *= dragFactor;

				}

			} else {
				if (targetTrackIndex == startingTargetIndex && startingTargetIndex != 0 && startingTargetIndex != trackCount) {
					if (moving == motion.right) {
						lerpToSelectedIndex (targetTrackIndex + 1);
					}
					if (moving == motion.left) {
						lerpToSelectedIndex (targetTrackIndex - 1);
					}

				} else {
					if (startingTargetIndex == 0 && centerOffset >= 0) {
						lerpToSelectedIndex (targetTrackIndex + 1);
					} else if (startingTargetIndex == trackCount && centerOffset <= 0) {
						lerpToSelectedIndex (targetTrackIndex - 1);
					} else {

						lerpToSelectedIndex (targetTrackIndex);
					}

				}

				if (scrollPosition % (targetTrackIndex * mapWidth) < 0.1f) {
					lerpEnd = true;
				}
			}

			if (lerpEnd) {
				StopAllCoroutines ();
			}

			yield return new WaitForEndOfFrame ();
		}
	}

	void lerpToSelectedIndex (int index) {

		scrollPosition = Mathf.Lerp (scrollPosition, (index * mapWidth), Time.unscaledDeltaTime * 5);
	}

}