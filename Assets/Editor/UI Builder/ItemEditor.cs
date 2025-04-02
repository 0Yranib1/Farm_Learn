using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ItemEditor : EditorWindow
{
    [SerializeField] 
    private VisualTreeAsset m_VisualTreeAsset = default;
    
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    //获得visualelement listview
    private ListView itemListView;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;
    private VisualElement iconPreview;
    //默认预览图片
    private Sprite defaultIcon;
    [MenuItem("Window/UI Toolkit/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        
        //相对路径拿到模板数据
        itemRowTemplate =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");
        
        //变量赋值
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        //设置每个数据项的高度为50px
        itemListView.fixedItemHeight = 50;
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("icon");
        defaultIcon=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_Game.png");
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteItemClicked;
        //加载数据
        LoadDataBase();
        
        GenerateListView();
    }

    private void OnAddItemClicked()
    {
        ItemDetails newItem = new ItemDetails();
        //初始化数据
        newItem.itemID = itemList.Count + 1000+1;
        newItem.itemName = "New Item";
        itemList.Add(newItem);
        itemListView.Rebuild();
        
    }
    private void OnDeleteItemClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }
    //加载数据
    private void LoadDataBase()
    {
       var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");

       if (dataArray.Length > 1)
       {
           dataBase = AssetDatabase.LoadAssetAtPath<ItemDataList_SO>(AssetDatabase.GUIDToAssetPath(dataArray[0]));
       }
       
       itemList = dataBase.itemDetailsList;
       // Debug.Log(itemList[0].itemID);
       
       //保存数据用标记
       EditorUtility.SetDirty(dataBase);
    }

    //生成列表项格式 数据绑定
    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;

                e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].itemName;
            }
        };

        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        
        itemListView.selectionChanged += OnListSelectionChange;
        itemDetailsSection.visible = false;
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem =(ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }
    
    //详细列表数据绑定
    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemID;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemID = evt.newValue;
        });
        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();
        });
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<UnityEditor. UIElements. ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<UnityEditor. UIElements. ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;
        
            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });
        
        itemDetailsSection.Q<UnityEditor. UIElements. ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<UnityEditor. UIElements. ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = (Sprite)evt.newValue;
        });
        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.ItemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.ItemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemType = (ItemType)evt.newValue;
        });
        
        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });
        
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });
        
        itemDetailsSection.Q<Toggle>("CanPickUp").value = activeItem.canPickedUp;
        itemDetailsSection.Q<Toggle>("CanPickUp").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedUp = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });
        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}
