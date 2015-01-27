using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GAKind
{
  LogoinBtn = 0,
  FastLogoin = 1, 
  FBLogoin = 2, 
  NowPlay = 3,
  Rank = 4,
  Store = 5,
  Activity = 6,
  Mission = 7,
  Game = 8,
  Registration = 9,
  LinkFB = 10,
  MusicSwitch = 11,
  SoundSwitch = 12,
  App = 13,
  FBpage = 14,
  LogOut = 15,
  Setting = 16,
  Login_Male,
  Login_Female,
  Room_OpenRoomBtn,
  Room_QuickBtn,
  Room_NormalBtn,
  Room_RichBtn,
  Room_RegalBtn,
  OpenRoom_Normal,
  OpenRoom_Rich,
  OpenRoom_Regal,
  OpenRoom_3S,
  OpenRoom_5S,
  OpenRoom_8S,
  OpenRoom_1Round,
  OpenRoom_3Rounds,
  Mission_Daily,
  Mission_Owned,
  Mission_NotHave,
  Ingame_SetingBtn,
  Ingame_EmotionBtn,
  Ingame_GiftBtn,
  GameSeting_AIOn,
  GameSeting_AIOff,
  GameSeting_SoundOn,
  GameSeting_SoundOff,
  GameSeting_LeaveGame,
  GameOver_Continue,
  GameOver_NotContinue,
  Rank_Friend,
  Rank_Player,
  Rank_GameHistory,
  FriendRank_Lv,
  FriendRank_Win,
  FriendRank_Follow,
  PlayerRank_Lv,
  PlayerRank_Win,
  PlayerRank_Follow,
  Store_Buy,
  Store_Fitting,
  Store_Money,
  Buy_Hair,
  Buy_Head,
  Buy_Facial,
  Buy_Cloth,
  Buy_Acce,
  Buy_Emotion,
  Fitting_Hair,
  Fitting_Head,
  Fitting_Facial,
  Fitting_Cloth,
  Fitting_Acce,
  Fitting_Style1,
  Fitting_Style2,
  Fitting_Save,
  Vip_Info,
}

public class GoogleAnalytics : MonoBehaviour
{
  private bool mIsOpenTestUI = true;
  public string propertyID = "UA-44846076-2";
  public static GoogleAnalytics instance;
  public string bundleID = "com.nicemarket.nbaa";
  public string appName = "nbaA";
  public string appVersion = "1.0";
  private string screenRes;
  private string clientID;
  
  void Awake()
  {
    if(instance)
      DestroyImmediate(gameObject);
    else
    {
      DontDestroyOnLoad(gameObject);
      instance = this;
    }
   
    OneCount = (int)(Screen.width - 10) / BtnW;
  }
  
  void Start()
  {
  
    screenRes = Screen.width + "x" + Screen.height;
    
    #if UNITY_IPHONE
    clientID = iPhoneSettings.uniqueIdentifier;
    #else
    clientID = SystemInfo.deviceUniqueIdentifier;
    #endif
      
  }

  public void LogScreen(string title)
  {
    Debug.Log("Google Analytics - Screen --> " + title);
    
    title = WWW.EscapeURL(title);
    
    var url = "http://www.google-analytics.com/collect?v=1&ul=en-us&t=appview&sr=" + screenRes + 
      "&an=" + WWW.EscapeURL(appName) + 
      "&a=448166238&tid=" + propertyID + 
      "&aid=" + bundleID + 
      "&cid=" + WWW.EscapeURL(clientID) + 
      "&_u=.sB&av=" + appVersion + 
      "&_v=ma1b3&cd=" + title + 
      "&qt=2500&z=185";


    Debug.LogError("LogScreen : " + url);
    StartCoroutine(Process(new WWW(url)));
  }
  
  /*  MOBILE EVENT TRACKING:  https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide */
  public void LogEvent(string titleCat,string titleAction)
  {
    Debug.Log("Google Analytics - Event --> " + titleAction);
    
    titleCat = WWW.EscapeURL(titleCat);
    titleAction = WWW.EscapeURL(titleAction);
    
    var url = "http://www.google-analytics.com/collect?v=1&ul=en-us&t=event&sr=" + screenRes + 
      "&an=" + WWW.EscapeURL(appName) + 
      "&a=448166238&tid=" + propertyID + 
      "&aid=" + bundleID + 
      "&cid=" + WWW.EscapeURL(clientID) + 
      "&_u=.sB&av=" + appVersion + 
      "&_v=ma1b3&ec=" + titleCat + 
      "&ea=" + titleAction + 
      "&qt=2500&z=185";
  
    StartCoroutine(Process(new WWW(url)));
  }

