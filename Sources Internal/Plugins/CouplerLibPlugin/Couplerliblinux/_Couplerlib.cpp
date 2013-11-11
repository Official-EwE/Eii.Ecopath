// This is the main DLL file.



#include "StdAfx.h"
#include "math.h"
#include <time.h>
//#include <wchar.h>

//#include "DictContext.h"
#include "ProtocolSock.h" 
#include "DataSock.h"
#include "MessageSock.h"
#include "pipelink.h"
#include "Couplerlib.h"
//#undef DOMDocument 
#include<xercesc/dom/DOM.hpp>
#include<xercesc/dom/DOMImplementation.hpp>
using namespace Couplerlib;
using namespace xercesc;
//using namespace System::Threading;

Coupler::Coupler()
{
usenetwork=false;
}



Coupler::Coupler(bool usesocks,bool iserver,bool isactive)
{   int tr,rec;
	//int nci;
    xstring *errob,*conob;
	usenetwork=usesocks;
	conob=new xstring(L"Consolepipe");
errob=new xstring(L"Errorpipe");
	pp1=new pipelink(&cont,conob);
	pp2=new pipelink(&errt,errob);
	if (usesocks)
	{
		
        ps = new ProtocolSock(iserver, isactive);
#ifdef __cplusplus_cli
		linkglue ^lnkgl=gcnew linkglue(NULL,0,ps->serverep);
		iThread *backthread=new iThread(gcnew System::Threading::ThreadStart(lnkgl,&linkglue::Receive));
#else
		iThread *backthread=new iThread(NULL,NULL);
#endif
		//Thread *backthread=new Thread(new ThreadStart(ps,&ProtocolSock::Pollreceive));
        backthread->Start();
		if (iserver)
		{tr=5002;
		rec=5003;
		}
		else
		{tr=5003;
		rec=5002;
		}
		tds= new DataSock(this,true,tr);
		rds= new DataSock(this,false,rec);
		outputmessage=new MessageSock(this,!iserver,5004);
	    errormessage=new MessageSock(this,!iserver,5005);
		if (iserver)
		{
			errormessage->ReceiveOuterLoop();
			outputmessage->ReceiveOuterLoop();
		}
   #ifdef __cplusplus_cli
#ifdef _Has_GDI
	pp1->setdelegate(nullptr,nullptr,true,outputmessage);
	pp2->setdelegate(nullptr,nullptr,true,errormessage);
#endif
#endif
	rds->ReceiveOuterLoop();	

	}
	
}

Coupler::~Coupler()
{
	;
}

void Coupler::EstablishTransmit(xstring newdest)
{
	if (usenetwork)
	{
tds->InitiateSend(newdest);
outputmessage->InitiateSend(newdest);
errormessage->InitiateSend(newdest);
	}
}

void Coupler::FinishTransmit()
{
//ds->SleepAgain();
}

double Coupler::GetDataItem(int mfrom,int intfrom, int orgfrom)
{
	return orgbuf[mfrom][intfrom][orgfrom];
}

int Coupler::Initialize(string isearchroot,string dict,xstring xmlname)
{   int a,b;
    searchroot=*(new xstring(isearchroot.length(),L' '));
	std::copy(isearchroot.begin(),isearchroot.end(),isearchroot.begin());
	a=SpecifyInterface(xmlname);
	if (a)
	{
	  ctextd(L"Could not open I/F file "); //+xmlname);
	}

	b=LoadDictionary(dict);
	if (b)
	{
		ctextd(L"Could not open Dict File "); //+dict);
	}
	return(a+b);
}

int Coupler::SpecifyInterface(xstring xmlname)
{unsigned int n,m;
    DOMElement *DE;
	varnames=*(new list<const XMLCh *>);
	valuenames=*(new list<const XMLCh *>);
	specfilenames=new vector<const XMLCh *>;
	try
	{
	parser=new XercesDOMParser;
	parser->parse(xmlname.c_str());
	Specification= parser->getDocument(); 
	NL = Specification->getElementsByTagName(L"SystemVariable");
	for (n=0;n<NL->getLength();n++)
	{
		DE=(DOMElement *)(NL->item(n));
		for (m=0;m<DE->getAttributes()->getLength();m++)
		{
			if(DE->getTagName()==L"Variable")
			{varnames.push_back(DE->getAttributes()->item(m)->getNodeValue());}
			if (((DOMElement *)(DE->getAttributes()->item(m)))->getTagName()==L"Value")
			{valuenames.push_back(DE->getAttributes()->item(m)->getNodeValue());}
		}
	}
	NL=Specification->getElementsByTagName(L"SpecificationFileList");
	for (n=0;n<NL->item(0)->getChildNodes()->getLength();n++)
	{
		specfilenames->push_back(NL->item(0)->getChildNodes()->item(n)->getNodeValue());
	}
	NL=Specification->getElementsByTagName(L"ModelInterface");
	}
	catch(const XMLException &xmle)
	{
		return(1);
	}
	return(0);
}


