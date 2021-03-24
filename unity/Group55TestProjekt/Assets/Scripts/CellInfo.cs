using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    [SerializeField] private Image cheYBar;
    [SerializeField] private Image cheBBar;
    [SerializeField] private Image cheABar;
    [SerializeField] private Image mBar;
    [SerializeField] private Image lifeBar;
    [SerializeField] private Image divBar;
    [SerializeField] private Image cBar;

    public static Cell focusedCell; //really bad way of doing things but works as a place holder
    private Model model;

    // Start is called before the first frame update
    void Start()
    {
        model = Model.GetInstance();

        UpdateBarGraph(cheYBar, 0);
        UpdateBarGraph(cheBBar, 0);
        UpdateBarGraph(cheABar, 0);
        UpdateBarGraph(mBar, 0);
        UpdateBarGraph(lifeBar, 0);
        UpdateBarGraph(divBar, 0);
        UpdateBarGraph(cBar, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(focusedCell != null)
        {
            State cellState = focusedCell.GetInternalState();
            UpdateBarGraph(cheYBar, (float)cellState.yp);
            UpdateBarGraph(cheBBar, (float)cellState.bp);
            UpdateBarGraph(cheABar, (float)cellState.ap);
            UpdateBarGraph(mBar, (float)cellState.m/15);
            UpdateBarGraph(lifeBar, 0);
            UpdateBarGraph(divBar, 0);
            UpdateBarGraph(cBar, (float)cellState.l/model.environment.GetMaxVal());
        }
    }

    //code not used atm size you cant disable the hud right now
   /* private void OnEnable()
    {
        State cellState = focusedCell.GetInternalState();
        UpdateBarGraph(cheYBar, (float)cellState.yp);
        UpdateBarGraph(cheBBar, (float)cellState.bp);
        UpdateBarGraph(cheABar, (float)cellState.ap);
        UpdateBarGraph(mBar, (float)cellState.m);
        UpdateBarGraph(lifeBar, 0);
        UpdateBarGraph(divBar, 0);
        UpdateBarGraph(cBar, (float)cellState.l);
    }

    private void OnDisable()
    {
        focusedCell = null;
    }*/

    private void UpdateBarGraph(Image bar,float q)
    {
        bar.rectTransform.sizeDelta = new Vector2(200*q, 20);
        bar.rectTransform.anchoredPosition = new Vector2(-200 * (1 - q) / 2, 0);
    }

  
}
