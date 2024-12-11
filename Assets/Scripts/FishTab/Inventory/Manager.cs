
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] tabs;
    public GameObject cellPrefab;     // Assign your cell prefab here
    public GameObject inventoryGrid;  // Assign your grid panel here
    private List<GameObject> inventorySlots = new List<GameObject>();
    public int maxInventorySlots = 30; // 30 cells in the grid
    protected Player player;

    public Item[] inventoryItems; // Assign your InventoryItems in the Inspector

    public Item GetItemById(string id)
    {
        foreach (Item item in inventoryItems)
        {
            if (item.type == ItemType.Fish && item.id == id)
                return item;
        }
        return null; // Return null if no item matches
    }

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("InventoryManager Start");
        List<Fish> fishInventory = player.fishInventory;
        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject newSlot = Instantiate(cellPrefab, inventoryGrid.transform);
            inventorySlots.Add(newSlot);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Image slotImage = inventorySlots[i].GetComponent<Image>();
            if (slotImage.sprite == null) // Check for an empty slot
            {
                Fish fish = player.fishInventory[i];

                Item inventoryItem = GetItemById(fish.speciesId);
                // TODO: figure out how to use fish.speciesId to get the image in ScriptableObject
                // slotImage.sprite = fish.speciesId;
                // fish.speciesId
                // slotImage.sprite = fishSprites[fishIndex]; // Add fish sprite
                break;
            }
        }
    }
}
