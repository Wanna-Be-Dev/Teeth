using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int solvedCount;

    public bool[] teethState;
    public Vector3[] positions;

    public GameData()
    {
        this.solvedCount = 0;
        this.teethState = new bool[28];
        this.positions = new Vector3[28];
    }

}
