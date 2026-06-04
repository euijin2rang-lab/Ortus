using UnityEngine;
using UnityEngine.UI;

// 💡 팁: 이 스크립트를 붙이면 Button 컴포넌트가 자동으로 생겨!
[RequireComponent(typeof(Button))]
public class TowerFloorItem : MonoBehaviour
{
    [Header("이 층의 고유 데이터")]
    public int floorLevel = 1; 
    public string ruleText = "명운 10 감소"; 
    public string enemyName = "보스 슬라임";

    private void Start()
    {
        // 내 오브젝트에 달려있는 버튼을 가져와서 클릭 이벤트를 달아줌
        Button myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClickThisFloor);
    }

    private void OnClickThisFloor()
    {
        Debug.Log($"[{floorLevel}층] 패널 클릭됨!");

        // 아까 만든 매니저가 씬에 존재한다면, 내 정보를 넘기면서 화면 띄워달라고 요청!
        if (TowerDetailManager.Instance != null)
        {
            TowerDetailManager.Instance.OpenDetailScreen(floorLevel, ruleText, enemyName);
        }
        else
        {
            Debug.LogError("앗! 씬에 TowerDetailManager가 없어!");
        }
    }
}