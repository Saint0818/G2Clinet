﻿using UnityEngine;
using System.Collections;
using GameStruct;

namespace  SkillBuffSpace {

	[System.Serializable]
	public struct TBuff {
		public UISprite SpriteBuff;
		public GameObject Info;
		public Animator AnimatorInfo;
		public float LifeTime;
		public int InfoIndex;
		public bool isClose;
		
		public TBuff(int i){
			this.SpriteBuff = null;
			this.Info = null;
			this.AnimatorInfo = null;
			this.LifeTime = 0;
			this.InfoIndex = 0;
			this.isClose = true;
		}
	}

	[System.Serializable]
	public struct TBuffRefresh {
		public int Index;
		public float LifeTime;
		
		public TBuffRefresh(int id, float time)
		{
			this.Index = id;
			this.LifeTime = time;
		}
	}

	public class SkillBuff{
		private UILabel labelPlayerName;
		private TBuff[] buffInfo = new TBuff[4];
		private TBuffRefresh[] refreshIndex = new TBuffRefresh[4];
		
		private int [] recordIndex = new int[4];

		public void InitBuff (GameObject playerInfo, TPlayer Attribute, GameObject player){
			playerInfo.transform.parent = player.transform;
			playerInfo.transform.name = "PlayerInfo";
			playerInfo.transform.localPosition = Vector3.zero;
			labelPlayerName = playerInfo.transform.FindChild("Scale/Billboard/PlayerNameLabel").GetComponent<UILabel>();
			labelPlayerName.text = Attribute.Name;
			for(int i=0; i<buffInfo.Length; i++) {
				buffInfo[i].Info = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()).gameObject;
				buffInfo[i].AnimatorInfo = buffInfo[i].Info.GetComponent<Animator>();
				buffInfo[i].Info.SetActive(false);
				buffInfo[i].SpriteBuff = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()+"/BuffSprite").GetComponent<UISprite>();
			}

			initValue ();
		}

