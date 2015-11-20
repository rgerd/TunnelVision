using UnityEngine;
using System.Collections;

public class WhiteboardScript : MonoBehaviour {
	public int textureWidth;
	public int textureHeight;
	void Start() {
		Texture2D texture = new Texture2D(textureWidth, textureHeight);
		
		Color fillColor = Color.white;
		Color[] fillColorArray = texture.GetPixels();
		for (var i = 0; i < fillColorArray.Length; ++i)
			fillColorArray[i] = fillColor;
		texture.SetPixels(fillColorArray);
		texture.Apply();
		
		GetComponent<Renderer>().material.mainTexture = texture;
	}
}