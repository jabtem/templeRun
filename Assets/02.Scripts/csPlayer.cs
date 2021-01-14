using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csPlayer : MonoBehaviour
{
    GameObject manager; //Bridge Manager
    public int speedForward = 2; //전진속도 
    public int speedSide = 6; //옆걸음 속도 
    public int jumpPower = 300; //점프
    bool canJump = true; //점프가능 
    bool canTurn = false; //회전가능 
    bool canLeft = true; //왼쪽이동가능
    bool canRight = true; //오른쪽이동가능 
    bool isGround = true; //바닥에 있는지? 
    bool isDead = false; //죽었나?
    float dirX = 0; //좌우이동방향 > -1:왼쪽 1:오른쪽 
    float score = 0;
    Vector3 touchStart; //모바일기기의 Touch 시작위치

    Animation anim;


    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        manager = GameObject.Find("BridgeManager");
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == true)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        CheckMove(); //이동 및 점프가능여부 체크 
        MoveHuman(); //주인공 이동
        score += Time.deltaTime * 1000;//득점처리
    }

    void CheckMove()
    {
        RaycastHit hit;
        //디버그용 
        Debug.DrawRay(transform.position, Vector3.down * 2f, Color.red); 
        Debug.DrawRay(transform.position, Vector3.left * 0.7f, Color.red); 
        Debug.DrawRay(transform.position, Vector3.right * 0.7f, Color.red);

        isGround = true;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.transform.tag == "BRIDGE")
                isGround = true;
        }
        canLeft = true;
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 0.7f))
        {
            if (hit.transform.tag == "GUARD")
                canLeft = false;
        }
        canRight = true;
        if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.7f))
        {
            if (hit.transform.tag == "GUARD")
                canRight = false;
        }
    }

    void MoveHuman()
    {
        dirX = 0;
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            CheckMobile();
        }
        else
        {
            CheckKeyboard();
        }
        Vector3 moveDir = new Vector3(dirX * speedSide, 0, speedForward);
        transform.Translate(moveDir * Time.deltaTime);
    }

    void CheckMobile()
    {
        float x = Input.acceleration.x;
        if(canLeft && isGround)
        {
            if (x < -0.2f)
                dirX = -0.6f;
            else if (x > 0.2f)
                dirX = 0.6f;
        }
        foreach(Touch tmp in Input.touches)
        {
            if(tmp.phase == TouchPhase.Began)
            {
                touchStart = tmp.position;
            }
            if (tmp.phase == TouchPhase.Moved)
            {
                Vector3 touchEnd = tmp.position;
                if (isGround && canJump && touchEnd.y - touchStart.y > 100) 
                { 
                    StartCoroutine("JumpHuman"); 
                }
                if (canTurn && touchEnd.x - touchStart.x < -100) 
                { 
                    RotateHuman("LEFT"); 
                }
                if (canTurn && touchEnd.x - touchStart.x > 100) 
                {
                    RotateHuman("RIGHT"); 
                }
            }
        }
    }

    void CheckKeyboard()
    {
        if(isGround)
        {
            dirX = Input.GetAxis("Horizontal");
            if(canJump && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine("JumpHuman");
            }
           
        }
        if(canTurn)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                RotateHuman("LEFT");
            if (Input.GetKeyDown(KeyCode.E))
                RotateHuman("RIGHT");
        }
    }

    IEnumerator JumpHuman()
    {
        canJump = false;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower);
        anim.Play("jump_pose");
        yield return new WaitForSeconds(1);
        anim.Play("run");
        canJump = true;
    }

    void RotateHuman(string sDir) 
    {
        canTurn = false;
        Vector3 rot = transform.eulerAngles;

        switch(sDir)
        {
            case "LEFT":
                rot.y -= 90;
                break;
            case "RIGHT":
                rot.y += 90;
                break;
        }
        transform.eulerAngles = rot;
        manager.SendMessage("MakeBridge", sDir, SendMessageOptions.DontRequireReceiver);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "DEAD")
        {
            isDead = true;
            anim.Play("idle");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        switch(other.transform.tag)
        {
            case "TURN":
                canTurn = true;
                break;
            case "COIN":
                score += 1000;
                Destroy(other.gameObject);
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "TURN")
        {
            canTurn = false;
        }
    }

    void OnGUI()
    {
        string str = "<size=20><color=#000000>score: ##</color></size>";

        GUI.Label(new Rect(10, 10, 300, 80), str.Replace("##", "" + (int)score));

        if (!isDead)
            return;

        int w = Screen.width / 2;
        int h = Screen.height / 2;

        if(GUI.Button(new Rect(w-60,h-50,120,50),"Play Game"))
        {
            SceneManager.LoadScene("Main");
        }
        if (GUI.Button(new Rect(w - 60, h + 50, 120, 50), "Quit Game"))
        {
            Application.Quit();
        }

    }
}
