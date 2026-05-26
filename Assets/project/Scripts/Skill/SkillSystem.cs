using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Instance { get; private set; }

    // 스킬 효과 연산이 순서대로 안전하게 진행되도록 하는 큐
    private Queue<FileSkillAction> skillQueue = new Queue<FileSkillAction>();
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 스킬 트리거 발동 엔드포인트
    public void EnqueueSkill(FileSkillAction action)
    {
        skillQueue.Enqueue(action);
        Debug.Log($"<color=magenta>[스킬 큐 진입]</color> {action.Caster.gameObject.name}의 스킬 <b>[{action.SkillName}]</b> 대기열 등록.");
        
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueueRoutine());
        }
    }

    private IEnumerator ProcessQueueRoutine()
    {
        isProcessing = true;

        while (skillQueue.Count > 0)
        {
            FileSkillAction currentAction = skillQueue.Dequeue();
            Debug.Log($"<color=magenta>[스킬 큐 실행]</color> <b>[{currentAction.SkillName}]</b> 효과 연산 시작.");
            
            // 각 스킬 고유의 연산 루틴 실행 (중간에 딜레이나 연출을 섞을 수 있음)
            yield return StartCoroutine(currentAction.ExecuteRoutine());
            
            yield return new WaitForSeconds(0.5f); // 스킬 간 물리적 간격
        }

        isProcessing = false;
    }
}

// ====================================================
// [커맨드 구현 체] 첫 4인방 고유 스킬 클래스 정의
// ====================================================

// 1. 아리스: 구원의 네펠레 (Cost: 4)
public class Action_Aris : FileSkillAction
{
    public string SkillName => "구원의 네펠레";
    public PlayerHand Caster { get; }
    public PlayerHand Target { get; }
    private TurnManager turnManager;

    public Action_Aris(PlayerHand caster, PlayerHand target, TurnManager manager) { Caster = caster; Target = target; turnManager = manager; }

    public IEnumerator ExecuteRoutine()
    {
        if (Caster.handList.Count > 0)
        {
            // 즉시 패 1장 버리기 (가장 최근에 뽑은 마지막 패)
            int lastIndex = Caster.handList.Count - 1;
            TileData discarded = Caster.RemoveTile(lastIndex);
            turnManager.discardPool.Add(discarded);
            Debug.Log($"[아리스 스킬] 패 [{discarded.tileName}]을 강제 투산했습니다.");

            // 1장 새로 드로우
            turnManager.deckManager.DrawCardTo(Caster);
            TileData drawnTile = Caster.handList[Caster.handList.Count - 1];
            Debug.Log($"[아리스 스킬] 새로운 패 [{drawnTile.tileName}]을 획득했습니다.");

            // 그 패가 [화] 속성일 경우 버프 적용
            if (drawnTile.element == ElementType.Fire)
            {
                Caster.baseATK += 300;
                Debug.Log($"<color=red>[스킬 대박]</color> 화 속성 적중! 이번 매치 타격치 +300 증가 (현재 ATK: {Caster.baseATK})");
            }
        }
        yield return null;
    }
}

// 2. 멜리노에: 스피카의 빛 (Cost: 3)
public class Action_Melinoe : FileSkillAction
{
    public string SkillName => "스피카의 빛";
    public PlayerHand Caster { get; }
    public PlayerHand Target { get; }
    private TurnManager turnManager;

    public Action_Melinoe(PlayerHand caster, PlayerHand target, TurnManager manager) { Caster = caster; Target = target; turnManager = manager; }

    public IEnumerator ExecuteRoutine()
    {
        if (turnManager.discardPool.Count > 0)
        {
            // 버림패 풀에서 마지막 패 1장 소멸
            int lastIndex = turnManager.discardPool.Count - 1;
            TileData targetTile = turnManager.discardPool[lastIndex];
            turnManager.discardPool.RemoveAt(lastIndex);
            
            // 게이지 코스트 1칸 충전 (게이지 20)
            GimunManager gimun = Caster.GetComponent<GimunManager>();
            if (gimun != null) gimun.ForceAddGauge(20);

            Debug.Log($"[멜리노에 스킬] 버림패 [{targetTile.tileName}]을 소멸시키고 기문 게이지 +20을 회복했습니다.");
        }
        else
        {
            Debug.Log("[멜리노에 스킬] 버림패 풀이 비어있어 효과가 무효화되었습니다.");
        }
        yield return null;
    }
}

// 3. 네르: 폴룩스의 거울상 (Cost: 3)
public class Action_Ner : FileSkillAction
{
    public string SkillName => "폴룩스의 거울상";
    public PlayerHand Caster { get; }
    public PlayerHand Target { get; }

    public Action_Ner(PlayerHand caster, PlayerHand target) { Caster = caster; Target = target; }

    public IEnumerator ExecuteRoutine()
    {
        if (Caster.handList.Count >= 2)
        {
            // 가장 왼쪽 패(index 0)와 가장 오른쪽 패(마지막 index)
            TileData leftTile = Caster.handList[0];
            TileData rightTile = Caster.handList[Caster.handList.Count - 1];

            // 런타임 인스턴스 복제를 통해 가장 왼쪽 패의 숫자를 복사
            rightTile.value = leftTile.value;
            rightTile.tileName = $"{rightTile.element}_{rightTile.value}";

            Debug.Log($"<color=cyan>[네르 스킬]</color> 좌측 패({leftTile.tileName})의 숫자를 우측 패로 복제 완료 ➡️ 우측 패 변경됨: [{rightTile.tileName}]");
        }
        yield return null;
    }
}

// 4. 카르키: 알 타르프의 귀환 (Cost: 2)
public class Action_Karki : FileSkillAction
{
    public string SkillName => "알 타르프의 귀환";
    public PlayerHand Caster { get; }
    public PlayerHand Target { get; }
    private TurnManager turnManager;

    public Action_Karki(PlayerHand caster, PlayerHand target, TurnManager manager) { Caster = caster; Target = target; turnManager = manager; }

    public IEnumerator ExecuteRoutine()
    {
        if (turnManager.discardPool.Count > 0 && Caster.handList.Count < 8)
        {
            // 상대가 방금 버린 마지막 패 추출
            int lastIndex = turnManager.discardPool.Count - 1;
            TileData stolenTile = turnManager.discardPool[lastIndex];
            turnManager.discardPool.RemoveAt(lastIndex);

            // 내 손패로 편입
            Caster.AddTile(stolenTile);
            Debug.Log($"<color=blue>[카르키 스킬]</color> 상대가 버린 패 <b>[{stolenTile.tileName}]</b>을 내 손패로 강제 귀환시켰습니다.");
        }
        else
        {
            Debug.Log("[카르키 스킬] 가져올 버림패가 없거나 손패가 가득 차 스킬이 불발되었습니다.");
        }
        yield return null;
    }
}