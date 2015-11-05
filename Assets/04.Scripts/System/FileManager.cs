//#define Debug
#define Release

using System.Collections;
using System.Collections.Generic;
using System.IO;
using GamePlayStruct;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public delegate void DownloadFinsh();
public delegate void DownloadFileText(string ver, string text, bool saveVersion);
public delegate void DownloadFileWWW(string ver, string fileName, WWW www);

public struct TDownloadData
{
	public string fileName;
	public string version;

	public TDownloadData(string FileName, string Version)
	{
		fileName = FileName;
		version = Version;
	}
}

public enum VersionMode
{
	Debug = 1,
	Release = 2
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

public class FileManager : KnightSingleton<FileManager> {
	#if Release
	public const string URL = "http://g2.nicemarket.com.tw/";
	public const VersionMode NowMode = VersionMode.Release;
	#else
	public const string URL = "http://localhost:3600/";
	public const VersionMode NowMode = VersionMode.Debug;						
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

	private static string[] downloadFiles =
	{
	    "greatplayer", "tactical", "baseattr", "ballposition", "skill", "item", "stage", "stagechapter",
        "createroleitem", "aiskilllv", "preloadeffect", "tutorial", "stagetutorial"
	};

	private static DownloadFileText[] downloadCallBack = new DownloadFileText[downloadFiles.Length];
	private static List<TDownloadData> dataList = new List<TDownloadData>();
	private static List<TDownloadData> downloadList = new List<TDownloadData>();
	private static Dictionary<string, DownloadFileText> CallBackFun = new Dictionary<string, DownloadFileText> ();
	private static Dictionary<string, DownloadFileWWW> CallBackWWWFun = new Dictionary<string, DownloadFileWWW> ();
	private static Dictionary<string, int> FailuresData = new Dictionary<string, int> ();
	private static TDownloadTimeRecord NowDownloadFileName = new TDownloadTimeRecord (0);
	private static DownloadFinsh FinishCallBack = null;

	public static int DownlandCount = 0;
	public static int AlreadyDownlandCount = 0;

	public void LoadFileServer(List<TDownloadData> DataList, DownloadFinsh callback = null){
		if (downloadList.Count > 0) {
			for(int i = 0 ; i < downloadList.Count; i++)
				Debug.LogError(downloadList[i].fileName);
		}

		DownlandCount = DataList.Count;
		AlreadyDownlandCount = 0;
		FinishCallBack = callback;
		downloadList.Clear ();
		
		for(int i = 0; i < DataList.Count; i++)
			downloadList.Add (DataList[i]);
	}

	public void LoadFileResource(DownloadFinsh callback = null){
		DownlandCount = dataList.Count;
		AlreadyDownlandCount = 0;
		FinishCallBack = callback;

		for (int i = 0; i < dataList.Count; i++) {
			TextAsset tx = Resources.Load (ClientFilePath + dataList[i].fileName) as TextAsset;
			if (tx) {
				if(CallBackFun.ContainsKey(dataList[i].fileName)){
					CallBackFun[dataList[i].fileName](dataList[i].version, tx.text, false);
					AlreadyDownlandCount++;
					if (UILoading.Visible)
						UILoading.Get.UpdateProgress();
				}else
					Debug.LogError("No handle function : " + dataList[i].fileName);
			}	
		}

		DownloadFinish ();
	}

