using UnityEngine;
using System.Collections;

public class UIPassEvent : UIDragDropItem {
		
	protected override void OnDragDropStart (){
		base.OnDragDropStart();
	}

	protected override void OnDragDropEnd(){
//		base.OnDragDropStart();
	}

	protected override void OnDragDropMove(Vector2 delta){
	}

	protected override  void OnDragDropRelease (GameObject surface) {
		if(surface.name.Equals("ButtonObjectA")) {
			UIGame.Get.DoPassTeammateA(gameObject);
		}else 
		if(surface.name.Equals("ButtonObjectB")) {
			UIGame.Get.DoPassTeammateB(gameObject);
		}else{
			UIGame.Get.DoPassNone();
		}
		base.OnDragDropRelease(surface);
	}
}
