using System.Collections;
using System.Collections.Generic;
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
    public enum EEquip
    {
        Body,
        Hair,
        Cloth,
        Pants,
        Shoes
    }

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
    private readonly Dictionary<EEquip, int> mEquips = new Dictionary<EEquip, int>();

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

        TItemData[] items = findItems(CreateRoleDataMgr.Ins.GetHairs(pos));
        mEquips.Add(EEquip.Hair, items[0].ID);
        HairBtn.SetData(items);

        items = findItems(CreateRoleDataMgr.Ins.GetCloths(pos));
        mEquips.Add(EEquip.Cloth, items[0].ID);
        ClothBtn.SetData(items);

        items = findItems(CreateRoleDataMgr.Ins.GetPants(pos));
        mEquips.Add(EEquip.Pants, items[0].ID);
        PantsBtn.SetData(items);

        items = findItems(CreateRoleDataMgr.Ins.GetShoes(pos));
        mEquips.Add(EEquip.Shoes, items[0].ID);
        ShoesBtn.SetData(items);

        items = findItems(CreateRoleDataMgr.Ins.GetBody(pos));
        mEquips.Add(EEquip.Body, items[0].ID);
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

    private void onPartSelected(EEquip equip)
    {
        UI3DCreateRole.Get.StyleView.SetCamera(equip);
    }

    private void onPartItemSelected(EEquip equip, int index, int itemID)
    {
//        Debug.LogFormat("Equip:{0}, Index:{1}, ItemID:{2}", equip, index, itemID);

        UI3DCreateRole.Get.StyleView.UpdateModel(equip, itemID);
        mEquips[equip] = itemID;
        UI3DCreateRole.Get.StyleView.PlayIdleAnimation();
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

    private IEnumerator playExitAnimation(CommonDelegateMethods.Action action)
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
        equipmentItemIDs[0] = mEquips[EEquip.Body];
        equipmentItemIDs[1] = mEquips[EEquip.Hair];
        equipmentItemIDs[3] = mEquips[EEquip.Cloth];
        equipmentItemIDs[4] = mEquips[EEquip.Pants];
        equipmentItemIDs[5] = mEquips[EEquip.Shoes];

        GameData.Team.Player.ID = mPlayerID;
        GameData.Team.Player.Items = new TItem[equipmentItemIDs.Length];
        for (int i = 0; i < equipmentItemIDs.Length; i++)
            GameData.Team.Player.Items[i].ID = equipmentItemIDs[i];
        GameData.Team.Player.Init();

        WWWForm form = new WWWForm();
        form.AddField("PlayerID", mPlayerID);
//        form.AddField("Name", GameData.Team.Player.Name);
        form.AddField("Name", SystemInfo.deviceUniqueIdentifier);
        form.AddField("Items", JsonConvert.SerializeObject(equipmentItemIDs));

        SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
    }

    private void waitCreateRole(bool ok, WWW www)
    {
        UICreateRole.Get.Hide();
        UI3DCreateRole.Get.Hide();

        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.SkillCards = team.SkillCards;
            GameData.Team.Player.Init();
            GameData.SaveTeam();
        }
        else
            Debug.LogError("Create Role fail!");

        if(SceneMgr.Get.CurrentScene != ESceneName.Lobby)
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
        else
            LobbyStart.Get.EnterLobby();
    }
}