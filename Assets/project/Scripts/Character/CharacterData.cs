using UnityEngine;

// 네임스페이스나 다른 스크립트와의 충돌을 방지하기 위해 
// 클래스 구조를 CharacterData 내부나 별도 규격으로 래핑함
[System.Serializable]
public class CharacterSkillInfo
{
    public string skillName;
    [TextArea(2, 5)] public string description;
}

[System.Serializable]
public class CharacterSignatureYakuInfo
{
    public string yakuName;
    public float bonusMultiplier;
}

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObjects/CharacterData", order = 2)]
public class CharacterData : ScriptableObject
{
    [Header("[ 기본 식별 정보 ]")]
    public string characterID;       // 캐릭터 고유 ID (예: CH_001)
    public string characterName;     // 캐릭터 이름
    public global::ElementType houseElement; // ⚠️ 기존 프로젝트의 파일명/Enum명에 맞게 매칭 (예: ElementType 또는 Element)

    [Header("[ GDD 1.0 기본 스탯 ]")]
    public int baseATK;              // 기본 공격력
    public int maxHp;                // 최대 체력

    [Header("[ 특수 고유 에셋 연동 ]")]
    public CharacterSkillInfo characterSkill;           // 고유 스킬 데이터
    public CharacterSignatureYakuInfo signatureYaku;    // 시그니처 족보
}