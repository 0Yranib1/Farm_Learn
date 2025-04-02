using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;

    private bool isMoving;
    private bool inputDisable;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent+=OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPositionEvent;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }
    private void OnDisable()
    {
        
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPositionEvent;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }
    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        //执行动画
        EventHandler.CallExecuteActionAfterAnimation(pos,itemDetails);
    }
    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }
    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;
    }
    private void OnMoveToPositionEvent(Vector3 pos)
    {
        transform.position = pos;
    }

    //获取输入方向变量
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * .05f;
            inputY = inputY * .05f;
        }
        
        movementInput = new Vector2(inputX, inputY).normalized;
        isMoving = movementInput != Vector2.zero;
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
        // rb.velocity= movementInput * speed;
    }
    // Start is called before the first frame update

    void FixedUpdate()
    {
        if(!inputDisable)
            Movement();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (inputDisable == false)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
            
        }
    }
}
