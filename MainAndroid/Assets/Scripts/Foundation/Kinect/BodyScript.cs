using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BodyScript : MonoBehaviour {
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

	public float rotationSpeed;
	private float rotationAcc;
	private int rotationDirection;
	public GameObject camHolder;
	public GameObject northWall;
	public GameObject southWall;
	public GameObject eastWall;
	public GameObject westWall;

	public GameObject head_prefab;
	public GameObject bone_prefab;

	public static GameObject head;
	public static GameObject[] bones;
	public static GameObject[] joints;
	public static string handLeftState;
	public static string handRightState;

	private Vector2 lean;

	void Start () {
		joints = new GameObject[15];
		foreach (JointType joint in Enum.GetValues(typeof (JointType))) {
			GameObject jointObj = new GameObject (joint.ToString ());
			jointObj.name = joint.ToString ();
			jointObj.transform.parent = transform;
			joints [(int)joint] = jointObj; 
		}

		head = (GameObject) Instantiate (head_prefab, Vector3.zero, Quaternion.identity);//new GameObject (joint.ToString ());
		head.name = "_Head";
		head.transform.parent = transform;

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

		Vector3 lean3 = joints [(int)JointType.SpineShoulder].transform.position - joints [(int)JointType.SpineBase].transform.position;
		lean = new Vector2 (lean3.x, lean3.z);
	}

	void Update () {
		float[] data = OSCReceiver.vars;
		if (data == null)
			return;

		int numJoints = data.Length / 3;

		for(int i = 0; i < numJoints; i++) {
			GameObject joint = joints[i];
			int _i = i * 3;
			float x = data[_i]; 
			float y = data[_i + 1]; 
			float z = data[_i + 2];
			Vector3 newPosition = new Vector3(x, y, z);
			//if(newPosition.

			joint.transform.position = new Vector3(x, y, z);
		}

		head.transform.localPosition = joints [0].transform.position;

		transform.position.Set (transform.position.x, -joints [(int)JointType.AnkleLeft].transform.position.y, transform.position.z);

		camHolder.transform.position = head.transform.position;

		handLeftState = OSCReceiver.hand_states [0];
		handRightState = OSCReceiver.hand_states [1];

		Vector3 lean3 = joints [(int)JointType.SpineShoulder].transform.position - joints [(int)JointType.SpineBase].transform.position;
		lean = new Vector2 (lean3.x, lean3.z) / bones[(int)BoneType.Core].transform.localScale.y;
		Debug.Log (lean.magnitude);
		if(Mathf.Abs (lean.y) > 0.5 || Mathf.Abs(lean.x) > 0.2) {
				transform.Translate(lean.x * 10 * Time.deltaTime, 0, lean.y * 10 * Time.deltaTime);
		}

		if (Input.GetAxis ("Mouse X") < -0.75) {
			rotationAcc = 90;
			rotationDirection = 1;
		} else if (Input.GetAxis ("Mouse X") > 0.75) {
			rotationAcc = 90;
			rotationDirection = -1;
		}

		if (rotationAcc > 0) {
			camHolder.transform.Rotate (0, rotationDirection * rotationSpeed * Time.deltaTime, 0);
			transform.Rotate (0, rotationDirection * rotationSpeed * Time.deltaTime, 0);
			rotationAcc -= rotationSpeed * Time.deltaTime;
		} 

		if(rotationAcc < 0) {
			camHolder.transform.Rotate (0, rotationDirection * rotationAcc * Time.deltaTime, 0);
			transform.Rotate (0, rotationDirection * rotationAcc * Time.deltaTime, 0);
			rotationAcc = 0;
			rotationDirection = 0;
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
