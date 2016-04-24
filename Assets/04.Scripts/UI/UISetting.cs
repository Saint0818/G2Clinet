﻿using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class GameSettingView
{
	private GameObject self;
	private UIButton musicBtn;
	private UIButton soundBtn;
	private UIButton[] effectBtn = new UIButton[3];
    private UIToggle[] effectBtnToggle = new UIToggle[3];
	private UIButton[] aiBtns = new UIButton[6];
	private UIToggle[] aiBtnsToggle = new UIToggle[6];
	private UISprite[] musicSp = new UISprite[2];
	private UISprite[] soundSp = new UISprite[2];
	private UILabel warningLabel;
	private ELanguage language;

	public void Init(GameObject obj)
	{
		if (obj) {
			self = obj;
			musicBtn = self.transform.FindChild("Music/0").gameObject.GetComponent<UIButton>();
			soundBtn = self.transform.FindChild("Music/1").gameObject.GetComponent<UIButton>();
			
			warningLabel = self.transform.FindChild("GameAI/WarningLabel").gameObject.GetComponent<UILabel>();

			for(int i = 0; i< aiBtns.Length;i++)
			{
				aiBtns[i] = self.transform.FindChild(string.Format("GameAI/{0}", i)).gameObject.GetComponent<UIButton>();
				aiBtnsToggle[i] = aiBtns[i].GetComponent<UIToggle>();
			}

            for (int i = 0; i < effectBtn.Length; i++)
            {
                effectBtn[i] = self.transform.FindChild("Effect/" + i).gameObject.GetComponent<UIButton>();
                effectBtnToggle[i] = effectBtn[i].GetComponent<UIToggle>();
            }

			for(int i = 0; i < musicSp.Length; i++)
			{
				musicSp[i] = musicBtn.transform.FindChild(string.Format("Switch{0}", i)).gameObject.GetComponent<UISprite>();
			}

			for(int i = 0; i < soundSp.Length; i++)
			{
				soundSp[i] = soundBtn.transform.FindChild(string.Format("Switch{0}", i)).gameObject.GetComponent<UISprite>();
			}
		}
		language = GameData.Setting.Language;
	}

	public void InitBtttonFunction(EventDelegate musicFunc, EventDelegate soundFunc, EventDelegate effectFunc, EventDelegate ailvFunc)
	{
		musicBtn.onClick.Add (musicFunc);
		soundBtn.onClick.Add (soundFunc);
       
		for(int i = 0; i< aiBtns.Length;i++)
			aiBtns[i].onClick.Add (ailvFunc);
        
        for(int i = 0; i< effectBtn.Length;i++)
            effectBtn[i].onClick.Add (effectFunc);
	}

	public void UpdateView()
	{
		musicSp [0].enabled = GameData.Setting.Music;
		musicSp [1].enabled = !GameData.Setting.Music;
		soundSp [0].enabled = GameData.Setting.Sound;
		soundSp [1].enabled = !GameData.Setting.Sound;

        for (int i = 0; i < effectBtnToggle.Length; i++)
           effectBtnToggle[i].Set(i == GameData.Setting.Quality);

		for (int i = 0; i< aiBtnsToggle.Length; i++) {
			aiBtnsToggle[i].Set (i == GameData.Setting.AIChangeTimeLv);
		}

		string tip = GameData.Setting.AIChangeTimeLv == 5 ? "∞" : GameConst.AITime [GameData.Setting.AIChangeTimeLv].ToString();
		warningLabel.text = TextConst.StringFormat (12013, tip);

		if (language != GameData.Setting.Language) {
			UISetting.Get.initDefaultText(self);
			language = GameData.Setting.Language;
		}
	}

	public bool Enable
	{
		set{self.gameObject.SetActive(value);}
		get{return self.gameObject.activeSelf;}
	}
}

public class LanguageView
{
	private GameObject self;
	private UIButton[] lnBtns = new UIButton[2];
	private UIToggle[] btnsToggle = new UIToggle[2];
	private ELanguage language;
	
	public void Init(GameObject obj)
	{
		if (obj) {
			self = obj;
			
			for(int i = 0; i< lnBtns.Length;i++)
			{
				lnBtns[i] = self.transform.FindChild(string.Format("Languages/{0}", i)).gameObject.GetComponent<UIButton>();
				btnsToggle[i] = lnBtns[i].GetComponent<UIToggle>();
			}
		}
		language = GameData.Setting.Language;
	}
	
	public void InitBtttonFunction(EventDelegate languagesFunc)
	{
		for(int i = 0; i< lnBtns.Length;i++)
			lnBtns[i].onClick.Add (languagesFunc);
	}
	
	public void UpdateView()
	{
        int index = language.GetHashCode();
		for(int i = 0; i< btnsToggle.Length;i++)
			btnsToggle[i].Set(i == index);

		if (language != GameData.Setting.Language) {
			UISetting.Get.initDefaultText(self);
			language = GameData.Setting.Language;
		}
	}

