using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class AnimatorEnmu
    {


    }

    public enum PublicState {
        Idle = 0,
        Speak = 50,
        Walk = 100,
        Run = 150,
        CustomAction = 200,
        Die = 1000,
        MoguGrow = 1001,
    }

    public enum Liyw 
    {
        Idle = 0,
        Speak = 50,
        Jump = 100,
        Run = 150,
        Left = 151,
        Right = 152,
        Hit = 501,
        CalHit = 502,
        Die = 1000,
    }
}