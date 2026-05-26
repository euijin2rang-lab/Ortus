using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WinningChecker
{
    public static bool CheckWin(List<TileData> hand, out int finalMultiplier, out List<string> achievedYaku)
    {
        finalMultiplier = 0;
        achievedYaku = new List<string>();

        if (hand == null || hand.Count != 8) return false;

        bool hasCheatedTile = hand.GroupBy(t => new { t.element, t.value }).Any(g => g.Count() >= 5);
        if (hasCheatedTile)
        {
            Debug.LogWarning("<color=red>[반칙 패 감지]</color> 동일한 타일이 5장 이상 중복되어 결상 판정을 즉시 취소합니다.");
            return false;
        }

        List<TileData> sortedHand = hand.OrderBy(t => t.element).ThenBy(t => t.value).ToList();

        for (int i = 0; i < sortedHand.Count - 1; i++)
        {
            if (sortedHand[i].element == sortedHand[i + 1].element && sortedHand[i].value == sortedHand[i + 1].value)
            {
                List<TileData> remainingTiles = new List<TileData>(sortedHand);
                TileData head1 = sortedHand[i];
                TileData head2 = sortedHand[i + 1];
                
                remainingTiles.Remove(head1);
                remainingTiles.Remove(head2);

                if (ValidateBodyRecursive(remainingTiles))
                {
                    string casterName = "";
                    
                    // 🛠️ [교정] 유니티 6 사양에 맞추어 Deprecated된 FindObjectOfType을 신형인 FindFirstObjectByType으로 보정
                    var turnManager = Object.FindFirstObjectByType<TurnManager>();
                    if (turnManager != null)
                    {
                        if (turnManager.playerHand.handList.SequenceEqual(hand)) casterName = turnManager.playerHand.characterData.characterName;
                        else if (turnManager.aiHand.handList.SequenceEqual(hand)) casterName = turnManager.aiHand.characterData.characterName;
                    }

                    float score = ScoreCalculator.CalculateFinalMultiplier(hand, out achievedYaku, casterName);
                    finalMultiplier = Mathf.RoundToInt(score * 100f); 
                    return true;
                }
                i++; 
            }
        }
        return false;
    }

    private static bool ValidateBodyRecursive(List<TileData> tiles)
    {
        if (tiles.Count == 0) return true;
        TileData first = tiles[0];

        var sameTiles = tiles.Where(t => t.element == first.element && t.value == first.value).Take(3).ToList();
        if (sameTiles.Count == 3)
        {
            List<TileData> nextHand = new List<TileData>(tiles);
            nextHand.Remove(sameTiles[0]); nextHand.Remove(sameTiles[1]); nextHand.Remove(sameTiles[2]);
            if (ValidateBodyRecursive(nextHand)) return true;
        }

        TileData next1 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 1);
        TileData next2 = tiles.FirstOrDefault(t => t.element == first.element && t.value == first.value + 2);
        if (next1 != null && next2 != null)
        {
            List<TileData> nextHand = new List<TileData>(tiles);
            nextHand.Remove(first); nextHand.Remove(next1); nextHand.Remove(next2);
            if (ValidateBodyRecursive(nextHand)) return true;
        }

        var sameValueGroup = tiles.Where(t => t.value == first.value).ToList();
        if (sameValueGroup.Count >= 3)
        {
            var uniqueElementCombo = sameValueGroup.GroupBy(t => t.element).Select(g => g.First()).ToList();
            if (uniqueElementCombo.Count >= 3)
            {
                var targetCombo = uniqueElementCombo.Take(3).ToList();
                List<TileData> nextHand = new List<TileData>(tiles);
                nextHand.Remove(targetCombo[0]); nextHand.Remove(targetCombo[1]); nextHand.Remove(targetCombo[2]);
                if (ValidateBodyRecursive(nextHand)) return true;
            }
        }
        return false;
    }
}