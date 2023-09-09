using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


namespace Cupboard{


public enum PartitionDirection
{
    Vertical = 1,   // 竖着切一刀
    Horizontal = 2, // 横向切一刀
}


public enum CornerType
{
    LeftBottom = 0,
    RightBottom = 1,
    RightTop = 2,
    LeftTop = 3,
}

public enum HorizontalType
{
    Left = 1,
    Right = 2,
}

public enum VerticalType
{
    Bottom =1,
    Top = 2,
}


// Partition 的哪一侧:
public enum PartitionSide
{
    LeftBottom = 1,
    RightTop = 2,
}






public static class CupboardStates
{
    public static float cupboardWidth  = 20f;
    public static float cupboardHeight = 20f;

    public static float partitionRadius  = 0.15f; // 隔板厚度半径 0.25f

    public static float partitionFullInfiltratingPercent  = 1f; // 有些隔板整个都会被保留, 不会被删减, 选择保留的随机百分比; 推荐: 0.5ff

    public static float minGap = 0.6f; // 统计所有 candidates 的aabb 的最短边长;

    public static int floatPresicion = 4;


    public static Corner[] corners = new Corner[4]
    {
        new Corner(){ type = CornerType.LeftBottom,    horizontalType = HorizontalType.Left,   verticalType = VerticalType.Bottom,    wVec = Vector3.right,   hVec = Vector3.up },
        new Corner(){ type = CornerType.RightBottom,   horizontalType = HorizontalType.Right,  verticalType = VerticalType.Bottom,    wVec = -Vector3.right,  hVec = Vector3.up },
        new Corner(){ type = CornerType.RightTop,      horizontalType = HorizontalType.Right,  verticalType = VerticalType.Top,       wVec = -Vector3.right,  hVec = -Vector3.up },
        new Corner(){ type = CornerType.LeftTop,       horizontalType = HorizontalType.Left,   verticalType = VerticalType.Top,       wVec = Vector3.right,   hVec = -Vector3.up }
    };


    // 限制 vector 小数点精度
    public static Vector3 LimitVector3Precision( Vector3 old_ )
    {
        return new Vector3(
            (float)Math.Round((double)old_.x, floatPresicion),
            (float)Math.Round((double)old_.y, floatPresicion),
            (float)Math.Round((double)old_.z, floatPresicion)
        );
    }


    public static void Clear()
    {
        vertexHash_2_idxs.Clear();
        vertices.Clear();
        triangles.Clear();
    }

    // ========================================= 全局 顶点pos 管理 ================================================
    // -2-: 全局维护, 以便直接写入一个 mesh 中;
    // -2-: 目的是让一些靠的非常近的顶点, 直接算作一个 顶点;
    // k: vertex hash value
    // v: vertexIdx
    static Dictionary<string,int> vertexHash_2_idxs = new Dictionary<string,int>();
    static List<Vector3> vertices =  new List<Vector3>(); // 用 vertexIdx 来获得具体 pos, 对应: Meh.vertices

    public static List<Vector3> GetVertices() 
    {
        return vertices;
    }


    // Vector3.GetHashCode() 重合率很高, 改用本函数, 一定不重合
    public static string GetVertexHash( Vector3 pos_ ) 
    {
        string hash =   string.Format("{0:f4}", pos_.x) + "-" +
                        string.Format("{0:f4}", pos_.y) + "-" +
                        string.Format("{0:f4}", pos_.z);
        return hash;
    }


    public static int GetVertexIdx( Vector3 pos_ ) 
    {
        pos_ = LimitVector3Precision(pos_); // 限制精度
        string posHash = GetVertexHash(pos_); 
        if( vertexHash_2_idxs.ContainsKey(posHash) ) 
        {
            // 存在此 Vertex 了:
            int retIdx = vertexHash_2_idxs[posHash];
            Debug.Assert( retIdx >= 0 && retIdx < vertices.Count );
            return retIdx;
        }
        else 
        {
            // 不存在此 Vertex:
            vertices.Add(pos_);
            int retIdx = vertices.Count-1;
            vertexHash_2_idxs.Add(posHash, retIdx);
            return retIdx;
        }
    }

    public static Vector3 GetVertex( int vertexIdx_ ) 
    {
        Debug.Assert( vertexIdx_ >= 0 && vertexIdx_ < vertices.Count );
        return vertices[vertexIdx_];
    }


    // ========================================= 全局 三角形 管理 ================================================
    public static List<int> triangles =  new List<int>(); // 对应 Meh.triangles

    // 存储一条边 被几个三角形用过了:
    static Dictionary<ulong,int> edgeHash_2_usedTimes = new Dictionary<ulong, int>();


    public static ulong GetEdgeHash( int vertexIdx_a_, int vertexIdx_b_ ) 
    {
        ulong sml = (ulong)Mathf.Min( vertexIdx_a_, vertexIdx_b_);
        ulong big = (ulong)Mathf.Max( vertexIdx_a_, vertexIdx_b_);
        return (sml << 32) + big;
    }

    public static void RecordEdge( int vertexIdx_a_, int vertexIdx_b_ ) 
    {
        ulong edgeHash = GetEdgeHash( vertexIdx_a_, vertexIdx_b_ );
        if( edgeHash_2_usedTimes.ContainsKey( edgeHash ) )
        {
            edgeHash_2_usedTimes[edgeHash] ++;
            // 不准被 2个以上三角形公用
            Debug.Assert( edgeHash_2_usedTimes[edgeHash] <= 2,     "a:" + vertexIdx_a_ + ": " + vertices[vertexIdx_a_].ToString() + 
                                                                "\n b:" + vertexIdx_b_ + ": " + vertices[vertexIdx_b_].ToString() ); 
        }
        else 
        {
            edgeHash_2_usedTimes[edgeHash] = 0;
        }
    }

    // 在边界上的 edge, 可以向后延展出一个面;
    public static bool IsEdgeOnTheBorder( Vector3 a_, Vector3 b_ ) 
    {
        ulong edgeHash = GetEdgeHash( GetVertexIdx(a_), GetVertexIdx(b_) );
        Debug.Assert(edgeHash_2_usedTimes.ContainsKey(edgeHash));
        int usedTimes = edgeHash_2_usedTimes[edgeHash];
        Debug.Assert( usedTimes == 1 || usedTimes == 2 );
        return usedTimes == 1;
    }

}

}
