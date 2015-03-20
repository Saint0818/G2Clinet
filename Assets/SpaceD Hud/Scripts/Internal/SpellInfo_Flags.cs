using UnityEngine;
using System;

[System.Flags]
public enum SpellInfo_Flags
{
	Passive = (1 << 0),
	InstantCast = (1 << 1),
	PowerCostInPct = (1 << 2),
}

public static class SpellInfo_FlagsExtensions
{
	public static bool Has(this SpellInfo_Flags type, SpellInfo_Flags value)
	{
		try {
			return (((int)type & (int)value) == (int)value);
		} 
		catch {
			return false;
		}
	}
	
	public static bool Is(this SpellInfo_Flags type, SpellInfo_Flags value)
	{
		try {
			return (int)type == (int)value;
		}
		catch {
			return false;
		}    
	}

	public static SpellInfo_Flags Add(this SpellInfo_Flags type, SpellInfo_Flags value)
	{
		try {
			return (SpellInfo_Flags)(((int)type | (int)value));
		}
		catch(Exception ex)
		{
			throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(SpellInfo_Flags).Name), ex);
		}    
	}

	public static SpellInfo_Flags Remove(this SpellInfo_Flags type, SpellInfo_Flags value)
	{
		try {
			return (SpellInfo_Flags)(((int)type & ~(int)value));
		}
		catch (Exception ex)
		{
			throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(SpellInfo_Flags).Name), ex);
		}  
	}
}