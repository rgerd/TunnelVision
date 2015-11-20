using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum JointType
{
	Head,
	SpineShoulder,
	ShoulderRight,
	ShoulderLeft,
	ElbowRight,
	ElbowLeft,
	SpineBase,
	WristRight,
	WristLeft,
	HipRight,
	HipLeft,
	KneeRight,
	KneeLeft,
	AnkleRight,
	AnkleLeft,
}

public enum BoneType {
	Core,
	RightArmTop,
	LeftArmTop,
	RightArmBottom,
	LeftArmBottom,
	RightLegTop,
	LeftLegTop,
	RightLegBottom,
	LeftLegBottom
}

public class BodyScript : MonoBehaviour {
	public GameObject head_prefab;
	public GameObject bone_prefab;

	public static GameObject head;
	public static GameObject[] bones;
	public static GameObject[] joints;


	void Start () {
		joints = new GameObject[15];
		foreach (JointType joint in Enum.GetValues(typeof (JointType))) {
			GameObject jointObj = new GameObject (joint.ToString ());
			jointObj.name = joint.ToString ();
			jointObj.transform.parent = transform;
			joints [(int)joint] = jointObj; 
		}

		bones = new GameObject[9];
		bones[(int)BoneType.Core] = addBone("Core", 1.5f, bone_prefab, JointType.SpineBase, JointType.SpineShoulder);

		bones[(int)BoneType.LeftArmTop] = addBone("LeftArmTop", 0.8f, bone_prefab, JointType.ShoulderLeft, JointType.ElbowLeft);
		bones[(int)BoneType.LeftArmBottom] = addBone("LeftArmBottom", 0.5f, bone_prefab, JointType.ElbowLeft, JointType.WristLeft);

		bones[(int)BoneType.RightArmTop] = addBone("RightArmTop", 0.8f, bone_prefab, JointType.ShoulderRight, JointType.ElbowRight);
		bones[(int)BoneType.RightArmBottom] = addBone("RightArmBottom", 0.5f, bone_prefab, JointType.ElbowRight, JointType.WristRight);

		bones[(int)BoneType.LeftLegTop] = addBone("LeftLegTop", 0.8f, bone_prefab, JointType.HipLeft, JointType.KneeLeft);
		bones[(int)BoneType.LeftLegBottom] = addBone("LeftLegBottom", 0.5f, bone_prefab, JointType.KneeLeft, JointType.AnkleLeft);

		bones[(int)BoneType.RightLegTop] = addBone("RightLegTop", 0.8f, bone_prefab, JointType.HipRight, JointType.KneeRight);
		bones[(int)BoneType.RightLegBottom] = addBone("RightLegBottom", 0.5f, bone_prefab, JointType.KneeRight, JointType.AnkleRight);
	}

	void Update () {
		float[] data = OSCReceiver.vars;
		if (data == null)
			return;
		for(int i = 0; i < data.Length; i++) {
			GameObject joint = joints[i / 3];
			float x = data[i]; float y = data[++i]; float z = data[++i];
			joint.transform.position = new Vector3(x, y, z);
		}
	}

	private GameObject addBone(string name, float radius, GameObject prefab, JointType joint1, JointType joint2) {
		GameObject bone = (GameObject) Instantiate(prefab, Vector3.zero, Quaternion.identity);
		bone.name = name; bone.transform.parent = transform;
		BoneScript script = bone.GetComponent("BoneScript") as BoneScript;
		script.radius = radius;
		script.joint1 = transform.FindChild (joint1.ToString()).gameObject;
		script.joint2 = transform.FindChild (joint2.ToString()).gameObject;
		return bone;
	}
}
