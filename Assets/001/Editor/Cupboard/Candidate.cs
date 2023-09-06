using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{


// 存放的物体 "候选者"
public class Candidate
{
    public Vector3 aabb;
    public Object obj;


    public Candidate( Vector3 aabb_, Object obj_ ) 
    {
        aabb = aabb_;
        obj = obj_; // 暂不关心它是否为空
    }

}

}

