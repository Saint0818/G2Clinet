﻿using UnityEngine;
using System.Collections;
using System;

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

	public static float ElevationAngle(Vector3 source, Vector3 target)
	{
		// find the cannon->target vector:
		Vector3 dir = target - source;
		// create a horizontal version of it:
		Vector3 dirH = new Vector3(dir.x, 0, dir.y);
		// measure the unsigned angle between them:
		float ang = Vector3.Angle(dir, dirH);
		// add the signal (negative is below the cannon):
		if (dir.y < 0)
		{ 
			ang = -ang;
		}
		
		return ang;
	}

	public static Vector3 GetVelocity(Vector3 source, Vector3 target, float angle)
	{
		try
		{
			Vector3 dir = target - source;  // get target direction
			float h = dir.y;  // get height difference
			dir.y = 0;  // retain only the horizontal direction
			float dist = dir.magnitude;  // get horizontal distance
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
}
