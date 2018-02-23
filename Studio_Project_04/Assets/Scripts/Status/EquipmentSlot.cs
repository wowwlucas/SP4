﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{

    public string slotType;

    bool isEmpty;

	// Use this for initialization
	void Start () {
        isEmpty = true;
	}

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        if (this.isEmpty)
        {
            if (this.slotType == "Weapon")
            {
                if (droppedItem.weapon != null)
                {
                    droppedItem.equipped = true;
                    droppedItem.transform.SetParent(this.transform);
                    droppedItem.transform.position = this.transform.position;
                }       
            }
            
        }
        else
        {
            //InventoryObject temp = Inventory.Instance.items[id];
            //Inventory.Instance.items[id] = Inventory.Instance.items[droppedItem.slot];
            //Inventory.Instance.items[droppedItem.slot] = new InventoryObject();
            //Inventory.Instance.items[droppedItem.slot] = temp;

            //Transform item = this.transform.GetChild(0);
            //item.GetComponent<ItemData>().slot = droppedItem.slot;
            //item.transform.SetParent(Inventory.Instance.slots[droppedItem.slot].transform);
            //item.transform.position = Inventory.Instance.slots[droppedItem.slot].transform.position;
            //droppedItem.slot = id;
            //droppedItem.transform.SetParent(this.transform);
            //droppedItem.transform.position = this.transform.position;
        }
    }
}