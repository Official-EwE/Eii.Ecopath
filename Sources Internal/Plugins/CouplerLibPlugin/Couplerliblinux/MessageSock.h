#pragma once
#include <string>
#include <vector>
#ifdef __cplusplus_cli
//#include <vcclr.h>
using namespace System;
using namespace System::Threading;
using namespace System::Windows::Forms;
#endif
#include "unetdefs.h"
#ifdef __cplusplus_cli
#include "dispdelegates.h"
#endif
using namespace std;

namespace Couplerlib
{



class MessageSock
{
	
	
#ifdef __cplusplus_cli
#ifdef _Has_GDI
    gcroot<textedelegate ^>textd;
#endif
	gcroot<Control ^>contd;
#endif
	
	const xwchar_t *outbuffer;
	class iIPHostEntry *hostentry;
int socknum;
char *hostname;

class Coupler *cp;

public:
	xstring ccstore;
	xwchar_t *buffer;
	iIPEndPoint *ep;
gsocket *sock,*rsock;
	bool sconnected;
	MessageSock(class Coupler *cpi,bool istransmit,int sockno);
#ifdef __cplusplus_cli
#ifdef _Has_GDI
void setdelegate(Control ^cd,textedelegate ^);
#endif
static void ReceiveLink(void *);
#else
static	void ReceiveLink(void *);
	
#endif

	
void ReceiveOuterLoop();
	void ReceiveLoop();
public : 	void InitiateSend(xstring destination);
			void SendLoop(xstring message);
private : 	class iThread *messagethread;

};
}
