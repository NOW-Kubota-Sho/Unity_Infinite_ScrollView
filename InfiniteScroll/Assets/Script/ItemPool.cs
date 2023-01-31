using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab = null;
    [SerializeField] private GameObject container = null;

    private List<GameObject> itemList = new List<GameObject>();
    private List<ItemElement> elementList = new List<ItemElement>();

    private int maxCreateCount = 10;
    private float itemHeight = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        createPoolItem(maxCreateCount);

    }

    private void createPoolItem(int createCount)
    {
        for(int i = 0; i < createCount; i++)
        {
            GameObject tmpItem = Instantiate(itemPrefab);
            tmpItem.transform.parent = container.transform;
            tmpItem.name += i;
            tmpItem.SetActive(false);
            itemList.Add(tmpItem);
            elementList.Add(tmpItem.GetComponent<ItemElement>());
        }
        itemHeight = elementList[0].GetComponent<RectTransform>().rect.height;
    }

    public ItemElement lendItem()
    {
        if (elementList.Count == 0) return null;

        ItemElement tmpElement = null;

        for (int i = 0; i < elementList.Count; i++){

            if (!elementList[i].IsUsed)
            {
                tmpElement = elementList[i];

                elementList[i].changeUsed(true);

                break;
            }
        }

        return tmpElement;
    }

    public void collectItem(ItemElement item)
    {
        item.hideFromScrollView();
    }

    public int getMaxCreateCount()
    {
        return maxCreateCount;
    }

    public float getItemHeight()
    {
        return itemHeight;
    }
}
