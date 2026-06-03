using Firebase.Functions;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GachaManager : MonoBehaviour
{
    private FirebaseFunctions functions;

    void Start()
    {
        // Firebase Functions 초기화 (지역 설정 맞추기)
        functions = FirebaseFunctions.DefaultInstance;
    }

    // 인게임 가챠 버튼의 OnClick 이벤트에 이 함수를 연결해!
    public async void RequestGacha()
    {
        Debug.Log("서버에 가챠 요청 중...");

        try
        {
            // 아까 Node.js에서 만든 함수 이름 "doGacha"를 정확히 호출
            var function = functions.GetHttpsCallable("doGacha");
            var result = await function.CallAsync();

            // 서버에서 리턴해준 데이터 파싱하기
            var resultData = (Dictionary<object, object>)result.Data;
            bool isSuccess = (bool)resultData["success"];

            if (isSuccess)
            {
                var data = (Dictionary<object, object>)resultData["data"];
                string pulledChar = (string)data["charId"];
                bool isDup = (bool)data["isDuplicate"];
                int stardust = Convert.ToInt32(data["stardustEarned"]);

                // 네가 원했던 개발 로직! 서버가 주는 대로 화면에 띄워주기만 하면 됨.
                if (isDup)
                {
                    Debug.Log($"아앗! 중복이야! [{pulledChar}] 대신 별먼지 {stardust}개를 획득했어!");
                    // TODO: 유니티 UI - 별먼지 아이콘이랑 갯수 화면에 띄우기
                }
                else
                {
                    Debug.Log($"야호! 신규 캐릭터 [{pulledChar}] 획득!");
                    // TODO: 유니티 UI - 화려한 신규 캐릭터 연출 화면 띄우기
                }
            }
        }
        catch (FunctionsException e)
        {
            // 재화가 부족하거나(관측석 부족) 에러가 났을 때 캐치!
            Debug.LogError($"가챠 실패: {e.Message}");
            // TODO: 유니티 UI - "관측석이 부족합니다" 알림 팝업 띄우기
        }
    }
}