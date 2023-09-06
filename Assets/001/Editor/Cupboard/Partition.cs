using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

using mattatz.Triangulation2DSystem;

namespace Cupboard{


// 柜子隔板
public class Partition
{

    public PartitionDirection partitionDirection = PartitionDirection.Vertical; // 切一刀的方向
    public float t = 0.5f; // [0f,1f] 切一刀的位置
    public Cell parentCell = null;

    public SegmentMerge infiltrating_Ts = new SegmentMerge(); // 浸润的 t 值区间; 非浸润区将在建模时被剔除 


    Vector3 from,   // left/bottom
            to;     // right/top
    public RectInfo rectInfo;

    public List<Vector3> vertices; // 前4个顶点为边角: { leftBottom, rightBottom, rightTop, leftTop }, 后续的为 别的 partition 连上来的 连接点 (一定是成对添加的)


    public Partition( Cell parent_, PartitionDirection partitionDirection_, float t_ ) 
    {
        parentCell = parent_;
        partitionDirection = partitionDirection_;
        t = t_;
        //---
        if( partitionDirection_ == PartitionDirection.Vertical )
        {
            from = parentCell.BasePos + Vector3.right * parentCell.W * t; // bottom
            to = from + Vector3.up * parentCell.H; // top
            //---
            rectInfo = new RectInfo(
                from - Vector3.right * CupboardStates.partitionRadius, 
                CupboardStates.partitionRadius * 2f, 
                parentCell.H
            );
        }
        else 
        {
            from = parentCell.BasePos + Vector3.up * parentCell.H * t; // left
            to = from + Vector3.right * parentCell.W; // right 
            //---
            rectInfo = new RectInfo(
                from - Vector3.up * CupboardStates.partitionRadius, 
                parentCell.W,
                CupboardStates.partitionRadius * 2f
            );
        }

        //-- 有一定几率, 这个 partition 整个一条都是浸润的:
        if( Random.value < CupboardStates.partitionFullInfiltratingPercent ) 
        {
            SetAllInfiltrating();
        }

        // 塞入 4 个边角顶点:
        vertices = new List<Vector3>( rectInfo.GetCornerVertices() );


        // 试验: 长边塞入一对顶点:
        if( partitionDirection_ == PartitionDirection.Vertical )
        {
            vertices.Add( (vertices[0] + vertices[3]) * 0.5f );
            vertices.Add( (vertices[1] + vertices[2]) * 0.5f );
        }
        else 
        {
            vertices.Add( (vertices[0] + vertices[1]) * 0.5f );
            vertices.Add( (vertices[3] + vertices[2]) * 0.5f );
        }


    }

    

    public void AddJointedVertices( Vector3 a_, Vector3 b_ ) 
    {
        vertices.Add( a_ );
        vertices.Add( b_ );
    }


    public void AddInfiltrating( Vector3 lowPos_, Vector3 highPos_ ) 
    {
        float lowT  = ProjectAndCalcT( from, to, lowPos_ );
        float highT = ProjectAndCalcT( from, to, highPos_ );
        Debug.Assert( lowT < highT );
        infiltrating_Ts.Add( new SegmentMerge.Segment(lowT, highT) );
    }

    public void SetAllInfiltrating() 
    {
        infiltrating_Ts.Add( new SegmentMerge.Segment(0f, 1f) );
    }



    public List<RectInfo> GetInfiltratingRectInfos() 
    {
        List<RectInfo> rets = new List<RectInfo>();

        Vector3 from2to = to - from;

        foreach( var tSeg in infiltrating_Ts.segments ) 
        {
            Vector3 segFrom = from + from2to * tSeg.head;
            Vector3 segTo   = from + from2to * tSeg.end;

            if( partitionDirection == PartitionDirection.Vertical ) 
            {
                rets.Add( new RectInfo(
                    segFrom - Vector3.right * CupboardStates.partitionRadius,
                    CupboardStates.partitionRadius * 2f, 
                    (segTo - segFrom).magnitude
                ));
            }
            else 
            {
                rets.Add( new RectInfo(
                    segFrom - Vector3.up * CupboardStates.partitionRadius,
                    (segTo - segFrom).magnitude, 
                    CupboardStates.partitionRadius * 2f
                ));
            }
        }
        return rets;
    }



    
    public static float ProjectAndCalcT( Vector3 from_, Vector3 to_, Vector3 tgt_ ) 
    {
        Vector3 from2to = to_ - from_;
        float dotVal = Vector3.Dot( (tgt_ - from_), from2to.normalized );
        float ret = dotVal / from2to.magnitude;
        Debug.Log( "计算 t = " + ret );
        return Mathf.Clamp01(ret);
    }


    public void DrawAllVertices(Transform parentTF_) 
    {
        foreach( var pos in vertices )
        {
            FBXCreator_2.DrawPointGO(parentTF_, pos, 0.2f );
        }
    }


    // 排序之后才方便被 Triangulation2DSystem 自动生成 三角形信息;
    void SortVerties() 
    {
        Vector3 centerPos = rectInfo.GetCenterPos();

        var baseDir = ( partitionDirection == PartitionDirection.Vertical ) ? Vector3.up : Vector3.right;

        vertices.Sort( (a, b) =>{
            float angleA= Vector3.SignedAngle( baseDir, a - centerPos, Vector3.forward );
            float angleB= Vector3.SignedAngle( baseDir, b - centerPos, Vector3.forward );
            return (angleA < angleB) ? -1 : 1; 
        } );
    }


    public void BuildMesh() 
    {
        SortVerties();

        Vector2[] vectors2D = new Vector2[vertices.Count];
        for( int i=0; i<vertices.Count; i++ )
        {
            vectors2D[i] = vertices[i];
        }

        // construct Polygon2D 
        Polygon2D polygon = Polygon2D.Contour(vectors2D);
        // construct Triangulation2D with Polygon2D and threshold angle (18f ~ 27f recommended) 22.5f
        float thresholdAngle = 0.1f; // 这个角度足够小后, 插件就不会自作主张添加新顶点了;
        Triangulation2D triangulation = new Triangulation2D(polygon, thresholdAngle );
        // build a mesh from triangles in a Triangulation2D instance -- 没设置 uv 值
        Mesh mesh = triangulation.Build();

        // 装配 vertices, triangles
        for( int i=0; i<mesh.triangles.Length; i++ ) 
        {
            Vector3 pos = mesh.vertices[mesh.triangles[i]];
            int vertexIdx = CupboardStates.GetVertexIdx(pos);
            CupboardStates.triangles.Add( vertexIdx );
        }


    }
    

}

}
