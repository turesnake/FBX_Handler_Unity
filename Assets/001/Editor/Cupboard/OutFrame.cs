using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{


// 柜子的外壳
public class OutFrame
{
    public MyMesh myMesh;

    public OutFrame( Vector3 outPosLB_, float w_, float h_, float frameWidth_ ) 
    {

        myMesh = new MyMesh();

        // 无脑手动拼出所有 四边形....
        PartitionDirection capboardDir = (CupboardStates.cupboardWidth >= CupboardStates.cupboardHeight) ? PartitionDirection.Horizontal : PartitionDirection.Vertical;

        Vector3 frontOut_LB = outPosLB_;
        Vector3 frontOut_RB = frontOut_LB + Vector3.right * w_;
        Vector3 frontOut_RT = frontOut_LB + Vector3.right * w_ + Vector3.up * h_;
        Vector3 frontOut_LT = frontOut_LB + Vector3.up * h_;

        Vector3 frontInn_LB = frontOut_LB + Vector3.right * frameWidth_ + Vector3.up * frameWidth_;
        Vector3 frontInn_RB = frontOut_RB - Vector3.right * frameWidth_ + Vector3.up * frameWidth_;
        Vector3 frontInn_RT = frontOut_RT - Vector3.right * frameWidth_ - Vector3.up * frameWidth_;
        Vector3 frontInn_LT = frontOut_LT + Vector3.right * frameWidth_ - Vector3.up * frameWidth_;

        // ===== 正面一圈边框:
        Vector3 normal = Vector3.back;
        myMesh.FourVertices( frontOut_LB, frontOut_RB, frontInn_RB, frontInn_LB, normal, PartitionDirection.Horizontal );
        myMesh.FourVertices( frontInn_RB, frontOut_RB, frontOut_RT, frontInn_RT, normal, PartitionDirection.Vertical );
        myMesh.FourVertices( frontInn_LT, frontInn_RT, frontOut_RT, frontOut_LT, normal, PartitionDirection.Horizontal );
        myMesh.FourVertices( frontOut_LB, frontInn_LB, frontInn_LT, frontOut_LT, normal, PartitionDirection.Vertical );


        // ===== 内侧 5 个面:
        Vector3 toNear = Vector3.forward * CupboardStates.partitionInnDepth;

        Vector3 backInn_LB = frontInn_LB + toNear;
        Vector3 backInn_RB = frontInn_RB + toNear;
        Vector3 backInn_RT = frontInn_RT + toNear;
        Vector3 backInn_LT = frontInn_LT + toNear;

        myMesh.FourVertices( frontInn_LB, frontInn_RB, backInn_RB, backInn_LB, Vector3.up, PartitionDirection.Horizontal  );
        myMesh.FourVertices( backInn_RB, frontInn_RB, frontInn_RT, backInn_RT, Vector3.left, PartitionDirection.Vertical );
        myMesh.FourVertices( backInn_LT, backInn_RT, frontInn_RT, frontInn_LT, Vector3.down, PartitionDirection.Horizontal );
        myMesh.FourVertices( frontInn_LB, backInn_LB, backInn_LT, frontInn_LT, Vector3.right, PartitionDirection.Vertical );
        myMesh.FourVertices( backInn_LB, backInn_RB, backInn_RT, backInn_LT, Vector3.back, capboardDir ); // 向前的面


        // ===== 外侧 5 个面:
        Vector3 toFar = Vector3.forward * (CupboardStates.partitionInnDepth + frameWidth_);

        Vector3 backOut_LB = frontOut_LB + toFar;
        Vector3 backOut_RB = frontOut_RB + toFar;
        Vector3 backOut_RT = frontOut_RT + toFar;
        Vector3 backOut_LT = frontOut_LT + toFar;

        myMesh.FourVertices( backOut_LB, backOut_RB, frontOut_RB, frontOut_LB, Vector3.down, PartitionDirection.Horizontal );
        myMesh.FourVertices( frontOut_RB, backOut_RB, backOut_RT, frontOut_RT, Vector3.right, PartitionDirection.Vertical );
        myMesh.FourVertices( frontOut_LT, frontOut_RT, backOut_RT, backOut_LT, Vector3.up, PartitionDirection.Horizontal );
        myMesh.FourVertices( backOut_LB, frontOut_LB, frontOut_LT, backOut_LT, Vector3.left, PartitionDirection.Vertical );
        myMesh.FourVertices( backOut_LT, backOut_RT, backOut_RB, backOut_LB, Vector3.forward, capboardDir ); // 向后的面
    }

    






}



}
