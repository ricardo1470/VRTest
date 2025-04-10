using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class XRDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngleMin = -90f;
    [SerializeField] private float openAngleMax = 0f;
    [SerializeField] private float doorMass = 10f;
    [SerializeField] private float springForce = 20f;
    [SerializeField] private float damperForce = 5f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    [Header("Interaction")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    [SerializeField] private Transform doorHandleTransform;
    
    private Rigidbody doorRigidbody;
    private HingeJoint hingeJoint;
    private bool doorOpened = false;
    
    void Start()
    {
        doorRigidbody = GetComponent<Rigidbody>();
        
        // Si no hay HingeJoint, crearlo
        hingeJoint = GetComponent<HingeJoint>();
        if (hingeJoint == null)
        {
            hingeJoint = gameObject.AddComponent<HingeJoint>();
        }
        
        // Configurar el Rigidbody
        ConfigureRigidbody();
        
        // Configurar el HingeJoint
        ConfigureHingeJoint();
        
        // Configurar el XRGrabInteractable en la manija
        ConfigureGrabInteractable();
        
        Debug.Log("XRDoorController initialized");
    }
    
    private void ConfigureRigidbody()
    {
        if (doorRigidbody != null)
        {
            doorRigidbody.mass = doorMass;
            doorRigidbody.useGravity = false;
            doorRigidbody.isKinematic = false; // Asegúrate de que esto sea false
            doorRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            doorRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            doorRigidbody.constraints = RigidbodyConstraints.FreezePosition; // Mantiene la posición pero permite rotación
            Debug.Log("Rigidbody configured");
        }
        else
        {
            Debug.LogError("No Rigidbody found on the door");
        }
    }
    
    private void ConfigureHingeJoint()
    {
        if (hingeJoint != null)
        {
            // Configura el eje de rotación para una puerta vertical (alrededor del eje Y)
            hingeJoint.axis = new Vector3(0, 1, 0);
            
            // Sitúa el ancla en el borde de la puerta (ajusta según tu modelo)
            hingeJoint.anchor = new Vector3(-0.5f, 0, 0); // Ajusta esto a la posición real de tu bisagra
            
            // Configura los límites
            JointLimits limits = new JointLimits();
            limits.min = openAngleMin;
            limits.max = openAngleMax;
            limits.bounciness = 0.1f;
            hingeJoint.limits = limits;
            hingeJoint.useLimits = true;
            
            // Configura el spring (opcional, para cerrado automático)
            JointSpring spring = new JointSpring();
            spring.spring = springForce;
            spring.damper = damperForce;
            spring.targetPosition = openAngleMax; // Posición cerrada
            hingeJoint.spring = spring;
            hingeJoint.useSpring = true;
            
            Debug.Log("HingeJoint configured with limits: " + openAngleMin + " to " + openAngleMax);
        }
        else
        {
            Debug.LogError("No HingeJoint found or created on the door");
        }
    }
    
    private void ConfigureGrabInteractable()
    {
        // Si no se ha asignado una manija específica, intenta encontrarla
        if (doorHandleTransform == null)
        {
            Transform handleTransform = transform.Find("DoorHandle");
            if (handleTransform != null)
            {
                doorHandleTransform = handleTransform;
                Debug.Log("Found door handle: " + doorHandleTransform.name);
            }
            else
            {
                Debug.LogWarning("No door handle transform found or assigned");
            }
        }
        
        // Si no se ha asignado un XRGrabInteractable, intenta encontrarlo o crearlo
        if (grabInteractable == null && doorHandleTransform != null)
        {
            grabInteractable = doorHandleTransform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = doorHandleTransform.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                Debug.Log("Added XRGrabInteractable to door handle");
            }
            
            // Asegúrate de que la manija tiene un collider
            Collider handleCollider = doorHandleTransform.GetComponent<Collider>();
            if (handleCollider == null)
            {
                BoxCollider newCollider = doorHandleTransform.gameObject.AddComponent<BoxCollider>();
                newCollider.size = new Vector3(0.1f, 0.1f, 0.1f); // Ajusta según el tamaño real
                Debug.Log("Added collider to door handle");
            }
            
            // Configura el comportamiento del interactable
            grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;
            grabInteractable.throwOnDetach = false;
            
            // Añade event listeners
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
            
            Debug.Log("XRGrabInteractable configured");
        }
        else if (grabInteractable == null)
        {
            Debug.LogError("No XRGrabInteractable found or could be created");
        }
    }
    
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Opcional: Hacer que la puerta se sienta más ligera mientras se interactúa
        if (doorRigidbody != null)
        {
            doorRigidbody.mass = doorMass * 0.3f;
            Debug.Log("Door grabbed, reduced mass");
        }
    }
    
    private void OnReleased(SelectExitEventArgs args)
    {
        // Restaura la masa original
        if (doorRigidbody != null)
        {
            doorRigidbody.mass = doorMass;
            Debug.Log("Door released, restored mass");
        }
    }
    
    // Esta función permite abrir la puerta mediante código (por ejemplo, para testeo)
    public void TestOpenDoor()
    {
        if (doorRigidbody != null)
        {
            // Aplica torque para abrir la puerta
            doorRigidbody.AddTorque(transform.up * -500f, ForceMode.Impulse);
            Debug.Log("Test force applied to door");
        }
    }
    
    // Esta función ayuda a visualizar el eje de rotación en el editor
    private void OnDrawGizmos()
    {
        HingeJoint hinge = GetComponent<HingeJoint>();
        if (hinge != null)
        {
            Gizmos.color = Color.green;
            // Dibuja el eje del HingeJoint
            Vector3 anchor = transform.TransformPoint(hinge.anchor);
            Vector3 axis = transform.TransformDirection(hinge.axis);
            Gizmos.DrawSphere(anchor, 0.02f);
            Gizmos.DrawLine(anchor, anchor + axis * 0.5f);
        }
    }
}