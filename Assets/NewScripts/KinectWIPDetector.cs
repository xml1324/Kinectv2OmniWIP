using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinectWIPDetector : MonoBehaviour {
    [HideInInspector]
    public bool walkingMode = true;

    public Text walkingModeText;
    public GameObject transformTarget;
    public float moveSpeed;
    // public float highThreshold, lowThreshold;
    public float forwardYThreshold, leftXThreshold, rightXThreshold, backwardZThreshold, detectionMargin;

    bool _ini = false;
    float leftFootY_ini = 0, rightFootY_ini = 0;
    float leftFootX_ini = 0, rightFootX_ini = 0;
    float leftFootZ_ini = 0, rightFootZ_ini = 0;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        
        if (walkingMode)
        {
            Vector3 cameraYVector = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            cameraYVector.Normalize();

            Vector3 cameraYVector_Left = Quaternion.Euler(0, 270, 0) * cameraYVector;
            Vector3 cameraYVector_Right = Quaternion.Euler(0, 90, 0) * cameraYVector;
            Vector3 cameraYVector_Back = Quaternion.Euler(0, 180, 0) * cameraYVector;

            float cameraYRotation = Camera.main.transform.localEulerAngles.y;

            if (KinectManager.Instance.IsUserDetected())
            {
                long userID = KinectManager.Instance.GetPrimaryUserID();
                int footLeftJointType = (int)KinectInterop.JointType.AnkleLeft;
                int footRightJointType = (int)KinectInterop.JointType.AnkleRight;
                int kneeLeftJointType = (int)KinectInterop.JointType.KneeLeft;
                int kneeRightJointType = (int)KinectInterop.JointType.KneeRight;
                int hipLeftJointType = (int)KinectInterop.JointType.HipLeft;
                int hipRightJointType = (int)KinectInterop.JointType.HipRight;

                Vector3 leftFootPos = KinectManager.Instance.GetJointKinectPosition(userID, footLeftJointType);
                Vector3 rightFootPos = KinectManager.Instance.GetJointKinectPosition(userID, footRightJointType);
                Vector3 leftKneePos = KinectManager.Instance.GetJointKinectPosition(userID, kneeLeftJointType);
                Vector3 rightKneePos = KinectManager.Instance.GetJointKinectPosition(userID, kneeRightJointType);
                Vector3 leftHipPos = KinectManager.Instance.GetJointKinectPosition(userID, hipLeftJointType);
                Vector3 rightHipPos = KinectManager.Instance.GetJointKinectPosition(userID, hipRightJointType);


                if (KinectManager.Instance.IsJointTracked(userID, footLeftJointType) && KinectManager.Instance.IsJointTracked(userID, footRightJointType))
                {
                    if ((cameraYRotation > 44.99 && cameraYRotation < 45.01) || (cameraYRotation > 89.99 && cameraYRotation < 90.01) ||
                        (cameraYRotation > 134.99 && cameraYRotation < 135.01) || (cameraYRotation > 224.99 && cameraYRotation < 225.01) ||
                        (cameraYRotation > 269.99 && cameraYRotation < 270.01) || (cameraYRotation > 314.99 && cameraYRotation < 315.01))
                    {
                        _ini = false;
                    }


                    if (!_ini)
                    {
                        leftFootY_ini = leftFootPos.y;
                        rightFootY_ini = rightFootPos.y;
                        leftFootX_ini = leftFootPos.x;
                        rightFootX_ini = rightFootPos.x;
                        leftFootZ_ini = leftFootPos.z;
                        rightFootZ_ini = rightFootPos.z;

                        _ini = true;
                    }

                    // Front Tracking
                    if ((cameraYRotation >= 0 && cameraYRotation <= 90) || (cameraYRotation >= 270 && cameraYRotation <= 360))
                    {
                        // Original X-Z (Front)
                        if ((cameraYRotation >= 0 && cameraYRotation <= 45) || (cameraYRotation >= 315 && cameraYRotation <=360))
                        {
                            if (leftKneePos.z - leftHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.z - rightHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }
                        }

                        // Switched X-Z (Front-Side)
                        else if ((cameraYRotation > 45 && cameraYRotation <= 90))
                        {
                            if (leftKneePos.x - leftHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }
                        }

                        // Switched X-Z (Front-Side)
                        else if ((cameraYRotation >= 270 && cameraYRotation < 315))
                        {
                            if (leftKneePos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.x - rightHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }
                        }

                    }

                    // Back Tracking
                    else if (cameraYRotation > 90 && cameraYRotation < 270)
                    {
                        // Original X-Z (Back)
                        if (cameraYRotation >= 135 && cameraYRotation <= 225)
                        {
                            if (leftKneePos.z - leftHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.z - rightHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }
                        }

                        // Switched X-Z (Back)
                        else if (cameraYRotation > 90 && cameraYRotation < 135)
                        {
                            if (leftKneePos.x - leftHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }
                        }

                        // Switched X-Z (Back)
                        else if (cameraYRotation > 225 && cameraYRotation < 270)
                        {
                            if (leftKneePos.x - leftHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(leftFootY_ini - leftFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    leftFootY_ini = leftFootPos.y;
                                }
                            }

                            if (rightKneePos.x - rightHipPos.x < -detectionMargin)
                            {
                                if (Math.Abs(rightFootY_ini - rightFootPos.y) > forwardYThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector * moveSpeed);
                                    rightFootY_ini = rightFootPos.y;
                                }
                            }

                            if (leftFootPos.z - leftHipPos.z < -detectionMargin)
                            {
                                if (Math.Abs(leftFootZ_ini - leftFootPos.z) > leftXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Right * moveSpeed);
                                    leftFootZ_ini = leftFootPos.z;
                                }
                            }

                            if (rightFootPos.z - rightHipPos.z > detectionMargin)
                            {
                                if (Math.Abs(rightFootZ_ini - rightFootPos.z) > rightXThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Left * moveSpeed);
                                    rightFootZ_ini = rightFootPos.z;
                                }
                            }

                            if (leftFootPos.x - leftHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(leftFootX_ini - leftFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    leftFootX_ini = leftFootPos.x;
                                }
                            }

                            if (rightFootPos.x - rightHipPos.x > detectionMargin)
                            {
                                if (Math.Abs(rightFootX_ini - rightFootPos.x) > backwardZThreshold)
                                {
                                    transformTarget.GetComponent<Rigidbody>().AddForce(cameraYVector_Back * moveSpeed);
                                    rightFootX_ini = rightFootPos.x;
                                }
                            }
                        }
                        
                    }
                }

            }
        }

    }

    public void OnButtonClick()
    {
        walkingModeText.text = "Walking Mode: ON";
        walkingMode = true;
    }

    public void OffButtonClick()
    {
        walkingModeText.text = "Walking Mode: OFF";
        walkingMode = false;
    }
}
