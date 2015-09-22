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
    public Transform ModelPreview;
    public UICreateRoleStyleViewGroup HairGroup;
    public UICreateRoleStyleViewGroup ClothGroup;
    public UICreateRoleStyleViewGroup PantsGroup;
    public UICreateRoleStyleViewGroup ShoesGroup;
    public UICreateRoleStyleViewGroup BodyGroup;

    public Animator UIAnimator;

    private GameObject mModel;
    private int mPlayerID;

    private EEquip mCurrentEquip = EEquip.Hair;

    private readonly Dictionary<EEquip, UICreateRoleStyleViewGroup> mGroups = new Dictionary<EEquip, UICreateRoleStyleViewGroup>();
    private readonly Dictionary<EEquip, TItemData> mEquips = new Dictionary<EEquip, TItemData>();

    /// <summary>
    /// 撥頁面隱藏時的時間, 單位: 秒.
    /// </summary>
    private const float HideAnimationTime = 1;

    [UsedImplicitly]
    private void Awake()
    {
        mGroups.Add(EEquip.Hair, HairGroup);
        mGroups.Add(EEquip.Cloth, ClothGroup);
        mGroups.Add(EEquip.Pants, PantsGroup);
        mGroups.Add(EEquip.Shoes, ShoesGroup);
        mGroups.Add(EEquip.Body, BodyGroup);

//        HairGroup.OnTitleClickListener += onGroupTitleClick;
//        HairGroup.OnEquipClickListener += onEquipClick;
//
//        ClothGroup.OnTitleClickListener += onGroupTitleClick;
//        ClothGroup.OnEquipClickListener += onEquipClick;
//
//        PantsGroup.OnTitleClickListener += onGroupTitleClick;
//        PantsGroup.OnEquipClickListener += onEquipClick;
//
//        ShoesGroup.OnTitleClickListener += onGroupTitleClick;
//        ShoesGroup.OnEquipClickListener += onEquipClick;
//
//        BodyGroup.OnTitleClickListener += onGroupTitleClick;
//        BodyGroup.OnEquipClickListener += onEquipClick;
    }

    public void Show(EPlayerPostion pos, int playerID)
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.ShowStyleView(pos, playerID);

        initData(pos, playerID);
    }

    private void initData(EPlayerPostion pos, int playerID)
    {
//        mPlayerID = UI3DCreateRole.Get.GetPlayerID(pos);
        mPlayerID = playerID;

        mEquips.Clear();

        TItemData[] items = findItems(CreateRoleDataMgr.Ins.GetHairs(pos));
        mEquips.Add(EEquip.Hair, items[0]);
//        HairGroup.Init(EEquip.Hair, items);
//        HairGroup.Play();
//        HairGroup.SetSelected();

        items = findItems(CreateRoleDataMgr.Ins.GetCloths(pos));
        mEquips.Add(EEquip.Cloth, items[0]);
//        ClothGroup.Init(EEquip.Cloth, items);
//        ClothGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetPants(pos));
        mEquips.Add(EEquip.Pants, items[0]);
//        PantsGroup.Init(EEquip.Pants, items);
//        PantsGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetShoes(pos));
        mEquips.Add(EEquip.Shoes, items[0]);
//        ShoesGroup.Init(EEquip.Shoes, items);
//        ShoesGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetBody(pos));
        mEquips.Add(EEquip.Body, items[0]);
//        BodyGroup.Init(EEquip.Body, items);
//        BodyGroup.Hide();

        mCurrentEquip = EEquip.Hair;
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

    private void onGroupTitleClick(EEquip equip)
    {
//        Debug.LogFormat("onGroupTitleClick, equip:{0}", equip);

        if(mCurrentEquip == equip)
            return;

        mCurrentEquip = equip;

        foreach(KeyValuePair<EEquip, UICreateRoleStyleViewGroup> pair in mGroups)
        {
            if(pair.Key == mCurrentEquip)
                pair.Value.Play();
            else
                pair.Value.Hide();
        }
    }

    private void onEquipClick(EEquip equip, TItemData item)
    {
//        Debug.LogFormat("onEquipClick, equip:{0}, item:{1}", equip, item);

        if(mEquips.ContainsKey(equip))
            mEquips[equip] = item;
        else
            mEquips.Add(equip, item);

//        updateModel();
    }

//    private void updateModel()
//    {
//        if (mEquips.Count < 5)
//            return;
//
//        if (mModel)
//            Destroy(mModel);
//    
//        mModel = UICreateRole.CreateModel(ModelPreview, "StyleViewModel", mPlayerID,
//            mEquips[EEquip.Body].ID,
//            mEquips[EEquip.Hair].ID,
//            mEquips[EEquip.Cloth].ID,
//            mEquips[EEquip.Pants].ID,
//            mEquips[EEquip.Shoes].ID);
//    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnBackClick()
    {
        StartCoroutine(playHideAnimation(showPreviousPage));
    }

    private void showPreviousPage()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    private IEnumerator playHideAnimation(CommonDelegateMethods.Action action)
    {
        UIAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(HideAnimationTime);

        action();
    }

    public void OnNextClick()
    {
        StartCoroutine(playHideAnimation(showNextPage));
    }

    private void showNextPage()
    {
        int[] equipmentItemIDs = new int[8];
        equipmentItemIDs[0] = mEquips[EEquip.Body].ID;
        equipmentItemIDs[1] = mEquips[EEquip.Hair].ID;
        equipmentItemIDs[3] = mEquips[EEquip.Cloth].ID;
        equipmentItemIDs[4] = mEquips[EEquip.Pants].ID;
        equipmentItemIDs[5] = mEquips[EEquip.Shoes].ID; ;

        GameData.Team.Player.ID = mPlayerID;
        GameData.Team.Player.Items = new GameStruct.TItem[equipmentItemIDs.Length];
        for (int i = 0; i < equipmentItemIDs.Length; i++)
            GameData.Team.Player.Items[i].ID = equipmentItemIDs[i];
        GameData.Team.Player.Init();

        WWWForm form = new WWWForm();
        form.AddField("PlayerID", mPlayerID);
        form.AddField("Name", GameData.Team.Player.Name);
        form.AddField("Items", JsonConvert.SerializeObject(equipmentItemIDs));

        SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
    }

    private void waitCreateRole(bool ok, WWW www)
    {
		if(ok)
        {
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text); 
			GameData.Team.Player = team.Player;
			GameData.Team.SkillCards = team.SkillCards;
			GameData.Team.Player.Init();
			GameData.SaveTeam();
			UICreateRole.Get.Hide();
			
			if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
				SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			else
				LobbyStart.Get.EnterLobby();
		}
        else
		    Debug.LogError("Create Role fail!");
	}
}