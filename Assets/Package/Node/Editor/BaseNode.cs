using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {
	public Rect windowRect;
	public bool hasInputs = false; 
	public string windowTitle = "";

	public virtual void DrawWindow()
	{

	}

	public abstract void DrawCurves();

	public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
	{

	}

	public virtual void NodeDeleted(BaseNode node)
	{

	}

	public virtual BaseInputNode ClickedOnInput(Vector2 pos)
	{
		return null;
	}
}
