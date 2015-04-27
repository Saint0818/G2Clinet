/// This shader was altered from the shader available here: http://wiki.unity3d.com/index.php?title=DepthMask

Shader "ArmoireCannonball/SplitscreenMask" 
{ 
	SubShader 
	{
		// Apply this shader before drawing geometry
		Tags { "Queue" = "Geometry-500" }
 
		ColorMask 0
		ZWrite On
		Pass 
		{
			// Intentionally blank
		}
	}
}