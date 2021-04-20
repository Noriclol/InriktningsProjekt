using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShipType", menuName = "Ship/ShipType")]
public class ShipType : ScriptableObject
{
    public new string name;
    public shipCategory category;
    public float mass;
    public const float C_r = 0.8f;
}

public enum shipCategory
{
    SpeedBoat,
    CargoShip
}