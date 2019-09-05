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
	//checkRead列表
	List<Socket> checkRead = new List<Socket>();
	//点击连接按钮
	public void Connetion()
	{
		//Socket
		socket = new Socket(AddressFamily.InterNetwork,
			SocketType.Stream, ProtocolType.Tcp);
		//Connect
		socket.Connect("127.0.0.1", 8888);
		//socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, socket);
	}

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
		socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
	}

	//Send回调
	public void SendCallback(IAsyncResult ar){
		try {
			Socket socket = (Socket) ar.AsyncState;
			int count = socket.EndSend(ar);
			Debug.Log("Socket Send succ " + count);
		}
		catch (SocketException ex){
			Debug.Log("Socket Send fail" + ex.ToString());
		}

	}


	//public void Update(){
	//	text.text = recvStr;
	//}

	public void Update(){
		if(socket == null) {
			return;
		}
		//填充checkRead列表
		checkRead.Clear();
		checkRead.Add(socket);
		//select
		Socket.Select(checkRead, null, null, 1000);
		//check
		foreach (Socket s in checkRead){
			byte[] readBuff = new byte[1024];
			int count = socket.Receive(readBuff);
			string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
			text.text = recvStr;
		}
	}
}
