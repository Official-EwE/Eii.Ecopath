#include "StdAfx.h"
#include "MessageSock.h"
//#include "Couplerlib.h"
using namespace Couplerlib;
//using namespace System::Text;


MessageSock::MessageSock(class Coupler *cpi,bool istransmit,int sockno)
{
		sconnected=false;
	socknum=sockno;
	cp=cpi;
	    hostname=strcpy(hostname,iDns::GetHostName());
		hostentry=iDns::GetHostEntry(hostname);
		sock= new gsocket(AaF_INET,Stream,Tcp);
		ep=new iIPEndPoint(hostentry,sock);
		buffer=new xwchar_t[1024];
		outbuffer=new xwchar_t[1024];
}

void MessageSock::ReceiveOuterLoop()
{
#ifdef __cplusplus_cli
	linkglue ^lnkgl=gcnew linkglue((xwchar_t *)buffer,0,ep);
	messagethread=new iThread(gcnew System::Threading::ThreadStart(lnkgl,&linkglue::Receive));
#endif	
//messagethread=new Thread(new ThreadStart(this,&MessageSock::ReceiveLoop));
		
		messagethread->Start();	

}

/*
void MessageSock::ReceiveLink(LPVOID lpparm)
{((MessageSock *)lpparm)->ReceiveLoop();
 }
*/
// for server end
void MessageSock::ReceiveLoop()
{
	int sizerec;
	sconnected=true;
	sock->Bind(ep);
	sock->Listen(10);
	rsock=sock->Accept();
	string gstr;
	vector<string> gstrsegs;
	//double valuedat,value;
	while (true) 
	{
		sizerec=rsock->Receive((xwchar_t *)buffer,none);
		//ccstore=new string(Encoding::ASCII->GetString(buffer,0,sizerec));
		ccstore=*(new xstring((xwchar_t *)buffer));

#ifdef __cplusplus_cli
#ifdef _Has_GDI
	  contd->Invoke(textd,gcnew String(ccstore.c_str()));
#endif
#endif
	}
	
}

#ifdef __cplusplus_cli
#ifdef _Has_GDI
void MessageSock::setdelegate(Control ^cont,textedelegate ^td)
{
	contd=cont;
	textd=td;
}
#endif
#endif

void MessageSock::InitiateSend(xstring destination)
	{
		hostentry=iDns::GetHostEntry(cstxml(destination.c_str(),destination.length()));
		ep=new iIPEndPoint(hostentry,sock);
		if (!sconnected)
		{
		sock->Connect(ep);
		sconnected=true;
		}
		
	}

void MessageSock::SendLoop(xstring message)
{
	//Encoding^ ascii = Encoding::ASCII;
	//outbuffer=ascii->GetBytes(message);
	outbuffer=message.c_str();
	sock->Send(outbuffer,message.length(),none);
}
