using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CreateConfigFileTool
{

    [MenuItem("Tool/Create Config")]
    static void Make() {

        //string sourcePath = "Packages/com.haegindev.adhnetwork/ClientConfig.json";

        // ����׿� ���////////////////
        string sourcePath = "Assets/ADHNetwork-Unity-Package/ClientConfig.json";
        ////////////////////////////////

        string streamingAssetsPath = Application.streamingAssetsPath;

        if (!Directory.Exists(streamingAssetsPath)) {
            Directory.CreateDirectory(streamingAssetsPath);
            Debug.Log("StreamingAssets ���� ������");
        }

        string destinationPath = Path.Combine(streamingAssetsPath, "ClientConfig.json");

        if (!File.Exists(destinationPath)) {
            File.Copy(sourcePath, destinationPath);
            Debug.Log("config.json ������ StreamingAssets�� ����Ǿ����ϴ�.");
            AssetDatabase.Refresh();
        }

    }

}
