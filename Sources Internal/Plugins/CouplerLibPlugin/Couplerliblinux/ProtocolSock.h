#pragma once
#include <list>
#include <vector>
#include <string>
//#include <winsock.h>

#include "unetdefs.h"

//using namespace System;
//using namespace System::Threading;
//using namespace System::Windows::Forms;
//using namespace System::Net;
//using namespace System::Net::Sockets;
#include "dispdelegates.h"
using namespace std;



enum protocols
{
Opencoupler,
Ack_Opencoupler,
Assignnodenumber,
Establishxmllocation,
Ack_Establishxmllocation,
InitializeModel,
Ack_InitializeModel,
EditModel,
Ack_EditModel,
Runmodel,
Ack_Runmodel,
Timestep,
Returntimestep,
Finishtimestep,
Dataexchange,
Dataready,
Userrequest,
Modelterminated,
Modelfinalize,
Ack_Modelfinalize,
Modelreset,
Ack_Modelreset,
Terminatemodel,
Modelexception,
Detatchcoupler,
Ack_Detatchcoupler,
Last_Protocolno
};

enum statuscodes
{
Notdetermined,
Ok,
Waitingonresponse,
Warning,
Error,
Fatal,
};
namespace Couplerlib
{


class ProtocolSock
{


bool isactive;

int timeout;
int allocarray;
list<iIPEndPoint *> stationhosts; 
public :
iIPEndPoint *serverep;
int stationno;
bool ismaster;
class protmessage *currentmessage, *pollmessage;
unsigned short int *buffer;
unsigned short int*outbuffer;



gsocket *isocket;
gsocket *osocket;
private: int nexthostno;
		 int isoc,osoc;
public:
	ProtocolSock(bool imaster,bool active);
	int AddStation(xstring stationname);
	class protmessage *Establishcomms(int entryno, xstring strmessage);
	class protmessage *Unpack(int,int);
	int Pack(class protmessage *message);
	void SndMessage(protmessage *prmsg,bool newconnect,int entryno);
	static	void *ReceiveLink(void);
	 void Pollreceive();
	 vector <class CouplerLibEvent *> pollevent;
	 vector <statuscodes> stagestatus;
};

class protmessage
{
public :
	protmessage(int st,protocols msg,statuscodes stat,xstring test)
	{stationno=st;
	pr=msg;
	sc=stat;
	message=test;
	}
	protmessage()
	{;
	}
	int stationno;
	enum protocols pr;
	enum statuscodes sc;
	xstring message;
	xstring supplementary;
};
	
}