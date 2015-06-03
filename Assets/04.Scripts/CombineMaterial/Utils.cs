/*
  Juan Sebastian Munoz Arango
  naruse@gmail.com
  All rights reserved
 */
namespace ProMaterialCombiner {

	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using System.Text.RegularExpressions;

	public static class Utils {

        public static string GenerateTimeStamp() {
            return Time.realtimeSinceStartup.ToString().Replace(".","");
        }

        public static bool IsShaderStandard(string shaderName) {
            return shaderName.Contains(Constants.StandardShaderToken);
        }

        //basically what this does is get a substring of the initial standard shader name without
        //the token texture defines used by the shader; thats pretty much:
        //Converts i.e: Standard (Specular setup)-Mai-Spe-Bum to -> Standard (Specular setup)
        public static string ExtractStandardShaderOriginalName(string standardShaderName) {
            int firstOccurrenceOfCharSeparatorIndex = standardShaderName.IndexOf("-");
            if(firstOccurrenceOfCharSeparatorIndex == -1)//maybe the shader is just "Standard" and contains no '-'
                if(Utils.IsShaderStandard(standardShaderName))//check if is a standard shader
                    firstOccurrenceOfCharSeparatorIndex = standardShaderName.Length;//there is nothing to cut if the shader is called "Standard" (as is a standard shader with just colors (no textures))

            if(firstOccurrenceOfCharSeparatorIndex == -1) {
                Debug.LogError("Couldnt locate char separator '-' on shaderName: " + standardShaderName);
                return "";
            }
            string originalName = standardShaderName.Substring(0, firstOccurrenceOfCharSeparatorIndex);
            return originalName;
        }


        private enum StandardShaderBlendMode {//has to be consistent with the enum of BlendMode in StandardShaderGUI.cs (which is part of unity internals)
            Opaque = 0,
            Cutout = 1,
            Transparent = 2
        }
        //material has to have standard shader!
        private static string GetStandardShaderRenderingMode(Material m) {
            string renderMode = "";
            if(m.HasProperty("_Mode"))
                renderMode = ((StandardShaderBlendMode) m.GetFloat("_Mode")).ToString();
             if(renderMode == "")
                Debug.LogError("Couldnt find Rendering Mode for standard shader on " + m.name);
            return renderMode;

        }

        public static string ParseStandardShaderName(Material mat) {
            List<string> shaderTextureDefines = new List<string>();
            int count = ShaderUtil.GetPropertyCount(mat.shader);
            for(int i = 0; i < count; i++) {
                if(ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    if(ShaderUtil.GetTexDim(mat.shader, i) == ShaderUtil.ShaderPropertyTexDim.TexDim2D) {
                        string shaderDefine = ShaderUtil.GetPropertyName(mat.shader, i);
                        Texture2D retrievedTextureFromShader = mat.GetTexture(shaderDefine) as Texture2D;
                        if(retrievedTextureFromShader != null)
                            shaderTextureDefines.Add(shaderDefine);
                    }
            }
            string parsedShaderName = mat.shader.name + "-" + GetStandardShaderRenderingMode(mat) + "-";
            for(int i = 0; i < shaderTextureDefines.Count; i++)
                parsedShaderName += shaderTextureDefines[i].Substring(1,3) + "-";
            return parsedShaderName.Remove(parsedShaderName.Length-1);//remove ending "-"
        }

		public static bool TextureSupported(Texture2D tex) {
			return 	tex.format == TextureFormat.ARGB32 || tex.format == TextureFormat.RGBA32 ||
					tex.format == TextureFormat.BGRA32 || tex.format == TextureFormat.RGB24 ||
					tex.format == TextureFormat.Alpha8;
		}

		//used when generating a valid prefab folder name out of the gameobjs names
        public static string GetValidName(string fileName) {
			return Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "_");
		}

