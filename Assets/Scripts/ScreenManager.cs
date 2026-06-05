using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Header("Screens")]
    public List<GameObject> allScreens = new List<GameObject>();
    public GameObject titleScreen;      // 💡 새로 추가: 타이틀 화면
    public GameObject mainLobbyScreen; 
    public GameObject inGameScreen; 

    [Header("HUD Elements")]
    public GameObject profileUI;
    public GameObject currencyUI;   
    public GameObject backButtonUI;

    private Stack<GameObject> screenStack = new Stack<GameObject>();
    public GameObject currentScreen { get; private set; } // 💡 캡슐화 유지하면서 외부에서 읽을 수 있게 변경

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

        // 💡 시작할 때 로비가 아니라 타이틀 화면 띄우기
        if (titleScreen != null)
            OpenScreen(titleScreen);
    }

    // 💡 1. 유니티 버튼(Inspector)에서 연결할 기본 함수 (파라미터 1개 유지!)
    public void OpenScreen(GameObject targetScreen)
    {
        MoveScreenLogic(targetScreen, false);
    }

    // 💡 2. 타이틀 화면에서 로그인 완료 후 코드로 호출할 함수 (스택 비우기용)
    public void OpenRootScreen(GameObject targetScreen)
    {
        MoveScreenLogic(targetScreen, true);
    }

    // 💡 3. 실제 화면 이동 로직 (외부에서 직접 안 부름)
    private void MoveScreenLogic(GameObject targetScreen, bool isRoot)
    {
        if (targetScreen == null || targetScreen == currentScreen) return;

        if (isRoot)
        {
            screenStack.Clear(); // 스택 싹 비우기
        }
        else if (currentScreen != null && currentScreen != titleScreen) 
        {
            // 타이틀 화면은 뒤로가기로 돌아갈 곳이 아니므로 스택에 안 넣음
            screenStack.Push(currentScreen);
        }

        if (currentScreen != null)
            currentScreen.SetActive(false);

        currentScreen = targetScreen;
        currentScreen.SetActive(true);

        UpdateHUDVisibility();

        Debug.Log($"[화면 이동] ➡ {currentScreen.name} | 스택: {screenStack.Count}");
    }

    // 💡 isRoot 플래그 추가: true면 뒤로가기 스택을 비워버림 (타이틀 -> 로비로 갈 때 사용)
    public void OpenScreen(GameObject targetScreen, bool isRoot = false)
    {
        if (targetScreen == null || targetScreen == currentScreen) return;

        if (isRoot)
        {
            screenStack.Clear(); // 스택 싹 비우기
        }
        else if (currentScreen != null && currentScreen != titleScreen) 
        {
            // 타이틀 화면은 뒤로가기로 돌아갈 곳이 아니므로 스택에 안 넣음
            screenStack.Push(currentScreen);
        }

        if (currentScreen != null)
            currentScreen.SetActive(false);

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
        bool isTitle = (currentScreen == titleScreen);
        bool isMainLobby = (currentScreen == mainLobbyScreen);
        bool isInGame = (currentScreen == inGameScreen);

        // 💡 타이틀 화면이거나 인게임이면 HUD 전체 숨김!
        if (isTitle || isInGame)
        {
            profileUI.SetActive(false);
            currencyUI.SetActive(false);
            backButtonUI.SetActive(false);
        }
        else
        {
            profileUI.SetActive(isMainLobby);
            backButtonUI.SetActive(!isMainLobby); 
            currencyUI.SetActive(true);           
        }
    }
}