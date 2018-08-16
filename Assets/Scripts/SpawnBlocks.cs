using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class SpawnBlocks : MonoBehaviour
{
    public const float SLIDE_BLOCKS_SPEED = 10f;
    public static readonly Vector3 StandartMainBlockPos = new Vector3(-2.2f, -5f, 0f);

    private const float MIN_DISTANCE = -0.6f, MAX_DISTANCE = 2.5f;
    private const float DEFAULT_PROPORTION_SCALE_BLOCK = 3f;
    private const float DEFAULT_SIZE_BONUS = 0.10125f * DEFAULT_PROPORTION_SCALE_BLOCK;
    private const string CHILD_PLATFORM = "Platform", CHILD_BONUS = "Bonus";
    private const string NAME_FERST_BLOCK = "FirstBlock";
    private readonly Vector3 StandartSpawnBlockPos = new Vector3(7f, -5f, 0f);
    private readonly Vector3 StandartOldBlockPos = new Vector3(-12f, -5f, 0f);

    public delegate void action();

    public static event action OnSpawnBlock;

    public GameObject blockPrefab, nowBlock;

    private GameObject spawnBlock, oldBlock;
    private bool isOnPlace, isSlideSpawnBlock, isGameStartPosition;

    public static float MinScaleStick { get; private set; }
    public static float MaxScaleStick { get; private set; }
    public static float MinBonusScaleStick { get; private set; }
    public static float MaxBonusScaleStick { get; private set; }
    public static float OldMinScaleStick { get; private set; }
    public static float SizeOldPlatform { get; private set; }
    public static float SizeNowPlatform { get; private set; }
    public static Vector3 PositionSpawnBlock { get; private set; }


    private void Awake()
    {
        GameArrangement.OnStartGame += StartGame;
        GameArrangement.OnRestartGame += RestartBlocks;
        StickHero.OnHeroInPosition += SpawnNextBlock;
    }


    private void Update()
    {
        SetPositionSpawnBlock();
        SlideToNextBlock();
    }


    private void StartGame()
    {
        Spawn();
        isGameStartPosition = true;
    }


    private void RestartBlocks()
    {
        var tempPrefabPlatform = GetChildByName(blockPrefab.GetComponentsInChildren<Transform>(), CHILD_PLATFORM);
        GetChildByName(nowBlock.GetComponentsInChildren<Transform>(), CHILD_PLATFORM).transform.localScale
            = tempPrefabPlatform.transform.localScale;

        Destroy(spawnBlock);
        Destroy(oldBlock);

        Spawn();
        isOnPlace = false;
    }


    private void Spawn()
    {
        PositionSpawnBlock = new Vector3(Random.Range(MIN_DISTANCE, MAX_DISTANCE), StandartSpawnBlockPos.y,
            StandartSpawnBlockPos.z);

        spawnBlock = Instantiate(blockPrefab, StandartSpawnBlockPos, Quaternion.identity);
        var tempPlatform = GetChildByName(spawnBlock.GetComponentsInChildren<Transform>(), CHILD_PLATFORM);

        //Inst rand Scale. Size Platform;
        var tempScale = tempPlatform.transform.localScale;
        tempScale.x = RandScale();
        tempPlatform.transform.localScale = tempScale;

        //Inst new size BoxCollider
        var tempSize = spawnBlock.GetComponent<BoxCollider>().size;
        tempSize.x = tempPlatform.transform.localScale.x / 2;
        spawnBlock.GetComponent<BoxCollider>().size = tempSize;

        FindSizesStickForCheck();
    }


    private void SpawnNextBlock(bool isNowBock)
    {
        if (isNowBock)
        {
            SizeOldPlatform = oldBlock != null ? GetSizePlatform(oldBlock) : DEFAULT_PROPORTION_SCALE_BLOCK;

            oldBlock = nowBlock;
            nowBlock = spawnBlock;

            Spawn();

            isOnPlace = false;

            if (OnSpawnBlock != null)
            {
                OnSpawnBlock();
            }
        }
    }


    private void SetPositionSpawnBlock()
    {
        if (isGameStartPosition)
        {
            if (spawnBlock.transform.position != PositionSpawnBlock && !isOnPlace)
            {
                SlideBlockInPosition(spawnBlock, PositionSpawnBlock);
            }
            else if (spawnBlock.transform.position == PositionSpawnBlock)
            {
                isOnPlace = true;
            }
        }
    }


    private void SlideToNextBlock()
    {
        if (nowBlock != null && nowBlock.transform.position != StandartMainBlockPos &&
            nowBlock.name != NAME_FERST_BLOCK)
        {
            SlideBlockInPosition(nowBlock, StandartMainBlockPos);

            //Hide bonus on NowBlock
            var tempBonus = GetChildByName(nowBlock.GetComponentsInChildren<Transform>(), CHILD_BONUS);
            if (tempBonus != null)
            {
                tempBonus.gameObject.SetActive(false);
            }
        }

        if (oldBlock != null && oldBlock.transform.position != StandartSpawnBlockPos)
        {
            SlideBlockInPosition(oldBlock, StandartOldBlockPos);
        }
    }


    private void SlideBlockInPosition(GameObject block, Vector3 position)
    {
        block.transform.position
            = Vector3.MoveTowards(block.transform.position, position,
                Time.deltaTime * SLIDE_BLOCKS_SPEED);
    }


    private void FindSizesStickForCheck()
    {
        var posNowBlock = StandartMainBlockPos.x;
        var posSpawnBlock = 0f;
        if (PositionSpawnBlock.x <= 0)
            posSpawnBlock = Math.Abs(PositionSpawnBlock.x);
        else
            posSpawnBlock -= PositionSpawnBlock.x;

        SizeNowPlatform = GetSizePlatform(nowBlock);
        var sizeSpawnPlatform = GetSizePlatform(spawnBlock);

        OldMinScaleStick = MinScaleStick;

        MinScaleStick = (Math.Abs(posNowBlock + posSpawnBlock) * 2) - ((SizeNowPlatform + sizeSpawnPlatform) / 2);
        MaxScaleStick = MinScaleStick + sizeSpawnPlatform;

        MinBonusScaleStick = MinScaleStick + (sizeSpawnPlatform / 2) - DEFAULT_SIZE_BONUS / 2;
        MaxBonusScaleStick = MinBonusScaleStick + DEFAULT_SIZE_BONUS;

        //For check to console
        Debug.Log("minScale= " + MinScaleStick + " maxScale= " + MaxScaleStick
                  + " minBonus= " + MinBonusScaleStick + " maxBonus= " + MaxBonusScaleStick);
    }


    private float RandScale()
    {
        const float MIN_MIN_SIZE = 0.3f;
        const float MAX_MIN_SIZE = 0.6f;
        const float MIN_MAX_SIZE = 0.6f;
        const float MAX_MAX_SIZE = 1f;

        return Random.Range(0, 100) > 80
            ? Random.Range(MIN_MIN_SIZE, MAX_MIN_SIZE)
            : Random.Range(MIN_MAX_SIZE, MAX_MAX_SIZE);
    }


    private float GetSizePlatform(GameObject block)
    {
        return GetChildByName(block.GetComponentsInChildren<Transform>(), CHILD_PLATFORM)
                   .transform.localScale.x * DEFAULT_PROPORTION_SCALE_BLOCK;
    }


    private Transform GetChildByName(IEnumerable<Transform> getComponentsInChildren, string name)
    {
        return getComponentsInChildren.FirstOrDefault(child => child.name == name);
    }
}