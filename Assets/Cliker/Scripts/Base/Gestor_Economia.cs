using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
/// Gestor_Economia
/// 
/// Este script centraliza toda la economía principal del juego.
/// 
/// Responsabilidades actuales:
/// 1. Guardar la Agonía actual del jugador.
/// 2. Guardar la Agonía histórica total generada.
/// 3. Calcular la producción total por segundo sumando todos los Productores.
/// 4. Sumar Agonía automáticamente con el tiempo.
/// 5. Actualizar la interfaz principal.
/// 6. Comprar edificios.
/// 7. Comprar la mejora local de un edificio.
/// 8. Guardar y cargar el progreso económico principal.
/// 
/// Importante:
/// - Este script es el único que debe modificar la Agonía global.
/// - Los Productores no gastan ni suman Agonía por su cuenta.
/// - Los Productores solo exponen datos y progreso propio.
/// </summary>
public class Gestor_Economia : MonoBehaviour
{
    [Header("Moneda principal")]
    [Tooltip("Cantidad actual de Agonía disponible para gastar.")]
    [SerializeField] private double agonia;

    [Tooltip("Cantidad histórica total de Agonía generada en esta partida. No disminuye al gastar.")]
    [SerializeField] private double agoniaHistorica;

    [Header("Interfaz")]
    [Tooltip("Texto TMP donde se mostrará la Agonía actual.")]
    [SerializeField] private TMP_Text textAgonia;

    [Header("Productores registrados en la escena")]
    [Tooltip("Listado de todos los Productores activos en la escena.")]
    [SerializeField] private Productor[] productores;

    [Header("Debug / Solo lectura")]
    [Tooltip("Producción total por segundo calculada a partir de todos los productores.")]
    [SerializeField] private double produccionTotalPorSegundo;

    [Header("Click manual")]
    [Tooltip("Valor base de Agonía que se obtiene por cada click manual.")]
    [SerializeField] private double agoniaBasePorClick = 1d;

    [Tooltip("Multiplicador general aplicado al click manual.")]
    [SerializeField] private double multiplicadorClick = 1d;

    [Tooltip("Bono plano adicional al click manual. Se deja desde ya para mejoras futuras.")]
    [SerializeField] private double bonoPlanoClick = 0d;


    /// <summary>
    /// Clave para guardar la Agonía actual.
    /// Se guarda como texto para evitar perder precisión con números grandes.
    /// </summary>
    private const string CLAVE_AGONIA = "ECONOMIA_AGONIA_TOTAL";

    /// <summary>
    /// Clave para guardar la Agonía histórica.
    /// También se guarda como texto por precisión.
    /// </summary>
    private const string CLAVE_AGONIA_HISTORICA = "ECONOMIA_AGONIA_HISTORICA";

    /// <summary>
    /// Propiedad pública de solo lectura para consultar la Agonía actual.
    /// </summary>
    public double Agonia => agonia;

    /// <summary>
    /// Propiedad pública de solo lectura para consultar la Agonía histórica.
    /// </summary>
    public double AgoniaHistorica => agoniaHistorica;

    /// <summary>
    /// Propiedad pública de solo lectura para consultar la producción total por segundo.
    /// </summary>
    public double ProduccionTotalPorSegundo => produccionTotalPorSegundo;

    /// <summary>
    /// Start:
    /// - Carga el progreso guardado
    /// - Verifica referencias
    /// - Actualiza la UI inicial
    /// </summary>
    private void Start()
    {
        CargarDatos();
        VerificarReferencias();
        ActualizarTextoAgonia();
    }

    /// <summary>
    /// Update:
    /// - Recalcula la producción total
    /// - Genera Agonía por tiempo
    /// - Actualiza la UI principal
    /// </summary>
    private void Update()
    {
        CalcularProduccionTotal();
        GenerarAgoniaPorTiempo();
        ActualizarTextoAgonia();
    }

    /// <summary>
    /// Recorre todos los Productores y suma su producción total actual.
    /// </summary>
    private void CalcularProduccionTotal()
    {
        produccionTotalPorSegundo = 0d;

        if (productores == null || productores.Length == 0)
        {
            return;
        }

        for (int i = 0; i < productores.Length; i++)
        {
            if (productores[i] == null)
            {
                continue;
            }

            produccionTotalPorSegundo += productores[i].ObtenerProduccionTotal();
        }
    }

    /// <summary>
    /// Genera Agonía automáticamente según la producción total por segundo.
    /// 
    /// Importante:
    /// - Suma a la Agonía actual.
    /// - También suma a la Agonía histórica.
    /// </summary>
    private void GenerarAgoniaPorTiempo()
    {
        double agoniaGeneradaEsteFrame = produccionTotalPorSegundo * Time.deltaTime;

        agonia += agoniaGeneradaEsteFrame;
        agoniaHistorica += agoniaGeneradaEsteFrame;
    }

