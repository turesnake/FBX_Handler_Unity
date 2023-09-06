using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Formats.Fbx.Exporter;



namespace Cupboard{

public static class CupboardUtils
{
    


    // 测试用 物体表:
    static List<Candidate> fadeCandidates = new List<Candidate>(){

        new Candidate( new Vector3(4f,   4.5f, 1f), null ),
        new Candidate( new Vector3(2.4f, 2f, 1f), null ),
        new Candidate( new Vector3(5.1f, 4.2f, 1f), null ),
        new Candidate( new Vector3(2.2f, 1.1f, 1f), null ),
        new Candidate( new Vector3(5.3f, 4.8f, 1f), null ),
        new Candidate( new Vector3(4.4f, 2.2f, 1f), null ),
        new Candidate( new Vector3(1.3f, 3.9f, 1f), null ),
        new Candidate( new Vector3(1.9f, 2.3f, 1f), null ),
        new Candidate( new Vector3(3.21f, 3.15f, 1f), null ),
        new Candidate( new Vector3(2.55f, 4.77f, 1f), null ),
        new Candidate( new Vector3(3.11f, 1.31f, 1f), null ),
        new Candidate( new Vector3(3.05f, 3.66f, 1f), null ),
        new Candidate( new Vector3(2.31f, 2.5f, 1f), null ),
        new Candidate( new Vector3(4.05f, 2.89f, 1f), null ),
        new Candidate( new Vector3(3.41f, 2.65f, 1f), null ),
        new Candidate( new Vector3(1.24f, 1.76f, 1f), null ),
        new Candidate( new Vector3(5.41f, 2.7f, 1f), null ),
        new Candidate( new Vector3(1.31f, 1.23f, 1f), null ),
        new Candidate( new Vector3(1.95f, 3.88f, 1f), null ),
        new Candidate( new Vector3(2.3f, 1.3f, 1f), null ),
        new Candidate( new Vector3(1.22f, 5.33f, 1f), null ),
        new Candidate( new Vector3(2.31f, 2.5f, 1f), null ),
        new Candidate( new Vector3(2.2f, 1.1f, 1f), null ),
        new Candidate( new Vector3(2.2f, 1.5f, 1f), null ),
        new Candidate( new Vector3(1.2f, 2.1f, 1f), null ),
        new Candidate( new Vector3(1.4f, 1.3f, 1f), null ),
        new Candidate( new Vector3(1.05f, 1.5f, 1f), null ),
    };


    // 未来升级为一个 从小到大的容器;
    // todo: 最简模式: 用过的节点不删, 只是不用它
    static List<Cell> emptyCells = new List<Cell>();



    public static void Do() 
    {
        CupboardStates.Clear();

        SetMinGap( ref fadeCandidates);

        Cell rootCell = new Cell( 
            new RectInfo(
                Vector3.zero + Vector3.right * CupboardStates.partitionRadius + Vector3.up * CupboardStates.partitionRadius,
                CupboardStates.cupboardWidth - CupboardStates.partitionRadius,
                CupboardStates.cupboardHeight - CupboardStates.partitionRadius
            ),
            new Partition[4]{null,null,null,null}
        );
        emptyCells.Clear();
        emptyCells.Add( rootCell );
        //---

        foreach( var can in fadeCandidates ) 
        {
            DoSplit( can );
        }

        // --- debug:
        //DebugCell( rootCell, "Root" );
        var rootGO = new GameObject("root_Cupboard");
        DebugCell2( rootCell, rootGO );

        CreatePartitionGameObj();

    }


    static void DoSplit( Candidate candidate_ ) 
    {

        // 收集所有 合规的 cells:
        List<Cell> legalCells = new List<Cell>();
        foreach( var c in emptyCells ) 
        {
            if( c.IsEmptyLeaf() && IsCellBigEnough(c,candidate_ ) ) 
            {
                legalCells.Add( c );
            }
        }
        if( legalCells.Count == 0 ) 
        {
            Debug.LogError( "没能找到 足够大的 cell" );
            return;
        }

        // 拿到一个合规的 cell:
        Cell tgtCell = legalCells[ Random.Range( 0, legalCells.Count ) ];
        DoSplit_Simple( tgtCell, candidate_ );
    }




