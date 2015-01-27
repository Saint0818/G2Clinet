//#define Debug
#define Release
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public delegate void DownloadFinsh();
public delegate void DownloadFileText(string Ver, string Text, bool SaveVersion);
public delegate void DownloadFileWWW(string Ver, string FileName, WWW www);

public struct TUpdateData
{
	public string fileName;
	public string version;
}

public enum VersionMode
{
	debug = 1,
	release = 2
}

public struct TDownloadTimeRecord{
	public string FileName;
	public float StarTime;

	public TDownloadTimeRecord(int Value){
		FileName = "";
		StarTime = 0;
	}
}

public struct TStartCoroutine{
	public WWW www;
	public string Version;
}

public class FileManager : MonoBehaviour {
	public static string[] DownloadFiles = {"greatplayers", "stages", "exps", "upgrade", "items", "skills", "traintime", "stores", "tutorial", "logobingo", 
											"skilleffect", "Updateinfo", "mission", "playerstore", "vip", "stagechapter", "avatars", "talk"};
	#if Debug
	public const string URL = "http://172.19.0.17:3100/";
	public const VersionMode NowMode = VersionMode.debug;
	#endif
	
	#if Release
	public const string URL = "http://baskclub.nicemarket.com.tw/";
	public const VersionMode NowMode = VersionMode.release;								
	#endif
	private const int FileDownloadLimitTime = 30;
	private const string ServerFilePath = URL + "gamedata/";
	private const string ClientFilePath = "GameData/";
	#if UNITY_ANDROID
	private const string ServerFilePathAssetBundle =  URL + "assetbundle/android/";
	#endif
	#if UNITY_IPHONE
	private const string ServerFilePathAssetBundle =  URL + "assetbundle/ios/";
	#endif

	private static FileManager instance = null;
	private static List<TUpdateData> Download_list = new List<TUpdateData>();
	private static Dictionary<string, DownloadFileText> CallBackFun = new Dictionary<string, DownloadFileText> ();
	private static Dictionary<string, DownloadFileWWW> CallBackWWWFun = new Dictionary<string, DownloadFileWWW> ();
	private static Dictionary<string, int> FailuresData = new Dictionary<string, int> ();
	private static TDownloadTimeRecord NowDownloadFileName = new TDownloadTimeRecord (0);
	private static DownloadFinsh FinishCallBack = null;
	public static int DownlandCount = 0;
	public static int AlreadyDownlandCount = 0;

	public void LoadFileServer(List<TUpdateData> DataList, DownloadFinsh callback = null){
		if (Download_list.Count > 0) {
			for(int i = 0 ; i < Download_list.Count; i++)
				Debug.LogError(Download_list[i].fileName);
		}
		DownlandCount = DataList.Count;
		AlreadyDownlandCount = 0;
		FinishCallBack = callback;
		Download_list.Clear ();
		
		for(int i = 0; i < DataList.Count; i++)
			Download_list.Add (DataList[i]);
	}

	public void LoadFileResource(List<TUpdateData> DataList, DownloadFinsh callback = null){
		DownlandCount = DataList.Count;
		AlreadyDownlandCount = 0;
		FinishCallBack = callback;

		for (int i = 0; i < DataList.Count; i++) {
			TextAsset tx = Resources.Load (ClientFilePath + DataList[i].fileName) as TextAsset;
			if (tx) {
				if(CallBackFun.ContainsKey(DataList[i].fileName)){
					CallBackFun[DataList[i].fileName](DataList[i].version, tx.text, false);
					AlreadyDownlandCount++;
					UILoading.Get.UpdateProgress();
				}
			}	
		}

		DownloadFinish ();
	}

	public void LoadFileClient(List<TUpdateData> DataList, DownloadFinsh callback = null){
		DownlandCount = DataList.Count;
		AlreadyDownlandCount = 0;
		FinishCallBack = callback;
		string text;
		
		for (int i = 0; i < DataList.Count; i++) {
			if(File.Exists(Application.persistentDataPath + "/" + DataList[i].fileName))
			{
				using (StreamReader sr = File.OpenText(Application.persistentDataPath + "/" + DataList[i].fileName)) {
					text = sr.ReadToEnd ();
				}

				string[] strChars = DataList[i].fileName.Split(new char[] {'.'});
				if(strChars.Length > 0 && CallBackFun.ContainsKey(strChars[0])){
					CallBackFun[strChars[0]](DataList[i].version, text, false);
					AlreadyDownlandCount++;
					UILoading.Get.UpdateProgress();
				}
			}
		}
		
		DownloadFinish ();
	}


//	public static string StringRead(string OpenFileName)
//	{
//		string InData = "";
//		FileStream myFile = File.Open(OpenFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//		StreamReader myReader = new StreamReader(myFile);
//		InData = myReader.ReadToEnd();
//		myReader.Close();
//		myFile.Close();
//		return InData;
//	}
	
