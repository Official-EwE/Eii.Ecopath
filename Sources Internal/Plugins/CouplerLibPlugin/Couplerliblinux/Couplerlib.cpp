// This is the main DLL file.


#define allownetcdf
#include "StdAfx.h"
#include "math.h"
#include <time.h>
#include <fstream>
//#include <wchar.h>

#include "DictContext.h"
#include "ProtocolSock.h" 
#include "DataSock.h"
#include "MessageSock.h"
#include "pipelink.h"
#include "Couplerlib.h"
//#undef DOMDocument 
#include<xercesc/dom/DOM.hpp>
#include<xercesc/dom/DOMImplementation.hpp>
#ifdef __cplusplus_cli
#include "CCouplerlib.h"
#endif
using namespace Couplerlib;
using namespace xercesc;
//using namespace System::Threading;

#ifdef __cplusplus_cli
#define xstring wstring
#else
//#define xstring basic_string<XMLCh>
#endif

Coupler::Coupler()
{
icarfile=0;
usenetwork=false;
oneway=false;
rescaleallowblock=-1;
hasopenednetCDF=false;
noallocatedblocks=0;
XMLPlatformUtils::Initialize();
autorescale=false;
usecrit=false;
issliced=false;
rvset=false;
}



Coupler::Coupler(bool usesocks,bool iserver,bool isactive,bool iusenetCDF)
{   int tr,rec;
	//int nci;
    xstring *errob,*conob;
	usenetCDF=iusenetCDF;
    hasopenednetCDF=false;
	autorescale=false;
	usecrit=false;
	issliced=false;
	rescaleallowblock=-1;
	noallocatedblocks=0;
	icarfile=0;
	conob=new xstring(L"ConsolePipe");
        errob=new xstring(L"ErrorPipe");
	//pp1=new pipelink(&cont,conob);
	//pp2=new pipelink(&errt,errob);
	usenetwork=usesocks;
	oneway=false;
	rvset=false;
	if (usenetCDF)
	{
    SwitchCDF(true);
	}
	XMLPlatformUtils::Initialize();
	if (usesocks)
	{
		
        ps = new ProtocolSock(iserver, isactive);
#ifdef __cplusplus_cli
		linkglue ^lnkgl=gcnew linkglue(NULL,0,ps->serverep);
		iThread *backthread=new iThread(gcnew System::Threading::ThreadStart(lnkgl,&linkglue::Receive));
#else
		linkglue *linkg=new linkglue(NULL,none,ps->serverep); 
		iThread *backthread=new iThread(&(linkglue::Receive),NULL);
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
	rds->ReceiveOuterLoop();	

	}
	
}

Coupler::~Coupler()
{
	if (usenetCDF)
	{CleanUpnetCDF();
	}
}


void Coupler::SwitchCDF(bool iuseCDF)
{
    usenetCDF=iuseCDF;
	if (usenetCDF)
	{
		oneway=true;
		if (!hasopenednetCDF)
		{
	if (nc_create(FILE_NAME, NC_NETCDF4, &ncid))
	{hasopenednetCDF=true;
	}
		}
	}
}

void Coupler::CleanUpnetCDF()
{
int n,m;
for (n=0;n<nonetCDFvars-1;n++)
{
	
		delete [] varattname[n];
		delete [] varattval[n];
		delete [] vardims[n];
}
	delete [] vardims;
	delete [] varname;

   
	delete [] vardimt;
	delete [] vardsize;
	delete [] vardscale;
	delete [] vardoffset;
	delete [] vardxtent;
	delete [] varattname;
	delete [] varattval;
	delete [] varattnparts;
	delete [] dimname;
	delete [] dimlen;
	delete [] fillvalue;
}

bool Coupler::GetSpatialDims(int nopart, double &lgs,double &las,double &lgi, double &lai, int &lgx, int &lax)
{
	return(true);
}

bool Coupler::OpennetCDFExtension(double timex)
{
	;
	return (true);
}

char *Coupler::MakeCDFFileTable(char *iname)
{
	int n;
	std::ifstream IP;
	IP.open(iname);
	IP >> rezero;
	IP >> exfilelistno;
	txmin=new double[exfilelistno];
	txmax=new double[exfilelistno];
	netcdffile=new char *[exfilelistno];
	toffset=new double[exfilelistno];
    for (n=0;n<exfilelistno;n++)
	{
		netcdffile[n]=new char[100];
		IP >> netcdffile[n] >> toffset[n];
		//toffset[n]+=0.5;
		toffset[n]*=(60.0*60.0*24.0);
		txmin[n]=toffset[n];
		if (n>0)
		{
		txmax[n-1]=toffset[n];
		}
	}
	return(netcdffile[0]);
}

void Coupler::ReOpen(char *iname)
{
	int n,ret,novars;
	bool okstat=true;
	size_t dmlen;
	char dxname[100];
	int xnonetCDFdims;
	ret=nc_open(iname,NC_NOWRITE,&ncid);
	if (ret==0) //no error
	{
	//sumtra=0; This stops rescaling in new files!
	//ret=nc_inq_ndims(ncid,&xnonetCDFdims);
		for (n=0;n<nonetCDFdims;n++)
	{
		ret=nc_inq_dim(ncid,n,dxname,&dmlen);
		dimname[n]=std::string(dxname);
		dimlen[n]=dmlen;
		if (dimname[n]==std::string("time")) 
		{
			timedim=n;

		}
		//vardimsf[n]=0;
		}
		ret=nc_inq_nvars(ncid,&novars);
		for (n=0;n<novars;n++)
	{
		if (varname[n]!="time")
		{
    nc_set_var_chunk_cache(ncid,n,0,0,0.0);
		}
	}
	}

}

bool Coupler::OpenNetCDFOutput(char *iname,int nodims,int novars, int noglobatt, char **globatt, char **globattval, float *ncfill)
{
	int n,ok;
	const char *errc;
	sdims=nodims;
	dimid=new int[nodims];
	varid=new int[nodims+novars];
	ok=nc_create(iname, NC_CLASSIC_MODEL, &oncid);
	errc=nc_strerror(ok);
	for (n=0;n<noglobatt;n++)
	{
		ok+=nc_put_att_text(oncid,NC_GLOBAL,globatt[n],strlen(globattval[n]),globattval[n]);
	}
	*ncfill=NC_FILL_FLOAT;
	return(ok==NC_NOERR);


}


bool Coupler::CreateDimensions(int ndims, int *dimlen ,int *dimtype,double **dims, int *dimsattribno, char **dimsname, char ***dimsattribname,char ***dimsattribtext)
{
	int n,m,ok;
	int dimnos[1];
	char *ntemp;
	sdims=ndims;
	ok=0;
	dimvector=new float *[ndims];
	for (n=0;n<ndims;n++)
	{
		sdimlen[n]=dimlen[n];
		if (dimlen[n]==-1)
		{
        ok+=nc_def_dim(oncid,dimsname[n],NC_UNLIMITED,&(dimid[n]));
		}
		else
		{
		ok+=nc_def_dim(oncid,dimsname[n],dimlen[n],&(dimid[n]));
		}
		dimnos[0]=n;
		ok+=nc_def_var(oncid,dimsname[n],dimtype[n],1,dimnos,&(varid[n]));
		if (ok!=0)
		{
			int a=1;
		}
		if (dimlen[n]>0)
		{
		dimvector[n]=new float[dimlen[n]];
		for (m=0;m<dimlen[n];m++)
		{
        dimvector[n][m]=dims[0][n]+(dims[1][n]-dims[0][n])*(double(m)/(double(dimlen[n])-1.0));
		}
		}
		else
		{
		 dimvector[n]=new float[2];
		 dimvector[n][0]=dims[0][n];
		 dimvector[n][1]=dims[1][n];
		}
		for (m=0;m<dimsattribno[n];m++)
		{
			ok+=nc_put_att_text(oncid,varid[n],dimsattribname[n][m],strlen(dimsattribtext[n][m]),dimsattribtext[n][m]);
			ntemp=dimsattribtext[n][m];
			if (ok!=0)
			{
				int a=1;
			}

		}

	}
	return(ok==NC_NOERR);
}

bool Coupler::CreateVariables(int ngroups, char **igname, int nattrib, char **iattnames,char ***iattvals)
{
	int n,m,ok;
	int dimnos[10];
	for (n=0;n<sdims;n++)
	{
	dimnos[n]=n;
	}
	ok=0;
	for (n=0;n<ngroups;n++)
	{
		ok+=nc_def_var(oncid,igname[n],NC_FLOAT,sdims,dimnos,&(varid[n+sdims]));
		for (m=0;m<nattrib;m++)
		{
			ok+=nc_put_att_text(oncid,varid[n+sdims],iattnames[m],strlen(iattvals[m][n]),iattvals[m][n]);
		}
	}
	ok+=nc_enddef(oncid);
	return(ok==NC_NOERR);
}

bool Coupler::StoreDimVariables()
{
	int ok;
	int n;
	size_t istart[1];
	size_t icount[1];
	ok=0;
	for (n=0;n<sdims;n++)
	{
		if (sdimlen[n]>0)
		{
		istart[0]=0;
		icount[0]=sdimlen[n];
		ok+=nc_put_vara_float(oncid,varid[n],istart,icount,dimvector[n]);
		}
	}
	return(ok==NC_NOERR);
}


void Coupler::StoreVariables(int ngroups,float **ivalues, int tindex)
{
	int n,ok;
	size_t *istart,*icount;
	istart=new size_t[10];
	icount=new size_t[10];
	
	istart[0]=tindex-1;
	icount[0]=1;
	int *tvalues=new int[1];
	tvalues[0]=int(double(tindex)*dimvector[0][1]+dimvector[0][0]);
	ok=nc_put_vara_int(oncid,varid[0],istart,icount,tvalues);
	for (n=1;n<sdims;n++)
	{
		istart[n]=0;
		icount[n]=sdimlen[n];
	}
	for(n=0;n<ngroups;n++)
	{
    ok+=nc_put_vara_float (oncid, varid[n+sdims], istart,icount, ivalues[n]);
	}
}

bool Coupler::CloseNetCDF()
{
	return(nc_close(oncid)==0);
}


