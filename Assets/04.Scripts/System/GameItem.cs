using UnityEngine;
using System.Collections;
using GameStruct;

namespace GameItem
{
    public class TPlayerInGameBtn
    {
        private GameObject self;
        private UISprite headTexture;
        private UISprite position;
        private UILabel lv;

        public void Init(GameObject go)
        {
            if (go)
            {
                self = go;
                headTexture = self.transform.FindChild("PlayerPic").GetComponent<UISprite>();
                position = headTexture.transform.FindChild("PositionIcon").GetComponent<UISprite>();
                lv = self.transform.FindChild("LevelGroup").GetComponent<UILabel>();
            }
        }

        public void UpdateView(TPlayer player)
        {
            headTexture.spriteName = player.FacePicture;
            position.spriteName = player.PositionPicture;
            lv.text = player.Lv.ToString();
        }
    }

    public class TItemRankGroup
    {
        private GameObject self;
        private TPlayerInGameBtn playeHeadBtn;
        private UILabel playerName;
        private UISprite PvPRankIcon;
        private UILabel combatLabel;
		private UISprite GuildIcon;
		private UILabel GuildIDLabel;
        private UILabel WinLabel;
        private UILabel WinRateLabel;
        private UILabel PVPIntegral;
        private UIButton btn;
        private GameObject optionsBtnGroup;
        private UIButton optionsBtn;
        private bool isInit = false;

        public void Init(ref GameObject go, EventDelegate holdbtn = null, EventDelegate optionsFunc = null)
        {
            if(go){
                self = go;
                btn = self.GetComponent<UIButton>();
                playerName = self.transform.FindChild("Window/PlayerName/NameLabel").gameObject.GetComponent<UILabel>();
                combatLabel = self.transform.FindChild("Window/CombatLabel").gameObject.GetComponent<UILabel>();
                GameObject obj = self.transform.FindChild("Window/PlayerInGameBtn").gameObject;
                PvPRankIcon = self.transform.FindChild("Window/PvPRankIcon").gameObject.GetComponent<UISprite>();
				GuildIcon = self.transform.FindChild("Window/GuildView/GuildIcon").gameObject.GetComponent<UISprite>();
				GuildIDLabel = self.transform.FindChild("Window/GuildView/GuildIDLabel").gameObject.GetComponent<UILabel>();
                WinLabel = self.transform.FindChild("Window/DetailGroup/WinLabel").gameObject.GetComponent<UILabel>();
                WinRateLabel = self.transform.FindChild("Window/DetailGroup/WinRateLabel").gameObject.GetComponent<UILabel>();
                PVPIntegral = self.transform.FindChild("Window/DetailGroup/ScoreIcon/ScoreLabel").gameObject.GetComponent<UILabel>();
                optionsBtnGroup = self.transform.FindChild("Window/ButtonListGroup").gameObject;
                optionsBtn =  optionsBtnGroup.transform.FindChild("View/ProfileBtn").gameObject.GetComponent<UIButton>();

				isInit = self && btn && playerName && obj && combatLabel && PvPRankIcon && GuildIcon 
                    && GuildIDLabel && WinLabel && WinRateLabel && PVPIntegral && optionsBtnGroup && optionsBtn;

                if (isInit)
                {
                    playeHeadBtn = new TPlayerInGameBtn();
                    playeHeadBtn.Init(obj);

                    if (holdbtn != null)
                        btn.onClick.Add(holdbtn);

                    if (optionsFunc != null)
                    {
                        optionsBtn.onClick.Add(optionsFunc);
                    }
                    else
                    {
                        optionsBtnGroup.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Error : Init TItemRankGroup");
                }
            }
        }

        public bool Enable
        {
            set{ self.SetActive(value);}
            get{ return self.activeSelf;} 
        }

        public void UpdateView(TTeamRank rankData)
        {
            if (isInit)
            {
                playerName.text = rankData.Player.Name;
                playeHeadBtn.UpdateView(rankData.Player);
                combatLabel.text = rankData.Player.CombatPower().ToString ();
                PvPRankIcon.spriteName = string.Format("IconRank{0}", GameFunction.GetPVPLv(rankData.PVPIntegral));
				GuildIcon.spriteName = string.Format("LeagueIcon{0}", rankData.GuildIIcon);
                GuildIDLabel.text = rankData.GuildIName;
                WinLabel.text = rankData.PVPWin.ToString();
                WinRateLabel.text = string.Format("{0:0%}", (float)rankData.PVPWin / (float)rankData.PVPCount);
                PVPIntegral.text = rankData.PVPIntegral.ToString();
            }
        }

        public void SetParent(GameObject go)
        {
            if(isInit)
                self.transform.parent = go.transform;
        }

        public Vector3 LocalPosititon
        {
            set{ self.transform.localPosition = value;}
        }
    }

    public class TPvPLeagueGroup
    {
        public GameObject self;
        private UISprite PvPRankIcon;
        private UILabel RangeNameLabel;
        private bool isInit = false;

        public void Init(ref GameObject go, GameObject parent)
        {
            if (go)
            {
                self = go;
                self.transform.parent = parent.transform;
                PvPRankIcon = self.transform.FindChild("PvPRankIcon").GetComponent<UISprite>();
                RangeNameLabel = self.transform.FindChild("RangeNameLabel").GetComponent<UILabel>();

                isInit = PvPRankIcon && RangeNameLabel;

                if (!isInit)
                    Debug.LogError("Init Error");
            }
        }

        public bool Enable
        {
            set{ self.SetActive(value);}
            get{ return self.activeSelf;} 
        }
            
        public void UpdateView(int lv = 1)
        {
            if (isInit && GameData.DPVPData.ContainsKey(lv))
            {
                PvPRankIcon.spriteName = string.Format("IconRank{0}", lv);
                RangeNameLabel.text = GameData.DPVPData[lv].Name;
            }
        }

        public Vector3 LoacalScale
        {
            set{ self.transform.localScale = value;}
            get{ return self.transform.localScale;}
        }

        public Vector3 LoaclPosition
        {
			set{ self.transform.localPosition = value;}
			get{ return self.transform.localPosition;}
        }
    }
}