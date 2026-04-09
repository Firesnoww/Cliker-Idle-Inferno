using UnityEngine;

/// <summary>
/// FormateadorNumeros
/// 
/// Esta clase existe para convertir números grandes en versiones
/// más fáciles de leer en la interfaz.
/// 
/// Ejemplos:
/// 950       -> 950
/// 1200      -> 1.2K
/// 15000     -> 15K
/// 2300000   -> 2.3M
/// 4000000000 -> 4B
/// 
/// Importante:
/// - Es una clase estática, así que no se pone en un GameObject.
/// - Se llama directamente desde otros scripts.
/// - Sirve para mantener consistente el formato de todos los números del juego.
/// </summary>
public static class FormateadorNumeros
{
    /// <summary>
    /// Convierte un número en un formato abreviado para UI.
    /// 
    /// Ejemplo:
    /// 1500      -> 1.5K
    /// 2500000   -> 2.5M
    /// 730000000 -> 730M
    /// 
    /// Parámetros:
    /// - valor: número que se quiere formatear
    /// - decimales: cuántos decimales mostrar cuando el número se abrevia
    /// 
    /// Ejemplo:
    /// FormatearNumero(1530, 1) -> "1.5K"
    /// FormatearNumero(1530, 2) -> "1.53K"
    /// </summary>
    public static string FormatearNumero(double valor, int decimales = 1)
    {
        // Si el número es negativo, guardamos el signo
        // y trabajamos con el valor absoluto para no romper la lógica.
        bool esNegativo = valor < 0;
        double valorAbsoluto = Mathf.Abs((float)valor);

        // Si el número es menor a 1000, no lo abreviamos.
        // Solo lo devolvemos normal, redondeado sin decimales.
        if (valorAbsoluto < 1000d)
        {
            string numeroSimple = valor.ToString("F0");
            return numeroSimple;
        }

        // Lista de sufijos para números grandes.
        // Puedes ampliarla más adelante si tu juego crece muchísimo.
        string[] sufijos = new string[]
        {
            "K",   // mil
            "M",   // millón
            "B",   // billón (estilo idle/internacional)
            "T",   // trillion
            "Qa",  // quadrillion
            "Qi",  // quintillion
            "Sx",  // sextillion
            "Sp",  // septillion
            "Oc",  // octillion
            "No",  // nonillion
            "Dc"   // decillion
        };

        // Índice del sufijo que se va a usar.
        int indiceSufijo = -1;

        // Mientras el número sea 1000 o más, lo dividimos entre 1000
        // y subimos al siguiente sufijo.
        while (valorAbsoluto >= 1000d && indiceSufijo < sufijos.Length - 1)
        {
            valorAbsoluto /= 1000d;
            indiceSufijo++;
        }

        // Construimos el formato de decimales dinámicamente.
        // Ejemplo:
        // decimales = 1 -> "F1"
        // decimales = 2 -> "F2"
        string formato = "F" + decimales;

        // Aplicamos el signo negativo si hacía falta.
        string numeroFormateado = valorAbsoluto.ToString(formato);

        if (esNegativo)
        {
            numeroFormateado = "-" + numeroFormateado;
        }

        // Devolvemos el número + el sufijo.
        return numeroFormateado + sufijos[indiceSufijo];
    }
}