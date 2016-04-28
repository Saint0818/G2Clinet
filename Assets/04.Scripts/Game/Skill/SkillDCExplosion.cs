using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public struct TDCValue {
	public GameObject Obj;
	public SkillDCMove DCMove;

	public bool ObjVisible {
		set {Obj.SetActive(value);}
		get {return Obj.activeInHierarchy;}
	}
}

public class SkillDCExplosion : MonoBehaviour {
	private static SkillDCExplosion instance = null;

	private List<TDCValue> pooledObjects;
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
		pooledObjects = new List<TDCValue>();
	}

	void Destroy() {
		for(int i=0; i<pooledObjects.Count ;i++)
			Destroy(pooledObjects[i].Obj);

		pooledObjects.Clear();
	}

	public void BornDC (int count, GameObject position, GameObject target, GameObject parent = null) {
		if(count > 0) {
			if(parent){
				transform.parent = parent.transform;
				transform.localPosition = Vector3.zero;
			} else {
				transform.position = position.transform.position;
				if(position.transform.position.y <= 1) {
					transform.position = new Vector3(position.transform.position.x,
					                                 position.transform.position.y + 1.5f,
					                                 position.transform.position.z);
				}
			}
			for (int i=0; i<count; i++) {
				TDCValue obj =  getPooledObject();
				obj.Obj.name = i.ToString();
				Transform t = obj.Obj.transform.FindChild("Trail");
				if(t)
					t.gameObject.SetActive(false);
				
				obj.Obj.transform.parent = transform;
				obj.Obj.transform.localPosition = Vector3.zero;
				obj.ObjVisible = true;
				obj.DCMove.IsMove = false;
				obj.DCMove.Born = gameObject;
				obj.DCMove.Target = target;
				if(t)
					t.gameObject.SetActive(true);
				
				moveTo (obj);
			}
		}
	}

	private void moveTo (TDCValue skillSoul) {
		Tweener tweener =  skillSoul.Obj.transform.DOLocalMove(new Vector3(skillSoul.Obj.transform.localPosition.x + Random.Range(-2, 2) ,
																			skillSoul.Obj.transform.localPosition.y + Random.Range(-2, 2),
																			skillSoul.Obj.transform.localPosition.z + Random.Range(-2, 2)),0.5f);

		tweener.SetEase(Ease.InOutBack);
		tweener.OnComplete(delegate(){
			skillSoul.DCMove.IsMove = true;
		});
	}

	private TDCValue getPooledObject () {
		if(pooledObjects.Count > 0) 
			for(int i=0; i<pooledObjects.Count; i++) 
				if(!pooledObjects[i].ObjVisible)
					return pooledObjects[i];

		TDCValue value = new TDCValue();
		value.Obj = Instantiate(Resources.Load("Effect/DC_Soul") as GameObject);
		if(!value.Obj.GetComponent<SkillDCMove>())
			value.Obj.AddComponent<SkillDCMove>();
		
		value.DCMove = value.Obj.GetComponent<SkillDCMove>();
		pooledObjects.Add(value);
		return value;
	}

	public bool IsHaveDC {
		get {
			for(int i=0; i<pooledObjects.Count; i++) {
				if(pooledObjects[i].ObjVisible)
					return true;
			}
			return false;
		}
	}
}
