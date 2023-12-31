using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlotService : MonoSingletonGeneric<SlotService>
{
    [SerializeField] private List<ChestSlot> slots;
    private Queue<ChestView> chestQueue = new Queue<ChestView>();

    private void Start()
    {
        EventService.Instance.OnChestOpened += RemoveChestFromQueue;
    }

    private void OnDestroy()
    {
        EventService.Instance.OnChestOpened -= RemoveChestFromQueue;
    }

    public ChestSlot GetVacantSlot()
    {
        ChestSlot vacantSlot = null;
        foreach (ChestSlot slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.IsEmpty = false;
                vacantSlot = slot;
                break;
            }
        }
        return vacantSlot;
    }

    public void AddChestToTheQueue(ChestView chestView)
    {
        chestQueue.Enqueue(chestView);
    }

    private void RemoveChestFromQueue(ChestView chestView)
    {
        if (chestQueue.Count > 0)
            chestQueue = new Queue<ChestView>(chestQueue.Where(chest => chest != chestView));
        else
            Debug.Log("Chest Queue is empty");
    }

    public void StartNextChestUnlocking()
    {
        if (IsAnyChestUnlocking() || chestQueue.Count == 0) return;

        ChestView nextChestToUnlock = chestQueue.FirstOrDefault(chestView => chestView.CurrentState != EChestState.UNLOCKED);

        if (nextChestToUnlock != null)
            nextChestToUnlock.StartUnlocking();
        else
            Debug.Log("No chest in the queue to start unlocking.");
    }

    public bool IsAnyChestUnlocking()
    {
        return chestQueue.Any(chestView => chestView.CurrentState == EChestState.UNLOCKING);
    }

    //public bool IsAnyChestUnlocking()
    //{
    //    for (int i = 0; i < slots.Count; i++)
    //    {
    //        if (slots[i].ChestState == EChestState.UNLOCKING)
    //            return true;
    //    }
    //    return false;
    //}
}
