using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinectDataCollector : MonoBehaviour {

    public Text footLeftPosText, footRightPosText;
    public Text directionText;
    public Text cameraYRotationText;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (KinectManager.Instance.IsUserDetected())
        {
            long userID = KinectManager.Instance.GetPrimaryUserID();
            int footLeftJointType = (int)KinectInterop.JointType.AnkleLeft;
            int footRightJointType = (int)KinectInterop.JointType.AnkleRight;
            Vector3 leftFootPos = KinectManager.Instance.GetJointKinectPosition(userID, footLeftJointType);
            Vector3 rightFootPos = KinectManager.Instance.GetJointKinectPosition(userID, footRightJointType);

            float cameraYRotation = Camera.main.transform.eulerAngles.y;
            float n = cameraYRotation;

            string trackingDirection = "", detectionDirection = "";

            if ((n >= 0 && n <= 90) || (n >= 270 && n <= 360))
                trackingDirection = "Front Tracking";
            else
                trackingDirection = "Back Tracking";

            if ((n >= 0 && n <= 45) || (n >= 315 && n <= 360) || (n >= 135 && n <= 225))
                detectionDirection = "Original X-Z";
            else
                detectionDirection = "Switched X-Z";

            footLeftPosText.text = "LF Pos (Kinect): " + "x(" + leftFootPos.x.ToString("0.000") + "), y(" + leftFootPos.y.ToString("0.000") + "), z(" + leftFootPos.z.ToString("0.000") + ")";
            footRightPosText.text = "RF Pos (Kinect): " + "x(" + rightFootPos.x.ToString("0.000") + "), y(" + rightFootPos.y.ToString("0.000") + "), z(" + rightFootPos.z.ToString("0.000") + ")";

            cameraYRotationText.text = "Camera Y Rotation: " + cameraYRotation;
            directionText.text = "Direction: " + trackingDirection + ", " + detectionDirection;
        }
    }
}
