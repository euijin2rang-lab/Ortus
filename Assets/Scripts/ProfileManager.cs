using UnityEngine;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    [Header("[ 한마디 팝업 설정 ]")]
    public GameObject commentPopup;          // 팝업창 전체 (ChangeCommentPopup)
    public TMP_InputField commentInputField; // 글씨 입력하는 칸
    public TMP_Text profileCommentText;      // 프로필 화면에 보이는 진짜 한마디 텍스트

    [Header("[ 캐릭터 변경 팝업 설정 ]")]
    public GameObject confirmPopup;          // "설정하시겠습니까?" 팝업창 전체

    // --------------------------------------------------
    // 1. 한마디 팝업 관련 함수
    // --------------------------------------------------
    public void OpenCommentPopup()
    {
        // 팝업 열 때 기존에 적혀있던 한마디를 입력칸에 그대로 불러옴
        if(profileCommentText != null && commentInputField != null)
            commentInputField.text = profileCommentText.text; 
            
        commentPopup.SetActive(true);
    }

    public void ConfirmComment()
    {
        // 확인 누르면 입력칸의 글씨를 프로필 텍스트에 덮어씌우고 팝업 끔
        if(profileCommentText != null && commentInputField != null)
            profileCommentText.text = commentInputField.text;
            
        commentPopup.SetActive(false);
    }

    public void CloseCommentPopup()
    {
        commentPopup.SetActive(false);
    }

   // --------------------------------------------------
    // 2. 찐 공유 기능 (Native Share 적용)
    // --------------------------------------------------
    public void ShareProfile()
    {
        // 1. 공유할 텍스트 만들기
        string shareText = $"[내 게임 이름] {profileCommentText.text} - 내 프로필 구경오세요!";

        // 2. Native Share 에셋을 사용해서 폰의 진짜 공유 창 띄우기!
        new NativeShare()
            .SetText(shareText)
            .Share();

        Debug.Log("스마트폰 공유 창 호출 완료!");
    }
    
    // --------------------------------------------------
    // 3. 캐릭터 설정 팝업 관련 함수
    // --------------------------------------------------
    public void OpenConfirmPopup()
    {
        confirmPopup.SetActive(true);
    }

    public void CloseConfirmPopup()
    {
        confirmPopup.SetActive(false);
    }

    public void ApplyCharacterChange()
    {
        // 진짜 캐릭터가 바뀌는 로직이 들어갈 곳 (이미지 교체 등)
        Debug.Log("아레나 캐릭터가 변경되었습니다!");
        confirmPopup.SetActive(false);
    }

    // ProfileManager.cs 스크립트 안에 이 함수를 추가해!

    [Header("장비 목록 화면")]
    public GameObject equipListScreen; // 인스펙터에서 Screen_EquipDetail 또 연결해주면 됨

    public void OnProfileEquipChangeClick()
    {
        // 1. 타겟을 "프로필 아레나 캐릭터"로 지정!
        EquipmentManager.Instance.currentTargetCharId = "Arena_Defense_Char";
        
        // 2. 똑같은 장비 목록 창 열기
        ScreenManager.Instance.OpenScreen(equipListScreen);
    }
}