//#include "StdAfx.h"
//#include "windows.h"
//#include <vcclr.h>
#include "CCouplerlib.h"
#include "CouplerLibEvent.h"
using namespace Couplerlib;
using namespace System;


CCouplerlib::CCouplerlib(void)
{  
	cp=new Coupler();
	pp1=gcnew Cpipelink("ConsolePipe");
	pp2=gcnew Cpipelink("ErrorPipe");
	oneway=false;
	hassethabitat=false;
}

void CCouplerlib::CleanUp()
{

	delete cp;
}


Cprotmessage::Cprotmessage(int statno, Couplerlib::maprotocols mp, Couplerlib::mastatuscodes ms, System::String ^filestr)
{
	xstring tx;
	pm=new protmessage();
	pm->stationno=statno;
	pm->pr=(enum protocols)mp;
	pm->sc=(enum statuscodes)ms;
	tx=mastrtonat(filestr);
	//tx=PtrToStringChars(filestr);
	pm->message=tx;
}

String ^Cprotmessage::getmessage()
{
	return gcnew String(pm->message.c_str());
}
int CProtocolSock::AddStation(System::String ^st)
{
	
	return(ps->AddStation(mastrtonat(st)));
}

void CProtocolSock::SndMessage(Cprotmessage ^pmi,bool newcon, int entryn)
{
	ps->SndMessage(pmi->pm,newcon,entryn);
}

void CProtocolSock::Establishcomms(int stationno, System::String ^stname)
{
	xstring tx;
	tx=mastrtonat(stname);
	ps->Establishcomms(stationno,tx);
}

bool CProtocolSock::pollevent(protocols px, int tmx, bool wait)
{
//return (false);
return(ps->pollevent[px]->WaitOne(tmx,wait));
}
void CProtocolSock::resetpollevent(protocols px)
{
	ps->pollevent[px]->Reset();
}

void CProtocolSock::Setstagestatus(Couplerlib::maprotocols a,mastatuscodes b)
{
	ps->stagestatus[(enum protocol)a]=(enum statuscodes)b;
}

enum class mastatuscodes CProtocolSock::Getstagestatus(maprotocols a)
{
	return((enum class mastatuscodes)ps->stagestatus[(enum protocol)a]);
}


#ifdef _Has_GDI
void Cpipelink::setdelegate(Control ^cont,textedelegate ^td,bool usesocket,CMessageSock  ^messock)
{
	contd=cont;
	textd=td;
	messockd=messock;
	
}
#endif






const wstring Couplerlib::mastrtonat(System::String ^stin)
{
	//wstring *ms;
	return (xstring((xwchar_t *)(void *)Marshal::StringToHGlobalUni(stin)));
}

const std::string Couplerlib::mastrtonatc(System::String ^stin)
{
	//string *ms;
    //ms=new string("    ");
	//return(*ms);
	return(std::string((char *)(void *)Marshal::StringToHGlobalAnsi(stin)));
}

void CMessageSock::setdelegate(Control ^cont,textedelegate ^td,bool isconsol)
{
	cisconsol=isconsol;
	contd=cont;
	textd=td;
}


void CMessageSock::SendLoop(wchar_t *inb)
{
	wstring *chs=new wstring(inb);
	ms->SendLoop(*chs);
}

void CMessageSock::InitiateSend(System::String ^dest)
{
	ms->InitiateSend(mastrtonat(dest));
}

void CMessageSock::ReceiveLoop()
{
    int sizerec;
	ms->sconnected=true;
	ms->sock->Bind(ms->ep);
	ms->sock->Listen(10);
	ms->rsock=ms->sock->Accept();
	string gstr;
	vector<string> gstrsegs;
	//double valuedat,value;
	while (true) 
	{
		sizerec=ms->rsock->Receive(ms->buffer,0);
		//ccstore=new string(Encoding::ASCII->GetString(buffer,0,sizerec));
		ms->ccstore=*(new wstring(ms->buffer));


	  contd->Invoke(textd,gcnew String(ms->ccstore.c_str()));

	}
	
}

