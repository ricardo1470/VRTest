using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class NetworkInteractable : NetworkBehaviour
{
    private Collider _collider;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;

    private NetworkVariable<bool> isHeld = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private TextMeshProUGUI _statusText;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _statusText = GetComponentInChildren<TextMeshProUGUI>(true); // busca en hijos
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        isHeld.OnValueChanged += OnHeldStateChanged;

        _grabInteractable.selectEntered.AddListener(OnSelectEntered);
        _grabInteractable.selectExited.AddListener(OnSelectExited);

        // Aplicar el estado inicial al UI
        OnHeldStateChanged(false, isHeld.Value);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        isHeld.OnValueChanged -= OnHeldStateChanged;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            NetworkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        }

        SetHeldServerRpc(true);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        SetHeldServerRpc(false);
    }

    private void OnHeldStateChanged(bool previousValue, bool newValue)
    {
        _collider.enabled = !newValue;

        if (_statusText != null)
        {
            _statusText.text = newValue ? "Private" : "Public";
            _statusText.color = newValue ? Color.red : Color.green;
        }
    }

    [ServerRpc]
    void SetHeldServerRpc(bool value)
    {
        isHeld.Value = value;
    }
}
