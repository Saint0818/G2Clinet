using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerTrigger : MonoBehaviour {
	public int Direction = 0;
	public PlayerBehaviour Player;

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.gameObject.CompareTag("PlayerTrigger"))
			{
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if (obj)
					GameController.Get.PlayerTouchPlayer(Player, obj.Player, Direction);
			}
			else if (other.gameObject.CompareTag("RealBallTrigger"))
			{
				if((GameController.Get.situation == GameSituation.AttackA && Player.Team == TeamKind.Npc) ||
				   (GameController.Get.situation == GameSituation.AttackB && Player.Team == TeamKind.Self))
				{
					int Rate = UnityEngine.Random.Range(0, 100);
					Debug.Log(Rate);
					if(SceneMgr.Get.RealBallState == PlayerState.PassFlat || 
					   SceneMgr.Get.RealBallState == PlayerState.PassFloor ||
					   SceneMgr.Get.RealBallState == PlayerState.PassParabola)
					{
						if(GameController.Get.BallOwner == null && (Rate < 20 || Direction == 5))
						{
							if(Direction == 6)
							{
								Player.AniState(PlayerState.Intercept1, SceneMgr.Get.RealBall.transform.position);
							}
							else if(Direction == 5)
							{
								if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
									SceneMgr.Get.RealBall.transform.DOKill();
								
								if(GameController.Get.SetBall(Player))
									Player.AniState(PlayerState.HoldBall);
								
								GameController.Get.Catcher = null;
								GameController.Get.IsPassing = false;
							}
							else if(Direction != 0)
							{
								Player.AniState(PlayerState.Intercept0);
								
								if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
									SceneMgr.Get.RealBall.transform.DOKill();
								
								if(GameController.Get.SetBall(Player))
									Player.AniState(PlayerState.HoldBall);
								
								GameController.Get.Catcher = null;
								GameController.Get.IsPassing = false;
							}
						}
					}
				}else
					GameController.Get.BallTouchPlayer(Player, Direction);
			} 
		}
	}

	void OnTriggerStay(Collider other) {
		if (GameController.Visible){		
			if (other.gameObject.CompareTag("RealBallTrigger"))
				GameController.Get.BallTouchPlayer(Player, Direction);
		}
	}
}
