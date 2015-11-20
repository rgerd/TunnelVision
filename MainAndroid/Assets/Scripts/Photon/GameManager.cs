using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour
{
	
	// this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
	// read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
	public string playerPrefabName = "Sphere";
	
	
	void Start()
	{
		//PhotonNetwork.logLevel = NetworkLogLevel.Full;
		
		//Connect to the main photon server. This is the only IP and port we ever need to set(!)
		if (!PhotonNetwork.connected)
		{
			Debug.Log("photon not connected yet. try connecting");
			PhotonNetwork.ConnectUsingSettings("1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
		}
		
		
		//Load name from PlayerPrefs
		PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));
		
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
		
		PhotonNetwork.JoinOrCreateRoom("Whiteboard", new RoomOptions() { maxPlayers = 2 }, TypedLobby.Default);
		//}
	}
	
	
	void OnJoinedRoom()
	{
		Debug.Log("JOIN whiteboard room");
		Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    
		
		/*
        if (PhotonNetwork.countOfPlayers == 2)
        {
            OSCReceiver.playerID = "P2";
            OSCReceiver.anchor = new Vector3(0, 2, -30);
            Camera.main.transform.position = new Vector3(0, 2, -30);
            Camera.main.transform.LookAt(new Vector3(0, 2, 0));
            Camera.main.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            OSCReceiver.playerID = "P1";
            OSCReceiver.anchor = new Vector3(0, 2, 0);
            Camera.main.transform.position = new Vector3(0, 2, 0);
            Camera.main.transform.LookAt(new Vector3(0, 2, -30));
            Camera.main.transform.position = new Vector3(0, 0, 0);
        }

        Debug.Log("PLAYER ID " + OSCReceiver.playerID);
        GameObject head = PhotonNetwork.Instantiate("Head", new Vector3(0, 0, 0), Quaternion.identity, 0);
        head.gameObject.tag = OSCReceiver.playerID + "Head";
        OSCReceiver.head = head;

        Camera.main.transform.parent = head.gameObject.transform;

        GameObject body = PhotonNetwork.Instantiate("Body", new Vector3(0, 0, 0), Quaternion.identity, 0);
        body.gameObject.tag = OSCReceiver.playerID + "Body";
        OSCReceiver.body = body;


        GameObject leftHand = PhotonNetwork.Instantiate("LeftHand", new Vector3(0, 0, 0), Quaternion.identity, 0);
        leftHand.gameObject.tag = OSCReceiver.playerID + "LeftHand";
        OSCReceiver.leftHand = leftHand;

        GameObject rightHand = PhotonNetwork.Instantiate("RightHand", new Vector3(0, 0, 0), Quaternion.identity, 0);
        rightHand.gameObject.tag = OSCReceiver.playerID + "RightHand";
        OSCReceiver.rightHand = rightHand;

        GameObject leftElbow = PhotonNetwork.Instantiate("LeftElbow", new Vector3(0, 0, 0), Quaternion.identity, 0);
        leftElbow.gameObject.tag = OSCReceiver.playerID + "LeftElbow";
        OSCReceiver.leftElbow = leftElbow;

        GameObject rightElbow = PhotonNetwork.Instantiate("RightElbow", new Vector3(0, 0, 0), Quaternion.identity, 0);
        rightElbow.gameObject.tag = OSCReceiver.playerID + "RightElbow";
        OSCReceiver.rightElbow = rightElbow;
        */
	}
	
	
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			//level = (level + 1) % 2;
			PhotonNetwork.LeaveRoom();
			//Application.LoadLevel(1);
		}
	}
	
	IEnumerator OnLeftRoom()
	{
		//Easy way to reset the level: Otherwise we'd manually reset the camera
		
		Debug.Log("LEAVE whiteboard room");
		//PhotonNetwork.JoinOrCreateRoom("Default", new RoomOptions() { maxPlayers = 2 }, TypedLobby.Default);
		Application.LoadLevel(1);
		
		//Wait untill Photon is properly disconnected (empty room, and connected back to main server)
		while (PhotonNetwork.room != null || PhotonNetwork.connected == false)
			yield return 0;
		
		
		
	}
	
	
	void OnDisconnectedFromPhoton()
	{
		Debug.LogWarning("OnDisconnectedFromPhoton");
	}
	
	
	
}
