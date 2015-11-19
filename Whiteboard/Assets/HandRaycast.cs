using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class HandRaycast : MonoBehaviour {
    private GameObject leftWrist;
    private GameObject rightWrist;
    private GameObject leftHand;
    private GameObject rightHand;

    // Use this for initialization
    void Start () {
        leftWrist = SkeletonRender.body.transform.FindChild(Kinect.JointType.WristLeft.ToString()).gameObject;
        rightWrist = SkeletonRender.body.transform.FindChild(Kinect.JointType.WristRight.ToString()).gameObject;
        leftHand = SkeletonRender.body.transform.FindChild(Kinect.JointType.HandLeft.ToString()).gameObject;
        rightHand = SkeletonRender.body.transform.FindChild(Kinect.JointType.HandRight.ToString()).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(leftHand.transform.position, leftHand.transform.position - leftWrist.transform.position, out hit)) {

        }
	}
}
