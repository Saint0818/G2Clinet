using GameStruct;
using UnityEngine;

/// <summary>
/// 終生獎勵.
/// </summary>
public class UILifetimeReward : MonoBehaviour
{
    public class Data
    {
        public string LoginNum;
        public TItemData[] Items;
        public UIDailyLoginMain.EStatus Status;
    }

    public UILabel LoginNumLabel;
    public UILabel ReceivableLabel;
    public ItemAwardGroup[] ItemAwards;
    public GameObject ReceivedObj;
    public UIButton ReceiveButton;

    private Data mData;

    private void Start()
    {
        ReceiveButton.onClick.Add(new EventDelegate(() =>
        {
            if(mData.Status == UIDailyLoginMain.EStatus.Receivable)
            {
                var main = GetComponentInParent<UILifetimeLoginMain>();
                main.FireReceiveClick();
            }
        }));
    }

    public void Set(Data data)
    {
        mData = data;

        LoginNumLabel.text = data.LoginNum;

        updateStatus(data.Status);
        updateItems(data.Items);
    }

    private void updateStatus(UIDailyLoginMain.EStatus status)
    {
        ReceivableLabel.gameObject.SetActive(status == UIDailyLoginMain.EStatus.Receivable);
        ReceivedObj.SetActive(status == UIDailyLoginMain.EStatus.Received);
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