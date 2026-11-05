using System;
using System.Collections.Generic;
using UnityEngine;

public class RulebookView : MonoBehaviour
{
    [SerializeField] private Transform ruleContainer;
    [SerializeField] private RuleView rulePrefab;

    private readonly List<RuleView> ruleViews = new List<RuleView>();

    public event Action<RuleView> RuleClicked;

    public void Show(IReadOnlyList<RuleData> rules, ICollection<RuleData> newRules = null)
    {
        foreach (RuleView oldView in ruleViews)
        {
            Destroy(oldView.gameObject);
        }

        ruleViews.Clear();

        foreach (RuleData rule in rules)
        {
            if (rule == null)
            {
                continue;
            }

            RuleView view = Instantiate(rulePrefab, ruleContainer);
            view.Setup(rule, newRules != null && newRules.Contains(rule));
            view.Clicked += () => RuleClicked?.Invoke(view);
            ruleViews.Add(view);
        }
    }
}
