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

    private void UpdateResponseText(string obj)
    {
        responseText.text = obj;
    }

    public void OnClickLoginButton()
    {
        TronAPI.Instance.Login(email.text, password.text);
    }

    public void OnClickSignUpButton()
    {
        TronAPI.Instance.SignUp(email.text, password.text);
    }

    public void OnClickGetBids()
    {
        TronAPI.Instance.GetBids("TXt7Z1YgPCTujEJ6zMXN6Ywnhga8rUAkax");
    }

    public void OnClickGiveBid()
    {
        TronAPI.Instance.GiveBid("162", "TXt7Z1YgPCTujEJ6zMXN6Ywnhga8rUAkax");
    }
}
