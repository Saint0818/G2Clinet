using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    Idle = 0,
    Walk = 1,   
    Run = 2,            
    Block = 3,  
    Board = 4,  
    Catch = 5,  
    Defence = 6,    
    Dribble = 7,    
    Dunk = 8,   
    Fall = 9,   
    Hookshot = 10,  
    Jumper = 11,    
    Layup = 12, 
    Pass = 13,  
    Steal = 14, 
    Underdunk = 15, 
    Dunk1 = 17,
    ShootStart = 18,
    Shoot = 19,
    Pass2 = 20,
	AlleyOop_Pass = 21,
	AlleyOop_Dunk = 22
}

public static class PlayerActions
{
    public const string Idle = "StayIdle";
    public const string Walk = "Walk";
    public const string Run = "Run";
    public const string Block = "Block";
    public const string Board = "Board";
    public const string Catch = "Catch";
    public const string Defence = "Defence";
    public const string Dribble = "Dribble";
    public const string Dunk = "Dunk2";
    public const string Underdunk = "Underdunk";
    public const string Fall = "Fall";
    public const string Hookshot = "HookShot";
    public const string Jumper = "ShootStart";
    public const string Layup = "Layup";
    public const string Pass = "Pass";
    public const string Steal = "Steal";
    public const string Gangnamstyle = "gangnamstyle";
    public const string Stay = "Stay";
    public const string FakeJumper = "FakeJumper";
    public const string Shoot = "Shoot";
    public const string ShootStart = "ShootStart";
    public const string Pass2 = "Pass2";
}

public struct TSkillCD
{
    public float MaxTime;
    public float CurrentTimer;
    public bool CanUse;
}

public class TSkillAttribute
{
	public int No;
    public int Kind;
    public float Value;
    public float CDTime;
}

public class PlayerBehaviour : MonoBehaviour
{
    
}