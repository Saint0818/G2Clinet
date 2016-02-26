using UnityEngine;

public class OpenMallAction : UIItemSourceElement.IAction
{
    public void Do()
    {
		UIMall.Get.ShowView();
    }
}