  private IEnumerator Process(WWW www)
  {
    yield return www;
    
    if(www.error == null)
    {
      if(www.responseHeaders.ContainsKey("STATUS"))
      {
        if(www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")
        {
          Debug.Log("GA Success");
        }
        else
        {
          Debug.LogWarning(www.responseHeaders["STATUS"]);  
        }
      }
      else
      {
        Debug.LogWarning("Event failed to send to Google"); 
      }
    }
    else
    {
      Debug.LogWarning(www.error.ToString()); 
    }
    
    www.Dispose();
  }

  private const int Max_Btn = 16;
  private int BtnW = 100;
  private int BtnH = 50;
  private Rect BtnPos;
  private int OneCount = 0;
  private int[] mGAKindAy = new int[Max_Btn];
  private string testContent;

  void OnGUI()
  {
    if(mIsOpenTestUI == false)
      return;

    for(int i = 0; i < Max_Btn; i++)
    {
      testContent = string.Empty;
      if(i < 17)
        mGAKindAy[i] = 1;
        
      BtnPos.x = (i % OneCount) * 100;
      BtnPos.y = (i / OneCount) * 50;
      BtnPos.width = BtnW;
      BtnPos.height = BtnH;

      GAKind temp = (GAKind)i;
      testContent = GetContent(temp);
   
      if(GUI.Button(BtnPos, testContent))
        SendGAEvent(testContent);
    }
  }

  public void SendGA(GAKind kind)
  {
    string title = GetContent(kind);
    SendGAEvent(title);
  }

  public void SendGAEvent(string content)
  {
    LogEvent(content, "ClickBtn");
  }

  public void SendGAScreen(string content)
  {
    LogScreen(content);
  }

  private string GetContent(GAKind kind)
  {
    string result = string.Empty;
    
    switch (kind)
    {
      case GAKind.LogoinBtn:
        result = "Login_LoginBtn";
        break;
      case GAKind.FastLogoin:
        result = "Login_TestBtn";
        break;
      case GAKind.FBLogoin:
        result = "Login_FbBtn";
        break;
      case GAKind.Login_Male:
        result = "Login_Male";
        break;
      case GAKind.Login_Female:
        result = "Login_Female";
        break;
      case GAKind.NowPlay:
        result = "Lobby_QuickBtn";
        break;
      case GAKind.Rank:
        result = "Lobby_RankBtn";
        break;
      case GAKind.Store:
        result = "Lobby_StoreBtn";
        break;
      case GAKind.Activity:
        result = "Lobby_ActionBtn";
        break;
      case GAKind.Mission:
        result = "Lobby_QuestBtn";
        break;
      case GAKind.Game:
        result = "Lobby_RoomBtn";
        break;
      case GAKind.Registration:
        result = "Seting_SignUPBtn";
        break;
      case GAKind.LinkFB:
        result = "Seting_LinkFBBtn";
        break;
      case GAKind.MusicSwitch:
        result = "Seting_MusicBtn";
        break;
      case GAKind.SoundSwitch:
        result = "Seting_SoundBtn";
        break;
      case GAKind.App:
        result = "Seting_ReviewBtn";
        break;
      case GAKind.FBpage:
        result = "Seting_FbPageBtn";
        break;
      case GAKind.LogOut:
        result = "Seting_LogOutBtn";
        break;
      case GAKind.Room_OpenRoomBtn:
        result = "Room_OpenRoomBtn";
        break;
      case GAKind.Room_QuickBtn:
        result = "Room_QuickBtn";
        break;
      case GAKind.Room_NormalBtn:
        result = "Room_NormalBtn";
        break;
      case GAKind.Room_RichBtn:
        result = "Room_RichBtn";
        break;
      case GAKind.Room_RegalBtn:
        result = "Room_RegalBtn";
        break;
      case GAKind.OpenRoom_Normal:
        result = "OpenRoom_Normal";
        break;
      case GAKind.OpenRoom_Rich:
        result = "OpenRoom_Rich";
        break;
      case GAKind.OpenRoom_Regal:
        result = "OpenRoom_Regal";
        break;
      case GAKind.OpenRoom_3S:
        result = "OpenRoom_3S";
        break;
      case GAKind.OpenRoom_5S:
        result = "OpenRoom_5S";
        break;
      case GAKind.OpenRoom_8S:
        result = "OpenRoom_8S";
        break;
      case GAKind.OpenRoom_1Round:
        result = "OpenRoom_1Round";
        break;
      case GAKind.OpenRoom_3Rounds:
        result = "OpenRoom_3Rounds";
        break;
      case GAKind.Mission_Daily:
        result = "Mission_Daily";
        break;
      case GAKind.Mission_Owned:
        result = "Mission_Owned";
        break;
      case GAKind.Mission_NotHave:
        result = "Mission_NotHave";
        break;
      case GAKind.Ingame_SetingBtn:
        result = "Ingame_SetingBtn";
        break;
      case GAKind.Ingame_EmotionBtn:
        result = "Ingame_EmotionBtn";
        break;
      case GAKind.Ingame_GiftBtn:
        result = "Ingame_GiftBtn";
        break;
      case GAKind.GameSeting_AIOn:
        result = "GameSeting_AIOn";
        break;
      case GAKind.GameSeting_AIOff:
        result = "GameSeting_AIOff";
        break;
      case GAKind.GameSeting_SoundOff:
        result = "GameSeting_SoundOff";
        break;
      case GAKind.GameSeting_LeaveGame:
        result = "GameSeting_LeaveGame";
        break;
      case GAKind.GameOver_Continue:
        result = "GameOver_Continue";
        break;
      case GAKind.GameOver_NotContinue:
        result = "GameOver_NotContinue";
        break;
      case GAKind.Rank_Friend:
        result = "Rank_Friend";
        break;
      case GAKind.Rank_Player:
        result = "Rank_Player";
        break;
      case GAKind.Rank_GameHistory:
        result = "Rank_GameHistory";
        break;
      case GAKind.FriendRank_Lv:
        result = "FriendRank_Lv";
        break;
      case GAKind.FriendRank_Win:
        result = "FriendRank_Win";
        break;
      case GAKind.FriendRank_Follow:
        result = "FriendRank_Follow";
        break;
      case GAKind.PlayerRank_Lv:
        result = "PlayerRank_Lv";
        break;
      case GAKind.PlayerRank_Win:
        result = "PlayerRank_Win";
        break;
      case GAKind.PlayerRank_Follow:
        result = "PlayerRank_Follow";
        break;
      case GAKind.Store_Buy:
        result = "Store_Buy";
        break;
      case GAKind.Store_Fitting:
        result = "Store_Fitting";
        break;
      case GAKind.Store_Money:
        result = "Store_Money";
        break;
      case GAKind.Buy_Hair:
        result = "Buy_Hair";
        break;
      case GAKind.Buy_Head:
        result = "Buy_Head";
        break;
      case GAKind.Buy_Facial:
        result = "Buy_Facial";
        break;
      case GAKind.Buy_Cloth:
        result = "Buy_Cloth";
        break;
      case GAKind.Buy_Acce:
        result = "Buy_Acce";
        break;
      case GAKind.Buy_Emotion:
        result = "Buy_Emotion";
        break;
      case GAKind.Fitting_Hair:
        result = "Fitting_Hair";
        break;
      case GAKind.Fitting_Head:
        result = "Fitting_Head";
        break;
      case GAKind.Fitting_Facial:
        result = "Fitting_Facial";
        break;
      case GAKind.Fitting_Cloth:
        result = "Fitting_Cloth";
        break;
      case GAKind.Fitting_Acce:
        result = "Fitting_Acce";
        break;
      case GAKind.Fitting_Style1:
        result = "Fitting_Style1";
        break;
      case GAKind.Fitting_Style2:
        result = "Fitting_Style2";
        break;
      case GAKind.Fitting_Save:
        result = "Fitting_Save";
        break;
      case GAKind.Vip_Info:
        result = "Vip_Info";
        break;      
    }
    
    return result;
  }
}


