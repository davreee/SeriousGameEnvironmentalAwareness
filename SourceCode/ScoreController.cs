using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * Clase que se encarga de gestionar los datos de uso
 * 
 * @author David Real Ortega
 */
public class ScoreController : MonoBehaviour
{
    //Instancia actual del ScoreController
    public static ScoreController instance = null;

    /**
     * Enum que se utiliza para diferenciar el tipo de enemigo para
     * actualizar la información correspondiente a dicho tipo.
     */
    public enum TipoEnemigo
    {
        Vidrio,
        Organica,
        PapelCarton,
        PlasticoBriksLatas
    };
        
    //Información sobre los intentos
    private float intentosVidrio = 0;
    private float intentosOrganica = 0;
    private float intentosPapelCarton = 0;
    private float intentosPlasticoBrikLatas = 0;

    private float intentosVidrioTotales = 0;
    private float intentosOrganicaTotales = 0;
    private float intentosPapelCartonTotales = 0;
    private float intentosPlasticoBrikLatasTotales = 0;

    //Información sobre los aciertos
    private float aciertosVidrio = 0;
    private float aciertosOrganica = 0;
    private float aciertosPapelCarton = 0;
    private float aciertosPlasticoBrikLatas = 0;

    private float aciertosVidrioTotales = 0;
    private float aciertosOrganicaTotales = 0;
    private float aciertosPapelCartonTotales = 0;
    private float aciertosPlasticoBrikLatasTotales = 0;

    //Información sobre los disparos
    private float disparosRealizados = 0;
    private float disparosAcertados = 0;

    private float disparosRealizadosTotales = 0;
    private float disparosAcertadosTotales = 0;

    //Información sobre las veces que es herido y muere el jugador
    public int vecesHerido = 0;
    public int vecesMuerto = 0;

    //Información sobre la puntuación del jugador
    public int puntuacion;

    //Información sobre el tiempo que tarda el jugador en pasarse el juego.
    public float tiempoInicial;
    public float tiempoFinal;

    //Información de las encuestas antes de jugar
    public int basuraOrganicaBP = 0;
    public int basuraVidrioBP = 0;
    public int basuraPBLBP = 0;
    public int basuraPCBP = 0;

    //Información de las encuestas después de jugar
    public int basuraOrganicaAP = 0;
    public int basuraVidrioAP = 0;
    public int basuraPBLAP = 0;
    public int basuraPCAP = 0;

    //Información sobre el grado de satisfacción del usuario
    public float gradoSatisfaccion = 0;

    //Indica si el jugador ha terminado el nivel inicial o no
    public bool juegoTerminado = false;


    //Información de donde se guardará el archivo de los datos
    private string nombreCarpeta;
    private string nombreArchivo;
    public string marcaTemporal;

    /**
     * Awake se invoca ante de cualquier función Start.
     * En este caso establece el patrón Singleton.
     * Además recopilamos la información sobre la marca temporal.
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

        //Cogemos la información de la marca temporal.
        marcaTemporal = System.DateTime.Now.ToString("dd - MM - yyyy HH-mm-ss");
    }

    /**
     * Función que se encarga de inicializar las variables correspondientes
     * al nivel actual
     * 
     */
    public void inicializarVariables()
    {
        intentosVidrio = 0;
        intentosOrganica = 0;
        intentosPapelCarton = 0;
        intentosPlasticoBrikLatas = 0;
        aciertosVidrio = 0;
        aciertosOrganica = 0;
        aciertosPapelCarton = 0;
        aciertosPlasticoBrikLatas = 0;
        disparosRealizados = 0;
        disparosAcertados = 0;
        puntuacion = 0;
        tiempoInicial = Time.time;
        tiempoFinal = Time.time;
        UIController.instance.actualizarPuntuacion(puntuacion);

    }

    /**
     * Función que se encarga de actualizar la información referente cuando el
     * jugador realiza un disparo.
     * 
     */
    public void disparoRealizado()
    {
        disparosRealizados++;
    }

