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
                headTexture = self.transform.Find("PlayerPic").GetComponent<UISprite>();
                position = headTexture.transform.Find("PositionIcon").GetComponent<UISprite>();
                lv = self.transform.Find("LevelGroup").GetComponent<UILabel>();
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
        private UILabel playerIndex;
        private UILabel playerName;
        private UISprite PvPRankIcon;
        private UILabel combatLabel;
		private UISprite GuildIcon;
		private UILabel GuildIDLabel;
        private UILabel WinLabel;
        private UILabel WinRateLabel;
        private UILabel PVPIntegral;
        private UIButton btn;
        private UISprite BG;
        private GameObject optionsBtnGroup;
        private GameObject uiNo;
        private UIButton optionsBtn;
        private bool isInit = false;

        public void Init(ref GameObject go, EventDelegate holdbtn = null, EventDelegate optionsFunc = null)
        {
            if(go){
                self = go;
                btn = self.GetComponent<UIButton>();
                BG = self.transform.Find("Window/RankBG").gameObject.GetComponent<UISprite>();
                uiNo = self.transform.Find("Window/PVPPlaceLabel/BG").gameObject;
                playerName = self.transform.Find("Window/PlayerName/NameLabel").gameObject.GetComponent<UILabel>();
                combatLabel = self.transform.Find("Window/PlayerInGameBtn/CombatLabel").gameObject.GetComponent<UILabel>();
                GameObject obj = self.transform.Find("Window/PlayerInGameBtn").gameObject;
                PvPRankIcon = self.transform.Find("Window/PvPRankIcon").gameObject.GetComponent<UISprite>();
				GuildIcon = self.transform.Find("Window/GuildView/GuildIcon").gameObject.GetComponent<UISprite>();
				GuildIDLabel = self.transform.Find("Window/GuildView/GuildIDLabel").gameObject.GetComponent<UILabel>();
				self.transform.Find("Window/GuildView").gameObject.SetActive(false);
                WinLabel = self.transform.Find("Window/DetailGroup/WinLabel").gameObject.GetComponent<UILabel>();
                WinLabel.transform.Find("Label").gameObject.GetComponent<UILabel>().text = TextConst.S(9734);
                WinRateLabel = self.transform.Find("Window/DetailGroup/WinRateLabel").gameObject.GetComponent<UILabel>();
                WinRateLabel.transform.Find("Label").gameObject.GetComponent<UILabel>().text = TextConst.S(9733);
                self.transform.Find("Window/DetailGroup/ScoreIcon/Label").gameObject.GetComponent<UILabel>().text = TextConst.S(9735);
                PVPIntegral = self.transform.Find("Window/DetailGroup/ScoreIcon/ScoreLabel").gameObject.GetComponent<UILabel>();
                optionsBtnGroup = self.transform.Find("Window/ButtonListGroup").gameObject;
                optionsBtn =  optionsBtnGroup.transform.Find("View/ProfileBtn").gameObject.GetComponent<UIButton>();
                playerIndex = self.transform.Find("Window/PVPPlaceLabel").gameObject.GetComponent<UILabel>();

				isInit = self && btn && playerName && obj && combatLabel && PvPRankIcon && GuildIcon 
                    && GuildIDLabel && WinLabel && WinRateLabel && PVPIntegral && optionsBtnGroup && optionsBtn && playerIndex;

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
                rankData.Team.Init();
				
                playerName.text = rankData.Team.Player.Name;
                if (rankData.Team.Identifier == GameData.Team.Identifier)
                    BG.color = new Color32(0, 230, 255, 255);
                else
                    BG.color = Color.white;

                playeHeadBtn.UpdateView(rankData.Team.Player);
                combatLabel.text = string.Format("{0:F0}", rankData.Team.Player.CombatPower());
                PvPRankIcon.spriteName = string.Format("IconRank{0}", rankData.Team.PVPLv);
                WinLabel.text = rankData.Team.LifetimeRecord.PVPWin.ToString();
                PVPIntegral.text = rankData.Team.PVPIntegral.ToString();

                if (rankData.Index == 0)
                    playerIndex.text = "";
                else {
                    playerIndex.text = rankData.Index.ToString() + ".";
                    switch (rankData.Index) {
                        case 1: playerIndex.color = Color.yellow; break;
                        case 2: playerIndex.color = Color.blue; break;
                        case 3: playerIndex.color = Color.green; break;
                        default : uiNo.SetActive(false); break;
                    }   
                }
                
                if ((rankData.Team.LifetimeRecord.PVPWin == 0 && rankData.Team.LifetimeRecord.PVPCount == 0) ||
                    (rankData.Team.LifetimeRecord.PVPWin >= rankData.Team.LifetimeRecord.PVPCount))
                    WinRateLabel.text = "100%";
                else
                    WinRateLabel.text = string.Format("{0:0%}", (float)rankData.Team.LifetimeRecord.PVPWin / (float)rankData.Team.LifetimeRecord.PVPCount);

            }
        }

        public void SetParent(GameObject go)
        {
            self.transform.parent = go.transform;
			self.transform.localScale = Vector3.one;
        }

        public Vector3 LocalPosititon
        {
            set{ self.transform.localPosition = value;}
        }
    }

    public class TPVPLeagueGroup
    {
        public GameObject self;
        public GameObject ShineFX;
        private UISprite PvPRankIcon;
		private UISprite spritePass;
        private UILabel RangeNameLabel;
		private UILabel labelPass;
        private UILabel labelStep;
        private bool isInit = false;

        public void Init(ref GameObject go, GameObject parent)
        {
            if (go)
            {
                self = go;
                self.transform.parent = parent.transform;
                ShineFX = self.transform.Find("ShineFX").gameObject;
                PvPRankIcon = self.transform.Find("PvPRankIcon").GetComponent<UISprite>();
                RangeNameLabel = self.transform.Find("RangeNameLabel").GetComponent<UILabel>();
                labelStep = self.transform.Find("LabelStep").GetComponent<UILabel>();
                labelPass = self.transform.Find("Pass/LabelPass").GetComponent<UILabel>();
                spritePass = self.transform.Find("Pass").GetComponent<UISprite>();

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
                labelStep.text = string.Format(TextConst.S (9737), lv);

                ShineFX.SetActive(false);
				if (lv == GameData.Team.PVPLv) {
                    ShineFX.SetActive(true);
					labelPass.text = TextConst.S (9748);
					spritePass.spriteName = "Success";
				} else 
                if (lv < GameData.Team.PVPLv) {
					labelPass.text = TextConst.S (9747);
					spritePass.spriteName = "Select";
				} else {
					labelPass.text = "";
					spritePass.spriteName = "";
				}
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