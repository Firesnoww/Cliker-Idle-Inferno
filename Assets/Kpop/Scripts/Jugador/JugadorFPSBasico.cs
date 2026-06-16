using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JugadorFPSBasico : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 8f;
    public float fuerzaSalto = 6f;
    public float gravedad = -20f;

    [Header("Mouse / Cámara")]
    public Transform camaraJugador;
    public float sensibilidadMouse = 2f;
    public float limiteMiradaVertical = 80f;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private float rotacionVertical;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (camaraJugador == null)
        {
            Camera camaraEncontrada = GetComponentInChildren<Camera>();

            if (camaraEncontrada != null)
            {
                camaraJugador = camaraEncontrada.transform;
            }
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MirarConMouse();
        MoverJugador();
    }

    private void MirarConMouse()
    {
        if (camaraJugador == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        // El cuerpo gira horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // La cámara gira verticalmente
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -limiteMiradaVertical, limiteMiradaVertical);

        camaraJugador.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
    }

    private void MoverJugador()
    {
        bool estaEnSuelo = controller.isGrounded;

        if (estaEnSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movimiento = transform.right * horizontal + transform.forward * vertical;

        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift);
        float velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;

        controller.Move(movimiento * velocidadActual * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
        }

        velocidadVertical.y += gravedad * Time.deltaTime;

        controller.Move(velocidadVertical * Time.deltaTime);
    }
}