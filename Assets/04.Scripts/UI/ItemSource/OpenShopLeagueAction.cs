using UnityEngine;

public class OpenShopLeagueAction : UIItemSourceElement.IAction
{
    public void Do()
    {
//        Debug.Log("Open Shop League.");

        UIShop.Visible = true;
        UIShop.Get.openPage(1);
    }
}