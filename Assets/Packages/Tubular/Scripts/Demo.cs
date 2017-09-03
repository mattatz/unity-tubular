using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Curve;

namespace Tubular {

	[RequireComponent (typeof(CurveTester), typeof(MeshFilter), typeof(MeshRenderer))]
	public class Demo : MonoBehaviour {

		[SerializeField] protected int tubularSegments = 20;
		[SerializeField] protected float radius = 0.1f;
		[SerializeField] protected int radialSegments = 6;
		[SerializeField] protected bool closed = false;

		void Start () {
			var tester = GetComponent<CurveTester>();
			var curve = tester.Build();

			var filter = GetComponent<MeshFilter>();
			filter.sharedMesh = Tubular.Build(curve, tubularSegments, radius, radialSegments, closed);
		}

	}
		
}

