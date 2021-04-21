using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar las diferentes escenas existentes y
 * de comunicarse con el UIController para realizar la actualización de la
 * interfaz.
 * 
 * @author David Real Ortega
 */
public class GameManager : MonoBehaviour
{
    //Instancia actual del GameManager
    public static GameManager instance = null;

    //Variable que indica si el juego está pausado o no
    private bool juegoPausado = false;


    /**
     * Awake se invoca ante de cualquier función Start.
     * En este caso establece el patrón Singleton.
     * 
     */
    void Awake()
    {



        //Comprueba si la instancia existe
        if (instance == null)
        {
            //Si no, este se convierte en la instancia
            instance = this;
        }

        //En caso contrario, si la instancia existe y no es este
        else if (instance != this)

            //Destruye este, lo que fuerza el Singleton ya que solo puede haber un GameManager
            Destroy(gameObject);

        //Establecemos que este no se pueda destruir cuando se recarga la escena
        DontDestroyOnLoad(gameObject);

    }

    /**
     * Función que se encargar de pausar el juego si no está pausado y de
     * reanudarlo si esté se encuentra pausado y de informar a la interfaz
     * para que se actualice.
     * 
     */
    public void pausarJuego()
    {
        if (!instance.juegoPausado)
        {
            Time.timeScale = 0;

        }
        else
        {
            Time.timeScale = 1;

        }
        Cursor.visible = !Cursor.visible;
        instance.juegoPausado = !instance.juegoPausado;

        UIController.instance.pausarJuego();
    }

    /**
     * Función que se encarga de informar a la interfaz cuando
     * el usuario le comunica que su vida a cambiado
     * 
     * @param vida la cantidad de vida del jugador
     * @param modo si la vida ha cambiado hacia arriba o hacia abajo
     */
    public void actualizarVida(int vida, bool modo)
    {
        UIController.instance.actualizarVida(vida, modo);
    }

    /**
     * Función que informa a la interfaz para que esta muestre que
     * el jugador tiene toda la vida disponible
     *
     */
    public void inicializarVida()
    {
        UIController.instance.inicializarVida();
    }

    /**
     * Función que para el tiempo e indica a la interfaz que el jugador ha muerto
     * para que muestre la pantalla correspondiente
     *
     */
    public void jugadorMuerto()
    {
        Cursor.visible = true;
        Time.timeScale = 0;
        UIController.instance.juegoTerminado();

    }

    /**
     * Función que se encarga de cargar el menú
     *
     */
    public void cargarMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        Cursor.visible = true;

    }

    /**
     * Función que se encarga de cargar el tutorial
     *
     */
    public void cargarTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
        Cursor.visible = false;
        instance.juegoPausado = false;
        Time.timeScale = 1;


    }


    /**
     * Función que se encarga de cargar el nivel principal
     *
     */
    public void cargarNivel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
        Cursor.visible = false;
        instance.juegoPausado = false;
        Time.timeScale = 1;


    }

    /**
     * Función que carga la segunda parte de las encuestas
     *
     */
    public void seguirEncuestas()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EncuestaDespues");
    }

    /**
     * Función que recarga el tutorial
     *
     */
    public void recargarTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Cursor.visible = false;
        instance.juegoPausado = false;
    }

    /**
     * Función que recarga el nivel principal.
     * Además informa al ScoreController para que realice las acciones
     * necesarias sobre los datos de juego.
     *
     */
    public void recargarNivel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        ScoreController.instance.acumularTotales();
        ScoreController.instance.inicializarVariables();
        Cursor.visible = false;
        instance.juegoPausado = false;
        Time.timeScale = 1;

    }

    /**
     * Función que cierra la aplicación
     *
     */
    public void cerrarAplicacion()
    {
        Application.Quit();
    }

    /**
     * Función que gestiona la llegada del usuario al final de los niveles
     *
     */
    public void alcanzadoPuntoFinal()
    {
        ScoreController.instance.tiempoFinal = Time.time;
        ScoreController.instance.juegoTerminado = true;
        Time.timeScale = 0;
        UIController.instance.pantallaFinPartida();
        Cursor.visible = true;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Tutorial") ScoreController.instance.tutorialTerminado();
    }

    /**
     * Función que carga la pantalla correspondiente cuando todas las encuestas 
     * se han terminado
     * 
     */
    public void encuestasTerminadas()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Final");
        ScoreController.instance.guardarDatos();
    }

    /**
     * Función que indica si el juego está pausado o no
     *
     * @return true si el juego está pausado, false si no
     */
    public bool getJuegoPausado()
    {
        return instance.juegoPausado;
    }
}
