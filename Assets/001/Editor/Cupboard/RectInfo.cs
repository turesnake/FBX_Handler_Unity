using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cupboard{


// 在 x,y 平面构成的矩形包围盒信息
public class RectInfo
{
    
    public Vector3 basePos = Vector3.zero; // 左下角pos
    public float w = 1f;
    public float h = 1f;


    public RectInfo( Vector3 basePos_, float w_, float h_ )
    {
        basePos = basePos_;
        w = w_;
        h = h_;
        Debug.Assert( w>0f && h>0f );
    }


    public override string ToString() 
    {
        string ret = string.Format("pos: {0}, w: {1}, h: {2}", basePos.ToString(), w, h );
        return ret;
    }
    
    // 四个顶点:
    public Vector3[] GetCornerVertices() 
    {
        Vector3[] vertices = new Vector3[4]{
			basePos,                                        // left-bottom
			basePos + Vector3.right * w,                    // right-bottom
			basePos + Vector3.right * w + Vector3.up * h,   // right-top
			basePos + Vector3.up * h                        // left-top
		};
        return vertices;
    }

    public Vector3 GetCenterPos() 
    {
        return basePos + Vector3.right * w * 0.5f + Vector3.up * h * 0.5f;
    }

    



}



}
