using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;  // Nuevo namespace para URP
using UnityEngine.Rendering.Universal;  // Nuevo namespace para URP

namespace ReadyPlayerMe.XR
{
    public class CameraAnimation : MonoBehaviour
    {
        private const float END_FOCUS_VALUE = 2f;
        private const float POST_PROCCESS_PRIORITY = 100f;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 headViewPoint = new(0f, 0f, 0f);
        [SerializeField] private Vector3 footwearViewPoint = new(0f, 1.66f, 0.85f);
        [SerializeField] private Vector3 bottomsViewPoint = new(0f, 1.4f, 2.6f);
        [SerializeField] private Vector3 bodyViewPoint = new(0f, 1.4f, 2.6f);
        [SerializeField] private Vector3 topViewPoint = new(0f, 0f, 0f);
        [SerializeField] private float defaultDuration = 1f;

        // Referencia al Volume y al efecto DepthOfField de URP
        [SerializeField] private Volume volume;  // Asigna este volumen en el Inspector
        private DepthOfField depthOfField;

        private float duration;
        private float endFocus;
        private bool isTransitioning;
        private float startFocus;
        private Vector3 startPosition;

        private Vector3 targetPosition;
        private float transitionTime;

        private void Start()
        {
            // Si no hay un volumen asignado, intenta obtenerlo del componente adjunto
            if (volume == null)
            {
                volume = GetComponent<Volume>();

                // Si aún no hay volumen, crea uno
                if (volume == null)
                {
                    volume = gameObject.AddComponent<Volume>();
                    volume.isGlobal = true;
                    volume.priority = POST_PROCCESS_PRIORITY;

                    // Crear un nuevo perfil
                    var profile = ScriptableObject.CreateInstance<VolumeProfile>();
                    volume.profile = profile;

                    // Añadir DepthOfField al perfil
                    depthOfField = profile.Add<DepthOfField>(true);
                }
                else
                {
                    // Si ya existe un volumen, asegúrate de que tiene un perfil
                    if (volume.profile == null)
                    {
                        volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
                    }

                    // Intenta obtener el componente DepthOfField o añádelo si no existe
                    if (!volume.profile.TryGet(out depthOfField))
                    {
                        depthOfField = volume.profile.Add<DepthOfField>(true);
                    }
                }
            }
            else if (!volume.profile.TryGet(out depthOfField))
            {
                // Si el volumen ya tiene un perfil pero no tiene DepthOfField, añádelo
                depthOfField = volume.profile.Add<DepthOfField>(true);
            }

            // Configurar el DepthOfField
            depthOfField.active = true;
            depthOfField.mode.value = DepthOfFieldMode.Bokeh;
            depthOfField.focusDistance.value = END_FOCUS_VALUE;
        }

        private void LateUpdate()
        {
            if (!isTransitioning)
            {
                return;
            }

            transitionTime += Time.deltaTime;
            if (transitionTime < duration)
            {
                var t = Mathf.SmoothStep(0.0f, 1.0f, transitionTime / duration);
                cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                depthOfField.focusDistance.value = math.lerp(startFocus, endFocus, t);
            }
            else
            {
                cameraTransform.position = targetPosition;
                isTransitioning = false;
            }
        }

        private void OnDestroy()
        {
            // Limpiar recursos
            if (volume != null && volume.profile != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(volume.profile);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + footwearViewPoint, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + bodyViewPoint, 0.1f);
        }

        public void FocusOnHead()
        {
            StartTransition(footwearViewPoint, defaultDuration, 2f, 0.8f);
            StartTransition(headViewPoint, defaultDuration, startFocus, endFocus);
        }

        public void FocusOnFoot()
        {
            StartTransition(footwearViewPoint, defaultDuration, 2f, 0.8f);
        }

        public void FocusOnBottoms()
        {
            StartTransition(bottomsViewPoint, defaultDuration, 2f, 1.2f);
        }

        public void FocusOnTop()
        {
            StartTransition(topViewPoint, defaultDuration, 2f, 1.2f);
        }

        public void FocusOnBody()
        {
            StartTransition(bodyViewPoint, defaultDuration, 1f, 2f);
        }

        public void StopTransition()
        {
            isTransitioning = false;
        }

        private void StartTransition(Vector3 newTargetLocalPosition, float transitionDuration, float newStartFocus,
            float newEndFocus)
        {
            startFocus = newStartFocus;
            endFocus = newEndFocus;
            startPosition = cameraTransform.position;
            targetPosition = transform.TransformPoint(newTargetLocalPosition);
            duration = transitionDuration;
            transitionTime = 0f;
            isTransitioning = true;
        }
    }
}