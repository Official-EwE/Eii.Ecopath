#include "StdAfx.h"
#include "DataSock.h"
#include "Couplerlib.h"
#include "unetdefs.h"
using namespace Couplerlib;

DataSock::DataSock(class Coupler *cpi,bool istransmit,int sockno)
{
	sconnected=false;
	socknum=sockno;
	cp=cpi;
	    hostname=strcpy(hostname,iDns::GetHostName());
		hostentry=iDns::GetHostEntry(hostname);
		sock= new gsocket(AaF_INET,Stream,Tcp);
		ep=new iIPEndPoint(hostentry,sock);
		
		//buffer=new char[1024];
		//outbuffer=new char[1024];
}

void DataSock::ReceiveOuterLoop()
{
#ifdef __cplusplus_cli
	linkglue ^lnkgl=gcnew linkglue(NULL,none,this->ep);
	datathread=new iThread(gcnew System::Threading::ThreadStart(lnkgl,&linkglue::Receive));
#else
	linkglue *linkgl=new linkglue(NULL,none,this->ep);
	datathread=new iThread(&linkglue::Receive,NULL);
	
#endif
//datathread=gcnew Thread(gcnew ThreadStart(this,&DataSock::ReceiveLoop));
		
		datathread->Start();	

}
/*
void DataSock::ReceiveLink(LPVOID lpparm)
{((DataSock *)lpparm)->ReceiveLoop();
 }
*/
void DataSock::ReceiveLoop()
{
	
	int sizerec;
	const xwchar_t *vsout;
	sconnected=true;
	sock->Bind(ep);
	sock->Listen(10);
	rsock=sock->Accept();
	xstring gstr;
	vector<xstring> gstrsegs;
	double valuedat,value;
	while (true) 
	{
		sizerec=rsock->Receive(buffer,none);
			//gstr=Encoding::ASCII->GetString(buffer,1,sizerec-1);
			gstr=xstring(buffer);
			for (int n=0;n<3;n++)
			{gstrsegs.push_back(gstr.substr(':'));
			}
			valuedat=cp->GetDataItem(converti(gstrsegs[0]),converti(gstrsegs[1]),converti(gstrsegs[2]));
			xstring vstr=xstring(converts(valuedat));
			vsout=vstr.c_str(); //Encoding::ASCII->GetBytes(vstr);
		//	outbuffer=cpyxch(vsout,vstr.length());
value=convertd(vsout);
			rsock->Send(vsout,vstr.length(),none);

	}
	  
}

	void DataSock::InitiateSend(xstring destination)
	{
		if (!sconnected)
		{
			hostentry2=iDns::GetHostEntry(cstxml(destination.c_str(),destination.length()));
		epr=new iIPEndPoint(hostentry2,sock);//(hostentry2->AddressList[0]->Address,socknum);
		sock->Connect(epr);
		sconnected=true;
		}
		
	}

void DataSock::SendLoop(int modelno, int linkno, int itemno, double &value )
{
	// Encoding^ ascii = Encoding::ASCII;
	xstring bstr=xstring(L" ")+converts(modelno)+xstring(L":")+converts(linkno)+xstring(L":")+converts(itemno);
		//const wchar_t *vsout=bstr.c_str(); //ascii->GetBytes(bstr);
		//outbuffer=cpyxch(bstr.c_str(),bstr.length());
		//wcscpy_s(outbuffer,wcslen(outbuffer),vsout);
		//outbuffer[0]=0;
		sock->Send(bstr.c_str(),bstr.length(),none);
		int nobytes=sock->Receive(buffer,none);
		value=convertd(buffer); //,0,nobytes));
		
}

void DataSock::SleepAgain()
{
//	socket->Shutdown(SocketShutdown::Both);
//socket->Disconnect(true);
//ReceiveOuterLoop();

}
	
