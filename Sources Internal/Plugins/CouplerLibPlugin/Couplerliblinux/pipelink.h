#pragma once


#include "dispdelegates.h"
#include "unetdefs.h"
#ifdef __cplusplus_cli
using namespace System;
//using namespace System::Windows::Forms;
#endif


using namespace std;






namespace Couplerlib
{
	/*
#ifdef __cplusplus_cli
ref class Pipelinkjoin
#else
class Pipelinkjoin
#endif
{
class pipelink *pl;
public :
Pipelinkjoin(class pipelink *iplj){pl=iplj;}
void pipeserver();	
};


class pipelink
{
xstring ccstore, destind,pipename;
#ifdef __cplusplus_cli
#ifdef _Has_GDI    
	gcroot<textedelegate ^>textd;
#endif
	gcroot<Object ^>contd;
#endif
	bool pipeend;
	class MessageSock *messockd;
public:
	bool usesocketd;
#ifdef __cplusplus_cli
	gcroot<Pipelinkjoin ^> plj;
#ifdef __Has_GDI
	void setdelegate(Object ^cd,textedelegate ^,bool usesocket,MessageSock *messock);
#endif
	void pipeserver();
#else
void pipeserver();
#endif
	void setdestin(xstring destin);
	pipelink(xstring *ccin, xstring *pipename);
	void endpipe();
	
	
};
*/
}
