using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private TMP_Text weatherText;
    [SerializeField] private TMP_Text degreeText;
    [SerializeField] private Image iconImage;

    private IReadOnlyList<DailyCondition> conditions;
    private int displayedIndex = -1;

    public void ShowDay(DayData day)
    {
        conditions = day != null ? day.Conditions : null;
        displayedIndex = int.MinValue;
        SetDayProgress(0f);
    }

    // Progress is 0 at the start of the playable day and 1 when its timer ends.
    public void SetDayProgress(float progress)
    {
        int count = conditions != null ? conditions.Count : 0;
        int index = count == 0
            ? -1
            : Mathf.Min(Mathf.FloorToInt(Mathf.Clamp01(progress) * count), count - 1);

        if (index == displayedIndex)
        {
            return;
        }

        displayedIndex = index;
        DailyCondition condition = index >= 0 ? conditions[index] : null;
        if (weatherText != null)
        {
            weatherText.text = condition != null ? condition.Weather : string.Empty;
        }

        if (degreeText != null)
        {
            degreeText.text = condition != null ? condition.Degree : string.Empty;
        }

        if (iconImage != null)
        {
            iconImage.sprite = condition != null ? condition.Icon : null;
            iconImage.enabled = iconImage.sprite != null;
        }
    }
}
