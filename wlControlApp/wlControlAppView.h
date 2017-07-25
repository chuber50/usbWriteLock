
// wlControlAppView.h : interface of the wlControlAppView class
//

#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "filterConnection.h"

class wlControlAppView : public CFormView
{
protected: // create from serialization only
	wlControlAppView();
	DECLARE_DYNCREATE(wlControlAppView)

public:
#ifdef AFX_DESIGN_TIME
	enum{ IDD = IDD_WLCONTROLAPP_FORM };
#endif

// Attributes
public:
	wlControlAppDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual void OnInitialUpdate(); // called first time after construct
	void OnStartTimer();
	void OnTimer(UINT_PTR nIDEvent);
	void RefreshVolumes();
	void OnStopTimer();

	// Implementation
public:
	virtual ~wlControlAppView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
public:
	CListCtrl lstVolumes;
	CEdit txtPollInterval;
	CButton chkAutoAttach;
	UINT_PTR m_nCallbackTimer;
	FilterConnection* filter = new FilterConnection();
//	afx_msg void OnEnChangeTxtPollinterval();
	afx_msg void OnEnKillfocusTxtPollinterval();
};

#ifndef _DEBUG  // debug version in wlControlAppView.cpp
inline wlControlAppDoc* wlControlAppView::GetDocument() const
   { return reinterpret_cast<wlControlAppDoc*>(m_pDocument); }
#endif

