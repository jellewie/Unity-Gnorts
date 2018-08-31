using UnityEngine;
using UnityEngine.UI;                                                               //We need this to interact with the UI

public class UserStats : MonoBehaviour {
    //Stockpile
    public long Wood;
    public long Stone;
    public long Iron;
    public long Money;
    public long Grain;
    public long flour;


    public Text TextHappiness;
    public Text TextMoney;
    public Text TextCitizen;

    public Text TextFood;
    public Text TextWood;
    public Text TextStone;
    public Text TextIron;

    //Granary
    public long Apple;
    public long Cheese;
    public long Meat;
    public long Bread;
    public long Fish;

    //Armory


    //
    public long Population;
    public long MaxPopulation;

    public void Start()
    {
        ChangeWood (0);                         //Update the stats bar
        ChangeStone(0);
        ChangeIron (0);
        ChangeMoney(0);
    }
    //These code will change the amounts
    public void ChangeWood(long Amount)
    {
        Wood += Amount;                         //Add or remove the amount (Wood +-10 = Wood-10)
        TextWood.text = Wood.ToString();        //Update the Stats bar
    }
    public void ChangeStone(long Amount)
    {
        Stone += Amount;
        TextStone.text = Stone.ToString();
    }
    public void ChangeIron(long Amount)
    {
        Iron += Amount;
        TextIron.text = Iron.ToString();
    }
    public void ChangeMoney(long Amount)
    {
        Money += Amount;
        TextMoney.text = Money.ToString();
    }
}
