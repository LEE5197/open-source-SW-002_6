using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private int maxChannels = 10; // 동시에 날 수 있는 최대 효과음 개수
    private AudioSource[] sfxChannels;
    private int currentChannelIndex = 0;

    [Header("Audio Clips")]
    public AudioClip BasicBGMClip;
    public AudioClip UISoundClip;
    public AudioClip PlayerShootClip;
    public AudioClip enemyShootClip;
    public AudioClip PlayerUltClip;
    public AudioClip ItemClip;
    public AudioClip BossEntryClip;
    public AudioClip BossBGMClip;
    public AudioClip GameOverClip;
    public AudioClip GameClearClip;

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
    }

    // 외부에서 소리를 재생할 때 부르는 함수
    public void Play(AudioClip clip)
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
