using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
public class RPMVRNetworkPlayer : NetworkBehaviour
{
    [Header("RPM Avatar Referencias")]
    public Transform avatarRoot;        // Objeto raíz del avatar RPM
    public Transform avatarHips;        // Componente Hips del Armature
    public Transform avatarHead;        // Renderer_Head o un transform específico para la cabeza
    public Transform leftHandBone;      // Hueso de la mano izquierda en el Armature
    public Transform rightHandBone;     // Hueso de la mano derecha en el Armature

    [Header("VR Referencias")]
    public Transform xrOrigin;          // XR Origin (XR Rig)
    public Transform xrCamera;          // Main Camera dentro de XR Origin
    public Transform leftController;    // Controlador izquierdo
    public Transform rightController;   // Controlador derecho

    [Header("Offsets de Sincronización")]
    public Vector3 headPositionOffset = Vector3.zero;
    public Vector3 headRotationOffset = Vector3.zero;
    public Vector3 handPositionOffset = Vector3.zero;
    public Vector3 handRotationOffset = Vector3.zero;

    [Header("Configuración")]
    public bool syncBodyRotationWithHead = true;
    public float positionThreshold = 0.001f;  // Umbral para enviar actualizaciones
    public float rotationThreshold = 0.1f;    // Umbral de ángulo para rotación

    // Variables de red
    private NetworkVariable<Vector3> networkBodyPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkBodyRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> networkHeadPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkHeadRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> networkLeftHandPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkLeftHandRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> networkRightHandPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRightHandRotation = new NetworkVariable<Quaternion>();

    // Componentes del avatar
    private Animator avatarAnimator;

    void Start()
    {
        // Obtener el Animator si existe
        avatarAnimator = avatarRoot.GetComponentInChildren<Animator>();

        if (IsOwner)
        {
            if (xrOrigin == null)
                xrOrigin = FindAnyObjectByType<XROrigin>()?.transform;
                xrOrigin = FindFirstObjectByType<XROrigin>()?.transform;

            if (xrCamera == null && xrOrigin != null)
                xrCamera = xrOrigin.GetComponentInChildren<Camera>()?.transform;

            if (leftController == null)
                leftController = GameObject.FindGameObjectWithTag("LeftHand")?.transform;

            if (rightController == null)
                rightController = GameObject.FindGameObjectWithTag("RightHand")?.transform;

            // Configurar para jugador local
            SetupLocalPlayer();
        }
        else
        {
            // Configurar para jugador remoto
            SetupRemotePlayer();
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            // Actualizar posiciones de red si somos el propietario
            UpdateNetworkFromLocalVR();
        }
        else
        {
            // Actualizar visual del avatar para jugadores remotos
            UpdateVisualFromNetwork();
        }
    }

    private void SetupLocalPlayer()
    {
        // Para el jugador local (dueño del avatar)
        Debug.Log("Configurando jugador local VR");

        // Opcionalmente, ocultar la cabeza del avatar para evitar obstrucciones con la cámara
        Transform headRenderer = avatarRoot.Find("Renderer_Head");
        if (headRenderer != null)
        {
            headRenderer.gameObject.SetActive(false);
        }

        // Configurar otros componentes específicos del avatar si es necesario
    }

    private void SetupRemotePlayer()
    {
        Debug.Log("Configurando jugador remoto");

        // Para jugadores remotos asegurarse que toda la parte visual está activada
        var renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }

