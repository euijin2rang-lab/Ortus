using UnityEngine;

// 속성을 표현할 열거형(Enum)
public enum ElementType
{
    Fire,   // 화
    Earth,  // 지
    Air,    // 공
    Water   // 수
}

[CreateAssetMenu(fileName = "NewTileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public ElementType element; // 원소 속성
    public int value;           // 숫자 (1 ~ 7)
    public string tileName;     // 타일 이름 (예: 화-7, 수-3)
    public Sprite tileImage;    // 나중에 UI에 띄울 이미지 (임시로 비워둬도 됨)
}