using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Penny
{
    public class LessonPropTFParams
    {
        public Vector3 Postion { get; set; } = Vector3.zero;
        public Vector3 Rotation { get; set; } = Vector3.zero;
        public Vector3 Scale { get; set; } = Vector3.zero;
    }


    public enum SoundLevel {
        None = 0,
        Once = 10,
        Loop = 50,
        Talk = 100,
    }
}