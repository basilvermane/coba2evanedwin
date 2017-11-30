using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public float sensitivity = 1.0f;
    public Vector3 offset = new Vector3();
    public float cameraYOffset = 20.0f;

    private Vector3 initPos;
    private Quaternion initRot;

	// Use this for initialization
	void Start () {
        initPos = transform.localPosition;
        initRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
        /*print("left " +
            OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTrackedRemote) +
            "right " +
            OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote));*/
        if (Input.GetKeyDown(KeyCode.A))
            print(OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote).eulerAngles);
        transform.localPosition = initPos + (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote) * sensitivity);
        transform.localEulerAngles = /*initRot.eulerAngles +*/ OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote).eulerAngles
            - (Vector3.up * (Camera.main.transform.rotation.eulerAngles.y + cameraYOffset));
    }
}
