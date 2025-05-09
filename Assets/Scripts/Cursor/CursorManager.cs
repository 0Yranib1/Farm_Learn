using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.CropPlant;
using MFarm.Inventory;
using MFarm.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal,tool,seed,item;
    private Sprite currentSprite;//当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;

    //建造图标
    private Image buildImage;
    
    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
    }

    public void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnBeforeSceneUnloadedEvent()
    {
        cursorEnable = false;
    }
    
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else
        {
            currentItem=itemDetails;
            
            currentSprite=itemDetails.ItemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal,
            };
            cursorEnable = true;

            //显示建造物品图片
            if (itemDetails.ItemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
            
        }

    }
    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        
        //获取建造图标
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);
        
        currentSprite = normal;
        SetCursorImage(normal);
        mainCamera=Camera.main;
    }

    private void Update()
    {
        if(cursorCanvas==null)
            return;
        cursorImage.transform.position=Input.mousePosition;
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            // buildImage.gameObject.SetActive(false);
        }

    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos,currentItem);
        }
    }
    
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }


    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        
        //建造图片跟随移动
        buildImage.rectTransform.position = Input.mousePosition;
        
        //判断使用范围
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius ||
            Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }
        
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        Crop crop=GridMapManager.Instance.GetCropObject(mouseWorldPos);
        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.getCropDetails(currentTile.seedItemID);
            switch (currentItem.ItemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug>-1 && currentTile.seedItemID==-1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.Commodity:
                    if(currentTile.canDropItem&&currentItem.canDropped) SetCursorValid();else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug>-1&&currentTile.daysSinceWatered==-1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if (crop.CanHarvest&&crop.cropDetails.CheckToolAvaiable(currentItem.itemID))
                        {
                            SetCursorValid();
                        }else
                        {
                            SetCursorInValid();
                        }
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvaiable(currentItem.itemID))
                        {
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays)
                            {
                                SetCursorValid();
                            }
                            else
                            {
                                SetCursorInValid();
                            }
                        }
                    }
                    else
                    {
                        SetCursorInValid();
                    }

                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos ,currentItem))
                    {
                        SetCursorValid();
                    }else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var bluePrintDetails =
                        InventoryManager.Instance.bluePrintData.GetBluePrintDetails(currentItem.itemID);
                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                
            }
                
        }
        else
        {
            SetCursorInValid();
        }
    }


    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;

        var otherColl = Physics2D.OverlapBox(point, size, 0);

        if (otherColl != null)
        {
            return otherColl.GetComponent<Furniture>();
        }
        else
        {
            return false;
        }
    }
    
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color=new Color(1,1,1,0.5f);
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        buildImage.color=new Color(1,0,0,0.5f);
    }
    
    /// <summary>
    /// 判断是否与UI交互
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }
    
}
