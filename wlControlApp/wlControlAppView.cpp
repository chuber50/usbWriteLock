
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
#include "LogHelper.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// wlControlAppView

IMPLEMENT_DYNCREATE(wlControlAppView, CFormView)

BEGIN_MESSAGE_MAP(wlControlAppView, CFormView)
	ON_WM_TIMER()
ON_BN_CLICKED(IDC_BTNConnectDriver, &wlControlAppView::OnBnClickedBtnconnectdriver)
ON_BN_CLICKED(IDC_BTNExit, &wlControlAppView::OnBnClickedBtnexit)
ON_BN_CLICKED(IDC_BTNApply, &wlControlAppView::OnBnClickedBtnapply)
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
	DDX_Control(pDX, IDC_BTNConnectDriver, btnConnectDriver);
	DDX_Control(pDX, IDC_btnConnectFilter, btnConnectFilter);
	DDX_Control(pDX, IDC_BTNExit, btnExitApplication);
	DDX_Control(pDX, IDC_BTNApply, btnApplySettings);
	DDX_Control(pDX, IDC_lstLogBox, lstLogBox);
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

	btnConnectDriver.SetShield(TRUE);

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

	// init log view
	lstLogBox.InsertColumn(0, L"Log Message");
	lstLogBox.SetColumnWidth(0, LVSCW_AUTOSIZE_USEHEADER);


	//wlMainFrame *pFrame = (wlMainFrame *)AfxGetMainWnd();
	HRESULT filterStatus = filter->ConnectFilter();
	 
	if (filterStatus == S_OK) {
		this->LogMessage(L"Driver connected");
		btnConnectDriver.EnableWindow(FALSE);

		m_nCallbackTimer = SetTimer(2, 1000, nullptr);
	} 
	else
	{
		_com_error err(filterStatus);
		std::wstring errMsg =  err.ErrorMessage();
		this->LogMessage(L"Locking driver not loaded");
		btnConnectDriver.EnableWindow(TRUE);
	}
}


afx_msg void wlControlAppView::OnTimer(UINT_PTR nIDEvent)
{
	this->RefreshVolumes();
}

void wlControlAppView::RefreshVolumes()
{
	/*std::vector<volume> newVolumes = filter->PollDevices();
	if (!compareVectors(volumes, newVolumes))
	{*/
		filter->PollDevices();

		if (filter->volumesChanged)
		{
			std::vector<volume> volumes = filter->volumes;
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

				if (volume.isUSB)
				{
					this->LogMessage(L"Detected USB device: " + volume.name + L" " + volume.driveLetter);
					if (attachFilter)
					{
						HRESULT hResult = filter->attachFilterToDevice(volume.name);
						_com_error err(hResult);
						this->LogMessage(L"Attaching lock to device " + volume.driveLetter + L" " + err.ErrorMessage());
					} else
					{
						HRESULT hResult = filter->detachFilterFromDevice(volume.name);
						_com_error err(hResult);
						this->LogMessage(L"Detaching lock from device " + volume.driveLetter + L" " + err.ErrorMessage());
					}
				}

				i++;
			}

			lstVolumes.ShowWindow(SW_SHOW);
			lstVolumes.SetRedraw(TRUE);
		}
		filter->volumesChanged = false;
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


void wlControlAppView::OnBnClickedBtnconnectdriver()
{
	HRESULT hResult = filter->LoadDriver();
	_com_error err(hResult);
	this->LogMessage(L"Loading filter driver: " + std::wstring(err.ErrorMessage()));

	m_nCallbackTimer = SetTimer(2, 1000, nullptr);
}


void wlControlAppView::OnBnClickedBtnexit()
{
	PostQuitMessage(0);
}


void wlControlAppView::OnBnClickedBtnapply()
{
	CString editText;
	txtPollInterval.GetWindowTextW(editText);

	wchar_t* p;
	long converted = wcstol(editText, &p, 10);
	if (*p || converted < 200 || converted > 10000) {
		MessageBox(_T("Enter an integer value between 200 and 10000"), _T("Poll interval"), MB_ICONEXCLAMATION | MB_OK);
	}
	else {
		pollInterval = converted;
		KillTimer(m_nCallbackTimer);
		m_nCallbackTimer = SetTimer(2, pollInterval, nullptr);
		MessageBox(_T("Settings applied."), _T("Settings"), MB_ICONASTERISK | MB_OK);
	}

	attachFilter = !!chkAutoAttach.GetCheck();
	filter->volumesChanged = true; // trigger update
}

void wlControlAppView::LogMessage(const std::wstring& message)
{
	LVITEM lvi;
	std::wstring unicMessage = LogHelper::GetMessageString(message);
	std::vector<wchar_t> vecMsg(unicMessage.begin(), unicMessage.end());
	vecMsg.push_back(L'\0');

	lvi.mask = LVIF_TEXT;
	lvi.iItem = lstLogBox.GetItemCount() + 1;
	lvi.iSubItem = 0;
	
	lvi.pszText = LPWSTR(&vecMsg[0]);
	lstLogBox.InsertItem(&lvi);
}