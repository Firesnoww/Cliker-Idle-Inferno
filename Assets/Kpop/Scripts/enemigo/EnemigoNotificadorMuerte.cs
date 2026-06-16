using UnityEngine;

public class EnemigoNotificadorMuerte : MonoBehaviour
{
    public SistemaOleadas sistemaOleadas;
    public bool esJefe;

    public void NotificarMuerte()
    {
        if (sistemaOleadas != null)
        {
            sistemaOleadas.AvisarEnemigoMuerto(esJefe);
        }
    }
}