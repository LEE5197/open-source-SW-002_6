using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private int maxChannels = 10; // 동시에 날 수 있는 최대 효과음 개수
    private AudioSource[] sfxChannels;
    private int currentChannelIndex = 0;

    private AudioSource BGMChannel;

    [Header("Audio Clips")]
    public AudioClip BasicBGMClip;
    public AudioClip UISoundClip;
    public AudioClip GameOverClip;
    public AudioClip GameClearClip;
    public AudioClip BossEntryClip;

    //싱글톤 패턴
    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        //오디오 소스를 원하는 개수만큼.
        sfxChannels = new AudioSource[maxChannels];
        for (int i = 0; i < maxChannels; i++)
        {
            sfxChannels[i] = gameObject.AddComponent<AudioSource>();
            sfxChannels[i] = gameObject.AddComponent<AudioSource>();
            sfxChannels[i].playOnAwake = false;
            sfxChannels[i].spatialBlend = 0f;
        }

        if (BGMChannel == null) BGMChannel = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (IsBGMOn == false)
        {
            AudioClip clip = BasicBGMClip;
            PlayBGM(clip);
            IsBGMOn = true;
        }
    }

    public bool IsBGMOn = false;
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        BGMChannel.clip = clip;
        BGMChannel.loop = true;
        BGMChannel.Play();
    }

    public void PlayBossBGM(AudioClip clip)
    {
        if (clip == null) return;

        StartCoroutine(BossSoundSequenceRoutine(BossEntryClip, clip));
    }
    private IEnumerator BossSoundSequenceRoutine(AudioClip entryClip, AudioClip bgmClip)
    {
        if (entryClip == null || bgmClip == null) yield break;

        // 1. 보스 엔트리 사운드 세팅 및 재생
        BGMChannel.clip = entryClip;
        BGMChannel.loop = false; //엔트리 연출 소리는 반복되면 안 되므로 false!
        BGMChannel.Play();

        // 2. 엔트리 사운드의 길이(초)만큼 정확하게 대기합니다.
        yield return new WaitForSeconds(entryClip.length);

        // 3. 엔트리 소리가 끝나면 바로 보스 본 BGM으로 교체 후 무한 반복 재생
        BGMChannel.clip = bgmClip;
        BGMChannel.loop = true;  //보스 음악은 계속 돌아야 하므로 true
        BGMChannel.Play();
    }

    // 외부에서 효과음을 재생할 때 부르는 함수
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;

        // 순환 큐(Circular Queue) 방식으로 다음 채널을 선택해 재생
        AudioSource currentChannel = sfxChannels[currentChannelIndex];
        currentChannel.clip = clip;
        currentChannel.Play();

        // 인덱스를 넘겨서 다음 소리 준비 (오버플로우 방지)
        currentChannelIndex = (currentChannelIndex + 1) % maxChannels;
    }
}
