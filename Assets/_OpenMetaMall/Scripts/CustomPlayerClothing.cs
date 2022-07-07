using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CustomPlayerClothing : MonoBehaviour
{
    [SerializeField] GameObject hat1;
    [SerializeField] GameObject hat2;
    [SerializeField] GameObject hat3;
    [SerializeField] GameObject hat4;

    [SerializeField] GameObject left_shoe1;
    [SerializeField] GameObject left_shoe2;
    [SerializeField] GameObject left_shoe3;
    [SerializeField] GameObject right_shoe1;
    [SerializeField] GameObject right_shoe2;
    [SerializeField] GameObject right_shoe3;

    [SerializeField] Renderer shirt;

    GameObject clothingObjects;

    private void Start()
    {
        if(false == GetComponentInParent<PhotonView>().IsMine)
            return;

        clothingObjects = GameObject.Find("Clothing Objects");

        foreach (Transform child in clothingObjects.transform)
        {
            if (child.GetComponent<HatClothing>())
            {
                HatClothing hatClothing = child.GetComponent<HatClothing>();
                if (hatClothing.name == "hat1")
                    hatClothing.hat = hat1;
                if (hatClothing.name == "hat2")
                    hatClothing.hat = hat2;
                if (hatClothing.name == "hat3")
                    hatClothing.hat = hat3;
                if (hatClothing.name == "hat4")
                    hatClothing.hat = hat4;
            }
            else if (child.GetComponent<ShoeClothing>())
            {
                ShoeClothing shoeClothing = child.GetComponent<ShoeClothing>();
                if (shoeClothing.name == "shoe1")
                {
                    shoeClothing.leftShoe = left_shoe1;
                    shoeClothing.rightShoe = right_shoe1;
                }
                else if (shoeClothing.name == "shoe2")
                {
                    shoeClothing.leftShoe = left_shoe2;
                    shoeClothing.rightShoe = right_shoe2;
                }
                else if (shoeClothing.name == "shoe3")
                {
                    shoeClothing.leftShoe = left_shoe3;
                    shoeClothing.rightShoe = right_shoe3;
                }
            }
            else if (child.GetComponent<ShirtClothing>())
            {
                ShirtClothing shirtClothing = child.GetComponent<ShirtClothing>();
                shirtClothing.characterShirtRenderer = shirt;
            }
        }
    }
}
