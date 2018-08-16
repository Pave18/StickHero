using UnityEngine;
using System.Collections;


public class Stick : MonoBehaviour
{
    private const float GROW_RATE = 0.13f;
    private const string SOUND_DROP_STICK = "DropStick", SOUND_GROW_STICK = "GrowStick";
    private const string IS_DROP_STICK = "IsDropStick", IS_FAIL_FROP_STICK = "IsFailDropStick";
    private const string DROP_STICK = "DropStick";
    private readonly Vector3 StandardScale = new Vector3(0.2f, 0f, 1f);
    private readonly Vector3 StandardPositionOldStick = new Vector3(-12f, -2.5f, 0f);

    private static int idIsDropStick, idIsFailDropStick;

    public delegate void action();

    public delegate void actionWhithFloat(float f);

    public static event action OnCheckGame;
    public static event actionWhithFloat OnSetEdgeStick;

    public GameObject stick;
    public Animator animator;

    private static bool isStartGrowingStick, isCanStickGrow;
    private Vector3 positionNowStick;
    private GameObject nowStick, oldStick;
    private AudioManager audioManager;


    private void Awake()
    {
        GameArrangement.OnStartGame += StartGame;
        GameArrangement.OnRestartGame += RestartStick;
        SpawnBlocks.OnSpawnBlock += SpawNewStick;
        StickHero.OnHeroDrop += OnFailDropStick;


        idIsDropStick = Animator.StringToHash(IS_DROP_STICK);
        idIsFailDropStick = Animator.StringToHash(IS_FAIL_FROP_STICK);
    }


    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Stick: No AudioManager found!");
        }
    }


    private void Update()
    {
        Growind();
        SlideToNextBlock();
    }


    private void StartGame()
    {
        isCanStickGrow = true;
    }


    private void RestartStick()
    {
        //  animator.SetBool(ANIM_FAIL_DROP_STICK, false);

        SpawNewStick();
        Destroy(nowStick);
    }


    private void OnMouseDown()
    {
        if (!isCanStickGrow)
            return;

        audioManager.PlaySound(SOUND_GROW_STICK);
        stick.SetActive(true);
        isStartGrowingStick = true;
    }


    private void OnMouseUp()
    {
        if (!isCanStickGrow)
            return;

        audioManager.StopSound(SOUND_GROW_STICK);

        if (OnSetEdgeStick != null)
        {
            OnSetEdgeStick(stick.transform.localScale.y);
        }

        isStartGrowingStick = false;

        StartCoroutine(DropAnimation());

        isCanStickGrow = false;
    }


    private void OnFailDropStick()
    {
        animator.SetBool(idIsDropStick, false);
        animator.SetBool(idIsFailDropStick, true);
    }


    private void Growind()
    {
        if (!isStartGrowingStick)
            return;

        stick.transform.localScale += new Vector3(0f, GROW_RATE, 0f);
    }


    private void SpawNewStick()
    {
        isCanStickGrow = true;

        oldStick = nowStick;
        nowStick = Instantiate(stick);

        FindPositionNowStick();
        SetStartTransformStick();
    }


    private void SlideToNextBlock()
    {
        SlideStickInPosition(nowStick, positionNowStick);
        SlideStickInPosition(oldStick, StandardPositionOldStick);
    }


    private void SlideStickInPosition(GameObject stick, Vector3 positon)
    {
        if (stick != null && stick.transform.position != positon)
        {
            stick.transform.position =
                Vector3.MoveTowards(stick.transform.position, positon, Time.deltaTime * SpawnBlocks.SLIDE_BLOCKS_SPEED);
        }
    }


    private void FindPositionNowStick()
    {
        positionNowStick = nowStick.transform.localPosition;
        positionNowStick.x = nowStick.transform.localPosition.x
                             - ((SpawnBlocks.SizeOldPlatform / 2 + SpawnBlocks.SizeNowPlatform / 2
                                                                 + nowStick.transform.localScale.y) / 2)
                             + ((nowStick.transform.localScale.y - SpawnBlocks.OldMinScaleStick) / 2);
    }


    private void SetStartTransformStick()
    {
        var tempPosition = stick.transform.position;
        tempPosition.x = SpawnBlocks.StandartMainBlockPos.x + (SpawnBlocks.SizeNowPlatform / 2) / 2;
        stick.transform.position = tempPosition;

        //Zeroing out Rotation
        var tempRotation = stick.transform.localRotation;
        tempRotation.z = 0f;
        stick.transform.localRotation = tempRotation;

        //Def Scale
        stick.transform.localScale = StandardScale;

        stick.SetActive(false);
    }


    private float GetLingthAnimClip(string nameClip)
    {
        float time = 0;
        foreach (var clips in animator.runtimeAnimatorController.animationClips)
        {
            if (clips.name == nameClip)
            {
                return clips.length;
            }
        }

        return time;
    }


    private IEnumerator DropAnimation()
    {
        animator.SetBool(idIsDropStick, true);

        yield return new WaitForSeconds(GetLingthAnimClip(DROP_STICK));

        audioManager.PlaySound(SOUND_DROP_STICK);

        if (OnCheckGame != null)
        {
            OnCheckGame();
        }
    }
}