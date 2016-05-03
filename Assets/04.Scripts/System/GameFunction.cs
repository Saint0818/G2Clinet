﻿using System;
using System.IO;
using GameStruct;
using UnityEngine;
using System.Collections.Generic;

public static class GameFunction
{
    private static byte[] mFlagAy = {1, 2, 4, 8, 16, 32, 64, 128};

    public static bool CheckByteFlag(int No, params byte[] FlagAy)
    {
        if ((No > 0) && (No <= FlagAy.Length * 8))
        {
            int i = (ushort)((No - 1) / 8);
            int j = (ushort)(((No - 1) % 8) + 1);
            byte Value = mFlagAy [j - 1];
            if (i >= 0 && i <= FlagAy.Length - 1)
            if ((FlagAy [i] & Value) == Value)
                return true;
        }

        return false;
    }
    
    public static void Add_ByteFlag(int No, ref byte[] FlagAy)
    {
        if (!CheckByteFlag(No, FlagAy))
        {
            int i = (No - 1) / 8 + 1;
            int j = (No - 1) % 8 + 1;
            byte Value = mFlagAy [j - 1];
            FlagAy [i - 1] = (byte)(FlagAy [i - 1] | Value);
        }
    }
    
    public static void Del_ByteFlag(int No, ref byte[] FlagAy)
    {
        if (CheckByteFlag(No, FlagAy))
        {
            int i = (No - 1) / 8 + 1;
            int j = (No - 1) % 8 + 1;
            byte Value = mFlagAy [j - 1];
            FlagAy [i - 1] = (byte)(FlagAy [i - 1] ^ Value);
        }
    }

    public static Vector3 CalculateNextPosition(Vector3 source, Vector3 velocity, float time)
    {
        if (IsParabolicVelocity(velocity))
        {
            return source + (velocity * time) + (0.5f * Physics.gravity * time * time);
        } else 
            return Vector3.zero;
    }

//  public static float ElevationAngle(Vector3 source, Vector3 target)
//  {
//      // find the cannon->target vector:
//      Vector3 dir = target - source;
//      // create a horizontal version of it:
//      Vector3 dirH = new Vector3(dir.x, 0, dir.y);
//      // measure the unsigned angle between them:
//      float ang = Vector3.Angle(dir, dirH);
//      // add the signal (negative is below the cannon):
//      if (dir.y < 0)
//      { 
//          ang = -ang;
//      }
//      
//      return ang;
//  }

//    /// <summary>
//    /// target 是否在 source +Z 軸的扇形範圍內.
//    /// </summary>
//    /// <param name="source"></param>
//    /// <param name="target"></param>
//    /// <param name="fanDis"></param>
//    /// <param name="fanAngle"></param>
//    /// <returns> true: target 在 source 的扇形範圍內. </returns>
//    public static bool IsInFanArea([NotNull] Transform source, Vector3 target, float fanDis, float fanAngle)
//  {
////        Vector3 relative = source.InverseTransformPoint(target);
////        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
//      var betweenAngle = MathUtils.FindAngle(source, target);
//        var betweenDis = MathUtils.Find2DDis(source.position, target);
//
//      return Mathf.Abs(betweenAngle) <= fanAngle * 0.5f && betweenDis <= fanDis;
//  }

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
        for (int i = 0; i < items.Length; i++)
        {
//            int kind = GetItemKind(items [i].ID);
            int index = GetItemAvatarIndex(items [i].ID);

			if (i > 0 && index <= 0)
            {
                if (i != 2 && i < 6)
                    index = 99001;
                else
                    index = 0;
            }
            
            switch (i)
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
        return GameData.DItemData.ContainsKey(id) ? GameData.DItemData[id].Kind : -1;
    }

    public static int GetItemAvatarIndex(int id)
    {
        if (GameData.DItemData.ContainsKey(id))
        {
            return  GameData.DItemData [id].Avatar;
        } else
            return -1;
    }

    public static bool IsActiveSkill(int id)
    {
        if (id >= GameConst.ID_LimitActive)
            return true;
        else
            return false;
    }

