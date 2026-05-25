using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    PlayerTurn_Draw,       // 플레이어 드로우
    PlayerTurn_WaitInput,  // 플레이어 고민 (입력 대기)
    PlayerTurn_Discard,    // 플레이어 투산(버리기)
    AITurn_Draw,           // AI 드로우
    AITurn_Action,         // AI 고민 (자동 연산)
    AITurn_Discard         // AI 투산(버리기)
}

public class TurnManager : MonoBehaviour
{
    [Header("[ Current Turn State ]")]
    public TurnState currentState;
    public bool isGameActive = true; // 게임 진행 중 여부

    [Header("[ Turn Counter ]")]
    public int currentTurnCount = 0; // 현재 진행된 총 턴 수 (최대 20)

    [Header("[ References ]")]
    public DeckManager deckManager;
    public PlayerHand playerHand;
    public PlayerHand aiHand;

    [Header("[ Discard Pool ]")]
    public List<TileData> discardPool = new List<TileData>();

    private void Start()
    {
        isGameActive = true;
        currentTurnCount = 0;
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        ChangeState(TurnState.PlayerTurn_Draw);
    }

    private void Update()
    {
        if (!isGameActive) return; // 게임이 종료되었다면 입력 무시

        if (currentState == TurnState.PlayerTurn_WaitInput)
        {
            HandlePlayerInput();
        }
    }

    public void ChangeState(TurnState newState)
    {
        if (!isGameActive) return; // 게임 종료 시 상태 전환 차단

        currentState = newState;
        Debug.Log($"<color=cyan>[상태 전환]</color> 현재 상태: <b>{currentState}</b> (총 {currentTurnCount}/20 턴)");

        switch (currentState)
        {
            case TurnState.PlayerTurn_Draw:
                currentTurnCount++;
                if (CheckTurnOver()) return; // 턴 한도 초과 체크
                StartCoroutine(PlayerDrawRoutine());
                break;

            case TurnState.PlayerTurn_WaitInput:
                LogCurrentHand();
                Debug.Log("<color=yellow>[플레이어 턴]</color> 버릴 카드의 번호(1~8키)를 누르세요.");
                break;

            case TurnState.PlayerTurn_Discard:
                ChangeState(TurnState.AITurn_Draw);
                break;

            case TurnState.AITurn_Draw:
                currentTurnCount++;
                if (CheckTurnOver()) return; // 턴 한도 초과 체크
                StartCoroutine(AIDrawRoutine());
                break;

            case TurnState.AITurn_Action:
                StartCoroutine(AIActionRoutine());
                break;

            case TurnState.AITurn_Discard:
                ChangeState(TurnState.PlayerTurn_Draw);
                break;
        }
    }

    // ----------------------------------------------------
    // [플레이어] 드로우 및 투산
    // ----------------------------------------------------
    private IEnumerator PlayerDrawRoutine()
    {
        Debug.Log("<color=green>[관성]</color> 플레이어가 덱에서 카드를 뽑습니다.");
        yield return new WaitForSeconds(0.6f);

        if (playerHand.handList.Count < 8)
        {
            deckManager.DrawCardTo(playerHand);
        }

        // 8장이 된 순간 즉시 화료(결상) 체크를 수행
        if (CheckWinningCondition(playerHand, aiHand))
        {
            yield break; // 결상 성공 시 턴 흐름 정지
        }

        ChangeState(TurnState.PlayerTurn_WaitInput);
    }

