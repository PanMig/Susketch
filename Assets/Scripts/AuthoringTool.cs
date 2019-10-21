using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private TileMap tileMap;
    private FPSClasses fpsClasses;

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

        Debug.Log(teamBlue.Class_params[0]);
        Debug.Log(teamBlue.Class_params[1]);

        var map = tileMap.GetTileMapToString();

        ArrayParsingUtils.ParseToChannelArray(map);
    }
}
