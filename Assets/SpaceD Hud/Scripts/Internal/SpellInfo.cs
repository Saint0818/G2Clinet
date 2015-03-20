using UnityEngine;
using System;

[System.Serializable]
public class SpellInfo
{
	public int ID;
	public string Name;
	public Texture Icon;
	public string Description;
	public float Range;
	public float Cooldown;
	public float CastTime;
	public float PowerCost;

	[BitMask(typeof(SpellInfo_Flags))]
	public SpellInfo_Flags Flags;
}