using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




namespace Cupboard{



/// <summary>
/// -1- 顶点 pos 存在精度误差, 人为约束精度, 并人为设计 hash 值以避免出现两个靠的特别近的顶点
/// -2- 同一个顶点, 在不同的面上分到的法线可能不同, 此时这个顶点就要被拆成 两个顶点 来处理
/// </summary>
public class Vertex
{
    public Vector3 pos; // 被限制过精度
    public Vector3 normal; // 归一化
    public string  poshash; // 基于 pos 生成的 hash, 
    public string hash; // 整个 vertex 的 hash

    public int idx = -1;


    public Vertex( Vector3 pos_, Vector3 normal_ ) 
    {
        pos = LimitVector3Precision(pos_,floatPresicion);
        normal = normal_.normalized;
        poshash = GetPosHash(pos);
        hash = poshash + "=" + GetNormalHash(normal); // todo: 无视长度和性能
    }

    public override string ToString()
    {
        return "pos:" + pos.ToString() + ";\n normal:" + normal.ToString() + ";\n hash" + hash;
    }

    // ====================================================

    public static int floatPresicion = 4;

    public static bool IsEque( Vertex a_, Vertex b_ )
    {
        return (a_.poshash == b_.poshash) && Vector3.Dot(a_.normal, b_.normal)>0.95f;
    }


    public static string GetPosHash( Vector3 pos_ ) 
    {
        string hash =   string.Format("{0:f4}", pos_.x) + '-' +
                        string.Format("{0:f4}", pos_.y) + '-' +
                        string.Format("{0:f4}", pos_.z);
        return hash;
    }

    public static string GetNormalHash( Vector3 normal_ ) 
    {
        string hash =   string.Format("{0:f2}", normal_.x) + '-' +
                        string.Format("{0:f2}", normal_.y) + '-' +
                        string.Format("{0:f2}", normal_.z);
        return hash;
    }


    public static Vector3 LimitVector3Precision( Vector3 old_, int presicion_ )
    {
        return new Vector3(
            (float)Math.Round((double)old_.x, presicion_),
            (float)Math.Round((double)old_.y, presicion_),
            (float)Math.Round((double)old_.z, presicion_)
        );
    }


}



}