bool Coupler::CheckInterface(vector<int> *lint)
{
	unsigned int n,m;
	int retok,fretok,tretok,comtok;
	wchar_t *srtemp;
	//bool interfacefound;
	//DOMNode *Name;
	DOMImplementation *pImpl = DOMImplementation::getImplementation();
	vector<DOMNode *> description;
	DOMNodeList /**interfaceparts,*/*Interfacedes/**implementparts*/;
	//vector<DOMNodeList *> *interfaces;
	vector<xercesc::DOMDocument *> doc;
	vector<const XMLCh *> ifname;
	int nofiles=specfilenames->size();
	description=*(new vector<DOMNode *>(nofiles));
	modelnames=*(new vector<xstring>(nofiles));
	systemnames=*(new vector<xstring>(nofiles));
	languagenames=*(new vector<xstring>(nofiles));
	languageversion=*(new vector<double>(nofiles));
	modelversion=*(new vector<double>(nofiles));
	systemversion=*(new vector<double> (nofiles));
	subinterf=*(new vector<vector<xstring> >(nofiles));
	ifmeth=*(new vector<vector<int> >(nofiles));
	nodims=*(new vector<vector<int> >(nofiles));
	ifdir=*(new vector<vector<int> >(nofiles));
	thrd=*(new vector<vector<int> >(nofiles));
	cstime=*(new vector<vector<tm> >(nofiles));
	cetime=*(new vector<vector<tm> >(nofiles));
	citime=*(new vector<vector<double> >(nofiles));
	dims=*(new vector<vector<int> >(nofiles));
	ifname=*(new vector<const XMLCh *>(nofiles));
	doc=*(new vector<xercesc::DOMDocument *>(nofiles));
	orgname=*(new  vector<vector<vector<XMLCh *> > >(nofiles)); 
	orgsname=*(new vector<vector<vector<const XMLCh *> > >(nofiles));
	orgssymb=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgsdes=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgsconst=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgunits=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgtype=*(new vector<vector<vector<int> > >(nofiles));
	orgdir=*(new vector<vector<vector<int> > >(nofiles));
	curorg=*(new vector<vector<vector<int> > >(nofiles));
	masterpresratio=*(new vector<vector<vector<int> > >(nofiles));
	orgbuf=*(new vector<vector<double>*>(nofiles));
	mastercon=*(new vector<vector<vector<vector<int> > > >(nofiles));
	masterunits=*(new vector<vector<vector<vector<double> > > >(nofiles));
	masterunitspf=*(new vector<vector<vector<vector<double> > > >(nofiles));
	masterunitslink=*(new vector<vector<vector<vector<int> > > >(nofiles));
	masterratio=*(new vector<vector<vector<vector<double> > > >(nofiles));
	supvarvals=*(new vector<vector<vector<vector<double> > > >(nofiles));
	supvarrefs=*(new vector<vector<vector<vector<int> > > >(nofiles));
	supvarorgnames=*(new vector<vector<vector<vector<const wchar_t *> > > >(nofiles));
	supvarnames=*(new vector<vector<vector<const wchar_t *> > >(nofiles));
	supvardefault=*(new vector<vector<vector<double> > >(nofiles));
	supvarnovalues=*(new vector<vector<vector<int> > >(nofiles));
	supvarnovariables=*(new vector<vector<unsigned int> >(nofiles)); 
	gmodels=*(new vector<int>(4));
	//list<const XMLCh *>::iterator nit=specfilenames.begin();
	for (n=0;n<specfilenames->size();n++)
	{
	try
	{
        srtemp=new wchar_t[searchroot.length()+2+wcslen((*specfilenames)[n])];
		wcscpy_s(srtemp,wcslen(srtemp),(searchroot+xstring(L"/")).c_str());
		wcscat_s(srtemp,wcslen(srtemp),(*specfilenames)[n]);
		parser->parse(srtemp);
		doc.push_back(parser->getDocument());
		delete srtemp;
	}
	catch(const XMLException &xmle)
	{
		this->ctextd("Could Not load xml File ");
	}
	GetModelDescriptionInterf(description[n],doc[n],n);
	curorg[n]=*(new vector<vector<int> >(NL->getLength()));
	orgbuf[n]=new vector<double>(NL->getLength());
	}
    gtable=*(new vector<int>(NL->getLength()*4));
    gntable=*(new vector<xstring>(NL->getLength()*4));
	for (n=0;n<NL->getLength();n++)
	{
	Interfacedes=NL->item(n)->getChildNodes();
	fretok=-1;
	tretok=-1;
	comtok=-1;
	checks=*(new vector<list<int> >(7)); 
	for (m=0;m<Interfacedes->getLength();m++)
	{
		if (Interfacedes->item(m)->getNodeName()==L"InterfaceName")
		{ifname[n]=(Interfacedes->item(m)->getNodeValue());}
		if (Interfacedes->item(m)->getNodeName()==L"ModelFrom")
		{fretok=GetDirectionInterf(Interfacedes->item(m),m,0,n);
		}
		if (fretok!=-1)
		{
		if (Interfacedes->item(m)->getNodeName()==L"ModelTo")
		{tretok=GetDirectionInterf(Interfacedes->item(m),m,1,n);
		}
		}
		if (Interfacedes->getLength()==4)
		{
			if ((fretok!=-1)&&(tretok!=-1))
			{
				if (Interfacedes->item(m)->getNodeName()==L"ModelBidir")
				{comtok=GetDirectionInterf(Interfacedes->item(m),m,2,n);
				}
			}
		}
	}
	retok=0;
	if ((fretok!=-1)&&(tretok!=-1))
	{
		if (((ifdir[gmodels[0]][fretok])==3)&&(ifdir[gmodels[1]][tretok]==3)&&(comtok==-1)) //bidir 
		{
    retok=JoinEnds(fretok,tretok,gmodels[0],gmodels[1],2);
	if (retok>0)
	{
	retok=JoinEnds(tretok,fretok,gmodels[1],gmodels[0],2);
	gtable[n*4]=gmodels[0];
	gtable[n*4+1]=gmodels[1];
	gtable[n*4+2]=gmodels[1];
	gtable[n*4+3]=gmodels[0];
	gntable[n*4]=modelnames[gmodels[0]];
	gntable[n*4+1]=modelnames[gmodels[1]];
	gntable[n*4+2]=modelnames[gmodels[1]];
	gntable[n*4+3]=modelnames[gmodels[0]];
	}
		}
		if ((ifdir[gmodels[0]][fretok]==2)&&(ifdir[gmodels[1]][tretok]==1)&&(comtok>=0)) // to-From-Bidir
		{
	retok=JoinEnds(fretok,comtok,gmodels[0],gmodels[2],1);
	if (retok>0)
	{
	retok=JoinEnds(comtok,tretok,gmodels[2],gmodels[1],1);
	retok*=3;
	gtable[n*4]=gmodels[0];
	gtable[n*4+1]=gmodels[2];
	gtable[n*4+2]=gmodels[2];
	gtable[n*4+3]=gmodels[1];
	gntable[n*4]=modelnames[gmodels[0]];
	gntable[n*4+1]=modelnames[gmodels[2]];
	gntable[n*4+2]=modelnames[gmodels[2]];
	gntable[n*4+3]=modelnames[gmodels[0]];
	}
		}
    if ((ifdir[gmodels[0]][fretok]==2)&&(ifdir[gmodels[1]][tretok]==1)&&(comtok=-1)) //From-To Singular 
	{
	retok=JoinEnds(fretok,tretok,gmodels[0],gmodels[1],0);
	//ctextd(retok);
	if (retok>0)
	{
	gtable[n*4]=gmodels[0];
	gtable[n*4+1]=gmodels[1];
	gtable[n*4+2]=-1;
	gtable[n*4+3]=-1;
	gntable[n*4]=modelnames[gmodels[0]];
	gntable[n*4+1]=modelnames[gmodels[1]];
	gntable[n*4+2]=L"NOTUSED";
	gntable[n*4+3]=L"NOTUSED";
	}
	else
	{
		ctextd("Join Failed for Model "); //+gmodels[0]+ " to "+gmodels[1]);
	}
	}
	}
	}
	if (retok==0)
	{
	ctextd("CheckInterfaceStatus Failed");
	}
	else
	{
	ctextd("Interface OK");
	}
	if (retok==0)
	{
		for (n=0;n<NL->getLength()*4;n++)
		{
		//	ctextd(Convert::ToString(n)); //+"  "+gntable[n]);
		}
	}
	return(retok>0);
	}

	double Coupler::GetFromBuffer(int modno,int ifno,int orgno)
	{double rval;
		if (usenetwork)
		{
           tds->SendLoop(modno,ifno,orgno,rval);
			   return(rval);
		}
		else
		{
			return orgbuf[modno][ifno][orgno];
			}

	}

int Coupler::GetIf(int modelno,int ifno, int modelxno, int ifxno,int modelyno,int ifyno, vector<double> &modeldat)
{
	unsigned int nparts,n,m,i,k,strflag;
	double orgam;
	vector<double> unitstore;
	nparts=masterpresratio[modelno][ifno].size();
	modeldat=*(new vector<double>(nparts));
	for (n=0;n<nparts;n++)
	{
		orgam=0.0;
		unitstore=*(new vector<double> (mastercon[modelno][ifno][n].size()));
		for (m=0;m<mastercon[modelno][ifno][n].size();m++)
		{
			if (masterunitspf[modelno][ifno][n][m]!=0.0)
			{
			unitstore[m]=pow(GetFromBuffer(modelxno,ifxno,masterunitslink[modelno][ifno][n][m]),masterunitspf[modelno][ifno][n][m]);
			}
			else
			{
				unitstore[m]=1.0;
			}
		}
	
		if (masterpresratio[modelno][ifno][n]==0)
		{
		for (m=0;m<mastercon[modelno][ifno][n].size();m++)
		{
			orgam+=GetFromBuffer(modelxno,ifxno,mastercon[modelno][ifno][n][m])*masterunits[modelno][ifno][n][m]*unitstore[m]*masterratio[modelno][ifno][n][m];
		}
		}
		if (masterpresratio[modelno][ifno][n]==1)
		{

			for (m=0;m<mastercon[modelno][ifno][n].size();m++)
			{
				masterratio[modelno][ifno][n][m]=GetFromBuffer(modelxno,ifxno,mastercon[modelno][ifno][n][m])*masterunits[modelno][ifno][n][m]*unitstore[m];
				orgam+=masterratio[modelno][ifno][n][m];
			}
			for (m=0;m<mastercon[modelno][ifno][n].size();m++)
			{
				masterratio[modelno][ifno][n][m]/=orgam;
			}
		}
		if (masterpresratio[modelno][ifno][n]==2)
		{
			for (m=0;m<mastercon[modelno][ifno][n].size();m++)
		{strflag=0;
			for (i=0;i<curorg[modelyno][ifyno].size();i++)
			{
				if ((i==mastercon[modelno][ifno][n][m])&&(masterpresratio[modelyno][ifyno][i]==1))
				{
					for (k=0;k<mastercon[modelyno][ifyno][i].size();k++)
					{
						if (mastercon[modelyno][ifyno][i][k]==n)
						{
							strflag=1;
			orgam+=GetFromBuffer(modelxno,ifxno,mastercon[modelno][ifno][n][m])*masterunits[modelno][ifno][n][m]*unitstore[m]*masterratio[modelyno][ifyno][i][k];
						}
					}
				}
			}
			if (strflag==0)
			{
				orgam+=GetFromBuffer(modelxno,ifxno,mastercon[modelno][ifno][n][m])*masterunits[modelno][ifno][n][m]*unitstore[m];
			}
			}
		}

		modeldat[n]=orgam;
	}
	return(nparts);
}