//		public void InitBuff (GameObject playerInfo){
//			playerInfo.transform.name = "PlayerInfo";
//			playerInfo.transform.localPosition = Vector3.zero;
//			labelPlayerName = playerInfo.transform.FindChild("Scale/Billboard/PlayerNameLabel").GetComponent<UILabel>();
//			for(int i=0; i<buffInfo.Length; i++) {
//				buffInfo[i].Info = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()).gameObject;
//				buffInfo[i].AnimatorInfo = buffInfo[i].Info.GetComponent<Animator>();
//				buffInfo[i].Info.SetActive(false);
//				buffInfo[i].SpriteBuff = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()+"/BuffSprite").GetComponent<UISprite>();
//			}
//			
//			initValue ();
//		} 

		public void UpdateBuff () {
			for (int i=0; i<buffInfo.Length; i++) {
				if(buffInfo[i].LifeTime > 0) {
					buffInfo[i].LifeTime -= Time.deltaTime;
					if(buffInfo[i].LifeTime > 0 && buffInfo[i].LifeTime <= 3 && !buffInfo[i].isClose) {
						buffInfo[i].isClose = true;
						buffInfo[i].AnimatorInfo.SetTrigger("TimeOut");
						
					}
					if(buffInfo[i].LifeTime <= 0) {
						buffInfo[i].LifeTime = 0;
						hideBuff(i);
					}
				}
			}
		}

		/// <summary>
		/// All of Buff Count is 4
		/// Position is from left to right;
		/// In the left, lifeTime is the shortest
		/// In the Right, lifeTime is the longest
		/// </summary>
		public void RemoveAllBuff () {
			initValue ();
		}
		
		public void RemoveBuff (int index) {
			int positionIndex = contains(index);
			if(positionIndex != -1) {
				buffInfo[positionIndex].Info.SetActive(false);
				buffInfo[positionIndex].InfoIndex = -1;
				buffInfo[positionIndex].LifeTime = 0;
				buffInfo[positionIndex].isClose = true;
				removeRecord(positionIndex);
				refreshBuff();
			} 
		}
		
		public void AddBuff (int index, float lifeTime) {
			if(buffCount() <= 4) {
				int positionIndex = contains(index);
				if(positionIndex != -1) {
					if(buffInfo[positionIndex].LifeTime <= 3) 
						buffInfo[positionIndex].Info.SetActive(false);
					buffInfo[positionIndex].LifeTime = lifeTime;
					buffInfo[positionIndex].isClose = false;
					addRecord(positionIndex);
					refreshBuff();
				} else {
					for(int i=0; i<buffInfo.Length; i++) {
						if(!buffInfo[i].Info.activeInHierarchy) {
							buffInfo[i].LifeTime = lifeTime;
							buffInfo[i].InfoIndex = index;
//							buffInfo[i].SpriteBuff.name
							buffInfo[i].isClose = false;
							addRecord(i);
							refreshBuff ();
							break;
						}	
					}
				}
			}
		}
		
		private void initValue (){
			for (int i=0; i<buffInfo.Length; i++) {
				buffInfo[i].Info.SetActive(false);
				recordIndex[i] = -1;
				buffInfo[i].isClose = true;
				buffInfo[i].LifeTime = 0;
				buffInfo[i].InfoIndex = -1;
			}
		}
		
		private int buffCount () {
			int result = 0;
			for (int i=0; i<recordIndex.Length; i++) {
				if(recordIndex[i] != -1)
					result ++;
			}
			return result;
		}
		
		private int contains (int index) {
			for (int i=0; i<buffInfo.Length; i++) {
				if(buffInfo[i].InfoIndex == index){
					return i;
				}
			}
			return -1;
		}
		
		private bool containsBool (int index) {
			for (int i=0; i<recordIndex.Length; i++) {
				if(recordIndex[i] == index){
					return true;
				}
			}
			return false;
		}
		
		private int indexOf (int index) {
			for (int i=0; i<recordIndex.Length; i++) {
				if(recordIndex[i] == index)
					return i;
			}
			return 0;
		}
		
		private void hideBuff(int index){
			buffInfo[index].Info.SetActive(false);
			buffInfo[index].LifeTime = 0;
			buffInfo[index].InfoIndex = -1;
			recordIndex[indexOf(index)] = -1;
			refreshBuff ();
		}
		
		private void refreshBuff () {
			for(int i=0; i<refreshIndex.Length; i++) {
				if(recordIndex[i] != -1) {
					refreshIndex[i].Index = recordIndex[i];
					refreshIndex[i].LifeTime = buffInfo[recordIndex[i]].LifeTime;
				} else {
					refreshIndex[i].Index = -1;
					refreshIndex[i].LifeTime = 0;
				}
			}
			
			for (int i=0; i<refreshIndex.Length; i++) {
				for (int j=i+1; j<refreshIndex.Length; j++){
					if (refreshIndex[i].LifeTime >= refreshIndex[j].LifeTime){
						TBuffRefresh temp = refreshIndex[i];
						refreshIndex[i] = refreshIndex[j];
						refreshIndex[j] = temp;
					}
				}
			}
			
			int index = 0;
			for (int i=0; i<refreshIndex.Length; i++) {
				recordIndex[i] = -1;
				if(refreshIndex[i].Index != -1) {
					recordIndex[index] = refreshIndex[i].Index;
					buffInfo[refreshIndex[i].Index].Info.transform.localPosition = new Vector3(-45 + (index * 30), -10, 0);
					buffInfo[refreshIndex[i].Index].Info.SetActive(true);
					index ++;
				} 
			}
			
		}
		
		private void addRecord (int index) {
			if(!containsBool(index)) {
				for(int i=0; i<recordIndex.Length; i++) {
					if(recordIndex[i] == -1){
						recordIndex[i] = index;
						break;
					}
				}
			}
		}
		
		private void removeRecord (int index) {
			if(containsBool(index)) {
				for(int i=0; i<recordIndex.Length; i++) {
					if(recordIndex[i] == index){
						recordIndex[i] = -1;
						break;
					}
				}
			}
		}
	}
}