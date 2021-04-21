using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del enemigo de
 * basura de papel y cartón
 * 
 * @author David Real Ortega
 */
public class PapelCartonController : MonoBehaviour
{
    //Indica si el enemigo se está moviendo
    private bool moviendo = false;
    //Indica si el enemigo está atacando
    private bool atacando = false;
    //Indica si el disparo se encuentra disponible
    private bool disparoDisponible = true;
    //Posición de disparo
    public Transform posicionDisparo;
    //Vida del enemigo
    private int vida = 2;
    //Posición a la que va a disparar
    public Transform ultimaPosicionEnemigo;
    //Prefab del proyectil que va a disparar
    public GameObject prefabProyectil;
    //Distancia maxima a la que se puede acercar el jugador
    public float distanciaMaxima = 5;

    //Posicion inicial del movimiento
    private Vector3 posicionInicial;
    //Posicion final del movimiento
    private Vector3 posicionFinal;
    //Tiempo de inicio del movimiento
    private float tiempoInicio;
    //Tiempo actual del movimiento
    private float tiempo;
    //Tiempo en el que el enemigo realiza el movimiento
    private float tiempoMovimiento = 1.5f;
    //Cantidad de desplazamiento que realiza el enemigo
    private float desplazamiento = 25.0f;

    //SpriteRenderer ligado al enemigo
    public SpriteRenderer spriterenderer;
    //Color del enemigo
    private Color colorOriginal;
    //Color que mostrará el enemigo cuando lo dañen
    public Color dmgColor;
    //Sistema de Partículas que muestra al destruirse
    public GameObject sistemaParticulas;
    //Fuente de audio que reproduce el sonido del enemigo
    public AudioSource fuenteAudio;
    //Variable que indica si el enemigo está orientado hacia la derecha o no
    private bool direccionDerecha = false;

    /**
     * Start es invocada antes de la actualización del primer frame
     */
    void Start()
    {
        colorOriginal = spriterenderer.color;
    }

    /**
     *  Update se invoca una vez por frame
     */
    void Update()
    {
        //Si nos estamos moviendo realizamos la interpolación lineal entre la posición inicial y la final
        if (moviendo)
        {
            tiempo = Time.time - tiempoInicio;
            if (tiempo <= tiempoMovimiento)
            {
                float t = tiempo / tiempoMovimiento;
                Vector3 pos = linearInterpolation(posicionInicial, posicionFinal, t);
                transform.position = pos;
            }
            else
            {
                moviendo = false;
            }
        }
    }

    /**
     * Función que se encarga de iniciar el ataque del enemigo
     */
    public void enemigoDetectado(Transform posicionEnemigo)
    {
        if (!atacando)
        {
            ultimaPosicionEnemigo = posicionEnemigo;
            StartCoroutine("ShootCoroutine");
            atacando = true;
        }
    }

    /**
     * Función que se encarga de detener el ataque del enemigo
     */
    public void enemigoPerdido()
    {
        StopCoroutine("ShootCoroutine");
        atacando = false;
    }

    /**
     * Función que se encarga de recibir la información sobre la posición
     * del enemigo
     */
    public void recibirPosicionEnemigo(Transform posicionEnemigo)
    {
        ultimaPosicionEnemigo = posicionEnemigo;

        if (Vector3.Distance(posicionEnemigo.position, transform.position) <= distanciaMaxima && !moviendo)
        {
            moviendo = true;
            posicionInicial = transform.position;
            posicionFinal = transform.position;
            tiempoInicio = Time.time;

            if (posicionEnemigo.position.x < gameObject.transform.position.x)
            {
                posicionFinal.x -= desplazamiento;
            }
            if (posicionEnemigo.position.x > gameObject.transform.position.x)
            {
                posicionFinal.x += desplazamiento;
            }
        }
    }

    /**
     * Corrutina que se encarga de realizar el disparo
     */
    IEnumerator ShootCoroutine()
    {
        for (; ; )
        {
            //Si no nos estamos moviendo, atacamos
            if (!moviendo)
            {
                if (disparoDisponible)
                {
                    Vector3 direccion = ultimaPosicionEnemigo.position - posicionDisparo.position;
                    float distancia = Vector3.Distance(ultimaPosicionEnemigo.position, transform.position);
                    Quaternion q = Quaternion.FromToRotation(Vector3.up, direccion / distancia);
                    GameObject objeto = Instantiate(prefabProyectil, posicionDisparo.position, q);
                    objeto.GetComponent<PapelCartonProjectileController>().moverProyectil(direccion / distancia);
                    disparoDisponible = false;
                    StartCoroutine("CooldownCoroutine");
                }
            }
            yield return new WaitForSeconds(1.65f);
        }
    }

    /**
     * Corrutina que se encarga de gestionar el cooldown del
     * disparo y de detenerlo si el enemigo ya no se encuentra
     * en modo de ataque
     */
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(1.65f);
        disparoDisponible = true;
        if (!atacando) StopCoroutine("ShootCoroutine");
    }

    /**
     * Función que se encarga de girar el sprite
     */
    void girarSprite()
    {
        direccionDerecha = !direccionDerecha;
        Vector3 escala = transform.localScale;
        escala.x = -escala.x;
        transform.localScale = escala;
    }

    /**
     * Función que realiza la interpolación lineal
     */
    public static Vector3 linearInterpolation(Vector3 inicial, Vector3 fin, float alpha)
    {
        if (alpha == 0)
            return inicial;

        if (alpha == 1)
            return fin;

        Vector3 diferencia = fin - inicial;
        return (inicial + alpha * diferencia);
    }

    /**
     * Función que gestiona el comportamiento del enemigo cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {

        //Si el proyectil que nos llega es del tipo PapelCarton nos hiere
        if (col.gameObject.tag == "BalaPC")
        {
            //Restamos un punto de vida y lanzamos la corrutina que muestra que lo han herido
            vida--;
            StartCoroutine("DmgCoroutine");

            //Nos destruimos si la vida es 0 o si el proyectil era potenciado
            if (vida == 0 || col.gameObject.GetComponent<ProjectileController>().getProyectilPotenciado())
            {
                Destroy(gameObject);
                //Creamos el sistema de partículas
                Instantiate(sistemaParticulas, transform.position, transform.rotation);
                ScoreController.instance.enemigoDisparadoCorrecto(ScoreController.TipoEnemigo.PapelCarton, true);
            }
            else
            {
                ScoreController.instance.enemigoDisparadoCorrecto(ScoreController.TipoEnemigo.PapelCarton, false);
                fuenteAudio.Play();
            }

        }

        //Si el proyectil es de otro tipo únicamente se informa al ScoreController
        if (col.gameObject.tag == "BalaOrganica" || col.gameObject.tag == "BalaPBL" || col.gameObject.tag == "BalaVidrio")
        {
            ScoreController.instance.enemigoDisparadoIncorrecto(ScoreController.TipoEnemigo.PapelCarton);
        }


    }

    /**
     * Función que se encarga de colorear el enemigo de rojo
     * indicando que este ha sido herido
     */
    IEnumerator DmgCoroutine()
    {
        spriterenderer.color = dmgColor;
        yield return new WaitForSeconds(.5f);
        spriterenderer.color = colorOriginal;
    }

}
