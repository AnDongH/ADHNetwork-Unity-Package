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

        // ��Ű�� ���ο� �ִ� JSON ���� ��� (Ŀ���� ��Ű���� ���� ��ġ)
        string sourcePath = "Packages/com.haegindev.adhnetwork/ClientConfig.json";

        // ������Ʈ�� StreamingAssets ���� ���
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");

        // StreamingAssets ������ ������ ����
        if (!Directory.Exists(streamingAssetsPath)) {
            Directory.CreateDirectory(streamingAssetsPath);
            Debug.Log("StreamingAssets ���� ������");
        }

        // ������ ������ ��� ���
        string destinationPath = Path.Combine(streamingAssetsPath, "ClientConfig.json");

        // ������ ������ ����
        if (!File.Exists(destinationPath)) {
            File.Copy(sourcePath, destinationPath);
            Debug.Log("config.json ������ StreamingAssets�� ����Ǿ����ϴ�.");
            AssetDatabase.Refresh();
        }

    }

}