int Coupler::OrgReference(int mod,int intf,int seq)
{
	return(curorg[mod][intf][seq]);
}

void Coupler::PutIf(int modelno,int ifno,vector<double> &modeldat,int noitems)
{
	int nxparts,n;
	nxparts=curorg[modelno][ifno].size();
	for (n=0;n<nxparts;n++)
	{
	orgbuf[modelno][ifno][n]=modeldat[curorg[modelno][ifno][n]];
	}
}



vector<int> &Coupler::GetIfAddress(vector<int> &modelx,xstring intername,bool isinput,bool ispaired)
{ 
	unsigned int n,q;
	vector<int> retlist=*(new vector<int>);
	modelx=*(new vector<int>);
	for (n=0;n<gtable.size();n+=2)
	{
		if ((gntable[n]==intername)&&((isinput&&(ispaired))||(!isinput&&!ispaired)))
		{
		retlist.push_back(n/4);
		if (ispaired)
		{
		modelx.push_back(gtable[n+1]);
		}
		else
		{
		modelx.push_back(gtable[n]);
		}
		}
		if ((gntable[n+1]==intername)&&(((isinput)&&(!ispaired))||((!isinput)&&(ispaired))))
		{
			retlist.push_back(n/4);
			if (ispaired)
			{
				modelx.push_back(gtable[n]);
			}
			else
			{
			modelx.push_back(gtable[n+1]);
			}
		}
	}
	return(retlist);
}

int Coupler::JoinEnds(int fend,int tend,int fmodel,int tomodel,int ltype)
{
	unsigned int n,m,k,i,temp;
	bool islinked,islinked2,islinked3;
	int linkfail,orgpresratio;
	linkfail=0;
	xstring errtext;
	const wchar_t *tounits;
	list<vector<xstring> >linklist;
	list<vector<double> > linkratiolist;
	list<int> ratioprestype;
	vector<int> orgarray;
	vector<double> orgratio; 
	vector<xstring> orgunx;
	xstring unitsymb,storelast;
		mastercon[tomodel][tend]=*(new vector<vector<int> > (curorg[tomodel][tend].size()));
		masterunits[tomodel][tend]=*(new vector<vector<double> > (curorg[tomodel][tend].size()));
		masterunitspf[tomodel][tend]=*(new vector<vector<double> > (curorg[tomodel][tend].size()));
		masterunitslink[tomodel][tend]=*(new vector<vector<int> >(curorg[tomodel][tend].size()));
		masterratio[tomodel][tend]=*(new vector<vector<double> > (curorg[tomodel][tend].size()));
		masterpresratio[tomodel][tend]=*(new vector<int>(curorg[tomodel][tend].size()));
		for (n=0;n<curorg[tomodel][tend].size();n++)
		{
			tounits=orgunits[tomodel][tend][curorg[tomodel][tend][n]];
			islinked=false;
			for (m=0;m<curorg[fmodel][fend].size();m++)
			{
				if (orgname[tomodel][tend][curorg[tomodel][tend][n]]==orgname[fmodel][fend][curorg[fmodel][fend][m]]) //direct equivalent
				{orgarray=*(new vector<int>(1));
				orgunx=*(new vector<xstring>(1));
				orgratio=*(new vector<double>(1));
				orgarray[0]=m; //curorg[gmodels[2]][fend][m];
				orgunx[0]=xstring(orgunits[fmodel][fend][curorg[fmodel][fend][m]]);
				orgratio[0]=1.0;
				orgpresratio=0;
				islinked=true;
				break;
				}
			}
			if (!islinked) //can't find in o/p list - look in translation tables
			{
				list<vector<xstring> >::iterator lki=linklist.begin();
				list<vector<double> >::iterator lkr=linkratiolist.begin();
				list<int>::iterator lkp=ratioprestype.begin();
				ratioprestype=GetRegroup(xstring(orgname[tomodel][tend][curorg[tomodel][tend][n]]),linklist,linkratiolist,modelnames[tomodel],subinterf[tomodel][tend]);
				for (k=0;k<linklist.size();k++)//for all possible rules
				{
					
					orgarray=*(new vector<int>((*lki).size()));
					orgunx=*(new vector<xstring>((*lki).size()));
					orgratio=*(new vector<double>((*lki).size()));
					for (i=0;i<(*lki).size();i++) //connections needed in each rule
					{
						islinked2=false;
						for (m=0;m<curorg[fmodel][fend].size();m++) 
						{
							if ((*lki)[i]==orgname[fmodel][fend][curorg[fmodel][fend][m]]) //direct equivalent
							{islinked2=true;
							orgarray[i]=m; //curorg[gmodels[0]][fend][m];
							orgunx[i]=xstring(orgunits[fmodel][fend][curorg[fmodel][fend][m]]);
							orgratio[i]=(*lkr)[i];
							if ((*lki).size()==1)
							{orgpresratio=(*lkp)*2;
							}
							else
							{
							orgpresratio=*lkp;
							}
							break;
							}
						}
						lki++;
						lkr++;
						lkp++;
						if (!islinked2) // can't connect 1 item
						{
						ctextd("Can't Link Organism "); //+linklist[k][i]);
						break;}
						else
						{
							islinked=true;
						}
					}
					if (islinked)
					{break;
					}
				}
			}
			if (!islinked)
			{linkfail=0;
			break;
			}
			else
			{mastercon[tomodel][tend][n]=orgarray;
			masterratio[tomodel][tend][n]=orgratio;
			masterpresratio[tomodel][tend][n]=orgpresratio;
			masterunits[tomodel][tend][n]=*(new vector<double>(orgarray.size()));
			masterunitspf[tomodel][tend][n]=*(new vector<double>(orgarray.size()));
			masterunitslink[tomodel][tend][n]=*(new vector<int>(orgarray.size()));
			linkfail=1;
			for (k=0;k<orgarray.size();k++)
			{masterunits[tomodel][tend][n][k]=ConvertUnits(tounits,orgunx[k],linkfail,modelnames[tomodel],subinterf[tomodel][tend],orgname[tomodel][tend][curorg[tomodel][tend][n]],orgsconst[tomodel][tend][curorg[tomodel][tend][n]], masterunitspf[tomodel][tend][n][k],unitsymb);
			if (unitsymb==L"ERROR")
			{
				islinked3=false;
			}
			else
			{
			//if (masterunitspf[tomodel][tend][n][k]!=0.0)
			if ((unitsymb==L"UNDEFINED")||(unitsymb==L"SAME"))
			{
				islinked3=true;
			}
			else
			{   islinked3=false;
				for (i=0;i<curorg[fmodel][fend].size();i++)
			{
				//islinked3=false;
				if (unitsymb==orgname[fmodel][fend][curorg[fmodel][fend][i]])
				{//ctextd(unitsymb+" T  "+orgname[fmodel][fend][curorg[fmodel][fend][i]]);
					masterunitslink[tomodel][tend][n][k]=i;
				islinked3=true;
				break;
				}
				else
				{
					storelast=unitsymb+L" F  "+orgname[fmodel][fend][curorg[fmodel][fend][i]];
				}
				}
			}
			}
            if (!islinked3)
	{linkfail=0;
	//ctextd("Interface Linking Failed");
	//ctextd(storelast);
	}
			}
			}

		}
    
	return(linkfail);
}



