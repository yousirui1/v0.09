using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/3/6
*Describe: 透明渐变
**************************************/


public class ToastAlpha: MonoBehaviour {
	
	private Material _mat;
	private float _alpha = 0;
	private bool _isFadeIn = true;
	void Start()
	{
		_mat = this.GetComponent<SpriteRenderer>().material;
		_mat.color = new Color(0, 0, 0, 0);
	}

	void Update()
	{
		_alpha += (_isFadeIn ? 1 : -1) * Time.deltaTime / 2;
		_mat.color = new Color(1, 1, 0, _alpha);
		if (_alpha > 1) _isFadeIn = false;
		if (_alpha < 0) _isFadeIn = true;
	}
}
