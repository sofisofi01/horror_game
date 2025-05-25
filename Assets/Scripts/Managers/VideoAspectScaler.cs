using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage))]
public class VideoAspectScaler : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private RawImage rawImage;
    private RectTransform rt;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rt = rawImage.rectTransform;
        
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        
        videoPlayer.prepareCompleted += OnVideoPrepared;
        
        if (videoPlayer.isPrepared)
        {
            AdjustAspect();
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        rawImage.texture = videoPlayer.texture;
        AdjustAspect();
    }

    void Update()
    {
        if (Screen.orientation != ScreenOrientation.LandscapeLeft && 
            Screen.orientation != ScreenOrientation.LandscapeRight)
        {
            AdjustAspect();
        }
    }

    void AdjustAspect()
    {
        if (videoPlayer.texture == null) return;

        float videoWidth = videoPlayer.texture.width;
        float videoHeight = videoPlayer.texture.height;
        float videoRatio = videoWidth / videoHeight;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenRatio = screenWidth / screenHeight;

        if (videoRatio > screenRatio)
        {
            float height = screenWidth / videoRatio;
            rt.sizeDelta = new Vector2(screenWidth, height);
        }
        else
        {
            float width = screenHeight * videoRatio;
            rt.sizeDelta = new Vector2(width, screenHeight);
        }
        
        rt.anchoredPosition = Vector2.zero;
    }

    void OnDestroy()
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
    }
}