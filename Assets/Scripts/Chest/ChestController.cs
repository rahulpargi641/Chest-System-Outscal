
public class ChestController
{
    public ChestModel ChestModel { get; private set; }
    public ChestView ChestView { get; private set; }
    public float TimeSecondsPerGem { get { return 600f; } private set { } } //10 minutes = 1 gem
    public ChestState ChestState { get { return currentState.GetChestState(); } private set { } }

    private IChestState currentState;
    private ChestLockedState chestLocked;
    private ChestUnlockingState chestUnlocking;
    private ChestUnlockedState chestUnlocked;

    public ChestController()
    {
        chestLocked = new ChestLockedState(this);
        chestUnlocking = new ChestUnlockingState(this);
        chestUnlocked = new ChestUnlockedState(this);

        currentState = chestLocked;
    }

    public void SetModel(ChestModel chestModel)
    {
        ChestModel = chestModel;
    }

    public void SetChestView()
    {
        ChestView = ChestPoolService.Instance.GetFromPool(this);
        SetInitialState();
    }

    public void RemoveView()
    {
        ChestPoolService.Instance.ReturnToPool(ChestView);
        ChestView = null;
    }

    public void SetInitialState()
    {
        currentState = chestLocked;
        currentState.OnStateEnable();
    }

    public void ChestButtonAction()
    {
        currentState.ChestButtonAction();
    }

    public void UnlockNow()
    {
        PlayerService.Instance.DecrementGems(currentState.GetRequiredGemsToUnlock());

        currentState.OnStateDisable();
        currentState = chestUnlocked;
        currentState.OnStateEnable();

        AudioService.Instance.PlaySound(SoundType.UnlockNow);
    }

    public void StartUnlocking()
    {
        currentState.OnStateDisable();
        currentState = chestUnlocking;
        currentState.OnStateEnable();

        AudioService.Instance.PlaySound(SoundType.StartUnlocking);
    }
}