    private void HandlePlayerInput()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i < playerHand.handList.Count)
                {
                    PlayerDiscard(i);
                }
            }
        }
    }

    private void PlayerDiscard(int index)
    {
        TileData discardedTile = playerHand.RemoveTile(index);
        if (discardedTile != null)
        {
            discardPool.Add(discardedTile);
            Debug.Log($"<color=red>[투산]</color> 플레이어가 <b>[{discardedTile.tileName}]</b>를 버렸습니다.");
        }
        ChangeState(TurnState.PlayerTurn_Discard);
    }

    // ----------------------------------------------------
    // [AI] 드로우 및 투산
    // ----------------------------------------------------
    private IEnumerator AIDrawRoutine()
    {
        Debug.Log("<color=blue>[AI 턴]</color> AI가 덱에서 카드를 뽑습니다.");
        yield return new WaitForSeconds(0.6f);

        if (aiHand.handList.Count < 8)
        {
            deckManager.DrawCardTo(aiHand);
        }

        // AI도 8장이 된 순간 결상 체크
        if (CheckWinningCondition(aiHand, playerHand))
        {
            yield break;
        }

        ChangeState(TurnState.AITurn_Action);
    }

    private IEnumerator AIActionRoutine()
    {
        Debug.Log("<color=blue>[AI 턴]</color> AI가 패를 고민 중...");
        yield return new WaitForSeconds(0.8f);

        int randomIndex = Random.Range(0, aiHand.handList.Count);
        TileData discardedTile = aiHand.RemoveTile(randomIndex);

        if (discardedTile != null)
        {
            discardPool.Add(discardedTile);
            Debug.Log($"<color=red>[투산]</color> AI가 무작위로 <b>[{discardedTile.tileName}]</b>를 버렸습니다.");
        }

        ChangeState(TurnState.AITurn_Discard);
    }

    // ----------------------------------------------------
    // [전투 연산] 결상 및 데미지 프로세스
    // ----------------------------------------------------
    private bool CheckWinningCondition(PlayerHand attacker, PlayerHand defender)
    {
        int multiplier;
        List<string> yaku;

        // WinningChecker를 통해 결상 판정
        if (WinningChecker.CheckWin(attacker.handList, out multiplier, out yaku))
        {
            isGameActive = false; // 루프 정지

            Debug.Log($"<color=red><b>✨✨✨ 결상(화료) 발생!!! ({attacker.gameObject.name}) ✨✨✨</b></color>");
            foreach (string y in yaku)
            {
                Debug.Log($"<color=pink>획득 족보: {y}</color>");
            }

            // GDD 1.0 데미지 공식: Base ATK * 족보 배수
            int finalDamage = Mathf.RoundToInt(attacker.baseATK * multiplier);
            Debug.Log($"<color=yellow>[공격력 연산]</color> {attacker.gameObject.name}의 Base ATK({attacker.baseATK}) x 족보 배수({multiplier}배) = <b>최종 데미지: {finalDamage}</b>");

            // 상대방 명운(HP) 깎기
            defender.TakeDamage(finalDamage);

            // 매치 종료 판정 호출
            EvaluateMatchResult();
            return true;
        }
        return false;
    }

    // ----------------------------------------------------
    // [종료 처리] 매치 엔드포인트 및 승패 조건 판정
    // ----------------------------------------------------
    private bool CheckTurnOver()
    {
        // 각자 10번씩 총 20턴이 지나면 종료
        if (currentTurnCount > 20)
        {
            isGameActive = false;
            Debug.Log("<color=red><b>[타임오버] 각자 10번의 턴(총 20턴)이 모두 소모되었습니다!</b></color>");
            EvaluateMatchResult();
            return true;
        }
        return false;
    }

    private void EvaluateMatchResult()
    {
        isGameActive = false;
        Debug.Log("<color=green><b>-------------------------------------------</b></color>");
        Debug.Log("<color=green><b>🏆 [매치 엔드] 최종 승패 판정 결과 🏆</b></color>");
        Debug.Log($"플레이어 최종 HP: {playerHand.currentHP} | AI 최종 HP: {aiHand.currentHP}");

        if (playerHand.currentHP > aiHand.currentHP)
        {
            Debug.Log("<color=cyan><b>🎉 PLAYER WIN! 플레이어의 성명이 더 높아 매치에서 승리했습니다! 🎉</b></color>");
        }
        else if (aiHand.currentHP > playerHand.currentHP)
        {
            Debug.Log("<color=red><b>💀 AI WIN! AI의 성명이 더 높아 매치에서 패배했습니다... 💀</b></color>");
        }
        else
        {
            Debug.Log("<color=yellow><b>🤝 DRAW! 두 진영의 남은 성명이 완벽히 일치하여 무승부입니다! 🤝</b></color>");
        }
        Debug.Log("<color=green><b>-------------------------------------------</b></color>");
    }

    private void LogCurrentHand()
    {
        string handLog = "현재 플레이어 손패: [ ";
        for (int i = 0; i < playerHand.handList.Count; i++)
        {
            handLog += $"{i + 1}:{playerHand.handList[i].tileName} ";
        }
        handLog += "]";
        Debug.Log($"<color=white>{handLog}</color>");
    }
}