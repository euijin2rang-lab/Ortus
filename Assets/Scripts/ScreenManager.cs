using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Header("Screens")]
    public List<GameObject> allScreens = new List<GameObject>();
    public GameObject mainLobbyScreen; // ← Inspector에서 직접 MainLobby 드래그

    [Header("HUD Elements")]
    public GameObject profileUI;
    public GameObject currencyUI;   // 재화 UI (항상 표시)
    public GameObject backButtonUI;

    private Stack<GameObject> screenStack = new Stack<GameObject>();
    private GameObject currentScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // allScreens 전체 비활성화
        foreach (var screen in allScreens)
        {
            if (screen != null)
                screen.SetActive(false);
        }

        // MainLobby 진입
        if (mainLobbyScreen != null)
            OpenScreen(mainLobbyScreen);
    }

    public void OpenScreen(GameObject targetScreen)
    {
        if (targetScreen == null || targetScreen == currentScreen) return;

        if (currentScreen != null)
        {
            screenStack.Push(currentScreen);
            currentScreen.SetActive(false);
        }

        currentScreen = targetScreen;
        currentScreen.SetActive(true);

        UpdateHUDVisibility();

        Debug.Log($"[화면 이동] ➡ {currentScreen.name} | 스택: {screenStack.Count}");
    }

    public void GoBack()
    {
        if (screenStack.Count == 0)
        {
            Debug.LogWarning("[뒤로가기] 스택이 비어 있음.");
            return;
        }

        if (currentScreen != null)
            currentScreen.SetActive(false);

        currentScreen = screenStack.Pop();
        currentScreen.SetActive(true);

        UpdateHUDVisibility();

        Debug.Log($"[뒤로가기] ↩ {currentScreen.name}");
    }

    private void UpdateHUDVisibility()
    {
        bool isMainLobby = (currentScreen == mainLobbyScreen);

        profileUI.SetActive(isMainLobby);
        backButtonUI.SetActive(!isMainLobby);
        // currencyUI는 항상 켜져 있으므로 건드리지 않음
    }
}