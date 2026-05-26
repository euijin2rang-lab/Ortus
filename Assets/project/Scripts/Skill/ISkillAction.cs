using System.Collections;

public interface FileSkillAction
{
    string SkillName { get; }
    PlayerHand Caster { get; }
    PlayerHand Target { get; }
    IEnumerator ExecuteRoutine(); // 코루틴을 지원하여 연출 시간 확보 가능
}