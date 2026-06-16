using System;
using UnityEngine;

/// <summary>
/// Productor
/// 
/// Este script representa UN edificio comprable dentro del juego.
/// 
/// Responsabilidades actuales:
/// - Referenciar los datos fijos del edificio.
/// - Guardar y cargar la cantidad comprada.
/// - Guardar y cargar el nivel de mejora local.
/// - Calcular el costo actual del siguiente edificio.
/// - Calcular la producción total actual del edificio.
/// - Calcular si la siguiente mejora local ya se puede desbloquear.
/// - Calcular el costo de la siguiente mejora local.
/// 
/// Importante:
/// - Este script NO modifica directamente la Agonía global.
/// - La compra y el gasto global siguen siendo responsabilidad del Gestor_Economia.
/// - Este script solo expone la información del edificio y su progreso propio.
/// </summary>
public class Productor : MonoBehaviour
{
    [Header("Datos fijos")]
    [Tooltip("Asset que define los datos base de este edificio.")]
    [SerializeField] private DatosEdificio datosEdificio;

    [Tooltip("Configuración global con las tablas de progreso de mejoras locales.")]
    [SerializeField] private ConfiguracionProgreso configuracionProgreso;

    [Header("Estado actual del edificio")]
    [Tooltip("Cantidad actual de unidades compradas de este edificio.")]
    [SerializeField] private int cantidadEdificios;

    [Tooltip("Nivel actual de la mejora local del edificio.")]
    [SerializeField] private int nivelMejoraLocal;

    /// <summary>
    /// ID del edificio.
    /// Si no hay datos asignados, devuelve -1.
    /// </summary>
    public int Id => datosEdificio != null ? datosEdificio.id : -1;

    /// <summary>
    /// Nombre visible del edificio.
    /// </summary>
    public string Nombre => datosEdificio != null ? datosEdificio.nombre : "Sin Datos";

    /// <summary>
    /// Cantidad comprada actual.
    /// </summary>
    public int CantidadEdificios => cantidadEdificios;

    /// <summary>
    /// Nivel actual de la mejora local.
    /// </summary>
    public int NivelMejoraLocal => nivelMejoraLocal;

    /// <summary>
    /// Costo base definido por diseño.
    /// </summary>
    public double CostoBase => datosEdificio != null ? datosEdificio.costoBase : 0d;

    /// <summary>
    /// Producción base por una sola unidad de este edificio.
    /// </summary>
    public double ProduccionBase => datosEdificio != null ? datosEdificio.produccionBase : 0d;

    /// <summary>
    /// Multiplicador de crecimiento del costo por compra.
    /// </summary>
    public double MultiplicadorCosto => datosEdificio != null ? datosEdificio.multiplicadorCosto : 1.15d;

    /// <summary>
    /// Clave de guardado para la cantidad comprada.
    /// </summary>
    private string ClaveCantidad => "EDIFICIO_CANTIDAD_" + Id;

    /// <summary>
    /// Clave de guardado para el nivel de mejora local.
    /// </summary>
    private string ClaveMejoraLocal => "EDIFICIO_MEJORA_LOCAL_" + Id;

    /// <summary>
    /// Start:
    /// - valida referencias
    /// - carga el progreso guardado
    /// </summary>
    private void Start()
    {
        VerificarReferencias();
        CargarDatos();
    }

    /// <summary>
    /// Calcula el costo actual del siguiente edificio.
    /// 
    /// Fórmula:
    /// costoBase * multiplicadorCosto ^ cantidadComprada
    /// 
    /// El resultado se redondea hacia arriba.
    /// </summary>
    public float ObtenerCostoActual()
    {
        if (datosEdificio == null)
        {
            return 0f;
        }

        double costoCalculado = CostoBase * Math.Pow(MultiplicadorCosto, cantidadEdificios);
        double costoRedondeado = Math.Ceiling(costoCalculado);

        return (float)costoRedondeado;
    }

    /// <summary>
    /// Devuelve la producción de UNA sola unidad de este edificio,
    /// incluyendo el efecto de la mejora local.
    /// 
    /// Cada nivel de mejora local duplica la producción:
    /// producciónBase * 2^nivelMejoraLocal
    /// </summary>
    public float ObtenerProduccionPorUnidad()
    {
        if (datosEdificio == null)
        {
            return 0f;
        }

        double multiplicadorLocal = Math.Pow(2d, nivelMejoraLocal);
        double produccionUnidad = ProduccionBase * multiplicadorLocal;

        return (float)produccionUnidad;
    }

    /// <summary>
    /// Devuelve la producción total actual de este edificio.
    /// 
    /// Fórmula:
    /// cantidadComprada * producciónPorUnidad
    /// </summary>
    public float ObtenerProduccionTotal()
    {
        double produccionTotal = cantidadEdificios * (double)ObtenerProduccionPorUnidad();
        return (float)produccionTotal;
    }

    /// <summary>
    /// Compra una unidad del edificio.
    /// 
    /// Este método solo aumenta la cantidad y guarda.
    /// El gasto de Agonía lo valida y lo hace el Gestor_Economia.
    /// </summary>
    public void ComprarEdificio()
    {
        cantidadEdificios++;
        GuardarDatos();
    }

