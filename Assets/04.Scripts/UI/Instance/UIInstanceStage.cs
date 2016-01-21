using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 副本章節內的關卡. 這對應到 UIInstanceStage.prefab.
/// </summary>
public class UIInstanceStage : MonoBehaviour
{
    public UILabel TitleLabel;
    public UILabel MoneyLabel;
    public UILabel ExpLabel;

    public class Data
    {
        public int ID;
        public string Title;
        public int Money;
        public int Exp;
    }

    private UIStageHint mHint;
    private void Awake()
    {
        mHint = GetComponent<UIStageHint>();
    }

    public void SetData(Data data)
    {
        mHint.UpdateUI(data.ID);

        TitleLabel.text = data.Title;
        MoneyLabel.text = data.Money.ToString();
        ExpLabel.text = data.Exp.ToString();
    }
}