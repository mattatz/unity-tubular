using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curve {

    public class CubicPoly {

        float c0, c1, c2, c3;

        public CubicPoly() {
        }

        /*
         * Compute coefficients for a cubic polynomial
         *   p(s) = c0 + c1*s + c2*s^2 + c3*s^3
         * such that
         *   p(0) = x0, p(1) = x1
         *  and
         *   p'(0) = t0, p'(1) = t1.
         */
        void Init(float x0, float x1, float t0, float t1) {
            c0 = x0;
            c1 = t0;
            c2 = - 3 * x0 + 3 * x1 - 2 * t0 - t1;
            c3 = 2 * x0 - 2 * x1 + t0 + t1;
        }

        public void InitCatmullRom(float x0, float x1, float x2, float x3, float tension) {
            Init(x1, x2, tension * (x2 - x0), tension * (x3 - x1));
        }

        public void InitNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2) {
            // compute tangents when parameterized in [t1,t2]
            var t1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
            var t2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

            // rescale tangents for parametrization in [0,1]
            t1 *= dt1;
            t2 *= dt1;
            Init(x1, x2, t1, t2);
        }

        public float Calc(float t) {
            var t2 = t * t;
            var t3 = t2 * t;
            return c0 + c1 * t + c2 * t2 + c3 * t3;
        }

    }

    public class CatmullRomCurve : Curve {

        CubicPoly px, py, pz;

        public CatmullRomCurve(List<Vector3> points, bool closed = false) : base(points, closed) {
            px = new CubicPoly();
            py = new CubicPoly();
            pz = new CubicPoly();
        }

        public override Vector3 GetPoint(float t) {
            var points = this.points;
            var l = points.Count;

            var point = (l - (this.closed ? 0 : 1)) * t;
            var intPoint = Mathf.FloorToInt(point);
            var weight = point - intPoint;

            if (this.closed) {
                intPoint += intPoint > 0 ? 0 : (Mathf.FloorToInt(Mathf.Abs(intPoint) / points.Count) + 1) * points.Count;
            } else if (weight == 0 && intPoint == l - 1) {
                intPoint = l - 2;
                weight = 1;
            }

            Vector3 tmp, p0, p1, p2, p3; // 4 points
            if (this.closed || intPoint > 0) {
                p0 = points[(intPoint - 1) % l];
            } else {
                // extrapolate first point
                tmp = (points[0] - points[1]) + points[0];
                p0 = tmp;
            }

            p1 = points[intPoint % l];
            p2 = points[(intPoint + 1) % l];

            if (this.closed || intPoint + 2 < l) {
                p3 = points[(intPoint + 2) % l];
            } else {
                // extrapolate last point
                tmp = (points[l - 1] - points[l - 2]) + points[l - 1];
                p3 = tmp;
            }

            const float tension = 0.5f;
            px.InitCatmullRom(p0.x, p1.x, p2.x, p3.x, tension);
            py.InitCatmullRom(p0.y, p1.y, p2.y, p3.y, tension);
            pz.InitCatmullRom(p0.z, p1.z, p2.z, p3.z, tension);

            return new Vector3(px.Calc(weight), py.Calc(weight), pz.Calc(weight));
        }


    }

}

