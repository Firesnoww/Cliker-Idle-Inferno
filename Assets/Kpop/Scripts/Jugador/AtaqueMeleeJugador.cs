using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueMeleeJugador : MonoBehaviour
{
    [Header("Referencias")]
    public JugadorOrientacionMouse orientacionMouse;
    public Transform puntoGolpe;
    public Animator animatorManos;

    [Header("Partícula de golpe")]
    public ParticleSystem particulaGolpe;
    public bool moverParticulaAlCentroDelGolpe = true;

    [Header("Animaciones")]
    public string animacionIdleMelee = "Idle_Melee";
    public string animacionAtaqueMelee = "Attack_Melee";

    [Header("Daño melee")]
    public int danio = 10;
    public float radioGolpe = 0.7f;
    public float distanciaGolpe = 1.2f;
    public float tiempoAntesDelDanio = 0.15f;
    public float duracionAtaque = 0.45f;

    [Header("Control")]
    public KeyCode teclaAtaque = KeyCode.Mouse0;
    public bool meleeActivo = true;

    [Header("Capas")]
    public LayerMask capasEnemigo;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private bool atacando;
    private Vector3 ultimaDireccionGolpe = Vector3.forward;

    private void Start()
    {
        ReproducirIdleMelee();

        if (particulaGolpe != null)
        {
            particulaGolpe.Stop();
        }
    }

    private void Update()
    {
        if (!meleeActivo) return;

        ActualizarDireccionGolpe();

        if (Input.GetKeyDown(teclaAtaque) && !atacando)
        {
            StartCoroutine(RealizarAtaqueMelee());
        }
    }

    private void ActualizarDireccionGolpe()
    {
        if (orientacionMouse != null)
        {
            ultimaDireccionGolpe = orientacionMouse.ObtenerDireccionActual();
        }
        else
        {
            ultimaDireccionGolpe = transform.forward;
        }

        ultimaDireccionGolpe.y = 0f;

        if (ultimaDireccionGolpe.sqrMagnitude > 0.001f)
        {
            ultimaDireccionGolpe.Normalize();
        }
    }

    private IEnumerator RealizarAtaqueMelee()
    {
        atacando = true;

        ReproducirAnimacionAtaque();

        yield return new WaitForSeconds(tiempoAntesDelDanio);

        HacerDanioMelee();
        ReproducirParticulaGolpe();

        float tiempoRestante = duracionAtaque - tiempoAntesDelDanio;

        if (tiempoRestante > 0f)
        {
            yield return new WaitForSeconds(tiempoRestante);
        }

        ReproducirIdleMelee();

        atacando = false;
    }

    private void HacerDanioMelee()
    {
        if (puntoGolpe == null) return;

        Vector3 centroGolpe = ObtenerCentroGolpe();

        Collider[] enemigosDetectados = Physics.OverlapSphere(
            centroGolpe,
            radioGolpe,
            capasEnemigo
        );

        List<EnemigoVida> enemigosGolpeados = new List<EnemigoVida>();

        foreach (Collider collider in enemigosDetectados)
        {
            EnemigoVida enemigo = collider.GetComponentInParent<EnemigoVida>();

            if (enemigo != null && !enemigosGolpeados.Contains(enemigo))
            {
                enemigo.RecibirDanio(danio);
                enemigosGolpeados.Add(enemigo);

                Debug.Log("Golpe melee a: " + enemigo.name);
            }
        }
    }

    private void ReproducirParticulaGolpe()
    {
        if (particulaGolpe == null) return;

        if (moverParticulaAlCentroDelGolpe)
        {
            particulaGolpe.transform.position = ObtenerCentroGolpe();

            Quaternion rotacionGolpe = Quaternion.LookRotation(ultimaDireccionGolpe);
            particulaGolpe.transform.rotation = rotacionGolpe;
        }

        particulaGolpe.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particulaGolpe.Play();
    }

    private Vector3 ObtenerCentroGolpe()
    {
        if (puntoGolpe == null)
        {
            return transform.position;
        }

        return puntoGolpe.position + ultimaDireccionGolpe * distanciaGolpe;
    }

    private void ReproducirIdleMelee()
    {
        if (animatorManos == null) return;

        animatorManos.Play(animacionIdleMelee);
    }

    private void ReproducirAnimacionAtaque()
    {
        if (animatorManos == null) return;

        animatorManos.Play(animacionAtaqueMelee, 0, 0f);
    }

    private void OnDrawGizmos()
    {
        if (!mostrarDebug || puntoGolpe == null) return;

        Vector3 direccionDebug = ultimaDireccionGolpe;

        if (direccionDebug.sqrMagnitude <= 0.001f)
        {
            direccionDebug = transform.forward;
        }

        Vector3 centroGolpe = puntoGolpe.position + direccionDebug.normalized * distanciaGolpe;

        Gizmos.DrawWireSphere(centroGolpe, radioGolpe);
        Gizmos.DrawRay(puntoGolpe.position, direccionDebug.normalized * distanciaGolpe);
    }
}