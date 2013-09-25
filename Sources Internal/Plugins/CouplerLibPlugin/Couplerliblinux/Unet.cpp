#include "StdAfx.h"
#include <string.h>
#include <stdlib.h>
#ifdef __cplusplus_cli
#include <vcclr.h>
#else
#include <sys/socket.h>
#include <netdb.h>
#endif
#include "unetdefs.h"


using namespace Couplerlib;



iIPHostEntry *iDns::GetHostEntry(const char *hostname)
{
#ifdef __cplusplus_cli

	IPHostEntry ^iphex=Dns::GetHostEntry(gcnew String(hostname));
return(new iIPHostEntry(iphex));

#else
struct hostent *he=gethostbyname(hostname);
if (he==NULL)
{return(new iIPHostEntry(gethostbyname("localhost")));
}
else{
return(new iIPHostEntry(he));
}
#endif
}


#ifdef __cplusplus_cli

const char *iDns::GetHostName(void)
{
	String ^st;
	//pin_ptr<const wchar_t> wstr;
	st=Dns::GetHostName();
	return((const char *)(void *)(Marshal::StringToHGlobalAnsi(st)));
	//wstr=PtrToStringChars(st);

}


iThread::iThread(Threading::ThreadStart ^ts)  
{
//	CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)&rout,param,0,id);

	td=gcnew System::Threading::Thread(ts); //  (Marshal::GetDelegateForFunctionPointer(IntPtr(rout),Void::typeid)));
}
#else

char  *iDns::GetHostName()
{
  string b=string("");
  if (getdomainname(iDns::dname,64)==0)
  {
    string c=string(dname);
    if (string(dname)==string(""))
    {
      if (gethostname(iDns::dname,64)==0)
      {
	return(dname);
      }
    }
    else
    {
      return(dname);
    }
  }
}



iThread::iThread(void *rroutine(void*),void *params)
{
	pthread_create(&thd1,NULL,rroutine,params);
}
#endif

void iThread::Start()
{
#ifdef __cplusplus_cli
	td->Start();
#else
	pthread_join(thd1,NULL);
#endif
}

#ifdef __cplusplus_cli
gsocket::gsocket(Couplerlib::AddressFam af,Couplerlib::SockType st, Couplerlib::ProtType pt)
{
	gbuffer=gcnew array<Byte>(1024);
	ssk=gcnew Socket((enum AddressFamily)af,(enum System::Net::Sockets::SocketType)st,(enum System::Net::Sockets::ProtocolType)pt);
}

gsocket::gsocket(Socket ^ssi)
{
	gbuffer=gcnew array<Byte>(1024);
	ssk=ssi;
	enc=gcnew UnicodeEncoding;
}

gsocket *gsocket::Accept()
{   Socket ^ssi;
	ssi=ssk->Accept();
	return(new gsocket(ssi));
}


gsocket *gsocket::Bind(iIPEndPoint *ep)
{   Socket ^ssi;
	ssk->Bind(ep->ipep);
	return(new gsocket(ssi));
}


const wstring mastrtonatx(System::String ^stin)
{
	//wstring *ms;
	xstring tx;
	tx=xstring((xwchar_t *)(void *)Marshal::StringToHGlobalUni(stin));
	//ms=new wstring(L"    ");
	//return(*ms);
	return(tx);
}

int gsocket::Receive(xwchar_t *bufin,int bix)
{
	int nobyte;
	xstring wch;
	nobyte=ssk->Receive(gbuffer,static_cast<SocketFlags>(bix));
	//xstring ccstore=xstring(enc->GetString(gbuffer,0,nobyte)->c_str());
	wch=mastrtonatx(enc->GetString(gbuffer,0,nobyte));
	//pin_ptr<const wchar_t> wch = PtrToStringChars(enc->GetString(gbuffer,0,nobyte));
    //xstring ccstore(wch);
	wcscpy(bufin,wch.c_str());
	return(nobyte);
}

void gsocket::Send(const xwchar_t *outbuffer,int bix,void *dum)
{
	String ^a=gcnew String(outbuffer);
	ssk->Send(enc->GetBytes(a),static_cast<SocketFlags>(bix));
}

string iPath::GetDirectoryName(xstring str)
{
	char *dirn; 
	dirn=(char *)(void *)Marshal::StringToHGlobalAnsi(System::IO::Path::GetDirectoryName(gcnew System::String(str.c_str())));
return(string(dirn));
}


string iPath::GetFileName(xstring str)
{
	char *filn; 
	filn=(char *)(void *)Marshal::StringToHGlobalAnsi(System::IO::Path::GetFileName(gcnew System::String(str.c_str())));
return(string(filn));
}


void ienvironment::SetEnvironmentVariable(string var,string val)
{
	System::Environment::SetEnvironmentVariable(gcnew String(var.c_str()),gcnew String(val.c_str()));
}



string ienvironment::GetEnvironmentVariable(string str)
{
	const char *varval;
	varval=(const char *)(void *)Marshal::StringToHGlobalAnsi(System::Environment::GetEnvironmentVariable(gcnew System::String(str.c_str())));
return(string(varval));
}
#else


gsocket::gsocket(ddomain af,dtype st, dprot pt)
{
	gbuffer=new unsigned short int[1024];
	ssk=socket((int)af,(int)st,(int)pt);
}

int gsocket::Bind(iIPEndPoint *ep)
{
 sep=ep;
 return( bind(ep->soc->ssk,&(ep->he->sa),ep->helen));
}


int gsocket::Listen(int back)
{
  return(listen(ssk,back));
}


gsocket *gsocket::Accept()
{
sockaddr *nsockaddr=new sockaddr;
int sochand=accept(ssk,nsockaddr,&(sep->helen));
gsocket *ssi=new gsocket(sep->af,sep->st,sep->pt);
ssi->sep=sep;
ssi->ssk=sochand;
return(ssi);
}

int gsocket::Receive(unsigned short *buffin,SocketFlags bix)
{
  int n,m;
  int nobytes=read(ssk,gbuffer,1024);
  buffin=new unsigned short int(nobytes/2+1);
  for (n=0,m=0;n<nobytes/2;n++,m+=2)
  {buffin[n]=gbuffer[m]*256+gbuffer[m+1];}
  return(nobytes/2);
}

void gsocket::Send(unsigned short const *buf,int nochars, SocketFlags bix)
{ int n,m;
  for (n=0,m=0;n<nochars;n++,m+=2)
  {
    gbuffer[m]=buf[n]/256;
    gbuffer[m+1]=buf[n]%256;
  }
  send(ssk,gbuffer,nochars*2,bix);
}
    
int gsocket::Connect(iIPEndPoint *ep)
{
  sep=ep;
 return( connect(ep->soc->ssk,&(ep->he->sa),ep->helen));
}


//iIPHostEntry::iIPHostEntry(char *addr)
//{
//  char *q=(char *)sa.sa_data;
//}


iIPHostEntry::iIPHostEntry(struct hostent *he)
{
 //sa.sa_data;
 for (int n=0;n<he->h_length;n++)
 {
 sa.sa_data[n]=he->h_addr_list[0][n];
 }
}
string ienvironment::GetEnvironmentVariable(string a)
{
  return(string(getenv(a.c_str())));
}

void ienvironment::SetEnvironmentVariable(string a, string b)
{
  setenv(a.c_str(),b.c_str(),1);
}

#endif