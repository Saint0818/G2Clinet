using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.IO.Compression;
using Newtonsoft.Json;
using WebSocketSharp;
using SocketIO;
using zlib;
using GameStruct;

public delegate void TNetMessageProc (string data);

public class TSendBase {
	public int K1;
	public int K2;
}

public class TRecBase {
	public int K1;
	public int K2;
	public int R;
}

public class TSend1_1 : TSendBase{
	public string Identifier;
	public string sessionID;
}

public class TSend1_5 : TSendBase{
	public int Index;
}

public class TRec1_1 : TRecBase {
	public TTeam[] Teams;
}

public class TRec1_2 : TRecBase {
	public int RoomIndex;
}

public struct TRoomInfo {
	public int Index;
	public int PlayerNum;
}

public class TRec1_4 : TRecBase {
	public TRoomInfo[] Rooms;
}

public class GSocket
{
	private static GSocket instance = null;
	private SocketIOComponent WebSocket = null;
	private TNetMessageProc[,] NetMsgProcs = new TNetMessageProc[10, 20];
	private CallBack onConnectFunc = null;

    public static GSocket Get
	{
		get
		{
			if (instance == null) {
				instance = new GSocket();
				instance.Init();
			}

            return instance;
        }
    }

	public bool Connected {
		get {
			if (WebSocket != null)
				return WebSocket.IsConnected; 
			else
				return false;
		}
	}

	public void Init ()
	{
		//NetMsgProcs[1, 2] = netmsg_1_2;
	}

	public void Connect (CallBack callback)
	{
		onConnectFunc = callback;
		if (!WebSocket) {
			GameObject obj = (GameObject)Resources.Load("Prefab/SocketIO", typeof(GameObject));
			if (obj) {
				GameObject ws = (GameObject)GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
				ws.name = "SocketIo";
				WebSocket = ws.GetComponent<SocketIOComponent>();
				WebSocket.OnOpenEvent = OnConnected;
				WebSocket.OnMessageEvent = OnMessage;
				WebSocket.socket.OnClose += OnClose;

				WebSocket.Connect();
			}
		} else
		if (!WebSocket.IsConnected)
			WebSocket.Connect();
    }
	
	private void OnConnected(Packet packet) {
		TSend1_1 data = new TSend1_1();
		data.Identifier = GameData.Team.Identifier;
		data.sessionID = GameData.Team.sessionID;
		Send(1, 1, data, waitRec1_1);
	}
    
    public void Close ()
    {
		if (WebSocket != null && WebSocket.IsConnected)
			WebSocket.Close();
	}

	private void OnClose(object sender, CloseEventArgs e)
	{
			
	}

	private void OnMessage(SocketIOEvent e) {
		try {
			int k1 = 0;
			e.data.GetField(ref k1, "K1");
			int k2 = 0;
			e.data.GetField(ref k2, "K2");
			if (k1 >= 0 && k1 < NetMsgProcs.GetLength(0) &&
			    k2 >= 0 && k2 < NetMsgProcs.GetLength(1) &&
			    NetMsgProcs[k1, k2] != null)
				NetMsgProcs[k1, k2](e.data.ToString());
			else
				Debug.Log(string.Format("Protocol error {0}-{1}", k1, k2));
		} catch (Exception exc) {
			Debug.Log(exc.ToString());
		}
	}

    public void Send (byte Kind1, byte Kind2, TSendBase data = null, Action<JSONObject> action = null)
	{
		if (data == null)
			data = new TSendBase();

		data.K1 = Kind1;
		data.K2 = Kind2;
		string s = JsonConvert.SerializeObject(data);
		WebSocket.Emit("message", new JSONObject(s), action);
    }

	public string ReadZlib(byte[] data)
	{
		return System.Text.Encoding.UTF8.GetString(Decompress(data));
	}

	private Stream DecompressStream(Stream SourceStream)
	{
		try
		{
			MemoryStream stmOutput = new MemoryStream();
			ZOutputStream outZStream = new ZOutputStream(stmOutput);
			SourceStream.Position = 0;
			outZStream.Position = 0;

			byte[] zTemp = new byte[SourceStream.Length];

			SourceStream.Read(zTemp, 0, (int)SourceStream.Length);
			outZStream.Write(zTemp, 0, (int)SourceStream.Length);
			outZStream.finish();
			
			return stmOutput;
		}
		catch
		{
			return null;
		}
	}
	
	private byte[] Decompress(byte[] SourceByte)
	{
		try
		{
			MemoryStream stmInput = new MemoryStream(SourceByte);
			Stream stmOutPut = DecompressStream(stmInput);
            byte[] bytOutPut = new byte[stmOutPut.Length];
            stmOutPut.Position = 0;
            stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
            return bytOutPut;
        }
        catch
        {
            return null;
        }
    }
	
	public string OnHttpText(string text) {
		byte[] buf = JsonConvert.DeserializeObject <byte[]>(text);
		return System.Text.Encoding.UTF8.GetString(Decompress(buf));
	}
    
	private void waitRec1_1(JSONObject obj) {
		TRec1_1[] result = JsonConvert.DeserializeObject<TRec1_1[]>(obj.ToString());
		if (result.Length > 0 && result[0].R == 1) {
			if (onConnectFunc != null) {
				onConnectFunc();
				onConnectFunc = null;
			}
		}
	}
}
