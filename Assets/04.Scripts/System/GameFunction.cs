using UnityEngine;
using System.Collections;

public static class GameFunction
{
	private static byte[] mFlagAy = {1, 2, 4, 8, 16, 32, 64, 128};

	public static bool CheckByteFlag(int No, params byte[] FlagAy){
		if((No > 0) && (No <= FlagAy.Length * 8))
		{
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
		if(!CheckByteFlag(No, FlagAy))
		{
			int i = (No - 1) / 8 + 1;
			int j = (No - 1) % 8 + 1;
			byte Value = mFlagAy[j - 1];
			FlagAy[i - 1] = (byte)(FlagAy[i - 1] | Value);
		}
	}
	
	public static void Del_ByteFlag(int No, ref byte[] FlagAy){
		if(CheckByteFlag(No, FlagAy))
		{
			int i = (No - 1) / 8 + 1;
			int j = (No - 1) % 8 + 1;
			byte Value = mFlagAy[j - 1];
			FlagAy[i - 1] = (byte)(FlagAy[i - 1] ^ Value);
		}
	}
}
