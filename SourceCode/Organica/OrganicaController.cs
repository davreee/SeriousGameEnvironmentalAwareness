using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del enemigo de
 * basura orgánica
 * 
 * @author David Real Ortega
 */
public class OrganicaController : MonoBehaviour
{
  
    //Indica si el enemigo está atacando o no
    private bool atacando = false;
    //Indica si el disparo se encuentra disponible o no
    private bool disparoDisponible = true;
    //Posiciones de disparo
    public Transform posicionDisparoUno;
    public Transform posicionDisparoDos;
    //Vida del enemigo
    private int vida = 2;
    //Prefab del proyectil que disparar
    public GameObject prefabProyectil;
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
     * Función que se encarga de iniciar el ataque del enemigo
     */
    public void enemigoDetectado()
    {
        if (!atacando)
        {
            StartCoroutine("ShootCoroutine");
            atacando = true;
        }
    }

    /**
     * Función que se encarga de detener el ataque del enemigo
     */
    public void enemigoPerdido()
    {
        atacando = false;
    }

    /**
     * Función que se encarga de recibir la información sobre la posición
     * del enemigo
     */
    public void recibirPosicionEnemigo(Transform posicionEnemigo)
    {
        if (posicionEnemigo.position.x > gameObject.transform.position.x && !direccionDerecha) girarSprite();
        if (posicionEnemigo.position.x < gameObject.transform.position.x && direccionDerecha) girarSprite();

    }

    /**
     * Corrutina que se encarga de realizar el disparo
     */
    IEnumerator ShootCoroutine()
    {
        for (; ; )
        {
            if (disparoDisponible)
            {
                //Indicamos que el disparo ya no está disponible e iniciamos el cooldown
                disparoDisponible = false;
                StartCoroutine("CooldownCoroutine");

                //Rotamos el proyectil en función de la orientación del enemigo
                Quaternion rotacion = posicionDisparoUno.rotation;
                if (!direccionDerecha) rotacion.y = 180;

                //Generamos los proyectiles
                Instantiate(prefabProyectil, posicionDisparoUno.position, rotacion);
                yield return new WaitForSeconds(0.75f);
                
                //Volvemos a comprobar la rotación por si el enemigo se gira en medio del ataque
                rotacion = posicionDisparoUno.rotation;
                if (!direccionDerecha) rotacion.y = 180;

                Instantiate(prefabProyectil, posicionDisparoDos.position, rotacion);
                yield return new WaitForSeconds(0.75f);
                
                rotacion = posicionDisparoUno.rotation;
                if (!direccionDerecha) rotacion.y = 180;
                
                Instantiate(prefabProyectil, posicionDisparoUno.position, rotacion);
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    /**
     * Corrutina que se encarga de gestionar el cooldown del
     * disparo y de detenerlo si el enemigo ya no se encuentra
     * en modo de ataque
     */
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
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
     * Función que gestiona el comportamiento del enemigo cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        //Si el proyectil que nos llega es del tipo Orgánica nos hiere
        if (col.gameObject.tag == "BalaOrganica")
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
                ScoreController.instance.enemigoDisparadoCorrecto(ScoreController.TipoEnemigo.Organica, true);
            }
            else
            {
                ScoreController.instance.enemigoDisparadoCorrecto(ScoreController.TipoEnemigo.Organica, false);
                fuenteAudio.Play();
            }

        }

        //Si el proyectil es de otro tipo únicamente se informa al ScoreController
        if (col.gameObject.tag == "BalaPC" || col.gameObject.tag == "BalaPBL" || col.gameObject.tag == "BalaVidrio")
        {
            ScoreController.instance.enemigoDisparadoIncorrecto(ScoreController.TipoEnemigo.Organica);
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
