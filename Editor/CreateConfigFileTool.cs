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

        string sourcePath = "Packages/com.haegindev.adhnetwork/ClientConfig.json";

        // 디버그용 경로////////////////
        //string sourcePath = "Assets/ADHNetwork-Unity-Package/ClientConfig.json";
        ////////////////////////////////

        string streamingAssetsPath = Application.streamingAssetsPath;

        if (!Directory.Exists(streamingAssetsPath)) {
            Directory.CreateDirectory(streamingAssetsPath);
            Debug.Log("StreamingAssets 폴더 생성됨");
        }

        string destinationPath = Path.Combine(streamingAssetsPath, "ClientConfig.json");

        if (!File.Exists(destinationPath)) {
            File.Copy(sourcePath, destinationPath);
            Debug.Log("config.json 파일이 StreamingAssets에 복사되었습니다.");
            AssetDatabase.Refresh();
        }

    }

}
