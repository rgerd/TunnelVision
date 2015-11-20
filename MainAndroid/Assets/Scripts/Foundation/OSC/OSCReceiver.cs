using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OSCReceiver : MonoBehaviour {
	public string remoteIP = "127.0.0.1";
	public int sendToPort = 8000;
	public int listenerPort = 9000;
	private Osc handler = null;
	public static float[] vars;
	
	void Start () {
		UDPPacketIO udp = (UDPPacketIO) GetComponent ("UDPPacketIO");
		udp.init (remoteIP, sendToPort, listenerPort);
		handler = (Osc) GetComponent ("Osc");
		handler.init(udp);
		handler.SetAllMessageHandler(AllMessageHandler);
		Debug.Log (remoteIP + " " + sendToPort + " " + listenerPort);
	}

	private bool running = false;
	public void AllMessageHandler(OscMessage oscMessage) {
		if (running) return;
		running = true;
		string msg = Osc.OscMessageToString (oscMessage).Substring (1);

		string[] _vals = msg.Split (' ');
		vars = new float[_vals.Length];
		for (int i = 0; i < vars.Length; i++)
			vars [i] = float.Parse (_vals [i]);
		running = false;
	}
}
