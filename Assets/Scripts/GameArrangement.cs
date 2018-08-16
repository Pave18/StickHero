using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;


public class GameArrangement : MonoBehaviour
{
    public const string PLAYER_PREFS_MUSIC = "Music", PLAYER_PREFS_ON = "On", PLAYER_PREFS_OFF = "Off";
    const string SOUND_BTN_CLICK = "ButtonClick", SOUND_BONUS = "Bonus", SOUND_WIN = "Win", SOUND_LOSE = "Lose";
    const float SPEED = 5f;
    readonly Vector3 StartPositionMainGameObjs = new Vector3(-2.2f, -1.5f, 0f);

    public delegate void action();
    public delegate void actionWhithBool(bool b);

    public static event action OnStartGame;
    public static event action OnRestartGame;
    public static event action OnLoseGame;
    public static event action OnGetPoin;
    public static event actionWhithBool OnCheckSizeStick;
    
    public GameObject canvasHome, canvesPlay, canvasLost, detectClick, stick, mainGameObjs;
    public Sprite musicOn, musicOff;
    public GameObject musicIcon;

    private bool isStartGame, isStartPosition;
    private AudioManager audioManager;

    
    private void Awake()
    {
        Stick.OnCheckGame += CheckGame;
        StickHero.OnHeroInPosition += CheckWin;
    }

    
    private void Start()
    {
        //Caching
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("GameArrangement: No AudioManager found!");
        }

        SetButnMusic();
    }


    private void Update()
    {
        SetStartPosinionMainGameObjs();
    }


    public void BtnStartGame()
    {
        audioManager.PlaySound(SOUND_BTN_CLICK);

        isStartGame = true;

        canvasHome.SetActive(false);
        canvesPlay.SetActive(true);
        detectClick.SetActive(true);

        if (OnStartGame != null)
        {
            OnStartGame();
        }
    }


    public void BtnRestartGame()
    {
        audioManager.PlaySound(SOUND_BTN_CLICK);

        detectClick.SetActive(true);
        canvasLost.SetActive(false);
        canvesPlay.SetActive(true);

        if (OnRestartGame != null)
        {
            OnRestartGame();
        }
    }


    public void BtnMusic()
    {
        audioManager.PlaySound(SOUND_BTN_CLICK);

        if (PlayerPrefs.GetString(PLAYER_PREFS_MUSIC) == PLAYER_PREFS_ON)
        {
            musicIcon.GetComponent<SpriteRenderer>().sprite = musicOff;
            PlayerPrefs.SetString(PLAYER_PREFS_MUSIC, PLAYER_PREFS_OFF);
        }
        else
        {
            musicIcon.GetComponent<SpriteRenderer>().sprite = musicOn;
            PlayerPrefs.SetString(PLAYER_PREFS_MUSIC, PLAYER_PREFS_ON);
        }
    }


    private void CheckGame()
    {
        CheckBons();

        if (stick.transform.localScale.y > SpawnBlocks.MinScaleStick &&
            stick.transform.localScale.y < SpawnBlocks.MaxScaleStick)
        {
            if (OnCheckSizeStick != null)
            {
                OnCheckSizeStick(true);
            }
        }
        else
        {
            if (OnCheckSizeStick != null)
            {
                OnCheckSizeStick(false);
            }
        }
    }


    private void CheckBons()
    {
        if (stick.transform.localScale.y > SpawnBlocks.MinBonusScaleStick &&
            stick.transform.localScale.y < SpawnBlocks.MaxBonusScaleStick)
        {
            audioManager.PlaySound(SOUND_BONUS);
            
            if (OnGetPoin != null)
            {
                OnGetPoin();
            }
        }
    }


    private void CheckWin(bool isWin)
    {
        if (isWin)
        {
            audioManager.PlaySound(SOUND_WIN);
            
            if (OnGetPoin != null)
            {
                OnGetPoin();
            }
        }
        else
        {
            audioManager.PlaySound(SOUND_LOSE);

            canvasLost.SetActive(true);
            canvesPlay.SetActive(false);
            detectClick.SetActive(false);

            if (OnLoseGame != null)
            {
                OnLoseGame();
            }
        }
    }


    private void SetStartPosinionMainGameObjs()
    {
        if (mainGameObjs.transform.position != StartPositionMainGameObjs && isStartGame)
        {
            mainGameObjs.transform.position
                = Vector3.MoveTowards(mainGameObjs.transform.position, StartPositionMainGameObjs,
                    Time.deltaTime * SPEED);
        }
        else if (mainGameObjs.transform.position == StartPositionMainGameObjs && !isStartPosition)
        {
            isStartPosition = true;
        }
    }


    private void SetButnMusic()
    {
        if (PlayerPrefs.GetString(PLAYER_PREFS_MUSIC) == PLAYER_PREFS_OFF)
        {
            musicIcon.GetComponent<SpriteRenderer>().sprite = musicOff;
        }
        else
        {
            PlayerPrefs.SetString(PLAYER_PREFS_MUSIC, PLAYER_PREFS_ON);
        }
    }
}