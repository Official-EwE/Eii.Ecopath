#include "StdAfx.h"
#ifdef __cplusplus_cli
#include "windows.h"
#endif
#include "pipelink.h"
#include "MessageSock.h"
#include "CCouplerlib.h"

//using namespace System;
//using namespace System::Runtime::InteropServices;
using namespace Couplerlib;
#define BUFSIZE 4096
 

Cpipelink::Cpipelink(String ^cc,String ^name)
{
pipeend=false;
ccstore=(wchar_t *)(void *)Marshal::StringToHGlobalUni(cc);
pipename=name;
}

Cpipelink::Cpipelink(String ^cc)
{
pipeend=false;
ccstore=(wchar_t *)(void *)Marshal::StringToHGlobalUni(cc);
pipename=cc;

}

Cpipelink::Cpipelink()
{
	;
}


Void Cpipelink::setusesocketd(bool u)
{
	usesocketd=u;
}

void Cpipelink::setdestin(String ^destin)
{
	destind=destin;
}


void Cpipelink::endpipe()
{
	DWORD rm,to;
	HANDLE hxPipe;
	bool fconnected;
	rm=PIPE_READMODE_MESSAGE+PIPE_NOWAIT;
	to=0;
	pipeend=true;
	Sleep(1000);
	if (!pipehasended)
	{
	 LPTSTR pipnamein=(LPTSTR)(void *)Marshal::StringToHGlobalUni("\\\\.\\pipe\\"+pipename);
	hxPipe = CreateFile( 
		  pipnamein,             // pipe name 
          GENERIC_WRITE,       // read/write access 
         0,
		 NULL,
		 OPEN_EXISTING,
		 0,
		 NULL);
	}
}






void Cpipelink::pipeserver()
{
CHAR chRequest[BUFSIZE]; 

 
 DWORD cbBytesRead;
 bool fconnected,fsuccess;
#ifdef  _UNICODE
 LPTSTR pipnamein=(LPTSTR)(void *)Marshal::StringToHGlobalUni("\\\\.\\pipe\\"+pipename);
#else
#ifdef _MBCS
LPTSTR pipnamein=(LPTSTR)(void *)Marshal::StringToHGlobalUni("\\\\.\\pipe\\"+pipename);
#else
 LPTSTR pipnamein=(LPTSTR)(void *)Marshal::StringToHGlobalAnsi("\\\\.\\pipe\\"+pipename);
#endif
#endif
if (usesocketd)
{messockd->InitiateSend(destind);
}
//strcat(+pipename); 
fconnected=false;
pipehasended=false;
hPipe = CreateNamedPipe( 
		  pipnamein,             // pipe name 
          PIPE_ACCESS_INBOUND,       // read/write access 
          PIPE_TYPE_MESSAGE |       // message type pipe 
          PIPE_READMODE_MESSAGE |   // message-read mode 
          PIPE_WAIT,                // blocking mode 
          PIPE_UNLIMITED_INSTANCES, // max. instances  
          BUFSIZE,                  // output buffer size 
          BUFSIZE,                  // input buffer size 
          1000,                        // client time-out 
          NULL);  
while ((!fconnected)&&(!pipeend))
{
 fconnected = ConnectNamedPipe(hPipe, NULL) ? 
         TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);
}
  if (fconnected) 
      {
		  while (!pipeend) 
   { 
   // Read client requests from the pipe. 
      fsuccess = ReadFile( 
         hPipe,        // handle to pipe 
         chRequest,    // buffer to receive data 
         BUFSIZE*sizeof(CHAR), // size of buffer 
         &cbBytesRead, // number of bytes read 
         NULL);        // not overlapped I/O 

      if (! fsuccess || cbBytesRead == 0) 
         break; 
      for (int n=0; n<cbBytesRead;n++)
	  {
		  ccstore[n]=(xwchar_t(chRequest[n]));
	  }
	  ccstore[cbBytesRead]=0;
	  //ccstore=wcsncpy(ccstore,chRequest,cbBytesRead);
	  if (!pipeend)
	  {
	  if (usesocketd)
	  {
      messockd->SendLoop(ccstore);
	  }
	  else
	  {
		  contd->Invoke(textd,(System::Object ^)gcnew System::String(ccstore));
	  }
	  }
		  }
		  pipehasended=true;
  
  }
  else
  {
  }


}