    static void DoSplit_Simple( Cell cell_, Candidate candidate_  ) 
    {
        // ======= 上下左右, 选择将 candidate 放置在 cell 的哪个角落 =========
        Corner corner = GetRendomCorner();

        bool isHorizonalCutFirst = Random.Range(0f,1f) > 0.5f;
        
        Cell outCell = null;
        bool isFstCutSuccess = isHorizonalCutFirst ? 
            DoHorizonalCut( cell_, candidate_, corner, out outCell ) : 
            DoVerticalCut(  cell_, candidate_, corner, out outCell );
        Debug.Assert( outCell != null );
            

        Cell outCell2 = null;
        bool isSecCutSuccess = isHorizonalCutFirst ? 
            DoVerticalCut(  outCell, candidate_, corner, out outCell2 ) : 
            DoHorizonalCut( outCell, candidate_, corner, out outCell2 );
        Debug.Assert( outCell2 != null );

        //outCell2.candidate = candidate_;
        outCell2.SetCandidate( candidate_ );
    }


    // 横着切一刀
    // ret: 是否切成功
    static bool DoHorizonalCut( Cell cell_, Candidate candidate_, Corner corner_, out Cell outCell_ ) 
    {
        float hGap   = cell_.H - candidate_.aabb.y;
        bool canHorizonalCut = ( hGap > CupboardStates.partitionRadius * 2f ) && ( hGap > CupboardStates.minGap );
        if( canHorizonalCut == false ) 
        {
            outCell_ = cell_;
            return false;
        }

        float t = (candidate_.aabb.y + CupboardStates.partitionRadius) / cell_.H;
        if( corner_.verticalType == VerticalType.Top ) 
        {
            t = 1f - t;
        }
        Debug.Assert( t > 0f && t < 1f );

        cell_.Split( PartitionDirection.Horizontal, t );
        emptyCells.Add( cell_.cell_LB );
        emptyCells.Add( cell_.cell_RT );

        outCell_ = ( corner_.verticalType == VerticalType.Bottom )  ? cell_.cell_LB : cell_.cell_RT;
        return true;
    }


    // 竖着切一刀
    // ret: 是否切成功
    static bool DoVerticalCut( Cell cell_, Candidate candidate_, Corner corner_, out Cell outCell_ ) 
    {
        float wGap = cell_.W - candidate_.aabb.x;
        bool canVerticalCut  = ( wGap > CupboardStates.partitionRadius * 2f ) && ( wGap > CupboardStates.minGap );
        if( canVerticalCut == false ) 
        {
            outCell_ = cell_;
            return false;
        }

        float t = (candidate_.aabb.x + CupboardStates.partitionRadius) / cell_.W;
        if( corner_.horizontalType == HorizontalType.Right ) 
        {
            t = 1f - t;
        }
        Debug.Assert( t > 0f && t < 1f );

        cell_.Split( PartitionDirection.Vertical, t );
        emptyCells.Add( cell_.cell_LB );
        emptyCells.Add( cell_.cell_RT );

        outCell_ = ( corner_.horizontalType == HorizontalType.Left ) ? cell_.cell_LB : cell_.cell_RT;
        return true;
    }



    static bool IsCellBigEnough( Cell cell_, Candidate candidate_ ) 
    {
        return ( cell_.W >= candidate_.aabb.x && cell_.H >= candidate_.aabb.y );
    }



    static void DebugCell( Cell cell_, string info_ ) 
    {
        if( cell_ == null ) 
        {
            return;
        }
        Debug.Log( info_ + ": " + cell_.ToString() );

        DebugCell( cell_.cell_LB, info_ + "_LB" );
        DebugCell( cell_.cell_RT, info_ + "_RT" );
    }


