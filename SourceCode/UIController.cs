using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Clase que se encarga de gestionar la interfaz
 * 
 * @author David Real Ortega
 */
public class UIController : MonoBehaviour
{
    //Instancia actual del UIController
    public static UIController instance = null;

    //Panel que se muestra dentro del juego
    public GameObject panelInGame;
    //Panel que se muestra cuando el juego está en pausa
    public GameObject panelPausa;
    //Panel que se muestra cuando el jugador muere
    public GameObject panelJuegoPerdido;
    //Panel que se muestra cuando el jugador gana la partida
    public GameObject panelFinPartida;
    //Objeto donde se muestra al usuario el feedback de la puntuación
    public GameObject textoJugador;
    //Texto donde se muestra la puntuación actual
    public Text textoPuntuacion;
    //Objetos donde se muestra al usuario el feedback de la vida
    public GameObject[] iconosVida;

    //Variable que indica si el juego está pausado
    public bool juegoPausado;

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
     * Función que se encarga de mostrar la pantalla correspondiente cuando el juego está en pausa
     * y de ocultarla cuando la partida se reanuda
     * 
     */
    public void pausarJuego()
    {
        juegoPausado = !juegoPausado;
        panelInGame.SetActive(!juegoPausado);
        panelPausa.SetActive(juegoPausado);
    }

    /**
     * Función que se encarga de mostrar u ocultar el icono de vida correspondiente en función de la
     * vida del usuario
     * 
     * @param vida el icono al que le afectará la acción
     * @param modo si la vida ha cambiado hacia arriba o hacia abajo
     */
    public void actualizarVida(int vida, bool modo)
    {
        iconosVida[vida].SetActive(modo);
    }

    /**
     * Función que se encarga de mostrar todos los iconos de vida
     */
    public void inicializarVida()
    {
        for (int i = 0; i < iconosVida.Length; i++)
        {
            iconosVida[i].SetActive(true);
        }
    }

    /**
     * Función que se encarga de mostrar la pantalla correspondiente cuando el usario pierde
     * 
     */
    public void juegoTerminado()
    {
        panelInGame.SetActive(false);
        panelPausa.SetActive(false);
        panelJuegoPerdido.SetActive(true);
    }

    /**
     * Función que se encarga de ocultar el resto de pantallas y mostrar únicamente la pantalla
     * in game
     * 
     */
    public void inicializarPartida()
    {
        panelInGame.SetActive(true);
        panelPausa.SetActive(false);
        panelJuegoPerdido.SetActive(false);
        panelFinPartida.SetActive(false);
        juegoPausado = false;
    }

    /**
    * Función que se encarga de mostrar la pantalla correspondiente cuando el usario gana
    * 
    */
    public void pantallaFinPartida()
    {
        panelInGame.SetActive(false);
        panelPausa.SetActive(false);
        panelFinPartida.SetActive(true);
    }

    /**
    * Función que se actualiza la parte de la interfaz referente a la puntuación
    * 
    */
    public void actualizarPuntuacion(int puntuacion)
    {
        textoPuntuacion.text = puntuacion.ToString();
    }

    /**
    * Función que se encarga de mostrar el feedback al usuario sobre la puntuación
    * 
    */
    public void informarJugador(int puntuacion)
    {
        textoJugador.GetComponent<TextMesh>().text = puntuacion.ToString();
        textoJugador.SetActive(true);
        textoJugador.GetComponent<Animation>().Play();
    }
}
