using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDerrota : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelDerrota;

    [Header("Pausa")]
    public bool pausarJuegoAlMorir = true;

    private void Awake()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void MostrarMenuDerrota()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        if (pausarJuegoAlMorir)
        {
            Time.timeScale = 0f;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CerrarJuego()
    {
        Time.timeScale = 1f;

        Application.Quit();

        Debug.Log("Cerrar juego");
    }
}