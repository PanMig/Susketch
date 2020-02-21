using UnityEngine;
using System.Collections;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class CharacterClassMng : MonoBehaviour
{
    public static CharacterClassMng Instance { get; private set; }
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
        SetClassParams();
    }

    public void SetClassParams()
    {
        AuthoringTool.blueClass = fpsClasses.characters[blueSelector.index];
        AuthoringTool.redClass = fpsClasses.characters[redSelector.index];
    }

    public void ClassSelectorListener()
    {
        SetClassParams();
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
        redSpawnPoint = AuthoringTool.tileMapMain.GetTileWithIndex(2, 17).gameObj.GetComponent<RectTransform>();
        redClassImage = GameObject.Instantiate(imagePrefab, redSpawnPoint.position, Quaternion.identity, AuthoringTool.tileMapViewMain.transform).GetComponent<Image>();
        redSpawnPoint.GetComponent<RectTransform>().position = redSpawnPoint.localPosition;
        redClassImage.sprite = sprites[redSelector.index];
        redClassImage.preserveAspect = true;

        //same process for blue. Dublicate code I know.
        blueSpawnPoint = AuthoringTool.tileMapMain.GetTileWithIndex(17, 2).gameObj.GetComponent<RectTransform>();
        blueClassImage = GameObject.Instantiate(imagePrefab, blueSpawnPoint.position, Quaternion.identity, AuthoringTool.tileMapViewMain.transform).GetComponent<Image>();
        blueSpawnPoint.GetComponent<RectTransform>().position = blueSpawnPoint.localPosition;
        blueClassImage.sprite = sprites[blueSelector.index];
        blueClassImage.preserveAspect = true;
    }

    public void SetClassSprites()
    {
        blueClassImage.sprite = sprites[blueSelector.index];
        redClassImage.sprite = sprites[redSelector.index];
    }
}