double Coupler::ConvertUnits(const wchar_t *tostr,xstring &fmstr,int &fail,xstring modelname, xstring ifxname,xstring onames, xstring constnames, double &muldiv, xstring &cvname)
{
	unsigned int n;
	wchar_t *wsmodel=new wchar_t[modelname.length()+2];
	wchar_t *wifxname=new wchar_t[ifxname.length()+2];
	wchar_t *wsonames=new wchar_t[onames.length()+2]; 
	wchar_t *wsconstnames=new wchar_t[constnames.length()+2];
	xstring wmodel=*(new xstring(modelname.length(),L' '));
	xstring wifx=*(new xstring(ifxname.length(),L' '));
    xstring wonames=*(new xstring(onames.length(),L' '));
	xstring wconstnames=*(new xstring(constnames.length(),L' '));
	std::copy(modelname.begin(),modelname.end(),wmodel.begin());
	std::copy(ifxname.begin(),ifxname.end(),wifx.begin());
	std::copy(onames.begin(),onames.end(),wonames.begin());
	std::copy(constnames.begin(),constnames.end(),wconstnames.begin());
	wcscpy_s(wsmodel,wmodel.length(),wmodel.c_str()); 
	wcscpy_s(wifxname,wifx.length(),wifx.c_str());
	wcscpy_s(wsonames,wonames.length(),wonames.c_str()); 
	wcscpy_s(wsconstnames,wconstnames.length(),wconstnames.c_str());
    int ifail;
	double conversion;
	ifail=1;
	cvname=L"UNDEFINED";
	conversion=0.0;
	if (fmstr==tostr)
	{
		ifail=0;
		conversion=1.0;
		muldiv=0.0;
		cvname=L"SAME";
	}
	else
	{
	for (n=0;n<convertunits.size();n++)
	{
		if(convertunits[n]==fmstr)
		{if ((convertto[n]==tostr)&&(convertcontext[n]->Check(wsmodel,wifxname,wsonames,wsconstnames)))
		{ifail=0;
		conversion=convertratio[n];
		muldiv=convertmultdivp[n];
		if (convertmultdivp[n]!=0.0)
		{cvname=convertmuldiv[n];
		}
		break;
		}
		}
	}
	}
    if (ifail==1)
	{fail=1;
	cvname=L"ERROR";
	}
	return(conversion);
}



int Coupler::GetDirectionInterf(DOMNode *dirnode,int compno,int isfrom,int ifno)
{
	unsigned int n,m,k,noallowed,i,j;
	int ar,icflag;
	int modelfound,sysfound,langfound,gridtype;
	list<int> startok,endok,intok,modellinkok,gridok,comptest;
	modelfound=-1;
	DOMNode *CaseNode,*SupNode;
	const XMLCh *allowlang,*allowsys;
	const XMLCh *tempname,*frname;
	tm startf,startt,endf,endt;
	double minint,maxint;
	bool hasmadelinks;
	double minver,maxver,langmin,langmax,sysmin,sysmax;
	ar=isfrom;
	hasmadelinks=false;
	for (n=0;n<dirnode->getChildNodes()->getLength();n++)
	{
		tempname=dirnode->getChildNodes()->item(n)->getNodeName();
		if(dirnode->getChildNodes()->item(n)->getNodeName()==L"Models")
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==L"ModelName")
					{
						frname=CaseNode->getNodeValue();
						
					}
					if (CaseNode->getNodeName()==L"MinimumVersion")
					{
						minver=convertd(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"MaximumVersion")
					{
						maxver=convertd(CaseNode->getNodeValue());
					}

				}
				modelfound=CheckTextName(frname,m,modelnames,L"Model Name");
				if (modelfound>=0)
				{
				modelfound=CheckTextVersion(minver,maxver,modelfound,modelversion,L"Model Version No.");
				}
				gmodels[ar]=modelfound;
			}

		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==L"Implementations")
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==L"AllowedLanguage")
					{
						allowlang=CaseNode->getNodeValue();
					}
					if (CaseNode->getNodeName()==L"AllowedSystem")
					{
						allowsys=CaseNode->getNodeValue();
					}
					if (CaseNode->getNodeName()==L"LanguageMinimumVersion")
					{
						langmin=convertd(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"LanguageMaximumVersion")
					{
						langmax=convertd(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"SystemMinimumVersion")
					{
						sysmin=convertd(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"SystemMaximumVersion")
					{
						sysmax=convertd(CaseNode->getNodeValue());
					}
				}
				langfound=CheckTextName(allowlang,m,languagenames,L"Language");
				if (langfound==modelfound)
				{
					langfound=CheckTextVersion(langmin,langmax,modelfound,languageversion,L"Language Version");
				}
				sysfound=CheckTextName(allowsys,m,systemnames,L"System");
				if (sysfound==modelfound)
				{
					sysfound=CheckTextVersion(sysmin,sysmax,m,systemversion,L"System Version");
				}
			}
		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==L"SupplementList")
		{
			int varcount=supvarnovariables[modelfound][ifno]=dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();
			supvarvals[modelfound][ifno]=*(new vector<vector<double> >(varcount));
			supvarrefs[modelfound][ifno]=*(new vector<vector<int> >(varcount));
			supvarorgnames[modelfound][ifno]=*(new vector<vector<const wchar_t *> >(varcount));
			supvarnames[modelfound][ifno]=*(new vector<const wchar_t *>(varcount));
			supvardefault[modelfound][ifno]=*(new vector<double> (varcount));
			supvarnovalues[modelfound][ifno]=*(new vector<int>(varcount));
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getNodeName()==L"Variable")
					for (k=0;k<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();k++)
					{
						SupNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
						if (SupNode->getNodeName()==L"Default")
						{
							supvardefault[modelfound][ifno][m]=convertd(SupNode->getNodeValue());
							
						}
						if (SupNode->getNodeName()==L"Name")
						{
							supvarnames[modelfound][ifno][m]=SupNode->getNodeValue();
						}
						if (SupNode->getNodeName()==L"ValueList")
						{
							supvarnovalues[modelfound][ifno][m]=SupNode->getChildNodes()->getLength();
							supvarvals[modelfound][ifno][m]=*(new vector<double>(SupNode->getChildNodes()->getLength()));
							supvarorgnames[modelfound][ifno][m]=*(new vector<const wchar_t *>(SupNode->getChildNodes()->getLength()));
							for (i=0;i<SupNode->getChildNodes()->getLength();i++)
							{
								if (SupNode->getChildNodes()->item(i)->getNodeName()==L"ValueItem")
								{
									for (j=0;j<SupNode->getChildNodes()->item(i)->getChildNodes()->getLength();j++)
									{
										if (SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeName()==L"Case")
										{
										 supvarorgnames[modelfound][ifno][m][i]=SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeValue();
										}
										if (SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeName()==L"Value")
										{
											supvarvals[modelfound][ifno][m][i]=convertd(SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeValue());
										}
									}
								}
							}
						}
					}
			}

		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==L"Timings")
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==L"MustStartAfter")
					{
						startf=convertt(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"MustStartBefore")
					{
						startt=convertt(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"MustEndAfter")
					{
						endf=convertt(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"MustEndBefore")
					{
						endt=convertt(CaseNode->getNodeValue());
					}
					if (CaseNode->getNodeName()==L"MiniumInterval")
					{
						minint=convertsp((xstring(CaseNode->getChildNodes()->item(0)->getNodeValue()).substr(1,xstring(CaseNode->getChildNodes()->item(0)->getNodeValue()).length()-2),0,0,0));
					}
					if (CaseNode->getNodeName()==L"MaximumInterval")
					{
						maxint=convertsp((xstring(CaseNode->getChildNodes()->item(0)->getNodeValue()).substr(1,xstring(CaseNode->getChildNodes()->item(0)->getNodeValue()).length()-2),0,0,0));
					}

				}
				checks[0]=startok=CheckDate(startf,startt,m,cstime[modelfound],L"Start Time");
				checks[1]=endok=CheckDate(endf,endt,m,cetime[modelfound],L"End Time");
				checks[2]=intok=CheckInterval(minint,maxint,m,citime[modelfound],L"Time Interval");
			}
		}
		if (dirnode->getChildNodes()->item(n)->getNodeName()==L"Grids")
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(0)->getNodeName()==L"GridFormNone")
				{
					gridtype=0;
				}
			checks[3]=gridok=CheckNumAllowed(gridtype,m,nodims[modelfound],L"Grid Dimensions");
			}
		}

		if(dirnode->getChildNodes()->item(n)->getNodeName()==L"DataList")
		{
			dataflux=*(new list<int>);
			datalist=*(new list<const wchar_t *>);
			dataconst=*(new list<const wchar_t *>);
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				for (i=0;i<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();i++)
				{
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeName()==L"DataItem")
				{   icflag=0;
					for (k=0;k<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->getLength();k++)
				{
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==L"Name")
					{datalist.push_back(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeValue());
					}
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==L"Constituent")
					{dataconst.push_back(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeValue());
					icflag=1;
					}
					}
					if (icflag==0)
					{
						dataconst.push_back(L"U");
				}

				}
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeName()==L"Flux")
				{
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"State")
			{
				dataflux.push_back(0);
			}

			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"Predation")
			{
				dataflux.push_back(1);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"GrossPrimaryProduction")
			{
				dataflux.push_back(2);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"Respiration")
			{
				dataflux.push_back(3);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"Excretion")
			{
				dataflux.push_back(4);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"Exudation")
			{
				dataflux.push_back(5);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeValue()==L"Uptake")
			{
				dataflux.push_back(6);
			}
				}
			}
			}
			checks[4]=modellinkok=LinkData(datalist,dataconst,dataflux,modelfound,ifno);
			hasmadelinks=true;

		}
		
	}
	for (m=0;m<supvarnovariables[modelfound][ifno];m++)
	{
		this->LinkSupp(supvarorgnames[modelfound][ifno][m],dataconst,modelfound,ifno,m,supvarnovalues[modelfound][ifno][m]);
	}
	if (!hasmadelinks)
	{
		checks[4]=modellinkok=LinkData(datalist,dataconst,dataflux,modelfound,ifno);
		
	}


	checks[5]=CheckDirs(modelfound,ifno,(1-isfrom)+1);
	comptest=PermittedInterfaces(checks,6);
	if (comptest.size()>0)
	{
	return(*(comptest.begin()));
	}
	else
	{return(-1);
	//ctextd("Could Not establish links");
	}
}



