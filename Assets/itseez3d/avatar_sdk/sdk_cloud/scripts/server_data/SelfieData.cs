using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SelfieData
{
    // Selfie code
    public string code;

    // ISO 8601 datetime after which, selfie will be unavailable (24 hours currently)
    public string expires;

    // Absolute URL to selfie file
    public string file_url;

    // Absolute URL to QR code file with selfie upload page URL
    public string upload_page_qr;

    // Absolute URL to selfie upload page
    public string upload_page_url;

    // Flag indicates whether upload page visited
    public bool upload_page_visited;

    // Absolute URL to this DTO
    public string url;
}
