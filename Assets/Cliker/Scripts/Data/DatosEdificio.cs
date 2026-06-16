using UnityEngine;

/// <summary>
/// DatosEdificio
/// 
/// ScriptableObject que contiene SOLO los datos fijos de diseño de un edificio.
/// 
/// Importante:
/// - Este asset NO guarda progreso del jugador.
/// - Aquí NO se guarda cantidad comprada.
/// - Aquí NO se guarda nivel de mejora.
/// - Aquí NO se guardan estados temporales.
/// 
/// Este archivo define:
/// - identidad del edificio
/// - costo base
/// - producción base
/// - crecimiento del costo por compra
/// - orden general dentro de la progresión del juego
/// </summary>
[CreateAssetMenu(fileName = "NuevoEdificio", menuName = "Idle Game/Datos Edificio")]
public class DatosEdificio : ScriptableObject
{
    [Header("Identidad")]
    [Tooltip("ID único del edificio. No debe repetirse.")]
    public int id;

    [Tooltip("Nombre visible del edificio en la interfaz.")]
    public string nombre;

    [Tooltip("Posición del edificio dentro de la progresión general del juego.")]
    public int ordenProgresion;

    [Header("Economía base")]
    [Tooltip("Costo base del edificio antes de aplicar crecimiento por cantidad comprada.")]
    public double costoBase;

    [Tooltip("Producción base de UNA unidad de este edificio.")]
    public double produccionBase;

    [Tooltip("Multiplicador de crecimiento del costo por compra. Ejemplo recomendado: 1.15")]
    public double multiplicadorCosto = 1.15;
}