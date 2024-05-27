using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    [SerializeField] private float LODDistance = 1024f;
    [SerializeField] private Camera mainCam;

    // portal A
    private GameObject portalA;
    private GameObject viewportA;
    private Transform portalACollider;
    private Camera camA;

    // portal B
    private GameObject portalB;
    private GameObject viewportB;
    private Transform portalBCollider;
    private Camera camB;

    private Camera playerCamera;
    private GameObject playerObject;
    private bool tpPlayer = false; // Here to only teleport the player during Update() for stability (weird behaviour occurred without this)

    // Start is called before the first frame update
    void Start()
    {
        // Get component and child objects
        portalA = transform.Find("PortalA").gameObject;
        portalB = transform.Find("PortalB").gameObject;

        camA = portalB.transform.Find("CameraA").GetComponent<Camera>(); // Cameras are tied to the opposite portal as that is where they are located
        camB = portalA.transform.Find("CameraB").GetComponent<Camera>();

        viewportA = portalA.transform.Find("ViewportA").gameObject;
        viewportB = portalB.transform.Find("ViewportB").gameObject;

        portalACollider = portalA.transform.Find("PortalColliderA");
        portalBCollider = portalB.transform.Find("PortalColliderB");

        // Material and rendertexture setup - These types aren't garbage collected so release them when done!!
        if (camA.targetTexture != null)
		{
            camA.targetTexture.Release(); 
		}

        if (camB.targetTexture != null)
		{
            camB.targetTexture.Release();
		}

        Material newMatA = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        camA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        newMatA.mainTexture = camA.targetTexture;
        viewportA.GetComponent<Renderer>().material = newMatA;

        Material newMatB = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        camB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        newMatB.mainTexture = camB.targetTexture;
        viewportB.GetComponent<Renderer>().material = newMatB;
    }

    // Update is called once per frame
    void Update()
    {
        // Take the player camera offset from portal B and set the portal B's view camera under portal A to reflect that offset
        playerCamera = mainCam;

        // Only do the following if the player is within range:
        // Get distance to closest portal
        float distanceToClosestPortal = Mathf.Max((playerCamera.transform.position - portalA.transform.position).magnitude, (playerCamera.transform.position - portalB.transform.position).magnitude);

        if (distanceToClosestPortal <= LODDistance)
        {
            UpdateCamera(camA, portalA, portalB, viewportA, viewportB);
            UpdateCamera(camB, portalB, portalA, viewportB, viewportA);

            if (tpPlayer)
            {
                TeleportPlayer();
            }
        }
    }

    // Updating each camera individually
    void UpdateCamera(Camera cameraToUpdate, GameObject portal, GameObject otherPortal, GameObject viewPort, GameObject otherViewPort)
    {
        Vector3 playerOffsetFromPortal = playerCamera.transform.position - portal.transform.position;
        Vector3 viewportOffset = (portal.transform.position - viewPort.transform.position);
        Vector3 otherViewportOffset = (otherViewPort.transform.position - otherPortal.transform.position);
        cameraToUpdate.transform.position = otherPortal.transform.position + viewportOffset + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.transform.rotation, otherPortal.transform.rotation);

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * playerCamera.transform.forward;
        cameraToUpdate.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }

    private void TeleportPlayer()
    {
        // Since OnTriggerEnter only detects collisions and doesn't store which of the two portals were collided with, we just work it out here instead
        GameObject enteredPortal = portalA;
        GameObject exitPortal = portalB;
        Transform inCollider = portalACollider;
        Transform outCollider = portalBCollider;

        if ((playerObject.transform.position - portalB.transform.position).sqrMagnitude <= (playerObject.transform.position - portalA.transform.position).sqrMagnitude)
        {
            enteredPortal = portalB;
            exitPortal = portalA;
            inCollider = portalBCollider;
            outCollider = portalACollider;
        }

        // actually teleporting the player, only if we're entering the portal
        Vector3 playerPortalOffset = playerObject.transform.position - inCollider.transform.position;
        if (Vector3.Dot(inCollider.up, playerPortalOffset) < 0f)
        {
            float rotationDiff = Quaternion.Angle(outCollider.transform.rotation, inCollider.transform.rotation) + 180f;
            playerObject.transform.Rotate(Vector3.up, rotationDiff);

            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * playerPortalOffset;
            playerObject.transform.position = outCollider.transform.position + positionOffset;

            Physics.SyncTransforms();
        }
    }

    // Teleporting the player if they step into the colliders
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerObject = other.gameObject;
            tpPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            tpPlayer = false;
        }
    }
}