	public bool Enable
	{
		set{self.gameObject.SetActive(value);}
		get{return self.gameObject.activeSelf;}
	}
}

public class AccountView
{
	private GameObject self;
	private UIButton othterCharacterBtn;
	private ELanguage language;
    private GameObject uiPubgame;
    public UILabel labelWidge;

	public void Init(GameObject obj)
	{
		if (obj) {
			self = obj;
            uiPubgame = self.transform.FindChild("Pubgame").gameObject;
            labelWidge = self.transform.FindChild("Pubgame/Widge/Label").gameObject.GetComponent<UILabel>();
			othterCharacterBtn = self.transform.FindChild("Account/0").gameObject.GetComponent<UIButton>();

            if (GameData.Company != ECompany.PubGame)
                uiPubgame.SetActive(false);
		}

		language = GameData.Setting.Language;
	}

	public void InitBtttonFunction(EventDelegate othterCharacterFunc)
	{
		othterCharacterBtn.onClick.Add (othterCharacterFunc);
	}

	public bool Enable
	{
		set{self.gameObject.SetActive(value);}
		get{return self.gameObject.activeSelf;}
	}

	public void UpdateView()
	{
		if (language != GameData.Setting.Language) {
			UISetting.Get.initDefaultText(self);
			language = GameData.Setting.Language;
		}
	}
}

public class UISetting : UIBase {
	private static UISetting instance = null;
	private const string UIName = "UISetting";
	private GameObject[] pages = new GameObject[4];
	private GameObject[] tabs = new GameObject[4];

	private UIToggle toggleMove;
	private UIToggle toggleRotation;
	private GameSettingView gameSetting = new GameSettingView();
	private LanguageView languageSetting = new LanguageView();
	private AccountView accountSetting = new AccountView();
    private int currentLanguage = 0;

