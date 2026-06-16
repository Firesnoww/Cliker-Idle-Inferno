using UnityEngine;

public class SistemaOleadas : MonoBehaviour
{
    [Header("Prefabs de enemigos normales")]
    public GameObject[] prefabsEnemigosNormales;

    [Header("Prefabs de jefes / enemigos fuertes")]
    public GameObject[] prefabsJefes;

    [Header("Puntos de aparición")]
    public Transform[] puntosSpawn;

    [Header("Spawn inicial de enemigos normales")]
    [Tooltip("Tiempo inicial entre cada enemigo. Mientras más alto, más lento empieza.")]
    public float tiempoEntreEnemigosInicial = 4f;

    [Tooltip("Tiempo actual entre enemigos. Se actualiza automáticamente.")]
    public float tiempoEntreEnemigosActual = 4f;

    [Tooltip("Tiempo mínimo entre enemigos. Evita que el juego se llene demasiado.")]
    public float tiempoMinimoEntreSpawns = 0.8f;

    [Header("Crecimiento exponencial")]
    [Tooltip("Multiplicador aplicado cada oleada. Ej: 0.92 reduce el tiempo de spawn un 8% por oleada.")]
    [Range(0.5f, 1f)]
    public float multiplicadorTiempoPorOleada = 0.92f;

    [Tooltip("Cada cuántas oleadas aumenta el máximo de enemigos vivos.")]
    public int cadaCuantasOleadasAumentaMaximo = 2;

    [Tooltip("Cuántos enemigos vivos extra se permiten cuando aumenta el máximo.")]
    public int aumentoMaximoEnemigosPorBloque = 2;

    [Header("Límite de enemigos normales vivos")]
    public int maximoEnemigosNormalesVivosInicial = 5;
    public int maximoEnemigosNormalesVivosActual = 5;
    public int maximoEnemigosNormalesVivosFinal = 35;

    [Header("Control de oleadas por tiempo")]
    public int oleadaActual = 1;
    public float duracionOleada = 30f;

    [Header("Jefes")]
    public int cadaCuantasOleadasSaleJefe = 3;
    public bool permitirVariosJefes = false;

    [Header("Selección de enemigos normales")]
    public bool elegirEnemigoNormalAleatorio = true;

    [Tooltip("Si elegirEnemigoNormalAleatorio está apagado, los enemigos normales saldrán en orden.")]
    public int indiceEnemigoNormalActual = 0;

    [Header("Selección de jefes")]
    public bool elegirJefeAleatorio = true;

    [Tooltip("Si elegirJefeAleatorio está apagado, los jefes saldrán en orden.")]
    public int indiceJefeActual = 0;

    [Header("Estado actual")]
    public int enemigosNormalesVivos;
    public int jefesVivos;

    [Header("Control general")]
    public bool sistemaActivo = true;

    private float contadorSpawnNormal;
    private float contadorOleada;

    private void Start()
    {
        tiempoEntreEnemigosActual = tiempoEntreEnemigosInicial;
        maximoEnemigosNormalesVivosActual = maximoEnemigosNormalesVivosInicial;

        contadorSpawnNormal = tiempoEntreEnemigosActual;
        contadorOleada = duracionOleada;

        Debug.Log("Sistema de oleadas iniciado. Oleada actual: " + oleadaActual);
    }

    private void Update()
    {
        if (!sistemaActivo) return;

        ControlarSpawnDeEnemigosNormales();
        ControlarAvanceDeOleadas();
    }

    private void ControlarSpawnDeEnemigosNormales()
    {
        contadorSpawnNormal -= Time.deltaTime;

        if (contadorSpawnNormal <= 0f)
        {
            if (enemigosNormalesVivos < maximoEnemigosNormalesVivosActual)
            {
                CrearEnemigoNormal();
            }

            contadorSpawnNormal = tiempoEntreEnemigosActual;
        }
    }

    private void ControlarAvanceDeOleadas()
    {
        contadorOleada -= Time.deltaTime;

        if (contadorOleada <= 0f)
        {
            AvanzarOleada();
            contadorOleada = duracionOleada;
        }
    }

    private void AvanzarOleada()
    {
        oleadaActual++;

        Debug.Log("Avanzó a la oleada: " + oleadaActual);

        AumentarDificultadProgresiva();

        bool tocaJefe = oleadaActual % cadaCuantasOleadasSaleJefe == 0;

        if (tocaJefe)
        {
            CrearJefe();
        }
    }

