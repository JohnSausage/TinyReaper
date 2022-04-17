using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementInputs
{
    public Vector2 Direction;// { get; set; }
    public Vector2 StrongDirection { get; set; }
    public bool Jump;// { get; set; }
    public bool JumpEvent { get; set; }

    public bool Shield { get; set; }
}
