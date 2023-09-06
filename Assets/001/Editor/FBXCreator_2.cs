using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using Cupboard;





public static class FBXCreator_2
{


	// 测试用, 生成一个 quad mesh go;
    public static GameObject CreateQuadGameObj( GameObject parent_, RectInfo rectInfo_, Color color_, string name_ )
    {
		int randomIdx = Random.Range(0, 1000);
		string name = name_ + "_" + randomIdx.ToString();
        var newgo = new GameObject(name);
		newgo.transform.SetParent( parent_.transform );

        // --- 随机颜色 --
        MeshRenderer meshRenderer = newgo.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		color_ = (color_==null) ? Random.ColorHSV() : color_;
        meshRenderer.sharedMaterial.SetColor("_BaseColor", color_ );

        // --- mesh:
        MeshFilter meshFilter = newgo.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
		mesh.name = "koko_Grid_" + randomIdx.ToString();

		// Vector3[] vertices = new Vector3[4]{
		// 	basePos_,
		// 	basePos_ + Vector3.right * w_,
		// 	basePos_ + Vector3.right * w_ + Vector3.up * h_,
		// 	basePos_ + Vector3.up * h_
		// };
		Vector3[] vertices = rectInfo_.GetCornerVertices();

		Vector2[] uv = new Vector2[4]{ 
			new Vector2( 0f, 0f),
			new Vector2( 1f, 0f),
			new Vector2( 1f, 1f),
			new Vector2( 0f, 1f)
		};

		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		Vector4[] tangents = new Vector4[4]{ tangent, tangent, tangent, tangent };
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		int[] triangles = new int[6]{
			3,1,0,
			3,2,1
		};
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		//---
		meshFilter.mesh = mesh;

		return newgo;
    }


	


}