List<int> ^CCouplerlib::GetIfAddress(System::Collections::Generic::List<int> ^%mlist, System::String ^iname, bool isinput,bool ispaired)
{
	int n;
	std::vector<int> alist; //=new std::vector<int>(20); //=new std::vector<int>;
	std::vector<int> *blist;
	List<int> ^rlist=gcnew List<int>;
	mlist=gcnew List<int>;
	blist=(cp->GetIfAddress(alist,mastrtonat(iname),isinput,ispaired));
    for (n=0;n<alist.size();n++)
	{
		mlist->Add((alist)[n]);
	}
	for (n=0;n<blist->size();n++)
	{
		rlist->Add((*blist)[n]);
	}
	delete blist;
	return(rlist);
}

void CCouplerlib::PutIf(int a,int b,array<const double> ^dat,int nodat)
{
	double cv;
	std::vector<double> *dvect=new std::vector<double>;
	for (int n=0;n<nodat;n++)
	{
		cv=dat[n];
		dvect->push_back(cv);
	}
		cp->PutIf(a,b,*dvect,nodat);
		dvect->clear();
		delete dvect;
}

void CCouplerlib::PutIf(int a,int b, array<array<const double>^ >^ dat,int nodat,int noelem)
{
	double cv;
	int noelements;
	std::vector<vector<double> > *dvect=new std::vector<vector<double> >;
	for (int n=0;n<nodat;n++)
	{
		std::vector<double> sv=*(new std::vector<double>);
		for (int m=0;m<cp->elements;m++)
		{
			cv=dat[n][m];
			sv.push_back(cv);
		}
		dvect->push_back(sv);
		
	}
	noelements=cp->PutIf(a,b,dvect,nodat,noelem);
	for (int n=0;n<nodat;n++)
	{
		(*dvect)[n].clear();
	}
	dvect->clear();
	delete dvect;
}

bool CCouplerlib::CheckInterface()
{
	return(cp->CheckInterface());
}
/*

int CCouplerlib::CheckInterface(array<int> ^ifs)
{
	int cv;
	std::vector<int> *dvect=new std::vector<int>;
	for (int n=0;n<ifs->Length;n++)
	{
		cv=ifs[n];
		dvect->push_back(cv);
	}
		return(cp->CheckInterface(dvect));
}
*/
void CCouplerlib::SetDimNames(array<String ^> ^dnames)
{
	std::xstring dd[3];
	for (int n=0;n<3;n++)
	{
		dd[n]=xstring((xwchar_t *)(void *)Marshal::StringToHGlobalUni(dnames[n]));
	}
	cp->SetDimNames(dd);
}



int CCouplerlib::GetIf(int a,int b,int c,int d,int e, int f,array<double> ^%dat)
{
	int nodat;
	std::vector<double> odat;
	nodat=cp->GetIf(a,b,c,d,e,f,odat);
	dat=gcnew array<double>(nodat);
	for (int n=0;n<nodat;n++)
	{
		dat[n]=odat[n];
	}
	odat.clear();
	return(nodat);
}

int CCouplerlib::GetIf(int a,int b,int c,int d,array<double> ^%dat)
{
	int nodat;
	std::vector<double> odat;
	nodat=cp->GetIf(a,b,c,d,odat);
	dat=gcnew array<double>(nodat);
	for (int n=0;n<nodat;n++)
	{
		dat[n]=odat[n];
	}
	odat.clear();
	return(nodat);
}

int CCouplerlib::GetIf(int a,int b,int c,int d,array<array<double>^> ^%dat,int noelem,bool noredim)
{
	int nodat;
	std::vector<vector<double>  > odat;
	nodat=cp->GetIf(a,b,c,d,odat,noelem,noredim);
	dat=gcnew array<array<double> ^>(nodat);
	for (int n=0;n<nodat;n++)
	{
		dat[n]=gcnew array<double>(noelem);
		for (int k=0;k<noelem;k++)
		dat[n][k]=odat[n][k];
	}
	odat.clear();
	return(nodat);
}



void CCouplerlib::EstablishTransmit(String ^stin)
{
	cp->EstablishTransmit(wstring((wchar_t *)(void *)Marshal::StringToHGlobalUni(stin)));
}

void CCouplerlib::FinishTransmit()
{
	cp->FinishTransmit();
}

