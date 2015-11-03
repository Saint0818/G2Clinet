/*

Copyright (c) 2012, nanmo (@nanimosa)
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
- Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
- Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
- Neither the name of the "nanmo" nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent( typeof( MeshRenderer ) )]
[RequireComponent( typeof( MeshFilter ) )]

public class CircularSectorMeshRenderer : MonoBehaviour {
	public float degree = 180;
	[HideInInspector]
	public float intervalDegree = 5;
	public float beginOffsetDegree = 0;
	public float radius = 10;
	public GameObject RefGameObject;
	
	Mesh mesh;
	MeshFilter meshFilter;
	
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	
	int i;
	float beginDegree ;
	float endDegree;
	float beginRadian;
	float endRadian;
	float uvRadius = 0.5f;
	Vector2 uvCenter = new Vector2( 0.5f, 0.5f );
	float currentIntervalDegree = 0;
	float limitDegree;
	int count;
	int lastCount;
	
	float beginCos;
	float beginSin;
	float endCos;
	float endSin;
	
	int beginNumber;
	int endNumber;
	int triangleNumber;
	
	// Use this for initialization
	void Start () {
		RefGameObject = gameObject;
		mesh = new Mesh();
		meshFilter = (MeshFilter)GetComponent("MeshFilter");
	}
	
	public void ChangeValue (float d, float dis) {
//	void Update () {
		degree = d;
		beginOffsetDegree = d * (-0.5f);
		radius = dis;

		currentIntervalDegree = Mathf.Abs(intervalDegree);
		
		count = (int)( Mathf.Abs(degree) / currentIntervalDegree );
		if ( degree % intervalDegree != 0 ) {
			++count;
		}
		if ( degree < 0 ){
			currentIntervalDegree = -currentIntervalDegree;
		}
		
		if ( lastCount != count )
		{
			mesh.Clear();
			vertices = new Vector3[ count*2 + 1 ];
			triangles = new int [ count*3 ];
			uvs = new Vector2[ count*2 + 1 ];
			vertices[0] = Vector3.zero;
			uvs[0] = uvCenter;
			lastCount = count;
		}
		
		i=0;
		beginDegree = beginOffsetDegree + 90;
		limitDegree = degree + beginOffsetDegree + 90;
		
		while( i < count ) {
			endDegree = beginDegree + currentIntervalDegree;
			
			if ( degree > 0 ) {
				if ( endDegree > limitDegree ) {
					endDegree = limitDegree;
				}
			} else {
				if ( endDegree < limitDegree ) {
					endDegree = limitDegree;
				}
			}
			
			beginRadian = Mathf.Deg2Rad * beginDegree;
			endRadian = Mathf.Deg2Rad * endDegree ;
			
			beginCos = Mathf.Cos( beginRadian );
			beginSin = Mathf.Sin( beginRadian );
			endCos = Mathf.Cos( endRadian );
			endSin = Mathf.Sin( endRadian );
			
			beginNumber = i*2 + 1;
			endNumber = i*2 + 2;
			triangleNumber = i*3;
			
			vertices[ beginNumber ].x = beginCos * radius;
			vertices[ beginNumber ].y = 0;
			vertices[ beginNumber ].z = beginSin * radius;
			vertices[ endNumber ].x = endCos * radius;
			vertices[ endNumber ].y = 0;
			vertices[ endNumber ].z = endSin * radius;
			
			triangles[ triangleNumber ] = 0;
			if ( degree > 0 ) {
				triangles[ triangleNumber + 1 ] = endNumber;
				triangles[ triangleNumber + 2 ] = beginNumber;
			} else {
				triangles[ triangleNumber + 1 ] = beginNumber;
				triangles[ triangleNumber + 2 ] = endNumber;
			}
			
			if ( radius > 0 ) {
				uvs[ beginNumber ].x = beginCos * uvRadius + uvCenter.x;
				uvs[ beginNumber ].y = beginSin * uvRadius + uvCenter.y;
				uvs[ endNumber ].x = endCos * uvRadius + uvCenter.x;
				uvs[ endNumber ].y = endSin * uvRadius + uvCenter.y;
			} else {
				uvs[ beginNumber ].x = -beginCos * uvRadius + uvCenter.x;
				uvs[ beginNumber ].y = -beginSin * uvRadius + uvCenter.y;
				uvs[ endNumber ].x = -endCos * uvRadius + uvCenter.x;
				uvs[ endNumber ].y = -endSin * uvRadius + uvCenter.y;
			}
			
			beginDegree += currentIntervalDegree;
			++i;
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		meshFilter.sharedMesh = mesh;
		meshFilter.sharedMesh.name = "CircularSectorMesh";
	}
}