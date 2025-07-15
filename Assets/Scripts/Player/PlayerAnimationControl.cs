using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enum
{
    public enum PlayerState
    {
        Idle = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        UpLeft = 5,
        UpRight = 6,
        DownLeft = 7,
        DownRight = 8
    }
}

public class PlayerAnimationControl : MonoBehaviour
{
    private Animator anim;

    private Enum.PlayerState state;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
       
    }

    public void SetState( Enum.PlayerState _state )
    {
        state = _state;
        anim.SetInteger( "State",(int) state );
    }

    
}
