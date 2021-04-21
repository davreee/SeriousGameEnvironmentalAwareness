using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Clase que se encarga de cargar los diferentes elementos de la interfaz en el
 * UIController para que pueda modificarlos
 * 
 * @author David Real Ortega
 */
public class UILoader : MonoBehaviour
{
    //Variables de la interfaz que se cargarán al UIController
    public GameObject panelInGame;
    public GameObject panelPausa;
    public GameObject panelJuegoPerdido;
    public GameObject panelFinPartida;
    public Text textoPuntuacion;
    public GameObject textoJugador;
    public GameObject[] iconosVida;

    /**
     * Awake se invoca ante de cualquier función Start.
     * Cargamos la interfaz en el UIController
     * 
     */
    void Awake()
    {
        UIController.instance.panelInGame = panelInGame;
        UIController.instance.panelPausa = panelPausa;
        UIController.instance.panelJuegoPerdido = panelJuegoPerdido;  
        UIController.instance.panelFinPartida = panelFinPartida;  
        UIController.instance.textoPuntuacion = textoPuntuacion;
        UIController.instance.inicializarPartida();
        UIController.instance.iconosVida = iconosVida;
        UIController.instance.textoJugador = textoJugador;
        
    }

}
