using UnityEngine;

public class DeckDetailManager : MonoBehaviour
{
    [Header("장비 목록 화면")]
    public GameObject equipListScreen; // 인스펙터에서 Screen_EquipDetail 연결

    public void OnClickEquipButton()
    {
        // 1. 지금 화면에 띄워진 캐릭터의 ID를 매니저한테 알려줌 (임시로 "Char_01" 할당)
        EquipmentManager.Instance.currentTargetCharId = "Char_01";
        
        // 2. 장비 목록 화면으로 이동
        ScreenManager.Instance.OpenScreen(equipListScreen);
    }
}