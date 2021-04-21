using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Clase que se encarga de gestionar el comportamiento del jugador
 * y la entrada del usuario
 * 
 * @author David Real Ortega
 */
public class PlayerController : MonoBehaviour
{
    //Atributo que indica si el jugador se encuentra orientado hacia la derecha
    private bool direccionDerecha = true;
    //Atributo que se actualiza en función de la entrada del usuario para saber si este se quiere mover hacia la izquierda o hacia la derecha
    private float direccionMovimientoActual;
    //Atributo que indica la velocidad de movimiento horizontal del personaje
    private float velocidadMovimiento = 10f;
    //Atributo que indica la fuerza con la que salta el jugador
    private float fuerzaSalto = 12.5f;
    //Atributo que indica la fuerza con la que cae el jugador
    private float fuerzaCaida = -2.5f;
    //Atributo que indica si el jugador tiene disponible el salto
    private bool saltoDisponible = true;
    //Atributo que se actualiza cuando el jugador es herido para saber que está realizando dicha animación
    private bool jugadorHerido = false;
    //Atributo que establece si el jugador está vivo o no
    private bool jugadorMuerto = false;

    //Vida actual del jugador
    private int vidaActual = 5;
    //Vida con la que el jugador comienza la partida
    private int vidaInicial = 5;


    //Fuente de audio que reproduce los sonidos del jugador
    public AudioSource fuenteAudio;
    //Clip que se reproduce cuando el jugador es Herido
    public AudioClip clipHerido;
    //Clip que se reproduce al recoger el power up de vida
    public AudioClip clipPowerUpVida;
    //Clip que se reproduce al recoger el power up de proyectil potenciado
    public AudioClip clipPowerUpProyectil;
    //Clip que se reproduce al disparar un proyectil normal
    public AudioClip clipDisparo;
    //Clip que se reproduce al disparar un proyectil potenciado
    public AudioClip clipDisparoPotenciado;

    //Sistema de partículas que se genera cuando el jugador dispara un proyectil potenciado
    public GameObject sistemaChispas;
    //Atributo que indica si el siguiente disparo del jugador será potenciado
    private bool proyectilPotenciado;

    //Canvas que muestra al usuario feedback sobre los disparos realizados 
    public Canvas canvas;
    //Animator que controla las animaciones del jugador
    public Animator animator;
    //Rigidbody 2D ligado al cuerpo del jugador
    public Rigidbody2D rigidBody;

    /**
     * Enum que se utiliza para diferenciar el proyectil que genera el jugador
     * en cada disparo
     */
    private enum ProyectilDisparo
    {
        PapelCarton,
        Vidrio,
        Plastico,
        Organica
    };

    //Atributo que indica el proyectil que toca generar en función de la entrada del usuario
    private ProyectilDisparo proyectilDisparado;

    //Prefabs de los diferentes tipos de proyectiles que genera el jugador
    public GameObject prefabPapelCarton;
    public GameObject prefabVidrio;
    public GameObject prefabPlastico;
    public GameObject prefabOrganica;

    //Posicion desde la que el jugador dispara los proyectiles
    public Transform posicionDisparo;

    //Atributo que indica si el disparo se encuentra disponible
    private bool disparoDisponible = true;
    //Atributo que indica si el jugador se encuentra realizado la animación de disparo
    private bool realizandoDisparo = false;
    //Atributo que indica el tiempo de cooldown de disparo
    private float tiempoCooldown = 1.2f;

    //Atributo que indica el tiempo de invulnerabilidad del jugador cuando este es herido.
    private float tiempoInvulnerabilidad = 1f;
    //SpriteRenderer asociado al jugador
    public SpriteRenderer sprRenderer;

