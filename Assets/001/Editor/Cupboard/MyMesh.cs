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

    public List<Vector3> verticePoses = new List<Vector3>();
    // 不关心 normal
    Dictionary<string,int> vertexPosHash_2_idxs = new Dictionary<string,int>();
    // 存储一条边 被几个三角形用过了:
    Dictionary<ulong,int> edgeHash_2_usedTimes = new Dictionary<ulong, int>();


    public void Debug_edgeHash_2_usedTimes() 
    {
        Debug.Log( "debug -- edgeHash_2_usedTimes ====" );
        foreach( var p in edgeHash_2_usedTimes )
        {
            Debug.Log( "k:" + p.Key + "; v:" + p.Value );
        }
    }

    public int GetVertexPosIdx( Vector3 pos_ ) 
    {
        string posHash = Vertex.GetPosHash(pos_);
        if( vertexPosHash_2_idxs.ContainsKey(posHash) ) 
        {
            // 存在此 Vertex 了:
            int retIdx = vertexPosHash_2_idxs[posHash];
            Debug.Assert( retIdx >= 0 && retIdx < verticePoses.Count );
            return retIdx;
        }
        else 
        {
            // 不存在此 Vertex:
            verticePoses.Add(pos_);
            int retIdx = verticePoses.Count-1;
            vertexPosHash_2_idxs.Add(posHash, retIdx);
            return retIdx;
        }
    }


    public void RecordEdge( int vertexIdx_a_, int vertexIdx_b_ ) 
    {
        Vector3 pos_a = GetVertex(vertexIdx_a_).pos;
        Vector3 pos_b = GetVertex(vertexIdx_b_).pos;
        int posIdx_a = GetVertexPosIdx(pos_a);
        int posIdx_b = GetVertexPosIdx(pos_b);
        //---
        ulong edgeHash = GetEdgeHash( posIdx_a, posIdx_b );
        if( edgeHash_2_usedTimes.ContainsKey( edgeHash ) )
        {
            edgeHash_2_usedTimes[edgeHash] ++;
            // 不准被 2个以上三角形公用
            Debug.Assert( edgeHash_2_usedTimes[edgeHash] <= 2,     "a:" + vertexIdx_a_ + ": " + vertices[vertexIdx_a_].ToString() + 
                                                                "\n b:" + vertexIdx_b_ + ": " + vertices[vertexIdx_b_].ToString() ); 
        }
        else 
        {
            edgeHash_2_usedTimes[edgeHash] = 1;
        }
    }


    // 在边界上的 edge, 可以向后延展出一个面;
    public bool IsEdgeOnTheBorder( Vector3 pos_a_, Vector3 pos_b_ ) 
    {
        return IsEdgeOnTheBorder( GetVertexPosIdx(pos_a_), GetVertexPosIdx(pos_b_) );
    }


    public bool IsEdgeOnTheBorder( int posIdx_a_, int posIdx_b_ ) 
    {
        ulong edgeHash = GetEdgeHash( posIdx_a_, posIdx_b_ );
        Debug.Assert(edgeHash_2_usedTimes.ContainsKey(edgeHash));
        int usedTimes = edgeHash_2_usedTimes[edgeHash];
        Debug.Assert( usedTimes == 1 || usedTimes == 2, "usedTimes = " + usedTimes );
        return usedTimes == 1;
    }


    static ulong GetEdgeHash( int posIdx_a_, int posIdx_b_ ) 
    {
        ulong sml = (ulong)Mathf.Min( posIdx_a_, posIdx_b_);
        ulong big = (ulong)Mathf.Max( posIdx_a_, posIdx_b_);
        return (sml << 32) + big;
    }


    // ====================================

    // 拿着 4个顶点,拼出两个三角形
    public int FourVertices( Vector3 pos_lb_, Vector3 pos_rb_, Vector3 pos_rt_, Vector3 pos_lt_, Vector3 normal_ ) 
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
        RecordEdge( idx_0, idx_1 );
        RecordEdge( idx_0, idx_2 );
        RecordEdge( idx_1, idx_2 );
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
        mesh.normals = ns;
        //---
        mesh.triangles = triangles.ToArray();
        return mesh;
    }



}



}
