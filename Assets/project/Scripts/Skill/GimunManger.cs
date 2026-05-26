using UnityEngine;

public class GimunManager : MonoBehaviour
{
    private PlayerHand ownerHand;

    [Header("[ 기문 게이지 (GDD 1.0) ]")]
    public int currentGauge = 0;
    public const int MaxGauge = 100;

    public void Setup(PlayerHand hand)
    {
        ownerHand = hand;
        currentGauge = 0;
    }

    // 패를 버릴 때 호출되는 게이지 충전 로직
    public void AddGauge(ElementType discardedElement)
    {
        if (ownerHand.characterData == null) return;

        // 내 캐릭터의 기숙사 속성과 버린 패의 속성 일치 여부 판정
        bool isMatch = ownerHand.characterData.houseElement == discardedElement;
        int amount = isMatch ? 20 : 10;

        currentGauge = Mathf.Min(currentGauge + amount, MaxGauge);
        
        Debug.Log($"<color=teal>[기문 충전]</color> {ownerHand.gameObject.name}이(가) [{discardedElement}] 속성을 투산하여 기문 <b>+{amount}</b> 충전 (현재: {currentGauge}/{MaxGauge})");
    }

    // 스킬 사용 가능 여부 체크 및 코스트 차감
    public bool CanCast(int cost)
    {
        // 기획서의 필요 기문(Cost) 단위를 게이지 통에 맞춰 환산 (예: 코스트 1당 게이지 20 소모)
        int requiredGauge = cost * 20; 

        if (currentGauge >= requiredGauge)
        {
            currentGauge -= requiredGauge;
            return true;
        }
        return false;
    }

    // 멜리노에 스킬용 게이지 강제 충전 헬퍼
    public void ForceAddGauge(int amount)
    {
        currentGauge = Mathf.Min(currentGauge + amount, MaxGauge);
    }
}