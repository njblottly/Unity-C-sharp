using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slotPrefab;
    [SerializeField] private Transform slotsRoot;
    private bool slotsCreated;

    public const int numSlots = 5;

    Image[] itemImages = new Image[numSlots];
    Item[] items = new Item[numSlots];
    GameObject[] slots = new GameObject[numSlots];

    public void Start()
    {
        CreateSlots();
    }

    public void CreateSlots()
    {
        if (slotsCreated) return;
        if (slotsRoot == null) slotsRoot = transform.Find("InventoryBackground");
        if (slotsRoot == null || slotPrefab == null)
            throw new System.InvalidOperationException("Inventory needs a slot prefab and an InventoryBackground/slotsRoot reference.");
        slotsCreated = true;
        if (slotPrefab != null)
        {
            for (int i = 0; i < numSlots; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.name = "ItemSlot_" + i;
                newSlot.transform.SetParent(slotsRoot, false);
                slots[i] = newSlot;
                Slot slot = newSlot.GetComponent<Slot>();
                if (slot.itemImage == null) slot.itemImage = newSlot.transform.Find("ItemImage").GetComponent<Image>();
                if (slot.qtyText == null) throw new System.InvalidOperationException("Slot quantity text is unassigned.");
                itemImages[i] = slot.itemImage;
                itemImages[i].enabled = false;
                slot.qtyText.enabled = false;
            }
        }
    }

    // Query and spend the same item stacks shown by the existing UI.
    public int Count(Item.ItemType type)
    {
        int total = 0;
        foreach (Item item in items) if (item != null && item.itemType == type) total += item.quantity;
        return total;
    }

    public bool TrySpend(Item.ItemType type, int amount)
    {
        if (amount <= 0 || Count(type) < amount) return false;
        int remaining = amount;
        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            if (items[i] == null || items[i].itemType != type) continue;
            int take = Mathf.Min(remaining, items[i].quantity);
            items[i].quantity -= take;
            remaining -= take;
            Slot slot = slots[i].GetComponent<Slot>();
            slot.qtyText.text = items[i].quantity.ToString();
            slot.qtyText.enabled = items[i].quantity > 1;
            if (items[i].quantity == 0)
            {
                Destroy(items[i]);
                items[i] = null;
                itemImages[i].enabled = false;
            }
        }
        return true;
    }

    // Preflight capacity so a multi-coin return succeeds in full or changes nothing.
    public bool TryAddItems(Item itemToAdd, int quantity)
    {
        if (itemToAdd == null) throw new System.ArgumentNullException(nameof(itemToAdd));
        if (quantity <= 0) return false;
        if (!slotsCreated) CreateSlots();
        int empty = 0;
        bool matchingStack = false;
        foreach (Item item in items)
        {
            if (item == null) empty++;
            else if (item.itemType == itemToAdd.itemType && itemToAdd.stackable) matchingStack = true;
        }
        if (itemToAdd.stackable ? (!matchingStack && empty == 0) : empty < quantity) return false;
        for (int i = 0; i < quantity; i++)
            if (!AddItem(itemToAdd)) throw new System.InvalidOperationException("Inventory capacity changed during an atomic return.");
        return true;
    }

    public bool AddItem(Item itemToAdd)
    {
        if (!slotsCreated) CreateSlots();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemType == itemToAdd.itemType && itemToAdd.stackable == true)
            {
                items[i].quantity = items[i].quantity + 1;
                Slot slotScript = slots[i].GetComponent<Slot>();
                Text quantityText = slotScript.qtyText;
                quantityText.enabled = true;
                quantityText.text = items[i].quantity.ToString();
                return true;
            }

            if (items[i] == null)
            {
                items[i] = Instantiate(itemToAdd);
                items[i].quantity = 1;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return true;
            }
        }
        return false;
    }
}
