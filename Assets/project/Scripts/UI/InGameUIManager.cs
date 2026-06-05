using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    // [HideInInspector]를 달아서 인스펙터에서 아예 안 보이게 가림. (드래그 할 필요 없음)
    [HideInInspector] 
    public TurnManager turnManager; 

    [Header("[ 시스템 버튼 UI ]")]
    public GameObject skillButton;      
    public GameObject gyeolsangButton;  
    public GameObject discardButton;    

    private void Start()
    {
        // 씬이 분리되어 있어도 유니티 전체를 뒤져서 TurnManager를 무조건 찾아옴
        turnManager = FindFirstObjectByType<TurnManager>();

        if (turnManager == null)
        {
            Debug.LogError("TurnManager를 못 찾음! GameManager가 씬에 있는지 확인해.");
        }
    }

    private void Update()
    {
        // 턴 매니저나 플레이어 세팅이 안 끝났으면 작동 안 함
        if (turnManager == null || turnManager.playerHand == null) return;

        // 플레이어가 조작할 수 있는 턴일 때만 버튼 활성화 여부 계산
        if (turnManager.currentState == TurnState.PlayerTurn_WaitInput)
        {
            CheckSkillButton();
            CheckGyeolsangButton();
            
            if (discardButton != null) discardButton.SetActive(true);
        }
        else
        {
            // 내 턴이 아니면 모든 버튼 숨기기
            skillButton.SetActive(false);
            gyeolsangButton.SetActive(false);
            if (discardButton != null) discardButton.SetActive(false);
        }
    }

    private void CheckSkillButton()
    {
        var pHand = turnManager.playerHand;

        if (pHand.gimunManager == null || pHand.characterData == null)
        {
            skillButton.SetActive(false);
            return;
        }

        string charName = pHand.characterData.characterName;
        int cost = 3; 
        if (charName.Contains("아리스")) cost = 4;
        else if (charName.Contains("카르키")) cost = 2;

        if (pHand.gimunManager.CanCast(cost))
        {
            skillButton.SetActive(true);
        }
        else
        {
            skillButton.SetActive(false);
        }
    }

    private void CheckGyeolsangButton()
    {
        int dummyMultiplier;
        List<string> dummyYaku;

        bool canWin = WinningChecker.CheckWin(turnManager.playerHand.handList, out dummyMultiplier, out dummyYaku);

        if (canWin)
        {
            gyeolsangButton.SetActive(true);
        }
        else
        {
            gyeolsangButton.SetActive(false);
        }
    }
}