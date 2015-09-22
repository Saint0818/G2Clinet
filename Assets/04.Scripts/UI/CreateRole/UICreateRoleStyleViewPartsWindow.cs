using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public class UICreateRoleStyleViewPartsWindow : MonoBehaviour
{
    public UIScrollView ScrollView;

    [UsedImplicitly]
	private void Start()
    {
	
	}

    public void UpdateData(TItemData[] data)
    {
        foreach(TItemData item in data)
        {
            Debug.LogFormat("Item:{0}", item);
        }
    }
}
