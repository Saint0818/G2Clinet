﻿using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEngine;

public struct TPlayerInfo
{
    public int Kind;
    public int Value;
}

public class GEGMTool : GEBase
{
    private int options = 0;
    private int itemOptions = 0;
    private string[] optionsTitle = new string[7]{ "物品", "關卡", "戰鬥", "場景", "人物資料", "其它" ,"介面"};
    private string[] itemOptionsTitle = new string[4]{ "Add Item單件物品", "Add Item每一個部位", "Add Item指定部位", "人物裝備時限" };

    void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            options = GUILayout.Toolbar(options, optionsTitle);
            switch (options)
            {
                case 0:
                    ItemHandle();
                    break;
                case 1:
                    StageHandle();
                    break;
                case 2:
                    BattleHandle();
                    break;
                case 3:
                    SceneHandle();
                    break;
                case 4:
                    PlayerInfoHandle();
                    break;
                case 5:
                    int i = 0;
                    saveOptions = new string[Enum.GetNames(typeof(ESave)).Length];
                    foreach (ESave item in Enum.GetValues(typeof(ESave)))
                    {
                        saveOptions[i] = item.ToString();
                        i++;
                    }
                    OtherHandle();
                    break;
				case 6:
					UIHandle ();
					break;
								
            }
        }
        else
            GUILayout.Label("想用？先執行遊戲再說"); 
    }

    private int addItemCount = 1;
    private int[] itemIds;
    private int[] NumberOfItems;
    private int countprekind = 1;
    private int playerPosition = 0;

    private void ItemHandle()
    {
        itemOptions = GUILayout.Toolbar(itemOptions, itemOptionsTitle);
        switch (itemOptions)
        {
            case 0:
                AddItemBySingle();
                break;
            case 1:
                AddItemByEveryPart();
                break;
            case 2:
                AddItemByLimitPart();
                break;
            case 3:
                PlayerItemsSetting();
                break;   
        }
    }

    private void AddItemBySingle()
    {
        //Add Item
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("物品陣列 : "); 
        addItemCount = EditorGUILayout.IntField(addItemCount, GUILayout.Width(100));
        if (GUILayout.Button("設定", GUILayout.Width(200)))
        {
            itemIds = new int[addItemCount];
            NumberOfItems = new int[addItemCount];
            for (int i = 0; i < itemIds.Length; i++)
            {
                itemIds[i] = -1;
                NumberOfItems[i] = 1;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (itemIds != null && itemIds.Length > 0)
            for (int i = 0; i < itemIds.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("物品編號 : "); 
                itemIds[i] = EditorGUILayout.IntField(itemIds[i], GUILayout.Width(100));
                GUILayout.Label("物品數量 : "); 
                NumberOfItems[i] = EditorGUILayout.IntField(NumberOfItems[i], GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("AddItem", GUILayout.Width(200)))
        {
            if (itemIds != null && itemIds.Length > 0)
            {
                WWWForm form = new WWWForm();
                form.AddField("AddIndexs", JsonConvert.SerializeObject(itemIds));
                form.AddField("AddNumberOfItems", JsonConvert.SerializeObject(NumberOfItems));
                SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
            }
            else
                ShowHint("請設定Item數量");
        }

        if (GUILayout.Button("Remove", GUILayout.Width(200)))
        {
            if (itemIds != null && itemIds.Length > 0)
            {
                WWWForm form = new WWWForm();
                form.AddField("RemoveIndexs", JsonConvert.SerializeObject(itemIds));
                SendHttp.Get.Command(URLConst.GMRemoveItem, waitGMAddItem, form);
            }
            else
                ShowHint("請設定Item數量");
        }

        if (GUILayout.Button("刪除背包", GUILayout.Width(200)))
        {
            WWWForm form = new WWWForm();
            form.AddField("RemoveAll", "true");
            SendHttp.Get.Command(URLConst.GMRemoveItem, waitGMAddItem, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private int AvatarPotential = 0;
    private int CrtAvatarPotential = 0;
    private int LvPotential = 0;
    private int CrtLvPotential = 0;

    private int playerlv = 0;
    private int avatarPotential = 0;
    private bool IsInitPlayerInfo = false;
    private int useLvPotential = 0;
    private int useAvatarPotential = 0;
    private int[] addPotential = new int[GameConst.PotentialCount];
    private Dictionary<EAttribute, int> Potential = new Dictionary<EAttribute, int>();

    private void InitPotentialPoint()
    {
        AvatarPotential = GameData.Team.AvatarPotential;
        avatarPotential = AvatarPotential;
        LvPotential = GameFunction.GetLvPotential(GameData.Team.Player.Lv);
        CrtAvatarPotential = GameFunction.GetAllPlayerTotalUseAvatarPotential();
        CrtLvPotential = GameFunction.GetCurrentLvPotential(GameData.Team.Player);
    }

    private void PlayerInfoHandle()
    {
        if (GUILayout.Button("讀取資料", GUILayout.Width(200)))
        {
            playerlv = GameData.Team.Player.Lv;
            Potential = GameData.Team.Player.Potential;
            InitPotentialPoint();
            IsInitPlayerInfo = true; 
        }

        if (!IsInitPlayerInfo)
            return;

        SetPlayeLv();
        AddAvatarPotential();

        if (Potential.Count > 0)
        {
            foreach (KeyValuePair<EAttribute, int> item in Potential)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0} : {1} + {2}/100", item.Key.ToString(), item.Value, addPotential[GameFunction.GetAttributeIndex(item.Key)])); 
                if (GUILayout.Button("+", GUILayout.Width(200)))
                {
                    if (CrtAvatarPotential > 0 && item.Value < 100)
                    {
                        if (CanUsePotential(GameFunction.GetAttributeIndex(item.Key)))
                        {
                            addPotential[GameFunction.GetAttributeIndex(item.Key)]++;
                            CalculateAddPotential();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("取消配點", GUILayout.Width(50)))
        {
            for (int i = 0; i < addPotential.Length; i++)
                addPotential[i] = 0;
			
            CalculateAddPotential();
        }

        if (GUILayout.Button("重置", GUILayout.Width(50)))
        {
//			for(int i = 0;i < Potential.Length; i++)
//				Potential[i] = 0;
//
//			InitPotentialPoint();
        }

        if (GUILayout.Button("存檔", GUILayout.Width(200)))
        {
            if (HaveChange())
            {
                WWWForm form = new WWWForm();
                Dictionary<EAttribute, int> save = new Dictionary<EAttribute, int>();
                save = GameFunction.SumAttribute(GameData.Team.Player.Potential, addPotential);

                form.AddField("Potential", JsonConvert.SerializeObject(save));
                SendHttp.Get.Command(URLConst.GMSavePotential, waitSaveMasteries, form);
            }


        }
        EditorGUILayout.EndHorizontal();
    }

    private int deleteSelected = 0;
    private string[] saveOptions;

    private void OtherHandle()
    {
        if (GUILayout.Button("刪除玩家all存檔", GUILayout.Width(200)))
        {
            PlayerPrefs.DeleteAll();
        }

        EditorGUILayout.BeginHorizontal();
        deleteSelected = EditorGUILayout.Popup("刪除玩家single存檔", deleteSelected, saveOptions);
        if (GUILayout.Button("刪除", GUILayout.Width(200)))
        {
            PlayerPrefs.DeleteKey(saveOptions[deleteSelected]);
        }
        EditorGUILayout.EndHorizontal();

        addMoney();
        addDiamond();
        addPower();
        addExp();
        setMaxPlayerBank();
        resetDailyLoginNums();
        setDailyLoginNums();
        resetReceivedDailyLoginNums();
        setLifetimeLoginNum();
        clearLifetimeReceivedLoginNum();
        deleteAllPlayerPrefabs();
    }

    private int mMaxPlayerBank = 2;

    private void setMaxPlayerBank()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Set MaxPlayerBank:");
        mMaxPlayerBank = EditorGUILayout.IntField(mMaxPlayerBank, GUILayout.Width(100));
        if (GUILayout.Button("Set", GUILayout.Width(50)))
        {
            var protocol = new GMSetMaxPlayerBankProtocol();
            protocol.Send(mMaxPlayerBank, ok => {});
        }
        EditorGUILayout.EndHorizontal();
    }

    private int mAddMoney;

    private void addMoney()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Add Money");
        mAddMoney = EditorGUILayout.IntField(mAddMoney, GUILayout.Width(100));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("AddMoney", mAddMoney);
            SendHttp.Get.Command(URLConst.GMAddMoney, waitGMAddMoney, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMAddMoney(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMAddMoney, ok:{0}", ok);

        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Money = team.Money;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddMoney);
    }

    private int mAddDiamond;

    private void addDiamond()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Add Diamond");
        mAddDiamond = EditorGUILayout.IntField(mAddDiamond, GUILayout.Width(100));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("AddDiamond", mAddDiamond);
            SendHttp.Get.Command(URLConst.GMAddDiamond, waitGMAddDiamond, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMAddDiamond(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMAddDiamond, ok:{0}", ok);

        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Diamond = team.Diamond;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddDiamond);
    }

    private int mAddPowerValue;

    private void addPower()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label("Power");
        mAddPowerValue = EditorGUILayout.IntField(mAddPowerValue, GUILayout.Width(100));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("Value", mAddPowerValue);
            SendHttp.Get.Command(URLConst.GMAddPower, waitGMAddPower, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMAddPower(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMAddPower, ok:{0}", ok);

        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Power = team.Power;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddPower);
    }

    private int mAddExp;

    private void addExp()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Add Exp");
        mAddExp = EditorGUILayout.IntField(mAddExp, GUILayout.Width(100));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("AddExp", mAddExp);
            SendHttp.Get.Command(URLConst.GMAddExp, waitGMAddExp, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private class TAddExp
    {
        [UsedImplicitly]
        public int Exp;
        [UsedImplicitly]
        public int Lv;
    }

    private void waitGMAddExp(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMAddExp, ok:{0}", ok);

        if (ok)
        {
            var team = JsonConvert.DeserializeObject<TAddExp>(www.text);
            GameData.Team.Player.Exp = team.Exp;
            GameData.Team.Player.Lv = team.Lv;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddPower);
    }

    private bool HaveChange()
    {
        for (int i = 0; i < addPotential.Length; i++)
            if (addPotential[i] > 0)
                return true;

        return false;
    }

    private bool CanUsePotential(int index)
    {
        return CrtAvatarPotential + CrtLvPotential >= useLvPotential + useAvatarPotential +
        GameFunction.GetPotentialRule(GameData.Team.Player.BodyType, index);
    }

    private void CalculateAddPotential()
    {
        int count = 0;
        for (int i = 0; i < addPotential.Length; i++)
        {
            count += addPotential[i] * GameFunction.GetPotentialRule(GameData.Team.Player.BodyType, i);
        }

        if (CrtLvPotential >= count)
        {
            useLvPotential = count;
            useAvatarPotential = 0;
        }
        else
        {
            useLvPotential = CrtLvPotential;
            useAvatarPotential = count - CrtLvPotential;
        }
    }

    private void SetPlayeLv()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("等級潛能點 : {0} - {1} / {2}", CrtLvPotential, useLvPotential, LvPotential)); 
        GUILayout.Label(", 設定人物等級 : "); 
        playerlv = EditorGUILayout.IntField(playerlv, GUILayout.Width(100));
		
        if (GUILayout.Button("設定", GUILayout.Width(200)))
        {
            if (playerlv != GameData.Team.Player.Lv)
            {
                WWWForm form = new WWWForm();
                form.AddField("Lv", playerlv);
                SendHttp.Get.Command(URLConst.GMSetLv, waitGMPlayerInfo, form);
            }
            else
                ShowHint("請設定Player Lv");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddAvatarPotential()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("裝備潛能點 : {0} - {1} / {2}", CrtAvatarPotential, useAvatarPotential, AvatarPotential)); 
        GUILayout.Label(", 設定裝備潛能點 : "); 
        avatarPotential = EditorGUILayout.IntField(avatarPotential, GUILayout.Width(100));
		
        if (GUILayout.Button("設定", GUILayout.Width(200)))
        {
            if (avatarPotential > 0)
            {
                WWWForm form = new WWWForm();
                form.AddField("AvatarPotential", avatarPotential);
                SendHttp.Get.Command(URLConst.GMAddAvatarPotential, waitGMAddAvatarPotential, form);
            }
            else
                ShowHint("請設定AvatarPotential");
        }
        EditorGUILayout.EndHorizontal();
    }

    List<int> itemIds2 = new List<int>();
    int[] NumberOfItems2;

    private void AddItemByEveryPart()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("位置： : "); 
        playerPosition = EditorGUILayout.IntField(playerPosition, GUILayout.Width(30));
        GUILayout.Label("(中鋒: 0 、前鋒：1、後衛：２)");
		
        GUILayout.Label("各部位＋ : "); 
        countprekind = EditorGUILayout.IntField(countprekind, GUILayout.Width(30));
        GUILayout.Label("個"); 
		
        if (GUILayout.Button("Add", GUILayout.Width(200)))
        {
            //note : Item data all kind 
            int findCount = 0;
            int currentkind = 0;
            itemIds2.Clear();

            foreach (KeyValuePair<int, TItemData> item in GameData.DItemData)
            {
                if (item.Value.Kind > 0)
                {
                    if (currentkind != item.Value.Kind)
                    {
                        findCount = 0;
                        currentkind = item.Value.Kind;
                    }
                    else
                    {
                        if (findCount < countprekind)
                        {
                            if (item.Value.Kind < 6 && item.Value.Position == playerPosition)
                            {
                                itemIds2.Add(item.Value.ID);
                            }
                            else
                                itemIds2.Add(item.Value.ID);

                            findCount++;
                        }
                    }
                }
            }

            NumberOfItems2 = new int[itemIds2.Count];
            for (int i = 0; i < NumberOfItems2.Length; i++)
                NumberOfItems2[i] = 1;	
						
            if (itemIds2 != null && itemIds2.Count > 0)
                SendGMAddItem(itemIds2, NumberOfItems2);
            else
                ShowHint("請設定Item數量");
        }
		
        EditorGUILayout.EndHorizontal();
    }
	
    //指定部位加Item
    private int limitposition = 0;
    private int limitcountprekind = 1;
    private int limitItemkind = 0;
    private int[] itemIds3;
    private int[] NumberOfItems3;

    private void AddItemByLimitPart()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("位置:"); 
        limitposition = EditorGUILayout.IntField(limitposition, GUILayout.Width(30));
        GUILayout.Label("(中鋒: 0 、前鋒：1、後衛：２)");

        GUILayout.Label("部位:(Kind) "); 
        limitItemkind = EditorGUILayout.IntField(limitItemkind, GUILayout.Width(30));

        if (limitItemkind > 7)
            ShowHint("Error Kind : " + limitItemkind);
		
        GUILayout.Label("各部位＋ : "); 
        limitcountprekind = EditorGUILayout.IntField(limitcountprekind, GUILayout.Width(30));
        GUILayout.Label("個"); 
		
        if (GUILayout.Button("Add", GUILayout.Width(200)))
        {
            int findCount;
//			itemIds3 = new int[limitcountprekind];
            findCount = 0;
            List<int> itemIds3 = new List<int>();
				
            foreach (KeyValuePair<int, TItemData> item in GameData.DItemData)
            {
                if (limitItemkind < 6)
                {
                    if (item.Value.Kind == limitItemkind && item.Value.Position == limitposition)
                    {
                        if (findCount < countprekind)
                        {
                            itemIds3.Add(item.Value.ID);
//							itemIds3[findCount] = item.Value.ID;
                            findCount++;
                        }
                        else
                            continue;
                    }
                }
                else
                {
                    if (item.Value.Kind == limitItemkind)
                    {
                        if (findCount < limitcountprekind)
                        {
                            itemIds3.Add(item.Value.ID);
//							itemIds3[findCount] = item.Value.ID;
                            findCount++;
                        }
                        else
                            continue;
                    }
                }
            }

            NumberOfItems3 = new int[itemIds3.Count];

            for (int i = 0; i < itemIds3.Count; i++)
            {
                NumberOfItems3[i] = 1;
            }

            if (itemIds3 != null && itemIds3.Count > 0)
            {
                SendGMAddItem(itemIds3, NumberOfItems3);
            }
            else
                ShowHint("請設定Item數量");
        }
		
        EditorGUILayout.EndHorizontal();
    }

    private string itemtip = "裝備狀況  ： ";
    private int PlayerItemsSettingOption = 0;
    private string[] PlayerItemsSettingOptionstr = new string[2]{"背包", "裝備中"};

    private void PlayerItemsSetting()
    {
        PlayerItemsSettingOption = GUILayout.Toolbar(PlayerItemsSettingOption, PlayerItemsSettingOptionstr);
        switch (PlayerItemsSettingOption)
        {
            case 0:
                InBackage();
                break;
            case 1:
                InPlayer();
                break;
        }
    }

    private void InPlayer()
    {
        if (GameData.Team.Player.Items != null && GameData.Team.Player.Items.Length > 0)
        {
            for (int i = 0; i < GameData.Team.Player.Items.Length; i++)
            {
                if (i > 0)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (GameData.Team.Player.Items[i].ID > 0)
                    {
                        GUILayout.Label("裝備中, ");

                        if (GameData.Team.Player.Items[i].UseKind == 1)
                            GUILayout.Label(itemtip + "時限內" + ", Time : " + GameData.Team.Player.Items[i].UseTime);
                        else if (GameData.Team.Player.Items[i].UseKind == 2)
                            GUILayout.Label(itemtip + "過期" + ", Time : " + GameData.Team.Player.Items[i].UseTime);
                        else
                            GUILayout.Label(itemtip + "永久");

                        if (GUILayout.Button("過期", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, 2, new DateTime().ToUniversalTime());

                        if (GUILayout.Button("時限內", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, 1, DateTime.Now.AddDays(1).ToUniversalTime());

                        if (GUILayout.Button("買斷", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, -1, new DateTime().ToUniversalTime());
                    }
                    else
                        GUILayout.Label("未裝備, ");                   

                    EditorGUILayout.EndHorizontal(); 
                }
            }
        }	
    }

	private Vector2 scrollPosition = new Vector2(0, 0);

    private void InBackage()
    {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);//  GUILayout.Width(620),  GUILayout.Height(500)); 
		GUILayout.BeginArea(new Rect(0, 0, 600, 600));    //Does not display correctly if this is not commented out!
		
        if (GameData.Team.Items != null && GameData.Team.Items.Length > 0)
        {
            for (int i = 0; i < GameData.Team.Items.Length; i++)
            {
                if (GameData.DItemData.ContainsKey(GameData.Team.Items[i].ID) && 
                    GameData.DItemData[GameData.Team.Items[i].ID].Kind > 0 &&
                    GameData.DItemData[GameData.Team.Items[i].ID].Kind < 8)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (GameData.Team.Items[i].ID > 0)
                    {
                        if (GameData.Team.Items[i].UseKind == 1)
                            GUILayout.Label(itemtip + "時限內" + ", Time : " + GameData.Team.Items[i].UseTime + " Index : " + i.ToString());
                        else if (GameData.Team.Items[i].UseKind == 2)
                            GUILayout.Label(itemtip + "過期" + ", Time : " + GameData.Team.Items[i].UseTime + " Index : " + i.ToString());
                        else
                            GUILayout.Label(itemtip + "永久");

                        if (GUILayout.Button("過期", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, 2, new DateTime().ToUniversalTime());

                        if (GUILayout.Button("時限內", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, 1, DateTime.Now.AddDays(1).ToUniversalTime());

                        if (GUILayout.Button("買斷", GUILayout.Width(50)))
                            SendGMChangeAvatarUseKind(i, -1, new DateTime().ToUniversalTime());
                    }
                    else
                        GUILayout.Label("未裝備, ");                   

                    EditorGUILayout.EndHorizontal(); 
                }
            }
        }

		GUILayout.EndArea();
		GUILayout.EndScrollView();
    }

    private void SendGMChangeAvatarUseKind(int index, int kind, DateTime time)
    {
        WWWForm form = new WWWForm();
        form.AddField("Index", index);
        form.AddField("UseKind", kind);
        string isoJson = JsonConvert.SerializeObject(time, new IsoDateTimeConverter());
        form.AddField("UseTime", isoJson);
        SendHttp.Get.Command(URLConst.GMChangeAvatarUseKind, waitGMChangeAvatarUseKind, form);
    }

    private void waitGMChangeAvatarUseKind(bool ok, WWW www)
    {
        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Player.Items = team.Player.Items;
            GameData.Team.Items = team.Items;
            GameData.Team.PlayerInit();
        }
    }


    private void SendGMAddItem(List<int> addindexs, int[] numberofitems)
    {
        WWWForm form = new WWWForm();
        form.AddField("AddIndexs", JsonConvert.SerializeObject(addindexs));
        form.AddField("AddNumberOfItems", JsonConvert.SerializeObject(numberofitems));
        SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
    }

    private void waitGMAddItem(bool ok, WWW www)
    {
        if (ok)
        {
//			ShowHint("Server Return : " + www.text);

            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Items = team.Items;
            GameData.Team.ValueItems = team.ValueItems;
            GameData.Team.MaterialItems = team.MaterialItems;
            if (team.Items.Length > 0)
                for (int i = 0; i < team.Items.Length; i++)
                    if (GameData.DItemData.ContainsKey(team.Items[i].ID))
                        Debug.Log("item : " + GameData.DItemData[team.Items[i].ID].Name);
            GameData.Team.SkillCards = team.SkillCards;
            GameData.Team.InitSkillCardCount();
            GameData.Team.GotItemCount = team.GotItemCount;

            if (UIAvatarFitted.Visible)
                UIAvatarFitted.Get.UpdateAvatar(true);
            if (UIEquipment.Get.Visible)
                UIEquipment.Get.Show();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddItem);
    }

    private void waitGMPlayerInfo(bool ok, WWW www)
    {
        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            ShowHint("PlayerLv Upgrade " + GameData.Team.Player.Lv + " > " + team.Player.Lv);
            GameData.Team.Player.Lv = team.Player.Lv;
            InitPotentialPoint();

            if (UIMainLobby.Get.IsVisible)
                UIMainLobby.Get.Show();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetLv);
    }

    private void waitGMAddAvatarPotential(bool ok, WWW www)
    {
        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.AvatarPotential = team.AvatarPotential;
            InitPotentialPoint();
        }
    }

    private void waitSaveMasteries(bool ok, WWW www)
    {
        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Player.Potential = team.Player.Potential;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetLv);
    }

    private int mNextMainStageID = GameConst.Default_MainStageID;

    private void StageHandle()
    {
        setNextMainStageID();
        resetStageChallengeNums();
        resetInstanceIDs();
        setNextInstanceID();
    }

    private void setNextMainStageID()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("NextMainStageID: ");
        mNextMainStageID = EditorGUILayout.IntField(mNextMainStageID, GUILayout.Width(100));
        if (GUILayout.Button("設定", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("NextMainStageID", mNextMainStageID);
            SendHttp.Get.Command(URLConst.GMSetNextMainStageID, waitGMSetNextMainStageID, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMSetNextMainStageID(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMSetNextMainStageID, ok:{0}", ok);

        if (ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Player.NextMainStageID = team.Player.NextMainStageID;
            updateUIMainStage();
            updateUIInstance();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetNextMainStageID);
    }

    private void resetStageChallengeNums()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("每日關卡限制: ");
        if (GUILayout.Button("重置", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            SendHttp.Get.Command(URLConst.GMResetStage, waitGMResetStage, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMResetStage(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMResetStage, ok:{0}", ok);

        if (ok)
        {
            GameData.Team.Player.StageDailyChallengeNums.Clear();
            updateUIMainStage();
            updateUIInstance();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetNextMainStageID);
    }

    private void resetInstanceIDs()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("副本進度: ");
        if (GUILayout.Button("重置", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            SendHttp.Get.Command(URLConst.GMResetNextInstanceIDs, waitGMResetNextInstanceIDs, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMResetNextInstanceIDs(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMResetNextInstanceIDs, ok:{0}", ok);

        if (ok)
        {
            if (GameData.Team.Player.NextInstanceIDs != null)
                GameData.Team.Player.NextInstanceIDs.Clear();
            updateUIInstance();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMResetNextInstanceIDs);
    }

    private int mNextInstanceChapter = 1;
    private int mNextInstanceID = 2111;

    private void setNextInstanceID()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Chapter: ");
        mNextInstanceChapter = EditorGUILayout.IntField(mNextInstanceChapter, GUILayout.Width(60));
        GUILayout.Label("NextInstanceID: ");
        mNextInstanceID = EditorGUILayout.IntField(mNextInstanceID, GUILayout.Width(60));
        if (GUILayout.Button("設定", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("Chapter", mNextInstanceChapter);
            form.AddField("InstanceID", mNextInstanceID);
            SendHttp.Get.Command(URLConst.GMSetNextInstanceID, waitSetNextInstanceIDs, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitSetNextInstanceIDs(bool ok, WWW www)
    {
        Debug.LogFormat("waitSetNextInstanceIDs, ok:{0}", ok);

        if (ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player.NextInstanceIDs = team.Player.NextInstanceIDs;
            updateUIInstance(mNextInstanceChapter);
        }
    }

    private void updateUIMainStage()
    {
        if (UIMainStage.Get.Visible)
        {
            UIMainStage.Get.Hide();
            UIMainStage.Get.Show();
        }
    }

    private void updateUIInstance(int chapter = 1)
    {
        if (UIInstance.Get.Visible)
        {
            UIInstance.Get.Hide();
            UIInstance.Get.ShowByChapter(chapter);
        }
    }

    private void resetDailyLoginNums()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("每月登入次數: ");
        if (GUILayout.Button("重置", GUILayout.Width(50)))
        {
            GMResetDailyLoginNumProtocol protocol = new GMResetDailyLoginNumProtocol();
            protocol.Send(waitGMResetDailyLoginNums);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMResetDailyLoginNums(bool ok)
    {
        Debug.LogFormat("waitGMResetDailyLoginNums, ok:{0}", ok);

        if (UIDailyLogin.Get.Visible)
            UIDailyLogin.Get.Show(UIDailyLogin.Get.Year, UIDailyLogin.Get.Month);
    }

    private void resetReceivedDailyLoginNums()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("每月登入獎勵: ");
        if (GUILayout.Button("重置", GUILayout.Width(50)))
        {
            var protocol = new GMResetReceivedDailyLoginNumProtocol();
            protocol.Send(waitGMResetReceivedDailyLoginNums);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMResetReceivedDailyLoginNums(bool ok)
    {
        Debug.LogFormat("waitGMResetReceivedDailyLoginNums, ok:{0}", ok);

        if(UIDailyLogin.Get.Visible)
            UIDailyLogin.Get.Show(UIDailyLogin.Get.Year, UIDailyLogin.Get.Month);
    }

    private int mDailyLoginYear = DateTime.Now.Year;
    private int mDailyLoginMonth = DateTime.Now.Month;
    private int mDailyLoginLoginNum;

    private void setDailyLoginNums()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("每月登入次數: ");
        GUILayout.Label("年:");
        mDailyLoginYear = EditorGUILayout.IntField(mDailyLoginYear);
        GUILayout.Label("月:");
        mDailyLoginMonth = EditorGUILayout.IntField(mDailyLoginMonth);
        GUILayout.Label("次數:");
        mDailyLoginLoginNum = EditorGUILayout.IntField(mDailyLoginLoginNum);
        if (GUILayout.Button("設定", GUILayout.Width(50)))
        {
            var protocol = new GMSetDailyLoginNumProtocol();
            protocol.Send(mDailyLoginYear, mDailyLoginMonth, mDailyLoginLoginNum, waitGMSetDailyLoginNum);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMSetDailyLoginNum(bool ok)
    {
        if (UIDailyLogin.Get.Visible)
            UIDailyLogin.Get.Show(UIDailyLogin.Get.Year, UIDailyLogin.Get.Month);
    }

    private int mLifetimeLoginNum;

    private void setLifetimeLoginNum()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("終生登入次數: ");
        mLifetimeLoginNum = EditorGUILayout.IntField(mLifetimeLoginNum, GUILayout.Width(60));
        mLifetimeLoginNum = Math.Max(0, mLifetimeLoginNum);
        if(GUILayout.Button("設定", GUILayout.Width(50)))
        {
            var protocol = new GMSetLifetimeLoginNumProtocol();
            protocol.Send(mLifetimeLoginNum, waitGMSetLifeTimeLoginNum);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMSetLifeTimeLoginNum(bool ok)
    {
        Debug.LogFormat("waitGMSetLifeTimeLoginNum, ok:{0}", ok);

        if (UIDailyLogin.Get.Visible)
            UIDailyLogin.Get.Show(UIDailyLogin.Get.Year, UIDailyLogin.Get.Month);
    }

    private void clearLifetimeReceivedLoginNum()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("終生登入的領獎記錄: ");
        if(GUILayout.Button("重置", GUILayout.Width(50)))
        {
            var protocol = new GMResetLifetimeReceivedLoginNumProtocol();
            protocol.Send(waitGMClearLifetimeReceivedLoginNum);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMClearLifetimeReceivedLoginNum(bool ok)
    {
        Debug.LogFormat("waitGMClearLifetimeReceivedLoginNum, ok:{0}", ok);

        if(UIDailyLogin.Get.Visible)
            UIDailyLogin.Get.Show(UIDailyLogin.Get.Year, UIDailyLogin.Get.Month);
    }

    private void deleteAllPlayerPrefabs()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Delete All Player Prefabs");
        if (GUILayout.Button("Delete All"))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Delete All PlayerPrefabs");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void BattleHandle()
    {
        if (GameController.Visible && GameController.Get.IsStart)
        {

            if (GUILayout.Button("Self Victory", GUILayout.Width(150)))
            {
                GameController.Get.GMGameResult(true);
            }
			
            if (GUILayout.Button("Self Defeat", GUILayout.Width(150)))
            {
                GameController.Get.GMGameResult(false);
            }
        }
    }

    private int sceneNo = 0;

    private void SceneHandle()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("場景編號: ");
        sceneNo = EditorGUILayout.IntField(sceneNo, GUILayout.Width(30));
        if (GUILayout.Button("切換", GUILayout.Width(50)))
        {
            SceneMgr.Get.ChangeLevel(sceneNo);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }

	private void UIHandle()
	{
		if (UIPlayerPotential.Visible) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("每次加: " + UIPlayerPotential.Get.AddLevel);
			if (GUILayout.Button("切換", GUILayout.Width(50)))
			{
				UIPlayerPotential.Get.OnChangeLvel ();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
