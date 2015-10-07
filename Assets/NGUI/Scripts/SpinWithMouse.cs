using JetBrains.Annotations;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse"), DisallowMultipleComponent]
public class SpinWithMouse : MonoBehaviour
{
	public Transform Target;
	public float Speed = 1f;
	
	Transform mTrans;
	
    [UsedImplicitly]
	void Start()
	{
		mTrans = transform;
	}

    [UsedImplicitly]
    void OnDrag(Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		
		if (Target != null)
			Target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * Speed, 0f) * Target.localRotation;
		else
			mTrans.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * Speed, 0f) * mTrans.localRotation;
	}
}