	    public static Texture2D GenerateTexture(Color c) {
	        int width = Constants.NullTextureSize;
	        int height = Constants.NullTextureSize;
	        Color[] colors = new Color[width*height];
	        for(int i = 0; i < colors.Length; i++)
	            colors[i] = c;
	        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32/*in case the texture needs to be resized we need a supported format->(TextureScale.cs)*/, /*NO mipmaps*/false);
	        result.SetPixels(0,0,width, height, colors);
            result.Apply();
	        return result;
	    }

	    public static Texture2D CopyTexture(Texture2D t, TextureFormat format) {
	        Texture2D copiedTex = new Texture2D(t.width, t.height, format, false/*no mipmaps*/);
	        Color[] pixels = t.GetPixels(0,0,t.width, t.height);
	        copiedTex.SetPixels(pixels);
	        copiedTex.Apply();
	        return copiedTex;
	    }


	    private static string malformedObjName = "";//used to not display warning several times for a malformed object
	    //transforms the UVCoordinates from a single texture to a position in an atlased texture
	    public static Vector2 ReMapUV(Vector2 currentUV, float atlasWidth, float atlasHeight, Rect texturePosInAtlas, string objName, bool generatedTexture) {
	        if(((currentUV.x > 1 || currentUV.y > 1) || (currentUV.x < 0 || currentUV.y < 0))
	           && objName != malformedObjName) {
	            Debug.LogWarning("Malformed UV Map for '" + objName + "' some meshes might look bad when atlassed.");
	            malformedObjName = objName;
	        }
            Vector2 remappedUV = Vector2.zero;
            if(!generatedTexture)
                remappedUV = new Vector2(((currentUV.x * texturePosInAtlas.width) + texturePosInAtlas.x) / atlasWidth,
                                         ((currentUV.y * texturePosInAtlas.height) + texturePosInAtlas.y) / atlasHeight);
            else {//if texture is generated then lets just point all the UVs to the center of the generated texture.
                remappedUV = new Vector2(((0.5f * texturePosInAtlas.width) + texturePosInAtlas.x) / atlasWidth,
                                         ((0.5f * texturePosInAtlas.height) + texturePosInAtlas.y) / atlasHeight);
            }
	        return remappedUV;
	    }

	    public static Mesh CombineMeshes(Mesh[] meshes) {
	        CombineInstance[] combinedInstances= new CombineInstance[meshes.Length];
	        for(int i = 0; i < combinedInstances.Length; i++) {
	            combinedInstances[i].mesh = meshes[i];
	        }
	        Mesh combinedMesh = new Mesh();
	        combinedMesh.CombineMeshes(combinedInstances, true, false);
	        return combinedMesh;
	    }

	    public static uint CalculateMD5Hash(string input) {
	        MD5 md5 = System.Security.Cryptography.MD5.Create();
	        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
	        byte[] hash = md5.ComputeHash(inputBytes);
	        return BitConverter.ToUInt32(hash, 0);
	    }

	    public static List<Texture2D> FilterTexsByIndex(List<Texture2D> textures, List<int> indexes) {
	        List<Texture2D> filteredTextures = new List<Texture2D>();
	        for(int i = 0; i < indexes.Count; i++) {
	            filteredTextures.Add(textures[indexes[i]]);
	        }
	        return filteredTextures;
	    }
	    public static List<Vector2> FilterVec2ByIndex(List<Vector2> vec, List<int> indexes) {
	        List<Vector2> filteredVec2 = new List<Vector2>();
	        for(int i = 0; i < indexes.Count; i++) {
	            filteredVec2.Add(vec[indexes[i]]);
	        }
	        return filteredVec2;
	    }

	    /*public static GameObject[] GetAllObjectsInHierarchy() {
	        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
	        List<GameObject> objsInHierarchy = new List<GameObject>();
	        foreach(GameObject obj in allObjects) {
	            string assetPath = AssetDatabase.GetAssetPath(obj.transform.root.gameObject);
	            if (!String.IsNullOrEmpty(assetPath))
	                continue;
	            if (obj.hideFlags == HideFlags.None) {
	                objsInHierarchy.Add(obj);
	            }
	        }
	        return objsInHierarchy.ToArray();
	    }*/
	}
}