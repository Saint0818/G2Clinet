using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using WebSocketSharp;
using SocketIO;
using zlib;

public delegate void TNetMessageProc ();

public class GSocket
{
	private static GSocket instance = null;
	private SocketIOComponent WebSocket = null;
	private TNetMessageProc[,] NetMsgProcs = new TNetMessageProc[10, 20];

	const int Min_RecMsgLen = 4;
	const int FMaxSendBuf = 512;
	private int FSendBufIndex = 0;
	private byte[] FSendBuf = new byte[FMaxSendBuf];
	private byte[] FLenBuf = new byte[2];

	const int FMaxRecBuf = 2*1024;
	private int FRecBufSize = 0;
	private int FRecBufIndex = 0;
	private byte[] FRecBuf = new byte[FMaxRecBuf];

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

	public void Init ()
	{

	}

	public void Connect ()
	{
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
    
    public void Close ()
    {
		if (WebSocket != null && WebSocket.IsConnected) {
			WebSocket.Close();
		}
	}

	private void OnClose(object sender, CloseEventArgs e)
	{
			
	}

    public void Send (byte Kind1, byte Kind2)
	{
		if (true) {
			try {
			byte[] buf = new byte[FSendBufIndex + 7];
			
			buf [0] = 91;
			buf [1] = 94;
			buf [2] = 37;
			buf [5] = Kind1;
			buf [6] = Kind2;
			FLenBuf = BitConverter.GetBytes (FSendBufIndex);
			Array.Copy (FLenBuf, 0, buf, 3, 2);
			Array.Copy (FSendBuf, 0, buf, 7, FSendBufIndex);

			Dictionary<string, JSONObject> obj = new Dictionary<string, JSONObject>();
			JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
			for (int i = 0; i < buf.Length;i ++)
				arr.Add(buf[i]);

			obj.Add("data", arr);
			WebSocket.Emit("message", new JSONObject(obj));

			//WebSocket.socket.Send(buf);
            FSendBufIndex = 0;
			} catch (Exception e) {
				Debug.Log(e.ToString());
			}
        }
    }

	public void WriteByte (byte Value)
	{
		if (FSendBufIndex + 1 <= FMaxSendBuf) {
			FSendBuf [FSendBufIndex] = Value;
			FSendBufIndex += 1;
        }
	}

	public void WriteUShort (ushort Value)
	{
		if (FSendBufIndex + 2 <= FMaxSendBuf) {
			Array.Copy (BitConverter.GetBytes (Value), 0, FSendBuf, FSendBufIndex, 2);
			FSendBufIndex += 2;
        }
	}

	public void WriteInt (int Value)
	{
		if (FSendBufIndex + 4 <= FMaxSendBuf) {
			Array.Copy (BitConverter.GetBytes (Value), 0, FSendBuf, FSendBufIndex, 4);
			FSendBufIndex += 4;
        }
    }

	public void WriteFloat (float Value)
	{
		int size = 4;
		if (FSendBufIndex + size <= FMaxSendBuf) {
			Array.Copy (BitConverter.GetBytes (Value), 0, FSendBuf, FSendBufIndex, size);
			FSendBufIndex += size;
        }
    }

	public void WriteString (string Value)
	{
		char[] chars = Value.ToCharArray();
		int len = chars.Length * sizeof(char);
            
        if (FSendBufIndex + len + 1 <= FMaxSendBuf) {
			Array.Copy (BitConverter.GetBytes (len), 0, FSendBuf, FSendBufIndex, 1);
			FSendBufIndex++;

			for (int i = 0; i < chars.Length; i ++) {
				int size = sizeof(char);
				Array.Copy (BitConverter.GetBytes (chars[i]), 0, FSendBuf, FSendBufIndex, size);
				FSendBufIndex += size;
			}
        }
    }

	public byte ReadByte()
	{
		int size = 1;
		if((FRecBufIndex + size <= FMaxRecBuf) && (FRecBufIndex < FRecBufSize))
		{
			byte Value = FRecBuf[FRecBufIndex];
			FRecBufIndex += size;
			return Value;
		}
		else
			return 0;
	}
	
	public ushort ReadUShort()
	{
		int size = 2;
		if((FRecBufIndex + size <= FMaxRecBuf) && (FRecBufIndex < FRecBufSize))
		{
			ushort Value = BitConverter.ToUInt16(FRecBuf, FRecBufIndex);
			FRecBufIndex += size;
			return Value;
		}
		else
			return 0;
	}
	
	public int ReadInt()
	{
		int size = 4;
		if((FRecBufIndex + size <= FMaxRecBuf) && (FRecBufIndex < FRecBufSize))
		{
			int Value = BitConverter.ToInt32(FRecBuf, FRecBufIndex);
			FRecBufIndex += size;
			return Value;
		}
		else
			return 0;
	}
	
	public float ReadFloat()
	{
		int size = 4;
		if((FRecBufIndex + size <= FMaxRecBuf) && (FRecBufIndex < FRecBufSize))
		{
			float Value = BitConverter.ToSingle(FRecBuf, FRecBufIndex);
			FRecBufIndex += size;
			return Value;
		}
		else
			return 0;
	}
	
	public string ReadString()
	{
		ushort size = ReadUShort();
        
        if((FRecBufIndex + size <= FMaxRecBuf) && (FRecBufIndex < FRecBufSize))
        {
			string str = Encoding.UTF8.GetString(FRecBuf, FRecBufIndex, size);
            FRecBufIndex += size;
            return str;
        }
        else
            return "";
    }

	public string ReadZlib()
	{
		int size = ReadUShort();
		if(FRecBufIndex + size <= FMaxRecBuf)
		{
			byte[] compressedBytes = new byte[size];
			Array.Copy(FRecBuf, FRecBufIndex, compressedBytes, 0, size);
			FRecBufIndex += size;
			return System.Text.Encoding.UTF8.GetString(Decompress(compressedBytes));
		}
		else
			return "";
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
    
    public bool Connected {
		get {
			if (WebSocket != null)
				return WebSocket.IsConnected; 
			else
				return false;
		}
	}

	private void OnConnected(Packet packet) {
		WriteString(GameData.Team.Identifier);
		WriteString(GameData.Team.sessionID);
		Send (1, 1);
	}

	private byte[] decodeJSONArray(string js) {
		try {
			List<byte> lb = new List<byte>();
			int offset = 2;
			int index = 2;
			while (offset < js.Length-1) {
				offset++;
				if (js[offset] == ',' || js[offset] == ']') {
					if (offset-index > 0) {
						char[] buf = new char[offset-index];
						js.CopyTo(index, buf, 0, offset-index);
						string s = new string(buf);
						byte a = 0;
						byte.TryParse(s, out a);
						lb.Add(a);
	                    index = offset+1;
	                } else
	                    break;
	            }
	        }
	        
	        return lb.ToArray();
		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
    }

	public string OnHttpText(string text) {
		byte[] buf = JsonConvert.DeserializeObject <byte[]>(text);
		return System.Text.Encoding.UTF8.GetString(Decompress(buf));
	}
    
    private void OnMessage(SocketIOEvent e) {
        try {
			byte[] data = decodeJSONArray(e.data.GetField("data").ToString());
			if (data != null && data.Length > 0) {
				
				if(data.Length > 0) {
					if(data.Length + FRecBufSize < FRecBuf.Length) {
						Array.Copy(data, 0, FRecBuf, FRecBufSize, data.Length);
						FRecBufSize += data.Length;
					}
					else
						Debug.Log("ReceiveMsg overflow");
		        }
			}
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
		}
    }

	public void ReceivePotocol() {
		try {
		while(FRecBufSize-FRecBufIndex >= Min_RecMsgLen)
		{
			int len = BitConverter.ToUInt16(FRecBuf, FRecBufIndex);
			if(FRecBufSize-FRecBufIndex < len+4)
				return;
			
			int kind1 = FRecBuf[FRecBufIndex + 2];
			int kind2 = FRecBuf[FRecBufIndex + 3];
			FRecBufIndex += Min_RecMsgLen;
			int Index = FRecBufIndex;
			try
			{
				if(kind1 <= 10 && kind2 <= 20 && NetMsgProcs[kind1, kind2] != null)
					NetMsgProcs[kind1, kind2]();
				else
					Debug.Log(string.Format("error protocol {0}-{1} size: {2} index: {3}", kind1, kind2, FRecBufSize, FRecBufIndex));
			}
			catch(Exception exc)
			{
				Debug.Log(string.Format("protocol {0}-{1} {2}", kind1, kind2, exc.Message));
				FRecBufIndex = Index + len;
			}
			
			FRecBufIndex = Index + len;
			if(FRecBufIndex >= FRecBufSize)
			{
				FRecBufIndex = 0;
				FRecBufSize = 0;
			}
		}
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
}
