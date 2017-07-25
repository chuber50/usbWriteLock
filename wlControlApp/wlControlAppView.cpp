
// wlControlAppView.cpp : implementation of the wlControlAppView class
//

#include "stdafx.h"
// SHARED_HANDLERS can be defined in an ATL project implementing preview, thumbnail
// and search filter handlers and allows sharing of document code with that project.
#ifndef SHARED_HANDLERS
#include "wlControlApp.h"
#endif

#include "wlControlAppDoc.h"
#include "wlControlAppView.h"
#include "filterConnection.h"
#include "MainFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// wlControlAppView

IMPLEMENT_DYNCREATE(wlControlAppView, CFormView)

BEGIN_MESSAGE_MAP(wlControlAppView, CFormView)
	ON_WM_TIMER()
//	ON_EN_CHANGE(IDC_TXT_POLLINTERVAL, &wlControlAppView::OnEnChangeTxtPollinterval)
ON_EN_KILLFOCUS(IDC_TXT_POLLINTERVAL, &wlControlAppView::OnEnKillfocusTxtPollinterval)
END_MESSAGE_MAP()

// wlControlAppView construction/destruction

wlControlAppView::wlControlAppView()
	: CFormView(IDD_WLCONTROLAPP_FORM)
{
}

wlControlAppView::~wlControlAppView()
{
}

void wlControlAppView::DoDataExchange(CDataExchange* pDX)
{
	CFormView::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LISTVOLUMES, lstVolumes);
	DDX_Control(pDX, IDC_TXT_POLLINTERVAL, txtPollInterval);
	DDX_Control(pDX, IDC_CHECK_AUTOATTACH, chkAutoAttach);
}

BOOL wlControlAppView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CFormView::PreCreateWindow(cs);
}

void wlControlAppView::OnInitialUpdate()
{
	CFormView::OnInitialUpdate();
	GetParentFrame()->RecalcLayout();
	ResizeParentToFit();


	// init list view
	CRect rect;
	lstVolumes.GetClientRect(&rect);
	int nColInterval = rect.Width() / 5;

	lstVolumes.InsertColumn(0, _T("Volume"), LVCFMT_LEFT, nColInterval * 2);
	lstVolumes.InsertColumn(1, _T("Drive Letter"), LVCFMT_LEFT, nColInterval);
	lstVolumes.InsertColumn(2, _T("USB"), LVCFMT_LEFT, nColInterval / 2);
	lstVolumes.InsertColumn(3, _T("Status"), LVCFMT_LEFT, rect.Width() - 3 * nColInterval);

	// init settings
	txtPollInterval.SetWindowText(_T("1000"));
	chkAutoAttach.SetCheck(true);


	wlMainFrame *pFrame = (wlMainFrame *)AfxGetMainWnd();
	HRESULT filterStatus = filter->ConnectFilter();
	 
	if (filterStatus == S_OK) {
		pFrame->get_StatusBar()->SetPaneText(1, L"Filter connected", TRUE);

		m_nCallbackTimer = SetTimer(2, 1000, nullptr);
	} 
	else
	{
		_com_error err(filterStatus);
		std::wstring errMsg =  err.ErrorMessage();
		pFrame->get_StatusBar()->SetPaneText(1, (L"Filter not connected: " + errMsg).c_str(), TRUE);
	}
}


afx_msg void wlControlAppView::OnTimer(UINT_PTR nIDEvent)
{
	this->RefreshVolumes();
}

void wlControlAppView::RefreshVolumes()
{
	std::vector<volume> volumes = filter->PollDevices();
	LVITEM lvi;
	CString strItem;
	int i = 0;

	lstVolumes.SetRedraw(FALSE);
	lstVolumes.ShowWindow(SW_HIDE);
	lstVolumes.DeleteAllItems();

	for (auto &volume : volumes)
	{
		// column volume
		lvi.mask = LVIF_TEXT;
		strItem.Format(volume.name.c_str());
		lvi.iItem = i;
		lvi.iSubItem = 0;
		lvi.pszText = LPTSTR(LPCTSTR(strItem));
		lstVolumes.InsertItem(&lvi);

		// column drive letter
		strItem.Format(volume.driveLetter.c_str());
		lvi.iSubItem = 1;
		lvi.pszText = LPTSTR(LPCTSTR(strItem));
		lstVolumes.SetItem(&lvi);

		// column USB
		strItem.Format(volume.isUSB ? L"Yes" : L"No");
		lvi.iSubItem = 2;
		lvi.pszText = LPTSTR(LPCTSTR(strItem));
		lstVolumes.SetItem(&lvi);

		// column status
		strItem.Format(volume.instanceCount > 0 ? L"Write lock enabled" : L"Write lock disabled");
		lvi.iSubItem = 3;
		lvi.pszText = LPTSTR(LPCTSTR(strItem));
		lstVolumes.SetItem(&lvi);

		i++;
	}

	lstVolumes.ShowWindow(SW_SHOW);
	lstVolumes.SetRedraw(TRUE);
}

void wlControlAppView::OnStopTimer()
{
	KillTimer(m_nCallbackTimer);
}

// wlControlAppView diagnostics

#ifdef _DEBUG
void wlControlAppView::AssertValid() const
{
	CFormView::AssertValid();
}

void wlControlAppView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}

wlControlAppDoc* wlControlAppView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(wlControlAppDoc)));
	return (wlControlAppDoc*)m_pDocument;
}
#endif //_DEBUG


// wlControlAppView message handlers


void wlControlAppView::OnEnKillfocusTxtPollinterval()
{
	CString editText;
	txtPollInterval.GetWindowTextW(editText);

	wchar_t* p;
	long converted = wcstol(editText, &p, 10);
	if (*p) {
		// conversion failed because the input wasn't a number
	}
	else {
		// use converted
	}
}
