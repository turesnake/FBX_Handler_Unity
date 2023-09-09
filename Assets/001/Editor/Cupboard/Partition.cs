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

    public Segment1DMerge infiltrating_Ts = new Segment1DMerge(); // 浸润的 t 值区间; 非浸润区将在建模时被剔除 


    Vector3 from,   // left/bottom
            to;     // right/top
    public RectInfo rectInfo;

    public List<Vector3> vertexPoses; // 前4个顶点为边角: { leftBottom, rightBottom, rightTop, leftTop }, 后续的为 别的 partition 连上来的 连接点 (一定是成对添加的)

    // todo: 暂时没用上...
    bool isLeftBottomSideAddVertices = false;
    bool isRightTopSideAddVertices = false;

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


        // // 试验: 长边塞入一对顶点:
        // if( partitionDirection_ == PartitionDirection.Vertical )
        // {
        //     vertices.Add( (vertices[0] + vertices[3]) * 0.5f );
        //     vertices.Add( (vertices[1] + vertices[2]) * 0.5f );
        // }
        // else 
        // {
        //     vertices.Add( (vertices[0] + vertices[1]) * 0.5f );
        //     vertices.Add( (vertices[3] + vertices[2]) * 0.5f );
        // }

       


    }

    

    public void AddJointedVertices( Vector3 a_, Vector3 b_, PartitionSide partitionSide_ ) 
    {
        vertexPoses.Add( a_ );
        vertexPoses.Add( b_ );
        if(partitionSide_ == PartitionSide.LeftBottom)
        {
            isLeftBottomSideAddVertices = true;
        }
        else 
        {
            isRightTopSideAddVertices = true;
        }
    }


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


    // 排序之后才方便被 Triangulation2DSystem 自动生成 三角形信息;
    // void SortVerties() 
    // {
    //     Vector3 centerPos = rectInfo.GetCenterPos();

    //     var baseDir = ( partitionDirection == PartitionDirection.Vertical ) ? Vector3.up : Vector3.right;

    //     vertexPoses.Sort( (a, b) =>{
    //         float angleA= Vector3.SignedAngle( baseDir, a - centerPos, Vector3.forward );
    //         float angleB= Vector3.SignedAngle( baseDir, b - centerPos, Vector3.forward );
    //         return (angleA < angleB) ? -1 : 1; 
    //     } );
    // }



    public class PPos 
    {
        public Vector3 pos;
        public float angle;

        public PPos( Vector3 pos_, float angle_ )
        {
            pos = pos_;
            angle = angle_;
        }
    }

    public void BuildMesh() 
    {

        Vector3 centerPos = rectInfo.GetCenterPos();
        Vector3 checkDir;
        if( partitionDirection == PartitionDirection.Vertical )
        {
            checkDir = Vector3.up;
        }
        else 
        {
            checkDir = Vector3.right;
        }

        List<PPos> PPoses = new List<PPos>();
        for( int k=0; k<vertexPoses.Count; k++ ) 
        {
            float v =  Vector3.SignedAngle( checkDir, vertexPoses[k] - centerPos, Vector3.forward );
            PPoses.Add( new PPos( vertexPoses[k], v ) );
        }

        List<PPos> pPoses_LB = new List<PPos>(); // 负值
        List<PPos> pPoses_RT = new List<PPos>(); // 正值

        foreach( PPos p in PPoses )
        {
            List<PPos> container = null;
            if( p.angle < 0f )
            {
                container = ( partitionDirection == PartitionDirection.Vertical ) ? pPoses_RT : pPoses_LB;
            }
            else 
            {
                container = ( partitionDirection == PartitionDirection.Vertical ) ? pPoses_LB : pPoses_RT;
            }
            container.Add( p );
        }

        // 排序, 之后都会从 t0 -> t1
        if(partitionDirection == PartitionDirection.Vertical)
        {
            pPoses_LB.Sort( (x,y)=>{ return (x.angle > y.angle) ? -1 : 1 ; } );
            pPoses_RT.Sort( (x,y)=>{ return (x.angle < y.angle) ? -1 : 1 ; } );
        }
        else 
        {
            pPoses_LB.Sort( (x,y)=>{ return (x.angle < y.angle) ? -1 : 1 ; } );
            pPoses_RT.Sort( (x,y)=>{ return (x.angle > y.angle) ? -1 : 1 ; } );
        }

        // //-- 排序结果 debug:
        // Debug.Log( "=== pPoses_LB ===" );
        // foreach( PPos p in pPoses_LB )
        // {
        //     Debug.Log( "pos:" + p.pos.ToString() + ", angle = " + p.angle );
        // }

        // Debug.Log( "=== pPoses_RT ===" );
        // foreach( PPos p in pPoses_RT )
        // {
        //     Debug.Log( "pos:" + p.pos.ToString() + ", angle = " + p.angle );
        // }

        bool is_LB_sml = pPoses_LB.Count <= pPoses_RT.Count; // 会影响 三角形生成时的面朝向

        int lenSml = is_LB_sml ? pPoses_LB.Count : pPoses_RT.Count;
        int lenBig = is_LB_sml ? pPoses_RT.Count : pPoses_LB.Count;

        List<PPos> smlPPos = is_LB_sml ? pPoses_LB : pPoses_RT;     // 短列
        List<PPos> bigPPos  = is_LB_sml ? pPoses_RT : pPoses_LB;    // 长列

        Vector3 normal = Vector3.back;

        // 拼出所有 四边形:
        int i = 1;
        for( ; i < lenSml; i++ )
        {
            if(partitionDirection == PartitionDirection.Vertical)
            {
                var noUse = is_LB_sml ?
                    myMesh.FourVertices( smlPPos[i-1].pos, bigPPos[i-1].pos, bigPPos[i].pos, smlPPos[i].pos,    normal ) :
                    myMesh.FourVertices( bigPPos[i-1].pos, smlPPos[i-1].pos, smlPPos[i].pos, bigPPos[i].pos,    normal );
            }
            else 
            {
                var noUse = is_LB_sml ?
                    myMesh.FourVertices( smlPPos[i-1].pos, smlPPos[i].pos, bigPPos[i].pos, bigPPos[i-1].pos,        normal ) :
                    myMesh.FourVertices( smlPPos[i-1].pos, bigPPos[i-1].pos, bigPPos[i].pos, smlPPos[i].pos,        normal );
            }
        }

        // 拼出所有 三角形:
        for( ; i < lenBig; i++ )
        {
            if(partitionDirection == PartitionDirection.Vertical)
            {
                var noUse = is_LB_sml ? 
                    myMesh.ThreeVertices( smlPPos[lenSml-1].pos, bigPPos[i].pos, bigPPos[i-1].pos,  normal ) :
                    myMesh.ThreeVertices( smlPPos[lenSml-1].pos, bigPPos[i-1].pos, bigPPos[i].pos,  normal );
            }
            else 
            {
                var noUse = is_LB_sml ? 
                    myMesh.ThreeVertices( smlPPos[lenSml-1].pos, bigPPos[i-1].pos, bigPPos[i].pos,  normal ) :
                    myMesh.ThreeVertices(  bigPPos[i-1].pos, smlPPos[lenSml-1].pos, bigPPos[i].pos, normal );
            }
        }
    }



    // 遍历 partition 四周一圈边, 若某个边是 单面边, 就立刻为它建立 inn face
    public void SearchOutEdgeAndBuildInnFace() 
    {        
        Vector3 centerPos = rectInfo.GetCenterPos();

        var baseDir = ( partitionDirection == PartitionDirection.Vertical ) ? Vector3.up : Vector3.right;

        vertexPoses.Sort( (a, b) =>{
            float angleA= Vector3.SignedAngle( baseDir, a - centerPos, Vector3.forward );
            float angleB= Vector3.SignedAngle( baseDir, b - centerPos, Vector3.forward );
            return (angleA < angleB) ? -1 : 1; 
        } );

        Vector3 normal = Vector3.up;

        for( int l=0; l<vertexPoses.Count; l++ )
        {
            int r = (l==vertexPoses.Count-1) ? 0 : l+1;
            //---
            Vector3 pos_l = vertexPoses[l];
            Vector3 pos_r = vertexPoses[r];
            if( myMesh.IsEdgeOnTheBorder(pos_l,pos_r) == false )
            {
                continue;
            }
            Vector3 moveInn = Vector3.forward * CupboardStates.partitionInnDepth;

            
            myMesh.FourVertices( pos_l + moveInn, pos_r + moveInn, pos_r, pos_l,    normal );
        }
    }




    // // 拿着 4个顶点,拼出两个三角形
    // public int FourVertices( Vector3 lb_, Vector3 rb_, Vector3 rt_, Vector3 lt_ ) 
    // {

    //     int idx_0 = CupboardStates.GetVertexIdx(lb_);
    //     int idx_1 = CupboardStates.GetVertexIdx(rb_);
    //     int idx_2 = CupboardStates.GetVertexIdx(rt_);
    //     int idx_3 = CupboardStates.GetVertexIdx(lt_);

    //     ConnectVerticesToTriangle( idx_3, idx_2, idx_1 );
    //     ConnectVerticesToTriangle( idx_3, idx_1, idx_0 );
    //     return 0;
    // }

    // // 顺时针 3 个点:
    // public int ThreeVertices( Vector3 pos_0, Vector3 pos_1, Vector3 pos_2 ) 
    // {
    //     int idx_0 = CupboardStates.GetVertexIdx(pos_0);
    //     int idx_1 = CupboardStates.GetVertexIdx(pos_1);
    //     int idx_2 = CupboardStates.GetVertexIdx(pos_2);
    //     ConnectVerticesToTriangle( idx_0, idx_1, idx_2 );
    //     return 0;
    // }

    // 顺时针 3 个点:
    // public void ConnectVerticesToTriangle( int idx_0, int idx_1, int idx_2 ) 
    // {
    //     CupboardStates.triangles.Add( idx_0 );
    //     CupboardStates.triangles.Add( idx_1 );
    //     CupboardStates.triangles.Add( idx_2 );

    //     CupboardStates.RecordEdge( idx_0, idx_1 );
    //     CupboardStates.RecordEdge( idx_0, idx_2 );
    //     CupboardStates.RecordEdge( idx_1, idx_2 );


    // }







    

}

}
