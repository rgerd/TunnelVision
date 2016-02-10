using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BodyScript : Photon.MonoBehaviour {
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
	private Vector2 normalLean;

	void Start () {


		//Connect to the main photon server. This is the only IP and port we ever need to set(!)
		if (!PhotonNetwork.connected)
		{
			Debug.Log("photon not connected yet. try connecting");
			PhotonNetwork.ConnectUsingSettings("1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
		}
		//Load name from PlayerPrefs
		PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + UnityEngine.Random.Range(1, 9999));
		//Set camera clipping for nicer "main menu" background
		Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;



	}


	void OnJoinedLobby()
	{
		Debug.Log("Joined Photon Lobby");
		if (PhotonNetwork.room != null)
			return; //Only when we're not in a Room
		
		//if (PhotonNetwork.GetRoomList().Length == 0)
		//{
		//    PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = 2 }, TypedLobby.Default);
		//}
		//else
		//{
		//PhotonNetwork.CreateRoom("Whiteboard");
		//PhotonNetwork.CreateRoom("Default");
		
		//PhotonNetwork.JoinRoom("Whiteboard");
		
		PhotonNetwork.JoinOrCreateRoom("Whiteboard", new RoomOptions() { maxPlayers = 10 }, TypedLobby.Default);
		//}
	}

	void OnJoinedRoom()
	{
		Debug.Log("JOIN whiteboard room");
		Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    
		
		joints = new GameObject[15];
		foreach (JointType joint in Enum.GetValues(typeof (JointType))) {
			GameObject jointObj = new GameObject (joint.ToString ());
			jointObj.name = joint.ToString ();
			jointObj.transform.parent = transform;
			joints [(int)joint] = jointObj; 
		}
		
		head = PhotonNetwork.Instantiate ("Head", Vector3.zero, Quaternion.identity, 0);
		//(GameObject) Instantiate (head_prefab, Vector3.zero, Quaternion.identity);//new GameObject (joint.ToString ());
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
		
		calcLean ();
		normalLean = lean;
		
		camHolder.transform.Rotate (Camera.main.transform.localRotation.eulerAngles * -1);
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
			if(x == 0 && y == 0 && z == 0)
				continue;
			//Vector3 newPosition = new Vector3(x, y, z);
			//newPosition.x = Mathf.Clamp(newPosition.x, westWall.transform.position.x, eastWall.transform.position.x);
			//newPosition.z = Mathf.Clamp(newPosition.x, southWall.transform.position.z, northWall.transform.position.z);

			joint.transform.localPosition = Vector3.Lerp (joint.transform.localPosition, new Vector3(x, y, z), Time.deltaTime * 10);
		}

		head.transform.localPosition = joints [0].transform.localPosition;

		if(joints [(int)JointType.AnkleLeft].transform.position.y < 0)
			transform.position.Set (transform.position.x, -joints [(int)JointType.AnkleLeft].transform.position.y, transform.position.z);

		camHolder.transform.position = head.transform.position;

		handLeftState = OSCReceiver.hand_states [0];
		handRightState = OSCReceiver.hand_states [1];



		calcLean ();
		if (lean.y < -0.3) {
			transform.Translate (0, 0, lean.y * 10 * Time.deltaTime);
		} else if (lean.y > 0.3) {
			transform.Translate (0, 0, lean.y * 10 * Time.deltaTime);
		}

		if (lean.x < -0.2) {
			transform.Translate (lean.x * 15 * Time.deltaTime, 0, 0);
		} else if (lean.x > 0.2) {
			transform.Translate (lean.x * 15 * Time.deltaTime, 0, 0);
		}
			/*
		if (Input.GetAxis ("Mouse X") < -0.75) {
			rotationAcc = 90;
			rotationDirection = -1;
		} else if (Input.GetAxis ("Mouse X") > 0.75) {
			rotationAcc = 90;
			rotationDirection = 1;
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
		}*/
	}

	private GameObject addBone(string name, float radius, GameObject prefab, JointType joint1, JointType joint2) {
		GameObject bone = PhotonNetwork.Instantiate ("Bone", Vector3.zero, Quaternion.identity, 0);
			//(GameObject) Instantiate(prefab, Vector3.zero, Quaternion.identity);
		bone.name = name; bone.transform.parent = transform;
		BoneScript script = bone.GetComponent("BoneScript") as BoneScript;
		script.radius = radius;
		script.joint1 = transform.FindChild (joint1.ToString()).gameObject;
		script.joint2 = transform.FindChild (joint2.ToString()).gameObject;
		return bone;
	}

	private void calcLean() {
		Vector3 lean3 = joints [(int)JointType.SpineShoulder].transform.localPosition - joints [(int)JointType.SpineBase].transform.localPosition;
		lean = new Vector2 (lean3.x, lean3.z) / bones [(int)BoneType.Core].transform.localScale.y;
	}
}
