using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ManageCartas : MonoBehaviour
{
    public GameObject carta;
    public const int quantidadeCartas = 13; // Quantidade de cartas do baralho
    public const int quantidadeLinhas = 2; // Quantidade de linhas a serem apresentadas no jogo 
    
    List<Carta> cartasEscolhidas; // Lista das cartas escolhidas pelo jogador
    AudioSource audio; // Som feito quando o jogador encontra um par de cartas
    bool timerAcionado = false; // Indicador para atualizar a rodada
    float timer; // Contador do tempo do fim da rodada

    int numTentativas = -1; // Numero de tentativas do jogador. Começa -1 para que no Start fique zero
    int numAcertos = 0; // Quantidade de acertos que o jogador fez
    int tentativasUltimoJogo; // Pontuação feita na última partida
    public class Carta // GameObjects das cartas selecionadas pelo jogador
    {
        public string Linha { get; set; } // Posição da linha da carta
        public bool Selecionada { get; set; } // Indicador de que a carta foi selecionada
        public GameObject GameObject { get; set; } // O GameObject atrelado à carta
        public Carta()
        {
            Selecionada = false;
        }

        public void Reset()
        {
            Linha = string.Empty;
            Selecionada = false;
            GameObject = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            cartasEscolhidas = CriarCartas(2);
            SetAudio();
            MostrarPontuacaoUltimoJogo();

            MostrarCartas();
            UpdateTentativas();
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
    }

    void SetAudio()
    {
        audio = GameObject.Find("gameManager").GetComponent<AudioSource>();
    }
    void MostrarPontuacaoUltimoJogo()
    {
        tentativasUltimoJogo = PlayerPrefs.GetInt("Jogadas", 0);
        GameObject.Find("ultimaJogada").GetComponent<Text>().text = "Ultimo Jogo = " + tentativasUltimoJogo;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (timerAcionado)
            {
                timer += Time.deltaTime;

                if (timer > 1)
                {
                    timerAcionado = false;
                    timer = 0;

                    string tag = cartasEscolhidas.First().GameObject.tag;
                    bool todasIguais = cartasEscolhidas.All(c => c.GameObject.tag == tag); // Se todas as cartas forem iguais, fica verdadeiro, ou seja, todas com a mesma tag

                    foreach (var c in cartasEscolhidas)
                    {
                        if (todasIguais)
                            Destroy(c.GameObject);
                        else
                            c.GameObject.GetComponent<Tile>().EscondeCarta();

                        c.Reset();
                    }

                    if (todasIguais) UpdateQuantidadeAcertos();

                    UpdateTentativas();
                }
            }

            if (numAcertos == quantidadeCartas)
            {
                PlayerPrefs.SetInt("Jogadas", numTentativas);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
        
    }

    void UpdateQuantidadeAcertos()
    {
        numAcertos++;
        audio.Play();

        GameObject.Find("numAcertos").GetComponent<Text>().text = "Acertos = " + numAcertos;
    }

    // Cria a lista para armazenar as cartas que serão selecionadas pelo jogador
    List<Carta> CriarCartas(int total)
    {
        List<Carta> cartas = new List<Carta>();

        for (int i = 0; i < total; i++)
            cartas.Add(new Carta());

        return cartas;
    }

    // Coloca na Scene todas as cartas do jogo
    void MostrarCartas()
    {
        List<int[]> cartasEmbaralhadas = CriarCartasEmbaralhadas();

        for (int i = 0; i < quantidadeLinhas; i++)
        {
            for (int j = 0; j < quantidadeCartas; j++)
                AdicionarUmaCarta(i, j, cartasEmbaralhadas[i][j]);
        }
    }

    // Cria o conjunto de cartas para cada linha que será apresentada
    List<int[]> CriarCartasEmbaralhadas()
    {
        List<int[]> cartas = new List<int[]>();
        for (int i = 0; i < quantidadeLinhas; i++)
            cartas.Add(CriarArrayEmbaralhado());

        return cartas;
    }
    int[] CriarArrayEmbaralhado()
    {
        int[] novoArray = Enumerable.Range(0, quantidadeCartas).ToArray();
        for (int i = 0; i < quantidadeCartas; i++)
        {
            int temp = novoArray[i];
            int r = UnityEngine.Random.Range(i, quantidadeCartas - 1);
            novoArray[i] = novoArray[r];
            novoArray[r] = temp;
        }

        return novoArray;
    }

    // Estabelece a carta do baralho que será mostrada quando o jogador clicar nela
    void AdicionarUmaCarta(int linha, int rank, int valor)
    {
        GameObject c = ClonarCarta(linha, rank, valor);

        string nomeCarta = GetNomeCarta(valor);

        Sprite s = Resources.Load<Sprite>(nomeCarta);
        GameObject.Find(c.name).GetComponent<Tile>().SetCartaOriginal(s);
    }

    // Cria um clone do Prefab Tile, para mostrar diversas cartas na Scene
    GameObject ClonarCarta(int linha, int rank, int valor)
    {
        Vector3 novaPosicao = GetPosicaoClone(linha, rank);

        GameObject c = Instantiate(carta, novaPosicao, Quaternion.identity);
        c.tag = valor.ToString();
        c.name = linha + "_" + rank + "_" + valor;

        return c;
    }

    // Calcula a posição que carta ficará na Scene 
    Vector3 GetPosicaoClone(int linha, int indice)
    {
        float escalaX = carta.transform.localScale.x;
        float escalaY = carta.transform.localScale.y;

        float fatorEscalaX = (650 * escalaX) / 110.0f;
        float fatorEscalaY = (945 * escalaY) / 110.0f;

        float deslocamentoX = (indice - (quantidadeCartas / 2)) * fatorEscalaX;
        float deslocamentoY = (linha - (quantidadeLinhas / 2)) * fatorEscalaY;

        GameObject centro = GameObject.Find("centroDaTela");
        float posX = centro.transform.position.x + deslocamentoX;
        float posY = centro.transform.position.y + deslocamentoY;

        return new Vector3(posX, posY, 0);
    }

    // Recupera o nome da carta pelo o valor atribuído à ela
    string GetNomeCarta(int valor)
    {
        string numeroCarta;

        if (valor == 0)
            numeroCarta = "ace";
        else if (valor == 10)
            numeroCarta = "jack";
        else if (valor == 11)
            numeroCarta = "queen";
        else if (valor == 12)
            numeroCarta = "king";
        else
            numeroCarta = (valor + 1).ToString();

        return numeroCarta + "_of_clubs";
    }

    // Coloca a carta na lista que de cartas que o jogador selecionou
    public void CartaSelecionada(GameObject carta)
    {
        if (!CartaValida(carta)) return;

        int qtdCartasEscolhidas = 0;
        foreach (var escolhida in cartasEscolhidas)
        {
            qtdCartasEscolhidas += 1;
            if (!escolhida.Selecionada)
            {
                escolhida.Selecionada = true;
                escolhida.Linha = carta.name.Substring(0, 1);
                escolhida.GameObject = carta;
                escolhida.GameObject.GetComponent<Tile>().RevelaCarta();
                break;
            }
        }

        if (qtdCartasEscolhidas == cartasEscolhidas.Count)
            VerificaCartas();

    }

    // Valida de se a carta escolhida pelo jogador é válida de entrar na lista das escolhidas
    // O nome dela deve ser diferente das que já existem e não pode ser escolhida após ter escolhido o par
    bool CartaValida(GameObject carta)
    {
        if (cartasEscolhidas == null || carta == null) return false;

        var cartasSelecionadas = cartasEscolhidas.Where(c => c.GameObject != null).ToList();

        return cartasSelecionadas.All(c => c.GameObject.name != carta.name) && !timerAcionado;
    }

    // Verifica se as cartas são iguais e determina um delay para a próxima rodada
    void VerificaCartas()
    {
        DisparaTimer();
    }

    // Habilita a verificação das cartas
    void DisparaTimer()
    {
        timerAcionado = true;
    }

    // Atualiza a quantidade de tentativas feita pelo jogador e mostra na Scene
    void UpdateTentativas()
    {
        numTentativas++;
        GameObject.Find("numTentativas").GetComponent<Text>().text = "Tentativas = " + numTentativas;
    }
}
