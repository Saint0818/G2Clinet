using UnityEngine;
using System.Collections;

public class AutoHide : MonoBehaviour {
	public float hideTime = 0;
	public float originalHideTime = 0;

	private MeshRenderer meshRenderer;
	private float alpha;
	void Start() {
		meshRenderer = gameObject.GetComponent<MeshRenderer> ();
		if (meshRenderer) {
			alpha = meshRenderer.material.color.a;
		}
	}

	void Update() {
		hideTime -= Time.deltaTime;
		if (hideTime <= 0)
			gameObject.SetActive(false);
		else 
		if (meshRenderer && originalHideTime > 0) {
			float a = alpha * (1 - (originalHideTime - hideTime) / originalHideTime);
			meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, a);
		}
	}

	public float HideTime {
		set {
			hideTime = value;
			originalHideTime = value;
		}
	}
}
