using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject loginPopup; // Inspector에서 로그인 팝업 오브젝트 연결

    private void Start()
    {
        // 시작할 때 팝업은 무조건 꺼두기
        if (loginPopup != null)
            loginPopup.SetActive(false);
    }

    public void OnTouchToStartClicked()
    {
        bool isLoggedIn = false; 

        if (isLoggedIn)
        {
            // 💡 OpenScreen 대신 OpenRootScreen 으로 변경!
            ScreenManager.Instance.OpenRootScreen(ScreenManager.Instance.mainLobbyScreen);
        }
        else
        {
            if (loginPopup != null)
                loginPopup.SetActive(true);
        }
    }

    public void OnLoginSuccess()
    {
        loginPopup.SetActive(false);
        // 💡 여기도 OpenRootScreen 으로 변경!
        ScreenManager.Instance.OpenRootScreen(ScreenManager.Instance.mainLobbyScreen);
    }
}