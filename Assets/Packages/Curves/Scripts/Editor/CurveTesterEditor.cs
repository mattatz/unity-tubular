using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Curve {

	[CustomEditor (typeof(CurveTester))]
	public class CurveTesterEditor : Editor {

		Quaternion handle;

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			if(GUILayout.Button("Add")) {
				var tester = target as CurveTester;
				var last = tester.Points[tester.Points.Count - 1];
				tester.AddPoint(last);
			}
		}

		void OnSceneGUI () {
			var tester = target as CurveTester;
			var points = tester.Points;
			handle = Tools.pivotRotation == PivotRotation.Local ? tester.transform.rotation : Quaternion.identity;
			var transform = tester.transform;

			for(int i = 0, n = points.Count; i < n; i++) {
				var point = transform.TransformPoint(points[i]);
				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, handle);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(tester, "Point");
					EditorUtility.SetDirty(tester);
					points[i] = transform.InverseTransformPoint(point);

					tester.Setup();
				}
			}
		}

	}
		
}

