﻿using System;
using System.IO;
using GameStruct;
using UnityEngine;
using System.Collections.Generic;

public static class GameFunction
{
	private static byte[] mFlagAy = {1, 2, 4, 8, 16, 32, 64, 128};

	public static bool CheckByteFlag(int No, params byte[] FlagAy){
		if((No > 0) && (No <= FlagAy.Length * 8)){
			int i = (ushort)((No - 1) / 8);
			int j = (ushort)(((No - 1) % 8) + 1);
			byte Value = mFlagAy[j - 1];
			if(i >= 0 && i <= FlagAy.Length - 1)
				if((FlagAy[i] & Value) == Value)
					return true;
		}

		return false;
	}
	
	public static void Add_ByteFlag(int No, ref byte[] FlagAy){
		if(!CheckByteFlag(No, FlagAy)){
			int i = (No - 1) / 8 + 1;
			int j = (No - 1) % 8 + 1;
			byte Value = mFlagAy[j - 1];
			FlagAy[i - 1] = (byte)(FlagAy[i - 1] | Value);
		}
	}
	
	public static void Del_ByteFlag(int No, ref byte[] FlagAy){
		if(CheckByteFlag(No, FlagAy)){
			int i = (No - 1) / 8 + 1;
			int j = (No - 1) % 8 + 1;
			byte Value = mFlagAy[j - 1];
			FlagAy[i - 1] = (byte)(FlagAy[i - 1] ^ Value);
		}
	}

	public static Vector3 CalculateNextPosition(Vector3 source, Vector3 velocity, float time) {
		if (IsParabolicVelocity(velocity)) {
			return source + (velocity * time) + (0.5f * Physics.gravity * time * time);
		} else 
			return Vector3.zero;
	}

//	public static float ElevationAngle(Vector3 source, Vector3 target)
//	{
//		// find the cannon->target vector:
//		Vector3 dir = target - source;
//		// create a horizontal version of it:
//		Vector3 dirH = new Vector3(dir.x, 0, dir.y);
//		// measure the unsigned angle between them:
//		float ang = Vector3.Angle(dir, dirH);
//		// add the signal (negative is below the cannon):
//		if (dir.y < 0)
//		{ 
//			ang = -ang;
//		}
//		
//		return ang;
//	}

//    /// <summary>
//    /// target 是否在 source +Z 軸的扇形範圍內.
//    /// </summary>
//    /// <param name="source"></param>
//    /// <param name="target"></param>
//    /// <param name="fanDis"></param>
//    /// <param name="fanAngle"></param>
//    /// <returns> true: target 在 source 的扇形範圍內. </returns>
//    public static bool IsInFanArea([NotNull] Transform source, Vector3 target, float fanDis, float fanAngle)
//	{
////		Vector3 relative = source.InverseTransformPoint(target);
////		float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
//	    var betweenAngle = MathUtils.FindAngle(source, target);
//        var betweenDis = MathUtils.Find2DDis(source.position, target);
//
//		return Mathf.Abs(betweenAngle) <= fanAngle * 0.5f && betweenDis <= fanDis;
//	}

	public static Vector3 GetVelocity(Vector3 source, Vector3 target, float angle, float distOffset = 0f)
	{
		try
		{
			Vector3 dir = target - source;  // get target direction
			float h = dir.y;  // get height difference
			dir.y = 0;  // retain only the horizontal direction
			float dist = dir.magnitude + distOffset;  // get horizontal distance
			float a = angle * Mathf.Deg2Rad;  // convert angle to radians
			float tan = Mathf.Tan(a);
			dir.y = dist * tan;  // set dir to the elevation angle
			if (Mathf.Abs(tan) >= 0.01f)
			{
				dist += h / tan;
			}  // correct for small height differences
			
			// calculate the velocity magnitude
			float sin = Mathf.Sin(2 * a);
			float vel = 1;
			if (sin != 0)
			{
				float value = Mathf.Abs(dist) * Physics.gravity.magnitude;
				
				vel = Mathf.Sqrt(value / sin);
			}
			
			return vel * dir.normalized;
		} catch (Exception e)
		{
			Debug.Log(e.ToString());
			return Vector3.one;
		}
	}

