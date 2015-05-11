using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TestSaveMesh : MonoBehaviour 
{
	public GameObject CloneObject;
	public Material DefaultMaterial;

	//儲存三角面參數
	private List<int> Triangles = new List<int>();

	// Update is called once per frame
	void Update() 
	{
	if (Input.GetKeyDown(KeyCode.S))
		{
			SaveMesh();
		}
	}

	void SaveMesh()
	{
		SkinnedMeshRenderer[] skinnMeshRenders = GetComponentsInChildren<SkinnedMeshRenderer> ();
		CombineInstance[] combine = new CombineInstance[skinnMeshRenders.Length];

		for(int skinnMeshNum = 0 ; skinnMeshNum<skinnMeshRenders.Length ; skinnMeshNum++)
		{
			Triangles.Clear();
			//Clone當前網面
			Mesh mesh = new Mesh();
			skinnMeshRenders[skinnMeshNum].BakeMesh(mesh);

			//將所有子網面的三角面參數加入至Triangles
			for(int subNum = 0 ; subNum<mesh.subMeshCount;subNum++)
			{
				Triangles.AddRange(mesh.GetTriangles(subNum).ToList());
			}
			//將儲存的Triangles寫入至mesh的第一個子網面，並設定子網面數量為1
			mesh.SetTriangles(Triangles.ToArray(),0);
			mesh.subMeshCount = 1;

			//mesh放到合併實例裡
			combine[skinnMeshNum].mesh = mesh;
		}

		Mesh Commesh = new Mesh();
		Commesh.CombineMeshes(combine,true,false);

		CloneObj (Commesh);
    }
        //Clone物件生成網面渲染   
	    private void CloneObj(Mesh ClonMesh)
	    {
		MeshFilter clone_meshFiltet = CloneObject.AddComponent<MeshFilter> ();
		MeshRenderer clone_meshRenderer = CloneObject.AddComponent<MeshRenderer> ();
		clone_meshFiltet.mesh = ClonMesh;
		clone_meshRenderer.material = DefaultMaterial;
     	}
 
}
