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

    public List<CharacterParams[]> matchups = new List<CharacterParams[]>();

    public void Awake()
    {
        matchups.Add( new CharacterParams[2] { sniperParams, scoutParams });
        //matchups.Add( new CharacterParams[2] { sniperParams, soldierParams });
        matchups.Add( new CharacterParams[2] { sniperParams, heavyParams });
        matchups.Add( new CharacterParams[2] { sniperParams, demoManParams });
        matchups.Add( new CharacterParams[2] { scoutParams, soldierParams });
        matchups.Add( new CharacterParams[2] { scoutParams, heavyParams });
        matchups.Add( new CharacterParams[2] { scoutParams, demoManParams });
        matchups.Add( new CharacterParams[2] { soldierParams, heavyParams });
        matchups.Add( new CharacterParams[2] { soldierParams, demoManParams });
        matchups.Add( new CharacterParams[2] { heavyParams, demoManParams });
    }
}
