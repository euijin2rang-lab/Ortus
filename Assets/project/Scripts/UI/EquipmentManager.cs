using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    // 어디서든 쉽게 부를 수 있게 Instance 설정
    public static EquipmentManager Instance { get; private set; }

    [Header("현재 장비 세팅 중인 캐릭터 ID")]
    public string currentTargetCharId = ""; 

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // 장비 카드에서 이 함수를 호출할 거야!
    public void EquipItemToCharacter(string equipId)
    {
        if (string.IsNullOrEmpty(currentTargetCharId))
        {
            Debug.LogError("장착할 캐릭터가 선택되지 않았어!");
            return;
        }

        // 여기서 실제 장착 데이터 변경 로직이 들어감 (RTDB나 로컬 변수 수정)
        Debug.Log($"[장비 장착 완료] 캐릭터({currentTargetCharId})에게 장비({equipId})를 꼈어!");

        // 장착 끝났으니까 이전 화면(캐릭터 세부 창)으로 돌아가기
        ScreenManager.Instance.GoBack();
    }
}