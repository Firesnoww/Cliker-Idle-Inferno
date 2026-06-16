using UnityEngine;
using TMPro;

public class SistemaPuntos : MonoBehaviour
{
    public static SistemaPuntos instancia;

    [Header("Puntos")]
    public int puntosActuales;

    [Header("UI")]
    public TMP_Text textoPuntos;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SumarPuntos(int cantidad)
    {
        puntosActuales += cantidad;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = "Puntos: " + puntosActuales;
        }
    }
}