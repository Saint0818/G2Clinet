using UnityEngine;
using System.Collections;
using UnityEditor;

public class BaseInputNode : BaseNode {

	public virtual string getResult()
	{
		return "None";
	}

	public override void DrawCurves()
	{

	}
}
