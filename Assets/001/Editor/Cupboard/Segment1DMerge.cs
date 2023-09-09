using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;


namespace Cupboard{






public class Segment1DMerge
{
    


    public class Segment1D
    {
        public float head;
        public float end;
        public bool isActive; // false 表示此 segment 已经被合并了, 变成了无用的
        public Segment1D( float head_, float end_ ) 
        {
            Debug.Assert( head_ < end_ );
            head = head_;
            end = end_;
            isActive = true;
        }
        public override string ToString() 
        {
            return string.Format( "seg -- {0} --> {1};  isActive:{2}", head, end, isActive );
        } 
    }


    public List<Segment1D> segments = new List<Segment1D>();


    public Segment1DMerge() 
    {
    }


    // 不关心性能版:
    public void Add(Segment1D newSegment_)
    {
        if( segments.Count == 0 )
        {
            segments.Add(newSegment_);
            return;
        }
        // ---1--- 插入
        segments.Add(newSegment_);
        segments.Sort( (x,y)=>{ return (x.head < y.head) ? -1 : 1;  } );
        // ---2--- 重排:
        for( int l=0; l<segments.Count-1; l++ ) 
        {
            if( segments[l].isActive == false )
            {
                continue;
            }
            for( int r=l+1; r<segments.Count; r++ ) 
            {
                if( segments[r].isActive == false )
                {
                    continue;
                }
                if( segments[r].head <= segments[l].end ) 
                {
                    // --- 合并 ---:
                    segments[l].end = Mathf.Max( segments[l].end, segments[r].end );
                    segments[r].isActive = false;
                }
                else 
                {
                    break;
                }
            }
        }
        // ---3--- 删除废弃 segment:
        segments.RemoveAll( (x) => x.isActive == false );
    }






}



}
