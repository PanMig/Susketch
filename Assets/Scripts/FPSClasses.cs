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

    public static List<CharacterParams[]> matchups = new List<CharacterParams[]>();

    public void Awake()
    {
        //scout
        matchups.Add(new CharacterParams[2] { scoutParams, sniperParams });
        matchups.Add(new CharacterParams[2] { scoutParams, soldierParams });
        matchups.Add(new CharacterParams[2] { scoutParams, heavyParams });
        matchups.Add(new CharacterParams[2] { scoutParams, demoManParams });
        //sniper
        matchups.Add(new CharacterParams[2] {sniperParams, scoutParams });
        //matchups.Add(new CharacterParams[2] {sniperParams, soldierParams });
        matchups.Add(new CharacterParams[2] {sniperParams, heavyParams });
        matchups.Add(new CharacterParams[2] {sniperParams, demoManParams});
        //heavy
        matchups.Add(new CharacterParams[2] { heavyParams, scoutParams });
        matchups.Add(new CharacterParams[2] { heavyParams, soldierParams });
        matchups.Add(new CharacterParams[2] { heavyParams, sniperParams });
        matchups.Add(new CharacterParams[2] { heavyParams, demoManParams });
        //soldier
        matchups.Add(new CharacterParams[2] { soldierParams, scoutParams });
        //matchups.Add(new CharacterParams[2] { soldierParams, sniperParams });
        matchups.Add(new CharacterParams[2] { soldierParams, demoManParams });
        matchups.Add( new CharacterParams[2] {soldierParams, heavyParams });
        //demolition man
        matchups.Add(new CharacterParams[2] { demoManParams, scoutParams });
        matchups.Add(new CharacterParams[2] { demoManParams, sniperParams });
        matchups.Add(new CharacterParams[2] { demoManParams, soldierParams });
        matchups.Add(new CharacterParams[2] { demoManParams, heavyParams });
    }
}
