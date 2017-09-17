unity-tubular
=====================

Tubular mesh (tube shape along a curve without torsion) builder for Unity.

![Demo](https://raw.githubusercontent.com/mattatz/unity-tubular/master/Captures/Capture.gif)

## Usage

```cs

using UnityEngine;

// include required packages
using Curve;
using Tubular;

[RequireComponent (typeof(MeshFilter))]
public class Demo : MonoBehaviour {

    void Start () {
        // define Curve with control points
        var controls = new List<Vector3>() {
            new Vector3(-5f, -2f, -5f),
            new Vector3(-4f, 1f, 1.5f),
            new Vector3(2.8f, 1f, 1.2f),
            new Vector3(-4.2f, 5.8f, -2.32f),
            new Vector3(-3f, 0.6f, -5.1f)
        };
        var curve = new CatmullRomCurve(controls);

        // Build tubular mesh with Curve
        int tubularSegments = 50;
        float radius = 0.5f;
        int radialSegments = 8;
        bool closed = false; // closed curve or not
        var mesh = Tubular.Tubular.Build(curve, tubularSegments, radius, radialSegments, closed);

        // visualize mesh
        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
    }

}

```

## Sources

- Parallel Transport Approach to Curve Framing - http://www.cs.indiana.edu/pub/techreports/TR425.pdf
- mrdoob/threejs THREE.TubeGeometry - https://github.com/mrdoob/three.js
- Cinder/samples/Tubular - https://github.com/cinder/Cinder/tree/master/samples/Tubular
- neilmendoza/ofxPtf - https://github.com/neilmendoza/ofxPtf

