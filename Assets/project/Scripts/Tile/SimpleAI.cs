using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    private PlayerHand aiHand;
    private TurnManager turnManager;

    public void Setup(PlayerHand hand, TurnManager manager)
    {
        aiHand = hand;
        turnManager = manager;
    }

    // ----------------------------------------------------
    // [메인 로직] AI의 행동을 개시하는 함수 (TurnManager가 호출함)
    // ----------------------------------------------------
    public void ExecuteTurn()
    {
        StartCoroutine(AIRoutine());
    }

    private IEnumerator AIRoutine()
    {
        // 1. AI 드로우 동기화: 덱에서 패를 1장 뽑아 AI 손패에 넣음 (총 8장)
        Debug.Log("<color=blue>[AI 턴]</color> AI가 덱에서 카드를 뽑습니다.");
        yield return new WaitForSeconds(0.6f);

        if (aiHand.handList.Count < 8)
        {
            turnManager.deckManager.DrawCardTo(aiHand);
        }

        // 2. 자체 결상 체크: 8장이 되었으니 스스로 화료했는지 검증
        // (TurnManager가 드로우 직후 체크해주지만, AI 자체 행동 단계의 정석 흐름 유지)
        if (turnManager.CheckWinningCondition(aiHand, turnManager.playerHand))
        {
            yield break; // 자기가 완성되었다면 여기서 행동을 종료하고 매치를 끝냄
        }

        // 3. 랜덤 투산: 결상이 아니라면 패 고민 후 무작위로 한 장 버림
        Debug.Log("<color=blue>[AI 턴]</color> AI가 패를 고민 중...");
        yield return new WaitForSeconds(0.8f);

        if (aiHand.handList.Count > 0)
        {
            // UnityEngine.Random.Range(0, 8)로 무작위 인덱스 추출
            int randomIndex = Random.Range(0, aiHand.handList.Count);
            TileData discardedTile = aiHand.RemoveTile(randomIndex);

            if (discardedTile != null)
            {
                turnManager.discardPool.Add(discardedTile);
                Debug.Log($"<color=red>[투산]</color> AI가 무작위로 <b>[{discardedTile.tileName}]</b> (패 {randomIndex + 1}번째)를 버렸습니다.");
            }
        }

        // 4. 투산이 끝났으니 AI의 디스카드 상태를 완료하고 플레이어 턴으로 넘김
        turnManager.ChangeState(TurnState.AITurn_Discard);
    }
}