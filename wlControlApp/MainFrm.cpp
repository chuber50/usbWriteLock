
// MainFrm.cpp : implementation of the wlMainFrame class
//

#include "stdafx.h"
#include "wlControlApp.h"

#include "MainFrm.h"
#include "FilterConnection.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// wlMainFrame

IMPLEMENT_DYNCREATE(wlMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(wlMainFrame, CFrameWnd)
	ON_WM_CREATE()
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_INDICATOR_STATUS,
	ID_SEPARATOR,           // status line indicator
	
};

// wlMainFrame construction/destruction

wlMainFrame::wlMainFrame()
{
	// TODO: add member initialization code here
}

wlMainFrame::~wlMainFrame()
{
}

int wlMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	if (!m_wndStatusBar.Create(this))
	{
		TRACE0("Failed to create status bar\n");
		return -1;      // fail to create
	}
	m_wndStatusBar.SetIndicators(indicators, sizeof(indicators)/sizeof(UINT));
	m_wndStatusBar.SetPaneInfo(1, ID_INDICATOR_STATUS, SBPS_NOBORDERS, 400);

	return 0;
}


BOOL wlMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
		return FALSE;
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	cs.cx = 600; // Breite
	cs.cy = 900; // Höhe

	cs.style = WS_OVERLAPPED | WS_CAPTION | FWS_ADDTOTITLE
		  | WS_MINIMIZEBOX | WS_SYSMENU;

	return TRUE;
}

CStatusBar* wlMainFrame::get_StatusBar()
{
	return &m_wndStatusBar;
}

// wlMainFrame diagnostics

#ifdef _DEBUG
void wlMainFrame::AssertValid() const
{
	CFrameWnd::AssertValid();
}

void wlMainFrame::Dump(CDumpContext& dc) const
{
	CFrameWnd::Dump(dc);
}
#endif //_DEBUG


// wlMainFrame message handlers

