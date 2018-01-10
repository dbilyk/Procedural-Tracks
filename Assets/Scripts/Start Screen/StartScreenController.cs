using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenController : MonoBehaviour {
    public float introFogDistance;
    public float GameloopFogDistance;
    public Color32 GameloopFogColor;
    public IntroCarMove CarController;
    public Animator CowAnimController;
    public Rigidbody CowRoot;
    public List<Rigidbody> AllCowRBs = new List<Rigidbody> ();
    public List<Rigidbody> CowSpine = new List<Rigidbody> ();
    public List<Rigidbody> CowFeet = new List<Rigidbody> ();
    public Camera Cam;
    public Camera MainCam;
    public GameObject GameLoopLight;
    public GameObject CarShotStart;
    public GameObject CarShotEnd;
    public GameObject CowShotStart;
    public GameObject CowShotEnd;
    public GameObject IntroRabbit;
    public GameObject HomeScreenUI;

    Vector3 CarInitialPosition;
    Quaternion CarInitialRotation;
    Vector3 RabbitInitialPosition;
    Quaternion RabbitInitialRotation;

    Vector3 CarCamInitialPosition;
    Quaternion CarCamInitialRotation;

    public event EndIntro OnEndIntro;
    public delegate void EndIntro ();

    private bool homeUIOn = false;

    void OnEnable () {
        HomeScreenUI.SetActive (false);
        GameLoopLight.SetActive (false);
        QualitySettings.shadowDistance = 150;
        CrashCamTarget.transform.parent = CarController.transform;
        if (CarInitialPosition == Vector3.zero) {
            CarInitialPosition = CarController.CarBody.gameObject.transform.localPosition;
            CarInitialRotation = CarController.CarBody.gameObject.transform.localRotation;
            RabbitInitialPosition = IntroRabbit.transform.localPosition;
            RabbitInitialRotation = IntroRabbit.transform.localRotation;
            CarCamInitialPosition = CrashCamTarget.transform.localPosition;
            CarCamInitialRotation = CrashCamTarget.transform.localRotation;

        } else {
            CarController.CarBody.isKinematic = true;
            CarController.CarBody.gameObject.transform.localPosition = CarInitialPosition;
            CarController.CarBody.gameObject.transform.localRotation = CarInitialRotation;

            IntroRabbit.transform.localPosition = RabbitInitialPosition;
            IntroRabbit.transform.localRotation = RabbitInitialRotation;

            CrashCamTarget.transform.localPosition = CarCamInitialPosition;
            CrashCamTarget.transform.localRotation = CarCamInitialRotation;

        }
        CowAnimController.enabled = false;
        for (int i = 0; i < AllCowRBs.Count; i++) {
            AllCowRBs[i].isKinematic = true;
        }

        CarController.CarBody.isKinematic = false;
        RenderSettings.fogEndDistance = introFogDistance;
        RenderSettings.fogColor = Cam.backgroundColor;
        MainCam.enabled = false;
        StartCoroutine (CarShot ());
    }

    void Update () {
        // Debug.Log (Input.GetMouseButtonDown (0));
        //enable the home screen UI
        if (!homeUIOn && Input.GetMouseButtonDown (0) || Input.touchCount > 0) {
            if (this.OnEndIntro != null) {
                OnEndIntro ();

            }
        }
    }

    public float CarShotDuration;
    public float CarShotStartingFOV;
    public float CarShotEndingFOV;
    IEnumerator CarShot () {
        float CarCamPosLerp = 0;
        float CarCamRotLerp = 0;
        float CarCamFOVLerp = 0;

        //starts the car moving
        CarController.ApplyAccel = true;
        Cam.transform.position = CarShotStart.transform.position;
        Cam.transform.rotation = CarShotStart.transform.rotation;
        Cam.fieldOfView = CarShotStartingFOV;
        while (CarCamPosLerp < 1 || CarCamRotLerp < 1 || CarCamFOVLerp < 1) {
            yield return new WaitForFixedUpdate ();
            Cam.transform.position = Vector3.Slerp (CarShotStart.transform.position, CarShotEnd.transform.position, Mathf.SmoothStep (0, 1, CarCamPosLerp));
            Cam.transform.rotation = Quaternion.Slerp (CarShotStart.transform.rotation, CarShotEnd.transform.rotation, Mathf.SmoothStep (0, 1, CarCamRotLerp));
            Cam.fieldOfView = Mathf.Lerp (CarShotStartingFOV, CarShotEndingFOV, Mathf.SmoothStep (0, 1, CarCamFOVLerp));
            CarCamPosLerp += Time.fixedDeltaTime / CarShotDuration;
            CarCamRotLerp += Time.fixedDeltaTime / (CarShotDuration + 0.2f);
            CarCamFOVLerp += Time.fixedDeltaTime / (CarShotDuration + 1.5f);

        }
        StartCoroutine (CowShot ());

    }

    public float CowShotDuration;
    public float CowShotStartingFOV;
    public float CowShotEndingFOV;

    IEnumerator CowShot () {
        float CarCamPosLerp = 0;
        float CarCamRotLerp = 0;
        float CarCamFOVLerp = 0;
        CowAnimController.enabled = true;
        Cam.transform.position = CowShotStart.transform.position;
        Cam.transform.rotation = CowShotStart.transform.rotation;

        while (CarCamPosLerp < 1 || CarCamRotLerp < 1 || CarCamFOVLerp < 1) {
            yield return new WaitForFixedUpdate ();
            Cam.transform.position = Vector3.Slerp (CowShotStart.transform.position, CowShotEnd.transform.position, Mathf.SmoothStep (0, 1, CarCamPosLerp));
            Cam.transform.rotation = Quaternion.Slerp (CowShotStart.transform.rotation, CowShotEnd.transform.rotation, Mathf.SmoothStep (0, 1, CarCamRotLerp));
            Cam.fieldOfView = Mathf.Lerp (CowShotStartingFOV, CowShotEndingFOV, Mathf.SmoothStep (0, 1, CarCamFOVLerp));
            CarCamPosLerp += Time.fixedDeltaTime / CowShotDuration;
            CarCamRotLerp += Time.fixedDeltaTime / (CowShotDuration);
            CarCamFOVLerp += Time.fixedDeltaTime / (CowShotDuration - 1);

        }
        CowAnimController.enabled = false;

        StartCoroutine (CrashShot ());

    }

    public bool CarCollidedWIthCow = false;
    public bool StartSlowMo = false;
    public GameObject CrashCamTarget;
    IEnumerator CrashShot () {
        Cam.fieldOfView = 60;
        Cam.transform.position = CrashCamTarget.transform.position;
        Cam.transform.rotation = CrashCamTarget.transform.rotation;
        bool AddedExplosionToCow = false;
        while (true) {
            yield return new WaitForFixedUpdate ();
            Cam.transform.position = Vector3.Lerp (Cam.transform.position, CrashCamTarget.transform.position, Time.deltaTime * 2);
            Cam.transform.rotation = Quaternion.Lerp (Cam.transform.rotation, CrashCamTarget.transform.rotation, Time.deltaTime * 2);

            if (CarCollidedWIthCow && !AddedExplosionToCow) {
                for (int i = 0; i < AllCowRBs.Count; i++) {
                    CowAnimController.enabled = false;
                    AllCowRBs[i].isKinematic = false;
                    AddedExplosionToCow = true;
                }

                for (int i = 0; i < CowSpine.Count; i++) {
                    CowSpine[i].AddForce (-7, 0, -4f, ForceMode.Impulse);

                }
                for (int i = 0; i < CowFeet.Count; i++) {
                    CowFeet[i].AddRelativeForce (0, 0, -10, ForceMode.Impulse);
                }
                //trigger end intro event
                if (!homeUIOn && OnEndIntro != null) {
                    OnEndIntro ();
                }

                //CowSpine[CowSpine.Count - 1].AddRelativeForce(0,0,10,ForceMode.Impulse);
                CowSpine[CowSpine.Count - 1].AddRelativeTorque (new Vector3 (0, 5, 0), ForceMode.Impulse);

                Time.timeScale = 0.02f;
                Time.fixedDeltaTime /= 50;
                CrashCamTarget.transform.localPosition = new Vector3 (162.3f, 8f, 20);
                CrashCamTarget.transform.localRotation = Quaternion.Euler (8, -115, 0);
            }
            if (StartSlowMo && !AddedExplosionToCow) {
                for (int i = 0; i < AllCowRBs.Count; i++) {
                    CowAnimController.enabled = false;
                    AllCowRBs[i].isKinematic = false;
                    CrashCamTarget.transform.parent = gameObject.transform;
                }
            } else {
                //CowSpine[1].AddRelativeTorque(new Vector3(0.2f, 0, 0), ForceMode.Impulse);
                //CowSpine[5].AddRelativeTorque(new Vector3(3, 0.7f, 0), ForceMode.Acceleration);
                //CowSpine[CowSpine.Count - 1].AddRelativeTorque(new Vector3(0.1f, 0f, 0), ForceMode.Impulse);

            }

        }
    }

    void OnDisable () {
        if (GameLoopLight != null) {
            GameLoopLight.SetActive (true);

        }

        QualitySettings.shadowDistance = 40;
        MainCam.enabled = true;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        CarCollidedWIthCow = false;

        StartSlowMo = false;
        RenderSettings.fogEndDistance = GameloopFogDistance;
        RenderSettings.fogColor = GameloopFogColor;
    }

}