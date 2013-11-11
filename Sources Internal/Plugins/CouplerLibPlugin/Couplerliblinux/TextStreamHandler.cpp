#include "StdAfx.h"
#include "TextStreamHandler.h"
using namespace Couplerlib;
using namespace System::Windows::Forms;
using namespace System::Text;
using namespace System;

TextStreamHandler::TextStreamHandler()
{
	canread=false;
	canwrite=false;
}

TextStreamHandler::TextStreamHandler(bool icanread, bool icanwrite)
{
	canread=icanread;
	canwrite=icanwrite;
}

void TextStreamHandler::AttachWriter(System::Windows::Forms::TextBox ^icontrol)
{
	Control=icontrol;
}
/*	
void TextStreamHandler::Write(array<unsigned char> ^outbuf,int start,int count)
{
	String ^gstr;
	gstr=Encoding::ASCII->GetString(outbuf,start,count);
	Control->Text=gstr;
}
*/

void TextStreamHandler::writemessage()
{

}