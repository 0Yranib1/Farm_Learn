using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;
        public bool canUse;
        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            canUse = false;
        }
        private void OnAfterSceneLoadedEvent()
        {
            canUse = true;
        }
        
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        }
        
        private void OnUpdateGameStateEvent(GameState gameState)
        {
            canUse=gameState==GameState.Gameplay;
        }
        private void Update()
        {
            if (Input.GetKeyDown(key) && canUse )
            {
                if (slotUI.itemDetails != null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                    if(slotUI.isSelected)
                        slotUI.inventoryUI.UpdateSlotHighlight(slotUI.slotIndex);
                    else
                    {
                        slotUI.inventoryUI.UpdateSlotHighlight(-1);
                    }
                    
                    EventHandler.CallItemSelectEvent(slotUI.itemDetails, slotUI.isSelected);
                    
                }
            }
        }
    }
    
}

