using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // 싱글톤으로 설계하여 어디서나 쉽게 전투 함수를 호출할 수 있도록 함
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ----------------------------------------------------
    // [메인 로직] 결상 성공 시 데미지를 계산하고 적용하는 함수
    // ----------------------------------------------------
    public void ProcessBattle(PlayerHand attacker, PlayerHand defender, float multiplier, List<string> yaku)
    {
        Debug.Log($"<color=red><b>⚔️ [전투 발생] {attacker.gameObject.name}의 결상 공격! ⚔️</b></color>");

        // 1. 활성화된 모든 족보 출력
        foreach (string y in yaku)
        {
            Debug.Log($"<color=pink> 활성화된 족보: {y}</color>");
        }

        // 2. 데미지 연산 산식 적용: Base ATK * 족보 배수
        int finalDamage = Mathf.RoundToInt(attacker.baseATK * multiplier);

        // GDD 1.0 보정치 한계 락 (최소 500 ~ 최대 7500)
        finalDamage = Mathf.Clamp(finalDamage, 500, 7500);

        Debug.Log($"<color=yellow>[산식 계산]</color> {attacker.gameObject.name}의 Base ATK({attacker.baseATK}) x 족보 배수({multiplier:F2}배) = <b>최종 피해량: {finalDamage}</b>");

        // 3. 방어자 체력 차감 및 로그 출력
        defender.TakeDamage(finalDamage);

        Debug.Log($"<color=orange><b>💥 {attacker.gameObject.name}의 결상 발동! {finalDamage}의 피해를 입혀 {defender.gameObject.name}의 남은 명운: {defender.currentHP}/{defender.maxHP}</b></color>");
    }
}