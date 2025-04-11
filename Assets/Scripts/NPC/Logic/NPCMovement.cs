using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.AStar;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    public ScheduleDetails currentSchedule;
    
    //临时存储信息
    [SerializeField] private string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    public string StarScene{set =>currentScene = value;}
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;

    [Header("移动速度")] 
    public float normalSpeed = 2f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    private Vector2 dir;
    public bool isMoving;

    
    //Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;
    private Grid grid;

    private Stack<MovementStep> movementSteps;

    private bool isInitialised;

    private bool npcMove;
    private bool sceneLoaded;
    
    private TimeSpan GameTime=> TimeManager.Instance.GameTime;
    
    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            Movement();
        }

    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }
    
    private void OnAfterSceneLoadedEvent()
    {
        grid= FindObjectOfType<Grid>();
        CheckVisiable();

            if (!isInitialised)
            {
                InitNPC();
                isInitialised = true;
            }

            sceneLoaded = true;
    }
    
    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
    }

    /// <summary>
    /// 根据schedule构建路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;
        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }
        //TODO:跨场景移动
        if (movementSteps.Count > 1)
        {
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }

    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;
        TimeSpan currentGameTime = GameTime;

        foreach (var step in movementSteps)
        {
            if (previousSetp == null)
            {
                previousSetp = step;
            }

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;
            if (MoveInDiagonal(step, previousSetp))
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            }
            else
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            }
            //累加获得下一步时间戳
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            //循环下一步
            previousSetp = step;
        }
    }

    /// <summary>
    /// 判断是否走斜向
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool MoveInDiagonal(MovementStep currentStep,MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x)&&(currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }
    
    #region 设置NPC显示
    
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        //影子关闭
        // transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        //影子开启
        // transform.GetChild(0).gameObject.SetActive(false);
    }
    
    private void InitNPC()
    {
        targetScene = currentScene;
        //保持在当前坐标网格中心点
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f,
            currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition= currentGridPosition;


    }
    #endregion

    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;
                CheckVisiable();
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);
            
                MoveToGridPosition(nextGridPosition, stepTime);
            }
        }

    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(nextGridPosition, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);
        //还有时间用来移动
        if (stepTime > GameTime)
        {
            //移动的时间差 以秒为单位
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //实际移动距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //实际速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));

            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir= (nextWorldPosition - transform.position).normalized;
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime,
                        dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position+ posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //时间到了瞬移角色
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }

    /// <summary>
    /// 网格坐标返回世界坐标中心点
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f, 0);
    }
    
}
