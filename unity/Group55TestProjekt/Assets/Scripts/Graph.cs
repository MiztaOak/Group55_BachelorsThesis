using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Based on the video https://www.youtube.com/watch?v=CmU5-v-v1Qo made by Code Monkey
 * All creadit for this implementation goes to him
*/
public class Graph : MonoBehaviour
{

    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform yValueTemp;
    [SerializeField] private RectTransform xValueTemp;
    [SerializeField] private TextMeshProUGUI averageC;
    [SerializeField] private TextMeshProUGUI maxC;
    [SerializeField] private TextMeshProUGUI minC;

    private RectTransform graphContainer;

    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        ShowGraph(Model.GetInstance().GetAverageLigandC());
    }

    //Method that generates the graph based on the list of values
    private void ShowGraph(float[] values)
    {
        if (values.Length == 0)
            return;

        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = Model.GetInstance().environment.GetMaxVal();
        float xSize = graphContainer.sizeDelta.x / values.Length;

        GameObject previousPoint = null;

        //this.yValueTemp.text = yMax.ToString();
        //xValueTemp.text = values.Length.ToString();

        float average = 0;
        float max = -1;
        float min = Mathf.Infinity;

        for (int i = 0; i < values.Length; i++)
        {
            float x = xSize + i * xSize;
            float y = (values[i] / yMax) * graphHeight;
            GameObject currentPoint = CreateCircle(new Vector2(x, y));

            if(previousPoint != null)
            {
                DrawLine(previousPoint.GetComponent<RectTransform>().anchoredPosition, currentPoint.GetComponent<RectTransform>().anchoredPosition);
            }
            previousPoint = currentPoint;

            average += values[i];
            if (values[i] > max)
                max = values[i];
            if (values[i] < min)
                min = values[i];
        }

        //place the labels on the x-axis
        int labelCount = 10;
        float graphhWidth = graphContainer.sizeDelta.x;
        for(int i = 0; i <= labelCount; i++)
        {
            //Generate the xlabel
            RectTransform labelX = Instantiate(xValueTemp);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            float normalizedValue = i * 1f / labelCount;
            labelX.anchoredPosition = new Vector2(xSize+normalizedValue*graphhWidth, -3f);
            labelX.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue*values.Length).ToString();

            //Generate the ylabel
            RectTransform labelY = Instantiate(yValueTemp);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-2f, normalizedValue*graphHeight);
            labelY.GetComponent<Text>().text = (Mathf.RoundToInt(normalizedValue * yMax*100)/100f).ToString();
        }

        averageC.text = "Average ligand consentraion: " + average/values.Length;
        minC.text = "Minimum average consentration: " + min;
        maxC.text = "Maximum average consentration: " + max;
    }


    //Method that draws the circle for a singel data point
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

    //Method that draws a line between two datapoints
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


    //Method that calcultes the angle that the line should have based on the difference between the two points
    private float CalculateAngle(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
