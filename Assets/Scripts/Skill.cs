using UnityEngine;
public class Skill
{
    public string SkillName { get; private set; }
    public int Level { get; private set; }
    public int MaxLevel { get; private set; }
    public bool hasPerk => Level == MaxLevel; // 최대 레벨떄 특전(퍽) 활성화
    public int[] expThresholds = { 100, 300, 500 };//스킬 레벨업 경험치 임계값(필요 경험치)

    public Skill(string skillName, int maxLevel)
    {
        SkillName = skillName;
        Level = 0;//처음엔 0레벨이고, 스킬 업을하면 1레벨, 2레벨, 3레벨 순서로 가야합니다.
        MaxLevel = maxLevel;//따라서 MaxLevel은 -1할필요 없어요
    }
    //max 레벨이 아니고, 경험치가 충분하면 레벨업 가능
    public bool CanLevelUp(int experience)
    {
        if (Level >= MaxLevel) return false; // MaxLevel이면 레벨업 불가
        return experience >= expThresholds[Level];
    }

    public void LevelUp()
    {//0 1 2 (1레벨, 2레벨 3레벨)
        if (Level < MaxLevel)
        {
            Level++;
            Debug.Log($"{SkillName} - {Level}");
        }
    }

    public int RequiredExp()
    {
        return expThresholds[Level];
    }

    public void ResetLevel()
    {
        Level = 0;
    }
}
