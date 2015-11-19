using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

namespace  SkillBuffSpace {

	public struct TBuff {
		public UISprite SpriteBuff;
		public GameObject DeBuff;
		public GameObject Buff;
		public GameObject Info;
		public Animator AnimatorInfo;
		public float LifeTime;
		public int InfoIndex;
		public bool isClose;
		
		public TBuff(int i){
			this.SpriteBuff = null;
			this.DeBuff = null;
			this.Buff = null;
			this.Info = null;
			this.AnimatorInfo = null;
			this.LifeTime = 0;
			this.InfoIndex = 0;
			this.isClose = true;
		}
	}

	public struct TBuffRefresh {
		public int Index;
		public float LifeTime;
		
		public TBuffRefresh(int id, float time)
		{
			this.Index = id;
			this.LifeTime = time;
		}
	}

	public delegate void OnFinishBuffDelegate(int skillID);

	public class SkillBuff{
		public OnFinishBuffDelegate OnFinishBuff = null;
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
				buffInfo[i].Buff = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()+"/BuffSprite/Buff").gameObject;
				buffInfo[i].DeBuff = playerInfo.transform.FindChild("Scale/Billboard/BuffPos_"+(i+1).ToString()+"/BuffSprite/DeBuff").gameObject;
			}

			initValue ();
		}

		public void HideName (){
			labelPlayerName.gameObject.SetActive(false);
		}

		public void UpdateBuff () {
			if(buffInfo.Length > 0) {
				for (int i=0; i<buffInfo.Length; i++) {
					if(buffInfo[i].LifeTime > 0) {
						buffInfo[i].LifeTime -= Time.deltaTime * TimerMgr.Get.CrtTime;
						if(buffInfo[i].LifeTime > 0 && buffInfo[i].LifeTime <= 3 && !buffInfo[i].isClose) {
							buffInfo[i].isClose = true;
							buffInfo[i].AnimatorInfo.SetTrigger("TimeOut");
						}

						if(buffInfo[i].LifeTime <= 0) {
							hideBuff(i);
						}
					}
				}
			}
		}

		public List<int> GetAllBuff (){
			List<int> buffIDs = new List<int>();
			if(recordIndex.Length > 0) {
				for (int i=0; i<recordIndex.Length; i++) {
					buffIDs.Add(buffInfo[i].InfoIndex);
				}
			}
			return buffIDs;
		}

		/// <summary>
		/// All of Buff Count is 4
		/// Position is from left to right;
		/// In the left, lifeTime is the shortest
		/// In the Right, lifeTime is the longest
		/// </summary>
		/// 

		public void RemoveAllBuff () {
			initValue ();
		}
		
		public void RemoveBuff (int skillIndex) {
			int positionIndex = contains(skillIndex);
			if(positionIndex != -1) {
				buffInfo[positionIndex].Info.SetActive(false);
				buffInfo[positionIndex].InfoIndex = -1;
				buffInfo[positionIndex].LifeTime = 0;
				buffInfo[positionIndex].isClose = true;
				removeRecord(positionIndex);
				refreshBuff();
			} 
		}
		
		public void AddBuff (int skillIndex, float lifeTime, float value) {
			if(buffCount <= 4) {
				int positionIndex = contains(skillIndex);
				if(positionIndex != -1) {
					if(buffInfo[positionIndex].LifeTime <= 3) 
						buffInfo[positionIndex].Info.SetActive(false);
					buffInfo[positionIndex].LifeTime = lifeTime;
					buffInfo[positionIndex].isClose = false;
					buffInfo[positionIndex].Buff.SetActive((value > 0));
					buffInfo[positionIndex].DeBuff.SetActive(!(value > 0));
					addRecord(positionIndex);
					refreshBuff();
				} else {
					for(int i=0; i<buffInfo.Length; i++) {
						if(!buffInfo[i].Info.activeInHierarchy) {
							buffInfo[i].LifeTime = lifeTime;
							buffInfo[i].InfoIndex = skillIndex;
							buffInfo[i].SpriteBuff.spriteName = "AttrKind_" + skillIndex.ToString();
							buffInfo[i].isClose = false;
							buffInfo[positionIndex].Buff.SetActive((value > 0));
							buffInfo[positionIndex].DeBuff.SetActive(!(value > 0));
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
		
		private int buffCount  {
			get {
				int result = 0;
				for (int i=0; i<recordIndex.Length; i++) {
					if(recordIndex[i] != -1)
						result ++;
				}
				return result;
			}
		}
		
		private int contains (int skillID) {
			for (int i=0; i<buffInfo.Length; i++) 
				if(buffInfo[i].InfoIndex == skillID)
					return i;
			return -1;
		}

		private bool containsSkill (int skillID) {
			for (int i=0; i<buffInfo.Length; i++) 
				if(buffInfo[i].InfoIndex == skillID)
					return true;

			return false;
		}
		
		private bool containsBool (int positioIndex) {
			for (int i=0; i<recordIndex.Length; i++) 
				if(recordIndex[i] == positioIndex)
					return true;

			return false;
		}
		
		private int indexOf (int positionIndex) {
			for (int i=0; i<recordIndex.Length; i++) 
				if(recordIndex[i] == positionIndex)
					return i;

			return 0;
		}
		
		private void hideBuff(int positionIndex){
			if(positionIndex >=0 && positionIndex < buffInfo.Length) {
				if(OnFinishBuff != null) 
					OnFinishBuff(buffInfo[positionIndex].InfoIndex);
				buffInfo[positionIndex].Info.SetActive(false);
				buffInfo[positionIndex].LifeTime = 0;
				buffInfo[positionIndex].InfoIndex = -1;
				recordIndex[indexOf(positionIndex)] = -1;
				refreshBuff ();
			}
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
		
		private void addRecord (int positionIndex) {
			if(!containsBool(positionIndex)) {
				for(int i=0; i<recordIndex.Length; i++) {
					if(recordIndex[i] == -1){
						recordIndex[i] = positionIndex;
						break;
					}
				}
			}
		}
		
		private void removeRecord (int positionIndex) {
			if(containsBool(positionIndex)) {
				for(int i=0; i<recordIndex.Length; i++) {
					if(recordIndex[i] == positionIndex){
						recordIndex[i] = -1;
						break;
					}
				}
			}
		}
	}
}