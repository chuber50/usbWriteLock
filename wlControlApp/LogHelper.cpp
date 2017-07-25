#include "stdafx.h"
#include "LogHelper.h"
#include "date.h"


LogHelper::LogHelper()
{
}


LogHelper::~LogHelper()
{
}

std::wstring LogHelper::CurrentDateTime()
{
	using namespace date;
	auto now = std::chrono::system_clock::now();
	auto today = date::floor<date::days>(now);

	std::wstringstream ss;

	ss << today << ' ' << make_time(now - today) << " UTC";
	return ss.str();
}

std::wstring LogHelper::GetMessageString(const std::wstring& message)
{
	std::wstring msg = LogHelper::CurrentDateTime();
	msg += L": ";
	msg += message;

	return msg;
}