    /// <summary>
    /// Actualiza el texto principal de Agonía usando formato abreviado.
    /// </summary>
    private void ActualizarTextoAgonia()
    {
        if (textAgonia == null)
        {
            return;
        }

        textAgonia.text = FormateadorNumeros.FormatearNumero(agonia, 2);
    }

    /// <summary>
    /// Devuelve true si el jugador tiene suficiente Agonía para pagar un costo.
    /// </summary>
    public bool PuedeComprar(double costo)
    {
        return agonia >= costo;
    }

    /// <summary>
    /// Intenta gastar Agonía.
    /// 
    /// Si alcanza:
    /// - resta la Agonía
    /// - actualiza UI
    /// - guarda
    /// 
    /// Si no alcanza:
    /// - no hace nada
    /// </summary>
    public bool GastarAgonia(double cantidad)
    {
        if (cantidad <= 0d)
        {
            return false;
        }

        if (agonia < cantidad)
        {
            return false;
        }

        agonia -= cantidad;

        ActualizarTextoAgonia();
        GuardarDatos();

        return true;
    }

    /// <summary>
    /// Suma Agonía manualmente.
    /// 
    /// Este método sirve para:
    /// - click manual
    /// - recompensas
    /// - bonos
    /// - pruebas
    /// 
    /// También aumenta la Agonía histórica.
    /// </summary>
    public void AgregarAgonia(double cantidad)
    {
        if (cantidad <= 0d)
        {
            return;
        }

        agonia += cantidad;
        agoniaHistorica += cantidad;

        ActualizarTextoAgonia();
        GuardarDatos();
    }

    /// <summary>
    /// Calcula cuánta Agonía da actualmente un click manual.
    /// 
    /// Fórmula base actual:
    /// (Agonía base por click + bono plano) * multiplicador
    /// 
    /// Esto se deja así desde ahora para que más adelante
    /// podamos conectar:
    /// - mejoras de click
    /// - mejoras globales
    /// - efectos temporales
    /// - prestigio
    /// - gachapón
    /// 
    /// sin tener que rehacer la base.
    /// </summary>
    public double ObtenerAgoniaPorClick()
    {
        double valorClick = (agoniaBasePorClick + bonoPlanoClick) * multiplicadorClick;
        return valorClick;
    }

    /// <summary>
    /// Ejecuta un click manual del jugador.
    /// 
    /// Este método:
    /// - calcula la Agonía que da el click actual
    /// - la suma a la Agonía disponible
    /// - la suma también a la Agonía histórica
    /// - actualiza la interfaz
    /// - guarda el progreso
    /// 
    /// Importante:
    /// Este será el punto base para todas las mejoras futuras del click.
    /// </summary>
    public void HacerClickManual()
    {
        double agoniaGanada = ObtenerAgoniaPorClick();

        agonia += agoniaGanada;
        agoniaHistorica += agoniaGanada;

        ActualizarTextoAgonia();
        GuardarDatos();
    }

    /// <summary>
    /// Método puente para conectar el botón principal del click
    /// desde el inspector de Unity.
    /// </summary>
    public void HacerClickManualDesdeBoton()
    {
        HacerClickManual();
    }

    /// <summary>
    /// Método puente para permitir comprar un Productor desde un botón de Unity.
    /// </summary>
    public void ComprarProductorDesdeBoton(Productor productor)
    {
        ComprarProductor(productor);
    }

    /// <summary>
    /// Intenta comprar una unidad de un Productor específico.
    /// 
    /// Flujo:
    /// 1. Verifica que exista el Productor
    /// 2. Consulta el costo actual
    /// 3. Valida si alcanza la Agonía
    /// 4. Resta la Agonía
    /// 5. Compra el edificio
    /// 6. Recalcula producción
    /// 7. Guarda
    /// </summary>
    public bool ComprarProductor(Productor productor)
    {
        if (productor == null)
        {
            Debug.LogError("Se intentó comprar un Productor nulo.");
            return false;
        }

        double costoActual = productor.ObtenerCostoActual();

        if (!PuedeComprar(costoActual))
        {
            Debug.Log("No hay suficiente Agonía para comprar: " + productor.Nombre);
            return false;
        }

        agonia -= costoActual;
        productor.ComprarEdificio();

        CalcularProduccionTotal();
        ActualizarTextoAgonia();
        GuardarDatos();

        Debug.Log("Compra realizada: " + productor.Nombre + " | Nuevo total: " + productor.CantidadEdificios);

        return true;
    }

    /// <summary>
    /// Método puente para permitir comprar la mejora local desde un botón de Unity.
    /// </summary>
    public void ComprarMejoraLocalDesdeBoton(Productor productor)
    {
        ComprarMejoraLocal(productor);
    }

