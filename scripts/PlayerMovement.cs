using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //parametre engine
    public Rigidbody2D body;
    public BoxCollider2D wallCheck;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public LayerMask wallMask;
    public Animator anim;

    //parametre generaux
    public float groundSpeed;
    public float jumpSpeed;
    [Range (0f, 1f)]
    public float drag;
    private Vector3 scale;
    
    //parametre du saut/movement

    private bool onWall;
    private bool grounded;
    private float wallJumpCooldown = 0f;
    public float xInput;
    public float yInput;
    private bool running;

    //parametre dash
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 25f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 1f;

    //effet de fantome/trainé lors du dash
    public TrailRenderer trail;
    public SpriteRenderer playerSprite;  
    public GameObject ghostPrefab;       
    public float ghostLifetime = 1f;     
    public float spawnInterval = 0.1f;   
    
    //initialisation du gameObject
    void Start() {
        scale = transform.localScale;
    }
    //1 fois par frame
    void Update() {
        if (isDashing) { return; }
        GetInput();
        HandleJump();
        HandleDash();
        wallJumpCooldown += 0.01f;
        anim.SetBool("run", running);
        anim.SetBool("grounded", grounded);
        anim.SetBool("dash", isDashing);
    }

    //peut tourner plus d'une fois par frame
    private void FixedUpdate() {
        if (isDashing) { return; }
        checkGround();
        checkWall();
        ApplyFriction();
        Move();
        if (grounded) { canDash = true; }
    }
    private void GetInput() {
        xInput = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.UpArrow)) {
            yInput = 1;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            yInput = -1;
        } else {
            float sign = Mathf.Sign(yInput);
            yInput =sign * Mathf.Clamp((Mathf.Abs(yInput) - 0.2f),0,1);
        }
    }

    private void Move() {
        if (Mathf.Abs(xInput) > 0 && wallJumpCooldown > 1f) {
            body.linearVelocity = new Vector2(xInput * groundSpeed, body.linearVelocity.y);
            SwitchSide(Mathf.Sign(xInput));
            running = true;
        } else {
            running = false;
        }
    }

    private void HandleJump() {
        if (Input.GetKeyDown(KeyCode.UpArrow) && grounded)        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
            anim.SetTrigger("jump");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && onWall)        {
            wallJumpCooldown = 0f;
            if (Mathf.Sign(transform.localScale.x) == -1)             {
                body.linearVelocity = new Vector2(groundSpeed*0.8f, jumpSpeed*0.8f);
                SwitchSide(1f);
            }
            else if (Mathf.Sign(transform.localScale.x) == 1)             {
                body.linearVelocity = new Vector2(-groundSpeed*0.8f, jumpSpeed*0.8f);
                SwitchSide(-1f);
            }
            anim.SetTrigger("jump");
        }
    }

    private void HandleDash() {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)        {
            float NS = 0;
            float WE = 0;
            GetInput();
            if (xInput > 0) {
                WE++;
            }
            if (xInput < 0) {
                WE--;
            }
            if (yInput > 0) {
                NS++;
            }
            if (yInput < 0) {
                NS--;
            }
            StartCoroutine(Dash(NS, WE));

        }
    }
    private void checkGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void checkWall() {
        onWall = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max,wallMask).Length > 0;
    }

    private void ApplyFriction() {
        if (grounded && xInput == 0f && yInput == 0)        {
            body.linearVelocity *= drag;
        }
    }

    private void SwitchSide(float Side)    {
        transform.localScale = new Vector3(Side * scale.x, scale.y, scale.z);
    }

    private IEnumerator Dash(float NS, float WE)    {
        canDash = false;
        isDashing = true;
        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;
        if (Mathf.Abs(WE) != Mathf.Abs(NS))         {
            body.linearVelocity = new Vector2(dashingPower * WE, dashingPower * NS);
        }
        else if (WE == 0 && NS == 0)         {
            body.linearVelocity = new Vector2(dashingPower * Mathf.Sign(transform.localScale.x), 0);
        }
        else         {
            body.linearVelocity = new Vector2(dashingPower * WE * 0.7f, dashingPower * NS * 0.7f);
        }
        trail.emitting = true;
        StartCoroutine(GhostEffect());
        yield return new WaitForSeconds(dashingTime);
        body.linearVelocity = new Vector2(xInput * groundSpeed*0.8f, body.linearVelocity.y*0.5f);
        trail.emitting = false;
        body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
    }

    IEnumerator GhostEffect()    {
        yield return new WaitForSeconds(0.001f);
        while (isDashing)        {
            CreateGhost();
            yield return new WaitForSeconds(spawnInterval);  
        }
    }

    void CreateGhost()    {
        
        GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        ghost.transform.position = playerSprite.transform.position;

        
        SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();
        ghostRenderer.sprite = playerSprite.sprite;
        ghostRenderer.transform.localScale = body.transform.localScale;
        ghostRenderer.color = playerSprite.color;  

        
        StartCoroutine(FadeOut(ghostRenderer, ghostLifetime));
    }

    IEnumerator FadeOut(SpriteRenderer ghost, float duration)    {
        float elapsed = 0f;
        Color startColor = ghost.color;

        while (elapsed < duration)        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration); 
            ghost.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(ghost);
    }
}
