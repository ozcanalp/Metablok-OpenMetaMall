using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Autentication : MonoBehaviour
{
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    [SerializeField] TextMeshProUGUI responseText;

    private void OnEnable()
    {
        TronAPI.Instance.OnResponse += UpdateResponseText;
    }

    private void OnDisable()
    {
        TronAPI.Instance.OnResponse -= UpdateResponseText;
    }

    private void UpdateResponseText(bool obj)
    {
        responseText.gameObject.SetActive(true);
        //responseText.text = obj;
        if (obj == true)
        {
            responseText.text = "Login Success";
            responseText.color = Color.green;
        }
        else
        {
            responseText.text = "Login Failed";
            responseText.color = Color.red;
        }

    }

    public void OnClickLoginButton()
    {
        StartCoroutine(TronAPI.Instance.Login(email.text, password.text));

    }

    public void OnClickSignUpButton()
    {
        StartCoroutine(TronAPI.Instance.SignUp(email.text, password.text));
    }
}
