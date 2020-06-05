using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FPSClasses : MonoBehaviour
{
    public CharacterParams[] characters; // TODO : characters should have the same order elements based on the dropdowns
    public CharacterParams scoutParams;
    public CharacterParams sniperParams;
    public CharacterParams soldierParams;
    public CharacterParams heavyParams;
    public CharacterParams demoManParams;

    public static List<CharacterParams[]> distinctMatches = new List<CharacterParams[]>();
    public static List<CharacterParams[]> EqualMatches = new List<CharacterParams[]>();

    public void Awake()
    {
        InitDistinctMatches();
        InitEqualMatches();
    }

    public void InitDistinctMatches()
    {
        //scout
        distinctMatches.Add(new CharacterParams[2] { scoutParams, sniperParams });
        distinctMatches.Add(new CharacterParams[2] { scoutParams, soldierParams });
        distinctMatches.Add(new CharacterParams[2] { scoutParams, heavyParams });
        distinctMatches.Add(new CharacterParams[2] { scoutParams, demoManParams });
        //sniper
        distinctMatches.Add(new CharacterParams[2] { sniperParams, scoutParams });
        distinctMatches.Add(new CharacterParams[2] { sniperParams, heavyParams });
        distinctMatches.Add(new CharacterParams[2] { sniperParams, demoManParams });
        //heavy
        distinctMatches.Add(new CharacterParams[2] { heavyParams, scoutParams });
        distinctMatches.Add(new CharacterParams[2] { heavyParams, soldierParams });
        distinctMatches.Add(new CharacterParams[2] { heavyParams, sniperParams });
        distinctMatches.Add(new CharacterParams[2] { heavyParams, demoManParams });
        //soldier
        distinctMatches.Add(new CharacterParams[2] { soldierParams, scoutParams });
        distinctMatches.Add(new CharacterParams[2] { soldierParams, demoManParams });
        distinctMatches.Add(new CharacterParams[2] { soldierParams, heavyParams });
        //demolition man
        distinctMatches.Add(new CharacterParams[2] { demoManParams, scoutParams });
        distinctMatches.Add(new CharacterParams[2] { demoManParams, sniperParams });
        distinctMatches.Add(new CharacterParams[2] { demoManParams, soldierParams });
        distinctMatches.Add(new CharacterParams[2] { demoManParams, heavyParams });
    }

    public void InitEqualMatches()
    {
        //scout
        EqualMatches.Add(new CharacterParams[2] {scoutParams, scoutParams});
        //sniper
        EqualMatches.Add(new CharacterParams[2] {sniperParams, sniperParams});
        //heavy
        EqualMatches.Add(new CharacterParams[2] {heavyParams, heavyParams});
        //soldier
        EqualMatches.Add(new CharacterParams[2] {soldierParams, soldierParams});
        //demolition man
        EqualMatches.Add(new CharacterParams[2] {demoManParams, demoManParams});
    }
}
