using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIBarManager : MonoBehaviour {

    RawImage[] listOfRawImages;
    LinkedList<RawImage> currentlyDisplayingImages;

    public RawImage UIBGLeftSide;
    public RawImage UIBGMiddle;
    public RawImage UIBGRightSide;

   // Use this for initialization
	void Start ()
    {
        listOfRawImages = GetComponentsInChildren<RawImage>();
        currentlyDisplayingImages = new LinkedList<RawImage>();

        // Hide all images at the start.
        foreach (RawImage image in listOfRawImages)
        {
            image.enabled = false;
        }
	}

    public void Reset()
    {
        currentlyDisplayingImages.Clear();
        foreach (RawImage image in listOfRawImages)
        {
            image.enabled = false;
        }
        RedrawImages();
    }

    public void Enable(string name)
    {
        foreach (RawImage image in listOfRawImages)
        {
            if (image.name == name)
            {
                // Early out if it's already enabled
                if (image.enabled == true)
                {
                    return;
                }

                image.enabled = true;
                currentlyDisplayingImages.AddLast(image);
                break;
            }
        }

        RedrawImages();
    }

    public void Disable(string name)
    {
        RawImage rawImage = null;

        // This is stupid that we're doing 2 searches,
        // one to find name<==>image and then to see
        // if it exists in currentlyDisplayingImages
        // But it'll have to do for now
        foreach (RawImage image in listOfRawImages)
        {
            if (image.name == name)
            {
                // If the image is not found or if it's disabled already,
                // exit
                if (!image.enabled)
                {
                    return;
                }

                rawImage = image;
            }
        }

       
        if (!rawImage)
        {
            return;
        }

        rawImage.enabled = false;

        if (currentlyDisplayingImages.Contains(rawImage))
        {
            currentlyDisplayingImages.Remove(rawImage);
        }

        RedrawImages();
    }

    private void RedrawImages()
    {
        UIBGLeftSide.enabled = UIBGMiddle.enabled = UIBGRightSide.enabled = false;

        if (currentlyDisplayingImages.Count <= 0) return;

        UIBGLeftSide.enabled = UIBGMiddle.enabled = UIBGRightSide.enabled = true;

        float xOffset = UIBGLeftSide.rectTransform.rect.width;
        UIBGMiddle.rectTransform.transform.localPosition = new Vector3(xOffset, 0, 0);

        float yOffset = 0;

        foreach (RawImage image in currentlyDisplayingImages)
        {
            yOffset = -(UIBGMiddle.rectTransform.rect.height - image.rectTransform.rect.height) / 2;
            image.rectTransform.transform.localPosition = new Vector3(xOffset, yOffset, 0);
            xOffset += image.rectTransform.rect.width;
        }

        UIBGRightSide.rectTransform.transform.localPosition = new Vector3(xOffset, 0, 0);

        float totalOffset = xOffset - UIBGLeftSide.rectTransform.rect.width;
        UIBGMiddle.rectTransform.sizeDelta = new Vector2(totalOffset, UIBGMiddle.rectTransform.rect.height);
    }
}