    private void AumentarDificultadProgresiva()
    {
        // Crecimiento exponencial controlado:
        // cada oleada reduce el tiempo entre enemigos.
        tiempoEntreEnemigosActual *= multiplicadorTiempoPorOleada;
        tiempoEntreEnemigosActual = Mathf.Max(tiempoEntreEnemigosActual, tiempoMinimoEntreSpawns);

        // Aumenta el máximo de enemigos vivos poco a poco.
        if (oleadaActual % cadaCuantasOleadasAumentaMaximo == 0)
        {
            maximoEnemigosNormalesVivosActual += aumentoMaximoEnemigosPorBloque;

            maximoEnemigosNormalesVivosActual = Mathf.Min(
                maximoEnemigosNormalesVivosActual,
                maximoEnemigosNormalesVivosFinal
            );
        }

        Debug.Log(
            "Dificultad actualizada. Tiempo entre enemigos: " +
            tiempoEntreEnemigosActual +
            " | Máximo enemigos vivos: " +
            maximoEnemigosNormalesVivosActual
        );
    }

    private void CrearEnemigoNormal()
    {
        if (prefabsEnemigosNormales == null || prefabsEnemigosNormales.Length == 0)
        {
            Debug.LogWarning("No hay prefabs de enemigos normales asignados.");
            return;
        }

        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogWarning("No hay puntos de spawn asignados.");
            return;
        }

        GameObject prefabSeleccionado = ObtenerPrefabEnemigoNormal();

        if (prefabSeleccionado == null)
        {
            Debug.LogWarning("El prefab de enemigo normal seleccionado está vacío.");
            return;
        }

        Transform puntoElegido = ObtenerPuntoSpawnAleatorio();

        GameObject enemigoCreado = Instantiate(
            prefabSeleccionado,
            puntoElegido.position,
            puntoElegido.rotation
        );

        enemigosNormalesVivos++;

        EnemigoNotificadorMuerte notificador = enemigoCreado.GetComponent<EnemigoNotificadorMuerte>();

        if (notificador == null)
        {
            notificador = enemigoCreado.AddComponent<EnemigoNotificadorMuerte>();
        }

        notificador.sistemaOleadas = this;
        notificador.esJefe = false;
    }

    private void CrearJefe()
    {
        if (prefabsJefes == null || prefabsJefes.Length == 0)
        {
            Debug.LogWarning("No hay prefabs de jefes asignados.");
            return;
        }

        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogWarning("No hay puntos de spawn asignados.");
            return;
        }

        if (!permitirVariosJefes && jefesVivos > 0)
        {
            Debug.Log("No se creó jefe porque ya hay uno vivo.");
            return;
        }

        GameObject prefabJefeSeleccionado = ObtenerPrefabJefe();

        if (prefabJefeSeleccionado == null)
        {
            Debug.LogWarning("El prefab de jefe seleccionado está vacío.");
            return;
        }

        Transform puntoElegido = ObtenerPuntoSpawnAleatorio();

        GameObject jefeCreado = Instantiate(
            prefabJefeSeleccionado,
            puntoElegido.position,
            puntoElegido.rotation
        );

        jefesVivos++;

        EnemigoNotificadorMuerte notificador = jefeCreado.GetComponent<EnemigoNotificadorMuerte>();

        if (notificador == null)
        {
            notificador = jefeCreado.AddComponent<EnemigoNotificadorMuerte>();
        }

        notificador.sistemaOleadas = this;
        notificador.esJefe = true;

        Debug.Log("Apareció jefe: " + jefeCreado.name + " en la oleada: " + oleadaActual);
    }

    private GameObject ObtenerPrefabEnemigoNormal()
    {
        if (elegirEnemigoNormalAleatorio)
        {
            int indiceAleatorio = Random.Range(0, prefabsEnemigosNormales.Length);
            return prefabsEnemigosNormales[indiceAleatorio];
        }

        GameObject enemigo = prefabsEnemigosNormales[indiceEnemigoNormalActual];

        indiceEnemigoNormalActual++;

        if (indiceEnemigoNormalActual >= prefabsEnemigosNormales.Length)
        {
            indiceEnemigoNormalActual = 0;
        }

        return enemigo;
    }

    private GameObject ObtenerPrefabJefe()
    {
        if (elegirJefeAleatorio)
        {
            int indiceAleatorio = Random.Range(0, prefabsJefes.Length);
            return prefabsJefes[indiceAleatorio];
        }

        GameObject jefe = prefabsJefes[indiceJefeActual];

        indiceJefeActual++;

        if (indiceJefeActual >= prefabsJefes.Length)
        {
            indiceJefeActual = 0;
        }

        return jefe;
    }

    private Transform ObtenerPuntoSpawnAleatorio()
    {
        return puntosSpawn[Random.Range(0, puntosSpawn.Length)];
    }

    public void AvisarEnemigoMuerto(bool eraJefe)
    {
        if (eraJefe)
        {
            jefesVivos--;
            jefesVivos = Mathf.Max(0, jefesVivos);

            Debug.Log("Murió un jefe. Jefes vivos: " + jefesVivos);
        }
        else
        {
            enemigosNormalesVivos--;
            enemigosNormalesVivos = Mathf.Max(0, enemigosNormalesVivos);

            Debug.Log("Murió un enemigo normal. Enemigos normales vivos: " + enemigosNormalesVivos);
        }
    }

    public void DetenerSistema()
    {
        sistemaActivo = false;
    }
}