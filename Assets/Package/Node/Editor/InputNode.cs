using UnityEngine;
using System.Collections;
using UnityEditor;

public class InputNode : BaseInputNode {

	private InputType inputType;

	public enum InputType
	{
		Number,
		Randomization
	}

	private string randomFrom = "";
	private string randomTo = "";
	private string inputValue = "";
	private string inputTitle = "Input Node";

	public InputNode()
	{
		windowTitle = inputTitle;
	}

	public override void DrawWindow()
	{
		base.DrawWindow ();

		inputType = (InputType)EditorGUILayout.EnumPopup ("Input type : ", inputType);
		inputTitle = EditorGUILayout.TextField ("Title", inputTitle);

		if (windowTitle != inputTitle)
			windowTitle = inputTitle;

		if (inputType == InputType.Number) 
		{
			inputValue = EditorGUILayout.TextField ("Value", inputValue);
		} 
		else if (inputType == InputType.Randomization) 
		{
			randomFrom = EditorGUILayout.TextField("From", randomFrom);
			randomTo = EditorGUILayout.TextField("To", randomTo);

			if(GUILayout.Button("Calculate Random"))
			{
				calculateRandom();
			}
		}
	}

	public override void DrawCurves()
	{}

	private void calculateRandom()
	{
		float rFrom = 0;
		float rTo = 0;
		float.TryParse (randomFrom, out rFrom);
		float.TryParse (randomTo, out rTo);

		int randFrom = (int)(rFrom * 10);
		int randTo = (int)(rTo * 10);

		int selected = UnityEngine.Random.Range (randFrom, randTo + 1);
		float selectedValue = selected / 10;

		inputValue = selectedValue.ToString ();
	}

	public override string getResult ()
	{
		return inputValue.ToString ();
	}
}
