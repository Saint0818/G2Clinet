using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

/// <summary>
/// sphere collider. 球員身上會有此 trigger.
/// </summary>
public class DefTrigger : MonoBehaviour
{
	public int Direction = 0;
	public PlayerBehaviour Player;
	
    [UsedImplicitly]
	void OnTriggerEnter(Collider other)
    {
		if (GameController.Visible && GameController.Get.IsStart)
        {
            // PlayerTrigger tag: 就是球員身上的 TriggerFR, TriggerBR, TriggerTop, TriggerSteal.
            if (other.gameObject.CompareTag("PlayerTrigger")) 
			{
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if(obj)
                {
					GameController.Get.DefRangeTouch(Player, obj.Player);
					obj.Player.IsTouchPlayerForCheckLayer(1);
				}
			}
            // RealBallTrigger tag: RealBall 的 child GameObject RealBallTrigger
            // 球員身上的 Sphere Collider 和球的 Box Collider 發生碰撞.
            else if(other.gameObject.CompareTag("RealBallTrigger"))
                GameController.Get.DefRangeTouchBall(Player);
		}
	}

    [UsedImplicitly]
    void OnTriggerExit(Collider other)
    {
		if (GameController.Visible && GameController.Get.IsStart)
        {
			if (other.gameObject.CompareTag("PlayerTrigger"))
            {
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if (obj)
					obj.Player.IsTouchPlayerForCheckLayer(-1);
			}
		}
	}
}
