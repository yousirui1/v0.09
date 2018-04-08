namespace tpgm.UI
{
	using UnityEngine;
	using System.Collections;


	public class UIBind : MonoBehaviour
	{
		static bool isBind = false;
		
		public static void Bind()
		{
			if(!isBind)
			{
				isBind = true;
				//bind loader api to load UI	
				UIPage.delegateSyncLoadUI = Resources.Load;

			}

		}

	}	

}
