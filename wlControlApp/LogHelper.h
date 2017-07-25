#pragma once

// https://github.com/HowardHinnant/date/issues/87
#ifdef min
#undef min
#endif
#ifdef max
#undef max
#endif

#include <string>

class LogHelper
{
public:
	LogHelper();
	~LogHelper();
	static std::wstring CurrentDateTime();
	static std::wstring GetMessageString(const std::wstring& message);
};

