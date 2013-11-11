#include <stdlib.h>
#include <time.h>
#include <string>
#include <sstream>
#include "unetdefs.h"
#include "Poco/DateTimeFormat.h"


namespace Couplerlib
{
  
  wchar_t *cwxml(const xwchar_t *a)
{
wchar_t *q=new wchar_t[wcslen(a)+1];
q=wcscpy(q,a);
return(q);
}



	int converti(xstring a)
	{
	        wchar_t *b=cwxml(a.c_str());
		return((int)wcstol(b,NULL,10));
	}


	

double convertsp(const xwchar_t *a)
{
wchar_t* b=cwxml(a); 
return(wcstod(b,NULL));
}

double convertsp(xstring a)
{
return(convertsp(a.c_str()));
}

xstring converts(double a)
{
	std::wstringstream *s=new std::wstringstream;
	*s << a;
	return(xstring(s->str().c_str()));
}

xstring converts(int a)
{
	std::wstringstream *s=new std::wstringstream;
	*s << a;
	return(xstring(s->str().c_str()));
}

wstring wconverts(int a)
{
	std::wstringstream *s=new std::wstringstream;
	*s << a;
	return(s->str().c_str());
}


struct Poco::DateTime convertt(const xwchar_t *a)
{
	Poco::DateTime *dtx=new(Poco::DateTime);
//__time32_t aclock;
//char *ss,*sa;
int tz;
//tm *tx=new(tm);
//_time32( &aclock );   // Get time in seconds.
//_localtime32_s( tx, &aclock );   // Convert time to struct tm form.
//ss=asctime(tx);
//ss[strlen(ss)-1]=0;
std::string ctt=string(cstxml(a,wcslen(a))); //a,wcslen(a)));
*dtx=Poco::DateTimeParser::parse(Poco::DateTimeFormat::ISO8601_FORMAT, ctt,tz);
return(*dtx);
}

struct Poco::DateTime convertti(const xwchar_t *a, bool inctime)
{

	Poco::DateTime *dti=new(Poco::DateTime);
	int tz;
	std::string ctt=string(cstxml(a,wcslen(a)));
	if (inctime)
	{
		*dti=Poco::DateTimeParser::parse("%d/%m/%Y %H:%M:%S",ctt,tz);
	}
	else
	{
	*dti=Poco::DateTimeParser::parse("%d",ctt,tz);
	}
	//wcsftime(ab,wcslen(ab)+1,L"P%DD",tx);
	return(*dti);
}


double convertd(const xwchar_t *a)
{
return(convertsp(a));
}
double convertd(xstring a)
{
const xwchar_t *b;
b=a.c_str();
return(convertsp(b));
}


double converd(const string a)
{
	return(strtod(a.c_str(),NULL));
}

/*
bool convertb(const wchar_t *a)
{
	if ((a==L"true")||(a==L"1")||(a==L"TRUE")||(a==L".TRUE.")||(a==L".true."))
	{
		return(true);
	}
	else
	{
		return(false);
	}
}
*/

bool convertb(const xwchar_t *a)
{
  xstring b=xstring(a); 
  if ((b==xstring(L"true"))||(b==xstring(L"1"))||(b==xstring(L"TRUE"))||(b==xstring(L".TRUE."))||(b==xstring(L".true."))||(b==xstring(L"true")))
  {
    return(true);
  }
  else
  {
    return(false);
  }
}
}