list<int> Coupler::PermittedInterfaces(vector<list<int> > checklist,int nochecks)
{
unsigned int n,m;
int checkok;
maxcheck=0;
list<int> retar=*(new list<int>);
list<int>::iterator chki;
for (n=0;n<checklist[0].size();n++)
{
	chki=checklist[0].begin();
if(*chki>(int)maxcheck)
{maxcheck=*chki;
}
chki++;
}
for (n=0;n<=maxcheck;n++)
{
checkok=1;
chki=checklist[n].begin();
for (m=0;m<(unsigned int)nochecks;m++)
{
	if (*chki==n)
	{checkok*=1;}
	else
	{checkok*=0;}
	chki++;
}
if (checkok==1)
{
retar.push_back(n);
}
}
return(retar);
}

list<int> Coupler::LinkSupp(vector<const wchar_t *> &iorgnames,list<const wchar_t *> &iorgconstit, int nno,int ifno,int varno,int noitems)
{ 
	int m,l;
	unsigned int k;
	bool allink;
	vector<int> orgtranstable;
	const wchar_t *inx;
	const wchar_t *outx;
	list<int> retar=*(new list<int>);
	supvarrefs[nno][ifno][varno]=*(new vector<int>(noitems));
	orgtranstable=Getsynonyms(modelnames[nno],subinterf[nno][ifno],orgname[nno][ifno],orgsconst[nno][ifno]);
	for (m=0;m<noitems;m++)
		{ allink=false;
		for (l=0;l<2;l++)
		{
			for (k=0;k<orgname[nno][ifno].size();k++)
			{
				inx=orgname[nno][ifno][k];
				outx=iorgnames[m];
				if (inx==outx)
				//if (orgname[tx][n][k]==iorgnames[m])
				{
				allink=true;
				supvarrefs[nno][ifno][varno][m]=orgtranstable[k]+1;
				break;
				}
			}
			if (allink)
			{break;}
		}
			if (allink==false)
			{   supvarrefs[nno][ifno][varno][m]=-1;
			break;}
	}
		if (allink==true)
		{
			retar.push_back(ifno);
		}
		else
		{
		ctextd("Could Not Find Organism "); // + outx);
		//System::Console::WriteLine("Could Not Find Organism " + outx);
		}

	return(retar);

}


list<int> Coupler::LinkData(list<const wchar_t *> iorgnames,list<const wchar_t *> iorgconstit, list<int> iorgflux, int nno,int ifno)
{
	int n,l,tx;
    unsigned int m,k;
	bool allink;
	xstring inx;
	xstring outx;
	vector<int> orgtranstable;
	list<int> retar=*(new list<int>);
	tx=nno;
	curorg[nno][ifno]=*(new vector<int>(iorgnames.size()));
	orgbuf[nno][ifno]=*(new vector<double>(iorgnames.size()));
	//for (n=0;n<this->orgname[nno]->Length;n++)
	{
		
		orgtranstable=Getsynonyms(modelnames[nno],subinterf[nno][ifno],orgname[nno][ifno],orgsconst[nno][ifno]);
		list<const wchar_t *>::iterator inmit=iorgnames.begin();
		list<const wchar_t *>::iterator iconit=iorgconstit.begin();
		for (m=0;m<iorgnames.size();m++)
		{ allink=false;
		for (l=0;l<2;l++)
		{
			for (k=0;k<orgname[nno][ifno].size();k++)
			{
				inx=orgname[tx][ifno][k];
				outx=xstring(*inmit);
				if (inx==outx)
				//if (orgname[tx][n][k]==iorgnames[m])
				{if ((orgsconst[tx][ifno][orgtranstable[k]]==*iconit)||(l==1))
				{allink=true;
				curorg[tx][ifno][m]=orgtranstable[k];
				break;
				}
				}
			}
			if (allink)
			{break;}
		}
			if (allink==false)
			{   curorg[tx][ifno][m]=-1;
			break;}
			inmit++;
			iconit++;
		}
		if (allink==true)
		{
			retar.push_back(ifno);
		}
		else
		{
		ctextd("Could Not Find Organism "); // + outx);
		//System::Console::WriteLine("Could Not Find Organism " + outx);
		}
	}

	return(retar);
}

list<int> Coupler::CheckDirs(int modelin,int interfin,int dirin)
{   unsigned int n;
	list<int> retar=*(new list<int>);
	for (n=0;n<ifdir.size();n++)
	{
		if ((dirin&ifdir[modelin][n])!=0)
		{retar.push_back(n);
		}
	}
	return(retar);
}


