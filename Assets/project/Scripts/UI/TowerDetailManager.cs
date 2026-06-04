using UnityEngine;
using TMPro; // 만약 TextMeshPro를 쓴다면 using TMPro; 로 바꾸고 Text를 TextMeshProUGUI로 바꿔줘!

public class TowerDetailManager : MonoBehaviour
{
    // 💡 싱글톤: 어디서든 쉽게 접근할 수 있게 만들어주는 마법의 코드
    public static TowerDetailManager Instance;

    [Header("화면 및 UI 연결")]
    public GameObject detailScreenObj; // Screen_TowerDetail 전체 화면 오브젝트
    public TextMeshProUGUI txtFloorName;          
    public TextMeshProUGUI txtRuleDesc;           
    public TextMeshProUGUI txtEnemyName;          // 적 이름 텍스트

    private void Awake()
    {
        // 씬이 시작될 때 자기 자신을 Instance에 등록
        Instance = this;
        
        // 시작할 땐 세부 화면이 안 보이게 꺼두기
        if (detailScreenObj != null) 
            detailScreenObj.SetActive(false);
    }

        public void OpenDetailScreen(int floorLevel, string rule, string enemy)
    {
        if (txtFloorName == null || txtRuleDesc == null || txtEnemyName == null)
        {
            Debug.LogError("[TowerDetailManager] 텍스트 연결 누락!");
            return; 
        }

        // 1. 텍스트 글씨 바꿔치기
        txtFloorName.text = $"몽환의 탑 {floorLevel}층";
        txtRuleDesc.text = rule;
        txtEnemyName.text = enemy;

        // 2. 💡 핵심 변경점: 냅다 SetActive(true)를 하는 게 아니라, ScreenManager를 통해 화면 이동!
        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.OpenScreen(detailScreenObj);
        }
        else
        {
            // ScreenManager가 혹시 없을 때를 대비한 안전빵
            detailScreenObj.SetActive(true); 
        }
    }

// 💡 더 이상 안 쓰는 이 함수는 지우거나 냅둬도 무방해! (공통 뒤로가기 쓸 거니까)
// public void CloseDetailScreen() { ... }
}