CCouplerlib::CCouplerlib(bool a,bool b,bool c,bool d)
{cp =new Coupler(a,b,c,d);
oneway=true;
#ifdef _Has_GDI
    cp->cpx=this;
#endif
    pp1=gcnew Cpipelink("ConsolePipe");
	pp2=gcnew Cpipelink("ErrorPipe");

}

void CCouplerlib::SwitchCDF(bool a)
{
	cp->SwitchCDF(a);
}


bool CCouplerlib::GetInfoFromnetCDF(System::String ^name, int %novars,int %nodims, bool %isthreed)
{
	int a,b;
	bool ret,isthreedr;
	char *nmx=(char *)(void *)Marshal::StringToHGlobalAnsi(name->ToUpper());
	ret=cp->GetInfoFromnetCDF(nmx,a,b,isthreedr);
	nodims=a;
	novars=b;
	isthreed=isthreedr;
	oneway=true;
	return(ret);
}

bool CCouplerlib::OpenNetCDFOutput(System::String ^name, int nodims,int novars,int noglobatt,array<String^> ^iglobatt,array<String ^>^iglobattval, float %fv)
{   int n;
	bool ret;
	globatt=new char *[noglobatt];
	globattval=new char *[noglobatt];
	float filvalue;
	char *nmx=(char *)(void *)Marshal::StringToHGlobalAnsi(name->ToUpper());
	for (n=0;n<noglobatt;n++)
	{
		globatt[n]=new char[iglobatt[n]->Length+1];
		globatt[n]=strcpy(globatt[n],(char *)(Marshal::StringToHGlobalAnsi(iglobatt[n]).ToPointer()));
	    globattval[n]=new char[iglobattval[n]->Length+1];
		globattval[n]=strcpy(globattval[n],(char *)(Marshal::StringToHGlobalAnsi(iglobattval[n]).ToPointer()));
	}

	ret=cp->OpenNetCDFOutput(nmx,nodims,novars, noglobatt, globatt, globattval,&filvalue);
	fv=filvalue;
	return(ret);
}


bool CCouplerlib::CreateDimensions(int ndims, array<int> ^idimlen ,array<int> ^idimtype,array<array<double>^>^idims, array<int> ^idimsattribno, array<String^>^idimsname, array<array<String^>^>^idimsattribname,array<array<String ^>^>^idimsattribtext)
{
	int n,m;
	bool ret;
	int *dimlen=new int[ndims];
	int *dimtype=new int[ndims];
	double **dims=new double *[2];
	dims[0]=new double[ndims];
	dims[1]=new double[ndims];
	int *dimsattribno=new int[ndims];
	char *tnam=new char[200];
	char **dimsname=new char *[ndims];
	char ***dimsattributename=new char **[ndims];
	char ***dimsattributetext=new char **[ndims];
	for (n=0;n<ndims;n++)
	{
		dimlen[n]=idimlen[n];
		dimtype[n]=idimtype[n];
		dims[0][n]=idims[0][n];
		dims[1][n]=idims[1][n];
		dimsattribno[n]=idimsattribno[n];
		dimsname[n]=new char[idimsname[n]->Length+1];
		strcpy(dimsname[n],(char *)(Marshal::StringToHGlobalAnsi(idimsname[n]).ToPointer()));
		dimsattributename[n]=new char *[idimsattribno[n]];
		dimsattributetext[n]=new char *[idimsattribno[n]];
		for (m=0;m<idimsattribno[n];m++)
		{
			dimsattributename[n][m]=new char[idimsattribname[n][m]->Length+1];
			tnam=(char *)(Marshal::StringToHGlobalAnsi(idimsattribname[n][m]).ToPointer());
			strcpy(dimsattributename[n][m],tnam);
			dimsattributetext[n][m]=new char[idimsattribtext[n][m]->Length+1];
			tnam=(char *)(Marshal::StringToHGlobalAnsi(idimsattribtext[n][m]).ToPointer());
			strcpy(dimsattributetext[n][m],tnam);
		}
	}

	ret=cp->CreateDimensions(ndims,dimlen,dimtype,dims,dimsattribno,dimsname,dimsattributename,dimsattributetext);
	return(ret);
}

