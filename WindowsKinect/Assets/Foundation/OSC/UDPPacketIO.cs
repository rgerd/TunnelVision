using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

// UdpPacket provides packetIO over UDP
public class UDPPacketIO : MonoBehaviour {
	private UdpClient Sender;
	private bool socketsOpen;
	private string remoteHostName;
	private int remotePort;
	
	public void init(string hostIP, int remotePort){
		RemoteHostName = hostIP;
		RemotePort = remotePort;
		socketsOpen = false;
	}

	~UDPPacketIO() { if (IsOpen()) Close(); }
	
	public bool Open() {
		try {
			Sender = new UdpClient();
			socketsOpen = true;
			return true;
		} catch (Exception e) {
			Debug.LogWarning(e);
		}
		return false;
	}

	public void Close() {
		if(Sender != null)
			Sender.Close();
		socketsOpen = false;
	}
	
	public void OnDisable() { Close(); }

	public bool IsOpen() { return socketsOpen; }

	public void SendPacket(byte[] packet, int length) {
		if (!IsOpen()) Open();
		if (!IsOpen()) return;
		Sender.Send(packet, length, remoteHostName, remotePort);
	}

	public string RemoteHostName {
		get { return remoteHostName; }
		set { remoteHostName = value; }
	}

	public int RemotePort {
		get { return remotePort; }
		set { remotePort = value; }
	}
}
