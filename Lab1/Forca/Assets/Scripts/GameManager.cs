using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject letra;                      // prefab da letra no Game
    public GameObject centro;                     // objeto de texto que indica o centro da tela

    private string   palavraOculta;               // palavra a ser descoberta
    private string[] palavrasOcultas;             // array de palavras ocultas
    private int      numeroAleatorio;
    private int      tamanhoPalavraOculta;        // tamanho da palavra oculta
    private char[]   letrasOcultas;               // letras da palavra oculta
    private bool[]   letrasDescobertas;           // indicador de quais letras foram descobertas

    private int numTentativas;                    // armazena as tentativas válidas da rodada 
    private int maxNumTentativas;                 // número máximo de tentativas para Forca ou Salvação
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            centro = GameObject.Find("centroDaTela");
            InitGame();
            InitLetras();
            UpdateNumTentativas();
            UpdateScore();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckTeclado();
    }

    // Inicia as letras da palavra oculta
    void InitLetras()
    {
        int numLetras = tamanhoPalavraOculta;

        for (int i = 0; i < numLetras; i++)
        {
            Vector3 novaPosicao = new Vector3(centro.transform.position.x + ((i-numLetras/2.0f)*80), centro.transform.position.y, centro.transform.position.z);
            GameObject l        = Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name              = "letra" + (i + 1);
            var canvas          = GameObject.Find("Canvas");
            l.transform.SetParent(canvas.transform);
        }
    }

    // Inicializa as variáveis do jogo, como a palavra oculta e o número de tentativas
    void InitGame()
    {
        palavraOculta        = PegaUmaPalavraDoArquivo().ToUpper();
        tamanhoPalavraOculta = palavraOculta.Length;
        letrasOcultas        = palavraOculta.ToCharArray();
        letrasDescobertas    = new bool[tamanhoPalavraOculta];
        numTentativas        = -1;
        maxNumTentativas     = 10;
    }

    // Verifica os eventos do teclado e verifica se a letra teclada correspnde a alguma letra da palavra oculta
    // Atualiza o score e o número de tentativas caso a escolha for correta
    void CheckTeclado()
    {
        if (Input.anyKeyDown)
        {
            char letraTeclada = Input.inputString
                .ToCharArray()
                .FirstOrDefault();

            int codeLetraTeclada = Convert.ToInt32(letraTeclada);
            Debug.Log((char) codeLetraTeclada);
            if (codeLetraTeclada >= 97 && codeLetraTeclada <= 122)
            {
                UpdateNumTentativas();

                if (numTentativas > maxNumTentativas)
                {
                    SceneManager.LoadScene("Lab1_Forca");
                }

                for (int i = 0; i < tamanhoPalavraOculta; i++)
                {
                    if (!letrasDescobertas[i])
                    {
                        letraTeclada = Char.ToUpper(letraTeclada);
                        if (letrasOcultas[i] == letraTeclada)
                        {
                            letrasDescobertas[i] = true;
                            var textLetra        = GameObject.Find("letra" + (i + 1)).GetComponent<Text>();
                            textLetra.text       = letraTeclada.ToString();
                            UpdateScore();
                            VerificaPalavraDescoberta();
                        }
                    }
                }
            }
        }
    }

    // Atualiza o número de tentativas
    void UpdateNumTentativas()
    {
        numTentativas++;
        GameObject.Find("numTentativas").GetComponent<Text>().text = numTentativas + " / " + maxNumTentativas;
    }

    // Atualiza a pontuação do jogo
    void UpdateScore()
    {
        score = PlayerPrefs.GetInt("score") + 1;
        PlayerPrefs.SetInt("score", score);
        GameObject.Find("scoreUI").GetComponent<Text>().text = "Score: " + score;
    }

    // Verifica se a palavra foi descoberta pelo jogador e redireciona para a tela final caso retorne seja descoberta
    void VerificaPalavraDescoberta()
    {
        bool condicao = true;

        for (int i = 0; i < tamanhoPalavraOculta; i++)
        {
            condicao = condicao && letrasDescobertas[i];
        }

        if (condicao)
        {
            PlayerPrefs.SetString("ultimaPalavraOculta", palavraOculta);
            SceneManager.LoadScene("Lab1_Salvo");
        }
    }

    // Inicializa a palavra oculta escolhendo aleatoriamente uma dentre um conjunto de palavras presentes em um arquivo
    string PegaUmaPalavraDoArquivo()
    {
        TextAsset txtAsset   = (TextAsset)Resources.Load("palavras", typeof(TextAsset));
        string[] palavras    = txtAsset.text.Split(' ');
        int palavraAleatoria = UnityEngine.Random.Range(0, palavras.Length + 1);

        return palavras[palavraAleatoria];
    }

}
