using UnityEngine;

public class JugadorOrientacionMouse : MonoBehaviour
{
    [Header("Cámara principal")]
    public Camera camaraPrincipal;

    [Header("Partes que deben mirar hacia el mouse")]
    public Transform cuerpo;
    public Transform manos;

    [Header("Animator del cuerpo")]
    public Animator animatorCuerpo;

    [Header("Animaciones del cuerpo")]
    public string animacionIdleCuerpo = "Idle";
    public string animacionCaminarCuerpo = "Walk";

    [Header("Rotación")]
    public float velocidadRotacion = 15f;
    public bool rotacionSuave = true;

    [Header("Corrección de orientación")]
    public float correccionRotacionY = 0f;

    [Header("Movimiento / Animación")]
    public bool animarMovimiento = true;
    public float sensibilidadMovimiento = 0.1f;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private Vector3 direccionActual = Vector3.forward;
    private string animacionActualCuerpo;

    private void Awake()
    {
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }

        if (animatorCuerpo == null && cuerpo != null)
        {
            animatorCuerpo = cuerpo.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        ReproducirAnimacionCuerpo(animacionIdleCuerpo);
    }

    private void Update()
    {
        ActualizarDireccionMouse();
        RotarPartes();

        if (animarMovimiento)
        {
            ActualizarAnimacionMovimiento();
        }
    }

    private void ActualizarDireccionMouse()
    {
        if (camaraPrincipal == null) return;

        Ray rayo = camaraPrincipal.ScreenPointToRay(Input.mousePosition);

        // Creamos un plano horizontal a la altura del jugador.
        Plane planoSuelo = new Plane(Vector3.up, transform.position);

        if (planoSuelo.Raycast(rayo, out float distancia))
        {
            Vector3 puntoMouseEnMundo = rayo.GetPoint(distancia);

            Vector3 direccion = puntoMouseEnMundo - transform.position;
            direccion.y = 0f;

            if (direccion.sqrMagnitude > 0.001f)
            {
                direccionActual = direccion.normalized;
            }
        }
    }

    private void RotarPartes()
    {
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionActual);

        // Sirve por si el modelo está mirando de lado o de espaldas.
        rotacionObjetivo *= Quaternion.Euler(0f, correccionRotacionY, 0f);

        RotarObjeto(cuerpo, rotacionObjetivo);
        RotarObjeto(manos, rotacionObjetivo);
    }

    private void RotarObjeto(Transform objeto, Quaternion rotacionObjetivo)
    {
        if (objeto == null) return;

        if (rotacionSuave)
        {
            objeto.rotation = Quaternion.Slerp(
                objeto.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }
        else
        {
            objeto.rotation = rotacionObjetivo;
        }
    }

    private void ActualizarAnimacionMovimiento()
    {
        bool seEstaMoviendo = EstaPresionandoMovimiento();

        if (seEstaMoviendo)
        {
            ReproducirAnimacionCuerpo(animacionCaminarCuerpo);
        }
        else
        {
            ReproducirAnimacionCuerpo(animacionIdleCuerpo);
        }
    }

    private bool EstaPresionandoMovimiento()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        return Mathf.Abs(horizontal) > sensibilidadMovimiento ||
               Mathf.Abs(vertical) > sensibilidadMovimiento;
    }

    private void ReproducirAnimacionCuerpo(string nombreAnimacion)
    {
        if (animatorCuerpo == null) return;
        if (string.IsNullOrEmpty(nombreAnimacion)) return;
        if (animacionActualCuerpo == nombreAnimacion) return;

        animacionActualCuerpo = nombreAnimacion;

        animatorCuerpo.Play(nombreAnimacion, 0, 0f);
    }

    public Vector3 ObtenerDireccionActual()
    {
        return direccionActual;
    }

    private void OnDrawGizmos()
    {
        if (!mostrarDebug) return;

        Gizmos.DrawRay(transform.position, direccionActual * 2f);
    }
}