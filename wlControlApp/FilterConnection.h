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
	BOOL visited = false;

	bool operator<(const volume& a) const
	{
		return isUSB > a.isUSB;
	}
	
	friend bool operator==(const volume& a, const volume* b)
	{
		return b->driveLetter.compare(a.driveLetter) == 0 &&
			b->name.compare(a.name) == 0 &&
			b->instanceCount == a.instanceCount &&
			b->isUSB == a.isUSB;
	}
};

class FilterConnection
{
private:
	HANDLE fltPort;
public:
	FilterConnection();
	~FilterConnection();

	std::vector<volume> volumes;

	HRESULT ConnectFilter();
	HRESULT LoadDriver();
	void PollDevices();
	bool volumesChanged = true;
	static ULONG IsAttachedToVolume(LPCWSTR VolumeName);
	static BOOL isUsbDevice(std::wstring volumeAccessPath);
};
