
// MainFrm.cpp : implementation of the wlMainFrame class
//

#include "stdafx.h"
#include "wlControlApp.h"

#include "MainFrm.h"

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

	return 0;
}


BOOL wlMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
		return FALSE;

	if (cs.hMenu != NULL)
	{
		::DestroyMenu(cs.hMenu);      // delete menu if loaded
		cs.hMenu = NULL;              // no menu for this window
	}

	cs.style = WS_OVERLAPPED | WS_CAPTION
		  | WS_MINIMIZEBOX | WS_SYSMENU;

	return TRUE;
}


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

