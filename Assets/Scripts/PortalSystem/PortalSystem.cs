using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    public Transform destinationPortal;
    public float exitOffset = 2f;
    public bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.transform.root.gameObject;

        if ((target.CompareTag("Player") || target.CompareTag("XRRig") || target.CompareTag("XRController") || target.name.Contains("RPM_Template_Avatar"))
            && isActive && destinationPortal != null)
        {
            Debug.Log("[PortalSystem] Activado instantáneo");
            TeleportImmediately(target);
        }
    }

    private void TeleportImmediately(GameObject target)
    {
        Vector3 exitPosition = destinationPortal.position + destinationPortal.forward * exitOffset;
        exitPosition.y = target.transform.position.y;

        target.transform.position = exitPosition;

        Debug.Log($"[PortalSystem] Teleport inmediato a: {exitPosition}");
    }

    private void OnDrawGizmos()
    {
        if (destinationPortal != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, destinationPortal.position);

            Vector3 exitPoint = destinationPortal.position + destinationPortal.forward * exitOffset;
            exitPoint.y = transform.position.y;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(exitPoint, 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(exitPoint, destinationPortal.forward * 2f);
        }
    }
}