    /**
     * Función que se encarga de actualizar la información referente cuando un
     * enemigo recibe un proyectil del jugador que no le hiere
     * 
     * @param tipo el tipo de enemigo que ha recibido el disparo
     */
    public void enemigoDisparadoIncorrecto(TipoEnemigo tipo)
    {
        switch (tipo)
        {
            case TipoEnemigo.Vidrio:
                intentosVidrio++;
                break;
            case TipoEnemigo.Organica:
                intentosOrganica++;
                break;
            case TipoEnemigo.PapelCarton:
                intentosPapelCarton++;
                break;
            case TipoEnemigo.PlasticoBriksLatas:
                intentosPlasticoBrikLatas++;
                break;
            default:
                break;
        }

        puntuacion -= 50;
        UIController.instance.actualizarPuntuacion(puntuacion);
        UIController.instance.informarJugador(-50);

    }

    /**
     * Función que se encarga de actualizar la información referente cuando un
     * enemigo es herido por un proyectil del jugador
     * 
     * @param tipo el tipo de enemigo que ha recibido el disparo
     * @param muerto si el enemigo ha muerto al recibir el disparo o no
     */
    public void enemigoDisparadoCorrecto(TipoEnemigo tipo, bool muerto)
    {
        switch (tipo)
        {
            case TipoEnemigo.Vidrio:
                intentosVidrio++;
                aciertosVidrio++;
                break;
            case TipoEnemigo.Organica:
                intentosOrganica++;
                aciertosOrganica++;
                break;
            case TipoEnemigo.PapelCarton:
                intentosPapelCarton++;
                aciertosPapelCarton++;
                break;
            case TipoEnemigo.PlasticoBriksLatas:
                intentosPlasticoBrikLatas++;
                aciertosPlasticoBrikLatas++;
                break;
            default:
                break;

        }

        disparosAcertados++;
        if (muerto)
        {
            puntuacion += 150;
            UIController.instance.actualizarPuntuacion(puntuacion);
            UIController.instance.informarJugador(150);
        }
        else
        {
            puntuacion += 100;
            UIController.instance.actualizarPuntuacion(puntuacion);
            UIController.instance.informarJugador(100);
        }
    }

    /**
     * Función que devuelve la puntuación actual del jugador
     * 
     * @return la puntuación actual
     */
    public float getPuntuacion()
    {
        return puntuacion;
    }

    /**
     * Función que devuelve la nota final del jugador
     * 
     * @return la nota final
     */
    public float getNotaFinal()
    {
        if (disparosRealizados == 0) return 0;
        double nota = System.Math.Truncate((disparosAcertados / disparosRealizados) * 1000);
        return (float)nota / 100;
    }

