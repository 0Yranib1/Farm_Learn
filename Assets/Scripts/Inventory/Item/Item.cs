using System.Collections;
using System.Collections.Generic;
using MFarm.CropPlant;
using UnityEngine;

namespace MFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;
        public  ItemDetails itemDetails;
        private BoxCollider2D coll;
        private SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Awake()
        {
            spriteRenderer=GetComponentsInChildren<SpriteRenderer>()[0];
            coll=GetComponent<BoxCollider2D>();
        }

        public void Init(int ID)
        {
            itemID=ID;
            itemDetails=InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite!=null?itemDetails.itemOnWorldSprite:itemDetails.itemIcon;
                
                //根据物品图片尺寸设置碰撞器尺寸
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size=newSize;
                coll.offset=new Vector2(0,spriteRenderer.sprite.bounds.center.y);

            }

            if (itemDetails.ItemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemID);
                gameObject.AddComponent<ItemInteractive>();
            }
        }

        void Start()
        {
            if (itemID != 0)
            {
                Init(itemID);
            }
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

