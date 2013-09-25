#ifndef _stringdef 
#ifdef __cplusplus_cli
#define xstring wstring
#define xwchar_t wchar_t
#define cxml(a) a
//unsigned short int *cxml(const wchar_t *a);
//unsigned short int *cxml(const char *a);
namespace Couplerlib
{
wchar_t* cwxml(const wchar_t *);
}
#else
#define xwchar_t unsigned short int


wchar_t* cwxml(const unsigned short int *);
//#define xstring std::u16string;
//#include <initializer_list>
using namespace std;
#endif
char *cstxml(const xwchar_t *a,int sz);
xwchar_t *cpyxch(const xwchar_t *,int);
#ifndef __cplusplus_cli
extern unsigned short int tx[2];
class xstring : public std::basic_string<unsigned short int>
{
public:
xstring():basic_string<unsigned short int>(){;}
xstring(const char *in): basic_string<unsigned short int>(tx)  //cxml(in))
{
  ;
}
xstring(const wchar_t *in):  basic_string<unsigned short int>(cxml(in))
{
  ;
  
}
xstring(size_t sz,const wchar_t ch): basic_string<unsigned short int>(sz,(unsigned short int)ch){;}
xstring(const unsigned short int *is):basic_string<unsigned short int>(is){;}
xstring &operator=(const basic_string<unsigned short int> xs){basic_string<unsigned short int>::operator=(xs);}
xstring(std::basic_string<unsigned short int> xs):basic_string<unsigned short int>(xs){;}
const unsigned short int*c_str() {return(basic_string<unsigned short int>::c_str());}
};

#endif
#endif
#define _stringdef 1 
//#define xstring ystring<unsigned short int>


