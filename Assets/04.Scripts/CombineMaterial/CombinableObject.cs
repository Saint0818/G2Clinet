/*
  Created by:
  Juan Sebastian Munoz
  naruse@gmail.com
  All rights reserved

 */
namespace ProMaterialCombiner {
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    public class CombinableObject {
        private MaterialCombiner materialCombiner;

        public int GetAproxAtlasSize(bool reuseTextures) {
            return materialCombiner.GetAproxAtlasSize(reuseTextures);
        }

        public string GetShaderUsed() {
            return materialCombiner.ShaderToCombine;
        }

        public int GetMaterialsToCombineCount() {
            return materialCombiner.ObjectMaterials.Length;
        }

        private bool isObjectCorrectlyAssembled = false;
        public bool IsCorrectlyAssembled { get { return isObjectCorrectlyAssembled; } }
        private bool usesSkinnedMeshRenderer = false;//gets set after CheckObjectIntegrity() is called
        private GameObject objectToCombine;
        public GameObject ObjectToCombine {
            get { return objectToCombine; }
            set {
                integrityLog[0] = integrityLog[1] = "";
                objectToCombine = value;
                CheckGameObjectIntegrity();
                if(isObjectCorrectlyAssembled)
                    materialCombiner = new MaterialCombiner(objectToCombine, usesSkinnedMeshRenderer);
                else
                    materialCombiner = null;
            }
        }
        private string[] integrityLog;
        public string[] IntegrityLog { get { return integrityLog; } }


        public CombinableObject() {
            integrityLog = new string[2];
        }

        public CombinableObject(GameObject g) {
            integrityLog = new string[2];
            ObjectToCombine = g;
        }


        public void CombineObject(string customAtlasName, bool reuseTextures, bool generatePrefab) {
            if(objectToCombine == null || !isObjectCorrectlyAssembled) {
                Debug.LogError("ERROR: Cant combine object!");
                return;
            }
            materialCombiner.CombineMaterials(usesSkinnedMeshRenderer, customAtlasName, reuseTextures, generatePrefab);
        }

        //this is already set in an Area called from OnGUI() on ProMaterialCombinerMenu.cs
        //todo, think how to remove the reuseTextures flag as is only used to get the aprox atlas size.
        public void DrawGUI(bool reuseTextures) {//refers to the area we are going to draw this GUI into
            if(!IsCorrectlyAssembled)
                return;
            GUILayout.BeginVertical();
            int atlasSize = materialCombiner.GetAproxAtlasSize(reuseTextures);
            GUI.Label(new Rect(10, 0, 200, 25), "Atlas size:~(" + atlasSize + "x" + atlasSize +")+2.5%+");
            GUILayout.Space(40);
            for(int i = 0; i < materialCombiner.ObjectMaterials.Length; i++) {
                Material mat = materialCombiner.ObjectMaterials[i];
                Texture tex = mat.mainTexture;
                bool textureGenerated = false;
                if(tex == null) {
                    tex = Utils.GenerateTexture(mat.color);
                    textureGenerated = true;
                }
                GUI.Label(new Rect(10, (30 + i*40), 25,25), (i+1)+":");

                EditorGUI.DrawPreviewTexture(new Rect(30, 30 + (i*40), 30, 30),
                                             tex,
                                             null,
                                             ScaleMode.StretchToFill);
                GUI.Label(new Rect(70, 15 + (30+ i*40), 305,25), "(" + tex.width + "x" + tex.height + ") Shader: " + mat.shader.name);
                GUI.Label(new Rect(70, 15 + 15 + (i*40), 105,25), (textureGenerated ? "Generated texture" : ""));
                GUILayout.Space(40);
            }
            //GUI.Label(new Rect(10, materialCombiner.ObjectMaterials.Length*40, 50, 25), "TEST");
            //GUILayout.Space(40);
            GUILayout.EndVertical();
        }


	    // If there are missing things, writes to the log of the object whats the problem
        //
        // There are 2 options, object has a SkinnedMeshRenderer OR (has a MeshFilter AND a MeshRenderer)
        // it first checks for skinned mesh renderer
	    private void CheckGameObjectIntegrity() {

	        bool objectHasMeshRenderer = false;
	        bool objectHasMeshFilter = false;
            bool objectHasSkinnedMeshRenderer = false;
            bool skinnedMeshRendererHasMesh = false;
            bool sameTypeOfShaderAcrossMaterials = false;
            bool objectHasMoreThanOneMaterial = false;
	        integrityLog[0] = integrityLog[1] = "";

            bool objectHasMaterials = false;
            Material[] materials = null;

            if(objectToCombine == null) {
                isObjectCorrectlyAssembled = false;
                return;
            }
            if(objectToCombine.GetComponent<SkinnedMeshRenderer>() != null) {
                objectHasSkinnedMeshRenderer = true;
                if(objectToCombine.GetComponent<SkinnedMeshRenderer>().sharedMesh == null)
                    integrityLog[0] = "No mesh attached on SkinnedMeshRenderer.";
                else
                    skinnedMeshRendererHasMesh = true;

                if(objectToCombine.GetComponent<SkinnedMeshRenderer>().sharedMaterials.Length == 0)
                    integrityLog[1] = "No material attached on SkinnedMeshRenderer";
                else {
                    objectHasMaterials = true;
                    materials = objectToCombine.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
                }
            } else {
                //Check MeshFilter integrity
                if(objectToCombine.GetComponent<MeshFilter>() == null) {
                    integrityLog[0] = "Missing MeshFilter";
                } else {
                    //Object has meshfilter component, check that it has a mesh.
                    if(objectToCombine.GetComponent<MeshFilter>().sharedMesh == null)
                        integrityLog[0] = "Missing Mesh on MeshFilter";
                    else
                        objectHasMeshFilter = true;
                }
                //check for mesh renderer integrity
                if(objectToCombine.GetComponent<MeshRenderer>() == null) {
                    integrityLog[1] = "Missing MeshRenderer";
                } else {
                    objectHasMeshRenderer = true;
                    //Object has a mesh renderer component, check that has at least 1 material.
                    if(objectToCombine.GetComponent<MeshRenderer>().sharedMaterials.Length == 0) {
                        integrityLog[1] = "No material attached on MeshRenderer";
                    } else {
                        objectHasMaterials = true;
                        materials = objectToCombine.GetComponent<MeshRenderer>().sharedMaterials;
                    }
                }
            }
            if(objectHasMaterials) {
                //check that the object has at least more than 1 material so the object can be optimizable.
                if(materials.Length < 2) {
                    objectHasMoreThanOneMaterial = false;
                    integrityLog[0] = "Not optimizable object. Object";
                    integrityLog[1] = "has to have more than 1 material.";
                } else
                    objectHasMoreThanOneMaterial = true;

                //check all the materials share the same shader
                bool materialsCorrect = true;
                string shaderUsed = "";
                for(int i = 0; i < materials.Length;i++) {
                    if(materials[i] != null) {
                        if(shaderUsed == "") {
                            shaderUsed = materials[i].shader.name;
                            if(Utils.IsShaderStandard(shaderUsed)) {
                                shaderUsed = Utils.ParseStandardShaderName(materials[i]);
                            }
                        } else if(shaderUsed != (Utils.IsShaderStandard(materials[i].shader.name) ? Utils.ParseStandardShaderName(materials[i]) : materials[i].shader.name)) {
                            if(Utils.IsShaderStandard(shaderUsed))
                                integrityLog[1] = "Different types of standard shader in the object, check textures";
                            else
                                integrityLog[1] = "Different shaders in the object";
                            materialsCorrect = false;
                            break;
                        }
                    } else {
                        integrityLog[1] = "Some materials are null";
                        materialsCorrect = false;
                        break;
                    }
                }
                if(materialsCorrect) {
                    sameTypeOfShaderAcrossMaterials = true;
                }
            }

            //object is correctly assembed if either the object has a skinned mesh renderer and has a mesh reference and has the same type of shader across materials OR
            //the object has a mesh renderer and a mesh filter and has the same type of shader across materials
	        isObjectCorrectlyAssembled = ((objectHasSkinnedMeshRenderer && skinnedMeshRendererHasMesh) || (objectHasMeshRenderer && objectHasMeshFilter)) && sameTypeOfShaderAcrossMaterials && objectHasMoreThanOneMaterial;
            usesSkinnedMeshRenderer = objectHasSkinnedMeshRenderer;
	    }
    }
}
