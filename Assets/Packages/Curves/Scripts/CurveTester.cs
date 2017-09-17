using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Curve {

	public enum CurveType {
		CatmullRom
	};

	public class CurveTester : MonoBehaviour {

		public List<Vector3> Points { get { return points; } }

		[SerializeField] protected CurveType type = CurveType.CatmullRom;
		[SerializeField] protected List<Vector3> points;
		[SerializeField] protected float unit = 0.1f;
		[SerializeField] protected bool point = true, tangent = true, frame = false;

		protected Curve curve;
		protected List<FrenetFrame> frames;

		void OnEnable () {
			if(points.Count <= 0) {
				points = new List<Vector3>() {
					Vector3.zero,
					Vector3.up,
					Vector3.right
				};
			}
		}

		void Start () {
			Setup();
		}

		public void AddPoint (Vector3 p) {
			points.Add(p);
		}

		public void Setup () {
			curve = Build();
		}

		public Curve Build() {
			Curve curve = default(Curve);
			switch(type) {
				case CurveType.CatmullRom:
					curve = new CatmullRomCurve(points);
					break;
				default:
					Debug.LogWarning("CurveType is not defined.");
					break;
			}
			return curve;
		}

		void OnDrawGizmos () {
			if(curve == null) {
				Setup();
			}
			DrawGizmos();
		}

		void DrawGizmos () {
			const float delta = 0.01f;
			float dunit = unit * 2f;
			var count = Mathf.FloorToInt(1f / delta);

			if(frames == null) {
				frames = curve.ComputeFrenetFrames(count, false);
			}

			Gizmos.matrix = transform.localToWorldMatrix;
			for(int i = 0; i < count; i++) {
				var t = i * delta;
				var p = curve.GetPointAt(t);

				if(point) {
					Gizmos.color = Color.white;
					Gizmos.DrawSphere(p, unit);
				}

				if(tangent) {
					var t1 = curve.GetTangentAt(t);
					var n1 = (t1 + Vector3.one) * 0.5f;
					Gizmos.color = new Color(n1.x, n1.y, n1.z); 
					Gizmos.DrawLine(p, p + t1 * dunit);
				}

				#if UNITY_EDITOR
				Handles.matrix = transform.localToWorldMatrix;
				Handles.Label(p, i.ToString());
				#endif

				if(frame) {
					var fr = frames[i];

					Gizmos.color = Color.blue;
					Gizmos.DrawLine(p, p + fr.Tangent * dunit);

					Gizmos.color = Color.green;
					Gizmos.DrawLine(p, p + fr.Normal * dunit);

					Gizmos.color = Color.red;
					Gizmos.DrawLine(p, p + fr.Binormal * dunit);
				}
			}
		}

	}
		
}

