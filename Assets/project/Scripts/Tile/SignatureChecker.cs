using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SignatureChecker
{
    // ----------------------------------------------------
    // [핵심 엔진] 캐릭터의 이름을 기반으로 시그니처 족보를 판정하는 메서드
    // ----------------------------------------------------
    public static bool CheckSignatureYaku(string characterName, List<TileData> hand, List<List<TileData>> bodies, out string yakuName, out float specialMultiplier, out bool isReplacement)
    {
        yakuName = "";
        specialMultiplier = 1.0f;
        isReplacement = false; // 기본 점수를 대체하는지, 추가 곱연산인지 여부

        if (hand == null || hand.Count != 8) return false;

        switch (characterName)
        {
            case "아리스":
                // 1. 백양의 뿔: [화(Fire)] 속성의 1, 2, 3 순행 몸통이 존재하는가?
                foreach (var body in bodies)
                {
                    if (IsStraight(body) && body[0].element == ElementType.Fire)
                    {
                        var values = body.Select(t => t.value).OrderBy(v => v).ToList();
                        if (values[0] == 1 && values[1] == 2 && values[2] == 3)
                        {
                            yakuName = "시그니처: 백양의 뿔 [3.5배 고정]";
                            specialMultiplier = 3.5f;
                            isReplacement = true; // 기본 배수 대체
                            return true;
                        }
                    }
                }
                break;

            case "멜리노에":
                // 2. 좌각성의 재단: 패의 모든 숫자(8장)가 짝수(2, 4, 6)로만 이루어져 있는가?
                bool allEven = hand.All(t => t.value == 2 || t.value == 4 || t.value == 6);
                if (allEven)
                {
                    yakuName = "시그니처: 좌각성의 재단 [5.0배 고정]";
                    specialMultiplier = 5.0f;
                    isReplacement = true; // 기본 배수 대체
                    return true;
                }
                break;

            case "네르":
                // 3. 카스토르의 궤도: 분해된 두 몸통의 숫자 합이 완전히 일치하는가?
                if (bodies.Count == 2)
                {
                    int sumBody1 = bodies[0].Sum(t => t.value);
                    int sumBody2 = bodies[1].Sum(t => t.value);
                    if (sumBody1 == sumBody2)
                    {
                        yakuName = "시그니처: 카스토르의 궤도 [4.0배 고정]";
                        specialMultiplier = 4.0f;
                        isReplacement = true; // 기본 배수 대체
                        return true;
                    }
                }
                break;

            case "카르키":
                // 4. 여귀의 울음소리: 패 전체에 [1, 2, 3] 순서와 [5, 6, 7] 순서 세트가 동시에 존재하느냐?
                // (속성은 달라도 됨, 순행 몸통 분기 기준)
                bool hasLowStraight = false;
                bool hasHighStraight = false;

                foreach (var body in bodies)
                {
                    if (IsStraight(body))
                    {
                        var values = body.Select(t => t.value).OrderBy(v => v).ToList();
                        if (values[0] == 1 && values[1] == 2 && values[2] == 3) hasLowStraight = true;
                        if (values[0] == 5 && values[1] == 6 && values[2] == 7) hasHighStraight = true;
                    }
                }

                if (hasLowStraight && hasHighStraight)
                {
                    yakuName = "시그니처: 여귀의 울음소리 [추가 x3.5]";
                    specialMultiplier = 3.5f;
                    isReplacement = false; // 기본 배수에 추가 곱연산 가산
                    return true;
                }
                break;
        }

        return false;
    }

    // 순행 몸통인지 판별하는 간단한 헬퍼
    private static bool IsStraight(List<TileData> body)
    {
        if (body.Count != 3) return false;
        var sorted = body.OrderBy(t => t.value).ToList();
        return sorted[0].element == sorted[1].element && sorted[1].element == sorted[2].element &&
               sorted[0].value + 1 == sorted[1].value && sorted[1].value + 1 == sorted[2].value;
    }
}