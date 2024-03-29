using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    Left = 0,
    Right = 1
}
public enum Orientation
{
    Top = 0,
    Bottom = 1
}
public class Teeth : MonoBehaviour
{
    [SerializeField]
    public bool isCorrect = false;
    [SerializeField]
    public Side side;
    [SerializeField]
    public Orientation orientation;
    [SerializeField]
    public int placement = 0;

    public Vector3 pos;
    public Vector3 rot;

    public void SetOriginalPosition()
    {
        pos = gameObject.transform.position;
        rot = gameObject.transform.eulerAngles;
    }
}

