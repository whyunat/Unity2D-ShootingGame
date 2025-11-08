using Singleton.Component;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillManager : SingletonComponent<SkillManager>
{
    [SerializeField] private Player player;
    private Skill[] skills = new Skill[5];
    public Skill[] Skills { get => skills; set => skills = value; }

    #region Singleton
    protected override void AwakeInstance()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ReleaseSingleton();
        }
    }

    public bool TryLevelUpSkill(Skill skill)
    {
        if (skill.CanLevelUp(GameManager.Instance.Experience))//레벨업이 가능하면
        {
            GameManager.Instance.UpdateExp(-skill.RequiredExp()); //경험치 차감
            skill.LevelUp(); //레벨업
            ApplySkillEffect(skill); //효과 적용
            return true; //true 반환
        }
        return false;//레벨업이 안되면 false반환
    }

    private void ApplySkillEffect(Skill skill)
    {
        //일단 하드코딩으로 구현
        //리터럴은 초기에 설정해둔 값.
        switch (skill.SkillName)
        {
            case "AttackPower"://공격력
                player.atkPower = 2 + skill.Level;
                if (skill.hasPerk) player.knockbackPower += 2; // 특전: 넉백 증가
                break;

            case "MoveSpeed"://이동속도
                player.moveSpeed = 5f * (1f + skill.Level * 0.1f);
                if (skill.hasPerk) player.evasionChance = 30f; // 특전: 회피 확률 증가
                break;

            case "AttackSpeed"://공격속도
                player.atkSpeed = 1f * (1f - skill.Level * 0.1f);
                // 특전: 투사체 추가
                break;

            case "ProjectileSpeed"://투사체 속도
                player.projectileSpeed = 10f * (1f + skill.Level * 0.3f);
                if (skill.hasPerk) player.projectilePenetration += 2; // 특전: 투사체 관통 증가
                break;

            case "MaxHealthIncrease"://최대체력 증가
                player.SetMaxHealth(5 + skill.Level * 5);
                // 특전: 주변 데미지
                break;
        }
    }

    public void ResetAllSkills()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].ResetLevel();
        }
    }
}
