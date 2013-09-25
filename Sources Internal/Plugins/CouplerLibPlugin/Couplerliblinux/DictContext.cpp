#include "StdAfx.h"
#include "DictContext.h"


//using namespace System::Xml;
using namespace Couplerlib;

DictContext::DictContext(void)
{
	this->allowedmodels=*(new list<const XMLCh *>);
	this->allowedgroups=*(new list<const XMLCh *>);
	this->allowedinterfs=*(new list<const XMLCh *>);
	this->allowedconstituents=*(new list<const XMLCh *>);
}

void DictContext::Assemble(DOMNodeList *xmlnodes)
{
	unsigned int n;
	for (n=0;n<xmlnodes->getLength();n++)
	{
		if (xmlnodes->item(n)->getNodeName()==xstring(L"Model"))
			{
				allowedmodels.push_back(xmlnodes->item(n)->getTextContent());
			}
			if (xmlnodes->item(n)->getNodeName()==xstring(L"Interface"))
			{
				allowedinterfs.push_back(xmlnodes->item(n)->getTextContent());
			}
			if (xmlnodes->item(n)->getNodeName()==xstring(L"GroupName"))
			{
				this->allowedgroups.push_back(xmlnodes->item(n)->getTextContent());
			}
			if (xmlnodes->item(n)->getNodeName()==xstring(L"Constituent"))
			{
				this->allowedconstituents.push_back(xmlnodes->item(n)->getTextContent());
		}
	}
}


bool DictContext::Check(xstring Model, xstring Interf)
{
	bool modok,interfok;
	list<const XMLCh *>::iterator XMLit;
	modok=false;
	interfok=false;
	if (allowedmodels.size()==0)
	{modok=true;}
	for (XMLit=allowedmodels.begin();XMLit!=allowedmodels.end();XMLit++)
	{
			if(Model==xstring(*XMLit))
		{
			modok=true;
		}
	}
	if (allowedinterfs.size()==0)
	{interfok=true;
	}
	for (XMLit=allowedinterfs.begin();XMLit!=allowedinterfs.end();XMLit++)
	{
		if (Interf==xstring(*XMLit))
		{
			interfok=true;
		}
	}
return(modok&&interfok);
}

bool DictContext::Check(xstring Model, xstring Interf, xstring Group, xstring Constituent)
{
	bool modifok,groupok,constok;
	list<const XMLCh *>::iterator XMLit;
	groupok=false;
	constok=false;
	modifok=Check(Model,Interf);
	if (allowedgroups.size()==0)
	{
		groupok=true;
	}
	for (XMLit=allowedgroups.begin();XMLit!=allowedgroups.end();XMLit++)
	{
		if (Group==xstring(*XMLit))
		{
			groupok=true;
		}
	}
	if (allowedconstituents.size()==0)
	{
		constok=true;
	}
	for (XMLit=allowedconstituents.begin();XMLit!=allowedconstituents.end();XMLit++)
	{
		if(Constituent==xstring(*XMLit))
		{
			constok=false;
		}
	}
	return(modifok&&groupok&&constok);
}

