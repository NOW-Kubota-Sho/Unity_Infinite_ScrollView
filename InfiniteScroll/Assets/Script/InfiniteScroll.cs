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

    private�@int showUpTextCount         = 0;   //��X�N���[�����ɕ\������e�L�X�g�̔ԍ�
    private�@int firstCreateCount        = 0;   //�ŏ��Ƀv���n�u������Ă�����
    private�@int showItemNumber       = 0;   //���b�Z�[�W�̕\���ԍ�
    private�@float maxContentHeight      = 0.0f;//�R���e�i�̃R���e���g�T�C�Y�̍ő�l(����)
    private�@float sumItemHeight      = 0.0f;//���b�Z�[�W�̃R���e���g�T�C�Y�𑫂����l�̕ۑ��p
    private�@float viewContentHeight     = 0.0f;//�X�N���[���r���[�̕\���͈�(�R���e���g�T�C�Y)
    private�@float itemContentHeight  = 0.0f;//�v���n�u�̃R���e���g�T�C�Y�̃f�t�H���g�l
    private�@float currentContainerPosY  = 0.0f;//���݂̃R���e�i�̈ʒu(�X�N���[���������m�p)
    private�@float previousContainerPosY = 0.0f;//�ߋ��̃R���e�i�̈ʒu(�X�N���[���������m�p)

    private List<string> itemList = new List<string>();
    private List<float> itemHeightList = new List<float>();
    private List<ItemElement> elementList = new List<ItemElement>();
    private Dictionary<int, TextData> itemDic = new Dictionary<int, TextData>();

    private bool isScrollBottom = true;//�X�N���[������(true = ������ / false = �����)



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("����ɂ���");

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
        //�R���e���g�T�C�Y�̍ő�l���ŏ��ɐݒ�
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
            Debug.Log(showItemNumber + "�ԍ�" + showUpTextCount);
            lastIndex = elementList.Count - 1;
            length = Mathf.Abs(elementList[lastIndex].GetComponent<RectTransform>().anchoredPosition.y) + elementList[lastIndex].getItemHeight();
            distanceContainer = length - currentContainerPosY;
            displayRangeBottom = viewContentHeight + elementList[lastIndex].getItemHeight(); ;
        }
    }
}
