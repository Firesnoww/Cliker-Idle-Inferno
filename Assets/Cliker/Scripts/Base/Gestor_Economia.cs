using TMPro;
using UnityEngine;

/// <summary>
/// Gestor_Economia
/// 
/// Este script centraliza toda la economía del juego.
/// 
/// Responsabilidades principales:
/// 1. Guardar la cantidad total de Agonía del jugador.
/// 2. Calcular la producción total por segundo sumando todos los Productores.
/// 3. Sumar Agonía automáticamente con el paso del tiempo.
/// 4. Actualizar el texto de la interfaz.
/// 5. Guardar y cargar la Agonía usando PlayerPrefs.
/// 6. Comprar edificios validando si el jugador tiene suficiente Agonía.
/// 
/// Importante:
/// - Este script será el ÚNICO que modificará la Agonía total de forma global.
/// - Los Productores NO deben sumar Agonía por su cuenta.
/// </summary>
public class Gestor_Economia : MonoBehaviour
{
    [Header("Moneda principal")]
    [Tooltip("Cantidad actual de Agonía del jugador.")]
    [SerializeField] private float agonia;

    [Header("Interfaz")]
    [Tooltip("Texto TMP donde se mostrará la Agonía actual.")]
    [SerializeField] private TMP_Text textAgonia;

    [Header("Productores registrados en la escena")]
    [Tooltip("Arreglo fijo de todos los Productores que existen en el juego. Se asignan manualmente desde el Inspector.")]
    [SerializeField] private Productor[] productores;

    [Header("Debug / Solo lectura")]
    [Tooltip("Producción total por segundo calculada a partir de todos los productores.")]
    [SerializeField] private float produccionTotalPorSegundo;

    /// <summary>
    /// Clave usada para guardar la Agonía total en PlayerPrefs.
    /// </summary>
    private const string CLAVE_AGONIA = "ECONOMIA_AGONIA_TOTAL";

    /// <summary>
    /// Propiedad pública de solo lectura para consultar la Agonía actual desde otros scripts.
    /// 
    /// Ojo:
    /// Se deja de solo lectura para evitar que cualquier script la modifique sin control.
    /// Si otro script necesita cambiar la Agonía, debería hacerlo usando métodos como:
    /// - AgregarAgonia()
    /// - GastarAgonia()
    /// - ComprarProductor()
    /// </summary>
    public float Agonia => agonia;

    /// <summary>
    /// Propiedad pública de solo lectura para consultar la producción total actual del juego.
    /// </summary>
    public float ProduccionTotalPorSegundo => produccionTotalPorSegundo;

    /// <summary>
    /// Start:
    /// Se ejecuta una vez al iniciar el objeto.
    /// 
    /// Aquí:
    /// - Cargamos la Agonía guardada.
    /// - Revisamos referencias importantes.
    /// - Actualizamos el texto al iniciar.
    /// </summary>
    private void Start()
    {
        // Cargar la Agonía guardada previamente.
        CargarDatos();

        // Revisar si las referencias importantes están bien puestas.
        VerificarReferencias();

        // Actualizar la UI apenas inicia el juego.
        ActualizarTextoAgonia();
    }

    /// <summary>
    /// Update:
    /// Se ejecuta una vez por frame.
    /// 
    /// Flujo actual:
    /// 1. Calculamos la producción total sumando todos los Productores.
    /// 2. Sumamos Agonía usando esa producción total y Time.deltaTime.
    /// 3. Actualizamos el texto de la interfaz.
    /// 
    /// Importante:
    /// Aunque esto se ejecuta cada frame, la producción está medida "por segundo"
    /// gracias al uso de Time.deltaTime.
    /// </summary>
    private void Update()
    {
        CalcularProduccionTotal();
        GenerarAgoniaPorTiempo();
        ActualizarTextoAgonia();
    }

    /// <summary>
    /// Recorre todos los productores registrados y suma su producción total.
    /// 
    /// Cada Productor ya sabe calcular cuánto produce según:
    /// - su cantidad comprada
    /// - su producción base
    /// 
    /// Este método simplemente centraliza la suma de todos ellos.
    /// </summary>
    private void CalcularProduccionTotal()
    {
        // Reiniciamos el acumulador antes de volver a sumar.
        produccionTotalPorSegundo = 0f;

        // Si no hay productores asignados, no hacemos nada.
        if (productores == null || productores.Length == 0)
        {
            return;
        }

        // Recorremos el arreglo completo.
        for (int i = 0; i < productores.Length; i++)
        {
            // Si por error un productor del arreglo está vacío, lo saltamos.
            if (productores[i] == null)
            {
                continue;
            }

            // Sumamos la producción de ese productor al total global.
            produccionTotalPorSegundo += productores[i].ObtenerProduccionTotal();
        }
    }