bool CCouplerlib::CreateVariables(int ngroups, array<String^>^igname, int nattrib, array<String ^> ^iattnames,array<array<String ^>^>^iattvals )
{
	int n,m;
	bool ret;
	char **gname=new char *[ngroups];
	char ** attnames=new char *[nattrib];
	char ***attvals=new char **[nattrib];
	for (n=0;n<ngroups;n++)
	{
		gname[n]=new char[igname[n]->Length+1];
		strcpy(gname[n],(char *)(Marshal::StringToHGlobalAnsi(igname[n]).ToPointer()));
	}
	for (m=0;m<nattrib;m++)
	{
		attnames[m]=new char[iattnames[m]->Length+1];
		attvals[m]=new char *[ngroups];
		strcpy(attnames[m],(char *)(Marshal::StringToHGlobalAnsi(iattnames[m]).ToPointer()));
		for (n=0;n<ngroups;n++)
		{
			attvals[m][n]=new char[iattvals[m][n]->Length+1];
			strcpy(attvals[m][n],(char *)(Marshal::StringToHGlobalAnsi(iattvals[m][n]).ToPointer()));
		}
	}
	ret=cp->CreateVariables(ngroups,gname,nattrib,attnames,attvals);
	return(ret);
}

bool CCouplerlib::StoreDimVariables()
{
	return(cp->StoreDimVariables());
}

void CCouplerlib::StoreVariables(int ngroups, array<array<double>^> ^ivalues,int arrsize,int tindex)
{
	int n,m;
	float **values;
	values=new float *[ngroups];
	for (n=0;n<ngroups;n++)
	{
		values[n]=new float [arrsize];
		for (m=0;m<arrsize;m++)
		{
			values[n][m]=ivalues[n][m];
		}
		
	}
	cp->StoreVariables(ngroups,values,tindex);
}

bool CCouplerlib::CloseNetCDF()
{
	return(cp->CloseNetCDF());
}

	int CCouplerlib::ScreennetCDFDimensions(int dimreqno, array<String ^>^dimtable,int blockno, int totblocks)
{
	int n,ret;
	char **dimnamestab=new char *[dimreqno];
	for (n=0;n<dimreqno;n++)
	{
    dimnamestab[n]=new char[dimtable[n]->Length+1];
    dimnamestab[n]=strcpy(dimnamestab[n],(char *)(Marshal::StringToHGlobalAnsi(dimtable[n]).ToPointer()));
	}
	ret=cp->ScreennetCDFDimensions(dimreqno, dimnamestab,blockno,totblocks);
	for (n=0;n<dimreqno;n++)
	{
		delete [] dimnamestab[n];
	}
	delete [] dimnamestab;
	return(ret);
}
array<String ^> ^CCouplerlib::GetnetCDFvarnames(int section)
{
	int n;
	std::vector<std::string> sl;
	sl=cp->GetnetCDFvarnames(section);
	array<String ^>^gret=gcnew array<String ^>(sl.size());
	for (n=0;n<sl.size();n++)
	{gret[n]=gcnew String(sl[n].c_str());
	}
	return(gret);
}

bool CCouplerlib::GetSpatialDims(int nopart,double %a,double %b, double %c, double %d,int %e,int %f)
{
	double aa,bb,cc,dd;
	int ff,ee;
	bool ret;
	ret=cp->GetSpatialDims(nopart,aa,bb,cc,dd,ee,ff); 
	a=aa;
	b=bb;
	c=cc;
	d=dd;
	e=ee;
	f=ff;
	return(ret);
}

void CCouplerlib::GetRedims(int %x,int %y)
{
	x=cp->dimfx;
	y=cp->dimfy;
}

bool CCouplerlib::SetCompressions(int nocomps,array<String^> ^compname, array<int> ^comptype, array<int> ^comdims)
{
	std::vector<std::string> sl;
	std::string ss;
	int *vflagsl=new int[nocomps];
	int *comtb=new int[nocomps];
	for (int n=0;n<nocomps;n++)
	{
		ss=std::string((char *)(Marshal::StringToHGlobalAnsi(compname[n]).ToPointer()));
		sl.push_back(ss);
		vflagsl[n]=comptype[n];
		comtb[n]=comdims[n];
	}
return(cp->SetCompressions(nocomps,sl,vflagsl,comtb));
}


