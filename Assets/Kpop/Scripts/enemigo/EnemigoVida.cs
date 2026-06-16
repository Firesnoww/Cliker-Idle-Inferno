using System.Collections;
using UnityEngine;

public class EnemigoVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 30;
    public int vidaActual;

    [Header("Puntos al morir")]
    public int puntosAlMorir = 10;

    [Header("Muerte")]
    public float tiempoAntesDeDestruir = 0.5f;

    [Header("Partícula al recibir daño")]
    public ParticleSystem particulaDanio;

    [Header("Opciones de partícula")]
    public bool reproducirParticulaDanio = true;

    private bool muerto;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        muerto = false;

        if (particulaDanio != null)
        {
            particulaDanio.Stop();
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ReproducirParticulaDanio();

        Debug.Log("Enemigo recibió daño. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            StartCoroutine(MorirConEspera());
        }
    }

    private void ReproducirParticulaDanio()
    {
        if (!reproducirParticulaDanio) return;
        if (particulaDanio == null) return;

        particulaDanio.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particulaDanio.Play();
    }

    private IEnumerator MorirConEspera()
    {
        if (muerto) yield break;

        muerto = true;

        // Detenemos movimiento si tiene NavMeshAgent.
        UnityEngine.AI.NavMeshAgent agente = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }

        // Apagamos colliders para que no siga recibiendo golpes ni bloqueando.
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Aquí después podremos activar animación de muerte.
        // Ejemplo futuro:
        // animator.Play("Death");

        yield return new WaitForSeconds(tiempoAntesDeDestruir);

        if (SistemaPuntos.instancia != null)
        {
            SistemaPuntos.instancia.SumarPuntos(puntosAlMorir);
        }

        EnemigoNotificadorMuerte notificador = GetComponent<EnemigoNotificadorMuerte>();

        if (notificador != null)
        {
            notificador.NotificarMuerte();
        }

        Destroy(gameObject);
    }
}