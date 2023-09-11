using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Cupboard;



public class FBXCreatorWindow : EditorWindow
{
    WindowParams windowParams = new WindowParams();


    [MenuItem("_柜子_/FBX 文件生成器")]
    static void CreateWindow()
    {
        var window = EditorWindow.GetWindow<FBXCreatorWindow>("FBX生成器");       
        window.maxSize = new Vector2(200f, 200f);
        window.minSize = window.maxSize; 
    }


    void OnGUI()
    {
        GUILayout.FlexibleSpace();

        windowParams.outFrameColor = EditorGUILayout.ColorField("外框 颜色", windowParams.outFrameColor);
        windowParams.partitionColor = EditorGUILayout.ColorField("隔板 颜色", windowParams.partitionColor);
        

        if(GUILayout.Button("生成 FBX 文件", GUILayout.Width(120), GUILayout.Height(50) ))
        {
            // 生成柜子:
            // -1- 在场景中新建一个 go
            // -2- 为这个 go 绑定一个 手动生成的 mesh
            // -3- 将这个 go 导出为一个 fbx 文件
            // -4- 删除这个 go (可选)
            Cupboard.CupboardUtils.Do(windowParams);
        }

        GUILayout.FlexibleSpace();
    }

}