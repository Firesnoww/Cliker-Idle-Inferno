using UnityEngine;
using UnityEngine.AI;

public class EnemigoAtaqueJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public NavMeshAgent agente;

    [Header("Ataque")]
    public int danio = 10;
    public float distanciaAtaque = 1.5f;
    public float tiempoEntreAtaques = 1f;

    [Header("Movimiento")]
    public bool detenerseAlAtacar = true;

    private float tiempoSiguienteAtaque;

    private void Awake()
    {
        if (agente == null)
        {
            agente = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        if (jugador == null)
        {
            GameObject jugadorEncontrado = GameObject.FindGameObjectWithTag("Player");

            if (jugadorEncontrado != null)
            {
                jugador = jugadorEncontrado.transform;
            }
        }
    }

    private void Update()
    {
        if (jugador == null) return;

        JugadorVida vidaJugador = jugador.GetComponent<JugadorVida>();

        if (vidaJugador != null && vidaJugador.estaMuerto)
        {
            if (agente != null)
            {
                agente.isStopped = true;
            }

            return;
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaAtaque)
        {
            IntentarAtacar(vidaJugador);
        }
    }

    private void IntentarAtacar(JugadorVida vidaJugador)
    {
        if (vidaJugador == null) return;

        if (detenerseAlAtacar && agente != null)
        {
            agente.isStopped = true;
        }

        if (Time.time >= tiempoSiguienteAtaque)
        {
            vidaJugador.RecibirDanio(danio);
            tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
        }
    }
}