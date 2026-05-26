using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ScoreCalculator
{
    // ----------------------------------------------------
    // [핵심 함수] 완성된 8장 패를 정밀 분석하여 최종 배수와 족보 이름을 반환
    // ----------------------------------------------------
    public static float CalculateFinalMultiplier(List<TileData> hand, out List<string> achievedYaku, string characterName = "")
    {
        achievedYaku = new List<string>();
        
        float finalMultiplier = 1.0f;
        bool hasReplacementYaku = false; // 시그니처 배수 대체 여부 플래그

        if (hand == null || hand.Count != 8) return finalMultiplier;

        // 몸통 분해 사전 실행
        List<List<TileData>> discoveredBodies = ExtractBodies(hand);

        // 🔥 [추가] 시그니처 족보 판정 인터셉트
        if (!string.IsNullOrEmpty(characterName))
        {
            string sigYakuName;
            float sigMultiplier;
            bool isReplacement;

            if (SignatureChecker.CheckSignatureYaku(characterName, hand, discoveredBodies, out sigYakuName, out sigMultiplier, out isReplacement))
            {
                achievedYaku.Add(sigYakuName);

                if (isReplacement)
                {
                    finalMultiplier = sigMultiplier; // 3.5배, 4.0배, 5.0배 등으로 원본 1.0배 완전 대체
                    hasReplacementYaku = true;
                }
                else
                {
                    finalMultiplier *= sigMultiplier; // 카르키용 추가 곱연산 (x3.5)
                }
            }
        }

        // 시그니처 대체 족보가 켜지지 않은 일반 평성 상태일 때만 기본 족보 연산 적용
        if (!hasReplacementYaku)
        {
            achievedYaku.Add("평성(기본 완성) [1.0배]");
            
            // 태극(올 컬러) 검증
            bool isTaegeuk = hand.GroupBy(t => t.element).Count() == 1;
            if (isTaegeuk)
            {
                achievedYaku.Add("태극(올 컬러) [x3.0]");
                finalMultiplier *= 3.0f;
            }

            if (discoveredBodies.Count == 2)
            {
                bool body1IsStraight = IsStraightBody(discoveredBodies[0]);
                bool body2IsStraight = IsStraightBody(discoveredBodies[1]);
                bool body1IsTriple = IsTripleBody(discoveredBodies[0]);
                bool body2IsTriple = IsTripleBody(discoveredBodies[1]);

                // 순진 검증
                if (body1IsStraight && body2IsStraight)
                {
                    achievedYaku.Add("순진(모든 몸통이 순행) [x1.5]");
                    finalMultiplier *= 1.5f;
                }

                // 중중삼재 검증
                if (body1IsTriple && body2IsTriple)
                {
                    achievedYaku.Add("중중삼재(모든 몸통이 삼재) [x1.5]");
                    finalMultiplier *= 1.5f;
                }
            }
        }

        return finalMultiplier;
    }

    // ----------------------------------------------------
    // [헬퍼] 8장 패에서 몸통 2개를 안전하게 분리해내는 역추적 함수
    // ----------------------------------------------------
    private static List<List<TileData>> ExtractBodies(List<TileData> hand)
    {
        List<List<TileData>> bodies = new List<List<TileData>>();
        List<TileData> sortedHand = hand.OrderBy(t => t.element).ThenBy(t => t.value).ToList();

        // 1단계 WinningChecker가 했던 방식 그대로 머리를 먼저 찾고 남은 패를 추적
        for (int i = 0; i < sortedHand.Count - 1; i++)
        {
            if (sortedHand[i].element == sortedHand[i + 1].element && sortedHand[i].value == sortedHand[i + 1].value)
            {
                List<TileData> remaining = new List<TileData>(sortedHand);
                remaining.Remove(sortedHand[i]);
                remaining.Remove(sortedHand[i + 1]);

                if (FindBodies(remaining, out bodies))
                {
                    break; 
                }
            }
        }
        return bodies;
    }

    private static bool FindBodies(List<TileData> tiles, out List<List<TileData>> foundBodies)
    {
        foundBodies = new List<List<TileData>>();
        if (tiles.Count == 0) return true;

        TileData first = tiles[0];

        // 동일 삼재 체크
        var matchC = tiles.Where(t => t.element == first.element && t.value == first.value).Take(3).ToList();
        if (matchC.Count == 3)
        {
            List<TileData> next = new List<TileData>(tiles);
            matchC.ForEach(t => next.Remove(t));
            if (FindBodies(next, out foundBodies))
            {
                foundBodies.Add(matchC);
                return true;
            }
        }

        // 순행 체크
        var next1 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 1);
        var next2 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 2);
        if (next1 != null && next2 != null)
        {
            List<TileData> next = new List<TileData>(tiles);
            next.Remove(first); next.Remove(next1); next.Remove(next2);
            if (FindBodies(next, out foundBodies))
            {
                foundBodies.Add(new List<TileData> { first, next1, next2 });
                return true;
            }
        }

        // 종류 확장 삼재 체크
        var sameValueGroup = tiles.Where(t => t.value == first.value).ToList();
        if (sameValueGroup.Count >= 3)
        {
            var uniqueCombo = sameValueGroup.GroupBy(t => t.element).Select(g => g.First()).Take(3).ToList();
            if (uniqueCombo.Count == 3)
            {
                List<TileData> next = new List<TileData>(tiles);
                uniqueCombo.ForEach(t => next.Remove(t));
                if (FindBodies(next, out foundBodies))
                {
                    foundBodies.Add(uniqueCombo);
                    return true;
                }
            }
        }

        return false;
    }

    // ----------------------------------------------------
    // [판별기] 단일 몸통(3장)의 속성 분석
    // ----------------------------------------------------
    private static bool IsStraightBody(List<TileData> body)
    {
        if (body.Count != 3) return false;
        var sorted = body.OrderBy(t => t.value).ToList();
        // 같은 속성이면서 숫자가 1씩 연속되는가
        return sorted[0].element == sorted[1].element && sorted[1].element == sorted[2].element &&
               sorted[0].value + 1 == sorted[1].value && sorted[1].value + 1 == sorted[2].value;
    }

    private static bool IsTripleBody(List<TileData> body)
    {
        if (body.Count != 3) return false;
        // 완전히 같은 카드 3장이거나, 숫자는 같은데 속성이 전부 다른 확장 삼재인가
        bool allSame = body[0].value == body[1].value && body[1].value == body[2].value;
        return allSame;
    }
}