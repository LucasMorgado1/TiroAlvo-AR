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
    public ARPlaneManager planeManager; //é o que detecta o ambiente (planos)
    private ARPlane plano; //variavel que vai salvar os ids encontrados
    [SerializeField] private GameObject alvo;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject crosshair;

    // CONTROLE DO JOGO
    private int pontuacao;
    public int numeroAlvos = 15;
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
        if (!acabou)
        {
            TratarPlano();
            TratarMira();
        } else
        {
            AparecerUIFimJogo();
        }

        if (timerActive) 
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0) 
            {
                timerActive = false;
                AparecerUIFimJogo();
                Start();
            }
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = "Timer: " + time.Seconds.ToString(); 
    }

    private void TratarPlano() 
    {
        if (plano != null)
            return;

        ScreenPosition();

        var hits = new List<ARRaycastHit>();

        raycastManager.Raycast(ScreenPosition(), hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ComecarJogo(hits[0]); //aqui eu passo o ponto em que bateu enquanto também chamo a função de comecar jogo
                timerActive = true;
            }
        }
    }

    private Vector3 ScreenPosition() 
    {
        Vector2 position = new Vector2(0.5f, 0.5f);
        return Camera.main.ViewportToScreenPoint(position);
    }

    private void ComecarJogo (ARRaycastHit hit)
    {
        var idPlano = hit.trackableId; //aqui eu atribuo o id do objeto em que bateu
        plano = planeManager.GetPlane(idPlano);

        transform.GetChild(0).gameObject.SetActive(false); //desativo o cursor
        planeManager.enabled = false;

        CriarAlvo();
    }

    private Vector3 ObterPontoAleatorio() 
    {
        var x = Random.Range(-1f, 0.5f);
        var z = Random.Range(-1f, 0.5f);
        var y = Random.Range(.1f, .9f);

        var vetorAleatorio = new Vector3(x, y, z);

        var posicaoAleatoria = plano.transform.TransformPoint(vetorAleatorio); //pega um ponto aleatório no plano

        return posicaoAleatoria;
    }

    private void CriarAlvo() 
    {
        var pontoAleatorio = ObterPontoAleatorio();

        GameObject target = GameObject.Instantiate(alvo, pontoAleatorio, Quaternion.identity);

        target.transform.LookAt(Camera.main.transform);
    }

    private void TratarMira() 
    {
        RaycastHit hitInfo;

        var cam = Camera.main.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hitInfo, 1000000f))
        {
            if (hitInfo.transform.CompareTag("Alvo") &&  Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Destroy(hitInfo.transform.gameObject);
                TratarAcerto();
            }
        }
    }

    private void TratarAcerto()
    {
        pontuacao += 10;
        score.text = "Score: " + pontuacao.ToString();
        numeroAlvos--;
        if (numeroAlvos > 0 || currentTime > 0)
            CriarAlvo();
        else
            acabou = true;
    }

    private void AparecerUIFimJogo () 
    {
        ui.onGame.SetActive(false);
        ui.onDefeat.SetActive(true);
        ui.score = this.score;
    }

    public void TratarFimJogo ()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
