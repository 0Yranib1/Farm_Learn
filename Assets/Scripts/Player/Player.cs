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

    private float mouseX;
    private float mouseY;

    private bool useTool;
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
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //执行动画
        if (itemDetails.ItemType != ItemType.Seed && itemDetails.ItemType != ItemType.Commodity &&
            itemDetails.ItemType != ItemType.Furniture)
        {
            mouseX= mouseWorldPos.x- transform.position.x;
            mouseY= mouseWorldPos.y- transform.position.y;

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos,itemDetails);
        }

    }
    
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }

        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos,itemDetails);
        yield return new WaitForSeconds(0.25f);
        useTool = false;
        inputDisable = false;
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
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
            
        }
    }
}
