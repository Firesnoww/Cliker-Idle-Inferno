using UnityEngine;

/// <summary>
/// ConfiguracionProgreso
/// 
/// ScriptableObject que guarda las tablas fijas de progresión
/// que usa la versión base del juego actual.
/// 
/// En esta etapa lo dejamos enfocado SOLO en:
/// - metas de desbloqueo de la mejora local del edificio
/// - multiplicadores de costo de la mejora local del edificio
/// 
/// Más adelante este script se puede ampliar con:
/// - infraestructura
/// - mejoras globales
/// - gachapón
/// - prestigio
/// 
/// Pero por ahora lo dejamos limpio y enfocado.
/// </summary>
[CreateAssetMenu(fileName = "ConfiguracionProgreso", menuName = "Idle Game/Configuracion Progreso")]
public class ConfiguracionProgreso : ScriptableObject
{
    [Header("Mejoras locales del edificio")]
    [Tooltip("Cantidad de edificios necesaria para desbloquear cada nivel de mejora local.")]
    public int[] metasLocales;

    [Tooltip("Multiplicadores del costo base del edificio para calcular el costo de cada mejora local.")]
    public double[] multiplicadoresCostoLocal;
}