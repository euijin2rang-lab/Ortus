using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public bool isAI = false; 
    
    [Header("[ 연동된 캐릭터 에셋 ]")]
    public CharacterData characterData; 

    [Header("[ Character Stats ]")]
    public int maxHP;
    public int currentHP;
    public int baseATK;

    public List<TileData> handList = new List<TileData>();

    private GimunManager _gimunManager;
    
    // 🛡️ [핵심 안전장치] 외부에서 찌르면 없어도 그 즉시 만들어서 반환하는 뚫리지 않는 방패
    public GimunManager gimunManager
    {
        get
        {
            if (_gimunManager == null)
            {
                _gimunManager = GetComponent<GimunManager>();
                if (_gimunManager == null)
                {
                    _gimunManager = gameObject.AddComponent<GimunManager>();
                }
                _gimunManager.Setup(this);
            }
            return _gimunManager;
        }
    }

    public void InitializeCharacter(CharacterData data)
    {
        characterData = data;
        
        // 게이지 초기화 강제 보장
        _gimunManager = gimunManager; 

        if (characterData != null)
        {
            maxHP = characterData.maxHp;
            currentHP = maxHP;
            baseATK = characterData.baseATK;
            Debug.Log($"<color=yellow>[캐릭터 안착]</color> {gameObject.name}: {characterData.characterName} (HP:{maxHP}/ATK:{baseATK})");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        Debug.Log($"<color=orange>[피격]</color> {gameObject.name}이(가) {damage}의 데미지를 입었습니다. (남은 HP: {currentHP}/{maxHP})");
    }

    public void AddTile(TileData tile) 
    { 
        if (handList.Count >= 8) return; 
        handList.Add(tile); 
    }

    public TileData RemoveTile(int index)
    {
        if (index < 0 || index >= handList.Count) return null;
        TileData targetTile = handList[index];
        handList.RemoveAt(index);

        if (gimunManager != null) gimunManager.AddGauge(targetTile.element);

        return targetTile;
    }
}