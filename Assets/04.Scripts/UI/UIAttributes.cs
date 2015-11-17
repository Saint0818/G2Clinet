using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 負責動態的顯示角色的能力值.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 將 UIAttributeHexagon prefab 拖曳到 NGUI UIRoot 底下. </item>
/// <item> Call SetVisible() 設定要不要顯示.(預設為不顯示) </item>
/// <item> Call SetValue() 設定數值. </item>
/// <item> (Optional) Call PlayScale() 播放 Animation. </item>
/// </list>
/// </remarks>
public class UIAttributes : MonoBehaviour
{
    public GameObject Root;
    public GameObject Inside;
	public GameObject Title;

    public enum EGroup
    {
        Block = 0,
        Steal = 1,
        Point2 = 2,
        Point3 = 3,
        Dunk = 4,
        Rebound = 5
    }

	private readonly int[] mIndices = {0, 1, 2};
	private readonly MeshFilter[] mMeshFilters = new MeshFilter[6];

    /*
          C      B
       D     []     A
          E      F
    */
    // 目前點的位置.
	private Vector3 mCurrentA = new Vector3(2, 0, 0);
	private Vector3 mCurrentB = new Vector3(1, Mathf.Sqrt(3), 0);
	private Vector3 mCurrentC = new Vector3(-1, Mathf.Sqrt(3), 0);
	private Vector3 mCurrentD = new Vector3(-2, 0, 0);
	private Vector3 mCurrentE = new Vector3(-1, -Mathf.Sqrt(3), 0);
	private Vector3 mCurrentF = new Vector3(1, -Mathf.Sqrt(3), 0);

    // 點最遠的位置.
	private readonly Vector3 mMaxA = new Vector3(2, 0, 0);
	private readonly Vector3 mMaxB = new Vector3(1, Mathf.Sqrt(3), 0);
	private readonly Vector3 mMaxC = new Vector3(-1, Mathf.Sqrt(3), 0);
	private readonly Vector3 mMaxD = new Vector3(-2, 0, 0);
	private readonly Vector3 mMaxE = new Vector3(-1, -Mathf.Sqrt(3), 0);
	private readonly Vector3 mMaxF = new Vector3(1, -Mathf.Sqrt(3), 0);

    /// <summary>
    /// value: 百分比, 數值範圍: [0, 1].
    /// </summary>
	private readonly Dictionary<EGroup, float> mOldValues = new Dictionary<EGroup, float>();
    private readonly Dictionary<EGroup, float> mNewValues = new Dictionary<EGroup, float>();

    public void SetVisible(bool visible)
    {
        Root.SetActive(visible);
    }

    [UsedImplicitly]
    private void Awake()
    {
        foreach(EGroup attribute in Enum.GetValues(typeof(EGroup)))
        {
            mOldValues.Add(attribute, 0);
            mNewValues.Add(attribute, 0);
        }

        var mat = Resources.Load("Materials/TriangleMaterial_Inside") as Material;

        createTriangle(Vector3.zero, mCurrentB, mCurrentA, 0, mat);
        createTriangle(Vector3.zero, mCurrentC, mCurrentB, 1, mat);
        createTriangle(Vector3.zero, mCurrentD, mCurrentC, 2, mat);
        createTriangle(Vector3.zero, mCurrentE, mCurrentD, 3, mat);
        createTriangle(Vector3.zero, mCurrentF, mCurrentE, 4, mat);
        createTriangle(Vector3.zero, mCurrentA, mCurrentF, 5, mat);
    }

