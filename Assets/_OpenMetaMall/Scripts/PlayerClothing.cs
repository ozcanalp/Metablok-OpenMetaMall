using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothing : MonoBehaviour
{
    [SerializeField] Transform[] shoeContainers;
    int currentShoeIndex = 0;

    [SerializeField] Transform hatContainer;
    [SerializeField] GameObject characterHair;

    [SerializeField] GameObject objectMaterialToChange;
    [SerializeField] Material[] materials;
    int currentMaterialIndex = 0;

    bool isAllHatsDisabled = true;

    private void Start()
    {
        //StartCoroutine(ChangeShirt());
        //StartCoroutine(ChangeShoe());
    }

    private void Update()
    {
        CheckHairVisibility();
    }

    void CheckHairVisibility()
    {
        isAllHatsDisabled = true;
        foreach (Transform hat in hatContainer)
        {
            if (true == hat.gameObject.activeInHierarchy)
                isAllHatsDisabled = false;
        }

        characterHair.SetActive(isAllHatsDisabled);

    }

    IEnumerator ChangeShoe()
    {
        int numberOfShoes = shoeContainers[0].childCount;
        Debug.Log(numberOfShoes);

        while (true)
        {
            yield return new WaitForSeconds(1);

            foreach (Transform shoes in shoeContainers)
            {
                shoes.transform.GetChild(currentShoeIndex).gameObject.SetActive(false);
                shoes.transform.GetChild((currentShoeIndex + 1) % numberOfShoes).gameObject.SetActive(true);
            }

            currentShoeIndex = (currentShoeIndex + 1) % numberOfShoes;
        }
    }

    IEnumerator ChangeShirt()
    {
        objectMaterialToChange.GetComponent<Renderer>().material = materials[currentMaterialIndex];

        while (true)
        {
            yield return new WaitForSeconds(2);

            objectMaterialToChange.GetComponent<Renderer>().material = materials[++currentMaterialIndex % materials.Length];
        }
    }




}
