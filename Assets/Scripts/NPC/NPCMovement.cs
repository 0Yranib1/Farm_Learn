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
    [SerializeField] private string currentScent;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    public string StarScent{set =>currentScent = value;}

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

    private Stack<MovementStep> movementSteps;
    
    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
            CheckVisiable();
    }
    
    private void CheckVisiable()
    {
        if (currentScent == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
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
    #endregion
}
