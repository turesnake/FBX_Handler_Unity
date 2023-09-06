using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;


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

    public List<Vector3> jointedVertices = new List<Vector3>(); // 别的 partition 连上来的 连接点 (一定是成对添加的)


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
        }
        else 
        {
            from = parentCell.BasePos + Vector3.up * parentCell.H * t; // left
            to = from + Vector3.right * parentCell.W; // right 
        }

        //-- 有一定几率, 这个 partition 整个一条都是浸润的:
        if( Random.value < CupboardStates.partitionFullInfiltratingPercent ) 
        {
            SetAllInfiltrating();
        }
    }

    

    public void AddJointedVertices( Vector3 a_, Vector3 b_ ) 
    {
        jointedVertices.Add( a_ );
        jointedVertices.Add( b_ );
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
    

}

}