list<int> Coupler::GetRegroup(xstring &istring,list<vector<xstring> > &globstring,list<vector<double> > &globvals,xstring modelname,xstring ifxname)
{
	unsigned int n;
	list<int> retr=*(new list<int>); 
	globstring=*(new list<vector<xstring> >);
	globvals=*(new list<vector<double> >);
	for (n=0;n<mapgroups.size();n++)
	{if ((istring==mapgroups[n])&&(mapcontext[n]->Check(modelname,ifxname)))
	{globstring.push_back(mapgroupsto[n]);
	if (this->mappreserveratio[n])
	{retr.push_back(1);
	}
	else
	{retr.push_back(0);
	}
	globvals.push_back(this->mapratio[n]);
	}
	}
	return(retr);
}


vector<int> Coupler::Getsynonyms(xstring modelname, xstring ifxname, vector<wchar_t *> &onames, vector<const wchar_t *> constnames)
{
	int nosynonyms,offsetptr;
	unsigned int n,m,oldlength;
	vector<int> retr;
	nosynonyms=0;
	oldlength=onames.size();
	for (n=0;n<oldlength;n++)
	{
		for (m=0;m<synonymvals.size();m++)
		{
			if ((synonymvals[m]==onames[n])&&(synonymcontext[m]->Check(modelname,ifxname,onames[n],constnames[n])))
			{nosynonyms++;
			break;
			}
		}
	}
	//Array::Resize(onames,oldlength+nosynonyms);
    retr=*(new vector<int> (oldlength)); //+nosynonyms);
	offsetptr=oldlength;
	for (n=0;n<oldlength;n++)
	{retr[n]=n;
	for (m=0;m<synonymvals.size();m++)
		{
			if ((synonymvals[m]==onames[n])&&(synonymcontext[m]->Check(modelname,ifxname,onames[n],constnames[n])))
			{
				//ctextd(onames[n]); // + " Is a Synonym of " + synonyms[m]);
				//System::Console::WriteLine(onames[n] + " Is a Synonym of " + synonyms[m]);
				wcscpy_s(onames[n],wcslen(onames[n]),synonyms[m].c_str());
			
			//retr[offsetptr]=n;
			//offsetptr++;
			break;
			}
		}
	}
	return(retr);
}



int Coupler::CheckTextName(const wchar_t *frname,int index,vector<xstring >nametables,xstring message)
{
	int comp=-1;

	for (unsigned int n=0;n<nametables.size();n++)
	{if (nametables[n]==frname)
	{comp=n;
	break;
    //ctextd(message+frname+" To "+nametables[n]);
	//System::Console::WriteLine(message+frname+" To "+nametables[n]);
	}
	}
	
	return(comp);
}


list<int> Coupler::CheckNames(xstring frname,int index,vector<xstring> nametables,xstring message)
{
	list<int> retar=*(new list<int>);
	for (unsigned int n=0;n<nametables.size();n++)
	{if (nametables[n]==frname)
	{retar.push_back(n);
	//ctextd(message) ; //+frname+" To "+nametables[n]);
	//System::Console::WriteLine(message+frname+" To "+nametables[n]);
	}
	}
	
	return(retar);
}

list<int> Coupler::CheckNumAllowed(int numin,int index,vector<int> valuearray,xstring message)
{
	list<int> retar=*(new list<int>);
	for (unsigned int n=0;n<valuearray.size();n++)
	{
		if (valuearray[n]==numin)
		{
			retar.push_back(n);
			//ctextd(message+Convert::ToString(numin)+" To "+ Convert::ToString(valuearray[n]));
			//System::Console::WriteLine(message+Convert::ToString(numin)+" To "+ Convert::ToString(valuearray[n]));
		}
	}
	
	return(retar);
}


int Coupler::CheckTextVersion(double minval,double maxval,int index,vector<double> versions,xstring message)
{
	//ctextd(message+Convert::ToString(versions[index]) +" Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	//System::Console::WriteLine(message+Convert::ToString(versions[index]) +" Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
if ((versions[index]>=minval)&&(versions[index]<=maxval))
{
	return(index);
}
else
{
	return(-1);
}
}

list<int> Coupler::CheckAllowed(double minval,double maxval,int index,vector<double> versions,xstring message)
{
	unsigned int n;
	list<int> retar=*(new list<int>);
	for (n=0;n<versions.size();n++)
	{
		if ((versions[n]>=minval)&&(versions[n]<=maxval))
		{
			//ctextd(message+Convert::ToString(versions[index]) +" Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
			//System::Console::WriteLine(message+Convert::ToString(versions[index]) +" Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
			retar.push_back(n);
		}
	}
	return(retar);
}



list<int> Coupler::CheckDate(tm minval,tm maxval,int index,vector<tm> times,xstring message)
{
unsigned int n;
list<int> retar=*(new list<int>);
double dtx,dty;
for (n=0;n<times.size();n++)
{
dtx=difftime(mktime(&times[index]),mktime(&minval));
dty=difftime(mktime(&times[index]),mktime(&maxval));
if ((dtx>=0.0)&&(dty<=0.0))
{
	//ctextd(message+Convert::ToString(times[index])+ " Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	//System::Console::WriteLine(message+Convert::ToString(times[index])+ " Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	retar.push_back(n);
}
}
return(retar);
}

list<int> Coupler::CheckInterval(double minval,double maxval,int index,vector<double> times,xstring message)
{
unsigned int n;
list<int> retar=*(new list<int>);
for (n=0;n<times.size();n++)
{
if ((times[index]>=minval)&&(times[index]<=maxval))
{
	//ctextd(message+Convert::ToString(*(times[index]))+ " Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	//System::Cbleonsole::WriteLine(message+Convert::ToString(*(times[index]))+ " Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	retar.push_back(n);
}
}
return(retar);
}



