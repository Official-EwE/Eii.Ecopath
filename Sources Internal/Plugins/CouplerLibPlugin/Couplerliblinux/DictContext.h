#pragma once

//using namespace System;
//using namespace System::Collections::Generic;
#include<xercesc/dom/DOM.hpp>
#include<list>
#include<string>

#include "stringdefs.h"
using namespace xercesc;
using namespace std;

namespace Couplerlib
{
class DictContext
{
public:
	DictContext(void);
	bool Check(xstring Model,xstring Interf,xstring Group,xstring Constituent);
	bool Check(xstring Model,xstring Interf);
	void Assemble(DOMNodeList *xmlnodes);
	list<const XMLCh *> allowedmodels;
	list<const XMLCh *> allowedinterfs;
	list<const XMLCh *> allowedgroups;
	list<const XMLCh *> allowedconstituents;

};
}