bool Coupler::GetInfoFromnetCDF(char *iname,int &nodims, int &novars , bool &isthreed)
{
	int ret,n,m,noatts,ndimsp,sl;
	int xdims[100];
	char *token1;
	const char *errcode=new char[1000];
	char *nexttoken1=NULL;
	char dxname[100],varnamet[100],varattnamet[100];
	nc_type atype;
	sl=strlen(iname);
	token1=new char[sl+2];
    strcpy_s(token1,sl+1,iname);
	token1=strtok_s(token1,".",&nexttoken1);
	token1=strtok_s(NULL,".",&nexttoken1);
	if (strcmp(token1,"NCX")==0)
	{
		iname=MakeCDFFileTable(iname);
	}
	size_t dmlen,lenp;
	icarfile=0;
	ret=nc_open(iname,NC_NOWRITE,&ncid);
	if (ret!=0)
	{
		errcode=nc_strerror(ret);
	}
	if (ret==0) //no error
	{
	sumtra=0;
	ret=nc_inq_ndims(ncid,&nonetCDFdims);
	nodims=nonetCDFdims;
	dimname=new std::string[nonetCDFdims];
	dimlen=new int[nonetCDFdims];
	vardimsf=new int[nonetCDFdims];
	vardscale=new double[nonetCDFdims];
	vardoffset=new int[nonetCDFdims];
	vardxtent=new int[nonetCDFdims];
	for (n=0;n<nonetCDFdims;n++)
	{
		ret=nc_inq_dim(ncid,n,dxname,&dmlen);
		dimname[n]=std::string(dxname);
		dimlen[n]=dmlen;
		if (dimname[n]==std::string("time")) 
		{
			timedim=n;
		}
		vardimsf[n]=0;
	}
	ret=nc_inq_nvars(ncid,&novars);
	nonetCDFvars=novars;
	varname=new std::string[novars];
	vardims=new int*[novars];
	vardimt=new int[novars];
	fillvalue=new double[novars];
	vardsize=new int[novars];
	varattname=new std::string *[novars+1];
	varattval=new std::string *[novars+1];
	varattnparts=new int[novars+1];
	ret=nc_inq_natts(ncid,&noatts);
	varattnparts[0]=noatts;
	varattname[0]=new std::string[noatts];
	varattval[0]=new std::string[noatts];
	for (n=0;n<noatts;n++)
	{
      nc_inq_attname(ncid,NC_GLOBAL,n,varattnamet);
      nc_inq_atttype(ncid,n,varattnamet,&atype);
	  nc_inq_attlen(ncid,n,varattnamet,&lenp);
	  varattname[0][n]=std::string(varattnamet);
	  if (atype==NC_CHAR)
		{
			varatttext=new char[lenp+1];
			nc_get_att_text(ncid,NC_GLOBAL,varattnamet,varatttext);
			varatttext[lenp]=0;
			varattval[0][n]=*(new std::string(varatttext));
			delete [] varatttext;
		}    
	  if (atype==0)
	  {
		  varattval[0][n]=std::string("NULL");
	  }
	}
	depfl=-1;
	for (n=0;n<novars;n++)
	{
		int fillok;
		double fillval;
		size_t tdimi[2],xdim;
		double tvars;
		ret=nc_inq_varname(ncid,n,varnamet);
		varname[n]=std::string(varnamet);
		if (varname[n]=="pdepthD")
		{
			depfl=n;
		}
		if (varname[n]=="time")
		{tmxvar=n;
		tdimi[0]=0;
		ret=nc_get_var1_double(ncid,n,tdimi,&tvars);
		if (!rezero)
		{
		//txmin[0]=tvars;
		//txmax[0]=tvars;
		}
		nc_inq_dimlen(ncid,timedim,&xdim);
		tdimi[0]=xdim-1;
		ret=nc_get_var1_double(ncid,n,tdimi,&tvars);
		
		}
		else
		{
        nc_set_var_chunk_cache(ncid,n,size_t(0),size_t(0),0.0);
		}
		ret=nc_inq_varndims(ncid,n,&ndimsp);
		vardims[n]=new int[ndimsp];
		ret=nc_inq_vardimid(ncid,n,xdims);
		ret=nc_inq_var_fill(ncid,n,&fillok,&fillval);
		if (fillok==1)
		{fillvalue[n]=fillval;}
		else
		{fillvalue[n]=-1e56;
		}
		vardsize[n]=ndimsp;
		vardimt[n]=1;
		for (m=0;m<ndimsp;m++)
		{
			vardims[n][m]=xdims[m];
			if (vardims[n][m]!=timedim)
			{vardimt[n]*=dimlen[vardims[n][m]];
			}
		}
		ret=nc_inq_varnatts(ncid,n,&noatts);
		varattname[n+1]=new std::string[noatts];
		varattval[n+1]=new std::string[noatts];
		varattnparts[n+1]=noatts;
		for (m=0;m<noatts;m++)
		{
			nc_inq_attname(ncid,n,m,varattnamet);
			varattname[n+1][m]=*(new std::string(varattnamet));
			nc_inq_atttype(ncid,n,varattnamet,&atype);
			nc_inq_attlen(ncid,n,varattnamet,&lenp);
			if (atype==NC_CHAR)
			{
				varatttext=new char[lenp+1];
				nc_get_att_text(ncid,n,varattnamet,varatttext);
				varatttext[lenp]=0;
				varattval[n+1][m]=std::string(varatttext);
				delete [] varatttext;
			}
			if (atype==0)
			{
			varattval[n+1][m]=std::string("NULL");
			}

		}
	}
	}
	else
	{
		}
	isthreed=(depfl!=-1);
	return(ret==0);
}

int Coupler::ScreennetCDFDimensions(int dimreqno, char **dimtable,int blockno, int totblocks)
{
	int m,n,i,j,k;
	char *tempstr;
	int * variqt,*varxqt;
	
	if (noallocatedblocks==0)
	{
		noallocatedblocks=totblocks;
		CDFiblocks=new int *[totblocks];
		CDFiblockcount=new int[totblocks];
		vari=new size_t*[totblocks];
		variq=new size_t*[totblocks];
	    varx=new size_t*[totblocks];
		varxq=new size_t*[totblocks];
		iscached=new bool[totblocks];
		cachesize=new int*[totblocks];
		cacheval=new int**[totblocks];
		cachesval=new int*[totblocks];
		cachesca=new double **[totblocks];
		cachenoitems=new int[totblocks];
		singlecache=new bool[totblocks];
		divcache=new double*[totblocks];
		divnoitem=new int[totblocks];
	}
	iscached[blockno]=false;
	dimallow=new int[dimreqno];
	dimdisallow=new int[dimreqno];
	variq[blockno]=new size_t[nonetCDFdims];
	varxq[blockno]=new size_t[nonetCDFdims];
	variqt=new int[nonetCDFdims];
	varxqt=new int[nonetCDFdims];
	for (n=0;n<dimreqno;n++)
	{
        dimallow[n]=-1;
		dimdisallow[n]=-1;
		for (m=0;m<nonetCDFdims;m++)
		{
		if (strcmp(dimtable[n],dimname[m].c_str())==0)
		{dimallow[n]=m;
		if (n==1)
		{
		variqt[m]=exxl;
		varxqt[m]=exxh-exxl+1;
		}
		if (n==0)
		{
			variqt[m]=exyl;
			varxqt[m]=exyh-exyl+1;
		}
		if (n==3)
		{
			variqt[m]=0;
			varxqt[m]=-1;
		}
		//break;
		}
		tempstr=new char[dimname[m].length()+2];
		tempstr[0]='!';
		tempstr[1]=0;
		tempstr=strcat(tempstr,dimname[m].c_str());
		if (strcmp(dimtable[n],tempstr)==0)
		{
		dimdisallow[n]=m;
		}
		delete [] tempstr;
		}
	}
	CDFiblocks[blockno]=new int [nonetCDFvars+3];
	i=0;
	for (m=0;m<nonetCDFvars;m++)
	{
		if (chkdims(dimreqno,m))
		{
			CDFiblocks[blockno][i]=m;
			if (i==0)
			{
				for (j=0;j<vardsize[m];j++)
				{
					for (k=0;k<nonetCDFdims;k++)
					{
					if (vardims[m][j]==k)
					{
						variq[blockno][j]=variqt[k];
						varxq[blockno][j]=varxqt[k];
					}
					}
				}
			}
			i++;
		}
	}
	CDFiblockcount[blockno]=i;
	delete [] dimallow;
	delete [] dimdisallow;
	delete [] variqt;
	delete [] varxqt;
	return(i);

}

bool Coupler::chkdims(int dimreqno,int varno)
{
	int n,m;
	bool dimok,dimiok;
	dimok=true;
	for (m=0;m<dimreqno;m++)
	{
		dimiok=false;
		for (n=0;n<vardsize[varno];n++)
	{
		

		if((dimallow[m]==vardims[varno][n])||(dimallow[m]==-1))
		{
			dimiok=dimiok|=true;
			//break;
		}
		if (dimdisallow[m]==vardims[varno][n])
		{
			dimiok=false;
			break;
		}
		}
		dimok=dimok&&dimiok;
	}
	return(dimok);
}
		






void Coupler::GetnetCDFScale(std::string dimname, double &dmin, double &dmax, double &dstep)
{
	;
}

std::vector<std::string> Coupler::GetnetCDFvarnames(int blockno)
{
	std::vector<std::string> retv=*(new std::vector<std::string>(CDFiblockcount[blockno]));
	for (int n=0;n<CDFiblockcount[blockno];n++)
	{
		retv[n]=varname[CDFiblocks[blockno][n]];
	}
	return(retv);
}

std::vector<std::string> Coupler::GetnetCDFvarattributes(char *atname,int blockno)
{
	std::vector<std::string> retv=*(new std::vector<std::string>(CDFiblockcount[blockno]));
	for (int n=0;n<CDFiblockcount[blockno];n++)
	{
		for (int m=0;m<varattnparts[CDFiblocks[blockno][n]+1];m++)
		{
		if (strcmp(atname,(varattname[CDFiblocks[blockno][n]+1][m]).c_str())==0)
		{
		retv[n]=varattval[CDFiblocks[blockno][n]+1][m];
		}
		}
	}
	return(retv);
}

bool Coupler::GetIndex(int blockno,int idno,std::string vname)
{
	bool retval;
	retval=false;
	for (int n=0;n<nonetCDFvars;n++)
	{
		if (varname[n]==vname)
		{
			CDFiblocks[blockno][idno]=n;
			break;
			retval=true;
		}
	}
	return(retval);
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
	std::copy(isearchroot.begin(),isearchroot.end(),searchroot.begin());
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
	parser->setValidationScheme(XercesDOMParser::Val_Always);
	ErrorHandler *errhandler=(ErrorHandler *)new HandlerBase();
	parser->setErrorHandler(errhandler);
	const wchar_t *xc=xmlname.c_str();
	parser->parse(xmlname.c_str());
	Specification= parser->getDocument(); 
	NL = Specification->getElementsByTagName(cxml(L"SystemVariable"));
	for (n=0;n<NL->getLength();n++)
	{
		filternodes(NL->item(n));
		DE=(DOMElement *)(NL->item(n));
		for (m=0;m<DE->getAttributes()->getLength();m++)
		{
			xstring ss;
			ss=xstring(DE->getAttributes()->item(m)->getNodeName());
			if(xstring(DE->getAttributes()->item(m)->getNodeName())==xstring(cxml(L"Variable")))
			{varnames.push_back(DE->getAttributes()->item(m)->getTextContent());}
			if(xstring(DE->getAttributes()->item(m)->getNodeName())==xstring(cxml(L"Value")))
			{valuenames.push_back(DE->getAttributes()->item(m)->getTextContent());}
		}
	}
	NL=Specification->getElementsByTagName(cxml(L"SpecificationFileList"));
	filternodes(NL->item(0));
	for (n=0;n<NL->item(0)->getChildNodes()->getLength();n++)
	{
		specfilenames->push_back(NL->item(0)->getChildNodes()->item(n)->getTextContent());
	}
	NL=Specification->getElementsByTagName(cxml(L"ModelInterface"));
	}
	catch(const XMLException &xmle)
	{
		return(1);
	}
	return(0);
}


