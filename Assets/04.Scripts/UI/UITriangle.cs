using UnityEngine;
using System.Collections;

public class UITriangle : KnightSingleton<UITriangle> {
	private Mesh m_Mesh;
	private Material [] MaterialAy = new Material[2];
	private GameObject TriangleInside;
	private GameObject TriangleOutside;
	private int[] m_Tris = new int[]{0,1,2};
	private MeshFilter [] MeshAy = new MeshFilter[6];
	private Vector3 source = new Vector3 (0, 0, 0);
	private Vector3 A = new Vector3 (2, 0, 0);
	private Vector3 B = new Vector3 (1, Mathf.Sqrt(3), 0);
	private Vector3 C = new Vector3 (-1, Mathf.Sqrt(3), 0);
	private Vector3 D = new Vector3 (-2, 0, 0);
	private Vector3 E = new Vector3 (-1, -Mathf.Sqrt(3), 0);
	private Vector3 F = new Vector3 (1, -Mathf.Sqrt(3), 0);

	private Vector3 A1 = new Vector3 (2, 0, 0);
	private Vector3 B1 = new Vector3 (1, Mathf.Sqrt(3), 0);
	private Vector3 C1 = new Vector3 (-1, Mathf.Sqrt(3), 0);
	private Vector3 D1 = new Vector3 (-2, 0, 0);
	private Vector3 E1 = new Vector3 (-1, -Mathf.Sqrt(3), 0);
	private Vector3 F1 = new Vector3 (1, -Mathf.Sqrt(3), 0);

	public GameObject Triangle;

	public void CreateSixAttr(Vector3 v1)
	{
		Triangle = new GameObject();
		Triangle.name = "Triangle";
		TriangleInside = new GameObject ();
		TriangleInside.name = "TriangleInside";
		TriangleInside.transform.parent = Triangle.transform;
		TriangleOutside = new GameObject ();
		TriangleOutside.name = "TriangleOutside";
		TriangleOutside.transform.parent = Triangle.transform;
		MaterialAy[0] = Resources.Load ("Materials/TriangleMaterial_Outside") as Material;
		MaterialAy[1] = Resources.Load ("Materials/TriangleMaterial_Inside") as Material;

		CreateTriangle (source, B1, A1, 10, MaterialAy[0]);
		CreateTriangle (source, C1, B1, 11, MaterialAy[0]);
		CreateTriangle (source, D1, C1, 12, MaterialAy[0]);
		CreateTriangle (source, E1, D1, 13, MaterialAy[0]);
		CreateTriangle (source, F1, E1, 14, MaterialAy[0]);
		CreateTriangle (source, A1, F1, 15, MaterialAy[0]);

		CreateTriangle (source, B, A, 0, MaterialAy[1]);
		CreateTriangle (source, C, B, 1, MaterialAy[1]);
		CreateTriangle (source, D, C, 2, MaterialAy[1]);
		CreateTriangle (source, E, D, 3, MaterialAy[1]);
		CreateTriangle (source, F, E, 4, MaterialAy[1]);
		CreateTriangle (source, A, F, 5, MaterialAy[1]);

		Triangle.transform.localPosition = v1;
		TriangleInside.transform.localPosition = new Vector3 (0, 0, -0.01f);
	}

	private void CreateTriangle(Vector3 V1, Vector3 V2, Vector3 V3, int Index, Material ma)
	{
		Vector3[] m_Vertexs = new Vector3[]{V1, V2, V3};
		m_Mesh = new Mesh ();
		m_Mesh.vertices = m_Vertexs;
		m_Mesh.triangles = m_Tris;		
		m_Mesh.RecalculateNormals();

		GameObject obj = new GameObject (Index.ToString());
		obj.AddComponent<MeshRenderer>();
		obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshCollider>();
		if (Index >= 0 && Index < MeshAy.Length) 
		{
			MeshAy [Index] = obj.GetComponent<MeshFilter> ();
			MeshAy [Index].mesh = m_Mesh;
			obj.transform.parent = TriangleInside.transform;
		}
		else
		{
			obj.GetComponent<MeshFilter> ().mesh = m_Mesh;
			obj.transform.parent = TriangleOutside.transform;
		}

		obj.GetComponent<MeshRenderer> ().material = ma;
	}

	void FixedUpdate()
	{
		for(int i = 0; i < OldValueAy.Length; i++)
		{
			if(OldValueAy[i] != NewValueAy[i])
			{
				if(OldValueAy[i] > NewValueAy[i])
				{
					ChangeValue(i, OldValueAy[i] - 0.01f, true);
				}
				else if(OldValueAy[i] < NewValueAy[i])
				{
					ChangeValue(i, OldValueAy[i] + 0.01f, true);
				}
			}
		}
	}

	private float [] OldValueAy = new float[6];
	private float [] NewValueAy = new float[6];
	public void ChangeValue(int Index, float Value, bool Update = false)
	{
		if (Index >= 0 && Index < MeshAy.Length) 
		{
			if(Value < 0)
				Value = 0;

			if(Value > 1)
				Value = 1;

			if(!Update)
			{
				if(OldValueAy[Index] == 0)
				{
					OldValueAy[Index] = Value;
					NewValueAy[Index] = Value;
					ChangeValue(Index, Value, true);
				}
				else
				{
					NewValueAy[Index] = Value;
				}
			}
			else
			{
				OldValueAy[Index] = Value;
				switch(Index)
				{
				case 0:
					//A
					A = new Vector3(A1.x * Value, A1.y * Value, A1.z);
					ResetMesh(source, B, A, 0);
					ResetMesh(source, A, F, 5);
					break;
				case 1:
					//B
					B = new Vector3(B1.x * Value, B1.y * Value, B1.z);
					ResetMesh (source, B, A, 0);
					ResetMesh (source, C, B, 1);
					break;
				case 2:	
					//C
					C = new Vector3(C1.x * Value, C1.y * Value, C1.z);
					ResetMesh (source, C, B, 1);
					ResetMesh (source, D, C, 2);
					break;
				case 3:
					//D
					D = new Vector3(D1.x * Value, D1.y * Value, D1.z);
					ResetMesh (source, D, C, 2);
					ResetMesh (source, E, D, 3);
					break;
				case 4:
					//E
					E = new Vector3(E1.x * Value, E1.y * Value, E1.z);
					ResetMesh (source, E, D, 3);
					ResetMesh (source, F, E, 4);
					break;
				case 5:
					//F
					F = new Vector3(F1.x * Value, F1.y * Value, F1.z);
					ResetMesh (source, F, E, 4);
					ResetMesh (source, A, F, 5);
					break;
				}
			}
		}
	}

	private void ResetMesh(Vector3 V1, Vector3 V2, Vector3 V3, int Index)
	{
		if (Index >= 0 && Index < MeshAy.Length && MeshAy [Index] != null) 
		{
			Vector3[] m_Vertexs = new Vector3[]{V1, V2, V3};
			m_Mesh = new Mesh ();
			m_Mesh.vertices = m_Vertexs;
			m_Mesh.triangles = m_Tris;		
			m_Mesh.RecalculateNormals();
			MeshAy [Index].mesh = m_Mesh;
		}
	}
}
