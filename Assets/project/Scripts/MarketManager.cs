using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 임시 아이템 데이터 구조체
[System.Serializable]
public class ShopItem
{
    public string itemId;
    public string itemName;
    public int price;
    public Sprite itemIcon;
}

public class MarketManager : MonoBehaviour
{
    [Header("[ Settings ]")]
    public int itemsPerPage = 8; // 4x2 격자니까 1페이지에 8개

    [Header("[ UI References ]")]
    public Transform itemGrid;       // 아이템 Grid
    public GameObject itemPrefab;    // 아이템 프리팹
    public Button btnPrev;           // 왼쪽 화살표
    public Button btnNext;           // 오른쪽 화살표

    [Header("[ Popup References ]")]
    public GameObject purchasePopup;       // 팝업 패널
    public TextMeshProUGUI popupNameText;  // 팝업 아이템 이름
    public TextMeshProUGUI popupPriceText; // 팝업 아이템 가격
    public Image popupIconImage;           // 팝업 아이템 아이콘
    public Button btnConfirmBuy;           // 팝업 안의 구매 버튼
    public Button btnCancelBuy;            // 팝업 안의 취소 버튼

    [Header("[ Dummy Data ]")]
    public List<ShopItem> allItems = new List<ShopItem>(); // 인스펙터에서 임시 데이터 채워넣기

    private int currentPage = 0;
    private ShopItem currentSelectedItem; // 현재 팝업에 띄운 아이템

    private void Start()
    {
        // 버튼 이벤트 연결
        btnPrev.onClick.AddListener(PagePrev);
        btnNext.onClick.AddListener(PageNext);
        btnConfirmBuy.onClick.AddListener(ProcessPayment);
        btnCancelBuy.onClick.AddListener(ClosePopup);

        // 초기 화면 로드
        LoadPage(0);
    }

    private void LoadPage(int pageIndex)
    {
        // 혹시라도 음수가 들어오면 0으로 강제 고정 (철벽 방어!)
        if (pageIndex < 0) pageIndex = 0; 

        currentPage = pageIndex;

        // 기존에 생성된 아이템 UI들 싹 다 지우기
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        // 현재 페이지에 표시할 아이템 인덱스 계산
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, allItems.Count);

        // 프리팹 생성 및 데이터 세팅
        for (int i = startIndex; i < endIndex; i++)
        {
            ShopItem itemData = allItems[i];
            GameObject go = Instantiate(itemPrefab, itemGrid);
            
            // 프리팹 안의 자식 오브젝트들 세팅 (구조에 맞게 이름 수정해서 써)
            go.transform.Find("Text_Name").GetComponent<TextMeshProUGUI>().text = itemData.itemName;
            go.transform.Find("Text_Price").GetComponent<TextMeshProUGUI>().text = itemData.price.ToString();
            // go.transform.Find("Image_Icon").GetComponent<Image>().sprite = itemData.itemIcon;

            // 버튼 클릭 시 팝업 띄우는 이벤트 연결
            go.GetComponent<Button>().onClick.AddListener(() => OpenPopup(itemData));
        }

        // 화살표 활성화/비활성화 처리
        btnPrev.interactable = (currentPage > 0);
        btnNext.interactable = (endIndex < allItems.Count);
    }

    public void PageNext()
    {
        // 아이템이 하나도 없으면 넘어가기 방지
        if (allItems.Count == 0) return;

        // 최대 페이지 번호 계산
        int maxPage = (allItems.Count - 1) / itemsPerPage;
        
        // 현재 페이지가 최대 페이지보다 작을 때만 넘어가기!
        if (currentPage < maxPage)
        {
            LoadPage(currentPage + 1);
        }
    }

    public void PagePrev()
    {
        // 현재 페이지가 0(첫 페이지)보다 클 때만 뒤로 가기!
        if (currentPage > 0)
        {
            LoadPage(currentPage - 1);
        }
    }

    // 아이템 클릭 시 팝업 열기
    private void OpenPopup(ShopItem item)
    {
        currentSelectedItem = item; // 선택한 아이템 기억해두기
        
        popupNameText.text = item.itemName;
        popupPriceText.text = $"{item.price} 다이아";
        // popupIconImage.sprite = item.itemIcon; // 아이콘 있으면 주석 해제

        purchasePopup.SetActive(true);
    }

    // 팝업 닫기
    private void ClosePopup()
    {
        purchasePopup.SetActive(false);
    }

    // 결제 연동 로직
    private void ProcessPayment()
    {
        if (currentSelectedItem == null) return;

        Debug.Log($"[{currentSelectedItem.itemName}] 결제 요청 시작! (가격: {currentSelectedItem.price})");

        // 여기서 뒤끝(Backend), 구글 플레이 빌링, 앱스토어 인앱 결제 API를 호출하면 돼!
        // ex) Backend.IAP.SendPurchase(currentSelectedItem.itemId, OnPurchaseSuccess);

        // 임시: 결제 성공했다고 가정하고 팝업 닫기
        Debug.Log("결제 성공 처리 완료!");
        ClosePopup();
    }
}