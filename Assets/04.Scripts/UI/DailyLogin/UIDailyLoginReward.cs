using GameStruct;
using UnityEngine;

public class UIDailyLoginReward : MonoBehaviour
{
    public class Data
    {
        public string Day;
        public string Name;
//        public int Num;
        public bool ShowClear;
        public TItemData ItemData;
    }

    public UILabel DayLabel;
    public UILabel NameLabel;
//    public UILabel NumLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;

    public void Set(Data data)
    {
        DayLabel.text = data.Day;
        NameLabel.text = data.Name;
//        NumLabel.text = string.Format("{0}", data.Num);
        Clear.SetActive(data.ShowClear);
        ItemAward.Show(data.ItemData);
    }
}
