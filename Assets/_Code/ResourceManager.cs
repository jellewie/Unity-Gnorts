using UnityEngine;
using UnityEngine.UI;                                                                   //We need this to interact with the UI

public class ResourceManager : MonoBehaviour {
    //Stockpile
    public float Wood;
    public float Stone;
    public float Iron;
    public float Gold;

    public long Grain;
    public long flour;

    public Text TextHappiness;
    public Text TextGold;
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

    public void Start()                                                                 //Triggered once on start
    {
        UpdateScreen();                                                                         //Make sure the screen is up to date
    }
    
    public void ChangeWood(float Amount)                                                //This will change the amount by X (TIP use - or +)
    {
        Wood += Amount;                                                                         //Add or remove the amount (Wood +-10 = Wood-10)
        TextWood.text = System.Convert.ToString(Mathf.Floor(Wood));                             //Update the Stats bar
    }
    public void ChangeStone(float Amount)                                               //This will change the amount by X (TIP use - or +)
    {
        Stone += Amount;                                                                        //Add or remove the amount (Stone +-10 = Stone-10)
        TextStone.text = System.Convert.ToString(Mathf.Floor(Stone));                           //Update the Stats bar
    }
    public void ChangeIron(float Amount)                                                //This will change the amount by X (TIP use - or +)
    {
        Iron += Amount;                                                                         //Add or remove the amount (Iron +-10 = Iron-10)
        TextIron.text = System.Convert.ToString(Mathf.Floor(Iron));                             //Update the Stats bar
    }
    public void ChangeGold(float Amount)                                               //This will change the amount by X (TIP use - or +)
    {
        Gold += Amount;                                                                         //Add or remove the amount (Gold +-10 = Gold-10)
        TextGold.text = System.Convert.ToString(Mathf.Floor(Gold));                             //Update the Stats bar
    }

    private void UpdateScreen()                                                         //This will make sure the code runs synchrone with the data the user sees
    {
        ChangeWood(0);                                                                          //Update the stats bar
        ChangeStone(0);                                                                         //^
        ChangeIron(0);                                                                          //^
        ChangeGold(0);                                                                          //^
    }
    public void Set(long AmountWood, long AmountStone, long AmountIron, long AmountGold) //To set all variables (on boot for example) 
    {
        Wood  = AmountWood;                                                                     //Set the given amount
        Stone = AmountStone;                                                                    //^
        Iron  = AmountIron;                                                                     //^
        Gold = AmountGold;                                                                      //^
        UpdateScreen();                                                                         //Make sure the screen is up to date
    }
}
