using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeCharts;
using System.Linq;

public class LineChartCtrl : MonoBehaviour
{
    public LineChart chart;
    private LineDataSet set1 = new LineDataSet();

    // Start is called before the first frame update
    void Start()
    {
        chart = gameObject.GetComponent<LineChart>();
        ConfigChart();
        SetChartData(new float[2]);
    }

    private void ConfigChart()
    {
        
    }

    public void SetChartData(float[] values)
    {
        //values = values.Select(r => r * 20.0f).ToArray();
        for (int i = 0; i < set1.Entries.Count; i++)
        {
            set1.RemoveEntry(i);
        }
        set1.AddEntry(new LineEntry(0, 0));
        set1.AddEntry(new LineEntry(1, values[0]));
        set1.AddEntry(new LineEntry(2, values[1]));
        set1.AddEntry(new LineEntry(3, values[2]));
        set1.AddEntry(new LineEntry(4, values[3]));
        set1.AddEntry(new LineEntry(5, values[4]));

        set1.LineColor = new Color32(255, 133, 0, 255);
        set1.FillColor = new Color32(0, 0, 0, 0);

        chart.GetChartData().DataSets.Add(set1);
        chart.SetDirty();
    }
}