	public void LoadFileClient(List<TDownloadData> DataList, DownloadFinsh callback = null){
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
	
	public void LoadFileAssetbundle(List<TDownloadData> DataList, DownloadFinsh callback = null)
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

	void Awake () {
		downloadCallBack[0] = parseGreatPlayerData;
		downloadCallBack[1] = parseTacticalData;
		downloadCallBack[2] = parseBaseAttr;
		downloadCallBack[3] = parseBasketShootPositionData;
		downloadCallBack[4] = parseSkillData;
		downloadCallBack[5] = parseItemData;
		downloadCallBack[6] = parseStageData;
		downloadCallBack[7] = parseStageChapterData;
		downloadCallBack[8] = parseCreateRoleData;
		downloadCallBack[9] = parseAISkillData;
		downloadCallBack[10] = parsePreloadEffect;
		downloadCallBack[11] = parseTutorialData;
		downloadCallBack[12] = ParseStageTutorialData;

		for (int i = 0; i < downloadFiles.Length; i ++) {
			CallBackFun.Add (downloadFiles[i], downloadCallBack[i]);
			dataList.Add (new TDownloadData (downloadFiles[i], "0"));
		}
	}
    
    private void DoStarDownload(){
		if (downloadList.Count > 0) {
			int Count = downloadList.Count;
			for(int i = 0; i < Count; i++){
				string[] strChars = downloadList[0].fileName.Split(new char[] {'.'});
				if(strChars.Length > 1){
					if(strChars[strChars.Length - 1] == "json"){
						if(CallBackFun.ContainsKey(strChars[0])){
							DoDownload(downloadList[0].version, downloadList[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + downloadList[0].fileName);
							downloadList.RemoveAt(0);
						}
					}else if(strChars[strChars.Length - 1] == "assetbundle"){
						if(CallBackWWWFun.ContainsKey(strChars[0])){
							DoDownload(downloadList[0].version, downloadList[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackWWWFun Function:" + downloadList[0].fileName);
							downloadList.RemoveAt(0);
						}
					}else{
						if(CallBackFun.ContainsKey(strChars[0])){
							DoDownload(downloadList[0].version, downloadList[0].fileName);
							break;
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + downloadList[0].fileName);
							downloadList.RemoveAt(0);
						}
					}
				}else{
					if(CallBackFun.ContainsKey(strChars[0])){
						DoDownload(downloadList[0].version, downloadList[0].fileName);
					}else{
						Debug.LogError("Download file no CallBackFun Function:" + downloadList[0].fileName);
						downloadList.RemoveAt(0);
					}
				}
			}		
		}
	}

	void Update () {
		if (downloadList.Count > 0) {
			if(NowDownloadFileName.FileName == ""){
				DoStarDownload();
			}else{
				if (Time.time - NowDownloadFileName.StarTime >= FileDownloadLimitTime){
					StopCoroutine("WaitForDownload");
					TDownloadData retry = downloadList[0];
					downloadList.RemoveAt (0);

					if(FailuresData.ContainsKey(retry.fileName))
						FailuresData[retry.fileName] = FailuresData[retry.fileName] + 1;
					else
						FailuresData.Add(retry.fileName, 1);

					if(FailuresData[retry.fileName] >= 3)
						Debug.LogWarning("Please check your Internet connection, [" + retry.fileName + "] downland error");
					else
						downloadList.Add(retry);

					if(downloadList.Count > 0){
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
			Debug.LogError("Server Path error : " + downloadList[0].fileName);
			downloadList.RemoveAt (0);
			if(downloadList.Count > 0)
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
		TextAsset tex = assetbunle.assetBundle.LoadAsset(fileName) as TextAsset;
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
		if (downloadList != null && downloadList.Count > 0) {
			if (string.IsNullOrEmpty (Data.www.error)) {
				AlreadyDownlandCount++;
				UILoading.Get.UpdateProgress();
				string FileName = downloadList[0].fileName;
				strChars = FileName.Split(new char[] {'.'});
				downloadList.RemoveAt (0);

				if(strChars.Length > 1){
					switch(strChars[strChars.Length - 1]){
					case "json":
						if(CallBackFun.ContainsKey(strChars[0])){
							CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + FileName);
							downloadList.RemoveAt(0);
						}
						break;
					case "assetbundle":
						if(CallBackWWWFun.ContainsKey(strChars[0])){
							CallBackWWWFun[strChars[0]](Data.Version, strChars[0], Data.www);
						}else{
							Debug.LogError("Download file no CallBackWWWFun Function:" + FileName);
							downloadList.RemoveAt(0);
						}
						break;
					default:
						if(CallBackFun.ContainsKey(strChars[0])){
							CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
						}else{
							Debug.LogError("Download file no CallBackFun Function:" + FileName);
							downloadList.RemoveAt(0);
						}
						break;
					}	
				}else{
					if(CallBackFun.ContainsKey(strChars[0])){
						CallBackFun[strChars[0]](Data.Version, Data.www.text, true);
					}else{
						Debug.LogError("Download file no CallBackFun Function:" + downloadList[0].fileName);
						downloadList.RemoveAt(0);
					}
				}
			}else{
				TDownloadData retry = downloadList[0];
				downloadList.RemoveAt (0);
				
				if(FailuresData.ContainsKey(retry.fileName))
					FailuresData[retry.fileName] = FailuresData[retry.fileName] + 1;
				else
					FailuresData.Add(retry.fileName, 1);
				
				if(FailuresData[retry.fileName] >= 3)
					Debug.LogWarning("Please check your Internet connection, [" + retry.fileName + "] downland error");
				else
					downloadList.Add(retry);
			}

			if (downloadList.Count > 0) 
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

	private void parseGreatPlayerData (string version, string text, bool isSaveVersion){
		try {
			GameData.DPlayers.Clear();

			TGreatPlayer[] data = (TGreatPlayer[])JsonConvert.DeserializeObject (text, typeof(TGreatPlayer[]));
			if (data != null) {
				for (int i = 0; i < data.Length; i++) 
					if (data[i].ID > 0 && !GameData.DPlayers.ContainsKey(data[i].ID))
						GameData.DPlayers.Add(data[i].ID, data[i]);
			}

			if(isSaveVersion)
				SaveDataVersionAndJson(text, "greatplayer", version);
			
			Debug.Log ("[greatplayer parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[greatplayer parsed error] " + ex.Message);
		}
	}

	private void parseTacticalData(string version, string jsonText, bool isSaveVersion)
    {
        TacticalTable.Ins.Load(jsonText);

        if(isSaveVersion)
            SaveDataVersionAndJson(jsonText, "tactical", version);
    }

    private void parseCreateRoleData(string version, string jsonText, bool isSaveVersion)
    {
        CreateRoleTable.Ins.Load(jsonText);

        if (isSaveVersion)
            SaveDataVersionAndJson(jsonText, "createrole", version);
    }

	private void parseAISkillData(string version, string jsonText, bool isSaveVersion)
    {
        AI.AISkillLvMgr.Ins.Load(jsonText);

        if (isSaveVersion)
            SaveDataVersionAndJson(jsonText, "aiskill", version);
    }

	private void parsePreloadEffect(string version, string jsonText, bool isSaveVersion)
	{
		GameData.PreloadEffect = (TPreloadEffect[])JsonConvert.DeserializeObject (jsonText, typeof(TPreloadEffect[]));
		
		if (isSaveVersion)
			SaveDataVersionAndJson(jsonText, "preloadEeffect", version);
	}

	private void parseBaseAttr (string version, string text, bool isSaveVersion){
		try {
			GameData.BaseAttr = (TPlayerAttribute[])JsonConvert.DeserializeObject (text, typeof(TPlayerAttribute[]));
			
			if(isSaveVersion)
				SaveDataVersionAndJson(text, "BaseAttr", version);
			
			Debug.Log ("[BaseAttr parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[BaseAttr parsed error] " + ex.Message);
		}
	}

	private void parseBasketShootPositionData (string version, string text, bool isSaveVersion){
		try {
			GameData.BasketShootPosition = (TBasketShootPositionData[])JsonConvert.DeserializeObject (text, typeof(TBasketShootPositionData[]));
			
			if(isSaveVersion)
				SaveDataVersionAndJson(text, "ballposition", version);
			
			Debug.Log ("[ballposition parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[ballposition parsed error] " + ex.Message);
		}
	}

	private void parseSkillData (string version, string text, bool isSaveVersion){
		try {
			GameData.DSkillData.Clear();

			TSkillData[] data = (TSkillData[])JsonConvert.DeserializeObject (text, typeof(TSkillData[]));
			for (int i = 0; i < data.Length; i++) {
				if(!GameData.DSkillData.ContainsKey(data[i].ID))
					GameData.DSkillData.Add(data[i].ID, data[i]);
				else
					Debug.LogError("GameData.DSkillData is ContainsKey:"+ data[i].ID);
			}

			if(isSaveVersion)
				SaveDataVersionAndJson(text, "skill", version);
			
			Debug.Log ("[skill parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[skill parsed error] " + ex.Message);
		}
	}

	private void parseItemData (string version, string text, bool isSaveVersion){
		try {
			GameData.DItemData.Clear();

			TItemData[] data = (TItemData[])JsonConvert.DeserializeObject (text, typeof(TItemData[]));
			for (int i = 0; i < data.Length; i++) {
				if(!GameData.DItemData.ContainsKey(data[i].ID))
					GameData.DItemData.Add(data[i].ID, data[i]);
				else 
					Debug.LogError("GameData.DItemData is ContainsKey:"+ data[i].ID);
			}
			
			if(isSaveVersion)
				SaveDataVersionAndJson(text, "item", version);
			
			Debug.Log ("[item parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[item parsed error] " + ex.Message);
		}
	}

	private void parseStageData(string version, string text, bool isSaveVersion)
    {
        StageTable.Ins.Load(text);

		if(isSaveVersion)
            SaveDataVersionAndJson(text, "stage", version);
    }

    private void parseStageChapterData(string version, string text, bool isSaveVersion)
    {
        ChapterTable.Ins.Load(text);

        if(isSaveVersion)
            SaveDataVersionAndJson(text, "chapter", version);
    }

    private void parseTutorialData(string version, string text, bool isSaveVersion) {
		var i = 0;
		try {
			GameData.DTutorial.Clear();
			GameData.DTutorialUI.Clear();
			GameData.DTutorialStageStart.Clear();
			GameData.DTutorialStageEnd.Clear();

			TTutorial[] data = (TTutorial[])JsonConvert.DeserializeObject (text, typeof(TTutorial[]));
			for (i = 0; i < data.Length; i++) {
				int id = data[i].ID * 100 + data[i].Line;
				if (!GameData.DTutorial.ContainsKey(id)) {
					GameData.DTutorial.Add(id, data[i]);

					if (!string.IsNullOrEmpty(data[i].UIName)) {
						if (!GameData.DTutorialUI.ContainsKey(data[i].UIName)) {
							GameData.DTutorialUI.Add(data[i].UIName, data[i].ID);
						} else
							Debug.LogError("Tutorial UI name repeat i : " + i.ToString());
					}

					switch (data[i].Kind) {
					case 1:
						if (data[i].Value > 0) {
							if (!GameData.DTutorialStageStart.ContainsKey(data[i].Value)) {
								GameData.DTutorialStageStart.Add(data[i].Value, data[i].ID);
							} else
								Debug.LogError("Tutorial stage start repeat i : " + i.ToString());
						}

						break;
					case 2:
						if (data[i].Value > 0) {
							if (!GameData.DTutorialStageEnd.ContainsKey(data[i].Value)) {
								GameData.DTutorialStageEnd.Add(data[i].Value, data[i].ID);
							} else
								Debug.LogError("Tutorial stage end i : " + i.ToString());
						}
						
						break;
					}

				} else 
					Debug.LogError("Tutorial key error i : " + i.ToString());
			}
			
			if (isSaveVersion)
				SaveDataVersionAndJson(text, "tutorial", version);
			
			Debug.Log ("[tutorial parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("Tutorial parsed error : " + ex.Message);
		}
	}

	public void LoadStageTutorial(ref TStageToturial[] data) {

	}

	public void ParseStageTutorialData(string version, string text, bool isSaveVersion) {
		try {
			TStageToturial[] data = (TStageToturial[])JsonConvert.DeserializeObject (text, typeof(TStageToturial[]));
			GameData.DStageTutorial.Clear();
			GameData.StageTutorial.Clear();
			
			for (int i = 0; i < data.Length; i++) {
				int id = data[i].ID;
				if (!GameData.DStageTutorial.ContainsKey(id)) {
					GameData.DStageTutorial.Add(id, data[i]);
					GameData.StageTutorial.Add(data[i]);
				} else 
					Debug.LogError("Stage tutorial key error i : " + i.ToString());
			}

			if (isSaveVersion)
				SaveDataVersionAndJson(text, "stagetutorial", version);
			
			Debug.Log ("[Stage tutorial parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("Stage tutorial parsed error : " + ex.Message);
		}
	}
}
