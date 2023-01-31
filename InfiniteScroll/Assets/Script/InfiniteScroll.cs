using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TextData
{
    private string itemText = "";
    private float itemHeight = 0.0f;

    public TextData(string text = "", float height = 0.0f)
    {
        itemText = text;
        itemHeight = height;
    }

    public string getItemText(){

        return this.itemText;
    }

    public float getItemHeight()
    {
        return this.itemHeight;
    }
}

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject container = null;
    [SerializeField] private RectTransform containerRect = null;
    [SerializeField] private RectTransform scrollViewRect = null;
    [SerializeField] private ItemPool itemPool = null;

    private int quotaCount = 100;

    private　int showUpTextCount         = 0;   //上スクロール時に表示するテキストの番号
    private　int firstCreateCount        = 0;   //最初にプレハブを作っておく数
    private　int showItemNumber       = 0;   //メッセージの表示番号
    private　float maxContentHeight      = 0.0f;//コンテナのコンテントサイズの最大値(高さ)
    private　float sumItemHeight      = 0.0f;//メッセージのコンテントサイズを足した値の保存用
    private　float viewContentHeight     = 0.0f;//スクロールビューの表示範囲(コンテントサイズ)
    private　float itemContentHeight  = 0.0f;//プレハブのコンテントサイズのデフォルト値
    private　float currentContainerPosY  = 0.0f;//現在のコンテナの位置(スクロール方向検知用)
    private　float previousContainerPosY = 0.0f;//過去のコンテナの位置(スクロール方向検知用)

    private List<string> itemList = new List<string>();
    private List<float> itemHeightList = new List<float>();
    private List<ItemElement> elementList = new List<ItemElement>();
    private Dictionary<int, TextData> itemDic = new Dictionary<int, TextData>();

    private bool isScrollBottom = true;//スクロール方向(true = 下方向 / false = 上方向)



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("こんにちは");

        itemContentHeight = itemPool.getItemHeight();

        createItemList(quotaCount);

        setOffsetContentHeight();

        firstCreateCount = itemPool.getMaxCreateCount();
        viewContentHeight = scrollViewRect.rect.height;

        lendItemFromPool(firstCreateCount);
    }

    private void createItemList(int count)
    {
        for(int i = 0; i < count; i++)
        {
            itemList.Add("ItemName" + i);
        }

        for(int i = 0; i < this.itemList.Count; i++)
        {
            //MEMO:
            itemHeightList.Add(itemPool.getItemHeight());
            //MEMO:
            itemDic.Add(i, new TextData(itemList[i],itemHeightList[i]));
        }
    }

    private void lendItemFromPool(int count)
    {
        for(int i = 0; i < count; i++)
        {
            ItemElement element = itemPool.lendItem();
            element.changeItemText(itemDic[showItemNumber].getItemText());
            float posY = itemDic[showItemNumber].getItemHeight();
            element.changePosition(new Vector2(0, -sumItemHeight));
            sumItemHeight += posY;
            elementList.Add(element);
            showItemNumber++;
        }
    }

    private void setOffsetContentHeight()
    {
        for (int i = 0; i < itemHeightList.Count; i++)
        {

            maxContentHeight += itemHeightList[i];
        }
        //コンテントサイズの最大値を最初に設定
        containerRect.sizeDelta = new Vector2(containerRect.rect.width, maxContentHeight);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentContainerPosY = containerRect.anchoredPosition.y;

        checkScrollDirection();

        if (isScrollBottom)
        {
            firstItemHide();
        }
        else
        {
            lastItemHide();
        }

        previousContainerPosY = currentContainerPosY;

    }

    private void checkScrollDirection()
    {
        if (currentContainerPosY > previousContainerPosY)
        {
            isScrollBottom = true;
        }
        else if (currentContainerPosY < previousContainerPosY)
        {
            isScrollBottom = false;
        }
    }

    private void firstItemHide()
    {
        if(quotaCount <= showItemNumber) { return; }

        //MEMO
        float firstItemPosY = elementList[0].GetComponent<RectTransform>().anchoredPosition.y + currentContainerPosY;

        //Mathf.Abs(firstItemPosY) + (itemHeight) < currentContentPosY
        //Debug.Log(firstItemPosY + "::" + elementList[0].getItemHeight());
        //Debug.Log(elementList[0]);

        while (firstItemPosY > elementList[0].getItemHeight() && quotaCount > showItemNumber)
        {
            itemPool.collectItem(elementList[0]);

            elementList.RemoveAt(0);

            ItemElement element = itemPool.lendItem();
            element.changeItemText(itemDic[showItemNumber].getItemText());
            float posY = itemDic[showItemNumber].getItemHeight();
            element.changePosition(new Vector2(0, -sumItemHeight));
            sumItemHeight += posY;
            elementList.Add(element);

            showItemNumber++;

            //MEMO
            firstItemPosY = (elementList[0].GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    private void lastItemHide()
    {
        if(currentContainerPosY <= 0 && showUpTextCount <= 0) { return; }

        showUpTextCount = showItemNumber - elementList.Count - 1;
        if (showUpTextCount < 0) { return; }

        int lastIndex = elementList.Count - 1;
        float length = Mathf.Abs(elementList[lastIndex].GetComponent<RectTransform>().anchoredPosition.y) + elementList[lastIndex].getItemHeight();
        float distanceContainer = length - currentContainerPosY;
        float displayRangeBottom = viewContentHeight + elementList[lastIndex].getItemHeight();

        while(distanceContainer > displayRangeBottom && showUpTextCount >= 0)
        {
            sumItemHeight -= itemDic[showItemNumber - 1].getItemHeight();
            itemPool.collectItem(elementList[lastIndex]);
            elementList.RemoveAt(lastIndex);

            ItemElement element = itemPool.lendItem();
            element.changeItemText(itemDic[showUpTextCount].getItemText());
            float posY = elementList[0].GetComponent<RectTransform>().anchoredPosition.y + itemDic[showUpTextCount].getItemHeight();
            element.changePosition(new Vector2(0, posY));
            elementList.Insert(0,element);

            showItemNumber--;

            showUpTextCount = showItemNumber - (elementList.Count + 1);
            Debug.Log(showItemNumber + "番号" + showUpTextCount);
            lastIndex = elementList.Count - 1;
            length = Mathf.Abs(elementList[lastIndex].GetComponent<RectTransform>().anchoredPosition.y) + elementList[lastIndex].getItemHeight();
            distanceContainer = length - currentContainerPosY;
            displayRangeBottom = viewContentHeight + elementList[lastIndex].getItemHeight(); ;
        }
    }
}