    /**
     * Start es invocada antes de la actualización del primer frame
     */
    void Start()
    {

        //Inicializamos las variables acorde al estado inicial del juego
        jugadorMuerto = false;
        vidaActual = vidaInicial;
        proyectilPotenciado = false;
        GameManager.instance.inicializarVida();

        //Ajustamos las físicas para que no ignore ninguna colisión con los enemigos.
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("EnemyBullets"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Enemigos"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Detectores"), false);

    }

    /**
     *  Update se invoca una vez por frame
     */
    void Update()
    {
        //Si el jugador no está muerto y el juego no está en pausa, gestionamos la entrada del usuario
        if (!jugadorMuerto && !GameManager.instance.getJuegoPausado())
        {
            //En función de la tecla pulsada por el usuario, almacenamos el tipo de proyectil que desea disparar y realizamos la animación de disparo.
            if (Input.GetKeyDown(KeyCode.Q) && disparoDisponible)
            {
                realizarAnimacionDisparo();
                proyectilDisparado = ProyectilDisparo.Organica;
            }
            if (Input.GetKeyDown(KeyCode.W) && disparoDisponible)
            {
                realizarAnimacionDisparo();
                proyectilDisparado = ProyectilDisparo.PapelCarton;
            }
            if (Input.GetKeyDown(KeyCode.E) && disparoDisponible)
            {
                realizarAnimacionDisparo();
                proyectilDisparado = ProyectilDisparo.Plastico;
            }
            if (Input.GetKeyDown(KeyCode.R) && disparoDisponible)
            {
                realizarAnimacionDisparo();
                proyectilDisparado = ProyectilDisparo.Vidrio;
            }

            //Si el jugador pulsa la tecla de salto y se cumplen las condiciones para que el salto esté disponible, se realiza el salto
            if (Input.GetButtonDown("Jump") && saltoDisponible && !jugadorHerido && !realizandoDisparo)
            {
                //Se actualiza el animator, se modifica el rigidbody y se actualiza el estado del jugador..
                animator.SetBool("estaSaltando", true);
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, fuerzaSalto);
                saltoDisponible = false;
            }

            //Almacenamos la dirección en la que el jugador quiere moverse y actualizamos el animator
            direccionMovimientoActual = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimientoActual));

            //Si el jugador no se encuentra relizando el disparo, lo movemos
            if (!realizandoDisparo)
            {
                rigidBody.velocity = new Vector2(direccionMovimientoActual * velocidadMovimiento, rigidBody.velocity.y);
            }
            else
            {
                //Si el jugador se encuentra realizando la animación de disparo, hacemos que no se pueda mover horizontalmente.
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);

            }

            //Si el jugador desea moverse hacia la derecha y está mirando hacia la izquierda, lo giramos
            if (direccionMovimientoActual == 1)
            {
                if (!direccionDerecha)
                    girarSprite();
            }

            //Si el jugador desea moverse hacia la izquierda y está mirando hacia la derecha, lo giramos
            if (direccionMovimientoActual == -1)
            {
                if (direccionDerecha)
                    girarSprite();
            }


        }

        //Gestionamos la entrada del usuario cuando pulsa el botón de pausa
        if (!jugadorMuerto)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.instance.pausarJuego();
            }
        }
    }

    /**
     * Función que se encarga de girar el sprite
     */
    void girarSprite()
    {
        //Actualizamos el estado del jugador
        direccionDerecha = !direccionDerecha;
        //Actualizamos la escala del jugador
        Vector3 escala = transform.localScale;
        escala.x = -escala.x;
        transform.localScale = escala;
        //Hacemos que el canvas se mantenga siempre orientado hacia la derecha, ya que no queremos que las letras salgan al revés
        Vector3 scale = canvas.transform.localScale;
        scale.x = -scale.x;
        canvas.transform.localScale = scale;
    }

    /**
     * Función que se encarga de actualizar las variables correspondientes del animator
     * para que el jugador haga la animación de disparo
     */
    void realizarAnimacionDisparo()
    {
        //Si el jugador se encuentra en el aire, desactivamos dichas animaciones
        if (!saltoDisponible)
        {
            animator.SetBool("estaSaltando", false);
            animator.SetBool("estaCayendo", false);
        }

        //Activamos la animación de disparo
        animator.SetBool("estaDisparando", true);

        //Actualizamos el estado del jugador
        disparoDisponible = false;
        realizandoDisparo = true;
    }

    /**
     * Función que se encarga de generar el proyectil deseado por el jugador cuando la
     * animación de disparo alcanza cierto frame.
     */
    void shootAnimationEnd()
    {
        //Rotamos el proyectil en función de la orientación del jugador
        Quaternion rotacion = posicionDisparo.rotation;
        if (!direccionDerecha) rotacion.y = 180;
        //GameObject que crearemos
        GameObject proyectil = null;
        //Indicamos al Score Controller que hemos realizado un disparo
        ScoreController.instance.disparoRealizado();

        //En función de la entrada del usuario, crearemos un proyectil u otro
        switch (proyectilDisparado)
        {
            case ProyectilDisparo.Organica:
                proyectil = Instantiate(prefabOrganica, posicionDisparo.position, rotacion);
                break;
            case ProyectilDisparo.PapelCarton:
                proyectil = Instantiate(prefabPapelCarton, posicionDisparo.position, rotacion);
                break;
            case ProyectilDisparo.Plastico:
                proyectil = Instantiate(prefabPlastico, posicionDisparo.position, rotacion);
                break;
            case ProyectilDisparo.Vidrio:
                proyectil = Instantiate(prefabVidrio, posicionDisparo.position, rotacion);
                break;
            default:
                break;
        };

        //Configuramos el proyectil en función  de si este es potenciado o no
        if (proyectilPotenciado)
        {
            if (proyectil != null)
                proyectil.GetComponent<ProjectileController>().configurarProyectil(clipDisparoPotenciado, true);
            generarChispas();
            proyectilPotenciado = false;
        }
        else
        {
            if (proyectil != null)
                proyectil.GetComponent<ProjectileController>().configurarProyectil(clipDisparo, false);
        }

        //Indicamos que la animación de disparo ha acabado
        animator.SetBool("estaDisparando", false);

        //Actualizamos el animator si el jugador se encuentra en el aire
        if (!saltoDisponible)
        {
            animator.SetBool("estaCayendo", true);
        }

        //Lanzamos la corrutina de cooldown de disparo
        StartCoroutine("ShootCooldownCoroutine");
    }

    /**
     * Corrutina que se encarga de indicar que el jugador ya no se encuentra realizando
     * un disparo y de indicar que este puede volver a disparar transcurrido un determinado
     * tiempo
     */
    IEnumerator ShootCooldownCoroutine()
    {
        //Primero indicamos que el jugador ya no se encuentra realizando el disparo
        yield return new WaitForSeconds(0.1f);
        realizandoDisparo = false;
        //Pasado el tiempo de cooldown indicamos que el jugador tiene el disparo disponible de nuevo
        yield return new WaitForSeconds(tiempoCooldown);
        disparoDisponible = true;
    }

    /**
     * Función que gestiona el comportamiento del jugador cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        //Comportamiento si chocamos con un proyectil enemigo o con el cuerpo de un enemigo
        if (col.gameObject.tag == "EnemyBullet" || col.gameObject.tag == "Enemy")
        {
            //Restamos un punto de vida
            vidaActual--;

            //Actualizamos la información del ScoreController
            ScoreController.instance.vecesHerido++;

            //Reproducimos el sonido correspondiente
            fuenteAudio.clip = clipHerido;
            fuenteAudio.Play();

            //Pedimos al GameManager que actualice la interfaz con la vida correspondiente
            GameManager.instance.actualizarVida(vidaActual, false);

            if (vidaActual > 0)
            {
                //Ignoramos las colisiones con los proyectiles enemigos y con los cuerpos de estos
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("EnemyBullets"), true);
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Enemigos"), true);

                //Iniciamos la animación correspondiente
                animator.SetBool("recibidoDanio", true);
                //Si se encuentra en el aire desactivamos la animación correspondiente.
                if (!saltoDisponible)
                {
                    animator.SetBool("estaSaltando", false);
                    animator.SetBool("estaCayendo", false);
                }

                //Actualizamos el estado del jugador
                jugadorHerido = true;

                //Lanzamos la corrutina que desactiva la invulnerabilidad del jugador
                StartCoroutine("InvulnerabilityCoroutine");
            }
            else if (vidaActual == 0)
            {
                //Actualizamos la información del ScoreController
                ScoreController.instance.vecesMuerto++;

                //Giramos al jugador para que su movimiento de muerte se corresponda con la dirección del último proyectil recibido
                if (col.transform.position.x < transform.position.x && direccionDerecha) girarSprite();
                if (col.transform.position.x > transform.position.x && !direccionDerecha) girarSprite();

                //Iniciamos la animación de muerte y desactivamos el resto
                animator.SetBool("muerte", true);
                animator.SetFloat("Velocidad", 0);
                if (!saltoDisponible)
                {
                    animator.SetBool("estaSaltando", false);
                    animator.SetBool("estaCayendo", false);
                }
                if (realizandoDisparo)
                {
                    animator.SetBool("estaDisparando", false);
                }

                //Actualizamos el movimiento del jugador
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
                //Actualizamos el estado del jugador
                jugadorMuerto = true;

                //Hacemos que los enemigos dejen de detectar al jugador
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Detectores"), true);


            }
        }
        //Comportamiento si chocamos con el suelo
        else if (col.gameObject.tag == "Floor")
        {
            //Si es porque el jugador ha muerto actualizamos el animator y restringimos su movimiento horizonal
            if (jugadorMuerto)
            {
                animator.SetBool("muerte", false);
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }
            else
            {
                //En caso contrario, desactivamos la animación de caida o salto e indicamos que este se encuentra disponivle
                animator.SetBool("estaSaltando", false);
                animator.SetBool("estaCayendo", false);
                saltoDisponible = true;
            }
        }

        //Comportamiento si llegamos a la zona objetivo
        else if (col.gameObject.tag == "EndPoint")
        {
            //Actualizamos el estado del jugador para que dejar de gestionar su entrada
            jugadorMuerto = true;
            //Indicamos al GameManager que hemos llegado al punto final
            GameManager.instance.alcanzadoPuntoFinal();

        }

        //Comportamiento si cogemos un power up para potenciar el proyectil
        else if (col.gameObject.tag == "PowerUpDmg")
        {
            //Actualizamos el estado del jugador, reproducimos el sonido correspondiente y destruimos el power up recogido
            proyectilPotenciado = true;
            //Reproducimos el sonido correspondiente
            fuenteAudio.clip = clipPowerUpProyectil;
            fuenteAudio.Play();
            //Destruimos el powerUp
            Destroy(col.gameObject);
        }

        //Comportamiento si cogemos un power up de curación
        else if (col.gameObject.tag == "PowerUpVida")
        {
            //Actualizamos el estado del jugador, reproducimos el sonido correspondiente
            if (vidaActual < vidaInicial)
            {
                //Reproducimos el sonido correspondiente
                fuenteAudio.clip = clipPowerUpVida;
                fuenteAudio.Play();
                GameManager.instance.actualizarVida(vidaActual, true);
                vidaActual++;
            }

            //En cualquier caso, destruimos el powerUp.
            Destroy(col.gameObject);
        }

    }

    /**
     * Función que gestiona el comportamiento del jugador cuando este sale de
     * una colisión
     *
     * @param other los datos asociados a la colisión
     */
    void OnCollisionExit2D(Collision2D other)
    {
        //Comportamiento si el jugador abandona el suelo
        if (other.gameObject.tag == "Floor")
        {
            if (saltoDisponible && !jugadorMuerto)
            {
                //Actualizamos el estado del jugador y activamos la animación correspondiente
                saltoDisponible = false;
                animator.SetBool("estaCayendo", true);
            }
        }
    }

    /**
     * Función que actualiza el estado del jugador cuando la
     * animación de daño alcanza cierto frame.
     */
    public void dmgAnimationEnd()
    {
        //Desactivamos la animación
        animator.SetBool("recibidoDanio", false);

        //Si el jugador se encuentra en el aire, activamos la animación correspondiente
        if (!saltoDisponible)
        {
            animator.SetBool("estaCayendo", true);
            animator.SetBool("estaSaltando", false);
        }

        //Actualizamos el estado del jugador
        jugadorHerido = false;

    }

    /**
     * Corrutina que se encarga de indicar que el jugador ya no es invulnerable al daño
     * transcurrido un determinado tiempo y de hacerlo parpadear durante dicho tiempo
     */
    IEnumerator InvulnerabilityCoroutine()
    {
        //Realizamos el parpadeo del jugador
        for (int i = 0; i < tiempoInvulnerabilidad / Time.deltaTime; i++)
        {
            sprRenderer.enabled = !sprRenderer.enabled;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //Evitamos que el jugador pueda quedarse transparente
        sprRenderer.enabled = true;

        //Ajustamos las físicas para que no ignore ninguna colisión con los enemigos.
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("EnemyBullets"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Enemigos"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Jugador"), LayerMask.NameToLayer("Detectores"), false);


    }

    /**
     * Corrutina que se encarga de informar al GameManager de que el jugador ha muerto
     * tras un pequeño delay
     */
    IEnumerator deadCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.jugadorMuerto();
    }

    /**
     * Función que se encarga de mover el cuerpo del jugador cuando este muere
     */
    void movimientoMuerte()
    {
        //En función de la dirección, lo lanzamos con una velocidad u otra
        if (direccionDerecha)
        {
            rigidBody.velocity = new Vector2(-5, -10);
        }
        else
        {
            rigidBody.velocity = new Vector2(5, -10);
        }

        //Lanzamos la corrutina de muerte
        StartCoroutine("deadCoroutine");

    }

    /**
     * Función que se encarga de asignarle al jugador una fuerza de caida
     */
    void realizarCaida()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, fuerzaCaida);

    }

    /**
     * Función que se encarga de generar el sistema de partículas cuando el jugador
     * realiza un disparo potenciado
     */
    private void generarChispas()
    {
        //Rotamos el sistema de partículas en función de la orientación del jugador
        Quaternion rotacion = sistemaChispas.transform.rotation;
        if (!direccionDerecha)
            rotacion.y = -rotacion.y;

        //Asignamos la posición para que este se vea en el frente
        Vector3 position = posicionDisparo.position;
        position.z = -1;

        //Creamos el sistema de partículas
        Instantiate(sistemaChispas, posicionDisparo.position, rotacion);

    }

}
