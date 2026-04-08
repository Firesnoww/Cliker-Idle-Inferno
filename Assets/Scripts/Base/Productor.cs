using UnityEngine;

/// <summary>
/// Este script representa UN tipo de productor/edificio dentro del juego.
/// 
/// Su responsabilidad es:
/// - Tener una referencia al ScriptableObject con los datos base del edificio.
/// - Guardar y cargar cuántas unidades de este edificio tiene el jugador.
/// - Calcular el costo actual del siguiente edificio.
/// - Calcular la producción total de este edificio.
/// - Comprar una unidad de este edificio.
/// 
/// Importante:
/// Este script YA NO suma Agonía directamente.
/// Más adelante eso lo hará el Gestor_Economia de forma centralizada.
/// </summary>
public class Productor : MonoBehaviour
{
    [Header("Datos base del edificio")]
    [Tooltip("ScriptableObject que contiene los datos fijos del edificio.")]
    public Datos_Edificios infoEdificio;

    [Header("Estado actual del edificio")]
    [Tooltip("Cantidad de edificios comprados por el jugador.")]
    [SerializeField] private int cantidadEdificios;

    /// <summary>
    /// Esta propiedad devuelve el ID del edificio leyendo directamente
    /// desde el ScriptableObject.
    /// </summary>
    public int Id
    {
        get
        {
            // Si por algún motivo no hay ScriptableObject asignado,
            // devolvemos -1 para marcar error.
            if (infoEdificio == null) return -1;
            return infoEdificio.id;
        }
    }

    /// <summary>
    /// Devuelve el nombre del edificio desde el ScriptableObject.
    /// </summary>
    public string Nombre
    {
        get
        {
            if (infoEdificio == null) return "Sin Datos";
            return infoEdificio.nombre;
        }
    }

    /// <summary>
    /// Devuelve el costo base del edificio desde el ScriptableObject.
    /// </summary>
    public float CostoBase
    {
        get
        {
            if (infoEdificio == null) return 0f;
            return infoEdificio.costoBase;
        }
    }

    /// <summary>
    /// Devuelve la producción base del edificio desde el ScriptableObject.
    /// </summary>
    public float ProduccionBase
    {
        get
        {
            if (infoEdificio == null) return 0f;
            return infoEdificio.produccionBase;
        }
    }

    /// <summary>
    /// Devuelve el factor de aumento del costo desde el ScriptableObject.
    /// Por ejemplo: 1.15 significa que cada compra aumenta el precio un 15%.
    /// </summary>
    public float AumentoCosto
    {
        get
        {
            if (infoEdificio == null) return 1f;
            return infoEdificio.aumentoCosto;
        }
    }

    /// <summary>
    /// Permite leer la cantidad comprada desde otros scripts,
    /// pero evita que se cambie directamente desde afuera.
    /// </summary>
    public int CantidadEdificios => cantidadEdificios;

    /// <summary>
    /// Clave única para PlayerPrefs.
    /// 
    /// Usamos el ID del edificio, no el nombre, porque:
    /// - El nombre puede cambiar.
    /// - El ID debe ser único y estable.
    /// </summary>
    private string ClaveGuardadoCantidad
    {
        get
        {
            return "EDIFICIO_CANTIDAD_" + Id;
        }
    }

    private void Start()
    {
        // Al iniciar, primero revisamos que exista un ScriptableObject asignado.
        if (infoEdificio == null)
        {
            Debug.LogError("Productor sin Datos_Edificios asignado en el objeto: " + gameObject.name);
            return;
        }

        // Cargamos la cantidad comprada desde PlayerPrefs.
        CargarDatos();
    }

    /// <summary>
    /// Devuelve la producción total de ESTE edificio.
    /// 
    /// Fórmula actual:
    /// cantidad comprada * producción base
    /// 
    /// Más adelante aquí mismo podemos meter multiplicadores,
    /// upgrades, sinergias, etc.
    /// </summary>
    public float ObtenerProduccionTotal()
    {
        return cantidadEdificios * ProduccionBase;
    }

    /// <summary>
    /// Calcula el costo del SIGUIENTE edificio que se va a comprar.
    /// 
    /// Fórmula:
    /// costoBase * (aumentoCosto ^ cantidadComprada)
    /// 
    /// Ejemplo:
    /// si el costo base es 10 y ya tengo 3 comprados:
    /// 10 * (1.15 ^ 3)
    /// </summary>
    public float ObtenerCostoActual()
    {
        float costoCalculado = CostoBase * Mathf.Pow(AumentoCosto, cantidadEdificios);

        // Redondeamos hacia arriba para que el costo nunca se quede en decimales raros
        // si más adelante quieres mostrarlo como número entero.
        return Mathf.Ceil(costoCalculado);
    }

    /// <summary>
    /// Suma una unidad a la cantidad de edificios comprados
    /// y guarda inmediatamente el dato.
    /// 
    /// Ojo:
    /// Este método NO valida si hay suficiente Agonía.
    /// Esa validación la hará después el Gestor_Economia.
    /// </summary>
    public void ComprarEdificio()
    {
        cantidadEdificios++;
        GuardarDatos();
    }

    /// <summary>
    /// Guarda en PlayerPrefs la cantidad actual de edificios comprados.
    /// </summary>
    public void GuardarDatos()
    {
        PlayerPrefs.SetInt(ClaveGuardadoCantidad, cantidadEdificios);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carga desde PlayerPrefs la cantidad de edificios comprados.
    /// 
    /// Si no existe nada guardado todavía, empieza en 0.
    /// </summary>
    public void CargarDatos()
    {
        cantidadEdificios = PlayerPrefs.GetInt(ClaveGuardadoCantidad, 0);
    }

    /// <summary>
    /// Reinicia este edificio a 0 unidades compradas
    /// y guarda el cambio.
    /// </summary>
    public void ReiniciarDatos()
    {
        cantidadEdificios = 0;
        GuardarDatos();
    }
}