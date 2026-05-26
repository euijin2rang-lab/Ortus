using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WinningChecker
{
    // ----------------------------------------------------
    // [메인 함수] 8장 패를 받아 결상 가능 여부와 최종 배수를 반환
    // ----------------------------------------------------
    public static bool CheckWin(List<TileData> hand, out int finalMultiplier, out List<string> achievedYaku)
    {
        finalMultiplier = 0;
        achievedYaku = new List<string>();

        if (hand == null || hand.Count != 8) return false;

        // 대소문자나 순서 꼬임을 방지하기 위해 속성->숫자 순으로 정렬된 복사본 생성
        List<TileData> sortedHand = hand.OrderBy(t => t.element).ThenBy(t => t.value).ToList();

        // 1. 머리(또이쯔) 후보 찾기: 원소와 숫자가 완전히 일치하는 2장 탐색
        for (int i = 0; i < sortedHand.Count - 1; i++)
        {
            if (sortedHand[i].element == sortedHand[i + 1].element && sortedHand[i].value == sortedHand[i + 1].value)
            {
                // 백트래킹 시작: 머리 후보 1쌍을 임시로 제외(Pop)
                List<TileData> remainingTiles = new List<TileData>(sortedHand);
                TileData head1 = sortedHand[i];
                TileData head2 = sortedHand[i + 1];
                
                remainingTiles.Remove(head1);
                remainingTiles.Remove(head2);

                // 2. 남은 6장으로 몸통 2개 검증 루프(재귀) 진입
               if (ValidateBodyRecursive(remainingTiles))
{
    // 정밀 족보 계산기에 현재 손패 주인의 캐릭터 이름을 함께 토스!
    // handList를 들고 있는 주체의 이름을 유추하기 위해, 안전하게 분기 조건 처리할 수 있게 구조 확장
    string casterName = "";
    
    // 유니티 씬 안의 플레이어와 AI 캐릭터 명칭을 안전하게 판별
    var turnManager = Object.FindObjectOfType<TurnManager>();
    if (turnManager != null)
    {
        // 8장 카드가 일치하는 주인의 캐릭터 데이터 이름을 가져옴
        if (turnManager.playerHand.handList.SequenceEqual(hand)) casterName = turnManager.playerHand.characterData.characterName;
        else if (turnManager.aiHand.handList.SequenceEqual(hand)) casterName = turnManager.aiHand.characterData.characterName;
    }

    float score = ScoreCalculator.CalculateFinalMultiplier(hand, out achievedYaku, casterName);
    finalMultiplier = Mathf.RoundToInt(score * 100f); 
    return true;
}
                
                // 실패했다면 루프를 돌며 다음 머리 후보를 시험해봄
                i++; // 똑같은 머리 쌍의 다음 카드는 건너뜀
            }
        }

        // 모든 머리 후보군으로 시도했으나 몸통 분해가 안 되면 결상 실패
        return false;
    }

    // ----------------------------------------------------
    // [몸통 검증] 백트래킹 알고리즘 (6장 -> 3장 -> 0장 구조)
    // ----------------------------------------------------
    private static bool ValidateBodyRecursive(List<TileData> tiles)
    {
        // 남은 카드가 0장이면 모든 몸통(2개)이 완벽하게 맞춰진 것!
        if (tiles.Count == 0) return true;

        // 항상 정렬된 리스트의 첫 번째 카드를 기준(pivot)으로 잡고 매칭 시작
        TileData first = tiles[0];

        // --- 조건 1: 삼재(커쯔) - 완전히 같은 원소와 숫자인 3장 ---
        var sameTiles = tiles.Where(t => t.element == first.element && t.value == first.value).Take(3).ToList();
        if (sameTiles.Count == 3)
        {
            List<TileData> nextHand = new List<TileData>(tiles);
            nextHand.Remove(sameTiles[0]);
            nextHand.Remove(sameTiles[1]);
            nextHand.Remove(sameTiles[2]);

            if (ValidateBodyRecursive(nextHand)) return true; // 성공하면 즉시 리턴
        }

        // --- 조건 2: 순행(슌쯔) - 같은 원소이면서 연속된 숫자 3장 (예: 1, 2, 3) ---
        TileData next1 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 1);
        TileData next2 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 2);
        if (next1 != null && next2 != null)
        {
            List<TileData> nextHand = new List<TileData>(tiles);
            nextHand.Remove(first);
            nextHand.Remove(next1);
            nextHand.Remove(next2);

            if (ValidateBodyRecursive(nextHand)) return true;
        }

        // --- 조건 3: 삼재(종류 확장) - 원소는 전부 다르고 숫자가 같은 3장 ---
        var sameValueGroup = tiles.Where(t => t.value == first.value).ToList();
        if (sameValueGroup.Count >= 3)
        {
            // 원소별로 1장씩만 남겨서 겹치지 않는 속성 조합 유도
            var uniqueElementCombo = sameValueGroup.GroupBy(t => t.element).Select(g => g.First()).ToList();
            
            // 만약 서로 다른 원소가 3개 이상 모였다면 (예: 화5, 지5, 공5)
            if (uniqueElementCombo.Count >= 3)
            {
                // 정확히 3장만 선택
                var targetCombo = uniqueElementCombo.Take(3).ToList();
                
                List<TileData> nextHand = new List<TileData>(tiles);
                nextHand.Remove(targetCombo[0]);
                nextHand.Remove(targetCombo[1]);
                nextHand.Remove(targetCombo[2]);

                if (ValidateBodyRecursive(nextHand)) return true;
            }
        }

        // 위의 3가지 몸통 조건 중 아무것도 만족하지 못하면 이 분기는 실패(False)
        return false;
    }

    
}