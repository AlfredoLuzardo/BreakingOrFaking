using System.Collections;
using TMPro;
using UnityEngine;

public class ContradictionChecker : MonoBehaviour
{
    [SerializeField] private DocumentView documentView;
    [SerializeField] private RulebookView rulebookView;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private float resultDuration = 2f;
    [SerializeField] private string contradictsMessage = "CONTRADICTS";
    [SerializeField] private string noContradictionMessage = "NO CONTRADICTION";
    [SerializeField] private Color contradictsColor = new Color(0.9f, 0.25f, 0.2f);
    [SerializeField] private Color noContradictionColor = Color.white;

    private SectionView selectedSection;
    private RuleView selectedRule;
    private Coroutine resetRoutine;

    private void OnEnable()
    {
        documentView.SectionClicked += OnSectionClicked;
        documentView.ContentCleared += Clear;
        rulebookView.RuleClicked += OnRuleClicked;
    }

    private void OnDisable()
    {
        documentView.SectionClicked -= OnSectionClicked;
        documentView.ContentCleared -= Clear;
        rulebookView.RuleClicked -= OnRuleClicked;
    }

    public void Clear()
    {
        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        if (selectedSection != null)
        {
            selectedSection.SetSelected(false);
        }

        if (selectedRule != null)
        {
            selectedRule.SetSelected(false);
        }

        selectedSection = null;
        selectedRule = null;
        resultText.text = string.Empty;
    }

    private void OnSectionClicked(SectionView view)
    {
        if (resetRoutine != null)
        {
            return;
        }

        selectedSection = Toggle(selectedSection, view);
        TryCheck();
    }

    private void OnRuleClicked(RuleView view)
    {
        if (resetRoutine != null)
        {
            return;
        }

        selectedRule = Toggle(selectedRule, view);
        TryCheck();
    }

    // Selects the clicked item, or deselects it if it was already selected.
    private static T Toggle<T>(T current, T clicked) where T : SelectableItem
    {
        if (current != null)
        {
            current.SetSelected(false);
        }

        if (current == clicked)
        {
            return null;
        }

        clicked.SetSelected(true);
        return clicked;
    }

    private void TryCheck()
    {
        if (selectedSection == null || selectedRule == null)
        {
            return;
        }

        bool contradicts = selectedSection.Section.Contradicts(selectedRule.Rule);
        resultText.text = contradicts ? contradictsMessage : noContradictionMessage;
        resultText.color = contradicts ? contradictsColor : noContradictionColor;
        resetRoutine = StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resultDuration);
        resetRoutine = null;
        Clear();
    }
}