    public static int GetActiveSkillCount(TSkill[] skill)
    {
        int count = 0;
        for (int i = 0; i < skill.Length; i++)
        {
            if (IsActiveSkill(skill [i].ID))
                count++;
        }
        return count;
    }

    public static int GetPassiveSkillCount(TSkill[] skill)
    {
        int count = 0;
        for (int i = 0; i < skill.Length; i++)
        {
            if (!IsActiveSkill(skill [i].ID))
                count++;
        }
        return count;
    }

    public static int GetEBounsIndexByAttribute(EAttribute att)
    {
        switch (att)
        {
            case EAttribute.Point2:
                return 1;
            case EAttribute.Point3:
                return 2;
            case EAttribute.Speed:
                return 3;
            case EAttribute.Stamina:
                return 4;
            case EAttribute.Strength:
                return 5;
            case EAttribute.Dunk:
                return 6;
            case EAttribute.Rebound:
                return 7;
            case EAttribute.Block:
                return 8;
            case EAttribute.Defence:
                return 9;
            case EAttribute.Steal:
                return 10;
            case EAttribute.Dribble:
                return 11;
            case EAttribute.Pass:
                return 12;
            default:
                return 0;
        }
    }

    public static int GetAttributeIndex(EAttribute att)
    {
        int index = -1;
        switch (att)
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
            case EAttribute.Stamina:
                index = 6;
                break;
            case EAttribute.Defence:
                index = 7;
                break;
            case EAttribute.Dribble:
                index = 8;
                break;
            case EAttribute.Pass:
                index = 9;
                break;
            case EAttribute.Speed:
                index = 10;
                break;
            case EAttribute.Strength:
                index = 11;
                break;
        }
        return index;
    }

    public static EAttribute GetAttributeKind(int index)
    {
        EAttribute att = EAttribute.Point2;

        switch (index)
        {
            case 0:
                att = EAttribute.Point2;
                break;

            case 1:
                att = EAttribute.Point3;
                break;

            case 2:
                att = EAttribute.Dunk;
                break;

            case 3:
                att = EAttribute.Rebound;
                break;

            case 4:
                att = EAttribute.Block;
                break;

            case 5:
                att = EAttribute.Steal;
                break;

			case 6:
				att = EAttribute.Speed;
				break;

            case 7:
                att = EAttribute.Stamina;
                break;

			case 8:
				att = EAttribute.Strength;
				break;

            case 9:
                att = EAttribute.Defence;
                break;
            case 10:
                att = EAttribute.Dribble;
                break;
            case 11:
                att = EAttribute.Pass;
                break;
        }
        return att;
    }

    public static void UpdateAttrHexagon(UIAttributes attHexagon, TPlayer player, int[] potentialAdds = null)
    {
        float add;

		if (player.Potential.Count == 0) {
			for (int i = 0; i < GameConst.PotentialCount; i++) {
				EAttribute kind = GetAttributeKind(i);
				SetAttrHexagon (ref attHexagon, kind, 0, player);
			}
		} else {
			foreach (var item in player.Potential)
			{
				add = 0;

				if (potentialAdds != null)
				{
					add = GetAttributeIndex(item.Key) < potentialAdds.Length ? potentialAdds [GetAttributeIndex(item.Key)] : 0;
					SetAttrHexagon (ref attHexagon, item.Key, add, player);
				}
				else
					SetAttrHexagon (ref attHexagon, item.Key, 0, player);					
			}			
		}
    }

	private static void SetAttrHexagon(ref UIAttributes attHexagon, EAttribute att, float add, TPlayer player)
	{
		switch (att)
		{
		case EAttribute.Point2:
			attHexagon.SetValue(UIAttributes.EGroup.Point2, (player.Point2 + add) / GameConst.AttributeMax);
				break;
		case EAttribute.Point3:
			attHexagon.SetValue(UIAttributes.EGroup.Point3, (player.Point3 + add) / GameConst.AttributeMax);
				break;
		case EAttribute.Dunk:
			attHexagon.SetValue(UIAttributes.EGroup.Dunk, (player.Dunk + add) / GameConst.AttributeMax);
				break;
		case EAttribute.Rebound:
			attHexagon.SetValue(UIAttributes.EGroup.Rebound, (player.Rebound + add) / GameConst.AttributeMax);
				break;
		case EAttribute.Block:
			attHexagon.SetValue(UIAttributes.EGroup.Block, (player.Block + add) / GameConst.AttributeMax);
				break;
		case EAttribute.Steal:
			attHexagon.SetValue(UIAttributes.EGroup.Steal, (player.Steal + add) / GameConst.AttributeMax);
				break;
		}	
	}

    /// <summary>屬性六角形</summary>
    public static void UpdateAttrHexagon(UIAttributes attHexagon, Dictionary<EAttribute, int> attData, int[] adds = null)
    {
        foreach (var item in attData)
        {
            int add = 0;

            if (adds != null)
                add = GetAttributeIndex(item.Key) < adds.Length ? adds [GetAttributeIndex(item.Key)] : 0;

            switch (item.Key)
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
    public static Dictionary<EAttribute, int> SumAttribute(Dictionary<EAttribute, int> attData, int[] adds)
    {
        int add = 0;
        Dictionary<EAttribute, int> sum = new Dictionary<EAttribute, int>();
    
        foreach (var item in attData)
        {
            if (adds != null)
                add = GetAttributeIndex(item.Key) < adds.Length ? adds [GetAttributeIndex(item.Key)] : 0;

            if (!sum.ContainsKey(item.Key))
                sum.Add(item.Key, item.Value + add);
            else
                sum [item.Key] = item.Value + add;
        }

        return sum;
    }
    
    /// <summary>單一player用了多少Lv點數</summary>
    public static int GetCurrentLvPotential(TPlayer player)
    {
        int lvpoint = GetLvPotential(player.Lv);
        int use = 0;
        
        if (player.Potential != null)
            foreach (var item in GameData.Team.Player.Potential)
            {
                use += item.Value * GetPotentialRule(player.BodyType, GameFunction.GetAttributeIndex(item.Key));
            }
        
        if (lvpoint > use)
            return lvpoint - use;
        else
            return 0;
    }

    /// <summary>單一player用了多少共用點數</summary>
    public static int GetUseAvatarPotential(TPlayer player)
    {
        int lvpoint = GetLvPotential(player.Lv);
        int use = 0;
        
        if (player.Potential != null)
            use = GameFunction.GetUseTotalPotential(player.Potential, player.BodyType);
        
        if (use > lvpoint)
            return use - lvpoint;
        else
            return 0;
    }

    /// <summary>單一player用了多少共用點數</summary>
    public static int GetUseAvatarPotential(TPlayerBank player)
    {
        int lvpoint = GetLvPotential(player.Lv);
        int use = 0;

        if (GameData.DPlayers.ContainsKey(player.ID))
        {
            if (player.Potential != null)
            {
                use = GameFunction.GetUseTotalPotential(player.Potential, GameData.DPlayers[player.ID].BodyType);
            }
        }
        else
        {
            Debug.LogError("Can't found Player.ID : " + player.ID);
        }

        if (use > lvpoint)
            return use - lvpoint;
        else
            return 0;
    }

    /// <summary>單一player總共使用potential點數</summary>
    public static int GetUseTotalPotential(Dictionary<EAttribute, int> potential, int bodytype)
    {
        int count = 0;
        
        foreach (var item in potential)
        {
            count += item.Value * GetPotentialRule(bodytype, item.Key);
        }
        
        return count;
    }

    /// <summary>還剩下多少可用的AvatarPotential</summary>
    public static int GetAllPlayerTotalUseAvatarPotential()
    {
        int use = 0;
        if (GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 1)
        {
            for (int i = 0; i< GameData.Team.PlayerBank.Length; i++)
            {
                if (GameData.Team.PlayerBank [i].RoleIndex != GameData.Team.Player.RoleIndex)
                {
                    use += GameFunction.GetUseAvatarPotential(GameData.Team.PlayerBank [i]);
                }
            }
        }
        
        use += GameFunction.GetUseAvatarPotential(GameData.Team.Player);
        
        return GameData.Team.AvatarPotential - use;
    }

    /// <summary>以等級查看會獲得多少點數</summary>
    public static int GetLvPotential(int lv)
    {
        return (lv - 1) * GameConst.PreLvPotential;
    }

	public static float GetKindFormula(int index, float value)
	{
		float result = 0;
		switch (index)
		{
		case 1:
			result = value * 0.5f;
			break;
		case 2:
			result = value * 0.5f;
			break;
		}
		return result;
	}

	public static string GetStringExplain (string explain ,int id, int lv) {
		float six = 0;
		float seven = 0;
		float eight = 0;
		float nine = 0;
		float ten = 0;
		float eleven = 0;
		float value =  0;
		if(GameData.DSkillData.ContainsKey(id)) {
			value = GameData.DSkillData[id].Value(lv);
			switch(GameData.DSkillData[id].AttrKind) {
			case 1:
				six = GetAttributeFormula(EPlayerAttributeRate.Point2Rate, value);
				break;
			case 2:
				six = GetAttributeFormula(EPlayerAttributeRate.Point3Rate, value);
				break;
			case 5:
				seven = GetAttributeFormula(EPlayerAttributeRate.ElbowingRate, value);
				eight = GetAttributeFormula(EPlayerAttributeRate.StrengthRate, value);
				nine = GetAttributeFormula(EPlayerAttributeRate.BlockPushRate, value);
				break;
			case 6:
				seven = GetAttributeFormula(EPlayerAttributeRate.DunkRate, value);
				eight = GetAttributeFormula(EPlayerAttributeRate.AlleyOopRate, value);
				nine = GetAttributeFormula(EPlayerAttributeRate.TipInRate, value);
				break;
			case 7:
				seven = GetAttributeFormula(EPlayerAttributeRate.TipInRate, value);
				ten = GetAttributeFormula(EPlayerAttributeRate.ReboundHeadDistance, value);
				eleven = GetAttributeFormula(EPlayerAttributeRate.ReboundHandDistance, value);
				break;
			case 8:
				seven = GetAttributeFormula(EPlayerAttributeRate.BlockRate, value);
				eight = GetAttributeFormula(EPlayerAttributeRate.FakeBlockrate, value);
				ten = GetAttributeFormula(EPlayerAttributeRate.BlockDistance, value);
				break;
			case 9:
				seven = GetAttributeFormula(EPlayerAttributeRate.PushingRate, value);
				eleven = GetAttributeFormula(EPlayerAttributeRate.PushingRate, value);
				ten = GetAttributeFormula(EPlayerAttributeRate.DefDistance, value);
				break;
			case 10:
				seven = GetAttributeFormula(EPlayerAttributeRate.StealRate, value);
				break;
			case 12:
				six = GetAttributeFormula(EPlayerAttributeRate.PassRate, value);
				seven = GetAttributeFormula(EPlayerAttributeRate.AlleyOopPassRate, value);
				break;
			}
			return string.Format(explain, 
				GameData.DSkillData[id].MaxAnger(lv),//0
				GameData.DSkillData[id].Rate(lv),//1
				GameData.DSkillData[id].Distance(lv),//2
				TextConst.S( 3301 + GameData.DSkillData[id].AttrKind),//3
				GameData.DSkillData[id].Value(lv),//4
				GameData.DSkillData[id].LifeTime(lv),//5
				six,//6
				seven,//7
				eight,//8
				nine,//9
				ten,//10
				eleven//11
			);
		}
		return "";

	}

	public static float GetAttributeFormula (EPlayerAttributeRate attr, float value) {
		switch (attr) {
		case EPlayerAttributeRate.Point2Rate:
		case EPlayerAttributeRate.Point3Rate:
		case EPlayerAttributeRate.BlockPushRate:
		case EPlayerAttributeRate.StealExtraAngle:
		case EPlayerAttributeRate.PushExtraAngle:
		case EPlayerAttributeRate.ElbowExtraAngle:
			return value * 0.5f;
		case EPlayerAttributeRate.SpeedValue:
			return value * 0.003f;
		case EPlayerAttributeRate.StaminaValue:
			return value * 1f;
		case EPlayerAttributeRate.PushingRate:
			return value * 0.8f;
		case EPlayerAttributeRate.StrengthRate:
		case EPlayerAttributeRate.DunkRate:
		case EPlayerAttributeRate.TipInRate:
		case EPlayerAttributeRate.ReboundRate:
		case EPlayerAttributeRate.BlockRate:
			return value * 0.9f;
		case EPlayerAttributeRate.ElbowingRate:
		case EPlayerAttributeRate.AlleyOopRate:
		case EPlayerAttributeRate.AlleyOopPassRate:
			return value * 0.6f;
		case EPlayerAttributeRate.ReboundHeadDistance:
		case EPlayerAttributeRate.StealDistance:
		case EPlayerAttributeRate.PushDistance:
		case EPlayerAttributeRate.ElbowDistance:
			return value * 0.005f;
		case EPlayerAttributeRate.ReboundHandDistance:
			return value * 0.01f;
		case EPlayerAttributeRate.FakeBlockrate:
			return (100 - (value / 1.15f));
		case EPlayerAttributeRate.BlockDistance:
		case EPlayerAttributeRate.DefDistance:
			return value * 0.1f;
		case EPlayerAttributeRate.StealRate:
			return value * 0.9f;
		case EPlayerAttributeRate.PassRate:
			return value * 0.7f;
		}
		return 0;
	}

	public static void InitDefaultText(GameObject obj) {
		UILabel[] labs = obj.GetComponentsInChildren<UILabel>();
		for (int i = 0; i < labs.Length; i++) {
			int id = 0;
			if (!string.IsNullOrEmpty(labs[i].text) && int.TryParse(labs[i].text, out id) && TextConst.HasText(id))
				labs[i].text = TextConst.S(id);
		}
	}

	public static string GetHintText(int index, int value, int id)
	{
		int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
		return TextConst.S(baseValue);
	}

	public static bool CheckItemIsExpired(DateTime usetime)
	{
        return usetime.CompareTo(DateTime.UtcNow) < 0;
	}

	public static void FindTalkManID(int tutorialID, ref int[] manay) {
		if (manay != null && manay.Length >= 2) {
			manay[0] = 0;
			manay[1] = 0;
			
			foreach (KeyValuePair<int, TTutorial> item in GameData.DTutorial) {
				if (item.Value.ID == tutorialID) {
					if (manay[0] == 0 && item.Value.TalkL != 0)
						manay[0] = item.Value.TalkL;
					
					if (manay[1] == 0 && item.Value.TalkR != 0)
						manay[1] = item.Value.TalkR;
					
					if (manay[0] != 0 && manay[1] != 0)
						return;
				}
			}
		}
	}

	public static void ShowInlay (ref GameObject[] emptyStars, ref  GameObject[] inlays, TPlayer player, int itemDataKind)
    {
        if(player.ValueItems == null)
            return;

		foreach (KeyValuePair<int, TValueItem> pair in player.ValueItems)
        {
			if (GameData.DItemData.ContainsKey(pair.Value.ID) && pair.Key == itemDataKind) {
				for(int i=0; i<inlays.Length; i++) {
					if(i < pair.Value.RevisionInlayItemIDs.Length) {
						inlays[i].SetActive((pair.Value.RevisionInlayItemIDs[i] != 0));
						emptyStars[i].SetActive(true);
					} else {
						inlays[i].SetActive(false);
						emptyStars[i].SetActive(false);
					}
				}
			}
		}
	}

	public static void ShowStar (ref SkillCardStar[] stars, int lv, int quality, int max, int sizeX = 30) {
		for (int i=0; i<stars.Length; i++) {
			if(i < lv) stars[i].ShowStar();
			else stars[i].HideStar();

			if(i <= (max -1)) stars[i].Show();
			else stars[i].Hide();

			stars[i].SetQuality(quality);
			stars[i].transform.localPosition = new Vector3(-(sizeX * 0.5f) * (max -1)+ i * sizeX, 0, 0);
		}
	}

	public static void ShowStar_Item (ref SkillCardStar[] stars, int lv, int quality, int max, int sizeX = 30) {
		for (int i=0; i<stars.Length; i++) {
			if(i < lv) stars[i].ShowStar();
			else stars[i].HideStar();

			if(i <= (max -1)) stars[i].Show();
			else stars[i].Hide();

			stars[i].SetQuality(quality);
			stars[i].gameObject.transform.localPosition = new Vector3(-145 + i * sizeX, 0, 0);
		}
	}

	public static string QualityName (int quality) {
		switch(quality) {
		case 1:
			return TextConst.S(7216);
		case 2:
			return TextConst.S(7217);
		case 3:
			return TextConst.S(7218);
		case 4:
			return TextConst.S(7219);
		case 5:
			return TextConst.S(7220);
		default:
			return TextConst.S(7216);
		}
	}

	public static string QualityNameSkill (int quality) {
		switch(quality) {
		case 1:
			return TextConst.S(4601);
		case 2:
			return TextConst.S(4602);
		case 3:
			return TextConst.S(4603);
		case 4:
			return TextConst.S(4604);
		case 5:
			return TextConst.S(4605);
		default:
			return TextConst.S(4601);
		}
	}

    public static TTeamRank TTeamCoverTTeamRank(TTeam team)
    {
        TTeamRank result = new TTeamRank();
        result.Team = team;
//        result.Player = team.Player;
//        result.PVPIntegral = team.PVPIntegral;
        //TODO : Guild
//        result.GuildIName = team.LeagueName;
//        result.GuildIIcon= team.LeagueIcon;
//		result.LifetimeRecord = team.LifetimeRecord;
        return result;
	}

	public static int GetPVPLv (int integral) {
		if (GameData.DPVPData.ContainsKey(GameConst.PVPMinLv) && integral < GameData.DPVPData[GameConst.PVPMinLv].LowScore)
			return GameConst.PVPMinLv;
		else 
			if (GameData.DPVPData.ContainsKey(GameConst.PVPMaxLv) && integral > GameData.DPVPData[GameConst.PVPMaxLv].HighScore)
				return GameConst.PVPMaxLv;
			else {
				int lv = 1;

				foreach (KeyValuePair<int, TPVPData> item in GameData.DPVPData) {
					if (integral >= item.Value.LowScore && integral <= item.Value.HighScore) {
						lv = item.Value.Lv;
						break;
					}
				}

				return lv;
			}
	}

	public static Transform FindQualityBG (Transform obj) {
		Transform t = obj.FindChild("QualityBG");
		if(t == null)
			t = obj.FindChild("ItemView/QualitySquare/QualityBG");

		if(t == null)
			t = obj.FindChild("ItemView/QualityOctagon/QualityBG");

		return t;
	}

    public static string SpendKindTexture(int kind) {
        switch (kind) {
            case 0: return "Icon_Gem";
            case 1: return "Icon_Coin";
            case 2: return "Icon_TokenPvp";
            case 3: return "Icon_TokenSocial";
            default: return "Icon_EXP";
        }
    }

    public static string GetTimeString(TimeSpan time)
    {
        if(time.TotalDays > 1)
            return string.Format("{0} Day", time.Days);
        else if(time.TotalHours > 1 && time.TotalDays < 1)
            return string.Format("{0}H {1}M", time.Hours, time.Minutes);
        else
            return string.Format("{0}M {1}S", time.Minutes, time.Seconds);
    }

	public static int GetSkillLevel (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			return Mathf.Clamp(skill.Lv, 0, GameData.DSkillData[skill.ID].MaxStar);
		}
		return 0;
	}

	public static string CardLevelName (int skillID) {
		if(GameData.DSkillData.ContainsKey(skillID))
			return "cardlevel_" + GameData.DSkillData[skillID].Quality.ToString();
		else 
			return "cardlevel_1";
	}

	public static string CardLevelBallName (int skillID) {
		if(GameData.DSkillData.ContainsKey(skillID))
			return "Levelball" + GameData.DSkillData[skillID].Quality.ToString();
		else 
			return "Levelball1";
	}

	public static string CardSuitItemBg (int skillID) {
		if(GameData.DSkillData.ContainsKey(skillID))
			return "SuitItemBg" + GameData.DSkillData[skillID].Quality.ToString();
		else 
			return "SuitItemBg1";
	}

	public static string CardSuitItemStarBg (int count) {
		if(count >= 5 && count <= 7)
			return "SuitItem" + count.ToString() + "LBg";
		else
			return "SuitItem7LBg";
	}

	public static void CardSuitItemStar (ref GameObject[] stars, int count, int current) {
		for(int i=0; i<stars.Length; i++)
			stars[i].SetActive(false);

		for (int i=0; i<stars.Length; i++) {
			if(count == 5) {//0~4
				if(current >= 0 && current <= 5 && i < current) 
					stars[i].SetActive(true);
				
			} else if(count == 6) {//1~6
				if(current >= 1 && current <= 6 && i >= 1 && i <= 6) {
					if(i <= current)
						stars[i].SetActive(true);
				}
			} else if (count == 7) {
				if(current >= 0 && current <= 7 && i < current)
					stars[i].SetActive(true);
			}
		}
	}

	public static string CardSuitLightName (int count) {
		if(count > 0  && count <= 3)
			return "SuitLight" + count.ToString();
		else 
			return "SuitLight0";
	}

	public static string PositionIcon (int bodytype) {
		switch (bodytype)
		{
		case 1: 
			return "IconForward";
		case 2:
			return "IconGuard";
		default:
			return "IconCenter";  
		}
	}

	public static string PVPRankIconName (int level) {
		return "IconRank" + level.ToString();
	}

    public static bool CanGetPVPReward(ref TTeam team)
    {
        if (GameData.DPVPData.ContainsKey(GameConst.PVPMinLv) && 
            team.DailyCount.PVPReaward == 0 && 
            team.PVPIntegral >= GameData.DPVPData[GameConst.PVPMinLv].LowScore && 
            team.Player.Lv >= LimitTable.Ins.GetLv(GameEnum.EOpenID.PVP))
            return true;
        else
            return false;
    } 

	//Distance 為 -1, 就不需要計算衰退值
	public static float ShootingCalculate (PlayerBehaviour player, float originalRate, float scoreRate, float extraScoreRate, float distance = -1) {
		float beginRate = 0;
		float decayRate = 0;
		float wristRate = 0;
		//1 動作影響的機率
		beginRate = Mathf.Max (0, (originalRate + scoreRate));


		if(distance >= 5) {
			//  (5~10)命中衰減率 = [1-((D-4)*0.06)]   
			//  (11~) 命中衰減率 = [1-((D-4)*D*0.008)]
			if(distance >= 5 && distance <= 11)
				decayRate = (1 - ((distance - 4) * 0.06f));
			else if(distance > 11) 
				decayRate = (1 - ((distance - 4) * distance * 0.008f));
			//腕力加成 (1-命中衰減率)＊(腕力＊比重換算 * 100%)->PlayerBehaviour已換算
			wristRate = (1 - decayRate) * player.Attr.PointRate3 * 0.01f;
		}

		if(decayRate == 0) 
			return Mathf.Max(2, (beginRate + extraScoreRate));
		else
			return Mathf.Max(2, (beginRate * (decayRate + wristRate) + extraScoreRate));
	}

    public static int GetPotentialRule(int bodytype, int index)
    {
        if (GameData.DPotential.ContainsKey(bodytype))
        {
            switch (index)
            {
                case 0:
                    return GameData.DPotential[bodytype].Point2;
                case 1:
                    return GameData.DPotential[bodytype].Point3;
                case 2:
                    return GameData.DPotential[bodytype].Dunk;
                case 3:
                    return GameData.DPotential[bodytype].Rebound;
                case 4:
                    return GameData.DPotential[bodytype].Block;
                case 5:
                    return GameData.DPotential[bodytype].Steal;
            }
        }

        return -1;
    }

    /// <summary>取得每一級potential需要多少點</summary>
    public static int GetPotentialRule(int boydkind, EAttribute kind)
    {
        if (GetAttributeIndex(kind) < GameConst.PotentialCount)
        {
            return GetPotentialRule(boydkind, GetAttributeIndex(kind));
        }
        else
            return -1;
    }

	public static float GetPercent (float value, float min, float max ) {
		if(value > min) 
			return (value - min) / max;
		else
			return 0;
	}

	//Array從０開始
	public static string GetBuildName (int index) {
		return TextConst.S(11001 + (index + 1) * 100);
	}

	public static int GetBuildItemID (int index, int type) {
		return 50000 + (index + 1) * 1000 + type;
	}

	public static int GetBuildLv (int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length) {
			return GameData.Team.GymBuild[index].LV;
		}
		return 1;
	}

	public static DateTime GetBuildTime (int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length) {
			return GameData.Team.GymBuild[index].Time;
		}
		return DateTime.UtcNow;
	}

	///	Basket = 0, Advertisement = 1, Store = 2,
	///	Gym = 3, Door = 4, Logo = 5,
	///	Chair = 6, Calendar = 7, Mail = 8
	public static DateTime GetOriTime (int index, int lv, DateTime time) {
		if(GameData.DArchitectureExp.ContainsKey(lv)) {
			switch(index) {
			case 0:
				return time.AddHours(-GameData.DArchitectureExp[lv].BasketTime).ToUniversalTime();
			case 1:
				return time.AddHours(-GameData.DArchitectureExp[lv].AdTime).ToUniversalTime();
			case 2:
				return time.AddHours(-GameData.DArchitectureExp[lv].StoreTime).ToUniversalTime();
			case 3:
				return time.AddHours(-GameData.DArchitectureExp[lv].GymTime).ToUniversalTime();
			case 4:
				return time.AddHours(-GameData.DArchitectureExp[lv].DoorTime).ToUniversalTime();
			case 5:
				return time.AddHours(-GameData.DArchitectureExp[lv].LogoTime).ToUniversalTime();
			case 6:
				return time.AddHours(-GameData.DArchitectureExp[lv].ChairTime).ToUniversalTime();
			case 7:
				return time.AddHours(-GameData.DArchitectureExp[lv].CalendarTime).ToUniversalTime();
			case 8:
				return time.AddHours(-GameData.DArchitectureExp[lv].MailTime).ToUniversalTime();
			}
		}
		return DateTime.UtcNow;
	}

	public static int GetIdleQueue {
		get {
			int count = 0;
			for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
				if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == -1)
					count ++;
			return count;
		}
	}

	public static bool IsBuildQueue (int buildIndex) {
		for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
			if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == buildIndex)
				return true;

		return false;
	}

	public static string GetBuildEnName (int index) {
		switch (index) {
		case 0:
			return "Basket";
		case 1:
			return "Advertisement";
		case 2:
			return "Store";
		case 3:
			return "Gym";
		case 4:
			return "Door";
		case 5:
			return "Logo";
		case 6:
			return "Chair";
		case 7:
			return "Calendar";
		case 8:
			return "Mail";
		}
		return "";
	}

	public static string GetBuildSpriteName (int index) {
		switch (index) {
		case -2:
			return "Icon_lock";
		case -1:
			return "Icon_court";
		case 0:
			return "Icon_Basket";
		case 1:
			return "Icon_Advertisement";
		case 2:
			return "Icon_Store";
		case 3:
			return "Icon_Gym";
		case 4:
			return "Icon_Door";
		case 5:
			return "Icon_Logo";
		case 6:
			return "Icon_Chair";
		case 7:
			return "Icon_Calendar";
		case 8:
			return "Icon_Mail";
		}
		return "";
	}

	public static int GetUnlockNumber (int openid) {
		return 1500 + openid;
	}
}