array<String ^> ^CCouplerlib::GetnetCDFvarattributes(int section, String ^ss)
{
	int n;
	std::vector<std::string> sl;
	char *snmx=(char *)(void *)Marshal::StringToHGlobalAnsi(ss);
	sl=cp->GetnetCDFvarattributes(snmx,section);
	array<String ^>^gret=gcnew array<String ^>(sl.size());
	for (n=0;n<sl.size();n++)
	{
		gret[n]=gcnew String(sl[n].c_str());
	}
	return(gret);
}

bool CCouplerlib::GetIndex(int blockno,int idno,String ^vname)
{
char *vnmx=(char *)(void *)Marshal::StringToHGlobalAnsi(vname);
return(cp->GetIndex(blockno,idno,std::string(vnmx)));
}

double CCouplerlib::GetStartTime(int modno)
{
	double tt;
	tt=cp->GetStartTime(modno);
	return(tt);
}

double CCouplerlib::GetEndTime(int modno)
{
	return(cp->GetEndTime(modno));
}

double CCouplerlib::GetProgress(double tval,int model)
{
	return(cp->GetProgress(tval,model));
}

array<double> ^CCouplerlib::GetnetCDFvalue(double tval, int block, int varno, bool notime,int depthadj)
{
	array<double> ^ret;
		std::vector<double> *vec=cp->GetnetCDFvalue(tval,block,varno,notime,depthadj);
		ret=gcnew array<double>(vec->size());
		for (int n=0;n<vec->size();n++)
		{
			ret[n]=(*vec)[n];
		}
		vec->clear();
		delete vec;
		return(ret);
}

void CCouplerlib::citextd(std::string s)
{
	System::String ^ss=gcnew String(s.c_str());
	ctextd(ss);
	//ctextd(String(s.c_str()));
}

void CCouplerlib::citextd(std::wstring s)
{
    System::String ^ss=gcnew String(s.c_str());
	ctextd(ss);
}

void CCouplerlib::GetSupplement(String ^nm, cli::array<double,1> ^dout, int a, int b, int no)
{
	int n;
	wchar_t *nmx=(wchar_t *)(void *)Marshal::StringToHGlobalUni(nm);
	vector<double> svalues;
	for (n=0;n<no;n++)
	{
		svalues.push_back(1.0);
	}
	cp->GetSupplement(wstring(nmx),svalues,a,b,no);
	for (n=0;n<no;n++)
	{
		dout[n]=svalues[n];
	}

}

int CCouplerlib::OrgReference(int a, int b, int c)
{
return(	cp->OrgReference(a,b,c));
}

#ifdef _Has_GDI
void CCouplerlib::setdelegate(Object ^cont,textedelegate ^td,bool isconsole)
{
	if (isconsole)
	{
	contd=cont;
	ctextd=td;
	}
	else
	{
	errord=cont;
    etextd=td;
	}
}
#endif

void CCouplerlib::SetStartDate(System::DateTime ^dt)
{
	cp->SetStartDate(dt->ToOADate()+2415018.5);
}

void CCouplerlib::SetEndDate(System::DateTime ^dt)
{
	cp->SetEndDate(dt->ToOADate()+2415018.5);

}

void CCouplerlib::setgrid(int %nor,array<DateTime ,2> ^tmx,array<double,2> ^vmin,array<double,2> ^vmax,array<String ^,2>^ nm, String ^finame, array<int> ^bounds,bool boundi)
{
int n,m;
int *boundx;
double **vmi,**vmx;
wchar_t ***nmx,***dd;
dd=new wchar_t**[3];
boundx=new int[4];
vmi=new double*[3];
vmx=new double*[3];
nmx=new wchar_t **[3];
for (m=0;m<3;m++)
{dd[m]=new wchar_t*[nor];
vmi[m]=new double[nor];
vmx[m]=new double[nor];
nmx[m]=new wchar_t *[nor];
for (n=0;n<nor;n++)
{
	dd[m][n]=(wchar_t *)(void *)Marshal::StringToHGlobalUni(tmx[m,n].ToString());
	vmi[m][n]=vmin[m,n];
	vmx[m][n]=vmax[m,n];
	nmx[m][n]=(wchar_t *)(void *)Marshal::StringToHGlobalUni(nm[m,n]);
}
for (n=0;n<4;n++)
{
	boundx[n]=bounds[n+1];
}
}
cp->setgrid(nor,dd,vmi,vmx,nmx,(wchar_t *)(void *)Marshal::StringToHGlobalUni(finame),boundx,boundi);
}

