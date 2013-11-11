#pragma once
//using namespace System::Threading;
#include "cpevent.h"

namespace Couplerlib
{
class CouplerLibEvent
{
public:
	bool flag,systemflag;
	cpevent *basevent;
	CouplerLibEvent(bool initialstate,bool usesystem);
	virtual ~CouplerLibEvent(void);
	bool WaitOne(int millisecondsTimeout,bool exitcontext);
	bool WaitOne();
	void Set();
	void Reset();
};
}