    /// <summary>
    /// Devuelve cuántos niveles máximos de mejora local existen,
    /// según la configuración cargada.
    /// </summary>
    public int ObtenerCantidadMaximaMejorasLocales()
    {
        if (configuracionProgreso == null ||
            configuracionProgreso.metasLocales == null ||
            configuracionProgreso.multiplicadoresCostoLocal == null)
        {
            return 0;
        }

        return Mathf.Min(
            configuracionProgreso.metasLocales.Length,
            configuracionProgreso.multiplicadoresCostoLocal.Length
        );
    }

    /// <summary>
    /// Indica si este edificio ya alcanzó el máximo nivel de mejora local.
    /// </summary>
    public bool TieneMaximaMejoraLocal()
    {
        return nivelMejoraLocal >= ObtenerCantidadMaximaMejorasLocales();
    }

    /// <summary>
    /// Devuelve la meta de cantidad necesaria para desbloquear
    /// la SIGUIENTE mejora local.
    /// 
    /// Si ya está al máximo, devuelve -1.
    /// </summary>
    public int ObtenerMetaSiguienteMejoraLocal()
    {
        if (TieneMaximaMejoraLocal())
        {
            return -1;
        }

        return configuracionProgreso.metasLocales[nivelMejoraLocal];
    }

    /// <summary>
    /// Revisa si la siguiente mejora local ya está desbloqueada
    /// por cantidad de edificios comprados.
    /// </summary>
    public bool PuedeDesbloquearSiguienteMejoraLocal()
    {
        if (TieneMaximaMejoraLocal())
        {
            return false;
        }

        int metaNecesaria = ObtenerMetaSiguienteMejoraLocal();
        return cantidadEdificios >= metaNecesaria;
    }

    /// <summary>
    /// Calcula el costo de la siguiente mejora local.
    /// 
    /// Fórmula:
    /// costoBaseDelEdificio * multiplicadorDeCostoDelNivel
    /// 
    /// El resultado se redondea hacia arriba.
    /// </summary>
    public float ObtenerCostoSiguienteMejoraLocal()
    {
        if (datosEdificio == null || configuracionProgreso == null || TieneMaximaMejoraLocal())
        {
            return 0f;
        }

        double multiplicadorCosto = configuracionProgreso.multiplicadoresCostoLocal[nivelMejoraLocal];
        double costoCalculado = CostoBase * multiplicadorCosto;
        double costoRedondeado = Math.Ceiling(costoCalculado);

        return (float)costoRedondeado;
    }

    /// <summary>
    /// Compra la siguiente mejora local.
    /// 
    /// Ojo:
    /// Este método NO descuenta Agonía.
    /// Eso lo hará el Gestor_Economia cuando montemos esa parte.
    /// 
    /// Aquí solo:
    /// - valida si no está al máximo
    /// - valida si ya se desbloqueó por cantidad
    /// - sube el nivel
    /// - guarda
    /// </summary>
    public bool ComprarSiguienteMejoraLocal()
    {
        if (TieneMaximaMejoraLocal())
        {
            return false;
        }

        if (!PuedeDesbloquearSiguienteMejoraLocal())
        {
            return false;
        }

        nivelMejoraLocal++;
        GuardarDatos();

        return true;
    }

    /// <summary>
    /// Guarda la cantidad comprada y el nivel de mejora local.
    /// </summary>
    public void GuardarDatos()
    {
        if (Id < 0)
        {
            return;
        }

        PlayerPrefs.SetInt(ClaveCantidad, cantidadEdificios);
        PlayerPrefs.SetInt(ClaveMejoraLocal, nivelMejoraLocal);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carga la cantidad comprada y el nivel de mejora local.
    /// 
    /// Si no existe nada guardado, ambos arrancan en 0.
    /// </summary>
    public void CargarDatos()
    {
        if (Id < 0)
        {
            return;
        }

        cantidadEdificios = PlayerPrefs.GetInt(ClaveCantidad, 0);
        nivelMejoraLocal = PlayerPrefs.GetInt(ClaveMejoraLocal, 0);
    }

    /// <summary>
    /// Reinicia el progreso local del edificio.
    /// </summary>
    public void ReiniciarDatos()
    {
        cantidadEdificios = 0;
        nivelMejoraLocal = 0;
        GuardarDatos();
    }

    /// <summary>
    /// Método viejo conservado para no romper llamadas anteriores
    /// si todavía existe alguna referencia en botones o inspector.
    /// </summary>
    public void reiniciar()
    {
        ReiniciarDatos();
    }

    /// <summary>
    /// Revisa si están todas las referencias necesarias.
    /// </summary>
    private void VerificarReferencias()
    {
        if (datosEdificio == null)
        {
            Debug.LogError("Productor sin DatosEdificio asignado en el objeto: " + gameObject.name);
        }

        if (configuracionProgreso == null)
        {
            Debug.LogError("Productor sin ConfiguracionProgreso asignada en el objeto: " + gameObject.name);
        }
    }
}