        // Desactivar componentes de interacción que no deben funcionar en jugadores remotos
        var interactors = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        foreach (var interactor in interactors)
        {
            interactor.enabled = false;
        }
    }

    private void UpdateNetworkFromLocalVR()
    {
        if (!IsSpawned || xrOrigin == null || xrCamera == null)
            return;

        // Calcular posición del cuerpo (mantener la altura Y del avatar)
        Vector3 bodyPosition = new Vector3(xrOrigin.position.x, avatarRoot.position.y, xrOrigin.position.z);

        // Calcular rotación del cuerpo (solo en el eje Y si sincronizamos con la cabeza)
        Quaternion bodyRotation;
        if (syncBodyRotationWithHead && xrCamera != null)
        {
            Vector3 flatForward = xrCamera.forward;
            flatForward.y = 0;
            flatForward.Normalize();
            bodyRotation = Quaternion.LookRotation(flatForward);
        }
        else
        {
            bodyRotation = avatarRoot.rotation;
        }

        // Enviar actualizaciones al servidor solo si hay cambios significativos
        if (Vector3.Distance(networkBodyPosition.Value, bodyPosition) > positionThreshold ||
            Quaternion.Angle(networkBodyRotation.Value, bodyRotation) > rotationThreshold)
        {
            UpdateBodyTransformServerRpc(bodyPosition, bodyRotation);
        }

        // Actualizar posición de la cabeza
        if (xrCamera != null && avatarHead != null)
        {
            Vector3 headPosition = xrCamera.position + headPositionOffset;
            Quaternion headRotation = xrCamera.rotation * Quaternion.Euler(headRotationOffset);

            if (Vector3.Distance(networkHeadPosition.Value, headPosition) > positionThreshold ||
                Quaternion.Angle(networkHeadRotation.Value, headRotation) > rotationThreshold)
            {
                UpdateHeadTransformServerRpc(headPosition, headRotation);
            }
        }

        // Actualizar posición de las manos
        if (leftController != null && leftHandBone != null)
        {
            Vector3 leftHandPosition = leftController.position + handPositionOffset;
            Quaternion leftHandRotation = leftController.rotation * Quaternion.Euler(handRotationOffset);

            if (Vector3.Distance(networkLeftHandPosition.Value, leftHandPosition) > positionThreshold ||
                Quaternion.Angle(networkLeftHandRotation.Value, leftHandRotation) > rotationThreshold)
            {
                UpdateLeftHandTransformServerRpc(leftHandPosition, leftHandRotation);
            }
        }

        if (rightController != null && rightHandBone != null)
        {
            Vector3 rightHandPosition = rightController.position + handPositionOffset;
            Quaternion rightHandRotation = rightController.rotation * Quaternion.Euler(handRotationOffset);

            if (Vector3.Distance(networkRightHandPosition.Value, rightHandPosition) > positionThreshold ||
                Quaternion.Angle(networkRightHandRotation.Value, rightHandRotation) > rotationThreshold)
            {
                UpdateRightHandTransformServerRpc(rightHandPosition, rightHandRotation);
            }
        }
    }

    private void UpdateVisualFromNetwork()
    {
        // Actualizar el avatar visual basado en los valores de red
        if (avatarRoot != null)
        {
            avatarRoot.position = networkBodyPosition.Value;
            avatarRoot.rotation = networkBodyRotation.Value;
        }

        if (avatarHead != null)
        {
            // En lugar de mover directamente la cabeza, podríamos usar IK
            // si el avatar tiene un sistema IK, o actualizar el hueso directamente
            avatarHead.position = networkHeadPosition.Value;
            avatarHead.rotation = networkHeadRotation.Value;
        }

        if (leftHandBone != null)
        {
            leftHandBone.position = networkLeftHandPosition.Value;
            leftHandBone.rotation = networkLeftHandRotation.Value;
        }

        if (rightHandBone != null)
        {
            rightHandBone.position = networkRightHandPosition.Value;
            rightHandBone.rotation = networkRightHandRotation.Value;
        }
    }

    #region ServerRPCs
    [ServerRpc]
    private void UpdateBodyTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        networkBodyPosition.Value = position;
        networkBodyRotation.Value = rotation;
    }

    [ServerRpc]
    private void UpdateHeadTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        networkHeadPosition.Value = position;
        networkHeadRotation.Value = rotation;
    }

    [ServerRpc]
    private void UpdateLeftHandTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        networkLeftHandPosition.Value = position;
        networkLeftHandRotation.Value = rotation;
    }

    [ServerRpc]
    private void UpdateRightHandTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        networkRightHandPosition.Value = position;
        networkRightHandRotation.Value = rotation;
    }
    #endregion
}
