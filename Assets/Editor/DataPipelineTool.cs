using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class DataPipelineTool : EditorWindow
{
    // ⚠️여기에 복사해 둔 구글 앱 스크립트 웹 앱 URL을 붙여넣으세요!
    private const string webAppUrl = "https://script.google.com/macros/s/AKfycbyXOeWXmzPaZsR8Sxl0kWvlmStS-k7rYWhy8dnPhcf9yItVoPCkRlb6nkvhgqtLb3lD/exec";
    
    // 저장할 폴더 및 파일 경로 설정
    private const string folderPath = "Assets/_Project/Data/JSON";
    private const string fileName = "BalanceData.json";

    // 유니티 상단 메뉴 바에 [Tools] -> [Fetch Balance Data] 메뉴를 생성
    [MenuItem("Tools/Fetch Balance Data")]
    public static void FetchDataFromGoogleSheets()
    {
        if (webAppUrl == "YOUR_GOOGLE_WEB_APP_URL_HERE" || string.IsNullOrEmpty(webAppUrl))
        {
            Debug.LogError("[파이프라인] 구글 앱 스크립트 URL이 비어있습니다! 스크립트 내부에서 URL을 먼저 등록해주세요.");
            return;
        }

        Debug.Log("<color=yellow>[파이프라인] 구글 시트로부터 최신 JSON 데이터를 요청하는 중...</color>");

        // UnityWebRequest를 이용해 구글 웹 앱 URL에 데이터 요청 보내기
        UnityWebRequest request = UnityWebRequest.Get(webAppUrl);
        
        // 요청이 끝날 때까지 동기적으로 대기 (에디터 전용 비동기 처리)
        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            // 에디터 툴이 멈추지 않고 데이터를 받을 때까지 대기 플래그
        }

        // 통신 성공 여부 확인
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonText = request.downloadHandler.text;
            SaveJsonToFile(jsonText);
        }
        else
        {
            Debug.LogError($"[파이프라인] 데이터 가져오기 실패: {request.error}\nURL 주소나 구글 앱 스크립트 배포 설정을 확인해주세요.");
        }
    }

    // 받아온 텍스트를 파일로 안전하게 저장하는 함수
    private static void SaveJsonToFile(string text)
    {
        // 폴더가 없다면 자동으로 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 최종 파일 저장 경로 조합 (Assets/_Project/Data/JSON/BalanceData.json)
        string fullPath = Path.Combine(folderPath, fileName);

        // 파일 쓰기
        File.WriteAllText(fullPath, text);

        // 유니티 엔진에게 "새로운 파일이 들어왔으니 프로젝트 창 새로고침해라!"고 신호 주기
        AssetDatabase.Refresh();

        Debug.Log($"<color=green><b>[파이프라인 성공]</b> 최신 밸런스 데이터가 안전하게 저장되었습니다!</color>\n경로: {fullPath}");
    }
}