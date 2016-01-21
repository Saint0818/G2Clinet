using UnityEngine;

/// <summary>
/// 副本章節內的關卡. 這對應到 UIInstanceStage.prefab.
/// </summary>
public class UIInstanceStage : MonoBehaviour
{
    public UILabel Title;

    public class Data
    {
        public int ID;
        public string Title;
    }

    private UIStageHint mHint;
    private void Awake()
    {
        mHint = GetComponent<UIStageHint>();
    }

    public void SetData(Data data)
    {
        mHint.UpdateUI(data.ID);

        Title.text = data.Title;
    }
}