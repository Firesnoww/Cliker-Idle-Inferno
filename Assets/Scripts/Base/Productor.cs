using UnityEngine;

public class Productor : MonoBehaviour
{
    public Datos_Edificios infoEdificio;

    public int          id;
    public string       nombre;
    public float        costoBase;
    public float        produccionBase;
    public float        aumentoCosto;
    public int          cantidad_Edificios;

    private Gestor_Economia gestor;
  
    void Start()
    {   
        gestor = FindObjectOfType<Gestor_Economia>();
        IniciarEdificio();
        if (gestor != null)
        {
            Debug.Log("Encontré el gestor");
        }
        else
        {
            Debug.LogError("No se encontró Gestor_Economia en la escena");
        }
        
        cargarDatos();
    }

    public void IniciarEdificio()
    {
        id = infoEdificio.id;
        nombre= infoEdificio.nombre;
        costoBase= infoEdificio.costoBase;
        produccionBase= infoEdificio.produccionBase;
        aumentoCosto = infoEdificio.aumentoCosto;
    }

    public void Edificasion()
    {
        cantidad_Edificios++;
        PlayerPrefs.SetInt("Edificio " + nombre, cantidad_Edificios);
        
    }
    public void cargarDatos()
    {
        cantidad_Edificios = PlayerPrefs.GetInt("Edificio " + nombre, 0);
    }

    public void reiniciar()
    {
        PlayerPrefs.SetInt("Edificio " + nombre, 0);
    }
    // Update is called once per frame
    void Update()
    {
        gestor.Agonia += (cantidad_Edificios * produccionBase) * Time.deltaTime;
    }

}
