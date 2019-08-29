using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public class Echo : MonoBehaviour {

	//定义套接字
	Socket socket;
	//UGUI
	public InputField InputFeld;
	public Text text;
	//接收缓冲区
	byte[] readBuff = new byte[1024]; 
	string recvStr = "";

	//Connect回调
	public void ConnectCallback(IAsyncResult ar){
		try{
			Socket socket = (Socket) ar.AsyncState;
			socket.EndConnect(ar);
			Debug.Log("Socket Connect Succ ");
			socket.BeginReceive( readBuff, 0, 1024, 0,
				ReceiveCallback, socket);
		}
		catch (SocketException ex){
			Debug.Log("Socket Connect fail" + ex.ToString());
		}
	}
	
		//Receive回调
	public void ReceiveCallback(IAsyncResult ar){
		try {
			Socket socket = (Socket) ar.AsyncState;
			int count = socket.EndReceive(ar);
			string s = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
			recvStr = s + "\n" + recvStr;

			socket.BeginReceive( readBuff, 0, 1024, 0,
				ReceiveCallback, socket);
		}
		catch (SocketException ex){
			Debug.Log("Socket Receive fail" + ex.ToString());
		}
	}
	
	//点击发送按钮
	public void Send()
	{
		//Send
		string sendStr = InputFeld.text;
		byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
		socket.Send(sendBytes);
		//Recv
		byte[] readBuff = new byte[1024]; 
		int count = socket.Receive(readBuff);
		string recvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
		text.text = recvStr;
		//Close
		socket.Close();
	}
}
