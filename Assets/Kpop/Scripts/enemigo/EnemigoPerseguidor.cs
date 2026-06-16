using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public NavMeshAgent agente;
    public Animator animatorEnemigo;

    [Header("Movimiento")]
    public float distanciaMinima = 1.5f;
    public bool mirarAlJugadorAlAtacar = true;
    public float velocidadRotacionAtaque = 12f;

    [Header("Ataque")]
    public int danio = 10;
    public float tiempoEntreAtaques = 1.2f;
    public float tiempoAntesDelDanio = 0.25f;
    public float duracionAnimacionAtaque = 0.7f;

    [Header("Animaciones")]
    public string animacionIdle = "Idle";
    public string animacionCaminar = "Walk";
    public string animacionAtaque = "Attack";

    [Header("Debug")]
    public bool mostrarDebug = true;

    private JugadorVida vidaJugador;
    private bool atacando;
    private float siguienteAtaque;
    private string animacionActual;

    private void Awake()
    {
        if (agente == null)
        {
            agente = GetComponent<NavMeshAgent>();
        }

        if (animatorEnemigo == null)
        {
            animatorEnemigo = GetComponentInChildren<Animator>();
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

        if (jugador != null)
        {
            vidaJugador = jugador.GetComponent<JugadorVida>();
        }

        ReproducirAnimacion(animacionIdle);
    }

    private void Update()
    {
        if (jugador == null) return;
        if (agente == null) return;
        if (!agente.isOnNavMesh) return;

        if (vidaJugador == null)
        {
            vidaJugador = jugador.GetComponent<JugadorVida>();
        }

        if (vidaJugador != null && vidaJugador.estaMuerto)
        {
            DetenerEnemigo();
            ReproducirAnimacion(animacionIdle);
            return;
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia > distanciaMinima)
        {
            PerseguirJugador();
        }
        else
        {
            PrepararAtaque();
        }
    }

    private void PerseguirJugador()
    {
        if (atacando) return;

        agente.isStopped = false;
        agente.SetDestination(jugador.position);

        ReproducirAnimacion(animacionCaminar);
    }

    private void PrepararAtaque()
    {
        DetenerEnemigo();

        if (mirarAlJugadorAlAtacar)
        {
            RotarHaciaJugador();
        }

        if (!atacando && Time.time >= siguienteAtaque)
        {
            StartCoroutine(AtacarJugador());
        }
        else if (!atacando)
        {
            ReproducirAnimacion(animacionIdle);
        }
    }

    private IEnumerator AtacarJugador()
    {
        atacando = true;
        siguienteAtaque = Time.time + tiempoEntreAtaques;

        DetenerEnemigo();

        ReproducirAnimacion(animacionAtaque);

        yield return new WaitForSeconds(tiempoAntesDelDanio);

        AplicarDanio();

        float tiempoRestante = duracionAnimacionAtaque - tiempoAntesDelDanio;

        if (tiempoRestante > 0f)
        {
            yield return new WaitForSeconds(tiempoRestante);
        }

        atacando = false;
    }

    private void AplicarDanio()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaMinima + 0.2f)
        {
            if (vidaJugador == null)
            {
                vidaJugador = jugador.GetComponent<JugadorVida>();
            }

            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanio(danio);

                if (mostrarDebug)
                {
                    Debug.Log(name + " atacó al jugador. Daño: " + danio);
                }
            }
        }
    }

    private void DetenerEnemigo()
    {
        if (agente == null) return;

        agente.isStopped = true;
        agente.velocity = Vector3.zero;
    }

    private void RotarHaciaJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude <= 0.001f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            velocidadRotacionAtaque * Time.deltaTime
        );
    }

    private void ReproducirAnimacion(string nombreAnimacion)
    {
        if (animatorEnemigo == null) return;
        if (string.IsNullOrEmpty(nombreAnimacion)) return;
        if (animacionActual == nombreAnimacion) return;

        animacionActual = nombreAnimacion;

        animatorEnemigo.Play(nombreAnimacion, 0, 0f);
    }
}