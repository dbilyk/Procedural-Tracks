using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MapViewer : MonoBehaviour {

	Color32 bgCol1 = new Color32 (255, 142, 126, 205);
	Color32 bgCol2 = new Color32 (109, 218, 123, 205);
	Color32 bgCol3 = new Color32 (73, 210, 255, 205);
	Color32 bgCol4 = new Color32 (162, 131, 176, 205);

	[Tooltip ("Use this to control the size of the rendered line")]
	[SerializeField]
	float trackScale = 0.4f;

	[SerializeField]
	MapRenderer mapRenderer;

	[SerializeField]
	Selectable TrackPickerContainer;

	public delegate void PickerDrag ();
	public event PickerDrag OnPickerDrag;

	[SerializeField]
	MapCreator mapCreator;

	[SerializeField]
	UI_MapSelector mapSelector;

	[SerializeField]
	GameObject TrackDisplayPrefab,
	TrackDisplayContainer;

	Track _genTrack = new Track ();

	public Track SelectedTrack {
		get {
			return _genTrack;
		}
		private set {
			_genTrack = value;
		}
	}

	List<Track> userTracks;
	LineRenderer newTrackLine;
	GameObject NewTrackDisplay;
	void Awake () {
		NewTrackDisplay = GameObject.Instantiate (TrackDisplayPrefab, TrackDisplayContainer.transform);
		userTracks = User.SavedTracks;
		newTrackLine = NewTrackDisplay.GetComponentInChildren<LineRenderer> ();
		RenderSavedTracks ();

	}

	void OnEnable () {
		mapSelector.OnClickNewTrackCoins += RedrawTrack;
		mapSelector.OnClickNewTrackVideo += RedrawTrack;
	}

	void OnDisable () {
		mapSelector.OnClickNewTrackCoins -= RedrawTrack;
		mapSelector.OnClickNewTrackVideo -= RedrawTrack;
	}

	//helper to convert all vec2 to vec3
	List<Vector3> convertVec2toVec3 (List<Vector2> vector2List) {
		List<Vector3> result = new List<Vector3> ();
		foreach (Vector2 vector2 in vector2List) {
			result.Add ((Vector3) vector2);
		}
		return result;
	}

	//jsut thins out our track data because we dont want the line so high res
	List<T> thinData<T> (List<T> list, int thinningFactor) {
		List<T> result = new List<T> ();
		for (int i = 0; i < list.Count; i += thinningFactor) {
			result.Add (list[i]);
		}
		return result;
	}

	void RenderSavedTracks () {
		for (int i = 0; i < userTracks.Count; i++) {
			GameObject savedTrack = Instantiate (TrackDisplayPrefab, TrackDisplayContainer.transform);
			LineRenderer trackLine = savedTrack.GetComponentInChildren<LineRenderer> ();
			List<Vector2> lowResTrackPts = thinData (userTracks[i].TrackPoints, 4);
			lowResTrackPts = mapCreator.ShrinkData (lowResTrackPts, trackScale, trackScale);
			List<Vector3> lowResVec3 = convertVec2toVec3 (lowResTrackPts);
			trackLine.positionCount = lowResTrackPts.Count;
			trackLine.SetPositions (lowResVec3.ToArray ());
			savedTrack.GetComponent<Image> ().color = GetRandomColor ();

		}
	}

	Color32 GetRandomColor () {
		float rand = Random.value;
		if (rand < 0.25f) {
			return bgCol1;
		} else if (rand < 0.5f && rand >= 0.25f) {
			return bgCol2;
		} else if (rand < 0.75f && rand >= 0.5f) {
			return bgCol3;
		} else {
			return bgCol4;
		}

	}

	//called when newTrack Btn is clicked with either coins or Video
	void RedrawTrack () {
		mapRenderer.GenerateNewTrackData (SelectedTrack);
		List<Vector2> hiResTrackPts = mapCreator.ShrinkData (SelectedTrack.TrackPoints, trackScale, trackScale);
		List<Vector3> lowResTrackPts = thinData (convertVec2toVec3 (hiResTrackPts), 4);
		newTrackLine.positionCount = lowResTrackPts.Count;
		newTrackLine.SetPositions (lowResTrackPts.ToArray ());

	}

}