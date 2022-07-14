using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hexagon : MonoBehaviour
{
    public Color hexColor;
    public bool willDestroyed = false;
    public Vector3[] corners;
    public Vector3[] cornersRot;
    public Text countDown;

    private void Start()
    {
        corners = new Vector3[] { new Vector3(-0.5f, 0.86f), new Vector3( 0.5f, 0.86f ), new Vector3( 1f, 0f),
        new Vector3( 0.5f, -0.86f ), new Vector3( -0.5f, -0.86f), new Vector3(-1f, 0f) };
        cornersRot = new Vector3[corners.Length]; 
    }

    public void changeColor(Color color)
    {
        hexColor = color;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = hexColor;
    }
    private void Update()
    {
        for (int i = 0; i < cornersRot.Length; i++)
        {
            cornersRot[i].x = corners[i].x * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - corners[i].y * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
            cornersRot[i].y = corners[i].y * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) + corners[i].x * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
        }
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 1);

        if (hit.collider == null && GameController.hexagonFrame != null && !GameController.hexagonFrame.GetComponent<HexagonFrame>().canRotate && !GameController.isCreating)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 25, transform.position.z);
            Physics.Raycast(transform.position, Vector3.down, out hit, 1);
            if (hit.collider != null)
            {
                transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + 1.72f, transform.position.z);
                checkForDestroy();
            }
        }
    }

    public void checkForDestroy()
    {
        int counter = 0;
        for (int i = 0; i < cornersRot.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + cornersRot[i], 0.2f);

            foreach (Collider collider in colliders)
            {
                if (collider.transform.parent != null && collider != transform.GetChild(0).GetComponent<MeshCollider>() && hexColor == collider.transform.parent.GetComponent<Hexagon>().hexColor)
                {
                    counter++;
                }
            }
            if (counter == 2)
            {
                willDestroyed = true;
                foreach (Collider collider in colliders)
                {
                    if (collider != transform.GetChild(0).GetComponent<MeshCollider>())
                    {
                        collider.transform.parent.GetComponent<Hexagon>().willDestroyed = true;
                    }
                }
            }
            counter = 0;
        }
        GameObject.Find("GameController").GetComponent<GameController>().destroySignedHexagons();
    }

    public void checkForInitialization(Color[] colors)
    {
        List<Color> forbiddenColors = new List<Color>();

        for (int i = 0; i < cornersRot.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + cornersRot[i], 0.2f);
            List<Color> neighborColors = new List<Color>();

            foreach (Collider collider in colliders)
            {
                if (collider != transform.GetChild(0).GetComponent<MeshCollider>() && collider.transform.parent != null)
                {
                    neighborColors.Add(collider.transform.parent.GetComponent<Hexagon>().hexColor);
                }
            }

            if (neighborColors.Count == 2 && neighborColors[0] == neighborColors[1])
            {
                forbiddenColors.Add(neighborColors[0]);
            }
        }

        while (forbiddenColors.Contains(hexColor))
        {
            changeColor(colors[Random.Range(0, colors.Length)]);
        }
    }

}