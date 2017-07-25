#include "stdafx.h"
#include "filterConnection.h"
#include <DriverSpecs.h>
#include <winioctl.h>
_Analysis_mode_(_Analysis_code_type_user_code_)

#include <stdlib.h>
#include <stdio.h>
#include <windows.h>
#include <assert.h>
#include <strsafe.h>
#include <fltUser.h>
#include "minispy.h"
#include <vector>
#include <algorithm>

FilterConnection::FilterConnection()
{
	fltPort = INVALID_HANDLE_VALUE;
}

FilterConnection::~FilterConnection()
{
}

HRESULT FilterConnection::ConnectFilter()
{
	HRESULT hResult = S_OK;
	hResult = FilterConnectCommunicationPort(MINISPY_PORT_NAME,
		0,
		nullptr,
		0,
		nullptr,
		&fltPort);

	return hResult;
}

std::vector<volume> FilterConnection::PollDevices()
{
	UCHAR buffer[1024];
	PFILTER_VOLUME_BASIC_INFORMATION volumeBuffer = PFILTER_VOLUME_BASIC_INFORMATION(buffer);
	HANDLE volumeIterator = INVALID_HANDLE_VALUE;
	ULONG volumeBytesReturned;
	HRESULT hResult = S_OK;
	WCHAR driveLetter[15] = { 0 };
	std::vector<volume> volumes;

	try {
		hResult = FilterVolumeFindFirst(FilterVolumeBasicInformation,
			volumeBuffer,
			sizeof(buffer) - sizeof(WCHAR),   //save space to null terminate name
			&volumeBytesReturned,
			&volumeIterator);

		if (IS_ERROR(hResult)) {

		}

		assert(INVALID_HANDLE_VALUE != volumeIterator);

		do {

			assert((FIELD_OFFSET(FILTER_VOLUME_BASIC_INFORMATION, FilterVolumeName) + volumeBuffer->FilterVolumeNameLength) <= (sizeof(buffer) - sizeof(WCHAR)));
			_Analysis_assume_((FIELD_OFFSET(FILTER_VOLUME_BASIC_INFORMATION, FilterVolumeName) + volumeBuffer->FilterVolumeNameLength) <= (sizeof(buffer) - sizeof(WCHAR)));

			volumeBuffer->FilterVolumeName[volumeBuffer->FilterVolumeNameLength / sizeof(WCHAR)] = UNICODE_NULL;

			ULONG instanceCount = IsAttachedToVolume(volumeBuffer->FilterVolumeName);

			volume *vol = new volume();
			if (HRESULT(FilterGetDosName(volumeBuffer->FilterVolumeName, driveLetter, sizeof driveLetter / sizeof(WCHAR))) >= 0) 
			{
				vol->driveLetter = driveLetter;
				vol->prefixedLetter += driveLetter;
			}
			vol->name = volumeBuffer->FilterVolumeName;
			vol->instanceCount = instanceCount;
			vol->isUSB = isUsbDevice(vol->prefixedLetter);
			volumes.push_back(*vol);

		} while (SUCCEEDED(hResult = FilterVolumeFindNext(volumeIterator,
			FilterVolumeBasicInformation,
			volumeBuffer,
			sizeof(buffer) - sizeof(WCHAR),    //save space to null terminate name
			&volumeBytesReturned)));

		if (HRESULT_FROM_WIN32(ERROR_NO_MORE_ITEMS) == hResult) {

			hResult = S_OK;
		}

	}
	catch(...) {
	}

	if (INVALID_HANDLE_VALUE != volumeIterator) {
		FilterVolumeFindClose(volumeIterator);
	}

	if (IS_ERROR(hResult)) {

		if (HRESULT_FROM_WIN32(ERROR_NO_MORE_ITEMS) == hResult) {

			printf("No volumes found.\n");

		}
		else {

			printf("Volume listing failed with error: 0x%08x\n",
				hResult);
		}
	}

	std::sort(volumes.begin(), volumes.end());

	return volumes;
}

ULONG FilterConnection::IsAttachedToVolume(_In_ LPCWSTR VolumeName)
/*++
Determine if our filter is attached to this volume
--*/
{
	CHAR buffer[1024];
	PINSTANCE_FULL_INFORMATION data = PINSTANCE_FULL_INFORMATION(buffer);
	HANDLE volumeIterator = INVALID_HANDLE_VALUE;
	ULONG bytesReturned;
	ULONG instanceCount = 0;
	HRESULT hResult;

	hResult = FilterVolumeInstanceFindFirst(VolumeName,
		InstanceFullInformation,
		data,
		sizeof(buffer) - sizeof(WCHAR),
		&bytesReturned,
		&volumeIterator);

	if (IS_ERROR(hResult)) {

		return instanceCount;
	}

	do {

		assert((data->FilterNameBufferOffset + data->FilterNameLength) <= (sizeof(buffer) - sizeof(WCHAR)));
		_Analysis_assume_((data->FilterNameBufferOffset + data->FilterNameLength) <= (sizeof(buffer) - sizeof(WCHAR)));

		//
		//  Get the name.  Note that we are NULL terminating the buffer
		//  in place.  We can do this because we don't care about the other
		//  information and we have guaranteed that there is room for a NULL
		//  at the end of the buffer.
		//

		PWCHAR filtername = PWCHAR(PUCHAR(data) + data->FilterNameBufferOffset);
		filtername[data->FilterNameLength / sizeof(WCHAR)] = L'\0';


		//
		//  Bump the instance count when we find a match
		//

		if (_wcsicmp(filtername, MINISPY_NAME) == 0) {
			instanceCount++;
		}

	} while (SUCCEEDED(FilterVolumeInstanceFindNext(volumeIterator,
		InstanceFullInformation,
		data,
		sizeof(buffer) - sizeof(WCHAR),
		&bytesReturned)));

	//
	//  Close the handle
	//

	FilterVolumeInstanceFindClose(volumeIterator);
	return instanceCount;
}

BOOL FilterConnection::isUsbDevice(std::wstring volumeAccessPath)
{

	HANDLE deviceHandle = CreateFileW(volumeAccessPath.c_str(), 0, // no access to the drive
		FILE_SHARE_READ | // share mode
		FILE_SHARE_WRITE, NULL,             // default security attributes
		OPEN_EXISTING,    // disposition
		0,                // file attributes
		nullptr);            // do not copy file attributes

	STORAGE_PROPERTY_QUERY query;
	memset(&query, 0, sizeof(query));
	query.PropertyId = StorageDeviceProperty;
	query.QueryType = PropertyStandardQuery;

	DWORD bytes;
	STORAGE_DEVICE_DESCRIPTOR devd;
	STORAGE_BUS_TYPE busType = BusTypeUnknown;

	if (DeviceIoControl(deviceHandle, IOCTL_STORAGE_QUERY_PROPERTY, &query,
		sizeof(query), &devd, sizeof(devd), &bytes, nullptr))
	{
		busType = devd.BusType;
	}

	CloseHandle(deviceHandle);

	return BusTypeUsb == busType;
}