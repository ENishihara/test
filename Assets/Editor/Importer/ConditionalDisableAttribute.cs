using UnityEngine;
using System;

/// <summary>
/// 与えられた条件に合致しないとき入力を無効にするAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalDisableAttribute : PropertyAttribute
{
    public string sourceField = "";
    public bool disable = false;
    public int enableValue = 0;

    public ConditionalDisableAttribute(string sourceField, int enableValue)
    {
        this.sourceField = sourceField;
        this.enableValue = enableValue;
        this.disable = false;
    }

    public ConditionalDisableAttribute(string sourceField, int enableValue, bool disable)
    {
        this.sourceField = sourceField;
        this.enableValue = enableValue;
        this.disable = disable;
    }
}