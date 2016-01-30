using UnityEngine;

public class OpenShopGeneralAction : UIItemSourceElement.IAction
{
    public void Do()
    {
//        Debug.Log("Open Shop General.");

        UIShop.Visible = true;
        UIShop.Get.OpenPage(0);
    }
}