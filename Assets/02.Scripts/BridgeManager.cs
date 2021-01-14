using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    public GameObject[] bridges;
    public GameObject[] coins;
    public GameObject bridgeTurn;

    GameObject newBridge;

    GameObject childBridge;

    GameObject oldBridge;

    GameObject bridge;
    GameObject coin;

    bool canCoin = false;

    int dir = 0;

    Quaternion quatAng;


    // Start is called before the first frame update
    void Start()
    {
        newBridge = GameObject.Find("StartBridge");
        oldBridge = GameObject.Find("OldBridge");
        childBridge = newBridge;

        MakeBridge("FORWARD");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeBridge(string sDir)
    {
        DeleteOldBridge();
        CalcRotation(sDir);
        MakeNewBridge();
    }

    void DeleteOldBridge()
    {
        Destroy(oldBridge);
        oldBridge = newBridge;

        newBridge = new GameObject("StartBridge");
    }

    void CalcRotation(string sDir)
    {
        switch(sDir)
        {
            case "LEFT":
                dir--;
                break;
            case "RIGHT":
                dir++;
                break;
        }

        //회전방향 제한
        if (dir < 0) dir = 3;
        if (dir > 3) dir = 0;

        quatAng.eulerAngles = new Vector3(0, dir * 90, 0);
    }

    void MakeNewBridge()
    {
        for(int i=0; i<10; i++)
        {
            bridge = bridges[0];
            coin = coins[Random.Range(0, 3)];

            canCoin = false;

            SelectBridge(i);

            Vector3 pos = Vector3.zero;

            Vector3 localPos = childBridge.transform.localPosition;

            switch(dir)
            {
                case 0:
                    pos = new Vector3(localPos.x, 0, localPos.z + 10);
                    break;
                case 1:
                    pos = new Vector3(localPos.x+10, 0, localPos.z);
                    break;
                case 2:
                    pos = new Vector3(localPos.x, 0, localPos.z - 10);
                    break;
                case 3:
                    pos = new Vector3(localPos.x-10, 0, localPos.z);
                    break;
            }

            Debug.Log(bridge);

            childBridge = Instantiate(bridge, pos, quatAng) as GameObject;
            childBridge.transform.parent = newBridge.transform;

            if(canCoin)
            {
                childBridge = Instantiate(coin, pos, quatAng) as GameObject;
                childBridge.transform.parent = newBridge.transform;
            }
        }
    }

    void SelectBridge(int n)
    {
        switch(n)
        {
            case 9:
                bridge = bridgeTurn;
                break;
            case 1 :
            case 3 :
            case 5 :
            case 7 :
                bridge = bridges[Random.Range(0, bridges.Length)];
                break;
            default :
                if(Random.Range(0,100)>50)
                {
                    canCoin = true;
                }
                break;
            
        }
    }
}