    static void DebugCell2( Cell cell_, GameObject parent_ ) 
    {
        if( cell_ == null ) 
        {
            return;
        }

        GameObject selfGO = null;
        if( cell_.IsLeaf() ) 
        {
            Color color = cell_.IsEmpty() ? 
                Random.ColorHSV(0.7f, 0.75f, 0.5f,0.55f, 0.9f, 0.95f) :   // 冷色
                Random.ColorHSV(0f, 0.1f, 0.8f,1f, 0.95f, 1f);           // 浅色
            selfGO = FBXCreator_2.CreateQuadGameObj( parent_, cell_.rectInfo, color, "cell" );
        }
        else 
        {
            Color color = Random.ColorHSV(0.3f, 0.4f, 0.05f,0.1f, 0.1f, 0.2f); // 隔板颜色
            var partition = cell_.partition;
            //selfGO = FBXCreator_2.CreateQuadGameObj( parent_, partition.rectInfo, color, "partition" );

            selfGO = new GameObject("partition");
            selfGO.transform.SetParent( parent_.transform );

            // todo: 先不画
            foreach( var rInfo in partition.GetInfiltratingRectInfos() ) 
            {
                FBXCreator_2.CreateQuadGameObj( selfGO, rInfo, color, "partition_segment" );
            }

            partition.DrawAllVertices(selfGO.transform);

            // ---
            partition.BuildMesh();
        }

        DebugCell2( cell_.cell_LB, selfGO );
        DebugCell2( cell_.cell_RT, selfGO );
    }



    // 使用此特殊分配器, 来保证每次分配的 值 都和上一次不一样
    static int lastCornerIdx = 0;
    static Corner GetRendomCorner() 
    {
        int val = Random.Range( 0, 4 );
        lastCornerIdx += Random.Range( 1, 4 ); // 向后步进 {1,2,3} 
        lastCornerIdx = lastCornerIdx % CupboardStates.corners.Length;
        //Debug.Log("CornerIdx = " + lastCornerIdx );
        return CupboardStates.corners[lastCornerIdx];
    }


    static void SetMinGap( ref List<Candidate> candidates_ ) 
    {
        float minGap = 999f;
        foreach( var candidate in candidates_ )
        {
            minGap = Mathf.Min( minGap, candidate.aabb.x );
            minGap = Mathf.Min( minGap, candidate.aabb.y );
        }
        CupboardStates.minGap = minGap;
    }


    // 全局唯一的 partition mesh go
    static void CreatePartitionGameObj() 
    {

		string name = "Partition_000";
        var newgo = new GameObject(name);
		//newgo.transform.SetParent( parent_.transform );

        // --- 随机颜色 --
        MeshRenderer meshRenderer = newgo.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color color = Random.ColorHSV(0.3f, 0.4f, 0.05f,0.1f, 0.1f, 0.2f); // 隔板颜色
        meshRenderer.sharedMaterial.SetColor("_BaseColor", color );

        // --- mesh:
        MeshFilter meshFilter = newgo.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
		mesh.name = "partition_Grid";

		//Vector3[] vertices = rectInfo_.GetCornerVertices();

		// Vector2[] uv = new Vector2[4]{ 
		// 	new Vector2( 0f, 0f),
		// 	new Vector2( 1f, 0f),
		// 	new Vector2( 1f, 1f),
		// 	new Vector2( 0f, 1f)
		// };

		// Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		// Vector4[] tangents = new Vector4[4]{ tangent, tangent, tangent, tangent };
		
		mesh.vertices = CupboardStates.vertices.ToArray();
		//mesh.uv = uv;
		//mesh.tangents = tangents;

		
		mesh.triangles = CupboardStates.triangles.ToArray();
		mesh.RecalculateNormals();
		//---
		meshFilter.mesh = mesh;

        // --- save to fbx:
        string filePath = System.IO.Path.Combine(Application.dataPath, name + ".fbx");
        //ModelExporter.ExportObject(filePath, Selection.objects[0]);
        ModelExporter.ExportObject(filePath, newgo );


    }

}

}
