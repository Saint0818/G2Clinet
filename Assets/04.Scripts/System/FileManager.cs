#define Debug
//#define Release
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameEnum;
using GamePlayStruct;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public delegate void DownloadFinsh();
public delegate void DownloadFileText(string ver,string text,bool saveVersion);
public delegate void DownloadFileWWW(string ver,string fileName,WWW www);

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

public struct TDownloadTimeRecord
{
    public string FileName;
    public float StarTime;

    public TDownloadTimeRecord(int Value)
    {
        FileName = "";
        StarTime = 0;
    }
}

public struct TStartCoroutine
{
    public WWW www;
    public string Version;
}

public class FileManager : KnightSingleton<FileManager>
{
    #if Release
    public const string URL = "http://g2.nicemarket.com.tw/";
    public const VersionMode NowMode = VersionMode.Release;
    #else
    public const string URL = "http://52.68.61.220:3500/";
//    public const string URL = "http://localhost:3500/";
	public const VersionMode NowMode = VersionMode.Debug;						
	#endif

    private const int FileDownloadLimitTime = 30;
    private const string ServerFilePath = URL + "gamedata/";
    private const string ClientFilePath = "GameData/";

    #if UNITY_IOS
	private const string ServerFilePathAssetBundle =  URL + "assetbundle/ios/";
	
#else
    private const string ServerFilePathAssetBundle = URL + "assetbundle/android/";
    #endif

