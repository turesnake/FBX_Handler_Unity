using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;


using UnityEditor.Formats.Fbx.Exporter;


public class HierarchyMonitorWindow : EditorWindow
{

    [MenuItem("Tools/_窗口_/FBX 文件生成器")]
    static void CreateWindow()
    {
        var window = EditorWindow.GetWindow<HierarchyMonitorWindow>("FBX生成器");       
        window.maxSize = new Vector2(200f, 200f);
        window.minSize = window.maxSize; 
    }


    void OnGUI()
    {
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("生成 FBX 文件", GUILayout.Width(120), GUILayout.Height(50) ))
        {
            Create();
        }
        GUILayout.FlexibleSpace();
    }


    // 正文: 
    // -1- 在场景中新建一个 go
    // -2- 为这个 go 绑定一个 手动生成的 mesh
    // -3- 将这个 go 导出为一个 fbx 文件
    // -4- 删除这个 go (可选)
    static void Create() 
    {
        string name = "fst_mesh_go_1";
        var newgo = new GameObject(name);
        // ---
        MeshRenderer meshRenderer = newgo.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        // ---
        MeshFilter meshFilter = newgo.AddComponent<MeshFilter>();
        meshFilter.mesh = FBXCreator.CreateMesh();


        string filePath = System.IO.Path.Combine(Application.dataPath, name + ".fbx");

        //ModelExporter.ExportObject(filePath, Selection.objects[0]);
        ModelExporter.ExportObject(filePath, newgo );
    }




}