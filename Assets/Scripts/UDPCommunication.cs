using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using UnityOSC;
using System.Collections.Generic;

public struct UdpFromMedia
{
    public UdpFromMedia(float x, float y, float z) { yaw = x; pitch = y; roll = z; }
    public float yaw;
    public float pitch;
    public float roll;
}

public class UDPCommunication : MonoBehaviour
{

    private MediaController controller;
	private UdpClient socket;
	private int READ_PORT;
	private int WRITE_PORT;
	private bool sendPermission;
	private Vector3 oldRotation;

    void Start()
    {
		controller = GameObject.Find("360VideoPlayer").GetComponent<MediaController>();
		READ_PORT = 4040;
		WRITE_PORT = 7880;
		sendPermission = true;
		oldRotation = new Vector3 (0, 0, 0);
		socket = new UdpClient(READ_PORT);
		socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    }

	// wait for MediaController to be fully initialized
	public void startReceive() {
		socket.BeginReceive(new AsyncCallback(receive), socket);
	}

	// Put OSC Package contents in a string
	private string DataToString(List<object> data)
	{
		string buffer = "";
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		buffer += "\n";
		return buffer;
	}

	// listen on port and receive message
    void receive(IAsyncResult result)
    {
		// this is what had been passed into BeginReceive as the second parameter:
        UdpClient sock = result.AsyncState as UdpClient;

		IPEndPoint source = new IPEndPoint(IPAddress.Parse("127.0.0.1"), WRITE_PORT);

        // get the actual message and fill out the source:
        byte[] message = sock.EndReceive(result, ref source);

		OSCPacket osc_msg = OSCMessage.Unpack (message);

		controller.ParseMessage(osc_msg.Address, DataToString(osc_msg.Data));

		// schedule the next receive operation once reading is done:
		sock.BeginReceive(new AsyncCallback(receive), sock);
    }

	public void Send(byte[] mess) {
		IPEndPoint endpoint = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), WRITE_PORT);
		socket.Send (mess, mess.Length, endpoint);
	}

	void Update()
    {
		// if no change
		// keep in memory old value
		// set old value to new value when slider values changed on plugin side
		if (sendPermission) {
			IPEndPoint endpoint = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), WRITE_PORT);
			Vector3 rot = controller.GetCameraRotation ();
			if (rot.x != oldRotation.x || rot.y != oldRotation.y || rot.z != oldRotation.z) {
				
				OSCMessage packet = new OSCMessage ("/yawpitchroll");
				packet.Append (rot.y);
				packet.Append (rot.x);
				packet.Append (rot.z);
				packet.Pack ();
				byte[] mess = packet.BinaryData;
				socket.Send (mess, mess.Length, endpoint);

				oldRotation = rot;
			}
		}
	}

	// keep in memory old rotation
	public void SetOldRotation(Vector3 newRotation) {
		oldRotation = newRotation;
	}

	// allow / forbid send permission
	public void SetSendPermission(bool state) {
		this.sendPermission = state;
	}

	// close socket
	public void socketShutdown() {
		socket.Close ();
	}
}
