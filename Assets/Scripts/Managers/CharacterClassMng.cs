using UnityEngine;
using System.Collections;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class CharacterClassMng : MonoBehaviour
{
    public static CharacterClassMng Instance { get; private set; }
    public CharacterParams BlueClass;
    public CharacterParams RedClass;
    public HorizontalSelector blueSelector;
    public HorizontalSelector redSelector;
    private FPSClasses fpsClasses;
    private Transform redSpawnPoint;
    private Transform blueSpawnPoint;
    private Image redClassImage;
    private Image blueClassImage;
    public Sprite[] sprites;
    public GameObject imagePrefab;


    public delegate void OnClassSelectorEdit();
    public static event OnClassSelectorEdit onClassSelectorEdit;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += InitClassSprite;
    }

    public void OnDisable()
    {
        AuthoringTool.onMapInitEnded -= InitClassSprite;
    }

    public void Start()
    {
        fpsClasses = GetComponent<FPSClasses>();
        SetClassParamsFromUI();
    }

    public void SetClassParamsFromUI()
    {
        BlueClass = fpsClasses.characters[blueSelector.index];
        RedClass = fpsClasses.characters[redSelector.index];
    }

    public void SetClassParams(CharacterParams blue, CharacterParams red)
    {
        BlueClass = blue;
        RedClass = red;
    }

    public void ClassSelectorListener()
    {
        SetClassParamsFromUI();
        SetClassSprites();
        onClassSelectorEdit?.Invoke();
    }

    public void InitClassSprite()
    {
        //wait for next frame so position in element of gridlayoutgroup is updated.
        StartCoroutine(CoWaitForPosition());
    }

    public IEnumerator CoWaitForPosition()
    {
        yield return new WaitForEndOfFrame();
        Color redColor = new Color(1, 0, 0.08627451f, 1);
        Color blueColor = new Color(0, 0.2313726f, 1, 1);
        redSpawnPoint = AuthoringTool.tileMapMain.GetTileWithIndex(2, 17).gameObj.GetComponent<RectTransform>();
        redClassImage = GameObject.Instantiate(imagePrefab, redSpawnPoint.position, Quaternion.identity, AuthoringTool.tileMapViewMain.transform).GetComponent<Image>();
        redSpawnPoint.GetComponent<RectTransform>().position = redSpawnPoint.localPosition;
        redClassImage.sprite = sprites[redSelector.index];
        redClassImage.color = redColor;
        redClassImage.preserveAspect = true;

        //same process for blue. Dublicate code I know.
        blueSpawnPoint = AuthoringTool.tileMapMain.GetTileWithIndex(17, 2).gameObj.GetComponent<RectTransform>();
        blueClassImage = GameObject.Instantiate(imagePrefab, blueSpawnPoint.position, Quaternion.identity, AuthoringTool.tileMapViewMain.transform).GetComponent<Image>();
        blueSpawnPoint.GetComponent<RectTransform>().position = blueSpawnPoint.localPosition;
        blueClassImage.sprite = sprites[blueSelector.index];
        blueClassImage.color = blueColor;
        blueClassImage.preserveAspect = true;
    }

    public void SetClassSprites()
    {
        blueClassImage.sprite = sprites[blueSelector.index];
        redClassImage.sprite = sprites[redSelector.index];
    }

    public void SetClassSprites(int indexBlue, int indexRed)
    {
        blueClassImage.sprite = sprites[indexBlue];
        redClassImage.sprite = sprites[indexRed];
    }

    public void SetClassSpriteSelectors(int blue, int red)
    {
        blueSelector.Label.text = blueSelector.itemList[blue].itemTitle;
        blueSelector.defaultIndex = blue;
        blueSelector.index = blue;
        redSelector.Label.text = redSelector.itemList[red].itemTitle;
        redSelector.defaultIndex = red;
        redSelector.index = red;
    }

    /*Really ugly... but it's for a quick implementation
      The convention followed is Scout, Sniper, Heavy, Soldier, Demolition man.
    .*/
    public int GetClassIndex(string className)
    {
        switch (className)
        {
            case "Scout":
                return 0;
                break;
            case "Sniper":
                return 1;
                break;
            case "Heavy":
                return 2;
                break;
            case "Soldier":
                return 3;
                break;
            case "Demolition Man":
                return 4;
                break;
            default:
                return 0;
                Debug.LogError("no matching class name found in GetClassIndex");
                break;
        }
    }
}
