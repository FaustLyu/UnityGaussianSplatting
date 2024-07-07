using UnityEngine;
using UnityEditor;
using System.IO;
using GaussianSplatting.Runtime;
using UnityEditor.VersionControl;

public class GaussianCreater : EditorWindow
{
    private string inputFolder;


    [MenuItem("Tools/GaussianCreater")]
    public static void ShowWindow()
    {
        GetWindow<GaussianCreater>("GaussianCreater");
    }

    private void OnGUI()
    {
        GUILayout.Label("GaussianCreater Empty GameObjects", EditorStyles.boldLabel);

        if (GUILayout.Button("GenerateGaussian"))
        {
            inputFolder = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            GenerateGameObjects();
        }
    }

    private void GenerateGameObjects()
    {
        if (string.IsNullOrEmpty(inputFolder) || !Directory.Exists(inputFolder))
        {
            UnityEngine.Debug.LogError("Input folder is invalid or does not exist.");
            return;
        }


        // 获取文件夹下的所有文件路径
        string[] filePaths = Directory.GetFiles(inputFolder, "*.asset", SearchOption.AllDirectories);

        foreach (string filePath in filePaths)
        {
            // 将完整路径转换为相对路径
            string relativePath = Path.GetRelativePath(Application.dataPath, filePath);
            CreateEmptyGameObject(relativePath);
            // Debug.Log("Relative path: " + relativePath);
        }

        //if (allFiles.Length == 0)
        //{
        //    UnityEngine.Debug.LogError("No header files found in the specified folder.");
        //    return;
        //}

        //for (int i = 0; i < allFiles.Length; i++)
        //{
        //    CreateEmptyGameObject(i, Path.GetFileName(allFiles[i]));
        //}
    }

    private void CreateEmptyGameObject(string fileName)
    {
        string objectName = $"{Path.GetFileName(fileName)}";

        // 构建完整的资源路径
        string assetPath = Path.Combine("Assets", fileName);

        // 加载 asset 文件
        GaussianSplatAsset loadedAsset = AssetDatabase.LoadAssetAtPath<GaussianSplatAsset>(assetPath);

        if (loadedAsset == null)
        {
            UnityEngine.Debug.LogError("Failed to load asset: " + assetPath);
            return;
        }

        // 创建一个新的空 GameObject，并将 asset 绑定到 GameObject 上
        GameObject newObject = new GameObject(objectName);
        // 将新创建的对象选中
        Selection.activeObject = newObject;

        GaussianSplatRenderer renderer = newObject.AddComponent<GaussianSplatRenderer>();
        renderer.m_Asset = loadedAsset;
        renderer.OnEnable();
        // Transform transform = newObject.transform;
        SphereCollider collider = newObject.AddComponent<SphereCollider>();
        collider.radius = (loadedAsset.boundsMax - loadedAsset.boundsMin).magnitude / 2 * 0.65f;
        collider.center = loadedAsset.posCenter;
    }

}

