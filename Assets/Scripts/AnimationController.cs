using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Sprite[] idleSprites;
    [SerializeField] int speedAnim;
    int count;
    SpriteRenderer spriteRenderer;
    public int countFrame;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = idleSprites[0];
    }
    
    // Update is called once per frame
    void Update()
    {
        countFrame = Time.frameCount;
        if (Time.frameCount % speedAnim == 0)
        {
            count++;
            if (count > idleSprites.Length)
                count = 0;
            spriteRenderer.sprite = idleSprites[count];
        }
        
    }
}
