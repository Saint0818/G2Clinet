using DG.Tweening;
using UnityEngine;

public enum EMovement {
	Horizonal,
	Vertical
}

public class AutoShow : MonoBehaviour {
	public EMovement Movement;
	public UIPanel Panel;
	public float OriginalPosition;
	public float MoveTimeOfSecond;
	public float Delaytime;
	public float ItemPixels;

	private float pos;

	void OnEnable () {
		pos = OriginalPosition;
		InvokeRepeating("RePlay", -1, Delaytime);
	}

	void OnDisable () {
		CancelInvoke();
	}

	private void RePlay () {
		pos += ItemPixels;
		if(Movement == EMovement.Horizonal){
			transform.DOLocalMoveX(pos, MoveTimeOfSecond).OnUpdate(FinX);
		} else {
			transform.DOLocalMoveY(pos, MoveTimeOfSecond).OnUpdate(FinY);
		}
	}

	public void FinX () {
		Panel.clipOffset = new Vector2(transform.localPosition.x * -1, 0);
	}

	public void FinY () {
		Panel.clipOffset = new Vector2(0, transform.localPosition.y * -1);
	}
}
