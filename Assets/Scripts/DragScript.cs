using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragScript : MonoBehaviour {

    public Transform Target;

    new private Camera camera;

    private bool swiping;

    private Vector3 startOfSwipe;

    private const float globeRadius = 20f;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();

        LookAtTarget();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {

            if (!swiping)
            {
                swiping = true;
                startOfSwipe = Input.mousePosition;
            }

            // distance of camera from center
            float magnitude = transform.position.magnitude;

            // camera position on unit sphere
            Vector3 oldPosition = transform.position / magnitude;

            // computing swipe in screen coordinates
            Vector3 startOfSwipe3 = new Vector3(startOfSwipe.x, startOfSwipe.y, magnitude - globeRadius);
            Vector3 endOfSwipe3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, magnitude - globeRadius);
            startOfSwipe = Input.mousePosition;

            // computing swipe vector in world coordinates
            Vector3 startOfSwipeWorld = camera.ScreenToViewportPoint(startOfSwipe3);
            Vector3 endOfSwipeWorld = camera.ScreenToViewportPoint(endOfSwipe3);
            Vector3 mouseSwipeWorld = startOfSwipeWorld - endOfSwipeWorld;

            if (mouseSwipeWorld.magnitude > 0)
            {

                // radians equal to axial distance
                float dragLat = mouseSwipeWorld.y;
                float dragLng = mouseSwipeWorld.x;



                // latitude of camera
                float lat = Mathf.Asin(oldPosition.y);

                // radius of parallel slice
                float parallelRadius = Mathf.Sqrt(Mathf.Pow(oldPosition.x, 2) + Mathf.Pow(oldPosition.z, 2));

                // longitude of camera
                float lng = Mathf.Atan2(oldPosition.z / parallelRadius, oldPosition.x / parallelRadius);

                // updating camera latitude and longitude
                lat += dragLat;
                lng += dragLng;

                // fixing pole problem
                if (lat * 180 / Mathf.PI > 80)
                {
                    lat = 80 * Mathf.PI / 180;
                }
                else if (lat * 180 / Mathf.PI < -80)
                {
                    lat = -80 * Mathf.PI / 180;
                }

                // converting latitude and logitude to 3d coordinates                
                float y = Mathf.Sin(lat);
                parallelRadius = Mathf.Cos(lat);

                float x = parallelRadius * Mathf.Cos(lng);
                float z = parallelRadius * Mathf.Sin(lng);

                // restoring magnitude
                Vector3 newPosition = new Vector3(x, y, z);
                newPosition *= magnitude;

                transform.position = newPosition;

                LookAtTarget();
            }


        }
        else
        {
            if (swiping)
            {
                swiping = false;
            }

        }
    }

    private void LookAtTarget()
    {
        transform.LookAt(Target);
    }
}
