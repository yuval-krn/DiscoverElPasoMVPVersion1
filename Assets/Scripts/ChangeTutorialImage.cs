using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// reference at https://www.youtube.com/watch?v=dSUWr5CkMw0
// reference at https://forum.unity.com/threads/swipe-in-all-directions-touch-and-mouse.165416/ 
public class ChangeTutorialImage : MonoBehaviour
{
    
    // note that the current format of the UI requires these instructions to be listed in a string below 
    public static List<string> tutorialText = new List<string>{"Point your phone down towards the sidewalk",
                                                               "Slowly move your phone side to side",
                                                               "Wait for the sidewalk to be recognized",
                                                               "Tap the highlighted sidewalk to place the chalk art",
                                                               "View and explore the details of the chalk art",
                                                               "Stay on the sidewalk and always be aware of your surroundings"} ;
    public static List<Sprite> tutorialImages = new List<Sprite>();
    public static List<Sprite> tutorialNodes = new List<Sprite>();
    public Image[] images;
    public Text txt;
    public Button bttn;
    public Text buttonText;

    // drag the How to Use GameObject here
    [SerializeField] Text howToText;

    // color to change the How to Use text to 
    [SerializeField] public Color textColor;

    // drag the GetStarted GameObject here
    [SerializeField] public GameObject getStarted;

    // keep track of initial color
    Color howToColor;

    // keep track of onboarding step 
    int imageIndex;
    int textIndex;
    int nodeIndex;

    // keep track of user tap/swipe
    private Touch touch;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    void Awake() 
    {
        // if the UI format is changed to utilize images or to have text gameObjects, this format will apply 
        // Object[] subListText= Resources.LoadAll("UIElements/tutorialtext", typeof(GameObject));
        // foreach (GameObject subListString in subListText) 
        // {    
        //     GameObject text = (GameObject)subListString;
        //     tutorialText.Add(text);
        // }

        Object[] subListImages= Resources.LoadAll("UIElements/tutorialimages", typeof(Sprite));
        foreach (Sprite subListImage in subListImages) 
        {    
            Sprite image = (Sprite)subListImage;
            tutorialImages.Add(image);
        }

        Object[] subListNodes= Resources.LoadAll("UIElements/tutorialdots", typeof(Sprite));
        foreach (Sprite subListNode in subListNodes) 
        {    
            Sprite node = (Sprite)subListNode;
            tutorialNodes.Add(node);
        }
    }
    void Start() 
    {
        Initialize();
    }

    public void Initialize()
    {
        // initialize text UI 
        txt = gameObject.GetComponentInChildren<Text>();
        txt.text = tutorialText[0];

        // initialize next/close button UI
        bttn = gameObject.GetComponentInChildren<Button>();
        buttonText = bttn.GetComponentInChildren<Text>();
        buttonText.text = "Next";

        // initialize image sprites for UI
        images = gameObject.GetComponentsInChildren<Image>();
        images[0].sprite = tutorialImages[0];
        images[1].sprite = tutorialNodes[0];

        getStarted.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // get the indices of each "step" 
        imageIndex = tutorialImages.FindIndex(x => x.Equals(images[0].sprite));
        textIndex = tutorialText.FindIndex(x => x.Equals(txt.text));
        nodeIndex = tutorialNodes.FindIndex(x => x.Equals(images[1].sprite)); 

        Swipe();
    }

    public void Swipe()
    {
        this.gameObject.SetActive(true);

        if (Input.touches.Length > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(touch.position.x,touch.position.y);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(touch.position.x,touch.position.y);
                            
                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                
                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe left
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    if (textIndex == 0) 
                    {
                        return;
                    }

                    // button text should always be "Next" when swiping left
                    buttonText.text = "Next";

                    // iteratre through nodes, images, and text so to see different "frames"/steps of the onboarding process
                    gameObject.GetComponentInChildren<Text>().text = tutorialText[textIndex-1];
                    gameObject.GetComponentsInChildren<Image>()[0].sprite = tutorialImages[imageIndex-1];
                    gameObject.GetComponentsInChildren<Image>()[1].sprite = tutorialNodes[nodeIndex-1];
                }
                //swipe right or tap
                else 
                { 
                    NextStep(); 
                }
            }
        }
    }
    public void NextStep()
    {
        // if the user is viewing the last step of the onboarding
        if (textIndex == (tutorialText.Count - 2))
        {
            // if user is seeing the AROnboarding for the first time, display the "Get Started" button
            if (PlayerPrefs.GetInt("seenAROnboarding") != 1) 
            {
                getStarted.SetActive(true);
            }

            // set the last text to be "Close" 
            buttonText.text = "Close";

            // set heading text to "DISCLAIMER" and keep track of its color 
            howToColor = howToText.GetComponent<Text>().color;
            howToText.text = "DISCLAIMER";
            howToText.GetComponent<Text>().color = textColor;
        }
        // if the user has swiped right or tapped to close the onboarding
        if (textIndex == (tutorialText.Count - 1))
        {
            // set the heading text back to "HOW TO USE" and keep track of its color
            howToText.text = "HOW TO USE";
            howToText.GetComponent<Text>().color = howToColor;

            // set the onboarding to be false
            GameObject parent = this.gameObject.transform.root.gameObject;
            parent.SetActive(false);

            // switch to AR functionality
            CanvasHandler.ARCanvas.SetActive(true);
            PlayerPrefs.SetInt("seenAROnboarding", 1);
            Initialize();
            return;
        }
        // else iterature through the next step 
        gameObject.GetComponentInChildren<Text>().text = tutorialText[textIndex+1];
        gameObject.GetComponentsInChildren<Image>()[0].sprite = tutorialImages[imageIndex+1];
        gameObject.GetComponentsInChildren<Image>()[1].sprite = tutorialNodes[nodeIndex+1];
    }
}
