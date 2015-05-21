using UnityEngine;
using System;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Glitch")]
public class CC_Glitch : CC_Base
{
	public enum Mode
	{
		Interferences,
		Tearing,
		Complete
	}

	[Serializable]
	public class InterferenceSettings
	{
		public float speed = 10f;
		public float density = 8f;
		public float maxDisplacement = 2f;
	}

	[Serializable]
	public class TearingSettings
	{
		public float speed = 1f;

		[Range(0f, 1f)]
		public float intensity = 0.25f;

		[Range(0f, 0.5f)]
		public float maxDisplacement = 0.05f;

		public bool allowFlipping = false;
		public bool yuvColorBleeding = true;

		[Range(-2f, 2f)]
		public float yuvOffset = 0.5f;
	}

	public Mode mode = Mode.Interferences;
	public InterferenceSettings interferencesSettings;
	public TearingSettings tearingSettings;

	protected Camera m_Camera;

	void OnEnable()
	{
		m_Camera = GetComponent<Camera>();
	}

	protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (mode == Mode.Interferences)
		{
			DoInterferences(source, destination, interferencesSettings);
		}
		else if (mode == Mode.Tearing)
		{
			DoTearing(source, destination, tearingSettings);
		}
		else // Complete
		{
			RenderTexture temp = RenderTexture.GetTemporary((int)m_Camera.pixelWidth, (int)m_Camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
			DoTearing(source, temp, tearingSettings);
			DoInterferences(temp, destination, interferencesSettings);
			temp.Release();
		}
	}

	void DoInterferences(RenderTexture source, RenderTexture destination, InterferenceSettings settings)
	{
		material.SetVector("_Params", new Vector3(settings.speed, settings.density, settings.maxDisplacement));
		Graphics.Blit(source, destination, material, 0);
	}

	void DoTearing(RenderTexture source, RenderTexture destination, TearingSettings settings)
	{
		material.SetVector("_Params", new Vector4(settings.speed, settings.intensity, settings.maxDisplacement, settings.yuvOffset));

		int pass = 1;
		if (settings.allowFlipping && settings.yuvColorBleeding) pass = 4;
		else if (settings.allowFlipping) pass = 2;
		else if (settings.yuvColorBleeding) pass = 3;

		Graphics.Blit(source, destination, material, pass);
	}
}
