using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("[ References ]")]
    public PlayerHand playerHand;
    public PlayerHand aiHand;

    [Header("[ Deck Pool ]")]
    public List<TileData> masterDeck = new List<TileData>(); // 112장 원본 덱

    private void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        DistributeInitialHands();
    }

    // 1. 4속성 x 7숫자 x 각 4장씩 = 총 112장 타일 생성
    private void GenerateDeck()
    {
        masterDeck.Clear();

        // 4가지 속성을 순회
        System.Array elements = System.Enum.GetValues(typeof(ElementType));
        foreach (ElementType el in elements)
        {
            // 1부터 7까지의 숫자
            for (int val = 1; val <= 7; val++)
            {
                // 똑같은 카드를 4장씩 생성
                for (int count = 0; count < 4; count++)
                {
                    // 런타임에 가상으로 ScriptableObject 데이터 인스턴스를 동적 생성
                    TileData newTile = ScriptableObject.CreateInstance<TileData>();
                    newTile.element = el;
                    newTile.value = val;
                    newTile.tileName = $"{el}_{val}";

                    masterDeck.Add(newTile);
                }
            }
        }
        Debug.Log($"<color=green>[덱 생성 완료]</color> 총 {masterDeck.Count}장의 타일이 준비되었습니다.");
    }

    // 2. Fisher-Yates 셔플 알고리즘으로 무작위 섞기
    private void ShuffleDeck()
    {
        for (int i = masterDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            
            // 값 스왑(Swap)
            TileData temp = masterDeck[i];
            masterDeck[i] = masterDeck[randomIndex];
            masterDeck[randomIndex] = temp;
        }
        Debug.Log("<color=yellow>[덱 셔플 완료]</color> 112장의 타일을 무작위로 섞었습니다.");
    }

    // 3. 시작 시 7장씩 패 분배하기
    private void DistributeInitialHands()
    {
        if (playerHand == null || aiHand == null)
        {
            Debug.LogError("PlayerHand 또는 AIHand 레퍼런스가 비어있습니다!");
            return;
        }

        // 플레이어에게 7장 분배
        for (int i = 0; i < 7; i++)
        {
            DrawCardTo(playerHand);
        }

        // AI에게 7장 분배
        for (int i = 0; i < 7; i++)
        {
            DrawCardTo(aiHand);
        }

        Debug.Log("<color=cyan>[초기 분배 완료]</color> 플레이어와 AI에게 각각 7장의 카드를 나누어주었습니다.");
    }

    // 덱에서 맨 위의 카드를 뽑아 특정 손패로 넘겨주는 공용 메서드 (나중에 TurnManager가 호출 가능)
    public void DrawCardTo(PlayerHand targetHand)
    {
        if (masterDeck.Count <= 0)
        {
            Debug.LogError("덱에 남은 카드가 없습니다!");
            return;
        }

        // 맨 앞(0번 인덱스) 카드를 뽑음
        TileData topTile = masterDeck[0];
        masterDeck.RemoveAt(0);

        // 대상 손패에 추가
        targetHand.AddTile(topTile);
    }
}