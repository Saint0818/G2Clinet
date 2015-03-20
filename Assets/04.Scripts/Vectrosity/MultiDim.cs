using UnityEngine;

public class MultiDim {	
	public static int[][] JaggedInt (int a) {
		return new int[a][];
	}

	public static float[][] JaggedFloat (int a) {
		return new float[a][];
	}
	
	public static string[][] JaggedString (int a) {
		return new string[a][];
	}
	
	public static Vector3[][] JaggedVector3 (int a) {
		return new Vector3[a][];
	}
	
	public static Vector2[][] JaggedVector2 (int a) {
		return new Vector2[a][];
	}
}