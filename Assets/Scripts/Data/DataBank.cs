using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBank", menuName = "Data Storage/_DataBank")]
public class DataBank : ScriptableObject
{
    public PlayerData playerData;
    public UIData uiData;
}
