/*
    * Written By Bags' Lord
    * 10.05.2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class GameManager : MonoBehaviour
{

    /*---------------------------Variables---------------------------*/
    
    /*--------Speed of Animation--------*/
    private float speed = 2;
    /*--------Width of the Area--------*/
    [SerializeField]
    private InputField width;

    /*--------height of the Area--------*/
    [SerializeField]
    private InputField height;

    /*--------Are letters generated?--------*/
    private bool generated;

    /*--------letter prefab--------*/
    /*--------in unity editor named as LetterButton--------*/
    [SerializeField]
    private GameObject letter;

    /*--------Instantion area of letters--------*/
    [SerializeField]
    private RectTransform panel;

    /*--------prefab of positions for each letter--------*/
    /*
     * they are usefull in the translation of letters 
     */
    [SerializeField]
    private GameObject positionPrefab;

    /*--------index for each letter--------*/
    /*
     * i just change index of each letter to translate
     */
    private int[] index;


    /*--------Nodes--------*/
    /*
     * it stores:
     * letter 
     * index
     * is button pressed or not
     */
    private List<Node> objects = new List<Node>();


    /*--------List of positionPrefab--------*/
    /*
     * it stores rectTransform of each Position
     */
    private List<GameObject> rect = new List<GameObject>();


    /*--------width and height of the area in floats, respectively--------*/
    private float x, y;





    /*---------------------------Functions---------------------------*/
    private void Start()
    {
    /*--------It changes the size of instantiation area depending on screen size--------*/
        panel.sizeDelta = new Vector2(Screen.width, Screen.height * 0.92f);
    }





    private void Update()
    {
    /*--------Calls animation for each Node if it is not pressed--------*/
        for (int i = 0; i < objects.Count; i++) {
            if (!objects[i].pressed) {
                Animation(objects[i], objects[i].obj.GetComponent<RectTransform>(), rect[objects[i].index].GetComponent<RectTransform>());
            }
        }
    }





    /*--------validation of inputs--------*/
    public bool Check(float x,float y)
    {
        x = float.Parse(width.text.ToString());
        y = float.Parse(height.text.ToString());
        if (x <= 0 && y <= 0)
            return false;
        return true;
    }





    /*--------Validation of input and letters instantion --------*/
    public void Generate()
    {
        if (width.text.ToString() == "" && height.text.ToString() == "") return;
        x = float.Parse(width.text.ToString());
        y = float.Parse(height.text.ToString());
        if (Check(x, y)) {
            generated = false;
            index = new int[(int)(x * y)];
            /*--------Assigning indexes by order--------*/
            for (int i = 0; i < x * y; i++) {
                index[i] = i;
            }
            Create();
            generated = true;
        }
    }





    /*--------Reallocation process--------*/
    public void Reallocate()
    {
        /*--------it does reallocation if and only if are letters generated--------*/
        if (generated) {
            int[] newIndex = new int[(int)(x * y)];
            for(int i = 0; i < index.Length; i++) {
                if (objects[i].pressed) {
                    newIndex[i] = index[i];
                }
            }
            for(int i = 0; i < index.Length; i++) {
                if (!objects[i].pressed) {
                    NewArray(ref newIndex, i);
                }
            }
            index = newIndex;
            Create();
        }
    }





    /*--------assigning nums of index to newIndex at random indexes--------*/
    /*--------does recursion if index is already occupied--------*/
    public void NewArray(ref int[] newIndex, int i)
    {
        int rand = Random.Range(0, (int)(x * y));
        if (newIndex[rand] == '\0') {
            if (CountBlankSpaces(newIndex) <= 1) {
                newIndex[rand] = index[i];
            } else if(rand == i) {
                NewArray(ref newIndex, i);
            } else {
                newIndex[rand] = index[i];
            }
        } else NewArray(ref newIndex, i);
    }





    /*--------Counts blank places of array--------*/
    public int CountBlankSpaces(int[] newIndex)
    {
        int count = 0;
        for(int i = 0; i < newIndex.Length; i++) {
            if (newIndex[i] == '\0') count++;
        }
        return count;
    }





    public void Create()
    {
        if(!generated) {
            /*--------Destroying previous gameobjects(letters and positions)--------*/
            while (objects.Count != 0) {
                Destroy(objects[objects.Count - 1].obj);
                objects.RemoveAt(objects.Count - 1);
            }
            while(rect.Count != 0) {
                Destroy(rect[rect.Count - 1]);
                rect.RemoveAt(rect.Count - 1);
            }
            /*--------Calculating height and width for each letters(Button)--------*/
            float h = panel.rect.height / (y);
            float w = panel.rect.width / (x);

            /*--------calculating minimum--------*/
            float min = Mathf.Min(h, w);


            /*--------changing instantion area depending on height and width for each letter(Button)--------*/
            if (w > h) {
                panel.sizeDelta = new Vector2(panel.rect.height, panel.rect.height);
            } else {
                panel.sizeDelta = new Vector2(panel.rect.width, panel.rect.width);
            }

            /*--------Instantion Process--------*/
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {

                    /*--------Calculating position for each Letter(Button) and Position--------*/
                    Vector2 position = new Vector2(panel.rect.width / (2 * x) * (2 * i + 1) - panel.rect.width / 2, panel.rect.height / (2 * y) * (2 * j + 1) - panel.rect.height / 2);

                    /*--------instantion of Letter and assigning "instantion field" as a parent--------*/
                    GameObject obj = Instantiate<GameObject>(letter, panel);

                    /*--------changing position depending on parent--------*/
                    obj.transform.localPosition = position;


                    /*--------changing size--------*/
                    obj.GetComponent<RectTransform>().sizeDelta = new Vector2(min, min);

                    /*--------Randomizing letter--------*/
                    obj.transform.GetChild(0).GetComponent<Text>().text = ((char)Random.Range(65, 90)).ToString();

                    /*--------assigning Letter size depending on Button--------*/
                    obj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;

                    /*--------calculating index of the letter--------*/
                    int a = (int)(i * y + j);

                    /*--------creating new node object and adding to objects (List<Node>)--------*/
                    objects.Add(new Node(obj, false, a));

                    /*--------Adding function to Button--------*/
                    obj.GetComponent<Button>().onClick.AddListener(() => Press(a));

                    /*--------instantion of Position and assigning "instantion field" as a parent--------*/
                    rect.Add(Instantiate<GameObject>(positionPrefab, panel));

                    /*--------changing position depending on parent--------*/
                    rect[rect.Count - 1].transform.localPosition = position;
                }
            }
        } else {

            /*--------Assigning changed indexes to object indexes--------*/
            for (int i = 0; i < objects.Count; i++) {
                objects[i].index = index[i];
            }
        }
    }





    /*--------Translating process--------*/
    public void Animation(Node obj, RectTransform objRect, RectTransform destination)
    {
        if (objRect.transform.localPosition.y > destination.transform.localPosition.y + 0.1) {
            objRect.Translate(Vector2.down * Time.deltaTime * speed);
        }
        if (objRect.transform.localPosition.x < destination.transform.localPosition.x - 0.1) {
            objRect.Translate(Vector2.right * Time.deltaTime * speed);
        }
        if (objRect.transform.localPosition.y < destination.transform.localPosition.y - 0.1) {
            objRect.Translate(Vector2.up * Time.deltaTime * speed);
        }
        if (objRect.transform.localPosition.x > destination.transform.localPosition.x + 0.1) {
            objRect.Translate(Vector2.left * Time.deltaTime * speed);
        }
    }





    /*--------Function for Button to pause letter before reallocation--------*/
    public void Press(int i)
    {
        objects[i].pressed = !objects[i].pressed;
    }
}





/*--------Class Node--------*/
[System.Serializable]
public class Node
{
    public GameObject obj;
    public bool pressed;
    public int index;

    /*--------Constructor--------*/
    public Node(GameObject obj, bool pressed, int index)
    {
        this.obj = obj;
        this.pressed = pressed;
        this.index = index;
    }
};
