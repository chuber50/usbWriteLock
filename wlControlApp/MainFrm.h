
// MainFrm.h : interface of the wlMainFrame class
//

#pragma once

class wlMainFrame : public CFrameWnd
{
	
protected: // create from serialization only
	wlMainFrame();
	DECLARE_DYNCREATE(wlMainFrame)

// Attributes
public:

// Operations
public:

// Overrides
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	CStatusBar* get_StatusBar();

	// Implementation
public:
	virtual ~wlMainFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // control bar embedded members
	CStatusBar        m_wndStatusBar;

// Generated message map functions
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	DECLARE_MESSAGE_MAP()

};


