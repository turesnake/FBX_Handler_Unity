using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{


// 柜子 空间格子, 二叉树结构
public class Cell
{
    public RectInfo rectInfo;   // 矩形包围盒信息
    public Cell cell_LB = null; // left/bottom
    public Cell cell_RT = null; // right/top
    public Partition partition; // 分隔板
    public Candidate candidate = null; // 放入的物体


    public Partition[] neighborPartitions = new Partition[4]{null,null,null,null}; // leftBottom, rightBottom, rightTop, leftTop

    
    public Vector3 BasePos => rectInfo.basePos;
    public float W => rectInfo.w;
    public float H => rectInfo.h;

    public MyMesh myMesh;



    public override string ToString() 
    {
        string ret = string.Format("{0}, isLeaf: {1}, hasCandidate: {2}", rectInfo.ToString(), IsLeaf(), (candidate!=null) );
        return ret;
    }


    public Cell( RectInfo rectInfo_, Partition[] neighborPartitions_, MyMesh myMesh_ )
    {
        rectInfo = rectInfo_;
        Debug.Assert(neighborPartitions_.Length == 4);
        neighborPartitions = neighborPartitions_;
        myMesh = myMesh_;
    }


    public void Split( PartitionDirection partitionDirection_, float t_ )
    {
        partition = new Partition( this, partitionDirection_, t_ );

        Vector3[] partitionCornerVertices = partition.rectInfo.GetCornerVertices();

        if(partitionDirection_ == PartitionDirection.Vertical) 
        {
            float leftW = W * t_;
            cell_LB = new Cell( 
                new RectInfo(
                    BasePos, 
                    leftW - CupboardStates.partitionRadius, 
                    H
                ),
                new Partition[4]{
                    neighborPartitions[0],
                    partition,
                    neighborPartitions[2],
                    neighborPartitions[3]
                },
                myMesh
            );
            cell_RT = new Cell( 
                new RectInfo(
                    BasePos + Vector3.right * (leftW + CupboardStates.partitionRadius),
                    W - leftW - CupboardStates.partitionRadius, 
                    H
                ),
                new Partition[4]{
                    neighborPartitions[0],
                    neighborPartitions[1],
                    neighborPartitions[2],
                    partition
                },
                myMesh
            );

            // 连接 下边 partition:
            if( neighborPartitions[0] != null ) 
            {
                neighborPartitions[0].AddJointedVertices(partitionCornerVertices[0], partitionCornerVertices[1], PartitionSide.LeftBottom );
            }
            // 连接 上边 partition:
            if( neighborPartitions[2] != null ) 
            {
                neighborPartitions[2].AddJointedVertices(partitionCornerVertices[3], partitionCornerVertices[2], PartitionSide.RightTop);
            }
        }
        else 
        {
            float bottomH = H * t_;
            cell_LB = new Cell( 
                new RectInfo(
                    BasePos, 
                    W,
                    bottomH - CupboardStates.partitionRadius
                ),
                new Partition[4]{
                    neighborPartitions[0],
                    neighborPartitions[1],
                    partition,
                    neighborPartitions[3]
                },
                myMesh
            );
            cell_RT = new Cell( 
                new RectInfo(
                    BasePos + Vector3.up * (bottomH + CupboardStates.partitionRadius),
                    W,
                    H - bottomH - CupboardStates.partitionRadius
                ),
                new Partition[4]{
                    partition,
                    neighborPartitions[1],
                    neighborPartitions[2],
                    neighborPartitions[3]
                },
                myMesh
            );

            // 连接 左边 partition:
            if( neighborPartitions[3] != null ) 
            {
                neighborPartitions[3].AddJointedVertices(partitionCornerVertices[0], partitionCornerVertices[3], PartitionSide.LeftBottom);
            }
            // 连接 右边 partition:
            if( neighborPartitions[1] != null ) 
            {
                neighborPartitions[1].AddJointedVertices(partitionCornerVertices[1], partitionCornerVertices[2], PartitionSide.RightTop);
            }
        }
    }

    // 填入 物品, 并将覆盖区域的 partition segment 浸润
    public void SetCandidate( Candidate candidate_ ) 
    {
        Debug.Assert( candidate_ != null );
        candidate = candidate_;

        float partitionDiameter = CupboardStates.partitionRadius * 2f;

        Vector3 topPos    = BasePos + Vector3.up * (H + partitionDiameter);
        Vector3 bottomPos = BasePos - Vector3.up * (partitionDiameter);
        Vector3 leftPos   = BasePos - Vector3.right * (partitionDiameter);
        Vector3 rightPos  = BasePos + Vector3.right * (W + partitionDiameter);

        if( neighborPartitions[0] != null )
        {
            neighborPartitions[0].AddInfiltrating( leftPos, rightPos );
        }
        if( neighborPartitions[1] != null )
        {
            neighborPartitions[1].AddInfiltrating( bottomPos, topPos );
        }
        if( neighborPartitions[2] != null )
        {
            neighborPartitions[2].AddInfiltrating( leftPos, rightPos );
        }
        if( neighborPartitions[3] != null )
        {
            neighborPartitions[3].AddInfiltrating( bottomPos, topPos );
        }
    }


    public bool IsLeaf() 
    {
        if( cell_LB == null ) 
        {
            Debug.Assert(cell_RT == null);
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool IsEmpty() 
    {
        return ( candidate == null );
    }

    public bool IsEmptyLeaf() 
    {
        return IsLeaf() && IsEmpty();
    }




}

}
