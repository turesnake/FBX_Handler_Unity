using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


namespace Cupboard{


public enum PartitionDirection
{
    Vertical = 1,   // 竖着切一刀
    Horizontal = 2, // 横向切一刀
}


public enum CornerType
{
    LeftBottom = 0,
    RightBottom = 1,
    RightTop = 2,
    LeftTop = 3,
}

public enum HorizontalType
{
    Left = 1,
    Right = 2,
}

public enum VerticalType
{
    Bottom =1,
    Top = 2,
}


// Partition 的哪一侧:
public enum PartitionSide
{
    LeftBottom = 1,
    RightTop = 2,
}








public static class CupboardStates
{
    public static float cupboardWidth  = 20f;
    public static float cupboardHeight = 20f;

    public static float partitionRadius  = 0.15f; // 隔板厚度半径 0.25f

    public static float partitionInnDepth = 3f; // 柜子内深

    public static float partitionFullInfiltratingPercent  = 1f; // 有些隔板整个都会被保留, 不会被删减, 选择保留的随机百分比; 推荐: 0.5ff

    public static float minGap = 0.6f; // 统计所有 candidates 的aabb 的最短边长;



    public static Corner[] corners = new Corner[4]
    {
        new Corner(){ type = CornerType.LeftBottom,    horizontalType = HorizontalType.Left,   verticalType = VerticalType.Bottom,    wVec = Vector3.right,   hVec = Vector3.up },
        new Corner(){ type = CornerType.RightBottom,   horizontalType = HorizontalType.Right,  verticalType = VerticalType.Bottom,    wVec = -Vector3.right,  hVec = Vector3.up },
        new Corner(){ type = CornerType.RightTop,      horizontalType = HorizontalType.Right,  verticalType = VerticalType.Top,       wVec = -Vector3.right,  hVec = -Vector3.up },
        new Corner(){ type = CornerType.LeftTop,       horizontalType = HorizontalType.Left,   verticalType = VerticalType.Top,       wVec = Vector3.right,   hVec = -Vector3.up }
    };


    public static void Clear()
    {
    }



}

}
