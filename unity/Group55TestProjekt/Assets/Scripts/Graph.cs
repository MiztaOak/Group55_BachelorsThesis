using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Based on the video https://www.youtube.com/watch?v=CmU5-v-v1Qo made by Code Monkey
 * All creadit for this implementation goes to him
*/
public class Graph : MonoBehaviour
{

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        ShowGraph(Model.GetInstance().GetAverageLigandC());
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject circle = new GameObject("circle", typeof(Image));
        circle.transform.SetParent(graphContainer, false);
        circle.GetComponent<Image>().sprite = circleSprite;

        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(12, 12);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return circle;
    }

    private void ShowGraph(float[] values)
    {
        if (values.Length == 0)
            return;

        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = Model.GetInstance().environment.GetMaxVal();
        float xSize = graphContainer.sizeDelta.x / values.Length;

        GameObject previousPoint = null;

        for (int i = 0; i < values.Length; i+=50)
        {
            float x = xSize + i * xSize;
            float y = (values[i] / yMax) * graphHeight;
            GameObject currentPoint = CreateCircle(new Vector2(x, y));

            if(previousPoint != null)
            {
                DrawLine(previousPoint.GetComponent<RectTransform>().anchoredPosition, currentPoint.GetComponent<RectTransform>().anchoredPosition);
            }
            previousPoint = currentPoint;
        }
    }

    private void DrawLine(Vector2 dotA, Vector2 dotB)
    {
        GameObject line = new GameObject("line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = new Color(1, 1, 1, .5f);

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 direction = (dotB - dotA).normalized;
        float distance = Vector2.Distance(dotA, dotB);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotA + direction * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, CalculateAngle(direction)); //last value migth be wron
    }



    private float CalculateAngle(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
