using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;

// using csDelaunay;
// using System.Drawing.Printing;


using mattatz.Triangulation2DSystem;

using Cupboard;


public static class DelaunayTest
{

    [MenuItem("Tools/Delaunay_三角形工具")]
    public static void DoDelaunay()
    {
        // input points for a polygon2D contor
        List<Vector2> points = new List<Vector2>();

        // Add Vector2 to points
        // 顺序不要乱填
        
        
        points.Add(new Vector2(0f, 0f));

        points.Add(new Vector2(1f, 0f));
        points.Add(new Vector2(1f, 0.5f));
        points.Add(new Vector2(1f, 1f));

        points.Add(new Vector2(1f, 9f));
        points.Add(new Vector2(1f, 9.5f));
        points.Add(new Vector2(1f, 10f));

        points.Add(new Vector2(0f, 10f));




        // points.Add(new Vector2(0f, 0f));
        // points.Add(new Vector2(10f, 0f));
        // points.Add(new Vector2(10f, 1f));
        // points.Add(new Vector2(1f, 1f));
        // points.Add(new Vector2(1f, 9f));
        // points.Add(new Vector2(10f, 9f));
        // points.Add(new Vector2(10f, 10f));
        // points.Add(new Vector2(0f, 10f));


        // construct Polygon2D 
        Polygon2D polygon = Polygon2D.Contour(points.ToArray());


        // construct Triangulation2D with Polygon2D and threshold angle (18f ~ 27f recommended) 22.5f
        float thresholdAngle = 0.1f; // 这个角度足够小后, 插件就不会自作主张添加新顶点了;
        Triangulation2D triangulation = new Triangulation2D(polygon, thresholdAngle );

        // build a mesh from triangles in a Triangulation2D instance -- 没写入 uv 值
        Mesh mesh = triangulation.Build();

        

        //---
		string name = "Tris_";
        var newgo = new GameObject(name);

        // --- 随机颜色 --
        MeshRenderer meshRenderer = newgo.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        meshRenderer.sharedMaterial.SetColor("_BaseColor", Random.ColorHSV() );

        // --- mesh:
        MeshFilter meshFilter = newgo.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;


        // --- save to fbx:
        string filePath = System.IO.Path.Combine(Application.dataPath, name + ".fbx");
        //ModelExporter.ExportObject(filePath, Selection.objects[0]);
        ModelExporter.ExportObject(filePath, newgo );


    }


    

    [MenuItem("Tools/线段合并")]
    public static void DoSegmentMerge()
    {

        Segment1DMerge sm = new Segment1DMerge();

        sm.Add( new Segment1DMerge.Segment1D( 0f, 3f ) );
        sm.Add( new Segment1DMerge.Segment1D( 1f, 2f ) );
        sm.Add( new Segment1DMerge.Segment1D( 1f, 4f ) );
        sm.Add( new Segment1DMerge.Segment1D( 5f, 7f ) );
        //sm.Add( new Segment1DMerge.Segment1D( 1f, 5f ) );
        // sm.Add( new Segment1DMerge.Segment1D( 0f, 0.1f ) );

        foreach (var item in sm.segments)
        {
            Debug.Log( item.ToString() );
        }

    }




    [MenuItem("Tools/---测试---")]
    public static void SomeTest()
    {

        // Vector3 a = new Vector3( 0.001f, 0.3f, 0.06f );
        // Vector3 b = new Vector3( 0.001f, 0.3f, 0.06f );

        // int aHash = a.GetHashCode();
        // int bHash = b.GetHashCode();

        // Debug.Log( string.Format("aHash:{0}, bHash{1}", aHash, bHash) );


        // Vector3 k = new Vector3( 0.00315321f, 0.3234546f, 1200.06654123f );
        // Vector3 newk = CupboardStates.LimitVector3Precision(k);

        // var hash = CupboardStates.GetVertexHash(newk);
        // Debug.Log( "hash :" + hash );

        // Debug.Log( string.Format("x:{0}, y:{1}, z:{2}", newk.x, newk.y, newk.z  ) );



    }

    [MenuItem("Tools/---测试-2---")]
    public static void SomeTest2()
    {
        Doo();
    }


    // 将一个 partition 三角形化
    public static void Doo(  )
    {
        List<Vector3> vertices = new List<Vector3>(){
            new Vector3( 0f, 0f, 0f),
            new Vector3( 0f, 10f, 0f),
            new Vector3( 1f, 0f, 0f),
            new Vector3( 1f, 1f, 0f),
            new Vector3( 1f, 3f, 0f),
            new Vector3( 1f, 10f, 0f)
        };

        Vector3 basePos = new Vector3( 0.5f, -1f, 0f);
        Vector3 checkDir = Vector3.up;


        List<float> dots = new List<float>();
        for( int i=0; i<vertices.Count; i++ ) 
        {
            float v =  Vector3.SignedAngle( checkDir, vertices[i] - basePos, Vector3.forward );
            Debug.Log( "v = " + v );
            dots.Add( v );
        }










    }






    



}
