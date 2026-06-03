using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArenaOpponentClick : MonoBehaviour
{
    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => 
        {
            if (ScreenManager.Instance != null)
            {
                Transform detailScreen = GameObject.Find("Canvas").transform.Find("Screen_ArenaDetail");
                
                if (detailScreen != null)
                {
                    // ✨ 네 원본 코드에 맞춰서 OpenScreen으로 변경!
                    ScreenManager.Instance.OpenScreen(detailScreen.gameObject);
                }
                else
                {
                    Debug.LogError("Screen_ArenaDetail 오브젝트를 찾을 수 없습니다.");
                }
            }
        });
    }
}