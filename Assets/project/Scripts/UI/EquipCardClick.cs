using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EquipCardClick : MonoBehaviour
{
    public string myEquipId = "Sword_001"; // 나중에 데이터랑 연동할 땐 동적으로 할당해야 함

    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => 
        {
            if (EquipmentManager.Instance != null)
            {
                // 매니저한테 "나 이거 장착할래!" 하고 던짐
                EquipmentManager.Instance.EquipItemToCharacter(myEquipId);
            }
        });
    }
}