bool Coupler::CheckInterface()
{

	unsigned int n,m;
	int retok,fretok,tretok,comtok;
	xstring strtemp;
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
	cstime=*(new vector<vector<Poco::DateTime> >(nofiles));
	cetime=*(new vector<vector<Poco::DateTime> >(nofiles));
	citime=*(new vector<vector<Poco::DateTime> >(nofiles));
	dims=*(new vector<vector<int> >(nofiles));
	dimsq=*(new vector<vector<int> >(nofiles));
	dimb=*(new vector<vector<double> >(nofiles));
	dimm=*(new vector<vector<double> >(nofiles));
	ifname=*(new vector<const XMLCh *>(nofiles));
	//doc=new vector<xercesc::DOMDocument *>;
	orgname=*(new  vector<vector<vector<xstring> > >(nofiles)); 
	orgsname=*(new vector<vector<vector<const XMLCh *> > >(nofiles));
	orgssymb=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgsdes=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgsconst=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgunits=*(new vector<vector<vector<const XMLCh *> > >(nofiles)); 
	orgtype=*(new vector<vector<vector<int> > >(nofiles));
	orgdir=*(new vector<vector<vector<int> > >(nofiles));
	curorg=*(new vector<vector<vector<int> > >(nofiles));
	masterpresratio=*(new vector<vector<vector<int> > >(nofiles));
	orgbuf=*(new vector<vector<vector<double> >  >(nofiles));
	aorgbuf=*(new vector<vector<vector<vector<double> > >  >(nofiles));
	mastercon=*(new vector<vector<vector<vector<int> > > >(nofiles));
	masterunits=*(new vector<vector<vector<vector<double> > > >(nofiles));
	masterunitspf=*(new vector<vector<vector<vector<double> > > >(nofiles));
	masterunitslink=*(new vector<vector<vector<vector<int> > > >(nofiles));
	masterratio= *(new vector<vector<vector<vector<double> > > >(nofiles));
	supvarvals=*(new vector<vector<vector<vector<double> > > >(nofiles));
	supvarrefs=*(new vector<vector<vector<vector<int> > > >(nofiles));
	supvarorgnames=*(new vector<vector<vector<vector<const xwchar_t *> > > >(nofiles));
	supvarnames=*(new vector<vector<vector<const xwchar_t *> > >(nofiles));
	supvardefault=*(new vector<vector<vector<double> > >(nofiles));
	supvarnovalues=*(new vector<vector<vector<int> > >(nofiles));
	supvarnovariables=*(new  vector<vector<unsigned int> >(nofiles)); 
	gmodels=*(new vector<int>(4));
	for (n=0;n<nofiles;n++)
	{
	try {
        //srtemp=new xstring; //unsigned short int[searchroot.length()+2+specfilenames[n].size()];
	        xstring sstemp=xstring(cxml(L"/"));
	        strtemp=searchroot+sstemp;
		//wcscpy_s(srtemp,srtemp->length,(searchroot+cxml(L"/")).c_str());
		strtemp=strtemp+xstring((*specfilenames)[n]);
		//wcscat_s(srtemp,wcslen(srtemp),(*specfilenames)[n]);
		parser->parse(strtemp.c_str());
		doc.push_back(parser->getDocument());
		//delete srtemp;
}
	catch(const XMLException &xmle)
	{
		this->ctextd(xstring(L"Could Not load xml File "));
	}
	int a=NL->getLength();
	GetModelDescriptionInterf(description[n],doc[n],n);
	curorg[n]=*(new vector<vector<int> >(NL->getLength()));
	orgbuf[n]=*(new vector<vector<double> >(NL->getLength()));
	aorgbuf[n]=*(new vector<vector<vector<double> > >(NL->getLength()));
    gtable=*(new vector<int>(NL->getLength()*4));
    gntable=*(new vector<xstring>(NL->getLength()*4));
	}
	for (n=0;n<NL->getLength();n++)
	{
	Interfacedes=NL->item(n)->getChildNodes();
	fretok=-1;
	tretok=-1;
	comtok=-1;
	checks=*(new vector<list<int> >(7)); 
	for (m=0;m<Interfacedes->getLength();m++)
	{
		if (xstring(Interfacedes->item(m)->getNodeName())==xstring(L"InterfaceName"))
		{ifname[n]=(Interfacedes->item(m)->getTextContent());}
		if (xstring(Interfacedes->item(m)->getNodeName())==xstring(L"ModelFrom"))
		{fretok=GetDirectionInterf(Interfacedes->item(m),m,0,n);
		}
		if (fretok!=-1)
		{
		if (xstring(Interfacedes->item(m)->getNodeName())==xstring(L"ModelTo"))
		{tretok=GetDirectionInterf(Interfacedes->item(m),m,1,n);
		}
		}
		if (Interfacedes->getLength()==4)
		{
			if ((fretok!=-1)&&(tretok!=-1))
			{
				if (xstring(Interfacedes->item(m)->getNodeName())==xstring(L"ModelBidir"))
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
		ctextd(xstring(L"Join Failed for Model ")+converts(gmodels[0])+ xstring(L" to ")+converts(gmodels[1]));
	}
	}
	}
	}
	if (retok==0)
	{
	ctextd(xstring(L"CheckInterfaceStatus Failed"));
	}
	else
	{
	ctextd(xstring(L"Interface OK"));
	}
	if (retok==0)
	{
		for (n=0;n<NL->getLength()*4;n++)
		{
			ctextd(converts((int)n) +xstring(L"  ")+gntable[n]);
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

	vector<double> &Coupler::GetFromBufferA(int modno,int ifno,int orgno)
	{
		double rval;
		if (usenetwork)
		{
			//TODO
		}
		else
		{
			return aorgbuf[modno][ifno][orgno];
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


int Coupler::GetIf(int modelno,int ifno, int modelxno, int ifxno,vector<double> &modeldat)
{
	unsigned int nparts,n,m,i,k,strflag;
	double orgam;
	//vector<double> unitstore;
	nparts=masterpresratio[modelno][ifno].size();
	modeldat=*(new vector<double>(nparts));
	for (n=0;n<nparts;n++)
	{
		orgam=0.0;
		//unitstore=*(new vector<double> (mastercon[modelno][ifno][n].size()));
		
		for (m=0;m<mastercon[modelno][ifno][n].size();m++)
		{
			orgam+=GetFromBuffer(modelxno,ifxno,mastercon[modelno][ifno][n][m])*masterunits[modelno][ifno][n][m]; 
		}
		
		

		modeldat[n]=orgam;
	}
	return(nparts);
}

int Coupler::GetIf(int modelno,int ifno, int modelxno, int ifxno,vector<vector<double> >&modeldat, int noels,bool noredim)
{
	unsigned int nparts,n,m,i,k,strflag;
	vector<double> orgvec;
	//vector<double> unitstore;
	vector<double> orgam=*(new vector<double>(noels));
	nparts=masterpresratio[modelno][ifno].size();
	modeldat=*(new vector<vector<double> > (nparts));
	for (n=0;n<nparts;n++)
	{

		for (k=0;k<noels;k++)
		{orgam[k]=0.0;
		}
		//unitstore=*(new vector<double> (mastercon[modelno][ifno][n].size(),0.0));
		for (m=0;m<mastercon[modelno][ifno][n].size();m++)
		{
			orgvec=GetFromBufferA(modelxno,ifxno,mastercon[modelno][ifno][n][m]);
			for (k=0;k<orgvec.size();k++)
			{
				try
				{
					if (k<noels)
					{
				orgam[k]+=orgvec[k]*masterunits[modelno][ifno][n][m]; 
					}
					else
					{
						int a=1;
					}
				}
				catch(exception ex1)
				{
					orgam[k]=0.0;
				}
			}
			for (k=orgvec.size();k<noels;k++) //pad extra buffer
			{
				orgam[k]=0.0;
			}
			orgvec.clear();
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

int Coupler::PutIf(int modelno,int ifno,vector<vector < double> >*modeldat,int noitems, int noelem)
{
int nxparts,n;
	nxparts=curorg[modelno][ifno].size();
	for (n=0;n<nxparts;n++)
	{
		aorgbuf[modelno][ifno][n]=(*modeldat)[curorg[modelno][ifno][n]];
	}
	return(elements);
}



vector<int> *Coupler::GetIfAddress(vector<int> &modelx,xstring intername,bool isinput,bool ispaired)
{ 
	unsigned int n,q;
	vector<int> *retlist=new vector<int>;
	//modelx.resize(gtable.size()-1); //  =new vector<int>(gtable.size());
	for (n=0;n<gtable.size();n+=2)
	{
		if ((gntable[n]==intername)&&((isinput&&(ispaired))||(!isinput&&!ispaired)))
		{
		retlist->push_back(n/4);
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
			retlist->push_back(n/4);
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
	const XMLCh *tounits;
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
				
				ratioprestype=GetRegroup(orgname[tomodel][tend][curorg[tomodel][tend][n]],linklist,linkratiolist,modelnames[tomodel],subinterf[tomodel][tend]);
				list<vector<xstring> >::iterator lki=linklist.begin();
				list<vector<double> >::iterator lkr=linkratiolist.begin();
				list<int>::iterator lkp=ratioprestype.begin();
				for (k=0;k<linklist.size();k++)//for all possible rules
				{  
					vector<xstring> ss=*lki;
				    int lsize=ss.size();	
					orgarray=*(new vector<int>(lsize));
					orgunx=*(new vector<xstring>(lsize));
					orgratio=*(new vector<double>(lsize));
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
						
						if (!islinked2) // can't connect 1 item
						{
						ctextd(xstring(L"Can't Link Organism ")); //+linklist[k][i]);
						break;}
						else
						{
							islinked=true;
						}
					}
					if (islinked)
					{break;
					}
					lki++;
					lkr++;
					lkp++;
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
			if (unitsymb==xstring(L"ERROR"))
			{
				islinked3=false;
			}
			else
			{
			//if (masterunitspf[tomodel][tend][n][k]!=0.0)
			if ((unitsymb==xstring(L"UNDEFINED"))||(unitsymb==xstring(L"SAME")))
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
					storelast=unitsymb+xstring(L" F  ")+orgname[fmodel][fend][curorg[fmodel][fend][i]];
				}
				}
			}
			}
            if (!islinked3)
	{linkfail=0;
	ctextd(L"Interface Linking Failed");
	ctextd(storelast);
	}
			}
			}

		}
	Scale(fmodel,fend, tomodel,tend);
	return(linkfail);
}


int Coupler::Scale(int fmod, int finterf,int tmod,int tinterf)
{
	int n;
	double offset,scale,dist,rescale,distf;
	elements=1;
	for (n=0;n<nodims[fmod][finterf];n++)
	{
		offset=(dimb[tmod][tinterf,n]-dimb[fmod][finterf,n])/dimm[fmod][finterf,n];
		scale=dimm[tmod][tinterf,n]/dimm[fmod][finterf,n];
		dist=dimm[tmod][tinterf,n]*dims[tmod][tinterf,n];
		distf=dist/dimm[fmod][finterf,n];
		rescale=distf/double(dims[tmod][tinterf,n]);
		stindex[n]=int(offset);
		scales[n]=rescale;
		stdims[n]=dims[tmod][tinterf,n];
		elements*=dims[tmod][tinterf,n];
	}
	if (issliced)
	{
		dimfy=dimsq[fmod][tinterf,0]-dimsq[fmod][tinterf,1];
		dimfx=dimsq[fmod][tinterf,1]-dimsq[fmod][tinterf,1];
	}
	else
	{
	dimfy=dims[fmod][tinterf,0];
	dimfx=dims[fmod][tinterf,1];
	}
	dimz=dims[fmod][tinterf,2];
return(0);

}
 


double Coupler::ConvertUnits(xstring tostr,xstring &fmstr,int &fail,xstring modelname, xstring ifxname,xstring onames, xstring constnames, double &muldiv, xstring &cvname)
{
	unsigned int n;
	xstring wsmodel;// =new wchar_t[modelname.length()+2];
        xstring wifxname; //=new wchar_t[ifxname.length()+2];
	xstring wsonames; //  wchar_t[onames.length()+2]; 
	xstring wsconstnames; //wchar_t[constnames.length()+2];
	xstring wmodel; //=*(new xstring(modelname.length(),L' '));
	xstring wifx;  //=*(new xstring(ifxname.length(),L' '));
    xstring wonames=*(new xstring(onames.length(),L' '));
	xstring wconstnames=*(new xstring(constnames.length(),L' '));
	wmodel.assign(modelname);
	wifx.assign(ifxname);
	wonames.assign(onames);
	wconstnames.assign(constnames);
	wsmodel=wmodel; 
	wifxname=wifx;
	wsonames=wonames; 
	wsconstnames=wconstnames;
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
	xstring aldimname[3];
	double alminstart[3],almaxstart[3],alminsize[3],almaxsize[3];
	int alminlen[3], almaxlen[3];
	list<int> startok,endok,intok,modellinkok,gridok,comptest;
	modelfound=-1;
	DOMNode *CaseNode,*SupNode,*gridnode;
	const XMLCh *allowlang,*allowsys;
	const XMLCh *tempname,*frname;
	Poco::DateTime startf,startt,endf,endt;
	Poco::DateTime minint,maxint;
	bool hasmadelinks;
	double minver,maxver,langmin,langmax,sysmin,sysmax;
	ar=isfrom;
	hasmadelinks=false;
	filternodes(dirnode);
	for (n=0;n<dirnode->getChildNodes()->getLength();n++)
	{
		filternodes(dirnode->getChildNodes()->item(n));
		tempname=dirnode->getChildNodes()->item(n)->getNodeName();
		if(dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"Models"))
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				filternodes(dirnode->getChildNodes()->item(m));
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==xstring(L"ModelName"))
					{
						frname=CaseNode->getTextContent();
						
					}
					if (CaseNode->getNodeName()==xstring(L"MinimumVersion"))
					{
						minver=convertd(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"MaximumVersion"))
					{
						maxver=convertd(CaseNode->getTextContent());
					}

				}
				modelfound=CheckTextName(xstring(frname),m,modelnames,xstring(L"Model Name"));
				if (modelfound>=0)
				{
				modelfound=CheckTextVersion(minver,maxver,modelfound,modelversion,xstring(L"Model Version No."));
				}
				gmodels[ar]=modelfound;
			}

		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"Implementations"))
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{ 
				filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m));
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==xstring(L"AllowedLanguage"))
					{
						allowlang=CaseNode->getTextContent();
					}
					if (CaseNode->getNodeName()==xstring(L"AllowedSystem"))
					{
						allowsys=CaseNode->getTextContent();
					}
					if (CaseNode->getNodeName()==xstring(L"LanguageMinimumVersion"))
					{
						langmin=convertd(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"LanguageMaximumVersion"))
					{
						langmax=convertd(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"SystemMinimumVersion"))
					{
						sysmin=convertd(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"SystemMaximumVersion"))
					{
						sysmax=convertd(CaseNode->getTextContent());
					}
				}
				langfound=CheckTextName(xstring(allowlang),m,languagenames,xstring(L"Language"));
				if (langfound==modelfound)
				{
					langfound=CheckTextVersion(langmin,langmax,modelfound,languageversion,xstring(L"Language Version"));
				}
				sysfound=CheckTextName(xstring(allowsys),m,systemnames,xstring(L"System"));
				if (sysfound==modelfound)
				{
					sysfound=CheckTextVersion(sysmin,sysmax,m,systemversion,xstring(L"System Version"));
				}
			}
		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"SupplementList"))
		{
			int varcount=supvarnovariables[modelfound][ifno]=dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();
			supvarvals[modelfound][ifno]=*(new vector<vector<double> >(varcount));
			supvarrefs[modelfound][ifno]=*(new vector<vector<int> >(varcount));
			supvarorgnames[modelfound][ifno]=*(new vector<vector<const xwchar_t *> >(varcount));
			supvarnames[modelfound][ifno]=*(new vector<const xwchar_t*>(varcount));
			supvardefault[modelfound][ifno]=*(new vector<double> (varcount));
			supvarnovalues[modelfound][ifno]=*(new vector<int>(varcount));
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m));
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getNodeName()==xstring(L"Variable"))
					for (k=0;k<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();k++)
					{
						SupNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
						if (SupNode->getNodeName()==xstring(L"Default"))
						{
							supvardefault[modelfound][ifno][m]=convertd(SupNode->getTextContent());
							
						}
						if (SupNode->getNodeName()==xstring(L"Name"))
						{
							supvarnames[modelfound][ifno][m]=SupNode->getTextContent();
						}
						if (SupNode->getNodeName()==xstring(L"ValueList"))
						{
							filternodes(SupNode);
							supvarnovalues[modelfound][ifno][m]=SupNode->getChildNodes()->getLength();
							supvarvals[modelfound][ifno][m]=*(new vector<double>(SupNode->getChildNodes()->getLength()));
							supvarorgnames[modelfound][ifno][m]=*(new vector<const xwchar_t *>(SupNode->getChildNodes()->getLength()));
							for (i=0;i<SupNode->getChildNodes()->getLength();i++)
							{
								filternodes(SupNode->getChildNodes()->item(i));
								if (SupNode->getChildNodes()->item(i)->getNodeName()==xstring(L"ValueItem"))
								{
									for (j=0;j<SupNode->getChildNodes()->item(i)->getChildNodes()->getLength();j++)
									{
										if (SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeName()==xstring(L"Case"))
										{
										 supvarorgnames[modelfound][ifno][m][i]=SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getTextContent();
										}
										if (SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getNodeName()==xstring(L"Value"))
										{
											supvarvals[modelfound][ifno][m][i]=convertd(SupNode->getChildNodes()->item(i)->getChildNodes()->item(j)->getTextContent());
										}
									}
								}
							}
						}
					}
			}

		}
		if(dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"Timings"))
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m));
				noallowed=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();
				for (k=0;k<noallowed;k++)
				{
					CaseNode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (CaseNode->getNodeName()==xstring(L"MustStartAfter"))
					{
						startf=convertt(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"MustStartBefore"))
					{
						startt=convertt(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"MustEndAfter"))
					{
						endf=convertt(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"MustEndBefore"))
					{
						endt=convertt(CaseNode->getTextContent());
					}
					if (CaseNode->getNodeName()==xstring(L"MiniumInterval"))
					{
						//filternodes(CaseNode);
						minint=convertti(CaseNode->getTextContent()); //.substr(1,xstring(CaseNode->getChildNodes()->item(0)->getTextContent()).length()-2))); //,0,0,0));
					}
					if (CaseNode->getNodeName()==xstring(L"MaximumInterval"))
					{   
						//filternodes(CaseNode);
						maxint=convertti(CaseNode->getTextContent()); //.substr(1,xstring(CaseNode->getChildNodes()->item(0)->getTextContent()).length()-2)));
					}

				}
				checks[0]=startok=CheckDate(startf,startt,m,cstime[modelfound],xstring(L"Start Time"));
				checks[1]=endok=CheckDate(endf,endt,m,cetime[modelfound],xstring(L"End Time"));
				checks[2]=intok=CheckDate(minint,maxint,m,citime[modelfound],xstring(L"Time Interval"));
			}
		}
		if (dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"Grids"))
		{
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m));
				gridtype=-1;
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(0)->getTextContent()==xstring(L"GridFormNone"))
				{
					gridtype=0;
				}
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(0)->getTextContent()==xstring(L"GridFormRaster1D"))

				{
					gridtype=1;
				}
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(0)->getTextContent()==xstring(L"GridFormRaster2D"))

				{
					gridtype=2;
				}
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(0)->getTextContent()==xstring(L"GridFormRaster3D"))

				{
					gridtype=3;
				}
				for (k=1;k<gridtype+1;k++)
				{

					gridnode=dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(k);
					if (gridnode->getNodeName()==xstring(L"AllowedDimension"))
					{
						filternodes(gridnode);
						for (i=0;i<gridnode->getChildNodes()->getLength();i++)
						{
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedDimName"))
							{
								aldimname[k-1]=*(new xstring(gridnode->getChildNodes()->item(i)->getTextContent()));
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMinimumStart"))
							{
								alminstart[k-1]=convertd(gridnode->getChildNodes()->item(i)->getTextContent());
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMaximumStart"))
							{
								almaxstart[k-1]=convertd(gridnode->getChildNodes()->item(i)->getTextContent());
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMinimumSize"))
							{
								alminsize[k-1]=convertd(gridnode->getChildNodes()->item(i)->getTextContent());
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMaximumSize"))
							{
								almaxsize[k-1]=convertd(gridnode->getChildNodes()->item(i)->getTextContent());
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMinimumLength"))
							{
								alminlen[k-1]=converti(gridnode->getChildNodes()->item(i)->getTextContent());
							}
							if (gridnode->getChildNodes()->item(i)->getNodeName()==xstring(L"AllowedMaximumLength"))
							{
								almaxlen[k-1]=converti(gridnode->getChildNodes()->item(i)->getTextContent());
							}
						}
					}
				}
			int gridoki[4];
			gridoki[0]=CheckNumAllowed(gridtype,m,nodims[modelfound],xstring(L"Grid Dimensions"));
			for (k=0;k<gridtype;k++)
			{
				gridoki[k+1]=CheckDim(modelfound,aldimname,k,m,alminstart,almaxstart,alminsize,almaxsize,alminlen,almaxlen,xstring(L"Dimension Specification"));
			}
			
			gridok=*new(std::list<int>);
			int chkok=-1;
			for (k=1;k<gridtype+1;k++)
			{
				if ((gridoki[k]==gridoki[0])&&(gridoki[k]!=-1))
				{
					chkok=gridoki[0];
				}
			}
			if (chkok!=-1)
			{
            gridok.push_back(chkok);
			checks[3]=gridok;
			}
			}
		}

		if(dirnode->getChildNodes()->item(n)->getNodeName()==xstring(L"DataList"))
		{
			dataflux=*(new list<int>);
			datalist=*(new list<const xwchar_t *>);
			dataconst=*(new list<const xwchar_t *>);
			for (m=0;m<dirnode->getChildNodes()->item(n)->getChildNodes()->getLength();m++)
			{
				filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m));
				for (i=0;i<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->getLength();i++)
				{
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeName()==xstring(L"DataItem"))
				{   icflag=0;
				    filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i));
					filternodes(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0));
					for (k=0;k<dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->getLength();k++)
				{
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==xstring(L"Name"))
					{datalist.push_back(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent());
					}
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==xstring(L"Constituent"))
					{dataconst.push_back(dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent());
					icflag=1;
					}
					}
					if (icflag==0)
					{
						dataconst.push_back(cxml(L"U"));
				}

				}
				if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getNodeName()==xstring(xstring(L"Flux")))
				{
					if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"State"))
			{
				dataflux.push_back(0);
			}

			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"Predation"))
			{
				dataflux.push_back(1);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"GrossPrimaryProduction"))
			{
				dataflux.push_back(2);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"Respiration"))
			{
				dataflux.push_back(3);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"Excretion"))
			{
				dataflux.push_back(4);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"Exudation"))
			{
				dataflux.push_back(5);
			}
			if (dirnode->getChildNodes()->item(n)->getChildNodes()->item(m)->getChildNodes()->item(i)->getTextContent()==xstring(L"Uptake"))
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
	ctextd("Could Not establish links");
	}
}



