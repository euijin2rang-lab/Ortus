using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log("=== 클릭 감지 ===");

            foreach (var r in results)
            {
                Debug.Log(r.gameObject.name);
            }
        }
    }
}