	public static Vector3 calculateTrajectory(Vector3 source, Vector3 target, Vector3 velocity, float distance)
	{
		velocity.y = 0;
		Vector3 currentPosition = source;
		if (IsParabolicVelocity(velocity))
		{
			float sp = 12;
			if (distance <= 13)
				sp *= 4;
			
			float sec = Math.Min(4, distance / sp);
			currentPosition += (velocity * sec);
		}
		
		return currentPosition;
	}

	public static bool IsParabolicVelocity(Vector3 velocity)
	{
		return !(velocity.x == 0 && velocity.z == 0);
	}

	public static void StringWrite(string fileName, string Data)
	{
		FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamWriter myWriter = new StreamWriter(myFile);
		myWriter.Write(Data);
		myWriter.Close();
		myFile.Close();
	}
	
	public static string StringRead(string OpenFileName)
	{
		string InData = "";
		FileStream myFile = File.Open(OpenFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamReader myReader = new StreamReader(myFile);
		InData = myReader.ReadToEnd();
		myReader.Close();
		myFile.Close();
		return InData;
	}

	public static void ItemIdTranslateAvatar(ref TAvatar avatar, TItem[] items)
	{
		for (int i = 0; i < items.Length; i++) {
			int kind = GetItemKind(items[i].ID);
			int index = GetItemAvatarIndex(items[i].ID);
			
			switch(kind)		
			{
			case 0:
				avatar.Body = GameData.Team.Player.Avatar.Body;
				break;
				
			case 1:
				avatar.Hair = index;
				break;
				
			case 2:
				avatar.MHandDress = index;//手飾
				break;
				
			case 3:
				avatar.Cloth = index;//上身
				break;
				
			case 4:
				avatar.Pants = index;//下身
				break;
				
			case 5:
				avatar.Shoes = index;//鞋
				break;
				
			case 6:
				avatar.AHeadDress = index;//頭飾(共用）
				break;
				
			case 7:
				avatar.ZBackEquip = index;//背部(共用)
				break;
			}
		}
	}

	public static int GetTextureIndex(int avatarindex)
	{
		return avatarindex % 1000;
	}

	public static int GetItemKind(int id)
	{
		if (GameData.DItemData.ContainsKey (id)) {
			return	GameData.DItemData[id].Kind;
		} else {
			return -1;
		}
	}
	
	public static int GetItemAvatarIndex(int id)
	{
		if (GameData.DItemData.ContainsKey (id)) {
			return	GameData.DItemData[id].Avatar;
		}else
			return -1;
	}

	public static bool IsActiveSkill(int id)
	{
		if (id > GameConst.ID_LimitActive)
			return true;
		else
			return false;
	}

	public static int GetActiveSkillCount(TSkill[] skill)
	{
		int count = 0;
		for(int i = 0; i < skill.Length;i++)
		{
			if(IsActiveSkill(skill[i].ID))
				count++;
		}
		return count;
	}

	public static int GetPassiveSkillCount(TSkill[] skill)
	{
		int count = 0;
		for(int i = 0; i < skill.Length;i++)
		{
			if(!IsActiveSkill(skill[i].ID))
				count++;
		}
		return count;
	}

	public static int GetAttributeIndex(EAttribute att)
	{
		int index = -1;
		switch(att)
		{
		case EAttribute.Point2:
			index = 0;
			break;
		case EAttribute.Point3:
			index = 1;
			break;
		case EAttribute.Dunk:
			index = 2;
			break;
		case EAttribute.Rebound:
			index = 3;
			break;
		case EAttribute.Block:
			index = 4;
			break;
		case EAttribute.Steal:
			index = 5;
			break;
		case EAttribute.Speed:
			index = 6;
			break;
		case EAttribute.Stamina:
			index = 7;
			break;
		case EAttribute.Strength:
			index = 8;
			break;
		case EAttribute.Defence:
			index = 9;
			break;
		case EAttribute.Dribble:
			index = 10;
			break;
		case EAttribute.Pass:
			index = 11;
			break;
		}
		return index;
	}

	/// <summary>取得每一級potential需要多少點</summary>
	public static int GetPotentialRule(EAttribute kind)
	{
		if(GetAttributeIndex (kind) < GameConst.PotentialRule.Length)
			return GameConst.PotentialRule[GetAttributeIndex (kind)];
		else
			return -1;
	}

	/// <summary>屬性六角形</summary>
	public static void UpdateAttrHexagon(UIAttributes attHexagon, Dictionary<EAttribute, int> attData, int[] adds = null)
	{
		foreach (var item in attData) {
			int add = 0;

			if(adds != null)
				add = GetAttributeIndex(item.Key) < adds.Length? adds[GetAttributeIndex(item.Key)] : 0;

			switch(item.Key)
			{
			case EAttribute.Point2:
				attHexagon.SetValue(UIAttributes.EGroup.Point2, (item.Value + add) / GameConst.AttributeMax);
				break;
			case EAttribute.Point3:
				attHexagon.SetValue(UIAttributes.EGroup.Point3, (item.Value + add) / GameConst.AttributeMax);
				break;
			case EAttribute.Dunk:
				attHexagon.SetValue(UIAttributes.EGroup.Dunk, (item.Value + add) / GameConst.AttributeMax);
				break;
			case EAttribute.Rebound:
				attHexagon.SetValue(UIAttributes.EGroup.Rebound, (item.Value + add) / GameConst.AttributeMax);
				break;
			case EAttribute.Block:
				attHexagon.SetValue(UIAttributes.EGroup.Block, (item.Value + add) / GameConst.AttributeMax);
				break;
			case EAttribute.Steal:
				attHexagon.SetValue(UIAttributes.EGroup.Steal, item.Value + add / GameConst.AttributeMax);
				break;
			}
		}
	}

	/// <summary>總和 ：Dictionary + 數字陣列</summary>
	public static Dictionary<EAttribute, int> SumAttribute(Dictionary<EAttribute, int> attData , int[] adds)
	{
		int add = 0;
		Dictionary<EAttribute, int> sum = new Dictionary<EAttribute, int> ();
	
		foreach (var item in attData) {
			if(adds != null)
				add = GetAttributeIndex(item.Key) < adds.Length? adds[GetAttributeIndex(item.Key)] : 0 ;

			if(!sum.ContainsKey(item.Key))
				sum.Add(item.Key, item.Value + add);
			else
				sum[item.Key] = item.Value + add;
		}

		return sum;
	}
	
	/// <summary>單一player用了多少Lv點數</summary>
	public static int GetCurrentLvPotential(TPlayer player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;
		
		if(player.Potential != null)
		foreach (var item in GameData.Team.Player.Potential) {
			use += item.Value * GameConst.PotentialRule[GameFunction.GetAttributeIndex(item.Key)]; 
		}
		
		if (lvpoint > use)
			return lvpoint - use;
		else
			return 0;
	}

	/// <summary>單一player用了多少共用點數</summary>
	public static int GetUseAvatarPotential(TPlayer player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;
		
		if (player.Potential != null)
			use = GameFunction.GetUseTotalPotential (player.Potential);
		
		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}

	/// <summary>單一player用了多少共用點數</summary>
	public static int GetUseAvatarPotential(TPlayerBank player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;
		
		if (player.Potential != null)
			use = GameFunction.GetUseTotalPotential (player.Potential);
		
		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}

	/// <summary>單一player總共使用potential點數</summary>
	public static int GetUseTotalPotential(Dictionary<EAttribute, int> potential)
	{
		int count = 0;
		
		foreach (var item in potential) {
			count += item.Value * GameConst.PotentialRule[GameFunction.GetAttributeIndex(item.Key)];
		}
		
		return count;
	}

	/// <summary>全部player用了多少共用點數</summary>
	public static int GetAllPlayerTotalUseAvatarPotential()
	{
		int use = 0;
		if (GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 1) {
			for(int i = 0;i< GameData.Team.PlayerBank.Length; i++){
				if(GameData.Team.PlayerBank[i].RoleIndex != GameData.Team.Player.RoleIndex){
					use += GameFunction.GetUseAvatarPotential(GameData.Team.PlayerBank[i]);
				}
			}
		}
		
		use += GameFunction.GetUseAvatarPotential (GameData.Team.Player);
		
		return GameData.Team.AvatarPotential - use;
	}

	/// <summary>以等級查看會獲得多少點數</summary>
	public static int GetLvPotential(int lv)
	{
		return (lv - 1) * GameConst.PreLvPotential;
	}


	
}
