using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public abstract class DocumentData : ScriptableObject
{
    [Tooltip("Publication date as eight digits: MMDDYYYY (for example, 10312026).")]
    [SerializeField] private string date;

    public string Date => date;
    public abstract string Label { get; }

    // Every document type exposes its clickable text as Sections, so the rulebook,
    // contradiction checks and results work the same way for all types.
    public abstract IEnumerable<Section> GetSections();

    public virtual string DescribeLocation(Section section)
    {
        return Label;
    }

    public bool HasContradictions()
    {
        return GetSections().Any(section => section != null
            && section.ContradictedRules.Any(rule => rule != null));
    }

    public bool TryGetDate(out DateTime value)
    {
        return DateTime.TryParseExact(date, "MMddyyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out value);
    }

    public string FormatDate(string format)
    {
        return TryGetDate(out DateTime value)
            ? value.ToString(format, CultureInfo.InvariantCulture)
            : date;
    }

    protected virtual void OnValidate()
    {
        if (!string.IsNullOrEmpty(date) && !TryGetDate(out _))
        {
            Debug.LogWarning($"{name}: Date must be a valid date in MMDDYYYY format.", this);
        }
    }
}
