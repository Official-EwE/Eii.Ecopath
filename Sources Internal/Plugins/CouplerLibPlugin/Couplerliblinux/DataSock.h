#pragma once
#include <string>
#include <vector>
#include "unetdefs.h"
//using namespace System;
//using namespace System::Text;
//using namespace System::IO;
//using namespace System::Net;
//using namespace System::Net::Sockets;
//using namespace System::Collections;
//using namespace System::Collections::Generic;
//using namespace System::Threading;
using namespace std;

namespace Couplerlib
{

class DataSock
{
xwchar_t *buffer;
xwchar_t *outbuffer;
iIPHostEntry *hostentry,*hostentry2;
int socknum;
char *hostname;
iIPEndPoint *ep,*epr;
gsocket *sock,*rsock;
class Coupler *cp;
iThread *datathread;
bool sconnected;
public:
	DataSock(class Coupler *,bool,int);
	static void ReceiveLink(void *lpparm);
	void ReceiveLoop();
	void ReceiveOuterLoop();
	void SendLoop(int,int,int,double &);
	void InitiateSend(xstring destination);
	void SleepAgain();
};
}