list<int> Coupler::PermittedInterfaces(vector<list<int> > checklist,int nochecks)
{
unsigned int n,m;
int checkok,chin;
maxcheck=0;
list<int> retar=*(new list<int>);
list<int>::iterator chki;
chki=checklist[0].begin();
for (n=0;n<checklist[0].size();n++)
{
	
if(*chki>(int)maxcheck)
{maxcheck=*chki;
}
chki++;
}
for (n=0;n<=maxcheck;n++)
{
checkok=1;
chin=0;
for (m=0;m<(unsigned int)nochecks;m++)
{
    chki=checklist[m].begin();
	while  (chki!=checklist[m].end())
	{ 
	if (*chki==n)
	{   chin=1;
	    break;
	}
	else
	{
		chin=0;
	}
	chki++;
	}
	checkok*=chin;
}
if (checkok==1)
{
retar.push_back(n);
}
}
return(retar);
}

list<int> Coupler::LinkSupp(vector<const xwchar_t *> &iorgnames,list<const xwchar_t *> &iorgconstit, int nno,int ifno,int varno,int noitems)
{ 
	int m,l;
	unsigned int k;
	bool allink;
	vector<int> orgtranstable;
	const xwchar_t *inx,*outx;
	list<int> retar=*(new list<int>);
	supvarrefs[nno][ifno][varno]=*(new vector<int>(noitems));
	orgtranstable=Getsynonyms(modelnames[nno],subinterf[nno][ifno],orgname[nno][ifno],orgsconst[nno][ifno]);
	for (m=0;m<noitems;m++)
		{ allink=false;
		for (l=0;l<2;l++)
		{
			for (k=0;k<orgname[nno][ifno].size();k++)
			{
				inx=orgname[nno][ifno][k].c_str();
				outx=iorgnames[m];
				if (wcscmp(inx,outx)==0)
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
		ctextd(xstring(L"Could Not Find Organism ")+ xstring(outx));
		//System::Console::WriteLine("Could Not Find Organism " + outx);
		}

	return(retar);

}


list<int> Coupler::LinkData(list<const xwchar_t *> iorgnames,list<const xwchar_t *> iorgconstit, list<int> iorgflux, int nno,int ifno)
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
	aorgbuf[nno][ifno]=*(new vector<vector<double> >(iorgnames.size()));
	//for (n=0;n<this->orgname.size();n++)
	{
		
		orgtranstable=Getsynonyms(modelnames[nno],subinterf[nno][ifno],orgname[nno][ifno],orgsconst[nno][ifno]);
		list<const xwchar_t *>::iterator inmit=iorgnames.begin();
		list<const xwchar_t *>::iterator iconit=iorgconstit.begin();
		for (m=0;m<iorgnames.size();m++)
		{ allink=false;
		for (l=0;l<2;l++)
		{
			for (k=0;k<orgname[nno][ifno].size();k++)
			{
				inx=xstring(orgname[tx][ifno][k]);
				outx=xstring(*inmit);
				if (std::wstring(inx)==outx)
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
		ctextd(xstring(L"Could Not Find Organism ")+xstring(outx));
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


list<int> Coupler::GetRegroup(xstring istring,list<vector<xstring> > &globstring,list<vector<double> > &globvals,xstring modelname,xstring ifxname)
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


vector<int> Coupler::Getsynonyms(xstring modelname, xstring ifxname, vector<xstring> &onames, vector<const xwchar_t *> constnames)
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
				onames[n]=synonyms[m];
				//wcscpy_s(onames[n],wcslen(onames[n]),synonyms[m].c_str());
			
			//retr[offsetptr]=n;
			//offsetptr++;
			break;
			}
		}
	}
	return(retr);
}



int Coupler::CheckTextName(xstring frname,int index,vector<xstring >nametables,xstring message)
{
	int comp=-1;

	for (unsigned int n=0;n<nametables.size();n++)
	{if (nametables[n]==frname)
	{comp=n;
	break;
    ctextd(message+frname+L" To "+nametables[n]);
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
	ctextd(message+frname+L" To "+nametables[n]);
	//System::Console::WriteLine(message+frname+" To "+nametables[n]);
	}
	}
	
	return(retar);
}

 int Coupler::CheckNumAllowed(int numin,int index,vector<int> valuearray,xstring message)
{
	int n;
	list<int> retar=*(new list<int>);
	for (int n=0;n<valuearray.size();n++)
	{
		if (valuearray[n]==numin)
		{
			retar.push_back(n);
			ctextd(message+converts(numin)+L" To "+ converts(valuearray[n]));
			return(n);
			break;
			//System::Console::WriteLine(message+Convert::ToString(numin)+" To "+ Convert::ToString(valuearray[n]));
		}
	}
	return(-1);
}

int Coupler::CheckDim(int modno,xstring iname[3],int dimno,int ifno,double minb[],double maxb[],double minm[],double maxm[],int minl[],int maxl[],xstring message)
{
    int retar=-1;
	for (unsigned int n=0;n<3;n++)
	{
		if (ddnames[n]==iname[dimno])
		{
			if ((dimb[modno][ifno,dimno]>=minb[n])&&(dimb[modno][ifno,dimno]<=maxb[n]))
			{
				if((dimm[modno][ifno,dimno]>=minm[n])&&(dimm[modno][ifno,dimno]<=maxm[n]))
				{
					if ((dims[modno][ifno,dimno]>=minl[n])&&(dims[modno][ifno,dimno]<=maxl[n]))
					{
						retar=ifno;
					}
				}
			}
		}
	}
	return(retar);
}



int Coupler::CheckTextVersion(double minval,double maxval,int index,vector<double> versions,xstring message)
{
	ctextd(message+converts(versions[index]) +L" Between "+ converts(minval)+ L" and " +converts(maxval));
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



list<int> Coupler::CheckDate(Poco::DateTime minval,Poco::DateTime maxval,int index,vector<Poco::DateTime> times,xstring message)
{
unsigned int n;
list<int> retar=*(new list<int>);
Poco::Timespan dtx,dty;
for (n=0;n<times.size();n++)
{
	dtx=times[index]-minval;
	dty=maxval-times[index];
if ((dtx>=0.0)&&(dty>=0.0))
{
	ctextd(message+converts(times[index].julianDay())+ L" Between "+ converts(minval.julianDay())+ L" and " +converts(maxval.julianDay()));
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
	ctextd(message+converts(times[index])+ L" Between "+ converts(minval)+ L" and " +converts(maxval));
	//System::Cbleonsole::WriteLine(message+Convert::ToString(*(times[index]))+ " Between "+ Convert::ToString(minval)+ " and " +Convert::ToString(maxval));
	retar.push_back(n);
}
}
return(retar);
}

void Coupler::SetDimNames(xstring *inames)
{
	for (int n=0;n<3;n++)
	{
	ddnames[n]=inames[n];
	}
}


int Coupler::GetModelDescriptionInterf(DOMNode *des,xercesc::DOMDocument *xdoc,int ifno)
				{
	unsigned int n,k,m,i,l, noparts,grdindex;
	DOMNodeList *desparts,*interfpart;
	DOMNodeList *implementparts;
	DOMNode *cn;
	des=(xdoc->getElementsByTagName(cxml(L"Description")))->item(0);
	filternodes(des);
	desparts=des->getChildNodes();
	for (m=0;m<desparts->getLength();m++)
	{
		if (desparts->item(m)->getNodeName()==xstring(L"ModelName"))
		{modelnames[ifno]=xstring(desparts->item(m)->getTextContent());
		}
		if (desparts->item(m)->getNodeName()==xstring(L"ModelVersion"))
		{modelversion[ifno]=convertd(desparts->item(m)->getTextContent());
		}
		if (desparts->item(m)->getNodeName()==xstring(L"ModelImplementation"))
		{
			filternodes(desparts->item(m));
			implementparts=desparts->item(m)->getChildNodes();
			for (k=0;k<implementparts->getLength();k++)
			{
				if (implementparts->item(k)->getNodeName()==xstring(L"SystemName"))
				{
					systemnames[ifno]=xstring(implementparts->item(k)->getTextContent());
				}
				if (implementparts->item(k)->getNodeName()==xstring(L"SystemVersion"))
				{
					systemversion[ifno]=convertd(implementparts->item(k)->getTextContent());
				}
				if (implementparts->item(k)->getNodeName()==xstring(L"ModelLanguage"))
				{
					languagenames[ifno]=xstring(implementparts->item(k)->getTextContent());
				}
				if (implementparts->item(k)->getNodeName()==xstring(L"LanguageVersion"))
				{
					languageversion[ifno]=convertd(implementparts->item(k)->getTextContent());
				}
			}
		}
	}
	interfpart=(xdoc->getElementsByTagName(cxml(L"Interface")));
	noparts=interfpart->getLength();
	subinterf[ifno]=*(new vector<xstring>(noparts));
	ifmeth[ifno]=*(new vector<int>(noparts));
	nodims[ifno]=*(new vector<int>(noparts));
	ifdir[ifno]=*(new vector<int>(noparts));
	thrd[ifno]=*(new vector<int>(noparts));
	cstime[ifno]=*(new vector<Poco::DateTime>(noparts));
	cetime[ifno]=*(new vector<Poco::DateTime>(noparts));
	citime[ifno]=*(new vector<Poco::DateTime>(noparts));
	dims[ifno]=*(new vector<int>(4)); //dim 2 =3
	dimsq[ifno]=*(new vector<int>(4));
	dimb[ifno]=*(new vector<double>(4));
	dimm[ifno]=*(new vector<double>(4));
	orgname[ifno]=*(new vector<vector<xstring> >(noparts)); 
	orgsname[ifno]=*(new vector<vector<const xwchar_t * > >(noparts));
	orgssymb[ifno]=*(new vector<vector<const xwchar_t *> >(noparts)); 
	orgsdes[ifno]=*(new vector<vector<const xwchar_t *> >(noparts)); 
	orgsconst[ifno]=*(new vector<vector<const xwchar_t *> >(noparts)); 
	orgunits[ifno]=*(new vector<vector<const xwchar_t * > >(noparts)); 
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
	supvarorgnames[ifno]=*(new vector<vector<vector<const xwchar_t *> > >(noparts));
	supvarnames[ifno]=*(new vector<vector<const xwchar_t *> >(noparts));
	supvardefault[ifno]=*(new vector<vector<double> >(noparts));
	supvarnovalues[ifno]=*(new vector<vector<int> >(noparts));
	supvarnovariables[ifno]=*(new vector<unsigned int>(noparts)); 
	for (n=0;n<noparts;n++)
	{
		filternodes(interfpart->item(n));
		for (m=0;m<interfpart->item(n)->getChildNodes()->getLength();m++)
		{
			cn=interfpart->item(n)->getChildNodes()->item(m);
			if (cn->getNodeName()==xstring(L"InterfaceName"))
			{subinterf[ifno][n]=xstring(cn->getTextContent());
			}
			if (cn->getNodeName()==xstring(L"InterfaceMethod"))
			{if (cn->getTextContent()==xstring(L"Direct"))
			{ifmeth[ifno][n]=1;}
			if (cn->getTextContent()==xstring(L"Managed"))
			{ifmeth[ifno][n]=2;}
			if (cn->getTextContent()==xstring(L"ASCII"))
			{ifmeth[ifno][n]=3;}
			if (cn->getTextContent()==xstring(L"NetCDF"))
			{ifmeth[ifno][n]=4;}
			}
			if (cn->getNodeName()==xstring(L"PeriodicData"))
			{
				filternodes(cn);
				filternodes(cn->getChildNodes()->item(0));
				for (k=0;k<3;k++)
				{if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==xstring(L"StartTime"))
				{cstime[ifno][n]=convertt(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent());
				}
				if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==xstring(L"EndTime"))
				{cetime[ifno][n]=convertt(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent());
				}
				if (cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getNodeName()==xstring(L"Interval"))
				{
					xstring aaa=cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent();

					// a=cn->ChildNodes[0]et->ChildNodes[k]->ChildNodes[0]->InnerText->Substring(1,cn->ChildNodes[0]->ChildNodes[k]->InnerText->Length-2);
					citime[ifno][n]=convertti(cn->getChildNodes()->item(0)->getChildNodes()->item(k)->getTextContent());
				}
				}
			}
			if (cn->getNodeName()==xstring(L"GridData"))
			{   dims[ifno][n,0]=1;
			dims[ifno][n,1]=1;
			dims[ifno][n,2]=1;
			dimsq[ifno][n,1]=0;
			dimsq[ifno][n,0]=0;
			dimsq[ifno][n,2]=0;
				filternodes(cn);
			    //filternodes(cn->getChildNodes()->item(0));
				xstring aa=cn->getChildNodes()->item(0)->getTextContent();
				//aa=cn->getChildNodes()->item(1)->getTextContent();
				//aa=cn->getChildNodes()->item(2)->getTextContent();
				if (cn->getChildNodes()->item(0)->getNodeName()==xstring(L"GridFormNone"))
			{
			nodims[ifno][n]=0;
			}
				else
				{
            if (cn->getChildNodes()->item(0)->getNodeName()==xstring(L"GridFormRaster2D"))
			{nodims[ifno][n]=2;
			}
			if (cn->getChildNodes()->item(0)->getNodeName()==xstring(L"GridFormRaster1D"))
			{nodims[ifno][n]=1;
			}
			if (cn->getChildNodes()->item(0)->getNodeName()==xstring(L"GridFormRaster3D"))
			{nodims[ifno][n]=3;
			}
				for (k=0;k<nodims[ifno][n];k++)
				{
					filternodes(cn->getChildNodes()->item(k+1));
					if (cn->getChildNodes()->item(k+1)->getNodeName()==xstring(ddnames[0]))
					{grdindex=0;
					dimsq[ifno][n,0]=exyh-exyl;
					}
					if (cn->getChildNodes()->item(k+1)->getNodeName()==xstring(ddnames[1]))
					{grdindex=1;
					dimsq[ifno][n,1]=exxh-exxl;
					}
					if (cn->getChildNodes()->item(k+1)->getNodeName()==xstring(ddnames[2]))
					{grdindex=2;
					}
					for (l=0;l<3;l++)
					{
						if (cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getNodeName()==xstring(L"Minimum"))
						{
							dimb[ifno][n,grdindex]=convertd(cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getTextContent());
						}
						if (cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getNodeName()==xstring(L"Interval"))
						{
							dimm[ifno][n,grdindex]=convertd(cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getTextContent());
						}
						if (cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getNodeName()==xstring(L"Length"))
						{
							dims[ifno][n,grdindex]=converti(cn->getChildNodes()->item(k+1)->getChildNodes()->item(l)->getTextContent());
						}
					}
				}
				}

			}
			if (cn->getNodeName()==xstring(L"DataDirection"))
			{if (cn->getTextContent()==xstring(L"Input"))
			{ifdir[ifno][n]=1;}
			if (cn->getTextContent()==xstring(L"Output"))
			{ifdir[ifno][n]=2;}
			if (cn->getTextContent()==xstring(L"Bidirectional"))
			{ifdir[ifno][n]=3;}
			}
			if (cn->getNodeName()==xstring(L"Threading"))
			{if (cn->getTextContent()==xstring(L"Unthreaded"))
			{thrd[ifno][n]=0;}
			if (cn->getTextContent()==xstring(L"Signalling"))
			{thrd[ifno][n]=1;}
			if (cn->getTextContent()==xstring(L"Blockable"))
			{thrd[ifno][n]=2;}
			if (cn->getTextContent()==xstring(L"Synchronous"))
			{thrd[ifno][n]=3;
			}
			}
			if (cn->getNodeName()==xstring(L"DataCollection"))
			{
			filternodes(cn);
			orgname[ifno][n]=*(new vector<xstring>(cn->getChildNodes()->getLength())); 
			orgsname[ifno][n]=*(new vector<const xwchar_t *>(cn->getChildNodes()->getLength())); 
			orgssymb[ifno][n]=*(new vector<const xwchar_t *>(cn->getChildNodes()->getLength())); 
			orgsdes[ifno][n]=*(new vector<const xwchar_t *>(cn->getChildNodes()->getLength())); 
			orgsconst[ifno][n]=*(new vector<const xwchar_t *>(cn->getChildNodes()->getLength()));  
			orgunits[ifno][n]=*(new vector<const xwchar_t *>(cn->getChildNodes()->getLength())); 
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
	xstring tempname;
	filternodes(xl);
	for (n=0;n<xl->getChildNodes()->getLength();n++)
	{
		if (xl->getChildNodes()->item(n)->getNodeName()==xstring(L"Name"))
		{

			tempname=xstring(xl->getChildNodes()->item(n)->getTextContent());
			orgname[fno][intf][it]=tempname;
			//wcscpy_s(orgname[fno][intf][it],wcslen(orgname[fno][intf][it]),tempname);
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==xstring(L"Flux"))
		{
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"State"))
			{
				orgdir[fno][intf][it]=0;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"Predation"))
			{
				orgdir[fno][intf][it]=1;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"GrossPrimaryProduction"))
			{
				orgdir[fno][intf][it]=2;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"Respiration"))
			{
				orgdir[fno][intf][it]=3;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"Excretion"))
			{
				orgdir[fno][intf][it]=4;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"Exudation"))
			{
				orgdir[fno][intf][it]=5;
			}
			if (xl->getChildNodes()->item(n)->getTextContent()==xstring(L"Uptake"))
			{
				orgdir[fno][intf][it]=6;
			}
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==xstring(L"Units"))
		{
			orgunits[fno][intf][it]=xl->getChildNodes()->item(n)->getTextContent();
		}
		if (xl->getChildNodes()->item(n)->getNodeName()==xstring(L"DataItem"))
		{
			filternodes(xl->getChildNodes()->item(n));
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Nutrient"))
			{
				orgtype[fno][intf][it]=0;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Pytoplankton"))
			{
				orgtype[fno][intf][it]=1;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Zooplankton"))
			{
				orgtype[fno][intf][it]=2;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Detritus"))
			{
				orgtype[fno][intf][it]=3;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Consumer"))
			{
				orgtype[fno][intf][it]=4;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"StateVariable"))
			{
				orgtype[fno][intf][it]=5;
			}
			if (xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getNodeName()==xstring(L"Other"))
			{
				orgtype[fno][intf][it]=6;
			}
			orgsconst[fno][intf][it]=cxml(L"U");
			for (m=0;m<xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getChildNodes()->getLength();m++)
			{
				orgnode=xl->getChildNodes()->item(n)->getChildNodes()->item(0)->getChildNodes()->item(m);
				if (orgnode->getNodeName()==xstring(L"Name"))
				{
					orgsname[fno][intf][it]=orgnode->getTextContent();
				}
				if (orgnode->getNodeName()==xstring(L"Symbol"))
				{
					orgssymb[fno][intf][it]=orgnode->getTextContent();
				}
				if (orgnode->getNodeName()==xstring(L"Description"))
				{
					orgsdes[fno][intf][it]=orgnode->getTextContent();
				}
				if (orgnode->getNodeName()==xstring(L"Constituent"))
				{
					orgsconst[fno][intf][it]=orgnode->getTextContent();
				}
			}
		}
	}


	return(1);
}



int Coupler::GetVariableValues(list<const xwchar_t *> &vn, list<const xwchar_t *> &vv)
{
	vn=varnames;
	vv=valuenames;
		return(varnames.size());
}

int Coupler::GetVariableValues(list<xstring> vn, list<xstring> vv)
{
	list<const xwchar_t *>::iterator varnamesi=varnames.begin();
	list<const xwchar_t *>::iterator varvaluesi=valuenames.begin();
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
	parser->setValidationScheme(XercesDOMParser::Val_Always);
	ErrorHandler *errhandler=(ErrorHandler *)new HandlerBase();
	parser->setErrorHandler(errhandler);
	const char *xc=dictname.c_str(); 
	xwchar_t aa[2];
	aa[0]=10;
	aa[1]=32;
	parser->ignorableWhitespace(aa,1,false);
	parser->setIncludeIgnorableWhitespace(false);
	parser->parse(dictname.c_str());
	dictionary=parser->getDocument();
	NL=dictionary->getElementsByTagName(cxml(L"GroupSynonym"));
	synonyms=*(new vector<xstring> (NL->getLength()));
	synonymvals=*(new vector<xstring> (NL->getLength()));
	synonymcontext=*(new vector<DictContext *>(NL->getLength()));
	for(n=0;n<NL->getLength();n++)
	{
		xstring xc[20];
		int xs[20];
		for (int g=0; g<NL->item(n)->getChildNodes()->getLength();g++)
		{
			xs[g]=NL->item(n)->getChildNodes()->item(g)->getNodeType();
			xc[g]=xstring(NL->item(n)->getChildNodes()->item(g)->getTextContent());
		}
		filternodes(NL->item(n));
		synonyms[n]=xstring(NL->item(n)->getChildNodes()->item(0)->getTextContent());
		synonymvals[n]=xstring(NL->item(n)->getChildNodes()->item(1)->getTextContent());
		synonymcontext[n]=new DictContext();
		if (NL->item(n)->getChildNodes()->getLength()>2)
		{
		if (NL->item(n)->getChildNodes()->item(2)->getNodeName()==xstring(L"Context"))
		{
		synonymcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(2)->getChildNodes());
		}
		}
	}
	NL=dictionary->getElementsByTagName(cxml(L"UnitConversion"));
	convertunits=*(new vector<xstring> (NL->getLength()));
	convertto=*(new vector<xstring> (NL->getLength()));
	convertratio=*(new vector<double> (NL->getLength()));
	convertmuldiv=*(new vector<xstring> (NL->getLength()));
	convertmultdivp=*(new vector<double> (NL->getLength()));
	convertcontext=*(new vector<DictContext *> (NL->getLength()));
	for(n=0;n<NL->getLength();n++)
	{
		filternodes(NL->item(n));
		convertunits[n]=xstring(NL->item(n)->getChildNodes()->item(0)->getTextContent());
		convertto[n]=xstring(NL->item(n)->getChildNodes()->item(1)->getTextContent());
		convertratio[n]=convertd(NL->item(n)->getChildNodes()->item(2)->getTextContent());
		hasmult=0;
		convertmultdivp[n]=0.0;
		if (NL->item(n)->getChildNodes()->getLength()>3)
		{
		if (NL->item(n)->getChildNodes()->item(3)->getTextContent()==xstring(L"Multiplier"))
		{
        convertmuldiv[n]=xstring(NL->item(n)->getChildNodes()->item(3)->getTextContent());
		convertmultdivp[n]=1.0;
        hasmult=1;
		}
		if (NL->item(n)->getChildNodes()->item(3)->getTextContent()==xstring(L"Divisor"))
		{
        convertmuldiv[n]=xstring(NL->item(n)->getChildNodes()->item(3)->getTextContent());
		convertmultdivp[n]=-1.0;
        hasmult=1;
		}
		}
		convertcontext[n]=new DictContext();
		if (NL->item(n)->getChildNodes()->getLength()>(3+hasmult))
		{
		if (NL->item(n)->getChildNodes()->item(3+hasmult)->getTextContent()==xstring(L"Context"))
		{
		convertcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(3+hasmult)->getChildNodes());
		}
		}
	}
	NL=dictionary->getElementsByTagName(cxml(L"GroupMapping"));
	mapgroups=*(new vector<xstring> (NL->getLength()));
	mapgroupsto=*(new vector<vector<xstring> > (NL->getLength()));
	mapratio=*(new vector<vector<double> > (NL->getLength()));
	mappreserveratio=*(new vector<bool> (NL->getLength()));
	mapcontext=*(new vector<DictContext *>(NL->getLength()));
	for (n=0;n<NL->getLength();n++)
	{
		filternodes(NL->item(n));
		mapgroups[n]=xstring(NL->item(n)->getChildNodes()->item(0)->getTextContent());
		mappreserveratio[n]=convertb(NL->item(n)->getChildNodes()->item(1)->getTextContent());
		mapcontext[n]=new DictContext();
		contoff=0;
		if (NL->item(n)->getChildNodes()->getLength()>2)
		{
		if (NL->item(n)->getChildNodes()->item(2)->getTextContent()==xstring(L"Context"))
		{
		contoff=1;
		mapcontext[n]->Assemble(NL->item(n)->getChildNodes()->item(2)->getChildNodes());
		}
		}
		NLL=NL->item(n)->getChildNodes();
		mapgroupsto[n]=*(new vector<xstring>((NLL->getLength()-2-contoff)));
		mapratio[n]=*(new vector<double>((NLL->getLength()-2-contoff)));
		for (m=2+contoff;m<NLL->getLength();m++)
		{filternodes(NLL->item(m));
			mapgroupsto[n][(m-2-contoff)]=xstring(NLL->item(m)->getChildNodes()->item(0)->getTextContent());
		mapratio[n][(m-2-contoff)]=convertd(NLL->item(m)->getChildNodes()->item(1)->getTextContent());
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

void Coupler::ctextd(wchar_t *in)
{
  ctextd(xstring(in));
}

void Coupler::ctextd(const xstring strin)
{
if ((usenetwork)&&(!ps->ismaster))
{
this->outputmessage->SendLoop(strin);
}
else
{
#ifdef _Has_GDI
cpx->citextd(strin);
#endif
}
}



void Coupler::ctextd(const std::string strin)
{
	if ((usenetwork)&&(!ps->ismaster))
	{
xstring *output=new xstring(strin.length(),L' ');
	std::copy(strin.begin(),strin.end(),output->begin());
	this->outputmessage->SendLoop(*output);
	delete output;
	}
	else
{
#ifdef _Has_GDI
cpx->citextd(strin);
#endif
}
}

void Coupler::filternodes(DOMNode*listin)
{
	int n=0;
	while(true)
	{
		if (n==listin->getChildNodes()->getLength())
		{break;
		}
		if (listin->getChildNodes()->item(n)->getNodeType()!=DOMNode::ELEMENT_NODE)
		{
			listin->removeChild(listin->getChildNodes()->item(n));
           n--;
		}
		n++;
		
	}
}
		
double Coupler::GetStartTime(int model)
{
	double tt;
	tt=cstime[model][0].julianDay();
	return(tt);
	//return(cstime[model][0].julianDay());
}

double Coupler::GetEndTime(int model)
{
	return(cetime[model][0].julianDay());
}

double Coupler::GetProgress(double tval,int model)
{
	return((tval-cstime[model][0].julianDay())/(cetime[model][0].julianDay()-cstime[model][0].julianDay()));
}


bool Coupler::SetCompressions(int nocomps, std::vector<std::string> comps, int comptype[],int comtb[])
{
	int n,m;
	bool retok;
	sumtra=0;
	retok=true;
	for (n=0;n<nocomps;n++)
	{
		for (m=0;m<nonetCDFdims;m++)
		{
		if (comps[n]==dimname[m])
		{
        vardimsf[m]=comptype[n];
		vardoffset[m]=stindex[comtb[n]];
		vardscale[m]=scales[comtb[n]];
		vardxtent[m]=stdims[comtb[n]];
		break;
		}
		retok=false;
		}
	}
	for (n=0;n<nonetCDFdims;n++)
	{
		sumtra+=vardimsf[n];
	}
	return(retok);
}

std::vector<double> *Coupler::GetnetCDFDirectValue(double tval,int block,int varno,bool usemod,bool notime,int depthadj)
{
	int n,m,ret,cvx,didx;
	int idx=CDFiblocks[block][varno];
	int tmx=GetnetCDFtime(tval);
	std::vector<double> *retval;
	bool alloc;
	double *rv=new double[vardimt[idx]];
	double cdt,cdtx;
	if ((depfl>-1)&&(depthadj>0))
	{
	didx=depfl; //CDFiblocks[block][depfl];
	if (!rvset)
	{
    rv2=new double[vardimt[idx]];
	rvset=true;
	}
	}
	if (!notime)
	{
	vari[block][timecache[block]]=tmx;
	variq[block][timecache[block]]=tmx;
	varxq[block][timecache[block]]=1;
	}
	if (usemod)
	{
		for (n=0;n<vardsize[idx];n++)
		{
			if (varxq[block][n]==-1)
			{
				varxq[block][n]=varx[block][n];
				variq[block][n]=vari[block][n];
			}
		}
		ret=nc_get_vara_double(ncid,idx,variq[block],varxq[block],rv);
		if ((depfl>-1)&&(depthadj>1)&&(idx!=didx))
		{
			ret=nc_get_vara_double(ncid,didx,variq[block],varxq[block],rv2);
		}
	}
	else
	{
	ret=nc_get_vara_double(ncid,idx,vari[block],varx[block],rv);
	if ((depfl>-1)&&(depthadj>1)&&(idx!=didx))
	{
		ret=nc_get_vara_double(ncid,didx,vari[block],varx[block],rv2);
	}
	}
	
	elements=(dimfy+1)*(dimfx+1);
	retval=new std::vector<double>(elements,0.0);
	zdimmod=1;
	if (depthadj)
	{
	zdimdiv=elements;
	}
	else
	{
		zdimdiv=0;
	}
	if (depfl>-1&&(depthadj>0)&&(idx!=didx))
	{
		for (n=0;n<elements;n++)
	    {
		(*retval)[n]=0.0;
		cdt=0.0;
		for (m=0;m<dimz;m++)
		{
		(*retval)[n]+=rv[m*zdimdiv+n*zdimmod]*rv2[m*zdimdiv+n*zdimmod];
		cdt+=rv2[m*zdimdiv+n*zdimmod];
		}
		(*retval)[n]/=cdt;
		if (((*retval)[n]>1e20)||(cdt<=0.0))
		{
			(*retval)[n]=0.0;
		}
		}
	}
	else
	{

	for (n=0;n<elements;n++)
	{
		(*retval)[n]=0.0;
		for (m=0;m<dimz;m++)
		{
		(*retval)[n]+=rv[m*zdimdiv+n*zdimmod];
		}
		(*retval)[n]/=double(dimz);
		if ((*retval)[n]>1e20)
		{
			(*retval)[n]=0.0;
		}
	}
	}
	if ((depfl>-1)&&depthadj)
	{
	//	delete [] rv2;
	}
    delete [] rv;
	return(retval);
	}




std::vector<double> *Coupler::GetnetCDFCachedValue(double tval,int block,int varno,bool notime)
{
	int n,m,ret,cvx;
	int idx=CDFiblocks[block][varno];
	int tmx=GetnetCDFtime(tval);
	std::vector<double> *retval;
	double *rv=new double[vardimt[idx]];
	if (!notime)
	{
	vari[block][timecache[block]]=tmx;
	variq[block][timecache[block]]=tmx;
	}
	ret=nc_get_vara_double(ncid,idx,vari[block],varx[block],rv);
	retval=new std::vector<double>(divnoitem[block],0.0);
	//for (n=0;n<divnoitem[block];n++)
	//{(*retval)[n]=0.0;
	//}
	if (singlecache[block])
	{
	for (n=0;n<cachenoitems[block];n++)
	{
		cvx=cachesval[block][n];
		if (cvx>=0)
		{
		(*retval)[cvx]+=rv[n];
		}
	}
	}
	else
	{
	for (n=0;n<cachenoitems[block];n++)
	{
		for (m=0;m<cachesize[block][n];m++)
		{
		   cvx=cacheval[block][n][m];
		   if (cvx>=0)
		   {
			(*retval)[cvx]+=rv[n]*cachesca[block][n][m];
		   }
		}
	}
	}
	for (n=0;n<divnoitem[block];n++)
	{
		if ((*retval)[n]>1e20)
		{
			(*retval)[n]=0.0;
		}
		else
		{
		(*retval)[n]/=divcache[block][n];
		}
	}

	delete [] rv;
	return(retval);
}





std::vector<double>  *Coupler::ExtractHabitat(std::vector<double> *dv,int blk,int varno,bool usecrt)
{
	int n,m,k,ci;
	if ((blk==rescaleallowblock)&&usecrt)
	{
		for (n=0;n<nocrit;n++)
		{
		for (m=0;m<3;m++)
		{
		if (crit[n*3+m]==varno)
		{
	for (k=0;k<divnoitem[blk];k++)
	{
	
	
			ci=cacheval[blk][k][0];
			if (ci>=0)
			{
		if (((*dv)[k]>=vmi[n][m])&&((*dv)[k]<=vmi[n][m]))
		{
        
		habitatc[n][ci]=habitatc[n][ci];
		}
		else
		{
		habitatc[n][ci]=false;
		}
			}
	}
		}
		}
		}
	}
	return(dv);
}

int *Coupler::ColCriteria(int blk)
{
	int n,k;
	for (n=0;n<divnoitem[blk];n++)
	{
		for (k=0;k<nocrit;k++)
		{
			if (habitatc[n][k])
			{
				habitat[n]=k;
				break;
			}
		}
	}
	return(habitat);
}



std::vector<double> *Coupler::GetnetCDFvalue(double tval,int block,int varno, bool notime,int depthadj)
{
	int tmx;
	double cvtp;
	if(usecrit)
	{
	return(GetnetCDFDirectValue(tval,block,varno, issliced,notime,depthadj));
	}
	if (iscached[block])
	{return(ExtractHabitat(GetnetCDFCachedValue(tval,block,varno,notime),block,varno,usecrit));
	}
	else
	{
		iscached[block]=true;
	int m,n,i,j,aa,ret,varcc,vardivx;
	int idx=CDFiblocks[block][varno];
	if (!notime)
	{
	tmx=GetnetCDFtime(tval);
	}
	std::vector<double> *retval;
	double divvdoub,fvt,tvalx,tval2;
	vari[block]=new size_t[vardsize[idx]];
	varx[block]=new size_t[vardsize[idx]];
	//variq[block]=new size_t[vardsize[idx]];
	//varxq[block]=new size_t[vardsize[idx]];
	size_t *varxx=new size_t[vardsize[idx]+1];
	size_t *varc=new size_t[vardsize[idx]];
	int *varp=new int[vardsize[idx]];
	int *dix=new int[vardsize[idx]+1];
	size_t *rem=new size_t[vardsize[idx]+1];
	//int **cval=new int*[vardsize[idx]+1];
	int cval[10][10];
	double cvmult[10][10];
	int *tdif=new int[vardsize[idx]+1];
	double *tdifb=new double[vardsize[idx]+1];
	double *tdifc=new double[vardsize[idx]+1];
	int *cvsize=new int [vardsize[idx]+1];
	int *pos=new int[vardsize[idx]+1];
	size_t *div=new size_t[vardsize[idx]+1];
	double *rv=new double[vardimt[idx]];
	cacheval[block]=new int *[vardimt[idx]];
	cachesval[block]=new int[vardimt[idx]];
	cachesize[block]=new int[vardimt[idx]];
	cachesca[block]=new double *[vardimt[idx]];
	    varcc=1;
		vardivx=1;
		for (m=0;m<this->vardsize[idx];m++)
		{
			if (vardims[idx][m]!=timedim)
			{
             vari[block][m]=0;
			 //variq[block][m]=0;
			 varx[block][m]=dimlen[vardims[idx][m]];
			 //varxq[block][m]=dimlen[vardims[idx][m]];
			 if (vardimsf[vardims[idx][m]]>0)
			 {
				 if (vardimsf[vardims[idx][m]]>2)
				 {
					 varc[m]=vardxtent[vardims[idx][m]];
					 varp[m]=1;
				 }
				 else
				 {
				 varc[m]=1;
				 varp[m]=0;
				 }
				 if ((vardimsf[vardims[idx][m]]==2)||(vardimsf[vardims[idx][m]]==4))
				 {

					
					 if (vardimsf[vardims[idx][m]]==4)
					 {
						 zdimdiv=vardivx;
						 zdimmod=dimz;
					 }
					 vardivx*=dimlen[vardims[idx][m]];
				 }
			 }
			 else
			 {
				 varc[m]=dimlen[vardims[idx][m]];
				 varp[m]=1;
				 varcc*=varc[m];
			}
			}
			else
			{
				timecache[block]=m;
				vari[block][m]=tmx;
				variq[block][m]=tmx;
				varx[block][m]=1;
				varxq[block][m]=1;
				varc[m]=1;
				varp[m]=0;
			}
		}
		divvdoub=double(vardivx);
	ret=nc_get_vara_double(ncid,idx,vari[block],varx[block],rv);
	if (sumtra==0) // no compression
	{
	retval=new std::vector<double>(vardimt[idx],0.0);
	cachenoitems[block]=vardimt[idx];
	singlecache[block]=true;
	for (n=0;n<vardimt[idx];n++)
	{
		cachesval[block][n]=n;
		(*retval)[n]=rv[n];
	}
	}
	else
	{   
		
		dix[0]=1;
		for (m=0;m<this->vardsize[idx];m++)
		{
			if (vardimsf[vardims[idx][m]]>2)
			{
			dix[m+1]=dix[m]*varc[m]; //-vardoffset[vardims[idx][m]])/vardscale[vardims[idx][m]];
			}
			else
			{
				dix[m+1]=dix[m]*varc[m];
			}
		}
		varcc=dix[vardsize[idx]];
		retval=new std::vector<double>(dix[vardsize[idx]],0.0);
		cachenoitems[block]=vardimt[idx];
		singlecache[block]=true;
		for (n=0;n<dix[vardsize[idx]];n++)
		{
			(*retval)[n]=0.0;
		}
	fvt=0;
    if (varcc==1)
	{
		
		for (n=0;n<vardimt[idx];n++)
		{
			if ((rv[n]<1e20)&&(rv[n]>-1e20))
			{
			cachesval[block][n]=0;
			(*retval)[0]+=rv[n];
			}
			else
			{
			fvt+=1;
			}
		}
	}
	else
	{
	singlecache[block]=true;
    for (n=0;n<vardimt[idx];n++)
	{
	rem[0]=n;
	cvsize[0]=1;
	//cval[0]=new int[1];
	cval[0][0]=0;
	cvmult[0][0]=1.0;
	varxx[vardsize[idx]-1]=1;
	for (m=vardsize[idx]-1;m>0;m--)
	{
		varxx[m-1]=varxx[m]*varx[block][m];
	}
    for (m=0;m<this->vardsize[idx];m++)
	{
		div[m+1]=rem[m]/varxx[m];
		rem[m+1]=rem[m]%varxx[m];
		pos[m+1]=div[m+1]; 
		if (vardimsf[vardims[idx][m]]>2)
		{
		tvalx=(pos[m+1]-vardoffset[vardims[idx][m]])/vardscale[vardims[idx][m]];
		tval2=(pos[m+1]+1-vardoffset[vardims[idx][m]])/vardscale[vardims[idx][m]];
		tdif[m+1]=ceil(tval2)-floor(tvalx);
		if (vardimsf[vardims[idx][m]]==4)
		{
			tdifb[m+1]=tdifc[m+1]=0.0;
		}
		else
		{
        tdifb[m+1]=tvalx-floor(tvalx);
		tdifc[m+1]=ceil(tval2)-tval2;
		}
		if (tdif[m+1]>1)
		{singlecache[block]=false;
		}
		pos[m+1]=tvalx;
		
		if ((pos[m+1]>=0)&&(pos[m+1]+tdif[m+1]<=vardxtent[vardims[idx][m]]))
		{
		aa=cval[m][0];
		//cval[m+1]=new int[tdif[m+1]*cvsize[m]];
        for (i=0;i<tdif[m+1];i++)
		{
			cvtp=1.0;
				if (i==0)
				{cvtp-=tdifb[m+1];}
				if (i==(tdif[m+1]-1))
				{cvtp-=tdifc[m+1];}
			for (j=0;j<cvsize[m];j++)
			{
				
		cval[m+1][i*cvsize[m]+j]=cval[m][j]+dix[m]*(pos[m+1]+i)*varp[m];
		cvmult[m+1][i*cvsize[m]+j]=cvmult[m][j]*cvtp;
		if (cvmult[m+1][i*cvsize[m]+j]<0)
		{
			int a=1;
		}
			}
		}
		cvsize[m+1]=cvsize[m]*tdif[m+1];
		//delete cval[m];
		}
		else
		{  // delete [] cval[m];
		//cval[vardsize[idx]]=new int[cvsize[m]];
			for (j=0;j<cvsize[m];j++)
			{
				
				cval[vardsize[idx]][j]=-1;
				cvsize[vardsize[idx]]=cvsize[m];
				cvmult[vardsize[idx]][j]=1.0;
		}
			cvsize[m+1]=cvsize[m];
		break;
		}
		}
		else
		{
			//cval[m+1]=new int[cvsize[m]];
			for (j=0;j<cvsize[m];j++)
			{
			cval[m+1][j]=cval[m][j]+dix[m]*pos[m+1]*varp[m]*cvmult[m][j];
			cvmult[m+1][j]=cvmult[m][j];
			}
			//delete [] cval[m];
			cvsize[m+1]=cvsize[m];
		}
	}
	   cachesize[block][n]=cvsize[vardsize[idx]];
	   cacheval[block][n]=new int[cvsize[vardsize[idx]]];
	   cachesval[block][n]=cval[vardsize[idx]][0];
	   cachesca[block][n]=new double[cvsize[vardsize[idx]]];
	   for (j=0;j<cvsize[vardsize[idx]];j++)
	   {
		   aa=cval[vardsize[idx]][j];
		   cachesca[block][n][j]=cvmult[vardsize[idx]][j];
	 cacheval[block][n][j]=aa;
		if (aa>=0)
		{

    (*retval)[aa]+=rv[n]*cvmult[vardsize[idx]][j];
	
		}
	   }
	
	
	//	delete [] cval[vardsize[idx]];
	}
	}
	}
	divvdoub-=double(fvt);
	cachenoitems[block]=vardimt[idx];
	divcache[block]=new double[varcc];
	divnoitem[block]=varcc;
	for (n=0;n<varcc;n++)
	{
		if ((*retval)[n]>1e20)
		{
			(*retval)[n]=0.0;
		}
		divcache[block][n]=1.0 ; //divvdoub;
		//(*retval)[n]/=divvdoub;
	}

	
	//delete [] cval;
	delete [] cvsize;
	delete [] tdif;
	delete [] varxx;
	delete [] rv;
	delete [] varp;
	delete [] varc;
	delete [] dix;
	delete [] rem;
	delete [] pos;
	delete [] div;
	return(ExtractHabitat(retval,block,varno,usecrit));
	}
}

int Coupler::GetnetCDFtime(double tval)
{
	int timetop,timebot,timehalf;
	int n,m;
	bool updowntflag,outsidetflag;
	double tmxval,tmxmin;
	outsidetflag=true;
	size_t idw[]={0};
	while (outsidetflag==true)
	{
	timetop=dimlen[timedim]-1;
	timebot=0;
	timehalf=(timetop+timebot)/2;
    idw[0]=timebot;
	outsidetflag=false;
	nc_get_var1_double(ncid,tmxvar,idw,&tmxval);
	//if (rezero)
		//{
					tmxval=txmin[icarfile];
		//}
    if (tval<tmxval)
	{
		outsidetflag=true;
		updowntflag=false;
	}
	 idw[0]=timetop;
	nc_get_var1_double(ncid,tmxvar,idw,&tmxval);
	//if ((rezero))
		//		{
					tmxval=txmax[icarfile];
			//	}
	if (tval>tmxval)
	{
		outsidetflag=true;
        updowntflag=true;
	}
    if (outsidetflag)
	{
		for (m=0;m<2;m++)
		{
		for (n=1;n<exfilelistno;n++)
		{
			if ((m==0)&&(tval>=txmin[n])&&(tval<(txmax[n])))
			{
				ReOpen(netcdffile[n]);
				icarfile=n;
				m=2;
				break;
			}
			if (m==1)
			{
				ReOpen(netcdffile[n]);
				timetop=dimlen[timedim]-1;
				timebot=0;
				icarfile=n;
				idw[0]=timebot;
				nc_get_var1_double(ncid,tmxvar,idw,&tmxval);
				if ((rezero))
				{
					tmxmin=txmin[n];
					tmxval=txmax[n];
				}
				else
				{
					tmxmin=0;
				}
				idw[0]=timetop;
				if ((tval>=tmxmin)&&(tval<=tmxval))
				{
					break;
				}
			}
				
		}
		}
		if (n==exfilelistno)
		{
			return(-1);
		}

	}
	}
	while (timetop-timebot>1)
	{
		timehalf=(timetop+timebot)/2;
		idw[0]=timehalf;
		nc_get_var1_double(ncid,tmxvar,idw,&tmxval);
		if ((rezero)&&(icarfile>0))
				{
					tmxval+=txmin[icarfile];
				}

		if (tval<tmxval)
		{
			timetop=timehalf;
		}
		else
		{
			timebot=timehalf;
		}
	}
	return(timebot);
}

void Coupler::SetStartDate(double st)
{
	cstime=*(new std::vector<std::vector<Poco::DateTime>>(1,*(new std::vector<Poco::DateTime>(1,*(new Poco::DateTime(st))))));
}

void Coupler::SetEndDate(double st)
{
	cetime=*(new std::vector<std::vector<Poco::DateTime>>(1,*(new std::vector<Poco::DateTime>(1,*(new Poco::DateTime(st))))));
}


void Coupler::setgrid(int nor,xwchar_t ***dd,double **vmi,double **vmx,xwchar_t ***nmx,xwchar_t *fname, int *boundx, bool boundi)
{
	parser=new XercesDOMParser;
	const XMLCh *cults;
	DOMElement *g,*gd,*gdi,*gdii,*gdiii;
	DOMNode *nd;
	int n,m;
	parser->setValidationScheme(XercesDOMParser::Val_Always);
	ErrorHandler *errhandler=(ErrorHandler *)new HandlerBase();
	parser->setErrorHandler(errhandler);
	parser->parse(fname);
	try
	{
	GridSpecification= parser->getDocument(); 
	NL = GridSpecification->getElementsByTagName(cxml(L"GridData"));
	g=GridSpecification->getDocumentElement();
	if (NL->getLength()!=0)
	{
		g->removeChild(NL->item(0));
	}
	NL=GridSpecification->getElementsByTagName(cxml(L"Culture"));
	if (NL->getLength()==0)
	{
		cults=cxml(L"en-GB");
	}
	else
	{
		cults=NL->item(0)->getTextContent();
	}
	NL=GridSpecification->getElementsByTagName(cxml(L"Extents"));
	if (NL->getLength()==0)
	{
		gd=GridSpecification->createElement(cxml(L"Extents"));
	}
	else
	{ 
		gd=(DOMElement *)NL->item(0);
	}
	    exxl=boundx[0];
		exxh=boundx[1];
		exyl=boundx[2];
		exyh=boundx[3];
		isbounded=boundi;
		gd->setAttribute(cxml(L"Xlow"),converts(exxl).c_str());
		gd->setAttribute(cxml(L"Xhigh"),converts(exxh).c_str());
		gd->setAttribute(cxml(L"Ylow"),converts(exyl).c_str());
		gd->setAttribute(cxml(L"Yhigh"),converts(exyh).c_str());
		gd->setAttribute(cxml(L"Boundaries"), converts(isbounded).c_str());
		g->appendChild(gd);
   
	if (nor>0)
	{
		gd=GridSpecification->createElement(cxml(L"GridData"));
		for (n=0;n<nor-1;n++)
		{
			gdi=GridSpecification->createElement(cxml(L"Row"));
			for (m=0;m<3;m++)
			{
			gdii=GridSpecification->createElement(cxml(L"Variable"));
			gdii->setAttribute(cxml(L"Name"),nmx[m][n]);
			gdii->setAttribute(cxml(L"Date"),dd[m][n]);
			gdii->setAttribute(cxml(L"Minimum"),(converts(vmi[m][n])).c_str());
			gdii->setAttribute(cxml(L"Maximum"),(converts(vmx[m][n])).c_str());
		    gdi->appendChild(gdii);
			}
			gd->appendChild(gdi);
		}
		g->appendChild(gd);
    
	DOMImplementation    *pImplement        = NULL;
    //DOMWriter            *pSerializer    = NULL;
	DOMLSOutput *pOutput=NULL;
	DOMLSSerializer *pSerializer=NULL; 
    XMLFormatTarget        *pTarget        = NULL;
    pImplement = DOMImplementationRegistry::getDOMImplementation(L"LS");
    pSerializer = ((DOMImplementationLS*)pImplement)->createLSSerializer();
	pTarget = new LocalFileFormatTarget(fname);
    pOutput = ((DOMImplementationLS*)pImplement)->createLSOutput();
	pOutput->setByteStream(pTarget);
	pOutput->setEncoding(L"UTF-8");
    pSerializer->write(GridSpecification,pOutput);
	//std::wofstream op;
	//op.open(fname);
	//op << ss;
	//op.close();

    // Set the stream to our target.
    

    // Write the serialized output to the destination.
    //pSerializer->write(GridSpecification, pOutput);

    // Cleanup.
    pSerializer->release();
	delete pTarget;

    //pSerializer->setFeature(XMLUni::fgDOMWRTFormatPrettyPrint, true);
	}
	}
	catch(int e)
	{
		int a=e;
	}

}
		
int Coupler::getgrid(int &nor,xwchar_t *** &idd,double ** &ivmi,double **&ivmx, xwchar_t *** &inmx,xwchar_t *fname,xwchar_t *&cult, int * &boundx, bool &boundi)
{
	DOMNodeList *NLL,*NLLL;
	DOMNode *dn,*atn;
	int n,m,nlen;
	parser=new XercesDOMParser;
	boundx=new int[4];
	parser->setValidationScheme(XercesDOMParser::Val_Always);
	ErrorHandler *errhandler=(ErrorHandler *)new HandlerBase();
	parser->setErrorHandler(errhandler);
	parser->parse(fname);
	GridSpecification= parser->getDocument(); 
    NL = GridSpecification->getElementsByTagName(cxml(L"Culture"));
	//cult=new xwchar_t[6];
	if (NL->getLength()==0)
	{
		cult=cwxml(L"en-GB");
	}
	else
	{
		cult=cwxml(NL->item(0)->getTextContent());
	}
	NL=GridSpecification->getElementsByTagName(cxml(L"Extents"));
	exxl=0;
    exxh=99999;
    exyl=0;
    exyh=99999;
	isbounded=false;
	if (NL->getLength()>0)
	{
		for (n=0;n<NL->getLength();n++)
		{
			dn=NL->item(n);
			isbounded=convertb(dn->getAttributes()->getNamedItem(L"Boundaries")->getTextContent());
			exxl=converti(dn->getAttributes()->getNamedItem(L"Xlow")->getTextContent());
			exxh=converti(dn->getAttributes()->getNamedItem(L"Xhigh")->getTextContent());
			exyl=converti(dn->getAttributes()->getNamedItem(L"Ylow")->getTextContent());
			exyh=converti(dn->getAttributes()->getNamedItem(L"Yhigh")->getTextContent());
		}
	}

	boundx[0]=exxl;
	boundx[1]=exxh;
	boundx[2]=exyl;
	boundx[3]=exyh;
	boundi=isbounded;
	NL = GridSpecification->getElementsByTagName(cxml(L"GridData"));
	nor=NL->getLength();
	if (nor=0)
	{
		return(0);
	}
	else
	{
		NLL=NL->item(0)->getChildNodes();
		nor=NLL->getLength();

		dd=new xwchar_t **[nor];
			nmx=new xwchar_t **[nor];
			vmi=new double *[nor];
			vmx=new double *[nor];
		crit=new int[nor*3];
		critflag=new bool [nor*3];
		for (n=0;n<nor;n++)
		{
			dd[n]=new xwchar_t *[3];
			nmx[n]=new xwchar_t *[3];
			vmi[n]=new double [3];
			vmx[n]=new double [3];
			NLLL=NLL->item(n)->getChildNodes();
			for (m=0;m<3;m++)
			{
				critflag[n*3+m]=false;
				dn=NLLL->item(m);
				atn=dn->getAttributes()->getNamedItem(L"Name");
				nmx[n][m]=cwxml(atn->getTextContent());
				atn=dn->getAttributes()->getNamedItem(L"Date");
				dd[n][m]=cwxml(atn->getTextContent());
				atn=dn->getAttributes()->getNamedItem(L"Minimum");
				vmi[n][m]=convertd(atn->getTextContent());
				atn=dn->getAttributes()->getNamedItem(L"Maximum");
				vmx[n][m]=convertd(atn->getTextContent());

			}
		}



	}


inmx=nmx;
idd=dd;
ivmi=vmi;
ivmx=vmx;
autorescale=true;
nocrit=nor;
return(nor);
}

bool Coupler::CheckRescale(double timebase,int nodataelements,xwchar_t**nnames,int alblock,double curtime,int *&ihabitat,int &nxdim,int &nydim)
{
	int n,m,k,tz;
	bool allcrit=true;
	Poco::DateTime dtx;
	wchar_t ff[100];
    rescaleallowblock=alblock;
	for (n=0;n<nocrit;n++)
	{
		for (m=0;m<3;m++)
		{
			dtx=convertti(dd[n][m],true);
			double dvtime=dtx.julianDay()-timebase;
			if (curtime>dtx.julianDay())
				{
			critflag[n*3+m]=true;
			for (k=0;k<nodataelements;k++)
			{
				wcscpy(ff,nnames[k]);
				if (std::wcscmp(nnames[k],nmx[n][m])==0)
				{
					crit[n*3+m]=k;
					break;
				}
				else
				{
					crit[n*3+m]=-1;
				}
			}
			}
			else
			{critflag[n*3+m]=false;
			}
			allcrit&=critflag[n*3+m];
			allcrit&=(crit[n*3+m]!=-1);
		}
	}
if (allcrit)
{
	if (isbounded)
	{
		nxdim=exxh-exxl;
		nydim=exyh-exyl;
		dimfy=nydim;
		dimfx=nxdim;
		issliced=true;
	}
	else
	{
	nxdim=stdims[0];
	nydim=stdims[1];
	nxdim=dimfx;
	nydim=dimfy;
	}
	habitatc=new bool *[3];
	for (n=0;n<3;n++)
	{
	habitatc[n]=new bool[elements];
	}
	ihabitat=habitat;
	
}
usecrit=allcrit;
return(allcrit);
}

bool Coupler::EvalHabitat(double **values, int nodataelements)
{
	int n,m,k;
	bool isvalh;
	habitat=new int[nodataelements];
	for (k=0;k<nodataelements;k++)
	{
	for (n=0;n<nocrit;n++)
	{
	isvalh=true;
    for (m=0;m<3;m++)
	{
		isvalh=isvalh&&((values[crit[n*3+m]][k]>=vmi[n][m])&&(values[crit[n*3+m]][k]<=vmx[n][m]));
	}
		if (isvalh)
		{
			habitat[k]=n+1;
		}
	}
	}
	return(false);
}

void Coupler::GetHabitat(float *&habb,int nodataelements)
{
	int n;
	habb=new float[nodataelements];
	for (n=0;n<nodataelements;n++)
	{
		habb[n]=habitat[n];
	}
}



