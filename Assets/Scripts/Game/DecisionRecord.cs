public class DecisionRecord
{
    public DecisionRecord(NewsItemData item, bool approved)
    {
        Item = item;
        Approved = approved;
    }

    public NewsItemData Item { get; }
    public bool Approved { get; }
    public bool IsCorrect => Approved == Item.IsReal;
}
