using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemElement : MonoBehaviour
{
    [SerializeField] private Text itemText;

    private bool isUsed;
    public bool IsUsed{ get { return isUsed; } set { isUsed = value; } }

    private float itemHeight = 0.0f;
    private float anchoredPosition;

    private void OnEnable()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition.y;
        itemHeight = this.GetComponent<RectTransform>().rect.height;
    }

    private void checkContentHeight()
    {
        if (itemHeight == this.GetComponent<RectTransform>().rect.height) return;

        itemHeight = this.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        checkContentHeight();
    }

    public float getItemHeight()
    {
        return itemHeight;
    }

    public float getanchoredPosition()
    {
        return anchoredPosition;
    }

    public void changeUsed(bool used)
    {
        gameObject.SetActive(used);
        isUsed = used;
    }

    public void changeItemText(string text)
    {
        this.gameObject.name = text;
        this.itemText.text = text;
    }

    public void changePosition(Vector2 position)
    {
        //MEMO:
        this.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void hideFromScrollView()
    {
        changeUsed(false);
    }
}
