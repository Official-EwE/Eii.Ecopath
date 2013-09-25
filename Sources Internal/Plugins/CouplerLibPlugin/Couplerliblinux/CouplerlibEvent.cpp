#include "CouplerLibEvent.h"
using namespace Couplerlib;

CouplerLibEvent::CouplerLibEvent(bool initialstate,bool usesystem) 
{

	flag=initialstate;
	systemflag=usesystem;
	basevent=new cpevent(initialstate);
	
}

CouplerLibEvent::~CouplerLibEvent(void)
{
}

void CouplerLibEvent::Set()
{
	if (systemflag)
	{
	basevent->Set();
	}
	else
	{
		flag=true;
	}
}

void CouplerLibEvent::Reset()
{
	if (systemflag)
	{
	basevent->Reset();
	}
	else
	{
		flag=false;
	}
}

bool CouplerLibEvent::WaitOne()
{
	bool retflag;
	if (systemflag)
	{
	retflag=basevent->WaitOne();
	}
	else
	{
		while(!flag)
		{  
		
#ifdef __cplusplus_cli
		Thread::Sleep(1);
		System::Windows::Forms::Application::DoEvents();
#else
		usleep(1000);
#endif
		}
		retflag=true;
	}
	return(retflag);
}

bool CouplerLibEvent::WaitOne(int millisecondsTimeout,bool exitcontext)
{
	bool retflag;
	if (systemflag)
	{
	retflag=basevent->WaitOne(millisecondsTimeout,exitcontext);
	}
	else
	{
		while(!flag)
		{
			
#ifdef __cplusplus_cli
			Thread::Sleep(1);
			System::Windows::Forms::Application::DoEvents();
#else
			usleep(1000);
#endif
		}
		retflag=true;
	}
	return(retflag);
}


#ifndef __cplusplus_cli 
cpevent::cpevent(bool initial)
{
  pthread_mutex_init(&mtt,NULL);
  pthread_cond_init(&mtc,NULL);

  
  if (initial)
  {
    pthread_mutex_lock(&mtt);
    pthread_cond_wait(&mtc,&mtt);
    pthread_mutex_unlock(&mtt);
  }
}
#endif
    