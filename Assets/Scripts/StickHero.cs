using UnityEngine;


public class StickHero : MonoBehaviour
{
    private const float RUNNING_SPEED = 3f, EDGE_SCREEN = 3.5f;
    private readonly Vector3 DefaultHeroPosition = new Vector3(-2.2f, -2.5f, 0f);
    private const string SOUND_STEPS = "Steps";

    public delegate void action();
    public delegate void actionWhithBool(bool b);

    public static event actionWhithBool OnHeroInPosition;
    public static event action OnHeroDrop;

    public GameObject hero;

    private bool isReturnDefPos, isGoEdgeStick, isGoNextBlock;
    private static Vector3 positionNextBloc, positionEdgeStick;
    private AudioManager audioManager;


    private void Awake()
    {
        GameArrangement.OnRestartGame += RestartHero;
        GameArrangement.OnCheckSizeStick += GoTo;
        Stick.OnSetEdgeStick += SetEdgeStick;
    }


    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("StickHero: No AudioManager found!");
        }
    }


    private void Update()
    {
        SetNextPositions();
        WayToNextBlock();
        ReturnToMainBlock();
        WayToEdgeStick();
    }


    private void RestartHero()
    {
        hero.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        hero.transform.position = DefaultHeroPosition;
    }


    private void GoTo(bool isTrueLengthStick)
    {
        audioManager.PlaySound(SOUND_STEPS);

        if (isTrueLengthStick)
        {
            isGoNextBlock = true;
        }
        else
        {
            isGoEdgeStick = true;
        }
    }


    private void WayToEdgeStick()
    {
        if (!isGoEdgeStick)
            return;

        if (IsHeroInPosition(positionEdgeStick, RUNNING_SPEED) || hero.transform.position.x > EDGE_SCREEN)
        {
            audioManager.StopSound(SOUND_STEPS);

            if (OnHeroInPosition != null)
            {
                OnHeroInPosition(false);
            }

            DropHero();
            isGoEdgeStick = false;

            if (OnHeroDrop != null)
            {
                OnHeroDrop();
            }
        }
    }


    private void SetNextPositions()
    {
        positionNextBloc = DefaultHeroPosition;
        positionNextBloc.x = SpawnBlocks.PositionSpawnBlock.x;
    }


    private void SetEdgeStick(float stickSize)
    {
        positionEdgeStick = DefaultHeroPosition;
        positionEdgeStick.x = DefaultHeroPosition.x + (stickSize + SpawnBlocks.SizeNowPlatform / 2) / 2;
    }


    private void WayToNextBlock()
    {
        if (!isGoNextBlock)
            return;


        if (IsHeroInPosition(positionNextBloc, RUNNING_SPEED))
        {
            audioManager.StopSound(SOUND_STEPS);

            if (OnHeroInPosition != null)
            {
                OnHeroInPosition(true);
            }

            isGoNextBlock = false;
            isReturnDefPos = true;
        }
    }


    private void ReturnToMainBlock()
    {
        if (!isReturnDefPos)
            return;

        if (IsHeroInPosition(DefaultHeroPosition, SpawnBlocks.SLIDE_BLOCKS_SPEED))
        {
            isReturnDefPos = false;
        }
    }


    private bool IsHeroInPosition(Vector3 position, float speed)
    {
        if (hero.transform.position != position)
        {
            hero.transform.position =
                Vector3.MoveTowards(hero.transform.position, position, Time.deltaTime * speed);
        }

        if (hero.transform.position == position)
        {
            return true;
        }

        return false;
    }


    private void DropHero()
    {
        hero.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}