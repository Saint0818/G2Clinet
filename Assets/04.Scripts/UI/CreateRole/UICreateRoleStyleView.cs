using System;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 創角介面中的重要頁面, 設定角色外觀. 可以更換頭髮, 衣服, 鞋子, 褲子, 身體.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRoleStyleViewPartsWindow PartsWindow;
    public UICreateRolePartButton BodyBtn;
    public UICreateRolePartButton HairBtn;
    public UICreateRolePartButton ClothBtn;
    public UICreateRolePartButton PantsBtn;
    public UICreateRolePartButton ShoesBtn;

    public Animator UIAnimator;

    private int mPlayerID;

    /// <summary>
    /// 目前球員穿搭的配件. Value: ItemID.
    /// </summary>
    private readonly Dictionary<UICreateRole.EEquip, int> mEquips = new Dictionary<UICreateRole.EEquip, int>();

    /// <summary>
    /// 撥頁面隱藏時的時間, 單位: 秒.
    /// </summary>
    private const float HideAnimationTime = 1;

    [UsedImplicitly]
    private void Awake()
    {
        PartsWindow.SelectListener += BodyBtn.OnPartItemSelected;
        PartsWindow.SelectListener += HairBtn.OnPartItemSelected;
        PartsWindow.SelectListener += ClothBtn.OnPartItemSelected;
        PartsWindow.SelectListener += PantsBtn.OnPartItemSelected;
        PartsWindow.SelectListener += ShoesBtn.OnPartItemSelected;
        PartsWindow.SelectListener += onPartItemSelected;

        BodyBtn.SelectedListener += onPartSelected;
        HairBtn.SelectedListener += onPartSelected;
        ClothBtn.SelectedListener += onPartSelected;
        PantsBtn.SelectedListener += onPartSelected;
        ShoesBtn.SelectedListener += onPartSelected;
    }

    public void Show(EPlayerPostion pos, int playerID)
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.ShowStyleView(pos, playerID);

        initData(pos, playerID);
    }

    private void initData(EPlayerPostion pos, int playerID)
    {
        mPlayerID = playerID;

        mEquips.Clear();

        TItemData[] items = findItems(CreateRoleTable.Ins.GetHairs(pos));
        mEquips.Add(UICreateRole.EEquip.Hair, items[0].ID);
        HairBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetCloths(pos));
        mEquips.Add(UICreateRole.EEquip.Cloth, items[0].ID);
        ClothBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetPants(pos));
        mEquips.Add(UICreateRole.EEquip.Pants, items[0].ID);
        PantsBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetShoes(pos));
        mEquips.Add(UICreateRole.EEquip.Shoes, items[0].ID);
        ShoesBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetBody(pos));
        mEquips.Add(UICreateRole.EEquip.Body, items[0].ID);
        BodyBtn.SetData(items);
        BodyBtn.SetSelected(); // 強制每次進入時, 都是 Body 按鈕被選擇.
    }

    private TItemData[] findItems(int[] itemIDs)
    {
        List<TItemData> data = new List<TItemData>();

        foreach(var itemID in itemIDs)
        {
            if(GameData.DItemData.ContainsKey(itemID))
                data.Add(GameData.DItemData[itemID]);
        }

        return data.ToArray();
    }

    private void onPartSelected(UICreateRole.EEquip equip)
    {
        UI3DCreateRole.Get.StyleView.SetCamera(equip);
    }

    private void onPartItemSelected(UICreateRole.EEquip equip, int index, int itemID)
    {
//        Debug.LogFormat("Equip:{0}, Index:{1}, ItemID:{2}", equip, index, itemID);

        UI3DCreateRole.Get.StyleView.UpdateModel(equip, itemID);
        mEquips[equip] = itemID;
//        UI3DCreateRole.Get.StyleView.PlayIdleAnimation();
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnBackClick()
    {
        StartCoroutine(playExitAnimation(showPreviousPage));
    }

    private void showPreviousPage()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    private IEnumerator playExitAnimation(Action action)
    {
        UICreateRole.Get.EnableBlock(true);
        UIAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(HideAnimationTime);
        UICreateRole.Get.EnableBlock(false);

        action();
    }

    /// <summary>
    /// 呼叫時機: 右下角的 Next 按鈕按下.
    /// </summary>
    public void OnNextClick()
    {
        StartCoroutine(playExitAnimation(sendDataToServer));
    }

    private void sendDataToServer()
    {
        int[] equipmentItemIDs = new int[8];
        equipmentItemIDs[0] = mEquips[UICreateRole.EEquip.Body]; // 陣列的魔術數字其實是對應到 TItemData.Kind.
        equipmentItemIDs[1] = mEquips[UICreateRole.EEquip.Hair];
        equipmentItemIDs[3] = mEquips[UICreateRole.EEquip.Cloth];
        equipmentItemIDs[4] = mEquips[UICreateRole.EEquip.Pants];
        equipmentItemIDs[5] = mEquips[UICreateRole.EEquip.Shoes];

        GameData.Team.Player.ID = mPlayerID;
        GameData.Team.Player.Items = new TItem[equipmentItemIDs.Length];
        for (int i = 0; i < equipmentItemIDs.Length; i++)
            GameData.Team.Player.Items[i].ID = equipmentItemIDs[i];
        GameData.Team.Player.Init();

        WWWForm form = new WWWForm();
        form.AddField("PlayerID", mPlayerID);
        form.AddField("Name",GameData.Team.Identifier);
        form.AddField("Items", JsonConvert.SerializeObject(equipmentItemIDs));

		if(SceneMgr.Get.CurrentScene != "Lobby")
			UILoading.UIShow(true, GameEnum.ELoading.Login);

        SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form);
    }

    private void waitCreateRole(bool ok, WWW www)
    {
        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
//            GameData.Team.Money = team.Money;
//            GameData.Team.Diamond = team.Diamond;
            GameData.Team.Player = team.Player;
            GameData.Team.SkillCards = team.SkillCards;
			GameData.Team.Items = team.Items;
			GameData.Team.Player.Init();
			GameData.Team.InitSkillCardCount();
//            GameData.SaveTeam();

			UICreateRole.Get.Hide();
			UI3DCreateRole.Get.Hide();

			if(SceneMgr.Get.CurrentScene != ESceneName.Lobby)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			else
				LobbyStart.Get.EnterLobby();
        }
        else
			UIHint.Get.ShowHint("Create Role fail!", Color.red);
    }
}