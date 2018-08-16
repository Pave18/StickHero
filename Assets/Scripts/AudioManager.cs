using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private Sound[] sounds;


    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }


    private void Start()
    {
        for (var i = 0; i < sounds.Length; i++)
        {
            var tempGO = new GameObject("Sound_" + i + "_" + sounds[i].name);
            tempGO.transform.SetParent(this.transform);
            sounds[i].SetSource(tempGO.AddComponent<AudioSource>());
        }
    }


    private void Update()
    {
        Mute();
    }


    public void PlaySound(string name)
    {
        FindSoundByName(name).Play();
    }


    public void StopSound(string name)
    {
        FindSoundByName(name).Stop();
    }


    private void Mute()
    {
        bool mute = PlayerPrefs.GetString(GameArrangement.PLAYER_PREFS_MUSIC) == GameArrangement.PLAYER_PREFS_OFF;

        foreach (var s in sounds)
        {
            s.GetSource().mute = mute;
        }
    }
    

    private Sound FindSoundByName(string name)
    {
        foreach (var s in sounds)
        {
            if (s.name == name)
            {
                return s;
            }
        }

        Debug.LogError("AudioManager: Sound not found, name = " + name);
        return null;
    }
}