using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{


// 柜子的外壳
public class OutFrame
{
    
    //public Vector3 posInn_LB;

    public MyMesh myMesh;


    public OutFrame( Vector3 outPosLB_, float w_, float h_, float frameWidth_ ) 
    {

        myMesh = new MyMesh();

        // 无脑手动拼出所有 四边形....

        Vector3 posOut_LB = outPosLB_;
        Vector3 posOut_RB = posOut_LB + Vector3.right * w_;
        Vector3 posOut_RT = posOut_LB + Vector3.right * w_ + Vector3.up * h_;
        Vector3 posOut_LT = posOut_LB + Vector3.up * h_;

        Vector3 posInn_LB = posOut_LB + Vector3.right * frameWidth_ + Vector3.up * frameWidth_;
        Vector3 posInn_RB = posOut_RB - Vector3.right * frameWidth_ + Vector3.up * frameWidth_;
        Vector3 posInn_RT = posOut_RT - Vector3.right * frameWidth_ - Vector3.up * frameWidth_;
        Vector3 posInn_LT = posOut_LT + Vector3.right * frameWidth_ - Vector3.up * frameWidth_;

        // ===== 正面一圈边框:
        Vector3 normal = Vector3.back;
        myMesh.FourVertices( posOut_LB, posOut_RB, posInn_RB, posInn_LB, normal );
        myMesh.FourVertices( posInn_RB, posOut_RB, posOut_RT, posInn_RT, normal );
        myMesh.FourVertices( posInn_LT, posInn_RT, posOut_RT, posOut_LT, normal );
        myMesh.FourVertices( posOut_LB, posInn_LB, posInn_LT, posOut_LT, normal );


        // ===== 内侧 5 个面:
        Vector3 toNear = Vector3.forward * CupboardStates.partitionInnDepth;

        Vector3 posNear_LB = posInn_LB + toNear;
        Vector3 posNear_RB = posInn_RB + toNear;
        Vector3 posNear_RT = posInn_RT + toNear;
        Vector3 posNear_LT = posInn_LT + toNear;

        myMesh.FourVertices( posInn_LB, posInn_RB, posNear_RB, posNear_LB, Vector3.up );
        myMesh.FourVertices( posNear_RB, posInn_RB, posInn_RT, posNear_RT, Vector3.left );
        myMesh.FourVertices( posNear_LT, posNear_RT, posInn_RT, posInn_LT, Vector3.down );
        myMesh.FourVertices( posInn_LB, posNear_LB, posNear_LT, posInn_LT, Vector3.right );
        myMesh.FourVertices( posNear_LB, posNear_RB, posNear_RT, posNear_LT, Vector3.back );


        // ===== 外侧 5 个面:
        Vector3 toFar = Vector3.forward * (CupboardStates.partitionInnDepth + frameWidth_);

        Vector3 posFar_LB = posOut_LB + toFar;
        Vector3 posFar_RB = posOut_RB + toFar;
        Vector3 posFar_RT = posOut_RT + toFar;
        Vector3 posFar_LT = posOut_LT + toFar;

        myMesh.FourVertices( posFar_LB, posFar_RB, posOut_RB, posOut_LB, Vector3.down );
        myMesh.FourVertices( posOut_RB, posFar_RB, posFar_RT, posOut_RT, Vector3.right );
        myMesh.FourVertices( posOut_LT, posOut_RT, posFar_RT, posFar_LT, Vector3.up );
        myMesh.FourVertices( posFar_LB, posOut_LB, posOut_LT, posFar_LT, Vector3.left );
        myMesh.FourVertices( posFar_LT, posFar_RT, posFar_RB, posFar_LB, Vector3.forward );

    }

    






}



}
