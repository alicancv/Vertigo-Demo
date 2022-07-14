using System.Collections.Generic;
using UnityEngine;

public class HexagonFrame : MonoBehaviour
{
    public List<Hexagon> selectedHexagons = new List<Hexagon>();
    public bool canRotate;
    GameController gameController;

    float theta;
    float firstAngle;
    float targetAngle;
    int counter;

    private void Start()
    {
        canRotate = false;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void RotateByAngle(float angle)
    {
        if(canRotate == false)
        {
            firstAngle = transform.eulerAngles.z;
            theta = firstAngle;
            targetAngle = firstAngle + angle;
            counter = 1;
            canRotate = true;
        }
    }

    private void FixedUpdate()
    {
        if (canRotate == true)
        {
            theta += Time.deltaTime * 450;

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, theta % 360);

            if (theta - firstAngle > (120 * counter))
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (firstAngle + 120 * counter) % 360);

                for (int i = 0; i < GameController.hexagons.Count; i++)
                {
                    GameController.hexagons[i].checkForDestroy();
                }

                counter++;
            }
            if(theta > targetAngle)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetAngle % 360);
                canRotate = false;
            }

        }
    }

    public void selectHexagons()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 0.2f);
        for (int i = 0; i < collider.Length; i++)
        {
            if (collider[i].transform.parent != null)
                selectedHexagons.Add(collider[i].transform.parent.GetComponent<Hexagon>());
        }
    }

    public void setHexParent(Transform parent)
    {
        if (selectedHexagons.Count > 0)
        {
            for (int i = 0; i < selectedHexagons.Count; i++)
            {
                if (selectedHexagons[i].transform.gameObject != null)
                    selectedHexagons[i].transform.SetParent(parent);
            }
        }
        if (parent == null)
            selectedHexagons.Clear();
    }
}
