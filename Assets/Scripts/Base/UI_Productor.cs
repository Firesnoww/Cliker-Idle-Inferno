using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI_Productor
/// 
/// Este script controla la interfaz visual de UN edificio/productor.
/// 
/// Su trabajo es:
/// 1. Leer los datos del Productor asignado.
/// 2. Mostrar en pantalla:
///    - Nombre del edificio
///    - Costo actual
///    - Cantidad comprada
///    - Producción total de ese edificio
/// 3. Permitir comprar ese edificio desde un botón.
/// 4. Opcionalmente bloquear el botón si no hay suficiente Agonía.
/// 
/// Importante:
/// - Este script NO guarda datos.
/// - Este script NO calcula la economía global.
/// - Este script solo muestra datos y envía la orden de compra al Gestor_Economia.
/// </summary>
public class UI_Productor : MonoBehaviour
{
    [Header("Referencias principales")]
    [Tooltip("Referencia al Productor que este panel UI representa.")]
    [SerializeField] private Productor productor;

    [Tooltip("Referencia al gestor central de economía.")]
    [SerializeField] private Gestor_Economia gestorEconomia;

    [Header("Textos de UI")]
    [Tooltip("Texto donde se mostrará el nombre del edificio.")]
    [SerializeField] private TMP_Text textNombre;

    [Tooltip("Texto donde se mostrará el costo actual del siguiente edificio.")]
    [SerializeField] private TMP_Text textCosto;

    [Tooltip("Texto donde se mostrará cuántos edificios de este tipo tiene el jugador.")]
    [SerializeField] private TMP_Text textCantidad;

    [Tooltip("Texto donde se mostrará la producción total de este edificio.")]
    [SerializeField] private TMP_Text textProduccion;

    [Header("Botón de compra")]
    [Tooltip("Botón que compra este edificio.")]
    [SerializeField] private Button botonComprar;

    /// <summary>
    /// Start:
    /// Se ejecuta una vez al inicio.
    /// 
    /// Aquí validamos referencias importantes y forzamos una primera actualización
    /// para que la UI no aparezca vacía al comenzar el juego.
    /// </summary>
    private void Start()
    {
        VerificarReferencias();
        ActualizarUICompleta();
    }

    /// <summary>
    /// Update:
    /// Se ejecuta una vez por frame.
    /// 
    /// En esta primera versión vamos a actualizar la UI continuamente
    /// para mantener todo simple y claro.
    /// 
    /// Más adelante, si quieres optimizar, esto lo podemos pasar a eventos
    /// para no actualizar cada frame.
    /// </summary>
    private void Update()
    {
        ActualizarUICompleta();
        ActualizarEstadoBoton();
    }

    /// <summary>
    /// Actualiza todos los textos del panel de este Productor.
    /// 
    /// Este método centraliza toda la parte visual:
    /// - Nombre
    /// - Costo
    /// - Cantidad
    /// - Producción
    /// </summary>
    private void ActualizarUICompleta()
    {
        ActualizarNombre();
        ActualizarCosto();
        ActualizarCantidad();
        ActualizarProduccion();
    }

    /// <summary>
    /// Muestra el nombre del edificio.
    /// 
    /// El nombre se toma directamente desde el Productor,
    /// que a su vez lo obtiene desde su ScriptableObject.
    /// </summary>
    private void ActualizarNombre()
    {
        if (textNombre == null || productor == null)
        {
            return;
        }

        textNombre.text = productor.Nombre;
    }

    /// <summary>
    /// Muestra el costo actual del siguiente edificio.
    /// 
    /// Este valor cambia conforme sube la cantidad comprada,
    /// porque el precio escala con la fórmula del Productor.
    /// 
    /// Ahora usamos el FormateadorNumeros para que el costo
    /// se vea más limpio cuando crezca mucho.
    /// </summary>
    private void ActualizarCosto()
    {
        if (textCosto == null || productor == null)
        {
            return;
        }

        float costoActual = productor.ObtenerCostoActual();

        textCosto.text = "Costo: " + FormateadorNumeros.FormatearNumero(costoActual, 2);
    }

