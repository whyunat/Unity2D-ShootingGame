using Singleton.Component;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonComponent<AudioManager>
{
    #region Singleton
    protected override void AwakeInstance()
    {
        Init();
    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {

    }
    #endregion

    //public static AudioManager instance;


    [Header("#BGM")]
    public AudioClip[] bgmClips;
    [SerializeField] private float[] bgmVolumes; // 각 BGM별 볼륨 배열
    private AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    [SerializeField] private float[] sfxVolumes; // 각 SFX별 볼륨 배열
    public int channels;
    private AudioSource[] sfxPlayers;
    private int channelIndex;

    public enum Bgm { Stage1, Stage2, Stage3, Stage4, Stage5 }
    public enum Sfx { PlayerThrow, Hit, ShoomDie, FireDragonDie, BossThrow }
    

    //private void Awake()
    //{
    //    instance = this;
    //    Init();
    //}

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        // BGM 배열 크기 확인 및 초기화
        if (bgmVolumes == null || bgmVolumes.Length != (int)Bgm.Stage5 + 1)
        {
            bgmVolumes = new float[(int)Bgm.Stage5 + 1];
            for (int i = 0; i < bgmVolumes.Length; i++)
            {
                bgmVolumes[i] = 1f; // 기본값 1로 초기화
            }
        }

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }

        // SFX 배열 크기 확인 및 초기화
        if (sfxVolumes == null || sfxVolumes.Length != (int)Sfx.BossThrow + 1)
        {
            sfxVolumes = new float[(int)Sfx.BossThrow + 1];
            for (int i = 0; i < sfxVolumes.Length; i++)
            {
                sfxVolumes[i] = 1f; // 기본값 1로 초기화
            }
        }

        
    }
    public void BgmController(int stage)
    {
       
        switch (stage)
        {
            case 1:
                PlayBgm(Bgm.Stage1);
                break;
            case 2:
                PlayBgm(Bgm.Stage2);
                break;
            case 3:
                PlayBgm(Bgm.Stage3);
                break;
            case 4:
                PlayBgm(Bgm.Stage4);
                break;
            case 5:
                PlayBgm(Bgm.Stage5);
                break;
            default:
                Debug.LogWarning($"알 수 없는 스테이지: {stage}. BGM 재생 안 함");
                bgmPlayer.Stop();
                break;
        }
    }


    public void PlayBgm(Bgm bgm)
    {
        int bgmIndex = (int)bgm;

        // bgmClips와 bgmVolumes 배열 범위 체크
        if (bgmIndex >= bgmClips.Length || bgmClips[bgmIndex] == null)
        {
            Debug.LogError($"BGM 클립이 설정되지 않았습니다: {bgm}");
            return;
        }
        if (bgmIndex >= bgmVolumes.Length)
        {
            Debug.LogError($"BGM 볼륨이 설정되지 않았습니다: {bgm}");
            return;
        }

        bgmPlayer.Stop(); // 기존 BGM 정지
        bgmPlayer.clip = bgmClips[bgmIndex];
        bgmPlayer.volume = bgmVolumes[bgmIndex]; // 개별 볼륨 적용
        bgmPlayer.Play();
        Debug.Log($"BGM 재생: {bgm}");
    }

    public void PlaySfx(Sfx sfx)
    {
        int sfxIndex = (int)sfx;

        // sfxClips와 sfxVolumes 배열 범위 체크
        if (sfxIndex >= sfxClips.Length || sfxClips[sfxIndex] == null)
        {
            Debug.LogError($"SFX 클립이 설정되지 않았습니다: {sfx}");
            return;
        }
        if (sfxIndex >= sfxVolumes.Length)
        {
            Debug.LogError($"SFX 볼륨이 설정되지 않았습니다: {sfx}");
            return;
        }

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[sfxIndex];
            sfxPlayers[loopIndex].volume = sfxVolumes[sfxIndex]; // 개별 볼륨 적용
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}