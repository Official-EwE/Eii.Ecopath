#include <iostream>
//#include "GOTMserve.h"
#include "pipelink.h"
#include "Couplerlib.h"



using namespace Couplerlib;


//unsigned short int tx[2] {(unsigned short)1,(unsigned short)2};
Coupler *cp;

/*
int main(int argc, char **argv) {
   // std::cout << "Hello, world!" << std::endl;
    cp=new Coupler(true, false,false);
    GOTMProcessing *GP=new GOTMProcessing(cp);
    GP->acceptprocessing(cp);
    return 0;
}
*/
#ifndef __cplusplus_cli
unsigned short int *cxml(const wchar_t *in)
{
  int b=wcslen(in);
  unsigned short int *c=new unsigned short int(b+1);
  for (int n=0;n<b;n++)
  {
    c[n]=(short)in[n];
  }
  return(c);
}





char iDns::dname[64]="    ";
unsigned short int  *linkglue::buf;
class gsocket *linkglue::gs;
SocketFlags linkglue::bix;
void *linkglue::Receive(void *){gs->Receive(buf,bix);}


unsigned short int *cxml(const char *in)
{
  int b=strlen(in);
  unsigned short int *c=new unsigned short int(b+1);
  for (int n=0;n<b;n++)
  {
    c[n]=(short)in[n];
  }
  return(c);
}
#endif
char *cstxml(const xwchar_t *in,int sz)
{
  char *b=new char[sz+20];
  for (int n=0;n<sz;n++)
  {
    b[n]=(char)in[n];
  }
  b[sz]=0;
  return(b);
}

xwchar_t *cpyxch(const xwchar_t *in,int len)
{
  xwchar_t *out=new xwchar_t(len+1);
  for (int n=0;n<len;n++)
  {
    out[n]=in[n];
  }
  out[len]=0;
  return(out);
}




