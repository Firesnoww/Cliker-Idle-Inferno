using UnityEngine;

public class JugadorVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("Estado")]
    public bool estaMuerto;

    [Header("UI")]
    public MenuDerrota menuDerrota;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        estaMuerto = false;
    }

    public void RecibirDanio(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        Debug.Log("Jugador recibió daño. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;

        Debug.Log("Jugador murió");

        if (menuDerrota != null)
        {
            menuDerrota.MostrarMenuDerrota();
        }
    }
}