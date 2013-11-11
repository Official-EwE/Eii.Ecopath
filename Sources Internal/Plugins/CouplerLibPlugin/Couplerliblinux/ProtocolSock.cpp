#include "StdAfx.h"
#include "ProtocolSock.h"
#include "MessageSock.h"
#include "CouplerLibEvent.h"
#include "stringdefs.h"

using namespace Couplerlib;

ProtocolSock::ProtocolSock(bool imaster,bool iactive)
{
	int n;
	iIPHostEntry *hostentry;
	nexthostno=0;
	this->ismaster=imaster;
	isactive=iactive;
	allocarray=-1;
	timeout=100000;
	buffer=new unsigned short int[1024];
	pollevent=*(new vector<class CouplerLibEvent *>((int)Last_Protocolno)); 
	stagestatus=*(new vector<statuscodes>((int)Last_Protocolno));
	for (n=0;n<(int)Last_Protocolno;n++)
	{
		pollevent[n]=new CouplerLibEvent(false,!imaster);
	}
	if (ismaster)
	{
		isoc=5001;
		osoc=5000;
	}
	else
	{
		isoc=5000;
		osoc=5001;
	}
	stationhosts=*(new list<iIPEndPoint *>);
	 
		const char *masters=iDns::GetHostName();
		hostentry=iDns::GetHostEntry(masters);
		isocket= new gsocket(AaF_INET,Stream,Ip);
		iIPEndPoint *ep=new iIPEndPoint(hostentry,isocket);
		stationhosts.push_back(ep);
		osocket= new gsocket(AaF_INET,Stream,Ip);
		serverep=new iIPEndPoint(hostentry,osocket);
		currentmessage =new protmessage(0,Opencoupler,Notdetermined,L"Test");

}

int ProtocolSock::AddStation(xstring stationname)
{
	try
	{
		iIPHostEntry *hostentry=iDns::GetHostEntry(cstxml(stationname.c_str(),stationname.length()));
	iIPEndPoint *ep=new iIPEndPoint(hostentry,osocket);
	stationhosts.push_back(ep);
	nexthostno++;
	}
	catch ( char *str)
	{
		return(-1);
	}
	return(nexthostno);

}

int ProtocolSock::Pack(protmessage *message)
{
xstring tempmessage=xstring(L"  ")+message->message;
//wcscpy_s(outbuffer,message->message.length()+3,(xstring(L"   ")+message->message).c_str()); //,3,message->message->Length);
outbuffer=(unsigned short int *)cpyxch(tempmessage.c_str(),tempmessage.length());
outbuffer[0]=(unsigned short int)(message->stationno);
outbuffer[1]=(unsigned short int)(message->pr);
outbuffer[2]=(unsigned short int)(message->sc);
	return(message->message.length()+3);
}

void ProtocolSock::SndMessage(protmessage *prmsg,bool newconnect,int entryno)
{
	int msglen;
	if (newconnect)
	{ 
		list<iIPEndPoint*>::iterator ipx=stationhosts.begin();
		for (int i=0;i<entryno;i++)
		{ipx++;}
		osocket->Connect(*ipx);
	}
	msglen=Pack(prmsg);
	osocket->Send(prmsg->message.c_str(),prmsg->message.length(),none );
}
protmessage *ProtocolSock::Unpack(int bytefrom,int nobytes)
{
	protmessage *returnmess=new protmessage;
	    returnmess->stationno=int(buffer[0]);
		returnmess->pr=(protocols)(int)(buffer[1]);
		returnmess->sc=(statuscodes)(int)(buffer[2]);
		returnmess->message=*(new xstring((xwchar_t *)buffer+bytefrom)); //,nobytes-3));
		return returnmess;
}

protmessage *ProtocolSock::Establishcomms(int entryno, xstring strmessage)
{
	int msglen;
	bool gotmessage;
	if (strmessage==xstring(L"HOST"))
	{
		strmessage=xstring((xwchar_t *)cxml(iDns::GetHostName()));
	}
	protmessage *prmess =new protmessage(entryno,Opencoupler,Notdetermined,strmessage);
	msglen=Pack(prmess);
	list<iIPEndPoint*>::iterator ipx=stationhosts.begin();
		for (int i=0;i<entryno;i++)
		{ipx++;}
		osocket->Connect(*ipx);
	//osocket->Connect(stationhosts[entryno]);
	pollevent[(int)Ack_Opencoupler]->Reset();
	osocket->Send(prmess->message.c_str(),prmess->message.length(),none);
	//gotmessage=pollevent->WaitOne(-1);
	//if (gotmessage)
	//{
	//	
	//}
	//pollevent->Reset();
return(currentmessage);
}

//void ProtocolSock::ReceiveLink(void)
//{((MessageSock *)lpparm)->ReceiveLoop();
 //}

void ProtocolSock::Pollreceive()
{
    iIPHostEntry *hostentry;
	gsocket *psocket;
	isocket->Bind(serverep);
    int sizerec;

	while (true)
	{
    isocket->Listen(10);
	psocket=isocket->Accept();
    //rep=psocket->RemoteEndPoint;
    while(true)
	{
		sizerec=psocket->Receive((xwchar_t *)buffer,none);
		pollmessage=Unpack(3,sizerec);
		
		pollevent[(int)pollmessage->pr]->Set();
	}
	}

}
