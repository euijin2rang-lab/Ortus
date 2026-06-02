using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] // 버튼 컴포넌트가 없으면 알아서 달아줌
public class CharCardClick : MonoBehaviour
{
    private void Start()
    {
        // 1. 이 프리팹에 달린 버튼을 가져온다.
        Button myButton = GetComponent<Button>();

        // 2. 버튼의 On Click 이벤트에 코드로 직접 함수를 연결해버린다!
        myButton.onClick.AddListener(() => 
        {
            // 씬에 있는 ProfileManager를 찾아서 팝업 여는 함수를 실행!
            ProfileManager manager = FindObjectOfType<ProfileManager>();
            
            if (manager != null)
            {
                manager.OpenConfirmPopup();
            }
            else
            {
                Debug.LogError("화면에 ProfileManager가 없어요!");
            }
        });
    }
}