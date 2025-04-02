using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {

        public SpriteRenderer itemSprite;
        public SpriteRenderer shadowSprite;

        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }

        // Start is called before the first frame update
        void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    
}

