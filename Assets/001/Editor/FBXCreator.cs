using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;







public static class FBXCreator
{


	// 生成一个 多格子的 矩形, 示例:
    public static Mesh CreateMesh_1()
    {

        Mesh mesh = new Mesh();
		mesh.name = "koko_Grid";

        int xSize = 2;
        int ySize = 2;

		Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];

		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] 							= vi;
				triangles[ti + 3] = triangles[ti + 2] 	= vi + 1;
				triangles[ti + 4] = triangles[ti + 1] 	= vi + xSize + 1;
				triangles[ti + 5] 						= vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

        return mesh;
    }


	// 无视 uv 值 (写相同的) 证明可行,
	public static Mesh CreateMesh_FakeUV()
    {
        Mesh mesh = new Mesh();
		mesh.name = "koko_Grid";

        int xSize = 2;
        int ySize = 2;

		Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];

		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x, y);
				//uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				uv[i] = new Vector2( 0.1f, 0.1f );
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] 							= vi;
				triangles[ti + 3] = triangles[ti + 2] 	= vi + 1;
				triangles[ti + 4] = triangles[ti + 1] 	= vi + xSize + 1;
				triangles[ti + 5] 						= vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

        return mesh;
    }


}