int CCouplerlib::getgrid(int %nor,array<DateTime ,2> ^%tmx,array<double,2> ^%vmin,array<double,2> ^%vmax,array<String ^,2>^ %nm, String ^finame, array<int> ^%bounds,bool %boundi)
{
int n,m,nori;
double **vmi,**vmx;
wchar_t ***nmx,***dd,*cults;
int *boundx;
bool boundix;
cp->getgrid(nori,dd,vmi,vmx,nmx,(wchar_t *)(void *)Marshal::StringToHGlobalUni(finame),cults,boundx,boundix);
IFormatProvider ^culture = gcnew System::Globalization::CultureInfo(gcnew String(cults), true);
vmin=gcnew array<double,2>(3,nori);
vmax=gcnew array<double,2>(3,nori);
nm=gcnew array<String ^,2>(3,nori);
tmx=gcnew array<DateTime,2>(3,nori);
bounds=gcnew array<int>(5);
for (n=0;n<4;n++)
{
	bounds[n+1]=boundx[n];
}
boundi=boundx;
for (m=0;m<3;m++)
{
	for (n=0;n<nori;n++)
	{
		vmin[m,n]=vmi[n][m];
		vmax[m,n]=vmx[n][m];
		nm[m,n]=gcnew String(nmx[n][m]);
		bool didparse=tmx[m,n].TryParse(gcnew String(dd[n][m]),tmx[m,n]); //DateTimeFormatInfo.InvariantInfo);
	}
}
nor=nori;
return(nor);
}


bool CCouplerlib::CheckRescale(double timebase, int nodataelements, array<String ^> ^names, int alblock,double curtime, array<int> ^%habitatarray, int %xdim, int %ydim)
{

	int n,m;
	bool ret;
	int nxdim,nydim;
	wchar_t **nnames=new wchar_t *[nodataelements];
	int *habitat;
	for (n=0;n<nodataelements;n++)
	{
	nnames[n]=new wchar_t[names[n]->Length+1];
	wcscpy(nnames[n],(wchar_t *)(void *)Marshal::StringToHGlobalUni(names[n]->ToString()));
	}
	ret=cp->CheckRescale(timebase,nodataelements,nnames,alblock,curtime,habitat,nxdim,nydim);
    for (n=0;n<nodataelements;n++)
	{
		delete nnames[n];
	}
	if (ret&hassethabitat)
	{
	
	xdim=nxdim;
	ydim=nydim;
habitatarray=gcnew array<int>(xdim*ydim); 
	for (n=0;n<xdim;n++)
	{
		for (m=0;m<ydim;m++)
		{
			habitatarray[n*ydim+m]=habitat[n*ydim+m];
		}
	}
	}

	delete nnames;
	return(ret);
}

bool CCouplerlib::EvalHabitat(array<array<double> ^> ^values,int nodataelements, int noelems)
{
int n,m;
int *habitat;
double **vval=new double *[nodataelements];
bool retval;
for (n=0;n<nodataelements;n++)
{
	vval[n]=new double[noelems];
	for (m=0;m<noelems;m++)
	{
		vval[n][m]=values[n][m];
	}
}
retval=cp->EvalHabitat(vval,noelems);
		for (n=0;n<nodataelements;n++)
		{
			delete vval[n];
		}
		hassethabitat=true;
		delete vval;
return retval;
}

void CCouplerlib::GetHabitat(array<float> ^%habitatarray,int nodataelements)
{
	float *habitx;
	int n;
	cp->GetHabitat(habitx,nodataelements);
     habitatarray=gcnew array<float> (nodataelements); 
		for (n=0;n<nodataelements;n++)
		{
			habitatarray[n]=habitx[n];
		}
		delete [] habitx;
}

int CCouplerlib::Getxdim()
{
	return(cp->dimfx);
}

int CCouplerlib::Getydim()
{
	return(cp->dimfy);
}

