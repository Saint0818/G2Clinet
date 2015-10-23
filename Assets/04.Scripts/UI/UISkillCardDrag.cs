using UnityEngine;

public class UISkillCardDrag : UIDragDropItem {
	private Vector3 OrigalPosition;
	private int originalIndes;

	protected override void Update ()
	{
		if (restriction == Restriction.PressAndHold)
		{
			if (mPressed && !mDragging && mDragStartTime < RealTime.time && !UISkillFormation.Get.CheckCardUsed(gameObject.name))
				StartDragging();
		}
	}

	protected override void OnDragDropStart ()
	{
		if(!UISkillFormation.Get.CheckCardUsed(gameObject.name)) {
			OrigalPosition = gameObject.transform.localPosition;
			originalIndes = getPositionIndex(gameObject.transform.position.x, gameObject.transform.position.y);
			UISkillFormation.Get.ShowInstallLight(gameObject, true);
			base.OnDragDropStart();
		}
	}

	public override void StartDragging () {
		if (!interactable) return;

		if (!mDragging && !UISkillFormation.Get.IsBuyState && !UISkillFormation.Get.IsDragNow)
		{
			if (cloneOnDrag)
			{
				mPressed = false;
				GameObject clone = NGUITools.AddChild(transform.parent.parent.FindChild("DragSpace").gameObject, gameObject);
				clone.transform.localPosition = transform.localPosition;
				clone.transform.localRotation = transform.localRotation;
				clone.transform.localScale = transform.localScale;
				
				UIButtonColor bc = clone.GetComponent<UIButtonColor>();
				if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;
				
				if (mTouch != null && mTouch.pressed == gameObject)
				{
					mTouch.current = clone;
					mTouch.pressed = clone;
					mTouch.dragged = clone;
					mTouch.last = clone;
				}
				
				UISkillCardDrag item = clone.GetComponent<UISkillCardDrag>();
				item.mTouch = mTouch;
				item.mPressed = true;
				item.mDragging = true;
				item.Start();
				item.OnClone(gameObject);
				item.OnDragDropStart();
				
				if (UICamera.currentTouch == null)
					UICamera.currentTouch = mTouch;
				
				mTouch = null;
				
				UICamera.Notify(gameObject, "OnPress", false);
				UICamera.Notify(gameObject, "OnHover", false);
			}
			else
			{
				mDragging = true;
				OnDragDropStart();
			}
		}
	}

	protected override void OnDragDropEnd () 
	{ 
		gameObject.transform.localPosition = OrigalPosition;
		base.OnDragDropEnd();
	}

	protected override void OnDrag (Vector2 delta)
	{
		if (!interactable || UISkillFormation.Get.CheckCardUsed(gameObject.name)) return;
		if (!mDragging || !enabled || mTouch != UICamera.currentTouch) return;
		OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
	}

	protected override void OnDragDropMove (Vector2 delta)
	{
		base.OnDragDropMove(delta);
	}
	
	protected override void OnDragDropRelease (GameObject surface)
	{
		if(surface == null)
			return ;
		base.OnDragDropRelease(surface);
		UISkillFormation.Get.ShowInstallLight(gameObject, false);
		int index = getPositionIndex(surface.transform.position.x, surface.transform.position.y);

		if(transform.parent != null) {
			if(transform.parent.name.Contains("ActiveCardBase")) {
				if(originalIndes != index) {
					if (index != -1 && index != 4){
						UISkillFormation.Get.SwitchItem(originalIndes, index);
					} else {
						transform.localPosition = Vector3.zero;
					}
				}
			}
		} else {
			if (index != -1 && Input.mousePosition.x > (Screen.width * 0.67f)){
				UISkillFormation.Get.AddItem(gameObject, index);
			}else{
				NGUITools.Destroy(gameObject);
			}
		}

	}

	private int getPositionIndex (float x, float y) {
		if(x > 0) {
			if(y > 0 && UISkillFormation.Get.IsCardActive) {
				if(y >= 0.15f && y<=0.25f)
					return 2;
				else if(y >= 0.35f && y<=0.45f)
					return 1;
				else if(y >= 0.55f && y<=0.65f)
					return 0;
			} else if(y < 0 && !UISkillFormation.Get.IsCardActive)
				return 4;
		}
		return -1;
	}
}
