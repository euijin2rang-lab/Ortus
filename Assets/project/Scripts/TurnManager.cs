using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 턴 상태 열거형
public enum TurnState
{
    PlayerTurn_Draw,       
    PlayerTurn_WaitInput,  
    PlayerTurn_Discard,    
    AITurn_Draw,           
    AITurn_Action,         
    AITurn_Discard         
}

public class TurnManager : MonoBehaviour
{
    [Header("[ Current Turn State ]")]
    public TurnState currentState;
    public bool isGameActive = true; 

    [Header("[ Turn Counter (GDD 1.0) ]")]
    public int currentTurn = 1; 

    [Header("[ References ]")]
    public DeckManager deckManager;
    public PlayerHand playerHand;
    public PlayerHand aiHand;
    
    [HideInInspector] public SimpleAI simpleAI; 

    [Header("[ Discard Pool ]")]
    public List<TileData> discardPool = new List<TileData>();

    [Header("[ 배정할 캐릭터 데이터 ]")]
    public CharacterData playerSelectedCharacter; 
    public CharacterData aiSelectedCharacter;     

    private void Start()
    {
        isGameActive = true;
        currentTurn = 1;

        if (playerSelectedCharacter != null) playerHand.InitializeCharacter(playerSelectedCharacter);
        if (aiSelectedCharacter != null) aiHand.InitializeCharacter(aiSelectedCharacter);

        simpleAI = aiHand.GetComponent<SimpleAI>();
        if (simpleAI == null) simpleAI = aiHand.gameObject.AddComponent<SimpleAI>();
        simpleAI.Setup(aiHand, this);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        ChangeState(TurnState.PlayerTurn_Draw);
    }

    private void Update()
    {
        if (!isGameActive) return; 

        if (currentState == TurnState.PlayerTurn_WaitInput)
        {
            HandlePlayerInput();
            HandleSkillInput(); // 스킬 입력 감지
        }
    }

    public void ChangeState(TurnState newState)
    {
        if (!isGameActive) return;
        if (CheckInstantDeath()) return; // 실시간 사망 체크

        currentState = newState;
        Debug.Log($"<color=cyan>[상태 전환]</color> 현재 상태: <b>{currentState}</b> (현재 매치 턴: {currentTurn}/10)");

        switch (currentState)
        {
            case TurnState.PlayerTurn_Draw:
                StartCoroutine(PlayerDrawRoutine());
                break;

            case TurnState.PlayerTurn_WaitInput:
                LogCurrentHand();
                Debug.Log($"<color=yellow>[플레이어 턴]</color> 현재 {currentTurn}턴. 버릴 카드의 번호(1~8키)를 누르세요. 스킬은 [S]키!");
                break;

            case TurnState.PlayerTurn_Discard:
                ChangeState(TurnState.AITurn_Draw);
                break;

            case TurnState.AITurn_Draw:
                simpleAI.ExecuteTurn(); // AI 주도권 이양
                break;

            case TurnState.AITurn_Discard:
                currentTurn++; 
                if (CheckTimeout()) return; // 10턴 초과 유국 판정
                ChangeState(TurnState.PlayerTurn_Draw);
                break;
        }
    }

