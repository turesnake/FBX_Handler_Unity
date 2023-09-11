using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;


using Cupboard;


public static class DelaunayTest
{

    

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