	private void createTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int index, Material ma)
    {
        // Triangle 要順時針定義才看的見.
		Vector3[] vertices = {v1, v2, v3};
	    var mesh = new Mesh
	    {
	        vertices = vertices,
	        triangles = mIndices
	    };
	    mesh.RecalculateNormals();

		GameObject obj = new GameObject(index.ToString());
		obj.layer = LayerMask.NameToLayer("UI");
		obj.AddComponent<MeshRenderer>();
		obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshCollider>();
		if(index >= 0 && index < mMeshFilters.Length)
        {
			mMeshFilters[index] = obj.GetComponent<MeshFilter>();
			mMeshFilters[index].mesh = mesh;
			obj.transform.parent = Inside.transform;
		}

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

		obj.GetComponent<MeshRenderer>().material = ma;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delay"> 經過幾秒後才播放, 單位: 秒. </param>
    public void PlayScale(float delay)
    {
        if(delay > 0)
            Invoke("Play", delay);
        else
            Debug.LogWarningFormat("parameter delay({0}) must be greater than zero.", delay);
    }

	public void Play()
    {
        SetVisible(true);

		Inside.transform.localScale = new Vector3(0,0,0);
		Inside.transform.DOScale(Vector3.one, 0.4f);
	}

    [UsedImplicitly]
	private void FixedUpdate()
    {
        foreach(EGroup g in Enum.GetValues(typeof(EGroup)))
        {
            float a = Mathf.Round(mOldValues[g] * 100.0f) / 100.0f;
            float b = Mathf.Round(mNewValues[g] * 100.0f) / 100.0f;
            if (a != b)
            {
                if(mOldValues[g] >= mNewValues[g])
                    SetValue(g, mOldValues[g] - 0.01f, true);
                else if (mOldValues[g] <= mNewValues[g])
                    SetValue(g, mOldValues[g] + 0.01f, true);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="percent"> 數值範圍: [0, 1]. </param>
    /// <param name="isUpdateMesh"></param>
	public void SetValue(EGroup g, float percent, bool isUpdateMesh = false)
    {
        percent = Mathf.Clamp01(percent);

		if(!isUpdateMesh)
        {
			if(mOldValues[g] == 0)
            {
				mOldValues[g] = percent;
				mNewValues[g] = percent;
				SetValue(g, percent, true);
			}
            else
				mNewValues[g] = percent;
		}
        else
        {
			mOldValues[g] = percent;
			switch(g)
            {
			case EGroup.Block:
				mCurrentA = new Vector3(mMaxA.x * percent, mMaxA.y * percent, mMaxA.z);
				updateMesh(Vector3.zero, mCurrentB, mCurrentA, 0);
				updateMesh(Vector3.zero, mCurrentA, mCurrentF, 5);
				break;
			case EGroup.Steal:
				mCurrentB = new Vector3(mMaxB.x * percent, mMaxB.y * percent, mMaxB.z);
				updateMesh(Vector3.zero, mCurrentB, mCurrentA, 0);
				updateMesh(Vector3.zero, mCurrentC, mCurrentB, 1);
				break;
			case EGroup.Point2:
				mCurrentC = new Vector3(mMaxC.x * percent, mMaxC.y * percent, mMaxC.z);
				updateMesh(Vector3.zero, mCurrentC, mCurrentB, 1);
				updateMesh(Vector3.zero, mCurrentD, mCurrentC, 2);
				break;
			case EGroup.Point3:
				mCurrentD = new Vector3(mMaxD.x * percent, mMaxD.y * percent, mMaxD.z);
				updateMesh(Vector3.zero, mCurrentD, mCurrentC, 2);
				updateMesh(Vector3.zero, mCurrentE, mCurrentD, 3);
				break;
			case EGroup.Dunk:
				mCurrentE = new Vector3(mMaxE.x * percent, mMaxE.y * percent, mMaxE.z);
				updateMesh(Vector3.zero, mCurrentE, mCurrentD, 3);
				updateMesh(Vector3.zero, mCurrentF, mCurrentE, 4);
				break;
			case EGroup.Rebound:
				mCurrentF = new Vector3(mMaxF.x * percent, mMaxF.y * percent, mMaxF.z);
				updateMesh(Vector3.zero, mCurrentF, mCurrentE, 4);
				updateMesh(Vector3.zero, mCurrentA, mCurrentF, 5);
				break;
			}
		}
	}

	private void updateMesh(Vector3 v1, Vector3 v2, Vector3 v3, int index)
    {
		if(index >= 0 && index < mMeshFilters.Length && mMeshFilters[index] != null)
        {
			Vector3[] vertices = {v1, v2, v3};
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = mIndices
            };

            mesh.RecalculateNormals();
			mMeshFilters[index].mesh = mesh;
		}
	}

	public bool EnableTitle
	{
		set{ Title.SetActive(value);}
	}
}
