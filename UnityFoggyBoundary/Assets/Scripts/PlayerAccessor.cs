using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VDV.FoggyBoundary
{
    public abstract class PlayerAccessor : MonoBehaviour
    {
        public abstract float WalkSpeed { get; set; }
        public abstract float RunSpeed { get; set; }
        public abstract Vector3 MoveDir { get; set; }
    }

}