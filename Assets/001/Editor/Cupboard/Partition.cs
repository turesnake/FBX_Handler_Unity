using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

namespace Cupboard{


// 柜子隔板 - 简化版, 每个 partition 正面只有 4 个顶点, 不关心 partition 和其它 partition 的连接;
public class Partition
{

    public PartitionDirection partitionDirection = PartitionDirection.Vertical; // 切一刀的方向
    public float t = 0.5f; // [0f,1f] 切一刀的位置
    public Cell parentCell = null;

    public Segment1DMerge infiltrating_Ts = new Segment1DMerge(); // 浸润的 t 值区间; 非浸润区将在建模时被剔除 


    Vector3 from,   // left/bottom
            to;     // right/top
    public RectInfo rectInfo;

    public List<Vector3> vertexPoses; // 前4个顶点为边角: { leftBottom, rightBottom, rightTop, leftTop }, 后续的为 别的 partition 连上来的 连接点 (一定是成对添加的)



    MyMesh myMesh;


    public Partition( Cell parent_, PartitionDirection partitionDirection_, float t_ ) 
    {
        parentCell = parent_;
        myMesh = parentCell.myMesh;
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
        vertexPoses = new List<Vector3>( rectInfo.GetCornerVertices() );
    }

    

    // 添加 浸润 段:
    public void AddInfiltrating( Vector3 lowPos_, Vector3 highPos_ ) 
    {
        float lowT  = ProjectAndCalcT( from, to, lowPos_ );
        float highT = ProjectAndCalcT( from, to, highPos_ );
        Debug.Assert( lowT < highT );
        infiltrating_Ts.Add( new Segment1DMerge.Segment1D(lowT, highT) );
    }

    public void SetAllInfiltrating() 
    {
        infiltrating_Ts.Add( new Segment1DMerge.Segment1D(0f, 1f) );
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


    public void BuildMeshSimple() 
    {
        Vector3 from2to = to - from;
        foreach( var tSeg in infiltrating_Ts.segments ) 
        {
            Vector3 segFrom = from + from2to * tSeg.head;
            Vector3 segTo   = from + from2to * tSeg.end;

            if( partitionDirection == PartitionDirection.Vertical ) 
            {
                BuildPartitionBox(
                    new RectInfo(
                        segFrom - Vector3.right * CupboardStates.partitionRadius,
                        CupboardStates.partitionRadius * 2f, 
                        (segTo - segFrom).magnitude
                    ),
                    tSeg
                );
            }
            else 
            {
                BuildPartitionBox(
                    new RectInfo(
                        segFrom - Vector3.up * CupboardStates.partitionRadius,
                        (segTo - segFrom).magnitude, 
                        CupboardStates.partitionRadius * 2f
                    ),
                    tSeg
                );
            }
        }
    }

    public void BuildPartitionBox( RectInfo rectInfo_, Segment1DMerge.Segment1D segment_ ) 
    {
        // 紧贴着其它 partitionBox 的面, 可以被省略
        bool[] fourSide = new bool[4]{ true, true, true, true };// lb,rb, rt, lt
        // if( partitionDirection == PartitionDirection.Vertical )
        // {
        //     if( segment_.head < 0.001f )
        //     {
        //         fourSide[0] = false;
        //     }
        //     if( segment_.end > 0.999f )
        //     {
        //         fourSide[2] = false;
        //     }
        // }
        // else
        // {
        //     if( segment_.head < 0.001f )
        //     {
        //         fourSide[3] = false;
        //     }
        //     if( segment_.end > 0.999f )
        //     {
        //         fourSide[1] = false;
        //     }
        // }

        // 建模需要用到的 8 个顶点:
        Vector3 toFar = Vector3.forward * CupboardStates.partitionInnDepth;
        var fourPoses = rectInfo_.GetCornerVertices();
        //---:
        Vector3 front_LB = fourPoses[0];
        Vector3 front_RB = fourPoses[1];
        Vector3 front_RT = fourPoses[2];
        Vector3 front_LT = fourPoses[3];
        Vector3 back_LB = front_LB + toFar;
        Vector3 back_RB = front_RB + toFar;
        Vector3 back_RT = front_RT + toFar;
        Vector3 back_LT = front_LT + toFar;

        // 建模 正面:
        myMesh.FourVertices( front_LB, front_RB, front_RT, front_LT,  Vector3.back,  partitionDirection );
        // 建模 4 个边面:
        // if( fourSide[0] == true ) 
        // {
        //     myMesh.FourVertices( back_LB, back_RB, front_RB, front_LB,     Vector3.down,  partitionDirection );
        // }
        // if( fourSide[1] == true ) 
        // {
        //     myMesh.FourVertices( front_RB, back_RB, back_RT, front_RT,     Vector3.right,  partitionDirection );
        // }
        // if( fourSide[2] == true ) 
        // {
        //     myMesh.FourVertices( front_LT, front_RT, back_RT, back_LT,     Vector3.up,  partitionDirection );
        // }
        // if( fourSide[3] == true ) 
        // {
        //     myMesh.FourVertices( back_LB, front_LB, front_LT, back_LT,     Vector3.left,  partitionDirection );
        // }
    }



    
    public static float ProjectAndCalcT( Vector3 from_, Vector3 to_, Vector3 tgt_ ) 
    {
        Vector3 from2to = to_ - from_;
        float dotVal = Vector3.Dot( (tgt_ - from_), from2to.normalized );
        float ret = dotVal / from2to.magnitude;
        //Debug.Log( "计算 t = " + ret );
        return Mathf.Clamp01(ret);
    }


    // 用红球标出顶点
    public void DrawAllVertices(Transform parentTF_) 
    {
        foreach( var pos in vertexPoses )
        {
            FBXCreator_2.DrawPointGO(parentTF_, pos, 0.2f );
        }
    }


    


    




}

}