	public void LoadFileAssetbundle(List<TUpdateData> DataList, DownloadFinsh callback = null)
	{
//		DownlandCount = DataList.Count;
//		AlreadyDownlandCount = 0;
//		FinishCallBack = callback;
//		byte[] bs;
//		
//		for (int i = 0; i < DataList.Count; i++) {
//			bs = File.ReadAllBytes(Application.persistentDataPath + "/" + DataList[i].fileName);
//
//			TextAsset tx = AssetBundle.CreateFromMemory(bs) as TextAsset;
//			if (tx) {
//				if(CallBackFun.ContainsKey(DataList[i].fileName)){
//					CallBackFun[DataList[i].fileName](DataList[i].version, tx.text);
//					AlreadyDownlandCount++;
//					UILoading.Get.UpdateProgress();
//				}
//			}	
//		}
//		
//		DownloadFinish ();
	}

	public static FileManager Get {
		get {
			if (instance == null) {
				GameObject go = new GameObject ("FileManager");
				instance = go.AddComponent<FileManager> ();
			}
			return instance;
		}
	}

	void Awake () {

		//TODO:	CallBackFun.Add ("greatplayers", parseGreatPlayer);
	}

	private void DoStarDownload(){
		if (Download_list.Count > 0) {
			int Count = Download_list.Count;
			for(int i = 0; i < Count; i++){
				string[] strChars = Download_list[0].fileName.Split(new char[] {'.'});
				if(strChars.Length > 1){
					if(strChars[strChars.Length - 1] == "json"){
						if(CallBackFun.ContainsKey(strChars[0])){
							DoDownload(Download_list[0].version, Download_list[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + Download_list[0].fileName);
							Download_list.RemoveAt(0);
						}
					}else if(strChars[strChars.Length - 1] == "assetbundle"){
						if(CallBackWWWFun.ContainsKey(strChars[0])){
							DoDownload(Download_list[0].version, Download_list[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackWWWFun Function:" + Download_list[0].fileName);
							Download_list.RemoveAt(0);
						}
					}else{
						if(CallBackFun.ContainsKey(strChars[0])){
							DoDownload(Download_list[0].version, Download_list[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + Download_list[0].fileName);
							Download_list.RemoveAt(0);
						}
					}
				}else{
					if(CallBackFun.ContainsKey(strChars[0])){
						DoDownload(Download_list[0].version, Download_list[0].fileName);
					}else{
						Debug.LogError("Download file no CallBackFun Function:" + Download_list[0].fileName);
						Download_list.RemoveAt(0);
					}
				}
			}		
		}
	}

	void Update () {
		if (Download_list.Count > 0) {
			if(NowDownloadFileName.FileName == ""){
				DoStarDownload();
			}else{
				if (Time.time - NowDownloadFileName.StarTime >= FileDownloadLimitTime){
					StopCoroutine("WaitForDownload");
					TUpdateData retry = Download_list[0];
					Download_list.RemoveAt (0);

					if(FailuresData.ContainsKey(retry.fileName))
						FailuresData[retry.fileName] = FailuresData[retry.fileName] + 1;
					else
						FailuresData.Add(retry.fileName, 1);

					if(FailuresData[retry.fileName] >= 3)
						Debug.LogWarning("Please check your Internet connection, [" + retry.fileName + "] downland error");
					else
						Download_list.Add(retry);

					if(Download_list.Count > 0){
						DoStarDownload();
					}else
						DownloadFinish();
				}
			}
		}
	}

	private void DoDownload(string Version, string FileName){
		string[] strChars = FileName.Split(new char[] {'.'});
		WWW www = null;

		if (strChars.Length > 1) {
			switch(strChars[strChars.Length - 1]){
			case "json":
				www = new WWW(ServerFilePath + FileName);
				break;
			case "assetbundle":
				www = new WWW(ServerFilePathAssetBundle + FileName);
				break;
			default:
				www = new WWW(ServerFilePath + FileName);
				break;
			}	
		}else
			www = new WWW(ServerFilePath + FileName);

		if (www != null) {
			NowDownloadFileName.FileName = FileName;
			NowDownloadFileName.StarTime = Time.time;
			TStartCoroutine Data = new TStartCoroutine ();
			Data.Version = Version;
			Data.www = www;
			StartCoroutine("WaitForDownload" , Data);	
		}else{
			Debug.LogError("Server Path error : " + Download_list[0].fileName);
			Download_list.RemoveAt (0);
			if(Download_list.Count > 0)
				DoStarDownload();
			else
				DownloadFinish();
		}
	}

	private void SaveJson(string text, string fileName)
	{
		string Path = Application.persistentDataPath + "/" + fileName + ".json";
		using (FileStream myFile = File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
			using (StreamWriter myWriter = new StreamWriter(myFile)) {
				myWriter.Write(text);
				myWriter.Close();
			}
			
			myFile.Close();
		}
	}

	private void SaveAssetbundle(WWW assetbunle, string fileName)
	{
		TextAsset tex = assetbunle.assetBundle.Load(fileName) as TextAsset;
		string Path = Application.persistentDataPath + "/" + fileName + ".assetbundle";
		FileStream myFile = File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamWriter myWriter = new StreamWriter(myFile);
		myWriter.Write(tex);
		myWriter.Close();
		myFile.Close();
	}
	
	private IEnumerator WaitForDownload(TStartCoroutine Data){
		yield return Data.www;
		string[] strChars;
		if (Download_list != null && Download_list.Count > 0) {
			if (string.IsNullOrEmpty (Data.www.error)) {
				AlreadyDownlandCount++;
				UILoading.Get.UpdateProgress();
				string FileName = Download_list[0].fileName;
				strChars = FileName.Split(new char[] {'.'});
				Download_list.RemoveAt (0);

				if(strChars.Length > 1){
					switch(strChars[strChars.Length - 1]){
					case "json":
						if(CallBackFun.ContainsKey(strChars[0])){
							CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + FileName);
							Download_list.RemoveAt(0);
						}
						break;
					case "assetbundle":
						if(CallBackWWWFun.ContainsKey(strChars[0])){
							CallBackWWWFun[strChars[0]](Data.Version, strChars[0], Data.www);
						}else{
							Debug.LogError("Download file no CallBackWWWFun Function:" + FileName);
							Download_list.RemoveAt(0);
						}
						break;
					default:
						if(CallBackFun.ContainsKey(strChars[0])){
							CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + FileName);
							Download_list.RemoveAt(0);
						}
						break;
					}	
				}else{
					if(CallBackFun.ContainsKey(strChars[0])){
						CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
					}else{
						Debug.LogError("Download file no CallBackFun Function:" + Download_list[0].fileName);
						Download_list.RemoveAt(0);
					}
				}
			}else{
				TUpdateData retry = Download_list[0];
				Download_list.RemoveAt (0);
				
				if(FailuresData.ContainsKey(retry.fileName))
					FailuresData[retry.fileName] = FailuresData[retry.fileName] + 1;
				else
					FailuresData.Add(retry.fileName, 1);
				
				if(FailuresData[retry.fileName] >= 3)
					Debug.LogWarning("Please check your Internet connection, [" + retry.fileName + "] downland error");
				else
					Download_list.Add(retry);
			}

			if (Download_list.Count > 0) 
				DoStarDownload();
			else
				DownloadFinish();
		}else
			Debug.LogWarning("Download_list.Count = 0 : " + Data.www.url );

		Data.www.Dispose();
		Data.www = null;
	}

	private void DownloadFinish(){
		NowDownloadFileName.FileName = "";
		NowDownloadFileName.StarTime = 0;
		FailuresData.Clear();
		if(DownlandCount != AlreadyDownlandCount)
			Debug.LogWarning("Fail download:" + (DownlandCount - AlreadyDownlandCount).ToString());

		if(FinishCallBack != null){
			DownloadFinsh TempCallBack = FinishCallBack;
			FinishCallBack = null;
			TempCallBack();
		}
	}
	
	//CallBack Function

	private void SaveDataVersionAndJson(string text, string fileName, string version)
	{
		SaveJson(text, fileName);
		PlayerPrefs.SetString(fileName, version);
		PlayerPrefs.Save();
	}
}
