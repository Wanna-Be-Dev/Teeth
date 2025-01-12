using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class MinMaxAttribute : PropertyAttribute
{
    public float min;
    public float max;

    public MinMaxAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}