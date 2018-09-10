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

    public void Start()                                                         //Triggered once on start
    {
        UpdateScreen();                                                                  //Make sure the screen is up to date
    }
    
    public void ChangeWood(long Amount)                                         //This will change the amount by X (TIP use - or +)
    {
        Wood += Amount;                                                                 //Add or remove the amount (Wood +-10 = Wood-10)
        TextWood.text = Wood.ToString();                                                //Update the Stats bar
    }
    public void ChangeStone(long Amount)                                        //This will change the amount by X (TIP use - or +)
    {
        Stone += Amount;                                                                //Add or remove the amount (Stone +-10 = Stone-10)
        TextStone.text = Stone.ToString();                                              //Update the Stats bar
    }
    public void ChangeIron(long Amount)                                         //This will change the amount by X (TIP use - or +)
    {
        Iron += Amount;                                                                 //Add or remove the amount (Iron +-10 = Iron-10)
        TextIron.text = Iron.ToString();                                                //Update the Stats bar
    }
    public void ChangeMoney(long Amount)                                        //This will change the amount by X (TIP use - or +)
    {
        Money += Amount;                                                                //Add or remove the amount (Money +-10 = Money-10)
        TextMoney.text = Money.ToString();                                              //Update the Stats bar
    }

    private void UpdateScreen()                                                 //This will make sure the code runs synchrone with the data the user sees
    {
        ChangeWood(0);                                                                  //Update the stats bar
        ChangeStone(0);                                                                 //^
        ChangeIron(0);                                                                  //^
        ChangeMoney(0);                                                                 //^
    }
    public void Set(long AmountWood, long AmountStone, long AmountIron, long AmountMoney)
    {
        Wood  = AmountWood;                                                             //Set the given amount
        Stone = AmountStone;                                                            //^
        Iron  = AmountIron;                                                             //^
        Money = AmountMoney;                                                            //^
        UpdateScreen();                                                                 //Make sure the screen is up to date
    }
}