    private static string[] downloadFiles =
        {
	    "greatplayer", "tactical", "baseattr", "ballposition", "skill", "item", "stage", "stagechapter",
        "createroleitem", "aiskilllv", "preloadeffect", "tutorial", "stagetutorial", "exp", "teamname", "textconst", 
        "skillrecommend", "mission", "pickcost", "shop", "mall", "pvp", "limit"
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
		DontDestroyOnLoad(gameObject);
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
		downloadCallBack[13] = parseExpData;
		downloadCallBack[14] = parseTeamname;
		downloadCallBack[15] = ParseTextConst;
        downloadCallBack[16] = ParseSkillRecommend;
		downloadCallBack[17] = ParseMission;
		downloadCallBack[18] = ParsePickCost;
		downloadCallBack[19] = ParseShop;
		downloadCallBack[20] = ParseMall;
        downloadCallBack[21] = ParsePVP;
        downloadCallBack[22] = parseLimitData;

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
		try
        {
			GameData.DItemData.Clear();

			TItemData[] data = JsonConvertWrapper.DeserializeObject<TItemData[]>(text);
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

	private void parseExpData(string version, string text, bool isSaveVersion){
		try {
			GameData.DExpData.Clear();
			
			TExpData[] data = (TExpData[])JsonConvert.DeserializeObject (text, typeof(TExpData[]));
			for (int i = 0; i < data.Length; i++) {
                if(!GameData.DExpData.ContainsKey(data[i].Lv)) {
					GameData.DExpData.Add(data[i].Lv, data[i]);
                    if (data[i].OpenIndex > 0 && !GameData.DOpenUILv.ContainsKey((EOpenUI)data[i].OpenIndex))
                        GameData.DOpenUILv.Add((EOpenUI)data[i].OpenIndex, data[i].Lv);
                } else
					Debug.LogError("GameData.DItemData is ContainsKey:"+ data[i].Lv);
			}
			
			if(isSaveVersion)
				SaveDataVersionAndJson(text, "item", version);
			
			Debug.Log ("[item parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[item parsed error] " + ex.Message);
		}
	}

	private void parseTeamname(string Version, string text, bool SaveVersion)
	{
		try {
			TextConst.TeamNameAy = (TTeamName[])JsonConvert.DeserializeObject (text, typeof(TTeamName[]));
			if(SaveVersion)
				SaveDataVersionAndJson(text, "teamname", Version);
			
			Debug.Log ("[Teamname parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[Teamname parsed error] " + ex.Message);
		}
	}

    private void parseLimitData(string version, string text, bool isSaveVersion)
    {
        LimitTable.Ins.Load(text);

        if(isSaveVersion)
            SaveDataVersionAndJson(text, "limit", version);
    }

    private void parseStageData(string version, string text, bool isSaveVersion)
    {
        StageTable.Ins.Load(text);

		if(isSaveVersion)
            SaveDataVersionAndJson(text, "stage", version);
    }

    private void parseStageChapterData(string version, string text, bool isSaveVersion)
    {
        StageChapterTable.Ins.Load(text);

        if(isSaveVersion)
            SaveDataVersionAndJson(text, "chapter", version);
    }

    private void parseTutorialData(string version, string text, bool isSaveVersion) {
		try {
			GameData.DTutorial.Clear();
			GameData.DTutorialUI.Clear();
			GameData.DTutorialStageStart.Clear();
			GameData.DTutorialStageEnd.Clear();

			TTutorial[] data = JsonConvert.DeserializeObject<TTutorial[]>(text);
			for (int i = 0; i < data.Length; i++) {
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
                        if (data[i].Value > 0 && data[i].Line == 1) {
							if (!GameData.DTutorialStageStart.ContainsKey(data[i].Value)) {
								GameData.DTutorialStageStart.Add(data[i].Value, data[i].ID);
							} else
								Debug.LogError("Tutorial stage start repeat i : " + i.ToString());
						}

						break;
					case 2:
                        if (data[i].Value > 0 && data[i].Line == 1) {
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

	public void ParseStageTutorialData(string version, string text, bool isSaveVersion) {
		try {
			TStageToturial[] data = JsonConvert.DeserializeObject<TStageToturial[]>(text);
			GameData.DStageTutorial.Clear();
			Array.Resize(ref GameData.StageTutorial, 0);
			Array.Resize(ref GameData.StageTutorial, data.Length);

			for (int i = 0; i < data.Length; i++) {
				int id = data[i].ID;
				if (!GameData.DStageTutorial.ContainsKey(id)) {
					GameData.DStageTutorial.Add(id, data[i]);
					GameData.StageTutorial[i] = data[i];
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

	public void ParseTextConst(string version, string text, bool isSaveVersion) {
		try {
			TTextConst[] data = JsonConvert.DeserializeObject<TTextConst[]>(text);
			TextConst.LoadText(ref data);
			
			if (isSaveVersion)
				SaveDataVersionAndJson(text, "textconst", version);
			
			Debug.Log ("[Text const parsed finished.]");
		} catch (System.Exception ex) {
			Debug.LogError ("Text const parsed error : " + ex.Message);
		}
	}

    public void ParseSkillRecommend(string version, string text, bool isSaveVersion) {
        try {
            GameData.SkillRecommends = JsonConvert.DeserializeObject<TSkillRecommend[]>(text);

            if (isSaveVersion)
                SaveDataVersionAndJson(text, "textconst", version);

            Debug.Log ("[SkillRecommend parsed finished.]");
        } catch (System.Exception ex) {
            Debug.LogError ("SkillRecommend parsed error : " + ex.Message);
        }
    }

    public void ParseMission(string version, string text, bool isSaveVersion) {
        try {
            GameData.MissionData = JsonConvertWrapper.DeserializeObject<TMission[]>(text);
			for (int i = 0; i < GameData.MissionData.Length; i++)
                if (!GameData.DMissionData.ContainsKey(GameData.MissionData[i].ID)) {
                    if (GameData.MissionData[i].Value != null && GameData.MissionData[i].Value.Length > 0 &&
                        (GameData.MissionData[i].AwardID == null || GameData.MissionData[i].Value.Length == GameData.MissionData[i].AwardID.Length) &&
                        (GameData.MissionData[i].AwardNum == null || GameData.MissionData[i].Value.Length == GameData.MissionData[i].AwardNum.Length) &&
                        (GameData.MissionData[i].Diamond == null || GameData.MissionData[i].Value.Length == GameData.MissionData[i].Diamond.Length) &&
                        (GameData.MissionData[i].Money == null || GameData.MissionData[i].Value.Length == GameData.MissionData[i].Money.Length) &&
                        (GameData.MissionData[i].Exp == null || GameData.MissionData[i].Value.Length == GameData.MissionData[i].Exp.Length))
					    GameData.DMissionData.Add(GameData.MissionData[i].ID, GameData.MissionData[i]);
                    else
                        Debug.Log("Mission value length not the same " + GameData.MissionData[i].ID.ToString());
                } else
					Debug.Log("Mission id repeat " + GameData.MissionData[i].ID.ToString());
			
            if (isSaveVersion)
                SaveDataVersionAndJson(text, "textconst", version);

            Debug.Log ("[Achievement parsed finished.]");
        } catch (System.Exception ex) {
            Debug.LogError ("Achievement parsed error : " + ex.Message);
        }
    }

	public void ParsePickCost(string version, string text, bool isSaveVersion) {
		try {
			TPickCost[] data = JsonConvertWrapper.DeserializeObject<TPickCost[]>(text);
			GameData.DPickCost = new TPickCost[data.Length];
			for (int i = 0; i < data.Length; i++) {
				GameData.DPickCost[i] = data[i];
				if (data[i].Order > data.Length) {
					Debug.Log("PickCost order bigger " + data[i].Order.ToString());
				}
			}

			if (isSaveVersion)
				SaveDataVersionAndJson(text, "textconst", version);

			Debug.Log ("[PickCost parsed finished.]");
		} catch (System.Exception ex) {
			Debug.LogError ("PickCost parsed error : " + ex.Message);
		}
	}

	public void ParseShop(string version, string text, bool isSaveVersion) {
		try {
			GameData.DShops = JsonConvertWrapper.DeserializeObject<TShop[]>(text);

			if (isSaveVersion)
				SaveDataVersionAndJson(text, "textconst", version);

			Debug.Log ("[Shop parsed finished.]");
		} catch (System.Exception ex) {
			Debug.LogError ("Shop parsed error : " + ex.Message);
		}
	}

	public void ParseMall(string version, string text, bool isSaveVersion) {
		try {
			GameData.DMalls = JsonConvertWrapper.DeserializeObject<TMall[]>(text);

			if (isSaveVersion)
				SaveDataVersionAndJson(text, "textconst", version);

			Debug.Log ("[Mall parsed finished.]");
		} catch (System.Exception ex) {
			Debug.LogError ("Mall parsed error : " + ex.Message);
		}
	}

	public void ParsePVP(string version, string text, bool isSaveVersion) 
	{
        try
        {
            GameData.DPVPData.Clear();
            TPVPData[] data = JsonConvertWrapper.DeserializeObject<TPVPData[]>(text);

            for (int i = 0; i < data.Length; i++) {
                if(!GameData.DPVPData.ContainsKey(data[i].Lv))
                    GameData.DPVPData.Add(data[i].Lv, data[i]);
                else 
                    Debug.LogError("GameData.DItemData is ContainsKey:"+ data[i].Lv);
            }

            if(isSaveVersion)
                SaveDataVersionAndJson(text, "item", version);

			Debug.Log ("[PVP parsed finished.] ");
        } catch (System.Exception ex) {
			Debug.LogError ("[PVP parsed error] " + ex.Message);
        }
	}
}
