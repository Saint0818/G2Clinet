using System;
using System.Collections.Generic;
using System.IO;
using GameStruct;
using Newtonsoft.Json;
using SocketIO;
using UnityEngine;
using WebSocketSharp;
using zlib;

public delegate void TNetMessageProc (string data);

public struct TNetData {
	public int K1;
	public int K2;
	public string Data;
}

public class TSendBase {
	public int K1;
	public int K2;
}

public class TRecBase {
	public int K1;
	public int K2;
	public int R;
	public int Index;
}

public class TSend1_1 : TSendBase{
	public string Identifier;
	public string sessionID;
	public float X;
	public float Z;
}

public class TSend1_5 : TSendBase{
	public int Index;
}

public class TSend2_1 : TSendBase{
	public int Kind;
	public TScenePlayer ScenePlayer = new TScenePlayer();
}

public class TRec1_1 : TRecBase {
	public string Name;
	public TTeam[] Teams;
}

public class TRec1_3 : TRecBase {
	public bool IsDisconnect;
}

public struct TRoomInfo {
	public int Index;
	public int PlayerNum;
}

public class TRec1_4 : TRecBase {
	public TRoomInfo[] Rooms;
}

public class TRec1_5 : TRecBase {
	public int PIndex;
	public bool IsStart;
	public int[] Scores;
	public TTeam Team;
	public TTeam[] Teams;
	public TScenePlayer ScenePlayer;
	public TScenePlayer[] ScenePlayers;
}

public class TRec2_1 : TRecBase{
	public string Name = "";
	public TScenePlayer ScenePlayer = new TScenePlayer();
}

public class GSocket : KnightSingleton<GSocket> {
	private static GSocket instance = null;
	private SocketIOComponent WebSocket = null;
	private TNetMessageProc[,] NetMsgProcs = new TNetMessageProc[10, 20];
	private CallBack onConnectFunc = null;

	private List<TNetData> netCommands = new List<TNetData>();

	public bool Connected {
		get {
			if (WebSocket != null)
				return WebSocket.IsConnected; 
			else
				return false;
		}
	}

	void FixedUpdate() {
		for (int i = netCommands.Count-1; i >= 0; i--) {
			NetMsgProcs[netCommands[i].K1, netCommands[i].K2](netCommands[i].Data);
			netCommands.RemoveAt(i);
		}
	}

	protected override void Init() {
		DontDestroyOnLoad(gameObject);
		NetMsgProcs[1, 1] = netmsg_1_1;
		NetMsgProcs[1, 3] = netmsg_1_3;
		NetMsgProcs[1, 5] = netmsg_1_5;

		NetMsgProcs[2, 1] = netmsg_2_1;
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
				WebSocket.transform.parent = this.transform;
			}
		} else
		if (!WebSocket.IsConnected)
			WebSocket.Connect();
    }

	public void SendLoginRTS(CallBack callback) {
		onConnectFunc = callback;
		TSend1_1 data = new TSend1_1();
		data.Identifier = GameData.Team.Identifier;
		data.sessionID = GameData.Team.sessionID;
		data.X = GameData.ScenePlayer.X;
		data.Z = GameData.ScenePlayer.Z;

		Send(1, 1, data, waitRec1_1);
	}

	private void OnConnected(Packet packet) {
		SendLoginRTS(onConnectFunc);
	}
    
    public void Close ()
    {
		if (WebSocket != null && WebSocket.IsConnected)
			WebSocket.Close();
	}

	private void OnClose(object sender, CloseEventArgs e)
	{
		GameData.RoomIndex = -1;
		GameData.IsLoginRTS = false; 

		TNetData command = new TNetData();
		command.K1 = 1;
		command.K2 = 3;
		command.Data = "{R: 22, IsDisconnect: true}";
		netCommands.Add(command);
    }

	private void OnMessage(SocketIOEvent e) {
		try {
			int k1 = 0;
			e.data.GetField(ref k1, "K1");
			int k2 = 0;
			e.data.GetField(ref k2, "K2");
			if (k1 >= 0 && k1 < NetMsgProcs.GetLength(0) &&
			    k2 >= 0 && k2 < NetMsgProcs.GetLength(1) &&
			    NetMsgProcs[k1, k2] != null) {
				TNetData commond = new TNetData();
				commond.K1 = k1;
				commond.K2 = k2;
				commond.Data = e.data.ToString();
				netCommands.Add(commond);
			}
			else
				Debug.Log(string.Format("Protocol error {0}-{1}", k1, k2));
		} catch (Exception exc) {
			Debug.Log(exc.ToString());
		}
	}

    public void Send (byte Kind1, byte Kind2, TSendBase data = null, Action<JSONObject> action = null)
	{
		if (WebSocket && WebSocket.IsConnected) {
			if (data == null)
				data = new TSendBase();

			data.K1 = Kind1;
			data.K2 = Kind2;
			string s = JsonConvert.SerializeObject(data);
			WebSocket.Emit("message", new JSONObject(s), action);
		} else
			Debug.Log("Is disconnected.");
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
		if (result.Length > 0) {
			if (result[0].R == 1) {
				GameData.IsLoginRTS = true;
				if (onConnectFunc != null) {
					onConnectFunc();
					onConnectFunc = null;
				}
			} else
				Debug.Log("1_1 error " + result[0].R.ToString());
		}
	}

	private void netmsg_1_1(string data) {
		TRec1_1 result = JsonConvert.DeserializeObject<TRec1_1>(data);
		switch (result.R) {
		case 21:
			UIHint.Get.ShowHint(result.Name + " online", Color.blue);
			break;
		case 22:
			UIHint.Get.ShowHint(result.Name + " offline", Color.red);
            break;
        }
    }

    private void netmsg_1_3(string data) {
		TRec1_3 result = JsonConvert.DeserializeObject<TRec1_3>(data);
		switch (result.R) {
			case 21:
			LobbyStart.Get.RemoveOnlinePlayer(result.Index);
			break;
            
            case 22:
			if (result.IsDisconnect)
				UIHint.Get.ShowHint("Lost connection.", Color.red);

			UIMain.Get.ExitRoom();
			break;
		}
	}

	private void netmsg_1_5(string data) {
		TRec1_5 result = JsonConvert.DeserializeObject<TRec1_5>(data);
		switch (result.R) {
		case 21:
			LobbyStart.Get.AddOnlinePlayer(result.PIndex, ref result.Team, ref result.ScenePlayer);
			break;
        }
    }

	private void netmsg_2_1(string data) {
		if (LobbyStart.Visible) {
			TRec2_1 result = JsonConvert.DeserializeObject<TRec2_1>(data);
			LobbyStart.Get.Rec_PlayerMove(ref result);
		}
	}
}
