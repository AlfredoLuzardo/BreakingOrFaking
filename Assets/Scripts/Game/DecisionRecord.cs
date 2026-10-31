public class DecisionRecord
{
    public DecisionRecord(ItemData item, bool approved)
    {
        Item = item;
        Approved = approved;
    }

    public ItemData Item { get; }
    public bool Approved { get; }
    public bool IsCorrect => Approved == Item.IsTrue;
}
