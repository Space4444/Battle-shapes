using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text moneyTxt;
    public AudioSource selectSource;
    public RectTransform saleContent;
    public GameObject shopBtn, shop, shopButton, minimap, item, sale;
    public Button[] buttons;
    public Player player;
    private int boughtLvs = 0;
    private int[] prices = { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, };
    private void Awake()
    {
        if (PlayerPrefs.HasKey("boughtLvs"))
            boughtLvs = PlayerPrefs.GetInt("boughtLvs");
        float level = PlayerPrefs.HasKey("lvl") ? PlayerPrefs.GetFloat("lvl") : 1f;
        if (level % 3f != 0f)
            gameObject.SetActive(false);
    }
    private void Start()
    {
        UpdatePrices();
        UpdateItems();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (((Vector2)(player.transform.position - transform.position)).magnitude < 2f)
        {
            if (!shopBtn.activeSelf)
                shopBtn.SetActive(true);
        }
        else if(shopBtn.activeSelf)
            shopBtn.SetActive(false);
    }
    public void Buy(int index)
    {
        if (index == -1)
        {
            player.AddLife();
            PlayerPrefs.SetInt("boughtLvs",++boughtLvs);
        }
        else
            player.Upgrade(index + 17);
        player.money -= index == -1 ? 2000<<(boughtLvs-1) : prices[index];
        moneyTxt.text = player.money.ToString();
        player.moneyText.text = moneyTxt.text;
        UpdatePrices();
        UpdateItems();
    }
    public void Sell(int index)
    {
        player.Downgrade(index);
        player.money += 100;
        moneyTxt.text = player.money.ToString();
        player.moneyText.text = moneyTxt.text;
        UpdatePrices();
        UpdateItems();
    }
    private void UpdatePrices()
    {
        Button b = buttons[12];
        int price = 2000 << boughtLvs;
        b.transform.GetChild(2).GetComponent<Text>().text = price.ToString();
        b.interactable = player.money >= price;
        for (int i = 0, len = buttons.Length - 1; i < len; i++)
        {
            int item = i + 17;
            int d = player.maxItemCounts[item] - player.itemCounts[item];
            if(d!=0)
                prices[i] = Mathf.Max(100*(2+player.itemCounts[item]), 102040 / (d*d));
            bool max = player.itemCounts[item] >= player.maxItemCounts[item];
            Button button = buttons[i];
            button.interactable = !max && (player.requiredItems[item] == -1 || player.itemCounts[player.requiredItems[item]] != 0) && player.money >= prices[i];
            button.transform.GetChild(2).GetComponent<Text>().text = max ? "<color=#00ff00ff>MAX</color>" : prices[i].ToString();
            string s="";
            string col1 = button.interactable ? "00ff00ff" : "007f00ff";
            string col2 = button.interactable ? "ffffffff" : "7f7f7fff";
            switch (item)
            {
                case 17:
                    s = "Damage <color=#"+col2+">("+ (player.damage * (player.piercing && !player.laser ? 10f : 2f)).ToString()+ (max? "" : "</color><color=#" + col1+ ">+"+(player.itemCounts[3]==0 ? "1" : "1.5")+"</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 18:
                    s = "Fire rate <color=#"+ col2+ ">("+ player.fireRate.ToString()+ (max ? "" : "</color><color=#" + col1 + ">-" + (player.fireRate - Mathf.FloorToInt(player.fireRate * .9f)).ToString() + "</color><color=#" + col2+ ">") + ")</color>";
                    break;
                case 19:
                    s = "Hull strength <color=#"+ col2+ ">("+ (player.maxHP*2f).ToString()+ (max ? "" : "</color><color=#" + col1+ ">+50</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 20:
                    s = "Repair <color=#"+ col2+ ">("+ ((int)(player.repair*100f)).ToString()+ (max ? "" : "</color><color=#" + col1+ ">+1</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 21:
                    s = "Speed <color=#"+ col2+ ">("+ ((int)(player.maxSpd*50f)).ToString()+ (max ? "m/s" : "</color><color=#" + col1+ ">+0.5m/s</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 22:
                    s = "Shot speed <color=#"+ col2+ ">("+ ((int)(player.shotSpd*50f)).ToString()+ (max ? "m/s" : "</color><color=#" + col1+ ">+0.5m/s</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 23:
                    s = "Life steal <color=#"+ col2+ ">("+ ((int)(player.lifeSteal*100f)).ToString()+ (max ? "%" : "</color><color=#" + col1+ ">+2%</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 24:
                    s = "Damage reflection <color=#"+ col2+ ">("+ ((int)(player.reflection*100f)).ToString()+ (max ? "%" : "</color><color=#" + col1+ ">+10%</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 25:
                    s = "Critical chance <color=#"+ col2+ ">("+ ((int)(player.critChance*100f)).ToString()+ (max ? "%" : "</color><color=#" + col1+ ">+5%</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 26:
                    s = "Critical damage <color=#"+ col2+ ">("+ ((int)(player.critDmg*100f)).ToString()+ (max ? "%" : "</color><color=#" + col1+ ">+10%</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 27:
                    s = "Stunning time <color=#"+ col2+ ">("+ (player.stun*0.02f).ToString()+ (max ? "s" : "</color><color=#" + col1+ ">+0.04s</color><color=#"+ col2+ ">") + ")</color>";
                    break;
                case 28:
                    s = "Slowing time <color=#"+ col2+ ">("+ (player.slow*0.02f).ToString()+ (max ? "s" : "</color><color=#" + col1+ ">+0.08s</color><color=#"+ col2+ ">") + ")</color>";
                    break;
            }
            button.transform.GetChild(0).GetComponent<Text>().text = s;
            RectTransform t = button.transform.GetChild(2).GetComponent<RectTransform>();
            t.GetChild(0).gameObject.SetActive(!max);
            t.anchoredPosition = new Vector2(max?84f:119f,t.anchoredPosition.y);
        }
    }
    private void UpdateItems()
    {
        for (int i = 0, count = saleContent.childCount; i < count; i++)
            Destroy(saleContent.GetChild(i).gameObject);
        bool saleActive = false;
        float j = 0f;
        for (int i = 0; i < player.itemCount; i++)
            if (player.itemCounts[i] != 0)
            {
                saleActive = true;
                RectTransform t = Instantiate(item,saleContent).GetComponent<RectTransform>();
                t.anchorMin = new Vector2(.5f, 1f);
                t.anchorMax = new Vector2(.5f, 1f);
                t.anchoredPosition = new Vector2(-52f, -19f - j * 39f);
                string s = StaticVars.itemNames[i];
                t.GetChild(0).GetComponent<Text>().text = s.Substring(s[1] == 'n' ? 3 : 2);
                t.GetChild(1).GetComponent<Image>().sprite = StaticVars.itemSprites[i];
                t.GetChild(2).GetComponent<Text>().text = "100";
                t.GetChild(3).GetComponent<Text>().text = string.Concat("(x", ((int)player.itemCounts[i]).ToString(), ")");
                int i1 = i;
                t.GetComponent<Button>().onClick.AddListener(delegate { Sell(i1); });
                j++;
                saleContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (j + 1f) * 39f);
            }
        sale.SetActive(saleActive);
    }

    public void OpenShop()
    {
        if (player.HP > 0)
        {
            moneyTxt.text = player.moneyText.text;
            if (selectSource.enabled)
                selectSource.Play();
            shop.SetActive(true);
            shopButton.SetActive(false);
            minimap.SetActive(false);
            UpdatePrices();
            UpdateItems();
            Time.timeScale = 0f;
        }
    }

    public void CloseShop()
    {
        if (selectSource.enabled)
            selectSource.Play();
        shop.SetActive(false);
        minimap.SetActive(true);
        Time.timeScale = 1f;
    }
}