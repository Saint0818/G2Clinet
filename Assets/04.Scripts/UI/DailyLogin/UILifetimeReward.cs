using GameStruct;
using UnityEngine;

/// <summary>
/// 終生獎勵.
/// </summary>
public class UILifetimeReward : MonoBehaviour
{
    public class Data
    {
        public int CurrentLoginNum; // 目前登入次數.(分子)
        public int TargetLoginNum; // 目標登入次數.(分母)

        public TItemData[] Items;
        public UIDailyLoginMain.EStatus Status;
    }

    private readonly Color32 mReceivableColor = new Color32(255, 255, 180, 255);
    private readonly Color32 mNoReceivableColor = new Color32(255, 255, 255, 255);

    public UILabel LoginNumLabel;
    public UILabel ReceivableLabel;
    public ItemAwardGroup[] ItemAwards;
    public GameObject ReceivedObj;
    public UIButton ReceiveButton;
    public UISprite BGSprite;

    private Data mData;

    private void Start()
    {
        ReceiveButton.onClick.Add(new EventDelegate(() =>
        {
            if(mData.Status == UIDailyLoginMain.EStatus.Receivable)
            {
                var main = GetComponentInParent<UILifetimeLoginMain>();
                main.FireReceiveClick(mData.TargetLoginNum);
            }
        }));
    }

    public void Set(Data data)
    {
        mData = data;

        LoginNumLabel.text = string.Format("{0}/{1}", mData.CurrentLoginNum, mData.TargetLoginNum);

        updateStatus(data.Status);
        updateItems(data.Items);
    }

    private void updateStatus(UIDailyLoginMain.EStatus status)
    {
        ReceivableLabel.gameObject.SetActive(status == UIDailyLoginMain.EStatus.Receivable);
        ReceivedObj.SetActive(status == UIDailyLoginMain.EStatus.Received);

        BGSprite.color = status == UIDailyLoginMain.EStatus.Receivable ? mReceivableColor : mNoReceivableColor;
    }

    private void updateItems(TItemData[] itemData)
    {
        for(var i = 0; i < ItemAwards.Length; i++)
        {
            if(i < itemData.Length)
            {
                ItemAwards[i].Show(itemData[i]);
                ItemAwards[i].gameObject.SetActive(true);
            }
            else
                ItemAwards[i].gameObject.SetActive(false);
        }
    }
}