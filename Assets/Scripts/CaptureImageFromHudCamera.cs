using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CaptureImageFromHudCamera : MonoBehaviour {

	public Camera camera;                   //The hud camera of the car
	
	public int image_width = 640;			
	public int image_height = 480;
    public int count = 0;

    string baseFolderName = "Data/testCapturePostProcess";
    StringBuilder folderPath;
    StringBuilder imagePath;

    Boolean folderCreated = false;

    //### Properties for image capturing (in document, there are detailed explanation for them)
    Texture2D tex;
	RenderTexture rt;
	Rect rectReadPicture;
	private byte[] image;                   //Used to save the image from camera

	//### Use this for initialization
	void Start()
	{
	
		//### Setup texture2D and render texture (no need to allocate new memory for every image capturing) ###//
		tex = new Texture2D(image_width, image_height, TextureFormat.ARGB32, false);
		rt = new RenderTexture(image_width, image_height, 24);
		camera.targetTexture = rt;
		rectReadPicture = new Rect(0, 0, image_width, image_height);
        image = new byte[0];


        //>>> Initialize Folders and files <<<//
        folderPath = new StringBuilder(Application.dataPath, 300);
        folderPath.Append("/../");
        folderPath.Append(baseFolderName);
        folderPath.Append(System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
        imagePath = new StringBuilder("");

    }


    private void LateUpdate()
    {
        if(Input.GetKeyDown("k")){
            Debug.Log("capture");
            if(folderCreated == false){
                Directory.CreateDirectory(folderPath.ToString());
                folderCreated = true;
            }
            captureImage_within_two_frames();

        }
    }

    /// <summary>
    /// Capture a image within two frames
    /// </summary>
    public void captureImage_within_two_frames() //Set as public, it is convenient for DataRecorder to call
	{
		StartCoroutine(_captureImage());
	}

    /// <summary>
    /// MEC Coroutine to capture image within two frames
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> _captureImage()
    {
        //###1. Render the current scene to RenderTexture
        camera.Render();

        //###2. wait for one frame
        yield return 0;

        //###3. Read pixels to texture
        RenderTexture.active = rt;
        tex.ReadPixels(rectReadPicture, 0, 0);

        yield return 0;
        //###4. Read texture to array
        image = tex.EncodeToJPG();

        //###5. Save the image into image dictionary
        imagePath.Length = 0;
        imagePath.Append(folderPath.ToString());
        imagePath.Append("/" + count + ".jpg");
        count++;
        File.WriteAllBytes(imagePath.ToString(), image);

    }
}
