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

    public UIButton RandomStyleButton;

    public Animator UIAnimator;

    private int mPlayerID;

    /// <summary>
    /// 目前球員穿搭的配件. Value: ItemID.
    /// </summary>
    private readonly Dictionary<UICreateRole.EPart, int> mCurrentPlayerParts = new Dictionary<UICreateRole.EPart, int>();

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

        RandomStyleButton.onClick.Add(new EventDelegate(onRandomStyle));
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

        mCurrentPlayerParts.Clear();

        TItemData[] items = findItems(CreateRoleTable.Ins.GetHairs(pos));
        mCurrentPlayerParts.Add(UICreateRole.EPart.Hair, items[0].ID);
        HairBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetCloths(pos));
        mCurrentPlayerParts.Add(UICreateRole.EPart.Cloth, items[0].ID);
        ClothBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetPants(pos));
        mCurrentPlayerParts.Add(UICreateRole.EPart.Pants, items[0].ID);
        PantsBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetShoes(pos));
        mCurrentPlayerParts.Add(UICreateRole.EPart.Shoes, items[0].ID);
        ShoesBtn.SetData(items);

        items = findItems(CreateRoleTable.Ins.GetBody(pos));
        mCurrentPlayerParts.Add(UICreateRole.EPart.Body, items[0].ID);
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

    private void onPartSelected(UICreateRole.EPart part)
    {
        UI3DCreateRole.Get.StyleView.SetCamera(part);
    }

    private void onPartItemSelected(UICreateRole.EPart part, int index, int itemID)
    {
        Debug.LogFormat("Part:{0}, Index:{1}, ItemID:{2}", part, index, itemID);

        UI3DCreateRole.Get.StyleView.UpdateModel(part, itemID);
        mCurrentPlayerParts[part] = itemID;
    }

    private void onRandomStyle()
    {
//        Debug.Log("onRandomStyle");

        UICreateRole.EPart[] parts = new UICreateRole.EPart[5];
        int[] itemIDs = new int[5];

        parts[0] = UICreateRole.EPart.Body;
        itemIDs[0] = BodyBtn.RandomItemData().ID;
        mCurrentPlayerParts[UICreateRole.EPart.Body] = itemIDs[0];

        parts[1] = UICreateRole.EPart.Hair;
        itemIDs[1] = HairBtn.RandomItemData().ID;
        mCurrentPlayerParts[UICreateRole.EPart.Hair] = itemIDs[1];

        parts[2] = UICreateRole.EPart.Cloth;
        itemIDs[2] = ClothBtn.RandomItemData().ID;
        mCurrentPlayerParts[UICreateRole.EPart.Cloth] = itemIDs[2];

        parts[3] = UICreateRole.EPart.Pants;
        itemIDs[3] = PantsBtn.RandomItemData().ID;
        mCurrentPlayerParts[UICreateRole.EPart.Pants] = itemIDs[3];

        parts[4] = UICreateRole.EPart.Shoes;
        itemIDs[4] = ShoesBtn.RandomItemData().ID;
        mCurrentPlayerParts[UICreateRole.EPart.Shoes] = itemIDs[4];

        UI3DCreateRole.Get.StyleView.UpdateModels(parts, itemIDs);
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
        int[] partItemIDs = new int[8];
        partItemIDs[0] = mCurrentPlayerParts[UICreateRole.EPart.Body]; // 陣列的魔術數字其實是對應到 TItemData.Kind.
        partItemIDs[1] = mCurrentPlayerParts[UICreateRole.EPart.Hair];
        partItemIDs[3] = mCurrentPlayerParts[UICreateRole.EPart.Cloth];
        partItemIDs[4] = mCurrentPlayerParts[UICreateRole.EPart.Pants];
        partItemIDs[5] = mCurrentPlayerParts[UICreateRole.EPart.Shoes];

        GameData.Team.Player.ID = mPlayerID;
        GameData.Team.Player.Items = new TItem[partItemIDs.Length];
        for (int i = 0; i < partItemIDs.Length; i++)
            GameData.Team.Player.Items[i].ID = partItemIDs[i];
        GameData.Team.PlayerInit();

        WWWForm form = new WWWForm();
        form.AddField("PlayerID", mPlayerID);
//        form.AddField("Name", GameData.Team.Identifier);
        form.AddField("AvatarItems", JsonConvert.SerializeObject(partItemIDs));

		if(SceneMgr.Get.CurrentScene != "Lobby")
			UILoading.UIShow(true, ELoading.Login);

        SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form);
    }

    private void waitCreateRole(bool ok, WWW www)
    {
        if(ok)
        {
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Player = team.Player;
            GameData.Team.Items = team.Items;
            GameData.Team.PlayerInit();

            if (team.PlayerBank != null) 
                GameData.Team.PlayerBank = team.PlayerBank;  

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