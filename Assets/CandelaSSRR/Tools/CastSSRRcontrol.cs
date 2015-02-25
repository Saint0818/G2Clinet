using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class CastSSRRcontrol : MonoBehaviour {
	
	public bool CastSSRR = false;
	
	private float ssrrControl = 1.0f;
	
	
	void Update ()
	{
	ssrrControl = CastSSRR ? 0.0f : 1.0f;
	this.gameObject.renderer.sharedMaterial.SetFloat("_ExcludeFromSSRR", ssrrControl);
	}
}
