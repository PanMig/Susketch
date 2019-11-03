using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;


// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private TileMap tileMap;
    private FPSClasses fpsClasses;
    public TFModel model;
    public MetricsManager metricsMng;

    private NDArray input_map;
    private NDArray input_weapons;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponentInChildren<TileMap>();
        fpsClasses = GetComponentInChildren<FPSClasses>();
        tileMap.InitTileMap();
        tileMap.InitRegions();
        tileMap.PaintRegion(3, 0, 4);
        tileMap.PaintRegion(0, 3, 5);

        CharacterClass teamBlue = new CharacterClass(fpsClasses.scoutParams.class_params);
        CharacterClass teamRed = new CharacterClass(fpsClasses.sniperParams.class_params);

        var map = tileMap.GetTileMapToString();

        input_map = ArrayParsingUtils.ParseToChannelArray(map);
        var blue = teamBlue.Class_params;
        var red = teamRed.Class_params;

        // concat two arrays
        var merged = new double[blue.Length + red.Length];
        blue.CopyTo(merged, 0);
        red.CopyTo(merged, blue.Length);

        NDArray arr = new NDArray(merged);
        input_weapons = np.array(arr);

        input_weapons = np.expand_dims(input_weapons, 0);
        input_map = np.expand_dims(input_map, 0);


    }

    public void ModelPredictionButtonHandler()
    {
        var results = model.Predict(np.zeros((1, 20, 20, 7)), np.zeros((1, 16)));
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.GenerateDeathHeatmap(heatmap);
    }

}
