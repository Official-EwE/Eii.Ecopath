
#ifdef __cplusplus_cli
using namespace System::Threading;
#else
#include <pthread.h> 
#endif
#include "unetdefs.h"
namespace Couplerlib
{

class cpevent
{
#ifdef __cplusplus_cli
gcroot<ManualResetEvent ^>ievent;
public :
	cpevent(bool initial){ievent=gcnew ManualResetEvent(initial);}
#else
public :
pthread_mutex_t mtt; //=PTHREAD_MUTEX_INITIALIZER;
pthread_cond_t mtc; //=PTHREAD_COND_INITIALIZER;
cpevent(bool initial);
#endif
public :

void Set()
{
#ifdef __cplusplus_cli
ievent->Set();
#else
pthread_mutex_lock(&mtt);
pthread_cond_signal(&mtc);
pthread_mutex_unlock(&mtt);
#endif
}
void Reset()
{
#ifdef __cplusplus_cli
ievent->Reset();
#else
//pthread_mutex_unlock(&mtt);
#endif
}
bool WaitOne(int timeout=0,bool context=false)
{
#ifdef __cplusplus_cli
return(ievent->WaitOne(timeout,context));
#else
//Set();
pthread_mutex_lock(&mtt);
pthread_cond_wait(&mtc,&mtt);
pthread_mutex_unlock(&mtt);
return(true);
#endif
}
};
}