    /// <summary>
    /// Suma Agonía automáticamente con el paso del tiempo.
    /// 
    /// Fórmula:
    /// Agonía += ProducciónTotalPorSegundo * Time.deltaTime
    /// 
    /// Esto hace que si el juego produce, por ejemplo, 10 Agonía por segundo,
    /// esa producción se distribuya correctamente entre los frames.
    /// </summary>
    private void GenerarAgoniaPorTiempo()
    {
        agonia += produccionTotalPorSegundo * Time.deltaTime;
    }

    /// <summary>
    /// Actualiza el texto que muestra la Agonía total del jugador.
    /// 
    /// Ahora usamos el FormateadorNumeros para que los valores grandes
    /// se vean mejor en pantalla.
    /// 
    /// Ejemplos:
    /// 1500 -> 1.5K
    /// 2500000 -> 2.5M
    /// </summary>
    private void ActualizarTextoAgonia()
    {
        // Si no hay texto asignado, salimos para evitar errores.
        if (textAgonia == null)
        {
            return;
        }

        // Mostramos la Agonía usando formato abreviado.
        textAgonia.text = FormateadorNumeros.FormatearNumero(agonia, 1);
    }

    /// <summary>
    /// Devuelve true si el jugador tiene suficiente Agonía para pagar un costo dado.
    /// 
    /// Este método sirve como validación simple antes de una compra.
    /// </summary>
    /// <param name="costo">Costo que se quiere validar.</param>
    public bool PuedeComprar(float costo)
    {
        return agonia >= costo;
    }

    /// <summary>
    /// Intenta gastar una cantidad específica de Agonía.
    /// 
    /// Si alcanza:
    /// - resta la Agonía
    /// - devuelve true
    /// 
    /// Si no alcanza:
    /// - no cambia nada
    /// - devuelve false
    /// </summary>
    /// <param name="cantidad">Cantidad de Agonía que se quiere gastar.</param>
    public bool GastarAgonia(float cantidad)
    {
        // Si el costo es inválido, no hacemos nada.
        if (cantidad <= 0f)
        {
            return false;
        }

        // Si no alcanza la Agonía, la compra o gasto falla.
        if (agonia < cantidad)
        {
            return false;
        }

        // Si sí alcanza, restamos la cantidad.
        agonia -= cantidad;

        // Actualizamos UI y guardamos.
        ActualizarTextoAgonia();
        GuardarDatos();

        return true;
    }

    /// <summary>
    /// Suma Agonía manualmente.
    /// 
    /// Este método sirve para:
    /// - clicks manuales
    /// - recompensas
    /// - bonos
    /// - pruebas
    /// </summary>
    /// <param name="cantidad">Cantidad de Agonía a sumar.</param>
    public void AgregarAgonia(float cantidad)
    {
        // Si la cantidad no es válida, no hacemos nada.
        if (cantidad <= 0f)
        {
            return;
        }

        agonia += cantidad;

        // Actualizamos UI y guardamos.
        ActualizarTextoAgonia();
        GuardarDatos();
    }

    /// <summary>
    /// Método puente para permitir que un botón de Unity compre un Productor.
    /// 
    /// Este método existe porque el sistema OnClick() de Unity solo muestra
    /// métodos públicos que devuelven void.
    /// 
    /// Nuestro método principal de compra devuelve bool:
    ///     ComprarProductor(Productor productor)
    /// 
    /// Por eso no aparece directamente en el Inspector del botón.
    /// 
    /// Esta función simplemente llama al método real de compra,
    /// pero sin devolver nada, para que Unity sí la muestre en el botón.
    /// </summary>
    /// <param name="productor">
    /// Productor que se quiere comprar desde el botón.
    /// </param>
    public void ComprarProductorDesdeBoton(Productor productor)
    {
        ComprarProductor(productor);
    }

