using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public GameObject cameraHolder;
    public GameObject tileMover;
    public RawImage overlay;

    public float mouseSens;
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;
    public float jumpForce;
    public float gravity;

    public Vector3 cameraOffset;
    public Vector3 camRotation;

    Vector3 moveDir = Vector3.zero;

    public bool canMove = true;

    CharacterController controller;
    Animator anim;

    bool dead = false;


    public float currentStamina = 100;
    public float staminaLoss;
    public float staminaGain;
    public bool staminaEmpty = false;
    public RawImage staminaBar;

    public RawImage healthBar;
    public TextMeshProUGUI healthText;

    public float health;
    public float coins;

    public int powerUpAmmount = 0;


    public GameObject speedBar;
    public GameObject slownessBar;
    public GameObject coinBar;
    public TextMeshProUGUI coinsText;
    public GameObject jumpBar;
    public GameObject textBar;

    public float coinMultiplier = 1;

    private Vector3 lastPosition;

    bool lastGroundCheck = true;
    Vector3 startJumpPos;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        lastPosition = transform.position;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        overlay.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            for (float i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.01f);
                overlay.GetComponent<RawImage>().color = new Color(0, 0, 0, (100 - i) / 100);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        coinsText.text = coins.ToString();
        health = Mathf.Clamp(health, -1, 101);

        Vector2 healthBarSize = healthBar.GetComponent<RectTransform>().sizeDelta;

        healthText.text = (Mathf.RoundToInt(healthBarSize.x / 10)).ToString() + "/100";


        if (healthBarSize.x - health * 10 > 5 && !dead)
        {
            healthBar.GetComponent<RectTransform>().sizeDelta -= new Vector2(Time.deltaTime*250, 0);
        }
        else if (healthBarSize.x - health * 10 < -5 && !dead)
        {
            healthBar.GetComponent<RectTransform>().sizeDelta += new Vector2(Time.deltaTime*250, 0);
        }
        if (healthBarSize.x <= 5)
        {
            dead = true;
            EndGame();
        }

        if (transform.position.y < -2 && !dead)
        {
            dead = true;
            EndGame();
        }

        Vector3 forward = cameraHolder.transform.TransformDirection(Vector3.forward);
        Vector3 right = cameraHolder.transform.TransformDirection(Vector3.right);


        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            float yrotation = ClampAngle(cameraHolder.transform.eulerAngles.y + mouseInput.x * Time.deltaTime * 10 * mouseSens, -45, 45);
            float xrotation = ClampAngle(cameraHolder.transform.eulerAngles.x - mouseInput.y * Time.deltaTime * 10 * mouseSens/5, -0.5f, 45);
           
            cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
            cameraHolder.transform.eulerAngles = new Vector3(xrotation, yrotation, 0);
        }
        else
        {
            cameraHolder.transform.eulerAngles = new Vector3(cameraHolder.transform.eulerAngles.x, Mathf.Round(cameraHolder.transform.eulerAngles.y), cameraHolder.transform.eulerAngles.z);
        }


        bool isRunning = staminaEmpty ? false : Input.GetKey(KeyCode.LeftControl);
        bool strafing = Input.GetAxis("Vertical") != 0 && Input.GetAxis("Horizontal") != 0; 
        float curSpeedX = canMove ? (strafing ? 0.75f : 1) * (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (strafing ? 0.75f : 1) * (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        if (!dead)
        {
            if (isRunning && Mathf.Abs(curSpeedX) > 0)
            {
                currentStamina -= staminaLoss * Time.deltaTime;
            }
            if (!staminaEmpty && currentStamina < 100 && !isRunning)
            {
                currentStamina += staminaGain * Time.deltaTime;
            }
            if (staminaEmpty)
            {
                currentStamina += staminaGain / 2 * Time.deltaTime;
                if(currentStamina > 50)
                {
                    staminaEmpty = false;
                    StartCoroutine(LerpImgColor(staminaBar, new Color(1, 1, 1), 0.5f));
                }
            }
            if (currentStamina <= 0 && !staminaEmpty)
            {
                staminaEmpty = true;
                StartCoroutine(LerpImgColor(staminaBar, new Color(0, 0, 0), 0.1f));
            }
            if(!staminaEmpty && currentStamina < 25 && currentStamina > 20)
            {
                StartCoroutine(LerpImgColor(staminaBar, new Color(1, 0, 0), 0.3f));
            }
            if(currentStamina > 25 && !staminaEmpty)
            {
                if(staminaBar.color != new Color(1, 1, 1))
                {
                    StartCoroutine(LerpImgColor(staminaBar, new Color(1, 1, 1), 0.5f));
                }
            }



            staminaBar.GetComponent<RectTransform>().sizeDelta = new Vector2(1000 * currentStamina / 100, 40);
        }

        float playerHeight = controller.bounds.extents.y;
        float raycastDistance = playerHeight + 0.2f;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, raycastDistance);


        if(isGrounded && !lastGroundCheck && startJumpPos.y >= transform.position.y-0.1f)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        float movementDirectionY = moveDir.y;
        moveDir = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButtonDown("Jump") && canMove && isGrounded)
        {
            startJumpPos = transform.position;
            moveDir.y = jumpForce;
            anim.SetBool("Jump", true);
        }
        else
        {
            moveDir.y = movementDirectionY;
        }
        if (!isGrounded)
        {
            moveDir.y -= gravity * Time.deltaTime;
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }

        Vector3 direction = (transform.position - lastPosition).normalized;
        if (direction.sqrMagnitude > 0.01f)
        {
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetYaw, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        lastPosition = transform.position;
        lastGroundCheck = isGrounded;


        controller.Move(moveDir * Time.deltaTime);

        tileMover.transform.position = transform.position;

        cameraHolder.transform.position = new Vector3(0 + cameraOffset.x, transform.position.y + cameraOffset.y, transform.position.z + cameraOffset.z);
    }

    public void EndGame()
    {
        canMove = false;
        StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            float widthx = healthBar.GetComponent<RectTransform>().sizeDelta.x;

            for (float i = 0; i < 50; i++)
            {
                healthBar.GetComponent<RectTransform>().sizeDelta -= new Vector2(widthx / 50, 0);
                yield return new WaitForSeconds(0.01f);
                overlay.GetComponent<RawImage>().color = new Color(0, 0, 0, i * 2 / 100);
            }
            overlay.GetComponent<RawImage>().color = new Color(0, 0, 0, 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;

        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }

    private IEnumerator LerpImgColor(RawImage img, Color targetColor, float lerpDuration)
    {
        float elapsedTime = 0f;
        Color currentColor = img.color;

        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            img.color = Color.Lerp(currentColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        img.color = targetColor;
    }

    public IEnumerator CoinMult()
    {
        powerUpAmmount++;
        coinMultiplier *= 2f;
        RectTransform bar = coinBar.GetComponent<RectTransform>();
        bar.anchoredPosition = GetPosForBar();
        coinBar.SetActive(true);

        for (float i = 0; i < 3000; i++)
        {
            bar.sizeDelta = new Vector2(340 - i / 3000 * 340, 30);
            yield return new WaitForSeconds(0.01f);
        }
        coinBar.SetActive(false);
        powerUpAmmount--;
        coinMultiplier /= 2f;
    }

    public IEnumerator JumpBoost()
    {
        powerUpAmmount++;
        jumpForce *= 1.5f;
        RectTransform bar = jumpBar.GetComponent<RectTransform>();
        bar.anchoredPosition = GetPosForBar();
        jumpBar.SetActive(true);

        for (float i = 0; i < 1500; i++)
        {
            bar.sizeDelta = new Vector2(340 - i / 1500 * 340, 30);
            yield return new WaitForSeconds(0.01f);
        }
        jumpBar.SetActive(false);
        powerUpAmmount--;
        jumpForce /= 1.5f;
    }

    public IEnumerator SpeedBoost()
    {
        powerUpAmmount++;
        walkSpeed *= 1.5f;
        runSpeed *= 1.5f;
        RectTransform bar = speedBar.GetComponent<RectTransform>();
        bar.anchoredPosition = GetPosForBar();
        speedBar.SetActive(true);

        for (float i = 0; i < 1500; i++)
        {
            bar.sizeDelta = new Vector2(340 - i / 1500 * 340, 30);
            yield return new WaitForSeconds(0.01f);
        }
        speedBar.SetActive(false);
        powerUpAmmount--;
        walkSpeed /= 1.5f;
        runSpeed /= 1.5f;
    }
    public IEnumerator Slowness()
    {
        powerUpAmmount++;
        walkSpeed /= 1.5f;
        runSpeed /= 1.5f;
        RectTransform bar = slownessBar.GetComponent<RectTransform>();
        bar.anchoredPosition = GetPosForBar();
        slownessBar.SetActive(true);

        for (float i = 0; i < 1500; i++)
        {
            bar.sizeDelta = new Vector2(340 - i / 1500 * 340, 30);
            yield return new WaitForSeconds(0.01f);
        }
        slownessBar.SetActive(false);
        powerUpAmmount--;
        walkSpeed *= 1.5f;
        runSpeed *= 1.5f;
    }

    Vector2 GetPosForBar()
    {
        switch (powerUpAmmount)
        {
            case 1: return new Vector2(-708f, -505);
            case 2: return new Vector2(-708f, -460);
            case 3: return new Vector2(-708f, -415);
            case 4: return new Vector2(-708f, -373);
            default: return new Vector2(-708f, -505);
        }
    }

    public void WriteText(string text)
    {
        TextMeshProUGUI bar = textBar.GetComponent<TextMeshProUGUI>();
        bar.fontSize = 40;
        bar.text = text;

        StopCoroutine(ShrinkText());

        StartCoroutine(ShrinkText());

        IEnumerator ShrinkText()
        {
            for (int i = 0; i < 52; i++)
            {
                bar.fontSize = i;
                yield return new WaitForSeconds(1 / 52);
            }

            yield return new WaitForSeconds(3);
            for (int i = 0; i < 52; i++)
            {
                if(bar.text == text)
                {
                    bar.fontSize = 52 - i;
                    yield return new WaitForSeconds(1 / 52);
                }
            }

            bar.text = "";
        }
    }

}
