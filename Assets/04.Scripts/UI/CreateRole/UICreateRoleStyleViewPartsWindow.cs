using JetBrains.Annotations;
using UnityEngine;

public class UICreateRoleStyleViewPartsWindow : MonoBehaviour
{
    [UsedImplicitly]
	private void Start()
    {
	
	}

    public void UpdateData(UICreateRoleStyleView.EEquip equip)
    {
        Debug.LogFormat("Equip:{0}", equip);
    }
}
