#pragma once
#include <stdexcept>
#include <vector>


#define MINISPY_NAME            L"MiniSpy"
#define PATH_PREFIX				L"\\\\.\\"

struct volume {
	std::wstring prefixedLetter = PATH_PREFIX;
	std::wstring driveLetter = L"";
	std::wstring name;
	ULONG instanceCount;
	BOOL isUSB = 0;

	bool operator<(const volume& a) const
	{
		return isUSB > a.isUSB;
	}
};

class FilterConnection
{
private:
	HANDLE fltPort;
public:
	FilterConnection();
	~FilterConnection();

	HRESULT ConnectFilter();
	static std::vector<volume> PollDevices();
	static ULONG IsAttachedToVolume(LPCWSTR VolumeName);
	static BOOL isUsbDevice(std::wstring volumeAccessPath);
};
