using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar las respuestas de los usuarios
 * en las encuestas
 * 
 * @author David Real Ortega
 */
public class EncuestaController : MonoBehaviour
{
    //Paneles en los que se van realizando las preguntas a los usuarios
    public GameObject[] paneles;
    //Cantidad de paneles existentes
    private int nPaneles;
    //Panel en el que se encuentra el usuario
    private int panelActual;
    //Atributo que indica si las preguntas realizadas son antes o después de la partida
    public bool despuesJugar;

    //Atributos que almacenan los valores de las respuestas
    private const int RESPUESTA_INCORRECTA = 0;
    private const int RESPUESTA_CORRECTA = 1;
    private const int RESPUESTA_PASADA = 2;

    /**
     * Enum que se utiliza para diferenciar sobre que tipo de basura
     * trata la pregunta actual
     * 
     */
    public enum TipoDato
    {
        basuraOrganica = 0,
        basuraVidrio = 1,
        basuraPBL = 2,
        basuraPC = 3
    }

    /**
     * Start es invocada antes de la actualización del primer frame
     * 
     */
    void Start()
    {
        //Se actualiza la información de los paneles y se muestra el primero
        nPaneles = paneles.Length;
        panelActual = 0;
        paneles[0].SetActive(true);

    }

    /**
     * Función que se encarga de desactivar el panel actual, mostrar el siguiente y en
     * caso de haber contestado a la última pregunta, cargar la siguiente escena
     * 
     */
    private void siguientePanel()
    {
        //Activamos el siguiente panel
        panelActual++;
        if (panelActual < nPaneles)
        {
            paneles[panelActual - 1].SetActive(false);
            paneles[panelActual].SetActive(true);
        }
        else
        {
            //Si hemos contestado la última pregunta, cargamos la escena correspondiente
            if (despuesJugar)
            {
                GameManager.instance.encuestasTerminadas();
            }
            else
            {
                GameManager.instance.cargarMenu();
            }
        }
    }

    /**
     * Función que se encarga de actualizar la información cuando se responde a una pregunta
     * incorrectamente
     * 
     */
    public void respuestaIncorrecta(int tipoDato)
    {
        switch (tipoDato)
        {
            case (int)TipoDato.basuraOrganica:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraOrganicaAP = RESPUESTA_INCORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraOrganicaBP = RESPUESTA_INCORRECTA;
                }

                break;
            case (int)TipoDato.basuraVidrio:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraVidrioAP = RESPUESTA_INCORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraVidrioBP = RESPUESTA_INCORRECTA;
                }

                break;
            case (int)TipoDato.basuraPBL:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraPBLAP = RESPUESTA_INCORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraPBLBP = RESPUESTA_INCORRECTA;
                }

                break;
            case (int)TipoDato.basuraPC:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraPCAP = RESPUESTA_INCORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraPCBP = RESPUESTA_INCORRECTA;
                }
                break;
            default:
                break;
        }
        siguientePanel();
    }

    /**
     * Función que se encarga de actualizar la información cuando se responde a una pregunta
     * correctamente
     * 
     */
    public void respuestaCorrecta(int tipoDato)
    {
        switch (tipoDato)
        {
            case (int)TipoDato.basuraOrganica:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraOrganicaAP = RESPUESTA_CORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraOrganicaBP = RESPUESTA_CORRECTA;
                }
                break;
            case (int)TipoDato.basuraVidrio:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraVidrioAP = RESPUESTA_CORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraVidrioBP = RESPUESTA_CORRECTA;
                }
                break;
            case (int)TipoDato.basuraPBL:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraPBLAP = RESPUESTA_CORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraPBLBP = RESPUESTA_CORRECTA;
                }
                break;
            case (int)TipoDato.basuraPC:
                if (despuesJugar)
                {
                    ScoreController.instance.basuraPCAP = RESPUESTA_CORRECTA;
                }
                else
                {
                    ScoreController.instance.basuraPCBP = RESPUESTA_CORRECTA;
                }
                break;
            default:
                break;
        }
        siguientePanel();
    }

    /**
     * Función que se encarga de actualizar la información cuando se responde a una pregunta
     * con la opción de "No lo sé"
     * 
     */
    public void noContesto(int tipoDato)
    {
        switch (tipoDato)
        {
            case (int)TipoDato.basuraOrganica:
                ScoreController.instance.basuraOrganicaBP = RESPUESTA_PASADA;
                break;
            case (int)TipoDato.basuraVidrio:
                ScoreController.instance.basuraVidrioBP = RESPUESTA_PASADA;
                break;
            case (int)TipoDato.basuraPBL:
                ScoreController.instance.basuraPBLBP = RESPUESTA_PASADA;
                break;
            case (int)TipoDato.basuraPC:
                ScoreController.instance.basuraPCBP = RESPUESTA_PASADA;
                break;
            default:
                break;
        }
        siguientePanel();
    }

    /**
     * Función que se encarga de actualizar la información cuando se responde a la pregunta
     * referente al grado de satisfacción del usuario con el juego
     * 
     */
    public void gradoSatisfaccionFuncion(float grado)
    {
        ScoreController.instance.gradoSatisfaccion = grado;
        siguientePanel();
    }
}
