using System.Collections;
using UnityEngine;
using TMPro;

public class FriendlyMatchManager : MonoBehaviour
{
    [Header("[ 1. 참여 코드 팝업 ]")]
    public GameObject joinCodePopup;           // 참여 코드 입력 팝업창
    public TMP_InputField codeInputField;      // 코드 입력칸
    public GameObject friendlyRoomScreen;      // 이동할 대기실 화면 (Screen_FriendlyRoom)

    [Header("[ 2. 대기실 시작 버튼 ]")]
    public GameObject startButton;             // 대기실의 중앙 시작하기 버튼
    
    [Header("[ 3. 캐릭터 변경 팝업 ]")]
    public GameObject changeCharPopup;         // 캐릭터 변경 확인 팝업창
    public GameObject friendCharSelectScreen;  // 이동할 캐릭터 선택 화면 (Screen_FriendCharSelect)

    private void Start()
    {
        if (startButton != null) startButton.SetActive(false);
    }

    // --------------------------------------------------
    // 1. 방 참여 팝업 기능
    // --------------------------------------------------
    public void OpenJoinPopup() { joinCodePopup.SetActive(true); }
    public void CloseJoinPopup() { joinCodePopup.SetActive(false); }

    public void EnterRoom()
    {
        if (string.IsNullOrEmpty(codeInputField.text)) return; 
        
        joinCodePopup.SetActive(false);
        // ✨ 네 원본 코드에 맞춰서 OpenScreen으로 변경!
        ScreenManager.Instance.OpenScreen(friendlyRoomScreen);
        
        SimulatePlayerEnter(); 
    }

    // --------------------------------------------------
    // 2. 10초 대기 후 시작 버튼 활성화
    // --------------------------------------------------
    public void SimulatePlayerEnter()
    {
        StartCoroutine(WaitAndShowStartButton());
    }

    private IEnumerator WaitAndShowStartButton()
    {
        startButton.SetActive(false);
        yield return new WaitForSeconds(10f);
        startButton.SetActive(true);          
    }

    // --------------------------------------------------
    // 3. 캐릭터 변경 화면 이동 및 팝업
    // --------------------------------------------------
    public void GoToCharSelectScreen()
    {
        // ✨ 네 원본 코드에 맞춰서 OpenScreen으로 변경!
        ScreenManager.Instance.OpenScreen(friendCharSelectScreen);
    }

    public void OpenChangePopup()
    {
        changeCharPopup.SetActive(true);
    }

    public void CloseChangePopup()
    {
        changeCharPopup.SetActive(false);
    }

    public void ConfirmCharacterChange()
    {
        changeCharPopup.SetActive(false);
        ScreenManager.Instance.GoBack(); 
    }
}