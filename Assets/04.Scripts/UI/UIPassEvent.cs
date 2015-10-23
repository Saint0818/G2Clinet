using UnityEngine;

public class UIPassEvent : UIDragDropItem {
		
	protected override void OnDragDropStart (){
		base.OnDragDropStart();
	}

	protected override void OnDragDropEnd(){
//		base.OnDragDropStart();
	}

	protected override void OnDragDropMove(Vector2 delta){
//		if(delta.x < -5){
//			UIGame.Get.DoPassTeammateA();
//		} else 
//		if(delta.y > 5)
//			UIGame.Get.DoPassTeammateB();
//		else 
//		if(delta.x < -5 && delta.y >= 5) {
//			if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
//				UIGame.Get.DoPassTeammateA();
//			else
//				UIGame.Get.DoPassTeammateB();
//		} 
	}

	protected override  void OnDragDropRelease (GameObject surface) {
//		if(surface.name.Equals("ButtonObjectA")) {
//			UIGame.Get.DoPassTeammateA();
//		}else 
//		if(surface.name.Equals("ButtonObjectB")) {
//			UIGame.Get.DoPassTeammateB();
//		}else{
			UIGame.Get.DoPassNone();
//		}
		base.OnDragDropRelease(surface);
	}
}
