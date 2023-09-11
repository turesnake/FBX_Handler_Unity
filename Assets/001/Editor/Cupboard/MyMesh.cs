using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{



public class MyMesh
{

    // ----------------------------------------
    // -1-: 全局维护, 以便直接写入一个 mesh 中;
    // -2-: 目的是让一些靠的非常近的顶点, 直接算作一个 顶点;
    // k: vertex hash value
    // v: vertexIdx
    Dictionary<string,int> vertexHash_2_idxs = new Dictionary<string,int>();

    public List<Vertex> vertices = new List<Vertex>(); // mesh.vertices, mesh.normals

    public List<Vector2> uv =  new List<Vector2>(); // 对应 mesh.uv


    // ----------------------------------------
    public List<int> triangles =  new List<int>(); // 对应 Meh.triangles

     


    // ----------------------------------------
    public MyMesh() {}


    public int GetVertexIdx( Vertex v_ ) 
    {
        if( vertexHash_2_idxs.ContainsKey(v_.hash) ) 
        {
            // 存在此 Vertex 了:
            int retIdx = vertexHash_2_idxs[v_.hash];
            Debug.Assert( retIdx >= 0 && retIdx < vertices.Count );
            return retIdx;
        }
        else 
        {
            // 不存在此 Vertex:
            vertices.Add(v_);
            uv.Add(Vector2.zero); // just init
            int retIdx = vertices.Count-1;
            vertexHash_2_idxs.Add(v_.hash, retIdx);
            return retIdx;
        }
    }

    public Vertex GetVertex( int vertexIdx_ ) 
    {
        Debug.Assert( vertexIdx_ >= 0 && vertexIdx_ < vertices.Count );
        return vertices[vertexIdx_];
    }


    // ====================================
    // 拿着 4个顶点,拼出两个三角形
    // 参数传入者需保证, 矩形 一定是个 高>宽 的 纵向矩形;  (这将影响 uv 值)
    public int FourVertices( Vector3 pos_lb_, Vector3 pos_rb_, Vector3 pos_rt_, Vector3 pos_lt_, Vector3 normal_, PartitionDirection partitionDirection_ ) 
    {
        if( partitionDirection_ == PartitionDirection.Vertical  )
        {
            return _FourVertices( pos_lb_, pos_rb_, pos_rt_, pos_lt_, normal_ );
        }
        else 
        {
            //return _FourVertices( pos_lt_, pos_lb_, pos_rb_, pos_rt_, normal_ ); // 将 横着的矩形 旋转为 竖着的
            return _FourVertices( pos_lb_, pos_rb_, pos_rt_, pos_lt_, normal_ );
        }
    }


    public int _FourVertices( Vector3 pos_lb_, Vector3 pos_rb_, Vector3 pos_rt_, Vector3 pos_lt_, Vector3 normal_ ) 
    {
        Vertex lb = new Vertex( pos_lb_, normal_ );
        Vertex rb = new Vertex( pos_rb_, normal_ );
        Vertex rt = new Vertex( pos_rt_, normal_ );
        Vertex lt = new Vertex( pos_lt_, normal_ );
        int idx_0 = GetVertexIdx(lb);
        int idx_1 = GetVertexIdx(rb);
        int idx_2 = GetVertexIdx(rt);
        int idx_3 = GetVertexIdx(lt);
        ConnectVerticesToTriangle( idx_3, idx_2, idx_1 );
        ConnectVerticesToTriangle( idx_3, idx_1, idx_0 );

        // todo: 简单版:
        uv[idx_0] = new Vector2( 0f, 0f );
        uv[idx_1] = new Vector2( 1f, 0f );
        uv[idx_2] = new Vector2( 1f, 1f );
        uv[idx_3] = new Vector2( 0f, 1f );


        return 0;
    }

    // 顺时针 3 个点:
    public int ThreeVertices( Vector3 pos_0, Vector3 pos_1, Vector3 pos_2, Vector3 normal_ ) 
    {
        Vertex v_0 = new Vertex( pos_0, normal_ );
        Vertex v_1 = new Vertex( pos_1, normal_ );
        Vertex v_2 = new Vertex( pos_2, normal_ );
        int idx_0 = GetVertexIdx(v_0);
        int idx_1 = GetVertexIdx(v_1);
        int idx_2 = GetVertexIdx(v_2);
        ConnectVerticesToTriangle( idx_0, idx_1, idx_2 );
        return 0;
    }

    // 顺时针 3 个点:
    public void ConnectVerticesToTriangle( int idx_0, int idx_1, int idx_2 ) 
    {
        triangles.Add( idx_0 );
        triangles.Add( idx_1 );
        triangles.Add( idx_2 );
    }




    // ====================================
    public Mesh GetMesh( string name_ )
    {
        Mesh mesh = new Mesh();
        mesh.name = name_;
        //...
        int verticesLen = vertices.Count;

        Debug.Log( "vertices Len = " + verticesLen );
        Debug.Log( "triangles len = " + triangles.Count );

        Vector3[] vs = new Vector3[verticesLen];
        Vector3[] ns = new Vector3[verticesLen];
        for( int i=0; i<verticesLen; i++ )
        {
            vs[i] = vertices[i].pos;
            ns[i] = vertices[i].normal;
            Debug.Log( vertices[i].ToString() );
        }
        mesh.vertices = vs;
        mesh.uv = uv.ToArray();
        mesh.normals = ns;
        //---
        mesh.triangles = triangles.ToArray();
        return mesh;
    }



}



}
