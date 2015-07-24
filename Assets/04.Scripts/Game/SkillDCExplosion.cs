using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SkillDCExplosion : MonoBehaviour {
	private static SkillDCExplosion instance = null;

	private List<GameObject> pooledObjects;
	private GameObject skillTarget;
	private int completeCount;

	public static SkillDCExplosion Get {
		get
		{
			if(instance == null)
			{
				GameObject go = new GameObject("SkillDCExplosion");
				instance = go.AddComponent<SkillDCExplosion>();
			}
			return instance;
		}
	}

	void Awake() {
		pooledObjects = new List<GameObject>();
	}

	void FixedUpdate() {

	}

	public void BornDC (int count, GameObject position, GameObject target, GameObject parent = null) {
		skillTarget = target;
		if(count > 0) {
			transform.position = position.transform.position;
			if(parent)
				this.transform.parent = parent.transform;
			for (int i=0; i<count; i++) {
				GameObject obj =  getPooledObject();
				obj.name = i.ToString();
				Transform t = obj.transform.FindChild("Trail");
				if(t)
					t.gameObject.SetActive(false);
				obj.transform.parent = transform;
				obj.transform.localPosition = Vector3.zero;
				obj.SetActive(true);
				if(!obj.GetComponent<SkillDCMove>())
					obj.AddComponent<SkillDCMove>();

				obj.GetComponent<SkillDCMove>().IsMove = false;
				obj.GetComponent<SkillDCMove>().Born = gameObject;
				obj.GetComponent<SkillDCMove>().Target = target;
				if(t)
					t.gameObject.SetActive(true);
				moveTo (obj);
			}
		}
	}

	private void moveTo (GameObject skillSoul) {
		Tweener tweener =  skillSoul.transform.DOLocalMove(new Vector3(skillSoul.transform.localPosition.x + Random.Range(-2, 2) ,
		                                                               skillSoul.transform.localPosition.y + Random.Range(0, 3),
		                                                               skillSoul.transform.localPosition.z + Random.Range(-2, 2)),0.1f);

		tweener.SetEase(Ease.OutCubic);
		tweener.OnComplete(delegate(){
			skillSoul.GetComponent<SkillDCMove>().IsMove = true;

//			tweenerTwo.OnUpdate(delegate(){
//			});
//			tweenerTwo.OnUpdate (() => tweenerTwo.ChangeEndValue (skillTarget.transform.position, true));
//			tweenerTwo.OnComplete(delegate(){
//				
//			});
		});
	}

	private GameObject getPooledObject () {
		if(pooledObjects.Count > 0) {
			for(int i=0; i<pooledObjects.Count; i++) {
				if(!pooledObjects[i].activeInHierarchy)
					return pooledObjects[i];
			}
		}

		GameObject obj = Instantiate(Resources.Load("Effect/DC_Soul") as GameObject);
		pooledObjects.Add(obj);
		return obj;
	}

}