    /// <summary>
    /// Intenta comprar una unidad de un Productor específico.
    /// 
    /// Flujo:
    /// 1. Verifica que exista el Productor.
    /// 2. Consulta cuánto cuesta actualmente.
    /// 3. Revisa si el jugador tiene suficiente Agonía.
    /// 4. Si alcanza, descuenta la Agonía y compra el edificio.
    /// 5. Guarda y actualiza interfaz.
    /// 
    /// Importante:
    /// Aquí es donde ya se centraliza la compra.
    /// Es decir, el Productor NO se compra solo:
    /// el Gestor_Economia autoriza la compra.
    /// </summary>
    /// <param name="productor">El productor que se quiere comprar.</param>
    /// <returns>Devuelve true si la compra se realizó, false si falló.</returns>
    public bool ComprarProductor(Productor productor)
    {
        // Validamos que el Productor exista.
        if (productor == null)
        {
            Debug.LogError("Se intentó comprar un Productor nulo.");
            return false;
        }

        // Obtenemos el costo actual de ese Productor.
        float costoActual = productor.ObtenerCostoActual();

        // Si no hay suficiente Agonía, la compra falla.
        if (!PuedeComprar(costoActual))
        {
            Debug.Log("No hay suficiente Agonía para comprar: " + productor.Nombre);
            return false;
        }

        // Restamos la Agonía necesaria.
        agonia -= costoActual;

        // Le decimos al Productor que aumente su cantidad comprada.
        productor.ComprarEdificio();

        // Recalculamos producción, actualizamos UI y guardamos.
        CalcularProduccionTotal();
        ActualizarTextoAgonia();
        GuardarDatos();

        Debug.Log("Compra realizada: " + productor.Nombre + " | Nuevo total: " + productor.CantidadEdificios);

        return true;
    }

    /// <summary>
    /// Guarda la Agonía total actual en PlayerPrefs.
    /// 
    /// Ojo:
    /// La cantidad de cada edificio la sigue guardando su propio Productor.
    /// Este gestor solo guarda la moneda global.
    /// </summary>
    public void GuardarDatos()
    {
        PlayerPrefs.SetFloat(CLAVE_AGONIA, agonia);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carga la Agonía total desde PlayerPrefs.
    /// 
    /// Si no hay nada guardado, empieza en 0.
    /// </summary>
    public void CargarDatos()
    {
        agonia = PlayerPrefs.GetFloat(CLAVE_AGONIA, 0f);
    }

    /// <summary>
    /// Reinicia la economía completa del juego.
    /// 
    /// Hace esto:
    /// 1. Pone la Agonía en 0.
    /// 2. Reinicia cada Productor.
    /// 3. Recalcula la producción total.
    /// 4. Actualiza la interfaz.
    /// 5. Guarda el estado limpio.
    /// 
    /// Este método es útil para:
    /// - pruebas
    /// - botón de reinicio
    /// - reset del save
    /// - futuras mecánicas de prestigio
    /// </summary>
    public void ReiniciarDatos()
    {
        // Reiniciamos la Agonía global.
        agonia = 0f;

        // Reiniciamos todos los productores registrados.
        if (productores != null && productores.Length > 0)
        {
            for (int i = 0; i < productores.Length; i++)
            {
                if (productores[i] == null)
                {
                    continue;
                }

                productores[i].ReiniciarDatos();
            }
        }

        // Recalculamos porque ahora todo debería producir 0.
        CalcularProduccionTotal();

        // Refrescamos la interfaz.
        ActualizarTextoAgonia();

        // Guardamos los cambios.
        GuardarDatos();

        Debug.Log("Economía reiniciada correctamente.");
    }

    /// <summary>
    /// Revisa referencias importantes del script para ayudarte a detectar
    /// errores desde el inicio.
    /// </summary>
    private void VerificarReferencias()
    {
        // Aviso si no se asignó el texto de UI.
        if (textAgonia == null)
        {
            Debug.LogWarning("Gestor_Economia: no se asignó el TMP_Text de Agonía.");
        }

        // Aviso si no hay productores en el arreglo.
        if (productores == null || productores.Length == 0)
        {
            Debug.LogWarning("Gestor_Economia: no hay Productores asignados en el arreglo.");
            return;
        }

        // Revisamos si hay posiciones vacías dentro del arreglo.
        for (int i = 0; i < productores.Length; i++)
        {
            if (productores[i] == null)
            {
                Debug.LogWarning("Gestor_Economia: el Productor en la posición " + i + " está vacío.");
            }
        }
    }

    /// <summary>
    /// Cuando el juego se cierra, guardamos la Agonía automáticamente.
    /// </summary>
    private void OnApplicationQuit()
    {
        GuardarDatos();
    }

    /// <summary>
    /// Cuando la app se pausa (por ejemplo, minimizas el juego),
    /// guardamos la Agonía automáticamente.
    /// </summary>
    /// <param name="pauseStatus">true si la app se pausó.</param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GuardarDatos();
        }
    }
}