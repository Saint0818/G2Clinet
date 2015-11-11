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
//    public GameObject Outside;

    public enum EAttribute
    {
        StrBlk = 0,
        DefStl = 1,
        DrbPass = 2,
        SpdSta = 3,
        Pt2Pt3 = 4,
        RebDnk = 5
    }

	private readonly Material[] mMaterials = new Material[2];
	private readonly int[] mIndices = {0,1,2};
	private readonly MeshFilter[] mMeshFilters = new MeshFilter[6];

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

    public void SetVisible(bool visible)
    {
        Root.SetActive(visible);
    }

    [UsedImplicitly]
    private void Awake()
    {
        foreach(EAttribute attribute in Enum.GetValues(typeof(EAttribute)))
        {
            mOldValues.Add(attribute, 0);
            mNewValues.Add(attribute, 0);
        }

        mMaterials[0] = Resources.Load("Materials/TriangleMaterial_Outside") as Material;
        mMaterials[1] = Resources.Load("Materials/TriangleMaterial_Inside") as Material;

//        createTriangle(Vector3.zero, mMaxB, mMaxA, 10, mMaterials[0]);
//        createTriangle(Vector3.zero, mMaxC, mMaxB, 11, mMaterials[0]);
//        createTriangle(Vector3.zero, mMaxD, mMaxC, 12, mMaterials[0]);
//        createTriangle(Vector3.zero, mMaxE, mMaxD, 13, mMaterials[0]);
//        createTriangle(Vector3.zero, mMaxF, mMaxE, 14, mMaterials[0]);
//        createTriangle(Vector3.zero, mMaxA, mMaxF, 15, mMaterials[0]);

        createTriangle(Vector3.zero, mCurrentB, mCurrentA, 0, mMaterials[1]);
        createTriangle(Vector3.zero, mCurrentC, mCurrentB, 1, mMaterials[1]);
        createTriangle(Vector3.zero, mCurrentD, mCurrentC, 2, mMaterials[1]);
        createTriangle(Vector3.zero, mCurrentE, mCurrentD, 3, mMaterials[1]);
        createTriangle(Vector3.zero, mCurrentF, mCurrentE, 4, mMaterials[1]);
        createTriangle(Vector3.zero, mCurrentA, mCurrentF, 5, mMaterials[1]);

        SetVisible(false);
    }

	private void createTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int index, Material ma)
    {
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
			mMeshFilters [index] = obj.GetComponent<MeshFilter>();
			mMeshFilters [index].mesh = mesh;
			obj.transform.parent = Inside.transform;
		}
//        else
//        {
//			obj.GetComponent<MeshFilter>().mesh = mesh;
//			obj.transform.parent = Outside.transform;
//		}

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
//		Outside.transform.localScale = new Vector3(0,0,0);
		Inside.transform.DOScale(Vector3.one, 0.4f);
//		Outside.transform.DOScale(Vector3.one, 0.4f);
	}

    [UsedImplicitly]
	private void FixedUpdate()
    {
        foreach(EAttribute attribute in Enum.GetValues(typeof(EAttribute)))
        {
            float a = Mathf.Round(mOldValues[attribute] * 100.0f) / 100.0f;
            float b = Mathf.Round(mNewValues[attribute] * 100.0f) / 100.0f;
            if (a != b)
            {
                if (mOldValues[attribute] >= mNewValues[attribute])
                    SetValue(attribute, mOldValues[attribute] - 0.01f, true);
                else if (mOldValues[attribute] <= mNewValues[attribute])
                    SetValue(attribute, mOldValues[attribute] + 0.01f, true);
            }
        }
    }

	private readonly Dictionary<EAttribute, float> mOldValues = new Dictionary<EAttribute, float>();
	private readonly Dictionary<EAttribute, float> mNewValues = new Dictionary<EAttribute, float>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="percent"> 數值範圍: [0, 1]. </param>
    /// <param name="isUpdateMesh"></param>
	public void SetValue(EAttribute attribute, float percent, bool isUpdateMesh = false)
    {
        percent = Mathf.Clamp01(percent);

		if(!isUpdateMesh)
        {
			if(mOldValues[attribute] == 0)
            {
				mOldValues[attribute] = percent;
				mNewValues[attribute] = percent;
				SetValue(attribute, percent, true);
			}
            else
				mNewValues[attribute] = percent;
		}
        else
        {
			mOldValues[attribute] = percent;
			switch(attribute)
            {
			case EAttribute.StrBlk:
				mCurrentA = new Vector3(mMaxA.x * percent, mMaxA.y * percent, mMaxA.z);
				updateMesh(Vector3.zero, mCurrentB, mCurrentA, 0);
				updateMesh(Vector3.zero, mCurrentA, mCurrentF, 5);
				break;
			case EAttribute.DefStl:
				mCurrentB = new Vector3(mMaxB.x * percent, mMaxB.y * percent, mMaxB.z);
				updateMesh(Vector3.zero, mCurrentB, mCurrentA, 0);
				updateMesh(Vector3.zero, mCurrentC, mCurrentB, 1);
				break;
			case EAttribute.DrbPass:
				mCurrentC = new Vector3(mMaxC.x * percent, mMaxC.y * percent, mMaxC.z);
				updateMesh(Vector3.zero, mCurrentC, mCurrentB, 1);
				updateMesh(Vector3.zero, mCurrentD, mCurrentC, 2);
				break;
			case EAttribute.SpdSta:
				mCurrentD = new Vector3(mMaxD.x * percent, mMaxD.y * percent, mMaxD.z);
				updateMesh(Vector3.zero, mCurrentD, mCurrentC, 2);
				updateMesh(Vector3.zero, mCurrentE, mCurrentD, 3);
				break;
			case EAttribute.Pt2Pt3:
				mCurrentE = new Vector3(mMaxE.x * percent, mMaxE.y * percent, mMaxE.z);
				updateMesh(Vector3.zero, mCurrentE, mCurrentD, 3);
				updateMesh(Vector3.zero, mCurrentF, mCurrentE, 4);
				break;
			case EAttribute.RebDnk:
				mCurrentF = new Vector3(mMaxF.x * percent, mMaxF.y * percent, mMaxF.z);
				updateMesh(Vector3.zero, mCurrentF, mCurrentE, 4);
				updateMesh(Vector3.zero, mCurrentA, mCurrentF, 5);
				break;
			}
		}
	}

	private void updateMesh(Vector3 v1, Vector3 v2, Vector3 v3, int index)
    {
		if (index >= 0 && index < mMeshFilters.Length && mMeshFilters [index] != null)
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
}
