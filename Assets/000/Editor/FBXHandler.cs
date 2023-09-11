using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;



public static class FBXHandler
{

    [MenuItem("Tools/合并一个FBX文件内的所有_Submehes")]
    public static void CombineAllSubmeshes()
    {
        // --- 确保用户选择了正确对象:
        if(     Selection.activeObject == null 
            ||  (Selection.activeObject is Mesh) == false
        ){
            UnityEditor.EditorUtility.DisplayDialog( "Error", "请选择一个 FBX 文件下层的 mesh 文件", "OK" );
            return;
        }

        Mesh mesh = (Mesh)Selection.activeObject;
        mesh.SetTriangles(mesh.triangles, 0); // 把所有三角形放入 submesh 0
        mesh.subMeshCount = 1; // 删除多余 submesh 
    }



    [MenuItem("Tools/复制一个 FBX 文件")]
    public static void CopyAFBX()
    {
        string selectedPath = "";

        // ---- 确保选中了一个 .fbx 文件 ----
        bool isSelectedError = true;
        if( Selection.objects.Length > 0 ) 
        {
            selectedPath = AssetDatabase.GetAssetPath( Selection.objects[0].GetInstanceID() ); // 文件path 或 目录path, 以 "Assets/" 开头
            Debug.Log( "选中 - " + selectedPath );
            bool isDirectoryExists = System.IO.Directory.Exists( selectedPath );
            bool isFileExists = System.IO.File.Exists( selectedPath );
            if( isFileExists && selectedPath.EndsWith(".fbx") )
            {
                isSelectedError = false;
            }
        }
        if(isSelectedError)
        {
            UnityEditor.EditorUtility.DisplayDialog( "Error", "请选择一个 FBX 文件", "OK" );
            return;
        }
        // ---------
        FileInfo fInfo = new FileInfo(selectedPath); // .FullName: E:\...\FBXs\A1.fbx
        string fileFullPath = fInfo.FullName; // E:\...\FBXs\A1.fbx

        Debug.Log( "fInfo = " + fInfo.FullName );
        

        // var k = Path.GetFileNameWithoutExtension(fileFullPath);
        // Debug.Log( "b --:" + k );

        var newPath= Path.Combine( fInfo.Directory.FullName, "B1.fbx");
        Debug.Log( "newPath --:" + newPath );


        //System.IO.File.Copy( fileFullPath, newPath, false );

    }



    [MenuItem("Tools/将一个GameObj导出为一个FBX文件")]
    public static void ExportGameObjects()
    {
        string filePath = Path.Combine(Application.dataPath, "MyGame_koko_2.fbx");

        //ModelExporter.ExportObject(filePath, Selection.objects[0]);
        ModelExporter.ExportObjects(filePath, Selection.objects );
    }



}
