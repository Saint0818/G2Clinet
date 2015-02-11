using UnityEngine;
using System.Collections;
using System;

public static class GameStruct
{
	public struct TPlayerAttribute
	{	
		public int Face;
		public int Cloth;
		public int Shoes;
		
		public TPlayerAttribute (int i)
		{
			Face = 0;
			Cloth = 0;
			Shoes = 0;
		}
	}
}
