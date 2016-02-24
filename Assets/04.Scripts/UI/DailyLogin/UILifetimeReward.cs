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
        public TItemData[] ItemData;
    }

    public UILabel LoginNumLabel;

    public void Set(Data data)
    {
        LoginNumLabel.text = data.LoginNum;
    }
}