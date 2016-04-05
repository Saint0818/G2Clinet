﻿using UnityEngine;
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
        private GameObject optionsBtnGroup;
        private UIButton optionsBtn;
        private bool isInit = false;

        public void Init(ref GameObject go, EventDelegate holdbtn = null, EventDelegate optionsFunc = null)
        {
            if(go){
                self = go;
                btn = self.GetComponent<UIButton>();
                playerName = self.transform.Find("Window/PlayerName/NameLabel").gameObject.GetComponent<UILabel>();
                combatLabel = self.transform.Find("Window/CombatLabel").gameObject.GetComponent<UILabel>();
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
				if (rankData.Index == 0)
					playerIndex.text = "";
				else
                	playerIndex.text = rankData.Index.ToString();
                playerName.text = rankData.Team.Player.Name;
                playeHeadBtn.UpdateView(rankData.Team.Player);
                combatLabel.text = rankData.Team.Player.CombatPower().ToString ();
                PvPRankIcon.spriteName = string.Format("IconRank{0}", rankData.Team.PVPLv);
                WinLabel.text = rankData.Team.LifetimeRecord.PVPWin.ToString();
                if (rankData.Team.LifetimeRecord.PVPWin == 0 && rankData.Team.LifetimeRecord.PVPCount == 0)
                {
                    WinRateLabel.text = "100%";
                }
                else
                    WinRateLabel.text = string.Format("{0:0%}", (float)rankData.Team.LifetimeRecord.PVPWin / (float)rankData.Team.LifetimeRecord.PVPCount);
                PVPIntegral.text = rankData.Team.PVPIntegral.ToString();
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

    public class TPvPLeagueGroup
    {
        public GameObject self;
        private UISprite PvPRankIcon;
		private UISprite spriteStep;
        private UILabel RangeNameLabel;
		private UILabel labelStep;
        private bool isInit = false;

        public void Init(ref GameObject go, GameObject parent)
        {
            if (go)
            {
                self = go;
                self.transform.parent = parent.transform;
                PvPRankIcon = self.transform.Find("PvPRankIcon").GetComponent<UISprite>();
                RangeNameLabel = self.transform.Find("RangeNameLabel").GetComponent<UILabel>();
				labelStep = self.transform.Find("Step/LabelStep").GetComponent<UILabel>();
				spriteStep = self.transform.Find("Step").GetComponent<UISprite>();

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

				if (lv == GameData.Team.PVPLv) {
					labelStep.text = TextConst.S (9748);
					spriteStep.spriteName = "Success";
				} else if (lv < GameData.Team.PVPLv) {
					labelStep.text = TextConst.S (97478);
					spriteStep.spriteName = "Select";
				} else {
					labelStep.text = "";
					spriteStep.spriteName = "";
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