using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DeckCardClick : MonoBehaviour
{
    private void Start()
    {
        Button btn = GetComponent<Button>();
        
        btn.onClick.AddListener(() => 
        {
            if (ScreenManager.Instance != null)
            {
                // 최상위 Canvas 아래에서 방금 만든 Screen_DeckDetail을 찾음
                Transform detailScreen = GameObject.Find("Canvas").transform.Find("Screen_DeckDetail");
                
                if (detailScreen != null)
                {
                    // ScreenManager의 OpenScreen 함수로 화면 열기!
                    ScreenManager.Instance.OpenScreen(detailScreen.gameObject);
                }
                else
                {
                    Debug.LogError("Screen_DeckDetail 오브젝트를 찾을 수 없어! 캔버스 아래에 이름이 정확히 맞는지 확인해.");
                }
            }
        });
    }
}