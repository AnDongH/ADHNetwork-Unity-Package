using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class StreamingAssetsSetup {
    static StreamingAssetsSetup() {
        // 패키지 내부에 있는 JSON 파일 경로 (커스텀 패키지의 파일 위치)
        string sourcePath = Path.Combine(Application.dataPath, "Packages/ADHNetwork-Unity-Package/ClientConfig.json");

        // 프로젝트의 StreamingAssets 폴더 경로
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");

        // StreamingAssets 폴더가 없으면 생성
        if (!Directory.Exists(streamingAssetsPath)) {
            Directory.CreateDirectory(streamingAssetsPath);
            Debug.Log("StreamingAssets 폴더 생성됨");
        }

        // 복사할 파일의 대상 경로
        string destinationPath = Path.Combine(streamingAssetsPath, "ClientConfig.json");

        // 파일이 없으면 복사
        if (!File.Exists(destinationPath)) {
            File.Copy(sourcePath, destinationPath);
            Debug.Log("config.json 파일이 StreamingAssets에 복사되었습니다.");
        }
    }
}

