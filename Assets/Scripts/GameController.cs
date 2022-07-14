using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int numRow;
    public int numCol;

    public GameObject hexagonObj;
    public GameObject hexagonFrameObj;
    public static GameObject hexagonFrame;
    public float upBorder;

    public static List<Hexagon> hexagons = new List<Hexagon>();
    public List<GameObject> hexagonFloors = new List<GameObject>();

    public List<Vector3> newHexPos = new List<Vector3>();
    private float newHexCounter = 0.2f;

    public int score;
    public int highScore;
    public Text scoreText;
    public Text highScoreText;
    public Button resetButton;
    public GameObject MainCamera;
    public float bombNum;
    public List<Hexagon> bombHexagons = new List<Hexagon>();

    public Color[] colors;

    public static bool isCreating = false;

    void Start()
    {
        bombNum = 1;
        score = 0;
        highScore = 0;
        highScoreText.text = "High Score: " + highScore.ToString();

        scoreText.text = "Score: " + score.ToString();
        colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta };
        StartCoroutine(CreateBoard());
        hexagonFrame = Instantiate(hexagonFrameObj);
        hexagonFrame.transform.position = new Vector3(100, 100, 100);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit hit;
            Physics.Raycast(mousePos, Vector3.forward, out hit, 15);
            if (hit.collider != null && !hexagonFrame.GetComponent<HexagonFrame>().canRotate)
            {
                hexagonFrame.GetComponent<HexagonFrame>().setHexParent(null);
                hexagonFrame.transform.position = moveHexagonFrame(mousePos, hit.collider.gameObject);
                hexagonFrame.GetComponent<HexagonFrame>().selectHexagons();
                hexagonFrame.GetComponent<HexagonFrame>().setHexParent(hexagonFrame.transform);
            }
        }

        if (hexagonFrame != null && !hexagonFrame.GetComponent<HexagonFrame>().canRotate)
            if (newHexCounter <= 0 && newHexPos.Count > 0)
            {
                createIndividualHex(newHexPos[0]);
                newHexPos.Remove(newHexPos[0]);
                newHexCounter = 0.2f;
            }
            else
            {
                newHexCounter -= Time.deltaTime;
            }
    }

    public void RotateSelected()
    {
        hexagonFrame.GetComponent<HexagonFrame>().RotateByAngle(360);
        bombCounterDecrease();
    }

    public void bombCounterDecrease()
    {
        if (bombHexagons.Count > 0)
        {
            for (int i = 0; i < bombHexagons.Count; i++)
            {
                bombHexagons[i].countDown.text = (Convert.ToInt32(bombHexagons[i].countDown.text) - 1).ToString();

                if (bombHexagons[i].countDown.text == "0")
                {
                    BoardClean();
                }
            }
        }
    }

    public void updateScore(int score)
    {
        this.score += score;
        scoreText.text = "Score: " + this.score.ToString();

        if (this.score >= 1000 * bombNum)
        {
            bombNum++;
            int a = UnityEngine.Random.Range(0, hexagons.Count);

            bombHexagons.Add(hexagons[a]);
            hexagons[a].countDown.text = UnityEngine.Random.Range(5, 15).ToString();
        }
    }

    public void colorInitalizeControl()
    {
        for (int i = 0; i < hexagons.Count; i++)
        {
            hexagons[i].checkForInitialization(colors);
        }
    }

    public void destroySignedHexagons()
    {
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].willDestroyed)
            {
                if (hexagonFrame.GetComponent<HexagonFrame>().canRotate)
                    hexagonFrame.GetComponent<HexagonFrame>().canRotate = false;

                if (hexagons[i].countDown.text != "")
                    bombHexagons.Remove(hexagons[i]);

                updateScore(5);
                newHexPos.Add(new Vector3(hexagons[i].transform.position.x, upBorder + 3, hexagons[i].transform.position.z));
                Destroy(hexagons[i].gameObject);
                hexagons.Remove(hexagons[i]);
                hexagonFrame.GetComponent<HexagonFrame>().setHexParent(null);
            }
        }
    }

    public Vector3 moveHexagonFrame(Vector3 mousePos, GameObject hitGameObject)
    {
        if (hitGameObject.transform.parent == null)
            return hexagonFrame.transform.position;

        Vector3 hexagonPos = hitGameObject.transform.parent.transform.position;

        float x = mousePos.x - hexagonPos.x;
        float y = mousePos.y - hexagonPos.y;

        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);

        Vector3 hexagonFramePos;

        if (x > 0 && y > 0)
        {
            if (absX > absY)
            {
                hexagonFramePos = hexagonPos + new Vector3(1, 0, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                hexagonFramePos = hexagonPos + new Vector3(0.5f, 0.86f, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (x > 0 && y < 0)
        {
            if (absX > absY)
            {
                hexagonFramePos = hexagonPos + new Vector3(1f, 0, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                hexagonFramePos = hexagonPos + new Vector3(0.5f, -0.86f, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (x < 0 && y > 0)
        {
            if (absX > absY)
            {
                hexagonFramePos = hexagonPos + new Vector3(-1f, 0, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                hexagonFramePos = hexagonPos + new Vector3(-0.5f, 0.86f, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            if (absX > absY)
            {
                hexagonFramePos = hexagonPos + new Vector3(-1f, 0, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                hexagonFramePos = hexagonPos + new Vector3(-0.5f, -0.86f, hexagonPos.z - 1);
                hexagonFrame.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        return hexagonFramePos;
    }

    public void BoardClean()
    {
        bombNum = 1;
        newHexPos.Clear();
        newHexCounter = 0.2f;

        if (highScore < score)
            highScore = score;
        highScoreText.text = "High Score: " + highScore.ToString();

        for (int i = 0; i < hexagons.Count; i++)
        {
            Destroy(hexagons[i].gameObject);
        }

        hexagonFrame.GetComponent<HexagonFrame>().setHexParent(null);
        hexagons.Clear();

        for (int i = 0; i < hexagonFloors.Count; i++)
        {
            Destroy(hexagonFloors[i].gameObject);
        }
        bombHexagons.Clear();
        hexagonFloors.Clear();
        StartCoroutine(CreateBoard());
        updateScore(-score);
    }

    public IEnumerator CreateBoard()
    {
        isCreating = true;
        float posX;
        if (numCol % 2 == 1)
        {
            posX = -((((Mathf.Ceil(numCol / 2f) - 2) * 2) + 2) + ((numCol - Mathf.Ceil(numCol / 2f)) * 1)) / 2f;
        }
        else
        {
            posX = -((((Mathf.Ceil((numCol + 1) / 2f) - 2) * 2) + 2) + (((numCol + 1) - Mathf.Ceil((numCol + 1) / 2f)) * 1)) / 2f;
        }

        float temp = posX;

        float posY = ((numRow - 1) * 1.72f) / 2f;

        upBorder = posY;
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                GameObject a = Instantiate(hexagonObj);
                if (j % 2 == 0)
                    a.transform.position = new Vector3(posX, posY, 0);
                else
                    a.transform.position = new Vector3(posX, posY - 0.86f, 0);

                posX += 1.5f;
                Hexagon aHex = a.GetComponent<Hexagon>();
                aHex.changeColor(colors[UnityEngine.Random.Range(0, colors.Length)]);
                hexagons.Add(aHex);
            }
            posY -= 1.72f;
            posX = temp;
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                GameObject a = Instantiate(hexagonObj.transform.GetChild(0).gameObject);
                hexagonFloors.Add(a);
                a.GetComponent<MeshRenderer>().enabled = false;
                if (j % 2 == 0)
                    a.transform.position = new Vector3(posX, posY, 0);
                else
                    a.transform.position = new Vector3(posX, posY - 0.86f, 0);

                posX += 1.5f;
            }
            posY -= 1.72f;
            posX = temp;
        }
        MainCamera.GetComponent<Camera>().orthographicSize = numRow;
        yield return new WaitForSeconds(0.1f);
        colorInitalizeControl();
        isCreating = false;
    }
    public void createIndividualHex(Vector3 pos)
    {
        GameObject a = Instantiate(hexagonObj);
        a.GetComponent<Hexagon>().changeColor(colors[UnityEngine.Random.Range(0, colors.Length)]);
        hexagons.Add(a.GetComponent<Hexagon>());
        a.transform.position = pos;
    }
}
