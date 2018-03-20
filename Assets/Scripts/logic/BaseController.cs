using System;
using Pomelo.DotNetClient;
using System.Collections.Generic;
using tpgm.UI;

namespace tpgm
{
	public abstract class BaseController<T_IView> where T_IView : UIPage
	{
		public BaseController(T_IView iviewOrNull)
		{
			m_iview = iviewOrNull;

			if (null != iviewOrNull)
			{
				m_initedPageID = iviewOrNull.m_pageID;
			}
		}

		public void setupIView(T_IView iview)
		{
			m_iview = iview;

			m_initedPageID = iview.m_pageID;
		}

		public void onIViewDestroy()
		{
			m_iview = default(T_IView);

			onDestroy();
		}

		public virtual void onDestroy()
		{
		}

		public ValTableCache getValTableCache()
		{
			return SavedContext.s_valTableCache;
		}

		public PomeloClient getPomeloClient()
		{
			return SavedContext.s_client;
		}

		public bool uiEventOk()
		{
			return 0 == m_uiEventFlag;
		}

		//#destroy后就会变为null的;
		protected T_IView m_iview;

		//#是否处理ui事件的标记;
		public int m_uiEventFlag = 1;

		public string m_initedPageID;
	}
}
	