    /**
     * Función que se encarga de guardar la información en el archivo correspondiente
     * 
     */
    public void guardarDatos()
    {
        nombreCarpeta = SystemInfo.deviceName;
        System.IO.Directory.CreateDirectory(nombreCarpeta);
        nombreArchivo = nombreCarpeta + "/" + SystemInfo.deviceName + " " + instance.marcaTemporal + ".csv";
        StreamWriter wr = new StreamWriter(nombreArchivo);

        wr.Write(SystemInfo.deviceName);
        wr.Write(",");
        wr.Write(instance.marcaTemporal);
        wr.Write(",");
        wr.Write(instance.getNotaFinal().ToString().Replace(',', '.'));
        wr.Write(",");
        wr.Write((instance.tiempoFinal - instance.tiempoInicial).ToString().Replace(',', '.'));
        wr.Write(",");
        wr.Write(instance.puntuacion);
        wr.Write(",");

        wr.Write(instance.basuraOrganicaBP);
        wr.Write(",");
        wr.Write(instance.basuraVidrioBP);
        wr.Write(",");
        wr.Write(instance.basuraPBLBP);
        wr.Write(",");
        wr.Write(instance.basuraPCBP);
        wr.Write(",");
        wr.Write(instance.basuraOrganicaAP);
        wr.Write(",");
        wr.Write(instance.basuraVidrioAP);
        wr.Write(",");
        wr.Write(instance.basuraPBLAP);
        wr.Write(",");
        wr.Write(instance.basuraPCAP);
        wr.Write(",");

        wr.Write(instance.intentosVidrio);
        wr.Write(",");
        wr.Write(instance.aciertosVidrio);
        wr.Write(",");

        wr.Write(instance.intentosOrganica);
        wr.Write(",");
        wr.Write(instance.aciertosOrganica);
        wr.Write(",");

        wr.Write(instance.intentosPapelCarton);
        wr.Write(",");
        wr.Write(instance.aciertosPapelCarton);
        wr.Write(",");

        wr.Write(instance.intentosPlasticoBrikLatas);
        wr.Write(",");
        wr.Write(instance.aciertosPlasticoBrikLatas);
        wr.Write(",");

        wr.Write(instance.intentosVidrioTotales);
        wr.Write(",");
        wr.Write(instance.aciertosVidrioTotales);
        wr.Write(",");

        wr.Write(instance.intentosOrganicaTotales);
        wr.Write(",");
        wr.Write(instance.aciertosOrganicaTotales);
        wr.Write(",");

        wr.Write(instance.intentosPapelCartonTotales);
        wr.Write(",");
        wr.Write(instance.aciertosPapelCartonTotales);
        wr.Write(",");

        wr.Write(instance.intentosPlasticoBrikLatasTotales);
        wr.Write(",");
        wr.Write(instance.aciertosPlasticoBrikLatasTotales);
        wr.Write(",");

        wr.Write(instance.disparosRealizados);
        wr.Write(",");
        wr.Write(instance.disparosAcertados);
        wr.Write(",");
        
        wr.Write(instance.disparosRealizadosTotales);
        wr.Write(",");
        wr.Write(instance.disparosAcertadosTotales);
        wr.Write(",");
        
        wr.Write(instance.vecesHerido);
        wr.Write(",");
        wr.Write(instance.vecesMuerto);
        wr.Write(",");
        wr.Write(instance.juegoTerminado);
        wr.Write(",");
        wr.Write(instance.gradoSatisfaccion.ToString().Replace(',', '.'));

        wr.Write(Environment.NewLine);
        wr.Close();
    }

    /**
     * Función que se reinicia la información del tutorial, ya que esta la queremos
     * desechar
     * 
     */
    public void tutorialTerminado()
    {

        vecesHerido = 0;
        vecesMuerto = 0;

        intentosVidrio = 0;
        intentosOrganica = 0;
        intentosPapelCarton = 0;
        intentosPlasticoBrikLatas = 0;

        intentosVidrioTotales = 0;
        intentosOrganicaTotales = 0;
        intentosPapelCartonTotales = 0;
        intentosPlasticoBrikLatasTotales = 0;

        aciertosVidrio = 0;
        aciertosOrganica = 0;
        aciertosPapelCarton = 0;
        aciertosPlasticoBrikLatas = 0;

        aciertosVidrioTotales = 0;
        aciertosOrganicaTotales = 0;
        aciertosPapelCartonTotales = 0;
        aciertosPlasticoBrikLatasTotales = 0;

        disparosRealizados = 0;
        disparosAcertados = 0;

        disparosRealizadosTotales = 0;
        disparosAcertadosTotales = 0;
    }

    /**
     * Función que se encarga de añadir a la información total la información del nivel actual
     * 
     */
    public void acumularTotales()
    {
        intentosVidrioTotales += intentosVidrio;
        intentosOrganicaTotales += intentosOrganica;
        intentosPapelCartonTotales += intentosPapelCarton;
        intentosPlasticoBrikLatasTotales += intentosPlasticoBrikLatas;

        aciertosVidrioTotales += aciertosVidrio;
        aciertosOrganicaTotales += aciertosOrganica;
        aciertosPapelCartonTotales += aciertosPapelCarton;
        aciertosPlasticoBrikLatasTotales += aciertosPlasticoBrikLatas;

        disparosRealizadosTotales += disparosRealizados;
        disparosAcertadosTotales += disparosAcertados;
    }
}