    private IEnumerator PlayerDrawRoutine()
    {
        Debug.Log("<color=green>[관성]</color> 플레이어가 덱에서 카드를 뽑습니다.");
        yield return new WaitForSeconds(0.6f);

        if (playerHand.handList.Count < 8)
        {
            deckManager.DrawCardTo(playerHand);
        }

        if (CheckWinningCondition(playerHand, aiHand)) yield break;

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

private void HandleSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 🛡️ 1차 가드: 플레이어 컴포넌트 자체가 비어있는가 체크
            if (playerHand == null) return;

            // 🛡️ 2차 가드: 프로퍼티를 통해 기문 매니저를 가져왔는데도 비어있는지 체크 (1단계 덕분에 절대 안 비게 됨)
            if (playerHand.gimunManager == null)
            {
                Debug.LogWarning("[스킬 락] 기문 매니저가 아직 생성되지 않았거나 초기화 중입니다.");
                return;
            }

            CharacterData myChar = playerHand.characterData;
            
            // 🛡️ 3차 가드: 인스펙터에 에셋이 제대로 안 꽂혔거나 데이터가 날아갔을 경우 방어
            if (myChar == null || string.IsNullOrEmpty(myChar.characterName))
            {
                Debug.LogWarning("[스킬 락] 인스펙터에 CharacterData 에셋이 유실되었거나 이름이 비어있습니다.");
                return;
            }

            // 기본 코스트 분기 설정
            int cost = 3; 
            if (myChar.characterName.Contains("아리스")) cost = 4;
            else if (myChar.characterName.Contains("카르키")) cost = 2;

            if (playerHand.gimunManager.CanCast(cost))
            {
                FileSkillAction skillAction = null;

                // 문자열 매칭 오류 방지를 위해 Contains 가용
                if (myChar.characterName.Contains("아리스")) skillAction = new Action_Aris(playerHand, aiHand, this);
                else if (myChar.characterName.Contains("멜리노에")) skillAction = new Action_Melinoe(playerHand, aiHand, this);
                else if (myChar.characterName.Contains("네르")) skillAction = new Action_Ner(playerHand, aiHand);
                else if (myChar.characterName.Contains("카르키")) skillAction = new Action_Karki(playerHand, aiHand, this);

                if (skillAction != null)
                {
                    SkillSystem.Instance.EnqueueSkill(skillAction);
                }
            }
            else
            {
                Debug.LogWarning($"[기문 부족] 스킬 기문 게이지 부족! (필요 코스트: {cost} = 게이지 {cost * 20} / 현재 게이지: {playerHand.gimunManager.currentGauge})");
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

    public bool CheckWinningCondition(PlayerHand attacker, PlayerHand defender)
    {
        int rawMultiplier;
        List<string> yaku;

        if (WinningChecker.CheckWin(attacker.handList, out rawMultiplier, out yaku))
        {
            isGameActive = false; 
            float finalMultiplier = rawMultiplier / 100f;

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.ProcessBattle(attacker, defender, finalMultiplier, yaku);
            }

            EvaluateFinalResult(false);
            return true;
        }
        return false;
    }

    private bool CheckInstantDeath()
    {
        if (playerHand.currentHP <= 0 || aiHand.currentHP <= 0)
        {
            isGameActive = false;
            Debug.Log("<color=red><b>[사망 감지] 어느 한 진영의 명운(HP)이 전부 소진되었습니다!</b></color>");
            EvaluateFinalResult(true); 
            return true;
        }
        return false;
    }

    private bool CheckTimeout()
    {
        if (currentTurn > 10)
        {
            isGameActive = false;
            Debug.Log("<color=red><b>[유국] 10턴 타임아웃 되었습니다!</b></color>");
            EvaluateFinalResult(false); 
            return true;
        }
        return false;
    }

    private void EvaluateFinalResult(bool isKO)
    {
        isGameActive = false;
        Debug.Log("<color=magenta><b>===========================================</b></color>");
        if (isKO) Debug.Log("<color=magenta><b>💀 [매치 종료] HP 0 소진으로 인한 KO 승패 확정 💀</b></color>");
        else Debug.Log("<color=cyan><b>⏳ [매치 종료] 10턴 제한 도달 정산 ⏳</b></color>");
            
        Debug.Log($"최종 스코어 -> 플레이어 명운: {playerHand.currentHP} | AI 명운: {aiHand.currentHP}");

        if (playerHand.currentHP > aiHand.currentHP) Debug.Log("<color=cyan><b>🎉 PLAYER WIN! 🎉</b></color>");
        else if (aiHand.currentHP > playerHand.currentHP) Debug.Log("<color=red><b>💀 AI WIN! 💀</b></color>");
        else Debug.Log("<color=yellow><b>🤝 DRAW! 🤝</b></color>");
        Debug.Log("<color=magenta><b>===========================================</b></color>");
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