    /// <summary>
    /// Intenta comprar la siguiente mejora local de un Productor.
    /// 
    /// Flujo:
    /// 1. Verifica que exista el Productor.
    /// 2. Verifica que no tenga el máximo nivel.
    /// 3. Verifica que la mejora ya esté desbloqueada por cantidad.
    /// 4. Consulta el costo de la mejora.
    /// 5. Revisa si alcanza la Agonía.
    /// 6. Compra la mejora.
    /// 7. Recalcula producción, actualiza UI y guarda.
    /// </summary>
    public bool ComprarMejoraLocal(Productor productor)
    {
        if (productor == null)
        {
            Debug.LogError("Se intentó comprar una mejora local de un Productor nulo.");
            return false;
        }

        if (productor.TieneMaximaMejoraLocal())
        {
            Debug.Log("El edificio ya alcanzó su mejora local máxima: " + productor.Nombre);
            return false;
        }

        if (!productor.PuedeDesbloquearSiguienteMejoraLocal())
        {
            Debug.Log("Aún no se desbloquea la siguiente mejora local de: " + productor.Nombre);
            return false;
        }

        double costoMejora = productor.ObtenerCostoSiguienteMejoraLocal();

        if (!PuedeComprar(costoMejora))
        {
            Debug.Log("No hay suficiente Agonía para la mejora local de: " + productor.Nombre);
            return false;
        }

        agonia -= costoMejora;

        bool compraRealizada = productor.ComprarSiguienteMejoraLocal();

        if (!compraRealizada)
        {
            return false;
        }

        CalcularProduccionTotal();
        ActualizarTextoAgonia();
        GuardarDatos();

        Debug.Log("Mejora local comprada: " + productor.Nombre + " | Nuevo nivel: " + productor.NivelMejoraLocal);

        return true;
    }

    /// <summary>
    /// Guarda la Agonía actual y la Agonía histórica.
    /// 
    /// Se guardan como texto para evitar pérdida de precisión
    /// con números grandes.
    /// </summary>
    public void GuardarDatos()
    {
        PlayerPrefs.SetString(CLAVE_AGONIA, agonia.ToString(CultureInfo.InvariantCulture));
        PlayerPrefs.SetString(CLAVE_AGONIA_HISTORICA, agoniaHistorica.ToString(CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carga la Agonía actual y la Agonía histórica.
    /// 
    /// Si no hay datos guardados, ambas empiezan en 0.
    /// </summary>
    public void CargarDatos()
    {
        string agoniaGuardada = PlayerPrefs.GetString(CLAVE_AGONIA, "0");
        string agoniaHistoricaGuardada = PlayerPrefs.GetString(CLAVE_AGONIA_HISTORICA, "0");

        double.TryParse(agoniaGuardada, NumberStyles.Any, CultureInfo.InvariantCulture, out agonia);
        double.TryParse(agoniaHistoricaGuardada, NumberStyles.Any, CultureInfo.InvariantCulture, out agoniaHistorica);
    }

    /// <summary>
    /// Reinicia la economía base del juego.
    /// 
    /// Esto:
    /// - pone la Agonía actual en 0
    /// - pone la Agonía histórica en 0
    /// - reinicia todos los Productores
    /// - recalcula producción
    /// - guarda el estado limpio
    /// 
    /// Más adelante, cuando exista el sistema de prestigio,
    /// este método se podrá separar entre:
    /// - reinicio total
    /// - reinicio de prestigio
    /// </summary>
    public void ReiniciarDatos()
    {
        agonia = 0d;
        agoniaHistorica = 0d;

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

        CalcularProduccionTotal();
        ActualizarTextoAgonia();
        GuardarDatos();

        Debug.Log("Economía reiniciada correctamente.");
    }

    /// <summary>
    /// Verifica referencias importantes del script.
    /// </summary>
    private void VerificarReferencias()
    {
        if (textAgonia == null)
        {
            Debug.LogWarning("Gestor_Economia: no se asignó el TMP_Text de Agonía.");
        }

        if (productores == null || productores.Length == 0)
        {
            Debug.LogWarning("Gestor_Economia: no hay Productores asignados en el arreglo.");
            return;
        }

        for (int i = 0; i < productores.Length; i++)
        {
            if (productores[i] == null)
            {
                Debug.LogWarning("Gestor_Economia: el Productor en la posición " + i + " está vacío.");
            }
        }
    }

    /// <summary>
    /// Guarda automáticamente al cerrar el juego.
    /// </summary>
    private void OnApplicationQuit()
    {
        GuardarDatos();
    }

    /// <summary>
    /// Guarda automáticamente al pausar la aplicación.
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GuardarDatos();
        }
    }
}