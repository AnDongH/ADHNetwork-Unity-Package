using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class StreamingAssetsSetup {
    static StreamingAssetsSetup() {
        // ��Ű�� ���ο� �ִ� JSON ���� ��� (Ŀ���� ��Ű���� ���� ��ġ)
        string sourcePath = Path.Combine(Application.dataPath, "Packages/ADHNetwork-Unity-Package/ClientConfig.json");

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
        }
    }
}