int Coupler::GetModelDescriptionInterf(DOMNode *des,xercesc::DOMDocument *xdoc,int ifno)
				{
	unsigned int n,k,m,i, noparts;
	DOMNodeList *desparts,*interfpart;
	DOMNodeList *implementparts;
	DOMNode *cn;
	des=(xdoc->getElementsByTagName(L"Description"))->item(0);
	desparts=des->getChildNodes();
	for (m=0;m<desparts->getLength();m++)
	{
		if (desparts->item(m)->getNodeName()==L"ModelName")
		{modelnames[ifno]=desparts->item(m)->getNodeValue();
		}
		if (desparts->item(m)->getNodeName()==L"ModelVersion")
		{modelversion[ifno]=convertd(desparts->item(m)->getNodeValue());
		}
		if (desparts->item(m)->getNodeName()==L"ModelImplementation")
		{
			implementparts=desparts->item(m)->getChildNodes();
			for (k=0;k<implementparts->getLength();k++)
			{
				if (implementparts->item(k)->getNodeName()==L"SystemName")
				{
					systemnames[ifno]=implementparts->item(k)->getNodeValue();
				}
				if (implementparts->item(k)->getNodeName()==L"SystemVersion")
				{
					systemversion[ifno]=convertd(implementparts->item(k)->getNodeValue());
				}
				if (implementparts->item(k)->getNodeName()==L"ModelLanguage")
				{
					languagenames[ifno]=implementparts->item(k)->getNodeValue();
				}
				if (implementparts->item(k)->getNodeName()==L"LanguageVersion")
				{
					languageversion[ifno]=convertd(implementparts->item(k)->getNodeValue());
				}
			}
		}
	}
	interfpart=(xdoc->getElementsByTagName(L"Interface"));
	noparts=interfpart->getLength();
	subinterf[ifno]=*(new vector<xstring>(noparts));
	ifmeth[ifno]=*(new vector<int>(noparts));
	nodims[ifno]=*(new vector<int>(noparts));
	ifdir[ifno]=*(new vector<int>(noparts));
	thrd[ifno]=*(new vector<int>(noparts));
	cstime[ifno]=*(new vector<tm>(noparts));
	cetime[ifno]=*(new vector<tm>(noparts));
	citime[ifno]=*(new vector<double>(noparts));
	dims[ifno]=*(new vector<int>(noparts)); //dim 2 =3
	orgname[ifno]=*(new vector<vector<wchar_t *> >(noparts)); 
	orgsname[ifno]=*(new vector<vector<const wchar_t * > >(noparts));
	orgssymb[ifno]=*(new vector<vector<const wchar_t *> >(noparts)); 
	orgsdes[ifno]=*(new vector<vector<const wchar_t *> >(noparts)); 
	orgsconst[ifno]=*(new vector<vector<const wchar_t *> >(noparts)); 
	orgunits[ifno]=*(new vector<vector<const wchar_t * > >(noparts)); 
	orgtype[ifno]=*(new vector<vector<int> >(noparts));
	orgdir[ifno]=*(new vector<vector<int> >(noparts));
	masterpresratio[ifno]=*(new vector<vector<int> >(noparts));
	mastercon[ifno]=*(new vector<vector<vector<int> > >(noparts));
	masterunits[ifno]=*(new vector<vector<vector<double> > >(noparts));
	masterunitspf[ifno]=*(new vector<vector<vector<double> > >(noparts));
	masterunitslink[ifno]=*(new vector<vector<vector<int> > >(noparts));
	masterratio[ifno]=*(new vector<vector<vector<double> > >(noparts));
	supvarvals[ifno]=*(new vector<vector<vector<double> > >(noparts));
	supvarrefs[ifno]=*(new vector<vector<vector<int> > >(noparts));
	supvarorgnames[ifno]=*(new vector<vector<vector<const wchar_t *> > >(noparts));
	supvarnames[ifno]=*(new vector<vector<const wchar_t *> >(noparts));
	supvardefault[ifno]=*(new vector<vector<double> >(noparts));
	supvarnovalues[ifno]=*(new vector<vector<int> >(noparts));
	supvarnovariables[ifno]=*(new vector<unsigned int>(noparts)); 
	for (n=0;n<noparts;n++)
	{
		for (m=0;m<interfpart->item(n)->getChildNodes()->getLength();m++)
		{
			cn=interfpart->item(n)->getChildNodes()->item(m);
			if (cn->getNodeName()==L"InterfaceName")
			{subinterf[ifno][n]=cn->getNodeValue();
			}
			if (cn->getNodeName()==L"InterfaceMethod")
			{if (cn->getNodeValue()==L"Direct")
			{ifmeth[ifno][n]=1;}
			if (cn->getNodeValue()==L"Managed")
			{ifmeth[ifno][n]=2;}
			if (cn->getNodeValue()==L"ASCII")
			{ifmeth[ifno][n]=3;}
			if (cn->getNodeValue()==L"NetCDF")
			{ifmeth[ifno][n]=4;}
			}
			if (cn->getNodeName()==L"PeriodicData")
			{
				for (k=0;k<3;k++)
				{if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==L"StartTime")
				{cstime[ifno][n]=convertt(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeValue());
				}
				if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==L"EndTime")
				{cetime[ifno][n]=convertt(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeValue());
				}
				if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==L"Interval")
				{
					// a=cn->ChildNodes[0]et->ChildNodes[k]->ChildNodes[0]->InnerText->Substring(1,cn->ChildNodes[0]->ChildNodes[k]->InnerText->Length-2);
                    xstring wstr(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeValue());
					citime[ifno][n]=convertsp(wstr.substr(1,wstr.length()-2).c_str());
				}
				}
			}
			if (cn->getNodeName()==L"GridData")
			{if (cn->getChildNodes()->item(0)->getNodeName()==L"GridFormNone")
			{dims[ifno][n,0]=1;
			dims[ifno][n,1]=1;
			dims[ifno][n,2]=1;
			nodims[ifno][n]=0;
			}
			}
			if (cn->getNodeName()==L"DataDirection")
			{if (cn->getNodeValue()==L"Input")
			{ifdir[ifno][n]=1;}
			if (cn->getNodeValue()==L"Output")
			{ifdir[ifno][n]=2;}
			if (cn->getNodeValue()==L"Bidirectional")
			{ifdir[ifno][n]=3;}
			}
			if (cn->getNodeName()==L"Threading")
			{if (cn->getNodeValue()==L"Unthreaded")
			{thrd[ifno][n]=0;}
			if (cn->getNodeValue()==L"Signalling")
			{thrd[ifno][n]=1;}
			if (cn->getNodeValue()==L"Blockable")
			{thrd[ifno][n]=2;}
			if (cn->getNodeValue()==L"Synchronous")
			{thrd[ifno][n]=3;
			}
			}
			if (cn->getNodeName()==L"DataCollection")
			{
			orgname[ifno][n]=*(new vector<wchar_t *>(cn->getChildNodes()->getLength())); 
			orgsname[ifno][n]=*(new vector<const wchar_t *>(cn->getChildNodes()->getLength())); 
			orgssymb[ifno][n]=*(new vector<const wchar_t *>(cn->getChildNodes()->getLength())); 
			orgsdes[ifno][n]=*(new vector<const wchar_t *>(cn->getChildNodes()->getLength())); 
			orgsconst[ifno][n]=*(new vector<const wchar_t *>(cn->getChildNodes()->getLength()));  
			orgunits[ifno][n]=*(new vector<const wchar_t *>(cn->getChildNodes()->getLength())); 
			orgtype[ifno][n]=*(new vector<int>(cn->getChildNodes()->getLength())); 
			orgdir[ifno][n]=*(new vector<int>(cn->getChildNodes()->getLength())); 
				for (i=0;i<cn->getChildNodes()->getLength();i++)
			{parsedataitem(ifno,n,i,cn->getChildNodes()->item(i));
			}
			}
		}
	}


return(1); //set to success or not
}

bool Coupler::GetSupplement(xstring varname,vector<double >svalues,int modno,int intno,int novalues)
{
	int n;
	unsigned int m,varno;
	bool found=false;
	for (n=0;n<supvarnovariables[modno][intno];n++)
	{
		if (supvarnames[modno][intno][n]==varname)
		{
			found=true;
			varno=n;
			break;
		}
	}
	if (found)
	{
		
		for (n=1;n<novalues;n++)
		{
			svalues[n]=supvardefault[modno][intno][varno];
			for (m=0;m<supvarnovalues[modno][intno][varno];m++)
			{
				if (supvarrefs[modno][intno][varno][m]==n)
				{
					svalues[n]=supvarvals[modno][intno][varno][m];
					break;
				}
			}
		}
		return(true);
	}
	else
	{
return(false);
	}

}

