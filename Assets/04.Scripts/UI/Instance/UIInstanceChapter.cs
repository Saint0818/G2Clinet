using UnityEngine;

/// <summary>
/// 副本章節. 這對應到 UIInstanceChapter.prefab.
/// </summary>
public class UIInstanceChapter : MonoBehaviour
{
    public UILabel Title;
    public UILabel Desc;
    public UILabel BossName;
    public GameObject[] FocusObjects;
    public Transform ModelParent;

    public class Data
    {
        public string Title;
        public string Desc;
        public string BossName;

        public GameObject Model;

        public UIInstanceStage.Data[] NormalStages;
        public UIInstanceStage.Data BossStage;
    }

    public bool FocusVisible
    {
        set
        {
            foreach(GameObject obj in FocusObjects)
            {
                obj.SetActive(value);
            }
        }
    }

    private Data mData;

    private void Awake()
    {
        GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClick));
    }

    public void SetData(Data data)
    {
        mData = data;

        Title.text = data.Title;
        Desc.text = data.Desc;
        BossName.text = data.BossName;

        data.Model.transform.parent = ModelParent;
        data.Model.transform.localPosition = Vector3.zero;
        data.Model.transform.localRotation = Quaternion.identity;
        data.Model.transform.localScale = Vector3.one;
    }

    private void OnClick()
    {
        UIInstance.Get.Main.ShowStages(mData.NormalStages, mData.BossStage);
    }
}