	private UILabel version;
	private bool isMusicOn;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISetting Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISetting;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, bool isMainLobby = true){
		if (instance) {
			if (!isShow){
                RemoveUI(instance.gameObject);
			}
			else 
				instance.Show(isShow);
		} else {
			if (isShow) {
				Get.Show(isShow);
				if(!isMainLobby) 
					for(int i=0; i<Get.tabs.Length; i++)
						if(i != 0)
							Get.tabs[i].SetActive(false);			
			}
		}
	}

	protected override void InitCom() {
        SetBtnFun(UIName + "Window/Center/NoBtn", OnReturn);
        SetBtnFun(UIName + "Window/Center/Pages/2/GameNews/0", OnNotic);
        SetBtnFun(UIName + "/Window/Center/Pages/2/Pubgame/Widge", OnPubgameWidge);
		SetBtnFun (UIName + "/Window/Center/Pages/0/CheckMove", OnMove);
		SetBtnFun (UIName + "/Window/Center/Pages/0/CheckRotation", OnRotation);

		toggleMove = GameObject.Find (UIName + "/Window/Center/Pages/0/CheckMove").GetComponent<UIToggle> ();
		toggleRotation = GameObject.Find (UIName + "/Window/Center/Pages/0/CheckRotation").GetComponent<UIToggle> ();

		for (int i = 0; i < pages.Length; i++) {
			pages[i] = GameObject.Find(UIName + string.Format("Window/Center/Pages/{0}", i));
			tabs[i] = GameObject.Find(UIName + string.Format("Window/Center/Tabs/{0}", i));
			SetBtnFun(UIName + string.Format("Window/Center/Tabs/{0}", i), OnPage);

			switch(i)
			{
				case 0:
					gameSetting.Init(pages[i]);
					gameSetting.InitBtttonFunction(new EventDelegate(OnMusic), new EventDelegate(OnSound), new EventDelegate(OnQuality), new EventDelegate(OnAILv));
					break;
				case 1:
					languageSetting.Init(pages[i]);
					languageSetting.InitBtttonFunction(new EventDelegate(OnLanguage));					
					break;
				case 2:
					accountSetting.Init(pages[i]);
					accountSetting.InitBtttonFunction(new EventDelegate(OnOtherCharacter));
					break;
				case 3:
					pages[i].gameObject.SetActive(false);
					GameObject.Find(UIName + string.Format("Window/Center/Tabs/{0}", i)).SetActive(false);
					break;
			}
		}

		version = GameObject.Find (UIName + "Window/Center/Pages/VersionLabel").gameObject.GetComponent<UILabel> ();
	}

	protected override void InitData() {
		OnPage ();
        currentLanguage = GameData.Setting.Language.GetHashCode();
	}

	public void OnPage()
	{
		int index = 0;

		if(UIButton.current)
			int.TryParse (UIButton.current.name, out index);

		for (int i = 0; i < pages.Length; i++) {
			pages[i].SetActive(index == i);
		}

		switch (index) 
		{
			case 0:
				gameSetting.UpdateView();	
				break;
			case 1:
				languageSetting.UpdateView();
				break;
			case 2:
				accountSetting.UpdateView();
				break;
		}

		version.text = TextConst.StringFormat (12006, BundleVersion.Version);
	}

	public void OnMove() {
		GameData.Setting.GameMove = toggleMove.value;
	}

	public void OnRotation() {
		GameData.Setting.GameRotation = toggleRotation.value;
	}

	public void OnNotic()
	{
        UINotic.Visible = true;
	}

    public void OnPubgameWidge() {
        if (UIPubgame.Get.OnSwitchWidge())
            accountSetting.labelWidge.text = TextConst.S(12039);
        else
            accountSetting.labelWidge.text = TextConst.S(12038);
    }

	public void OnReturn()
	{
        if (currentLanguage != GameData.Setting.Language.GetHashCode())
            SendChangeLanguage();
        
		UIShow (false);
	}

    private void SendChangeLanguage()
    {
        WWWForm form = new WWWForm();
        form.AddField("Language", GameData.Setting.Language.GetHashCode());
        SendHttp.Get.Command(URLConst.ChangeLanguage, null, form, false);
    }

	public void OnMusic()
	{
		GameData.Setting.Music = !GameData.Setting.Music;
		AudioMgr.Get.MusicOn(GameData.Setting.Music);
		gameSetting.UpdateView ();

		PlayerPrefs.SetInt(ESave.MusicOn.ToString(), GameData.Setting.Music == true? 1 : 0);
		PlayerPrefs.Save ();
	}

	public void OnSound()
	{
		GameData.Setting.Sound = !GameData.Setting.Sound;
		AudioMgr.Get.SoundOn(GameData.Setting.Sound);
		gameSetting.UpdateView ();

		PlayerPrefs.SetInt(ESave.SoundOn.ToString(), GameData.Setting.Sound == true? 1 : 0);
		PlayerPrefs.Save ();
	}

	public void OnQuality()
	{
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
        {
            if (GameData.Setting.Quality != index)
            {
                GameData.Setting.Quality = index;
                gameSetting.UpdateView ();

                //Setting 
                GameData.SetGameQuality((EQualityType)GameData.Setting.Quality);
            }
        }
		
        PlayerPrefs.SetInt(ESave.Quality.ToString(), GameData.Setting.Quality);
		PlayerPrefs.Save ();
	}

	public void OnAILv()
	{
		int index;
		
		if (int.TryParse (UIButton.current.name, out index)) {
			if(index < GameConst.AITime.Length)
				GameData.Setting.AIChangeTimeLv = index;
				PlayerPrefs.SetInt(ESave.AIChangeTimeLv.ToString(), index);
				PlayerPrefs.Save();
		}

		gameSetting.UpdateView ();
		if(GameController.Visible)
			GameController.Get.Joysticker.SetManually();
	}

	public void OnLanguage()
	{
        int index = 0;

        //順序 EN = 0,TW = 1, CN = 2, JP = 3

        if (int.TryParse (UIButton.current.name, out index)) {
            if (GameData.Setting.Language != (ELanguage)index) 
			{
                GameData.Setting.Language = (ELanguage)index;
                DoLanguage ();				
			}
		}
	}

	public void DoLanguage()
	{
        int index = GameData.Setting.Language.GetHashCode();
        PlayerPrefs.SetInt (ESave.UserLanguage.ToString(), index);
		PlayerPrefs.Save ();

		if (UISetting.Visible)
			UISetting.UIShow(true);

		if (UIMainLobby.Get.IsVisible)
			UIMainLobby.Get.UpdateText();

		if(UIGym.Visible)
			UIGym.Get.UpdateText();

		if(UIGymEngage.Visible)
			UIGymEngage.Get.UpdateText();

		version.text = TextConst.StringFormat (12006, BundleVersion.Version);
	}

	public void DoOtherCharacter(object obj)
	{
//		WWWForm form = new WWWForm();
//		SendHttp.Get.Command(URLConst.LookPlayerBank, waitLookPlayerBank, form);

        LookupPlayerBanksProtocol protocol = new LookupPlayerBanksProtocol();
        protocol.Send(waitLookupPlayerBanks);
	}

	public void OnOtherCharacter()
	{
		UIMessage.Get.ShowMessage(TextConst.S(211), TextConst.S(210), DoOtherCharacter);
	}

	private void waitLookupPlayerBanks(bool isSuccess, LookupPlayerBanksProtocol.Data data)
	{
	    if(isSuccess)
	    {
            var uiData = UICreateRoleBuilder.Build(data.PlayerBank);
            UICreateRole.Get.ShowFrameView(uiData, data.SelectedRoleIndex);

            UIShow(false);
            AudioMgr.Get.PlayMusic(EMusicType.MU_Create);
        }
	}
}
