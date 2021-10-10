using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite cartaOriginal;        // Sprite da carta desejada
    public Sprite backCarta;            // Sprite do avesso da carta
    // Start is called before the first frame update
    void Start()
    {
        EscondeCarta();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        GameObject.Find("gameManager").GetComponent<ManageCartas>().CartaSelecionada(gameObject);
    }

    public void EscondeCarta()
    {
        GetComponent<SpriteRenderer>().sprite = backCarta;
    }

    public void RevelaCarta()
    {
        GetComponent<SpriteRenderer>().sprite = cartaOriginal;
    }

    public void SetCartaOriginal(Sprite novaCarta)
    {
        cartaOriginal = novaCarta;
    }
}
