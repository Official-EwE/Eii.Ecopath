#pragma once
using namespace System;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace System::Text;
#include "dispdelegates.h"

namespace Couplerlib
{
public ref class TextStreamHandler
{
public:	TextStreamHandler(bool canread, bool canwrite);
		TextStreamHandler();
		void AttachWriter(System::Windows::Forms::TextBox ^icontrol);
		
		void writemessage();
private:
	bool canread,canwrite;

	TextBoxBase ^ Control;
};
}