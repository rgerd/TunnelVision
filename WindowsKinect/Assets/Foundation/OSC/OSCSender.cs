using UnityEngine;
using System.Collections;

public class OSCSender : MonoBehaviour {
	public string remoteIp = "127.0.0.1";
	public int sendToPort = 9000;
	private Osc handler = null;

	private SkeletonRender skeletonrender;
	
	void Start() {
		Debug.Log(remoteIp);
		UDPPacketIO udp = (UDPPacketIO) GetComponent("UDPPacketIO");
		udp.init(remoteIp, sendToPort);
		handler = (Osc) GetComponent("Osc");
		handler.init(udp);
		skeletonrender = GetComponent<SkeletonRender> ();
	}

    private bool running = false;
	void Update() {
		string message = skeletonrender.getString ();
		OscMessage oscM = null;
		oscM = Osc.StringToOscMessage("/" + message);
		handler.Send(oscM);
	}

	void OnDisable() {
		Debug.Log("Closing OSC UDP socket in OnDisable");
		handler.Cancel();
		handler = null;
	}
}
