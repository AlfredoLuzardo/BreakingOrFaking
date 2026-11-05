public class DecisionRecord
{
    public DecisionRecord(StoryCase storyCase, bool approved)
    {
        Case = storyCase;
        Approved = approved;
    }

    public StoryCase Case { get; }
    public bool Approved { get; }
    public bool IsCorrect => Approved == Case.IsTrue;
}
