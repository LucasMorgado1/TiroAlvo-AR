using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // CONTROLE AR
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager; //� o que detecta o ambiente (planos)
    private ARPlane plano; //variavel que vai salvar os ids encontrados
    [SerializeField] private GameObject alvo;
    [SerializeField] private GameObject alvoMovel;
    [SerializeField] private GameObject alvoMal;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image crosshair;
    // CONTROLE DO JOGO
    private int pontuacao;
    public int numeroMaxAlvos = 10;
    private int numeroInimigos = default;
    public int alvosAleatorios = 3;
    private bool acabou = false;

    // TIMER
    bool timerActive;
    float currentTime;
    public int startMinutes;
    [SerializeField] private TextMeshProUGUI currentTimeText;

    // REFERENCES
    private UI ui;

    private void Awake()
    {
        ui = GetComponent<UI>();
    }

    private void Start()
    {
        currentTime = startMinutes * 60;
    }

    void Update()
    {
        TratarTimer();

        if (acabou)
        {
            AparecerUIFimJogo();
            return;
        }

        TratarPlano();
        TratarMira();
    }

    #region Setup de Tela
    private void TratarPlano() 
    {
        if (plano != null) return;

        //ScreenPosition();

        var hits = new List<ARRaycastHit>();

        raycastManager.Raycast(ScreenPosition(), hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ComecarJogo(hits[0]); //aqui eu passo o ponto em que bateu enquanto tamb�m chamo a fun��o de comecar jogo
                timerActive = true;
            }
        }
    }

    private Vector3 ScreenPosition() 
    {
        Vector2 position = new Vector2(0.5f, 0.5f);
        return Camera.main.ViewportToScreenPoint(position);
    }
    #endregion
    private void ComecarJogo (ARRaycastHit hit)
    {
        var idPlano = hit.trackableId; //aqui eu atribuo o id do objeto em que bateu
        plano = planeManager.GetPlane(idPlano);

        transform.GetChild(0).gameObject.SetActive(false); //desativo o cursor
        planeManager.enabled = false;

        CriarAlvo(alvo);
    }

    #region Setup Alvo
    private Vector3 ObterPontoAleatorio() 
    {
        var x = Random.Range(-0.7f, 0.5f);
        var z = Random.Range(-0.7f, 0.5f);
        var y = Random.Range(.5f, .9f);

        var vetorAleatorio = new Vector3(x, y, z);

        var posicaoAleatoria = plano.transform.TransformPoint(vetorAleatorio); //pega um ponto aleat�rio no plano

        return posicaoAleatoria;
    }

    private void CriarAlvo(GameObject prefab) 
    {
        var pontoAleatorio = ObterPontoAleatorio();

        if (numeroInimigos < numeroMaxAlvos) { 

            GameObject target = GameObject.Instantiate(prefab, pontoAleatorio, Quaternion.identity);

            numeroInimigos++;

            if (target.CompareTag("Inimigo"))
                StartCoroutine(BombTimer(target));

            target.transform.LookAt(Camera.main.transform);
        }
    }

    private void RandomTargets()
    {
        for (int i = 0; i < alvosAleatorios; i++)
            CriarAlvo(TargetTypeRandomizer());
    }

    private GameObject TargetTypeRandomizer()
    {
        int value = Random.Range(0,101);

        switch (value)
        {
            case int n when n <= 20:
                return alvoMal;
            case int n when n <= 60:
                return alvoMovel;
            default:
                return alvo;
        }
    }

    #endregion

    private void TratarMira() 
    {
        RaycastHit hitInfo;

        var cam = Camera.main.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hitInfo, 1000000f))
        {
            if (hitInfo.transform.CompareTag("Alvo") || hitInfo.transform.CompareTag("Inimigo"))
                crosshair.color = Color.red;

            CheckTargetType(hitInfo);

        } else  
            crosshair.color = Color.white;

    }

    private void CheckTargetType(RaycastHit hit)
    {
        if (hit.transform.CompareTag("Inimigo") && InputCheck)
        {
            pontuacao -= 20;
            numeroInimigos--;
            score.text = "Score: " + pontuacao.ToString();
            Destroy(hit.transform.gameObject);
        }

        if (hit.transform.CompareTag("Alvo") && InputCheck)
        {
            TratarAcerto(hit.transform.gameObject);
        }

    }

    private bool InputCheck => Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;

    private void TratarAcerto(GameObject target)
    {
        pontuacao += target.GetComponentInParent<Target>().currentScore;
        score.text = "Score: " + pontuacao.ToString();
        numeroInimigos--;
        Destroy(target);
        AudioManager.Instance.Play("Bubble");
        if (currentTime > 0)
            RandomTargets();
        else
            acabou = true;
    }

    private void TratarTimer()
    {
        if (!timerActive) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            timerActive = false;
            AparecerUIFimJogo();
            Start();
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = "Timer: " + time.Seconds.ToString();
    }

    #region Fim de Jogo
    private void AparecerUIFimJogo () 
    {
        ui.onGame.SetActive(false);
        ui.onDefeat.SetActive(true);
        ui.score.text = "Your score: " + pontuacao;
    }

    public void TratarFimJogo ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator BombTimer (GameObject a) {
        yield return new WaitForSeconds(5f);
        if (a != null) {
            numeroInimigos--;
            Destroy(a);
        }
    }
    #endregion
}
