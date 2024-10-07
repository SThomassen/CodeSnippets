/// <summary>
/// The HoloLens room mapping was basic at the time and I needed a solution to place an anchor at position of a television, without having to do too much manual work as user.
/// By sending an image to Azura Prediction API that is taken by the HoloLens, it can be analyzed if and where there is a television within the image.
/// The prediction contains a position and based on the prediction I could calculate where my widgets should be placed.
/// </summary>
public IEnumerator AnalyzeImage(string a_imagePath)
{
    Debug.Log("Analyzing..");

    WWWForm form = new WWWForm();

    using (UnityWebRequest unityRequest = UnityWebRequest.Post(m_preditionEndpoint, form))
    {
        m_imageBytes = GetImageAsByteArray(a_imagePath);

        unityRequest.SetRequestHeader("Content-Type", "application/octet-stream");
        unityRequest.SetRequestHeader("Prediction-Key", m_preditionKey);

        // The upload handler will help uploading the byte array with the request
        unityRequest.uploadHandler = new UploadHandlerRaw(m_imageBytes);
        unityRequest.uploadHandler.contentType = "application/octet-stream";

        // The download handler will help receiving the analysis from Azure
        unityRequest.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        yield return unityRequest.SendWebRequest();

        string jsonResponse = unityRequest.downloadHandler.text;

        Debug.Log("response: " + jsonResponse);
        if (jsonResponse.Equals(string.Empty))
        {
            m_calibration.CancelPrediction();
        }

        // Create a texture. Texture size does not matter, since
        // LoadImage will replace with the incoming image size.
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(m_imageBytes);
        m_calibration.SetQuadTexture(tex);

        //The response will be in JSON format, therefore it needs to be deserialized
        AnalysisRootObject analysisRootObject = new AnalysisRootObject();
        analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);

        m_calibration.FinalisePrediction(analysisRootObject);
    }
}

/// <summary>
/// Initialize World Anchor based on best prediction
/// <param name="a_analysisObject"></param>
/// </summary>
public void FinalisePrediction(AnalysisRootObject a_analysisObject)
{
    if (a_analysisObject != null && a_analysisObject.predictions != null)
    {
        //Sort the predictions to locate the highest one
        List<Prediction> sortedPredictions = new List<Prediction>();
        sortedPredictions = a_analysisObject.predictions.OrderBy(p => p.probability).ToList();
        Prediction bestPrediction = new Prediction();
        bestPrediction = sortedPredictions[sortedPredictions.Count - 1];

        if (bestPrediction.probability > m_probability)
        {
            Bounds quadBounds = m_render.bounds;
            //Get position as close as possible to the Bounding Box of the prediction 
            Transform bound = CalculateBoundingBoxPosition(quadBounds, bestPrediction.boundingBox);

            // Check if prediction tag is equal to television
            Debug.LogFormat("tag Name: {0}", bestPrediction.tagName);
            if (bestPrediction.tagName == "television")
            {
                m_main.InitiateAnchor(bound);
            }
        }
        else
        {
            Debug.LogWarning("Prediction too small");
            StartCalibration();
        }
    }
    else
    {
        Debug.LogWarning("Analyse no success");
        StartCalibration();
    }
    ProgressIndicator.Instance.Close();

    //Stop the analysis process
    m_imageCapture.ResetImageCapture();
}

/// <summary>
/// This method hosts a series of calculations to determine the position 
/// of the Bounding Box on the quad created in the real world
/// by using the Bounding Box received back alongside the Best Prediction
/// </summary>
private Transform CalculateBoundingBoxPosition(Bounds a_bounds, BoundingBox a_boundingBox)
{
    Debug.LogFormat("BB: left {0}, top {1}, width {2}, height {3}", a_boundingBox.left, a_boundingBox.top, a_boundingBox.width, a_boundingBox.height);

    float centerX =  (float)(a_boundingBox.left + (a_boundingBox.width * 0.5f));
    float centerY = (float)(a_boundingBox.top + (a_boundingBox.height * 0.5f));
    Debug.LogFormat("BB CenterFromLeft {0}, CenterFromTop {1}", centerX, centerY);

    float quadWidth = a_bounds.size.normalized.x;
    float quadHeight = a_bounds.size.normalized.y;
    Debug.LogFormat("Quad Width {0}, Quad Height {1}", a_bounds.size.normalized.x, a_bounds.size.normalized.y);

    float predictionCenter_X = (float)((quadWidth * centerX) - (quadWidth * 0.5f));
    float predictionCenter_Y = (float)((quadHeight * centerY) - (quadHeight * 0.5f));
    Vector3 predictionCenter = new Vector3( m_quad.transform.position.x + predictionCenter_X, 
                                            m_quad.transform.position.y + predictionCenter_Y, 
                                            m_quad.transform.position.z);

    // Use raycast to get the depth
    RaycastHit rayHit;
    GameObject target = new GameObject();
    target.transform.rotation = m_quad.transform.rotation;
    target.transform.position = predictionCenter;
    target.transform.localScale = new Vector3((float)quadWidth, (float)quadHeight, 0.001f);
    if (Physics.Raycast(m_camera.position, predictionCenter, out rayHit, 30.0f))
    {
        target.transform.position = rayHit.point;
    }

    return target.transform;
}