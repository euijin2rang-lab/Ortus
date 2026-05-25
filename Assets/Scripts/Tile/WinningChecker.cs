using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinningChecker : MonoBehaviour
{
    // ----------------------------------------------------
    // [메인 함수] 8장 패를 받아 결상 가능 여부와 최종 배수를 반환
    // ----------------------------------------------------
    public static bool CheckWin(List<TileData> hand, out int finalMultiplier, out List<string> achievedYaku)
    {
        finalMultiplier = 0;
        achievedYaku = new List<string>();

        if (hand == null || hand.Count != 8) return false;

        // 1. 머리(2장) 후보를 찾기 위해 패를 그룹화 (숫자와 원소가 완전히 같은 장수 체크)
        // 결상 검증을 편하게 하기 위해 ID 기반 리스트로 변환하여 처리
        List<TileData> sortedHand = hand.OrderBy(t => t.element).ThenBy(t => t.value).ToList();

        // 모든 카드를 순회하며 '머리' 후보를 하나씩 지정해봄
        for (int i = 0; i < sortedHand.Count - 1; i++)
        {
            // 머리가 되려면 완전히 똑같은 카드 2장이거나, 족보 규칙에 따라 같은 숫자 2장이어야 함 (여기선 완전 일치 기준으로 기초 설계)
            if (sortedHand[i].element == sortedHand[i + 1].element && sortedHand[i].value == sortedHand[i + 1].value)
            {
                // 머리 후보 2장을 임시로 제외한 복사본 생성
                List<TileData> remainingRemaining = new List<TileData>(sortedHand);
                TileData head1 = sortedHand[i];
                TileData head2 = sortedHand[i + 1];
                remainingRemaining.Remove(head1);
                remainingRemaining.Remove(head2);

                // 남은 6장이 올바른 몸통 2개(3장씩)로 쪼개지는지 검증
                if (CheckBody(remainingRemaining))
                {
                    // 결상 성공! 이제 족보(역) 점수를 계산함
                    finalMultiplier = CalculateScore(hand, out achievedYaku);
                    return true;
                }
            }
        }

        return false;
    }

    // ----------------------------------------------------
    // [몸통 검증] 남은 6장(또는 3장)이 규칙에 맞는 몸통인지 재귀적으로 검증
    // ----------------------------------------------------
    private static bool CheckBody(List<TileData> tiles)
    {
        // 남은 타일이 없으면 모든 몸통 분해에 성공한 것!
        if (tiles.Count == 0) return true;

        // 기준이 될 첫 번째 타일 선택
        TileData first = tiles[0];

        // 조건 C: 완전히 같은 카드 3장 (동일 원소 + 동일 숫자)
        var matchC = tiles.Where(t => t.element == first.element && t.value == first.value).Take(3).ToList();
        if (matchC.Count == 3)
        {
            List<TileData> nextTiles = new List<TileData>(tiles);
            foreach (var t in matchC) nextTiles.Remove(t);
            if (CheckBody(nextTiles)) return true;
        }

        // 조건 A: 같은 원소이면서 연속된 숫자 3장 (순행 / 슌쯔)
        var next1 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 1);
        var next2 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 2);
        if (next1 != null && next2 != null)
        {
            List<TileData> nextTiles = new List<TileData>(tiles);
            nextTiles.Remove(first);
            nextTiles.Remove(next1);
            nextTiles.Remove(next2);
            if (CheckBody(nextTiles)) return true;
        }

        // 조건 B: 다른 원소이면서 완전히 같은 숫자 3장 (삼재 다른 버전)
        // first와 숫자는 같지만 원소가 다른 카드들을 조합해봄
        var sameValues = tiles.Where(t => t.value == first.value).ToList();
        if (sameValues.Count >= 3)
        {
            // 원소가 서로 다른 3장을 추출할 수 있는지 체크
            var uniqueElements = sameValues.GroupBy(t => t.element).Select(g => g.First()).Take(3).ToList();
            if (uniqueElements.Count == 3)
            {
                List<TileData> nextTiles = new List<TileData>(tiles);
                foreach (var t in uniqueElements) nextTiles.Remove(t);
                if (CheckBody(nextTiles)) return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------
    // [족보 스코어러] 완성된 8장의 패를 분석해 최종 배수를 연산
    // ----------------------------------------------------
    private static int CalculateScore(List<TileData> hand, out List<string> yakuList)
    {
        yakuList = new List<string>();
        int multiplier = 1; // 기본 1배 시작

        // 1. 태극 (모든 패가 단 하나의 원소 속성으로만 이루어짐)
        bool isTaegeuk = hand.GroupBy(t => t.element).Count() == 1;
        if (isTaegeuk)
        {
            yakuList.Add("태극(올 컬러) [+4배]");
            multiplier += 4;
        }

        // 2. 평성 / 순진 후보 검증을 위한 숫자 정렬
        List<int> values = hand.Select(t => t.value).OrderBy(v => v).ToList();

        // 3. 삼재 중심 패인지 검사 (동일 숫자가 3개 이상인 그룹이 2개 이상)
        int tripleCount = hand.GroupBy(t => t.value).FuncCount(g => g.Count() >= 3);
        if (tripleCount >= 2)
        {
            yakuList.Add("중중삼재(더블 트리플) [+3배]");
            multiplier += 3;
        }

        // 만약 아무런 특수 족보가 없다면 기본 평성(기본 완패) 처리
        if (yakuList.Count == 0)
        {
            yakuList.Add("평성(기본 화료) [1배]");
        }

        return multiplier;
    }
}

// GroupBy Count 예외 방지용 간단한 확장 메서드 헬퍼
public static class LinqHelper
{
    public static int FuncCount<T>(this IEnumerable<T> source, System.Func<T, bool> predicate)
    {
        return source.Count(predicate);
    }
}