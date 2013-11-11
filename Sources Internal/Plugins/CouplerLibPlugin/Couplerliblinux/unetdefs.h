
#ifndef unetinc
//#include <winsock.h>
#include <string>
#include <list>
#include <vector>
#include "POCO/DateTimeParser.h"
#include "POCO/DateTimeFormat.h"

#ifdef __cplusplus_cli
#include <vcclr.h>
using namespace System;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
#else
#include<sys/socket.h>
#endif

#include "stringdefs.h"

//xstring::xstring(const wchar_t *si)/
//{
//  
//  basic_string<unsigned short int>::basic_string<unsigned short int>(si); /
//}
  

namespace Couplerlib
{
using namespace std;
int converti(xstring a);
xstring converts(double a);
xstring converts(int a);
wstring wconverts(int a);
double convertd(xstring a);
double convertd(const xwchar_t *a);
Poco::DateTime  convertt(const xwchar_t *a);
Poco::DateTime convertti(const xwchar_t *a,bool inctime=false);
double convertsp(const xwchar_t *a);
double convertsp(const xstring a);
bool convertb(const xwchar_t *a);


enum AddressFam
	{
    AaF_UNSPEC,      
    AaF_UNIX,        
    AaF_INET,        
	};

enum SockType
{
Stream,     
Dgram,      
Raw,        
Rdm,        
Seqpack  
};

enum ProtType
{
Ip         =     0               /* dummy for IP */,
Icmp       =     1               /* control message protocol */,
Igmp       =     2               /* group management protocol */,
Ghp        =     3               /* gateway^2 (deprecated) */,
Tcp        =     6               /* tcp */,
Pup        =     12              /* pup */,
Udp        =     17              /* user datagram protocol */,
Idp        =     22              /* xns idp */,
Nd        =     77              /* UNOFFICIAL net disk proto */,
Rww        =     255             /* raw IP packet */,
};

#ifdef __cplusplus_cli;
typedef enum Couplerlib::AddressFam ddomain;
typedef enum Couplerlib::SockType dtype;
typedef enum Couplerlib::ProtType dprot;
#else
typedef int ddomain;
typedef int dtype;
typedef int dprot;
#endif

#ifndef __cplusplus_cli
enum SocketFlags
{
	none=0,
};


//struct sockaddr
//{
//  enum AddressFam sa_family;
//  char *sa_data[14];
//};


class iIPHostEntry
{
public :

	list<xstring> AddressList;
    list<string> Aliases;
	xstring HostName;
	int error;
	sockaddr sa;
	iIPHostEntry(struct hostent* addr);
	iIPHostEntry(int her){error=her;}
	
};

class iIPEndPoint
{
public:
	class gsocket *soc;
	class iIPHostEntry *he;
	ddomain af;
	dtype st; 
	dprot pt;
	socklen_t helen;
	iIPEndPoint(iIPHostEntry *ie,class gsocket *isoc){soc=isoc;he=ie;helen=(socklen_t)ie->HostName.length();};
	//iIPEndPoint(in_addr ie,SOCKET isoc);
};



class gsocket
{
  unsigned short int *gbuffer;
  int ssk;
  iIPEndPoint *sep;
public :
	gsocket(ddomain af,dtype st ,dprot pt);
	int Bind(iIPEndPoint *);
	int Connect(iIPEndPoint *);
	int Listen(int time);
	int Receive(unsigned short int *,SocketFlags);
	void Receive(char *,SocketFlags);
	void Send(const unsigned short int*,int nochars,SocketFlags);
	void Send(const char *,SocketFlags);
	gsocket *Accept();
	//{soc(af,st,pt);}
};

class linkglue
{
static SocketFlags bix;
static unsigned short int *buf;
static class gsocket *gs;
public :
	linkglue(unsigned short int *buffer,SocketFlags ibix,iIPEndPoint *ep) {buf=buffer,bix=ibix, gs=new gsocket(AF_INET,SOCK_STREAM,0);gs->Bind(ep);}
static void *Receive(void *);
};

#else
#define none 0
class iIPHostEntry
{
public :
gcroot<IPHostEntry ^>iphe;
iIPHostEntry(IPHostEntry ^he){iphe=he;}
iIPHostEntry(xstring *addr){iphe=gcnew IPHostEntry();}
gcroot<IPAddress ^> getIP(){return(iphe->AddressList[0]);}
};

class iIPEndPoint
{


enum AddressFam af;
	enum SockType st; 
	enum ProtType pt;
public:
	gcroot<IPEndPoint ^>ipep;
	iIPEndPoint(iIPHostEntry *ie,class gsocket *isoc){ipep=gcnew IPEndPoint(ie->getIP(),23);}
	iIPEndPoint(iIPHostEntry *ie,int isock){ipep=gcnew IPEndPoint(ie->getIP(),isock);}
	//iIPEndPoint(in_addr ie,Socket ^isoc){ipep=gcnew IPEndPoint(ie,isoc);}
};

class gsocket
{
gcroot <array<Byte> ^>gbuffer;
gcroot<Socket^> ssk;
gsocket(Socket ^ssi);
gcroot<System::Text::UnicodeEncoding ^>enc;
public :
gsocket(enum AddressFam af,SockType st ,ProtType pt);
gsocket *Bind(class iIPEndPoint *ep); 
void Listen(int dur) {ssk->Listen(dur);}
gsocket *Accept();
int Receive(xwchar_t *buffer,int bix);
void Send(const xwchar_t *outbuffer,int bix,void *dum);
void Connect(class iIPEndPoint *ep) {ssk->Connect(ep->ipep);}
};

ref class linkglue
{
int bix;
xwchar_t *buf;
class gsocket *gs;
public :
	linkglue(xwchar_t *buffer,int ibix,iIPEndPoint *ep) {buf=buffer,bix=ibix, gs=new gsocket(AaF_INET,Stream,Tcp);gs->Bind(ep);}
	void Receive() {gs->Receive(buf,bix);}
};








#endif

class ienvironment
{
public :
	static void SetEnvironmentVariable(string,string);
    static string GetEnvironmentVariable(string);
};


class iPath
{
public :
static	string GetDirectoryName(xstring);
static 	string GetFileName(xstring);
};

class iThread
{
#ifdef __cplusplus_cli
	gcroot<Threading::Thread ^>td;
public :
	iThread(Threading::ThreadStart ^);
#else
public :
pthread_t thd1;  
iThread(void *(*)(void *),void *);
#endif
public : unsigned long *id;
void Start();
static void Sleep(long miliseconds);
};
#ifdef __cplusplus_cli
class iDns
{
public :
gcroot<Dns ^> dns;

	//static	IPHostEntry ^GetHostEntry(std::xstring hostname){return(dns->GetHostEntry(hostname));}

#else
class iDns
{
  static char dname[64];

public :
#endif


//static int *GetHostEntry(xstring hostname);
static	class iIPHostEntry *GetHostEntry(const char *hostname);

static 	const char *GetHostName(void);
};
}
#define unetinc
#endif