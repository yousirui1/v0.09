/*
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Collections;
using UnityEngine;

public class WebView : MonoBehaviour
{
	public string DefaultUrl = "sample.html";
   // public GUIText status;
    WebViewObject webViewObject;

    IEnumerator Start()
    {
        webViewObject = this.gameObject.AddComponent<WebViewObject>();
		//webViewObject.enabled = false;
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                //status.text = msg;
                //status.GetComponent<Animation>().Play();
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                //status.text = msg;
                //status.GetComponent<Animation>().Play();
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if !UNITY_ANDROID
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  window.Unity = {
                    call: function(msg) {
                      window.location = 'unity:' + msg;
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  window.Unity = {
                    call: function(msg) {
                      var iframe = document.createElement('IFRAME');
                      iframe.setAttribute('src', 'unity:' + msg);
                      document.documentElement.appendChild(iframe);
                      iframe.parentNode.removeChild(iframe);
                      iframe = null;
                    }
                  }
                ");
#endif
#endif
                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            },
            //ua: "custom user agent string",
            enableWKWebView: true);
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        //webViewObject.bitmapRefreshCycle = 1;
		webViewObject.bitmapRefreshCycle = 1;
#endif
		//
		//设置边距 默认值 left,top,right,bottom
		SetMargins(440, 180, 70, 80);
		//显示控件 默认值
		SetVisibility(false);


#if !UNITY_WEBPLAYER
		if (DefaultUrl.StartsWith("http")) {
			webViewObject.LoadURL(DefaultUrl.Replace(" ", "%20"));
        } else {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts) {
				var url = DefaultUrl.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://")) {  // for Android
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
                } else {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html") {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
        webViewObject.EvaluateJS(
            "parent.$(function() {" +
            "   window.Unity = {" +
            "       call:function(msg) {" +
            "           parent.unityWebView.sendMessage('WebViewObject', msg)" +
            "       }" +
            "   };" +
            "});");
#endif
		//webViewObject.enabled = false;
        yield break;
    }

#if !UNITY_WEBPLAYER

	//设置边距
	public void SetMargins(int left, int top, int right, int bottom)
	{
		webViewObject.SetMargins (left,top,right,bottom);
	}


	//显示控件
	public void SetVisibility(bool isVisibility)
	{
		webViewObject.SetVisibility(isVisibility);
	}

	//是否能向后翻页
	public bool CanGoBack()
	{
		return webViewObject.CanGoBack ();
	}

	//是否能向前翻页
	public bool CanGoForward()
	{
		return webViewObject.CanGoForward ();
	}

	//向后翻页
	public void GoBack()
	{
		webViewObject.GoBack();
	}

	//向前翻页
	public void GoForward()
	{
		webViewObject.GoForward();
	}

	//载入和刷新
	public void LoadUrl(string url)
	{
		webViewObject.LoadURL(url);
	}

	//是否显示
	public void isShow(bool show)
	{
		webViewObject.SetVisibility(show);
	}

	//销毁
	public void ViewDestroy()
	{
		webViewObject.ViewDestroy();
	}

	//add custom header
	public void AddCustomHeader(string key, string desc)
	{
		webViewObject.AddCustomHeader(key, desc);
	}

	//get custom header
	public void GetCustomHeaderValue(string key)
	{
		webViewObject.GetCustomHeaderValue(key);
	}

	//remove custom header
	public void RemoveCustomHeader(string key)
	{
		webViewObject.RemoveCustomHeader(key);
	}

	//clear custom header
	public void ClearCustomHeader()
	{
		webViewObject.ClearCustomHeader();
	}

}

#endif

