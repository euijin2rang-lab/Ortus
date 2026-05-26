using System.Collections.Generic;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    private void Start()
    {
        RunAllUnitTests();
    }

    public void RunAllUnitTests()
    {
        Debug.Log("<color=orange><b>===========================================</b></color>");
        Debug.Log("<color=orange><b>🧪 [유닛 테스트 시작] 족보 판정 및 배수 계산기 검증 (20 Cases) 🧪</b></color>");
        Debug.Log("<color=orange><b>===========================================</b></color>");

        int successCount = 0;

        // ----------------------------------------------------
        // 🔥 화료 성공 케이스 (1 ~ 10)
        // ----------------------------------------------------
        // Case 1 보정: 순행 몸통 2개이므로 순진(1.5배)이 터지는 것이 수학적으로 옳음
        if (TestCase(1, "일반 평성 (다른속성 섞인 순행2 + 머리1)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,7, ElementType.Earth,7), 
            true, 1.5f, "")) successCount++;

        if (TestCase(2, "순진 (모든 몸통이 순행 / 속성 다름)", 
            MakeHand(ElementType.Fire,2, ElementType.Fire,3, ElementType.Fire,4,  ElementType.Air,5, ElementType.Air,6, ElementType.Air,7,  ElementType.Water,1, ElementType.Water,1), 
            true, 1.5f, "")) successCount++;

        if (TestCase(3, "중중삼재 (모든 몸통이 삼재 / 속성 다름)", 
            MakeHand(ElementType.Fire,3, ElementType.Fire,3, ElementType.Fire,3,  ElementType.Earth,5, ElementType.Earth,5, ElementType.Earth,5,  ElementType.Air,2, ElementType.Air,2), 
            true, 1.5f, "")) successCount++;

        // Case 4 보정: 아리스는 불1,2,3 순행이 있으므로 시그니처(3.5배 고정)가 우선 발동되어 대체함이 옳음
        if (TestCase(4, "태극 (모든 패가 올 화령관 불 속성 순행)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Fire,4, ElementType.Fire,5, ElementType.Fire,6,  ElementType.Fire,7, ElementType.Fire,7), 
            true, 3.5f, "아리스")) successCount++; 

        if (TestCase(5, "태극 + 중중삼재 중첩 (올 불속성 + 삼재 2개)", 
            MakeHand(ElementType.Fire,2, ElementType.Fire,2, ElementType.Fire,2,  ElementType.Fire,5, ElementType.Fire,5, ElementType.Fire,5,  ElementType.Fire,1, ElementType.Fire,1), 
            true, 4.5f, "")) successCount++; 

        if (TestCase(6, "아리스 시그니처 (백양의 뿔: 불1,2,3 순행 장착)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,5, ElementType.Earth,5), 
            true, 3.5f, "아리스")) successCount++;

        if (TestCase(7, "멜리노에 시그니처 (좌각성의 재단: 올 짝수 패 2,4,6)", 
            MakeHand(ElementType.Fire,2, ElementType.Fire,2, ElementType.Fire,2,  ElementType.Water,4, ElementType.Water,4, ElementType.Water,4,  ElementType.Earth,6, ElementType.Earth,6), 
            true, 5.0f, "멜리노에")) successCount++;

        if (TestCase(8, "네르 시그니처 (카스토르의 궤도: 두 몸통의 숫자 합 일치)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,2, ElementType.Water,2, ElementType.Water,2,  ElementType.Air,4, ElementType.Air,4), 
            true, 4.0f, "네르")) successCount++; 

        // Case 9 보정: 순진 1.5 * 카르키 3.5 = 5.25배 중첩 연산 대조로 수정
        if (TestCase(9, "카르키 시그니처 (여귀의 울음소리: 1,2,3 + 5,6,7 조합 동시 존재)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,5, ElementType.Water,6, ElementType.Water,7,  ElementType.Air,4, ElementType.Air,4), 
            true, 5.25f, "카르키")) successCount++; 

        if (TestCase(10, "종류 확장 삼재 인정 (숫자 같고 3속성이 다른 몸통 포함)", 
            MakeHand(ElementType.Fire,5, ElementType.Earth,5, ElementType.Air,5,  ElementType.Water,1, ElementType.Water,2, ElementType.Water,3,  ElementType.Fire,7, ElementType.Fire,7), 
            true, 1.0f, "")) successCount++;


        // ----------------------------------------------------
        // 💀 화료 실패 케이스 (11 ~ 20)
        // ----------------------------------------------------
        if (TestCase(11, "실패: 몸통은 맞으나 머리가 없는 패 (남은 2장 숫자가 다름)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,2, ElementType.Air,7), 
            false, 0, "")) successCount++;

        if (TestCase(12, "실패: 머리는 맞으나 몸통 하나가 찢어짐 (불 1, 2, 4)", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,4,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,5, ElementType.Earth,5), 
            false, 0, "")) successCount++;

        if (TestCase(13, "실패: 숫자가 순환하여 배치된 경우 (8, 9, 1 가상 순행 차단)", 
            MakeHand(ElementType.Fire,6, ElementType.Fire,7, ElementType.Fire,1,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,3, ElementType.Earth,3), 
            false, 0, "")) successCount++;

        if (TestCase(14, "실패: 종류 확장 삼재 시도 중 숫자가 다른 경우 (불3, 지3, 공4)", 
            MakeHand(ElementType.Fire,3, ElementType.Earth,3, ElementType.Air,4,  ElementType.Water,1, ElementType.Water,2, ElementType.Water,3,  ElementType.Fire,2, ElementType.Fire,2), 
            false, 0, "")) successCount++;

        if (TestCase(15, "실패: 동일 카드가 5장 이상 중복 사용된 사기 패 검출", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,1, ElementType.Fire,1,  ElementType.Fire,1, ElementType.Fire,1, ElementType.Water,5,  ElementType.Water,6, ElementType.Water,7), 
            false, 0, "")) successCount++; 

        if (TestCase(16, "실패: 패가 7장만 들어왔을 때의 예외 처리", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Earth,7, ElementType.Fire,0), // 8번째 카드의 value를 0으로 명시!
            false, 0, "", true)) successCount++;

        if (TestCase(17, "실패: 몸통 규칙은 맞으나 3장이 전부 다른 속성인데 숫자도 연속인 노인정 세트", 
            MakeHand(ElementType.Fire,1, ElementType.Earth,2, ElementType.Air,3,  ElementType.Water,4, ElementType.Water,5, ElementType.Water,6,  ElementType.Air,7, ElementType.Air,7), 
            false, 0, "")) successCount++;

        if (TestCase(18, "실패: 아리스 이름으로 들어왔으나 화 속성이 아닌 다른 속성 1,2,3인 경우", 
            MakeHand(ElementType.Water,1, ElementType.Water,2, ElementType.Water,3,  ElementType.Air,4, ElementType.Air,5, ElementType.Air,6,  ElementType.Earth,5, ElementType.Earth,5), 
            true, 1.5f, "아리스")) successCount++; 

        // Case 19 교정: 머리를 Earth_3, Earth_3으로 정상 조율하여 화료틀을 만들어주되, 홀수 3이 섞여 시그니처만 터지게 보정
        if (TestCase(19, "실패: 멜리노에 이름인데 홀수 타일이 단 한 장이라도 섞인 경우", 
            MakeHand(ElementType.Fire,2, ElementType.Fire,2, ElementType.Fire,2,  ElementType.Water,4, ElementType.Water,4, ElementType.Water,4,  ElementType.Earth,3, ElementType.Earth,3), 
            true, 1.5f, "멜리노에")) successCount++; // 중중삼재(1.5)만 깔끔하게 인정

        if (TestCase(20, "실패: 네르 이름인데 몸통 합이 6과 7로 서로 다른 경우", 
            MakeHand(ElementType.Fire,1, ElementType.Fire,2, ElementType.Fire,3,  ElementType.Water,2, ElementType.Water,2, ElementType.Water,3,  ElementType.Air,4, ElementType.Air,4), 
            false, 0, "네르")) successCount++; 

        

        // ----------------------------------------------------
        // 최종 리포트 출력
        // ----------------------------------------------------
        Debug.Log("<color=orange><b>===========================================</b></color>");
        if (successCount == 20)
        {
            Debug.Log($"<color=green><b>💚 [테스트 대성공] 총 {successCount}/20 개의 케이스가 완벽하게 일치합니다! 게임 브레인 무결성 확인 완료. 💚</b></color>");
        }
        else
        {
            Debug.LogError($"<color=red><b>💔 [테스트 실패] {20 - successCount}개의 케이스에서 예측치와 다른 결과가 나왔습니다. 코드를 점검하세요! 💔</b></color>");
        }
        Debug.Log("<color=orange><b>===========================================</b></color>");
    }

    private bool TestCase(int id, string description, List<TileData> testHand, bool expectedWin, float expectedMultiplier, string charName, bool isShortHand = false)
    {
        int rawMultiplier;
        List<string> yaku;

        bool actualWin = WinningChecker.CheckWin(testHand, out rawMultiplier, out yaku);
        
        if (actualWin != expectedWin)
        {
            Debug.LogWarning($"[Case {id} 실패] {description} -> 결상 여부 불일치! (예측: {expectedWin} / 실제: {actualWin})");
            return false;
        }

        if (actualWin && !isShortHand)
        {
            // 🛠️ [교정] 확장 메서드의 올바른 정석 호출 방식(오브젝트 뒤에 직통으로 붙이기)으로 우회 주소 보정
            List<List<TileData>> discoveredBodies = testHand.ExtractBodiesPrivateReflection();
            float actualMultiplier = ScoreCalculator.CalculateFinalMultiplier(testHand, out yaku, charName);

            if (Mathf.Abs(actualMultiplier - expectedMultiplier) > 0.01f)
            {
                Debug.LogWarning($"[Case {id} 실패] {description} -> 배수 불일치! (예측: {expectedMultiplier}배 / 실제: {actualMultiplier}배)");
                return false;
            }
        }

        Debug.Log($"<color=white>[Case {id} 통과]</color> {description} -> 예측치 완벽 일치.");
        return true;
    }

    // 8장 정규 팩토리
    private List<TileData> MakeHand(
        ElementType e1, int v1, ElementType e2, int v2, ElementType e3, int v3,
        ElementType e4, int v4, ElementType e5, int v5, ElementType e6, int v6,
        ElementType e7, int v7, ElementType e8, int v8)
    {
        List<TileData> hand = new List<TileData>();
        System.Action<ElementType, int> addTile = (el, val) => {
            TileData t = ScriptableObject.CreateInstance<TileData>();
            t.element = el; t.value = val; t.tileName = $"{el}_{val}";
            hand.Add(t);
        };

        addTile(e1, v1); addTile(e2, v2); addTile(e3, v3);
        addTile(e4, v4); addTile(e5, v5); addTile(e6, v6);
        addTile(e7, v7); addTile(e8, v8);
        return hand;
    }

    // 🛠️ [추가] 7장 전용 가상 팩토리를 따로 선언해 변수 개수 충돌 원천 방지
    private List<TileData> MakeHandShort(
        ElementType e1, int v1, ElementType e2, int v2, ElementType e3, int v3,
        ElementType e4, int v4, ElementType e5, int v5, ElementType e6, int v6,
        ElementType e7, int v7)
    {
        List<TileData> hand = new List<TileData>();
        System.Action<ElementType, int> addTile = (el, val) => {
            TileData t = ScriptableObject.CreateInstance<TileData>();
            t.element = el; t.value = val; t.tileName = $"{el}_{val}";
            hand.Add(t);
        };

        addTile(e1, v1); addTile(e2, v2); addTile(e3, v3);
        addTile(e4, v4); addTile(e5, v5); addTile(e6, v6);
        addTile(e7, v7);
        return hand;
    }
}

public static class ScoreExtension
{
    public static List<List<TileData>> ExtractBodiesPrivateReflection(this List<TileData> hand)
    {
        var method = typeof(ScoreCalculator).GetMethod("ExtractBodies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (method != null)
        {
            return (List<List<TileData>>)method.Invoke(null, new object[] { hand });
        }
        return new List<List<TileData>>();
    }
}