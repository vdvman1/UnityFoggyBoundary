using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDV.Spline;

namespace VDV.FoggyBoundary
{
    public class Boundary : GenericLine<BoundaryVertex>
    {
        public override Helper.LinePoint ClosestPoint(Vector3 pos)
        {
            var min = new Helper.LinePoint { Distance = Mathf.Infinity };
            for (var i = 1; i < points.Count; i++)
            {
                Vector3 a = points[i - 1].Position;
                Vector3 b = points[i].Position;

                Helper.LinePoint point = Helper.PointToLine(pos, a, b, i - 1);
                if (point.Distance < min.Distance)
                {
                    min = point;
                }
            }
            return min;
        }
    } 
}
