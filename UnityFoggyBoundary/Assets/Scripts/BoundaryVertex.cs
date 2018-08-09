using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDV.Spline;

namespace VDV.FoggyBoundary
{
    [Serializable]
    public class BoundaryVertex : LineVertex
    {
        public Vector3 Normal;

        public BoundaryVertex()
        {
            Normal = Vector3.one.normalized;
        }
    } 
}