int Coupler::parsedataitem(int fno,int intf,int it,DOMNode *xl)
{
	unsigned int n,m;
	DOMNode *orgnode;
	const wchar_t *tempname;
	for (n=0;n<xl->getChildNodes()->getLength();n++)
	{
		if (xl->getChildNodes()->item(n)->getNodeName()==L"Name")
		{

			tempname=xl->getChildNodes()->item(n)->getNodeValue();
			orgname[fno][intf][it]=new wchar_t[wcslen(tempname)+1];
			wcscpy_s(orgname[fno][intf][it],wcslen(orgname[fno][intf][it]),tempname);
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==L"Flux")
		{
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"State")
			{
				orgdir[fno][intf][it]=0;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"Predation")
			{
				orgdir[fno][intf][it]=1;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"GrossPrimaryProduction")
			{
				orgdir[fno][intf][it]=2;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"Respiration")
			{
				orgdir[fno][intf][it]=3;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"Excretion")
			{
				orgdir[fno][intf][it]=4;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"Exudation")
			{
				orgdir[fno][intf][it]=5;
			}
			if (xl->getChildNodes()->item(n)->getNodeValue()==L"Uptake")
			{
				orgdir[fno][intf][it]=6;
			}
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==L"Units")
		{
			orgunits[fno][intf][it]=xl->getChildNodes()->item(n)->getNodeValue();
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==L"DataItem")
		{
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Nutrient")
			{
				orgtype[fno][intf][it]=0;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Pytoplankton")
			{
				orgtype[fno][intf][it]=1;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Zooplankton")
			{
				orgtype[fno][intf][it]=2;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Detritus")
			{
				orgtype[fno][intf][it]=3;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Consumer")
			{
				orgtype[fno][intf][it]=4;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"StateVariable")
			{
				orgtype[fno][intf][it]=5;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==L"Other")
			{
				orgtype[fno][intf][it]=6;
			}
			orgsconst[fno][intf][it]=L"U";
			for (m=0;m<xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getChildNodes()->getLength();m++)
			{
				orgnode=xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getChildNodes()->item(m);
				if (orgnode->getNodeName()==L"Name")
				{
					orgsname[fno][intf][it]=orgnode->getNodeValue();
				}
				if (orgnode->getNodeName()==L"Symbol")
				{
					orgssymb[fno][intf][it]=orgnode->getNodeValue();
				}
				if (orgnode->getNodeName()==L"Description")
				{
					orgsdes[fno][intf][it]=orgnode->getNodeValue();
				}
				if (orgnode->getNodeName()==L"Constituent")
				{
					orgsconst[fno][intf][it]=orgnode->getNodeValue();
				}
			}
		}
	}


	return(1);
}

#ifdef __cplusplus_cli
#ifdef _Has_GDI
void Coupler::setdelegate(Object ^cont,textedelegate ^td,bool isconsole)
{
	if (isconsole)
	{
	contd=cont;
	//ctextd=td;
	}
	else
	{
	errord=cont;
//	etextd=td;
	}
}
#endif
#endif

int Coupler::GetVariableValues(list<const wchar_t *> &vn, list<const wchar_t *> &vv)
{
	vn=varnames;
	vv=valuenames;
		return(varnames.size());
}

int Coupler::GetVariableValues(list<xstring> vn, list<xstring> vv)
{
	list<const wchar_t *>::iterator varnamesi=varnames.begin();
	list<const wchar_t *>::iterator varvaluesi=valuenames.begin();
	while (varnamesi!=varnames.end())
	{
		vn.push_back(xstring(*varnamesi));
		varnamesi++;
	}
	while (varvaluesi!=valuenames.end())
	{
		vv.push_back(xstring(*varvaluesi));
	}
		return(varnames.size());
}

int Coupler::CreateDocument()
{
	return(0);
}

int Coupler::SaveDocument()
{
	return(0);
}

int Coupler::LoadDocument()
{
	return(0);
}

int Coupler::LoadDictionary(string dictname)
{
	unsigned int n,m;
	int contoff,hasmult;
	DOMNodeList *NL,*NLL;
	XercesDOMParser *parser;
	xercesc::DOMDocument *dictionary;
	
	try {
	parser=new XercesDOMParser;
	parser->parse(dictname.c_str());
	dictionary=parser->getDocument();
	NL=dictionary->getElementsByTagName(L"GroupSynonym");
	synonyms=*(new vector<xstring> (NL->getLength()));
	synonymvals=*(new vector<xstring> (NL->getLength()));
	synonymcontext=*(new vector<DictContext *>(NL->getLength()));
	for(n=0;n<NL->getLength();n++)
	{
		synonyms[n]=NL->item(n)->getChildNodes()->item(0)->getNodeName();
		synonymvals[n]=NL->item(n)->getChildNodes()->item(0)->getNodeValue();
		synonymcontext[n]=new DictContext();
		if (NL->item(n)->getChildNodes()->getLength()>2)
		{
		if (NL->item(n)->getChildNodes()->item(2)->getNodeName()==L"Context")
		{
		synonymcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(2)->getChildNodes());
		}
		}
	}
	NL=dictionary->getElementsByTagName(L"UnitConversion");
	convertunits=*(new vector<xstring> (NL->getLength()));
	convertto=*(new vector<xstring> (NL->getLength()));
	convertratio=*(new vector<double> (NL->getLength()));
	convertmuldiv=*(new vector<xstring> (NL->getLength()));
	convertmultdivp=*(new vector<double> (NL->getLength()));
	convertcontext=*(new vector<DictContext *> (NL->getLength()));
	for(n=0;n<NL->getLength();n++)
	{
		convertunits[n]=NL->item(n)->getChildNodes()->item(0)->getNodeValue();
		convertto[n]=NL->item(n)->getChildNodes()->item(1)->getNodeValue();
		convertratio[n]=convertd(NL->item(n)->getChildNodes()->item(2)->getNodeValue());
		hasmult=0;
		convertmultdivp[n]=0.0;
		if (NL->item(n)->getChildNodes()->getLength()>3)
		{
		if (NL->item(n)->getChildNodes()->item(3)->getNodeName()==L"Multiplier")
		{
        convertmuldiv[n]=NL->item(n)->getChildNodes()->item(3)->getNodeValue();
		convertmultdivp[n]=1.0;
        hasmult=1;
		}
		if (NL->item(n)->getChildNodes()->item(3)->getNodeName()==L"Divisor")
		{
        convertmuldiv[n]=NL->item(n)->getChildNodes()->item(3)->getNodeValue();
		convertmultdivp[n]=-1.0;
        hasmult=1;
		}
		}
		convertcontext[n]=new DictContext();
		if (NL->item(n)->getChildNodes()->getLength()>(3+hasmult))
		{
		if (NL->item(n)->getChildNodes()->item(3+hasmult)->getNodeName()==L"Context")
		{
		convertcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(3+hasmult)->getChildNodes());
		}
		}
	}
	NL=dictionary->getElementsByTagName(L"GroupMapping");
	mapgroups=*(new vector<xstring> (NL->getLength()));
	mapgroupsto=*(new vector<vector<xstring> > (NL->getLength()));
	mapratio=*(new vector<vector<double> > (NL->getLength()));
	mappreserveratio=*(new vector<bool> (NL->getLength()));
	mapcontext=*(new vector<DictContext *>(NL->getLength()));
	for (n=0;n<NL->getLength();n++)
	{
		mapgroups[n]=NL->item(n)->getChildNodes()->item(0)->getNodeValue();
		mappreserveratio[n]=convertb(NL->item(n)->getChildNodes()->item(1)->getNodeValue());
		mapcontext[n]=new DictContext();
		contoff=0;
		if (NL->item(n)->getChildNodes()->getLength()>2)
		{
		if (NL->item(n)->getChildNodes()->item(2)->getNodeName()==L"Context")
		{
		contoff=1;
		mapcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(2)->getChildNodes());
		}
		}
		NLL=NL->item(n)->getChildNodes();
		mapgroupsto[n]=*(new vector<xstring>(NLL->getLength()-2-contoff));
		mapratio[n]=*(new vector<double>(NLL->getLength()-2-contoff));
		for (m=2+contoff;m<NLL->getLength();m++)
		{mapgroupsto[n][m-2-contoff]=NLL->item(m)->getChildNodes()->item(0)->getNodeValue();
		mapratio[n][m-2-contoff]=convertd(NLL->item(m)->getChildNodes()->item(1)->getNodeValue());
		}

	}
	}
	catch(exception *XmlException)
	{
		return(1);
	}
		return(0);
}

//ref class Coupler ^cpxg;

//void Coupler::ctextd(std::string strin)
//{
//	;}

void Coupler::ctextd(const std::xstring strin)
{
if ((usenetwork)&&(!ps->ismaster))
{
this->outputmessage->SendLoop(strin);
}
}

void Coupler::ctextd(const std::string strin)
{
	if ((usenetwork)&&(!ps->ismaster))
	{
xstring output=*(new xstring(strin.length(),L' '));
	std::copy(strin.begin(),strin.end(),output.begin());
	this->outputmessage->SendLoop(output);
	}
}