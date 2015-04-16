﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BallTrigger : MonoBehaviour
{
	private Rigidbody ParentRigidbody;
	private GameObject followObject;
	private BoxCollider box;
	private GameObject HintObject;
	private bool passing = false;
	private Vector3 Parabolatarget;
	private float Parabolaspeed = 20;    
	private float ParaboladistanceToTarget; 
	private bool Parabolamove = true;  
	private bool doPassing = false;

	void Awake()
	{
		ParentRigidbody = gameObject.transform.parent.transform.gameObject.GetComponent<Rigidbody>();
		box = gameObject.GetComponent<BoxCollider>();
	}

	public void SetBoxColliderEnable(bool isShow)
	{
		if(box)
			box.enabled = isShow;
	}
	
	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.gameObject.CompareTag("Player"))
			{

			}
			else if (other.gameObject.CompareTag("Floor")) 
			{
				GameController.Get.BallOnFloor();
			} 
			else if (other.gameObject.CompareTag("Wall"))
			{
				EffectManager.Get.PlayEffect("BallTouchWall", gameObject.transform.position);
			}
		}
	}

	public void Falling()
	{
		ParentRigidbody.AddForce (new Vector3 (0, -100, 0));
	}
	
	public bool PassBall(int Kind = 0)
	{
		if (!passing && GameController.Get.Catcher) {
			passing = true;
			doPassing = true;

			SceneMgr.Get.SetBallState(PlayerState.PassFlat);
			float dis = Vector3.Distance(GameController.Get.Catcher.DummyBall.transform.position, SceneMgr.Get.RealBall.transform.position);
			float time = dis / (GameConst.BasicMoveSpeed * GameConst.AttackSpeedup * Random.Range(4, 6));

			switch(Kind)
			{
			case 0:
				SceneMgr.Get.RealBall.transform.DOMove(GameController.Get.Catcher.DummyBall.transform.position, time).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				break;
			case 2:
				Vector3 [] pathay = new Vector3[2];
				pathay[0] = GetMiddlePosition(GameController.Get.BallOwner.transform.position, GameController.Get.Catcher.DummyBall.transform.position);
				pathay[1] = GameController.Get.Catcher.DummyBall.transform.position;
				SceneMgr.Get.RealBall.transform.DOPath(pathay, time).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				break;
			case 1:
			case 3:
				Parabolamove = true;							  
				StartCoroutine(Parabola());
				break;
			}

			if(SceneMgr.Get.RealBall != null && GameController.Get.Catcher != null && Vector3.Distance(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.DummyBall.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;

			return true;
		}else
			return false;
	}

	private void PassUpdate()
	{
		if (GameController.Get.Catcher != null) 
		{
			float currentDist = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.transform.position);  
			if (currentDist < 3.5f && doPassing)
			{
				doPassing = false;
				GameController.Get.Catcher.AniState (PlayerState.Catch, GameController.Get.BallOwner.transform.position);		
			} 				
		}
	}
	
	private Vector3 GetMiddlePosition(Vector3 p1, Vector3 p2){
		Vector3 Result = Vector3.zero;
		Result.x = (p1.x + p2.x) / 2; 
		Result.y = 0; 
		Result.z = (p1.z + p2.z) / 2; 
		return Result;
	}

	void LateUpdate()
	{
		if (gameObject.activeInHierarchy) {
			if (gameObject.transform.position.y < 0)
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.3f, gameObject.transform.position.z);

			if (Mathf.Abs(gameObject.transform.position.x) > 20)
			    gameObject.transform.position = Vector3.zero;

			if (Mathf.Abs(gameObject.transform.position.z) > 20)
			    gameObject.transform.position = Vector3.zero;
		}
	}

	public void PassEnd(){
		GameController.Get.SetEndPass();
		passing = false;
		CameraMgr.Get.IsLongPass = false;
	}

	void Update()
	{
		gameObject.transform.localPosition = Vector3.zero;
	}	    

	IEnumerator Parabola()  
	{  		
		while (Parabolamove && GameController.Get.Catcher != null)  
		{  
			float [] disAy = new float[4];
			disAy [0] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPoint.FrontSteal.GetHashCode()].position, GameController.Get.BallOwner.transform.position);
			disAy [1] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPoint.BackSteal.GetHashCode()].position, GameController.Get.BallOwner.transform.position);
			disAy [2] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPoint.RightSteal.GetHashCode()].position, GameController.Get.BallOwner.transform.position);
			disAy [3] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPoint.LeftSteal.GetHashCode()].position, GameController.Get.BallOwner.transform.position);
			int Index = MinIndex(disAy);
			
			Parabolatarget = new Vector3(GameController.Get.Catcher.DefPointAy[4 + Index].position.x, 2, GameController.Get.Catcher.DefPointAy[4 + Index].position.z);
			ParaboladistanceToTarget = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget);

			Vector3 targetPos = Parabolatarget;  
			SceneMgr.Get.RealBall.transform.LookAt(targetPos);  
			float angle = Mathf.Min(1, Vector3.Distance(SceneMgr.Get.RealBall.transform.position, targetPos) / ParaboladistanceToTarget) * 45;  
			SceneMgr.Get.RealBall.transform.rotation = SceneMgr.Get.RealBall.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);  
			float currentDist = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget);  			 			

			if (currentDist <= 3){
				Parabolamove = false;  
				PassEnd();
			}else if(currentDist <= 6 && doPassing) 
			{
				doPassing = false;
				SceneMgr.Get.RealBall.transform.Translate(Vector3.forward * Mathf.Min(Parabolaspeed * Time.deltaTime, currentDist)); 
				GameController.Get.Catcher.AniState(PlayerState.Catch, GameController.Get.BallOwner.transform.position);
			}else
				SceneMgr.Get.RealBall.transform.Translate(Vector3.forward * Mathf.Min(Parabolaspeed * Time.deltaTime, currentDist)); 

			yield return null;  
		}  
	}

	private int MinIndex(float[] floatAy)
	{
		int Result = 0;
		
		float Min = floatAy [0];
		
		for (int i = 1; i < floatAy.Length; i++)
		{
			if (floatAy [i] < Min)
			{
				Min = floatAy [i];
				Result = i;
			}
		}

		return Result;	
	}
}