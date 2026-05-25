using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public bool isAI = false; // 플레이어인지 AI인지 구별용
    
    [Header("[ Character Stats (GDD 1.0) ]")]
    public int maxHP;
    public int currentHP;
    public int baseATK;

    // 현재 가지고 있는 손패 리스트
    public List<TileData> handList = new List<TileData>();

    private void Awake()
    {
        // GDD 1.0 밸런스 데이터에 맞추어 랜덤 스탯 초기화
        maxHP = Random.Range(10000, 15001); // 10,000 ~ 15,000
        currentHP = maxHP;
        baseATK = Random.Range(500, 1501);  // 500 ~ 1,500
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        Debug.Log($"<color=orange>[피격]</color> {gameObject.name}이(가) <b>{damage}</b>의 데미지를 입었습니다. (남은 HP: {currentHP}/{maxHP})");
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
        return targetTile;
    }
}