    /// <summary>
    /// Muestra la cantidad comprada de este edificio.
    /// </summary>
    private void ActualizarCantidad()
    {
        if (textCantidad == null || productor == null)
        {
            return;
        }

        textCantidad.text = "Cantidad: " + productor.CantidadEdificios.ToString();
    }

    /// <summary>
    /// Muestra la producción total de este edificio.
    /// 
    /// Ojo:
    /// Aquí estamos mostrando la producción de ESTE productor solamente,
    /// no la producción global del juego.
    /// 
    /// Ahora usamos el FormateadorNumeros para que se vea limpio
    /// incluso cuando los números crezcan bastante.
    /// </summary>
    private void ActualizarProduccion()
    {
        if (textProduccion == null || productor == null)
        {
            return;
        }

        float produccionTotal = productor.ObtenerProduccionTotal();

        textProduccion.text = "Producción: " + FormateadorNumeros.FormatearNumero(produccionTotal, 2) + "/s";
    }

    /// <summary>
    /// Activa o desactiva el botón de compra dependiendo de si
    /// el jugador tiene suficiente Agonía para comprar este edificio.
    /// 
    /// Si no hay referencias válidas, el botón se desactiva por seguridad.
    /// </summary>
    private void ActualizarEstadoBoton()
    {
        if (botonComprar == null || productor == null || gestorEconomia == null)
        {
            return;
        }

        float costoActual = productor.ObtenerCostoActual();

        // Si el jugador puede pagar el costo, el botón queda activo.
        // Si no, se desactiva.
        botonComprar.interactable = gestorEconomia.PuedeComprar(costoActual);
    }

    /// <summary>
    /// Método público para ser llamado desde el botón UI.
    /// 
    /// Cuando se pulsa el botón:
    /// - se valida que existan referencias
    /// - se le pide al Gestor_Economia que intente comprar este Productor
    /// 
    /// El Gestor_Economia sigue siendo quien autoriza realmente la compra.
    /// Este script solo manda la solicitud.
    /// </summary>
    public void ComprarDesdeBoton()
    {
        if (gestorEconomia == null)
        {
            Debug.LogError("UI_Productor: no hay Gestor_Economia asignado.");
            return;
        }

        if (productor == null)
        {
            Debug.LogError("UI_Productor: no hay Productor asignado.");
            return;
        }

        gestorEconomia.ComprarProductor(productor);
    }

    /// <summary>
    /// Verifica si las referencias principales están bien conectadas.
    /// 
    /// Esto ayuda mucho a detectar errores de configuración
    /// en el Inspector al empezar.
    /// </summary>
    private void VerificarReferencias()
    {
        if (productor == null)
        {
            Debug.LogWarning("UI_Productor: no se asignó un Productor en el objeto " + gameObject.name);
        }

        if (gestorEconomia == null)
        {
            Debug.LogWarning("UI_Productor: no se asignó Gestor_Economia en el objeto " + gameObject.name);
        }

        if (textNombre == null)
        {
            Debug.LogWarning("UI_Productor: falta asignar textNombre en " + gameObject.name);
        }

        if (textCosto == null)
        {
            Debug.LogWarning("UI_Productor: falta asignar textCosto en " + gameObject.name);
        }

        if (textCantidad == null)
        {
            Debug.LogWarning("UI_Productor: falta asignar textCantidad en " + gameObject.name);
        }

        if (textProduccion == null)
        {
            Debug.LogWarning("UI_Productor: falta asignar textProduccion en " + gameObject.name);
        }

        if (botonComprar == null)
        {
            Debug.LogWarning("UI_Productor: falta asignar botonComprar en " + gameObject.name);
        }
    }
}