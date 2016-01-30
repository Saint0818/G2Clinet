using UnityEngine;

public class OpenShopSocialAction : UIItemSourceElement.IAction
{
    public void Do()
    {
//        Debug.Log("Open Shop Social.");

        UIShop.Visible = true;
        UIShop.Get.OpenPage(2);
    }
}