using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    // 어디서나 쉽게 ScreenManager를 부를 수 있도록 싱글톤 세팅
    public static ScreenManager Instance { get; private set; }

    [Header("[ 관리할 화면 오브젝트 리스트 ]")]
    public GameObject screenMainLobby;  // 메인 로비
    public GameObject screenBattleMenu;  // 대전 메뉴
    public GameObject screenArenaList;   // 아레나 목록
    public GameObject screenArenaDetail; // 아레나 세부
    public GameObject screenTeamEdit;    // 편성 기본
    public GameObject screenTowerList;   // 몽환의 탑 목록
    public GameObject screenTowerDetail; // 몽환의 탑 세부
    public GameObject screenInGame;      // 인게임 배틀 필드

    // 🛡️ 뒤로가기 동작의 핵심: 지난 화면들을 순서대로 기억하는 자료구조 (Stack)
    private Stack<GameObject> screenStack = new Stack<GameObject>();
    
    // 현재 화면 추적용
    private GameObject currentScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 게임 시작 시 모든 화면을 일단 다 끄고, 메인 로비만 첫 화면으로 띄우기
        InitScreens();
        OpenScreen(screenMainLobby, isRoot: true);
    }

    // ----------------------------------------------------
    // [기능 1] 새로운 화면 열기 (기존 화면은 스택에 저장)
    // ----------------------------------------------------
    public void PushScreen(GameObject nextScreen)
    {
        if (nextScreen == null || nextScreen == currentScreen) return;

        // 현재 보던 화면이 있다면 히스토리(스택)에 저장하고 비활성화
        if (currentScreen != null)
        {
            screenStack.Push(currentScreen);
            currentScreen.SetActive(false);
        }

        // 새 화면 켜기
        currentScreen = nextScreen;
        currentScreen.SetActive(true);

        Debug.Log($"<color=lime>[화면 이동]</color> ➡️ <b>{currentScreen.name}</b> 진입. (남은 스택 수: {screenStack.Count})");
    }

    // ----------------------------------------------------
    // [기능 2] 잃어버린 기억 찾기: 뒤로가기 동작 (GoBack)
    // ----------------------------------------------------
    public void GoBack()
    {
        // 되돌아갈 이전 화면이 스택에 없다면 리턴 (예: 메인 로비가 최하단일 때)
        if (screenStack.Count == 0)
        {
            Debug.Log("<color=yellow>[뒤로가기 방어]</color> 더 이상 되돌아갈 이전 화면이 없습니다.");
            return;
        }

        // 현재 화면 끄기
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        // 스택의 맨 위에서 이전 화면을 꺼내와서(Pop) 다시 켜기
        currentScreen = screenStack.Pop();
        currentScreen.SetActive(true);

        Debug.Log($"<color=orange>[뒤로가기]</color> ↩️ <b>{currentScreen.name}</b> 리턴. (남은 스택 수: {screenStack.Count})");
    }

    // ----------------------------------------------------
    // [헬퍼] 초기화 및 화면 다이렉트 오픈 함수
    // ----------------------------------------------------
    private void OpenScreen(GameObject targetScreen, bool isRoot = false)
    {
        if (isRoot)
        {
            screenStack.Clear(); // 루트 화면일 땐 히스토리 초기화
        }

        if (currentScreen != null) currentScreen.SetActive(false);
        
        currentScreen = targetScreen;
        if (currentScreen != null) currentScreen.SetActive(true);
    }

    private void InitScreens()
    {
        if (screenMainLobby) screenMainLobby.SetActive(false);
        if (screenBattleMenu) screenBattleMenu.SetActive(false);
        if (screenArenaList) screenArenaList.SetActive(false);
        if (screenArenaDetail) screenArenaDetail.SetActive(false);
        if (screenTeamEdit) screenTeamEdit.SetActive(false);
        if (screenTowerList) screenTowerList.SetActive(false);
        if (screenTowerDetail) screenTowerDetail.SetActive(false);
        if (screenInGame) screenInGame.SetActive(false);
    }
}