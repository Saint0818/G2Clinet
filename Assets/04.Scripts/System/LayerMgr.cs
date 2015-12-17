using UnityEngine;

public enum EUIDepth
{
	Tutorial = 200,
	TutorialButton = 201,
	TutorialGuide = 202
}

public enum ELayer
{
	//for UI
	UI,
	ScrollVies,
	TopUI,
	UI3D,
	UIPlayer,
	UIPlayerInfo,
	//for game play
    Default,
	Player,
	Shooter,
	BasketCollider,
	RealBall,
	IgnoreRaycast
}

public enum ETag
{
	Player
}

public class LayerMgr : KnightSingleton<LayerMgr>
{
	public void ReSetLayerRecursively(GameObject obj, string layer, string containName1= "", string containName2= "") {
		//		obj.layer = LayerMask.NameToLayer(layer);
		if (obj == null) return;
		foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
			if(trans.name.Contains(containName1) || trans.name.Contains(containName2)){
				if(trans.gameObject.tag.Equals("Untagged")) 
					trans.gameObject.layer = LayerMask.NameToLayer(layer);
				else 
					trans.gameObject.layer = LayerMask.NameToLayer(trans.gameObject.tag);
			}
		}
	}

	public void SetLayerRecursively(GameObject obj, string layer, string containName1 = "", string containName2= "") {
		if (obj == null) return;

		foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
			if(trans.name.Contains(containName1) || trans.name.Contains(containName2))
				trans.gameObject.layer = LayerMask.NameToLayer(layer);
		}
	}

	public void SetLayerAllChildren(GameObject obj, string layer) {
		if (obj == null) return;
		
		foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
				trans.gameObject.layer = LayerMask.NameToLayer(layer);
		}
	}

	public void SetLayerAndTag(GameObject obj, ELayer layer, ETag tag)
	{
		SetLayer(obj, layer);
		SetTag(obj, tag);
	}

	public void SetLayer(GameObject obj, ELayer layer)
	{
		if (obj.layer != LayerMask.NameToLayer(layer.ToString())) {
			obj.layer = LayerMask.NameToLayer(layer.ToString());
		}
	}

	public void SetTag(GameObject obj, ETag tag)
	{
		obj.tag = tag.ToString();
	}

	public bool CheckLayer(GameObject obj, ELayer layer)
	{
		if (obj.layer == LayerMask.NameToLayer (layer.ToString ()))
			return true;
		else
			return false;
	}

	public void IgnoreLayerCollision(ELayer transform, ELayer target, bool isIgnore)
	{
		string name1 = transform.ToString();
		string name2 = target.ToString();

		if (transform == ELayer.IgnoreRaycast)
			name1 = "Ignore Raycast";

		if(target == ELayer.IgnoreRaycast)
			name2 = "Ignore Raycast";

		Physics.IgnoreLayerCollision (LayerMask.NameToLayer (name1), LayerMask.NameToLayer (name2), false);
	}


}
