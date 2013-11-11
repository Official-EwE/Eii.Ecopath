// Couplerlib.h

#pragma once

//using namespace System;
//using namespace System::Collections::Generic;
//using namespace System::Xml;
//using namespace System::IO;
#include <xercesc/dom/DOM.hpp>
#include "xercesc/parsers/XercesDOMParser.hpp"
#include "xercesc/dom/DOMDocument.hpp"
#include "xercesc/sax/HandlerBase.hpp"
#include "xercesc/framework/LocalFileFormatTarget.hpp"
#include <xercesc/dom/DOMLSSerializer.hpp>
//#include<xercesc/dom/domdocument.hpp>
#include<vector>
#include<list>
#include<string>
#include<time.h>
#ifdef allownetcdf
#include<netCDF.h>
#endif

using namespace xercesc;
using namespace std;
#include "unetdefs.h"
#include "dispdelegates.h"
#ifdef __cplusplus_cli
using namespace System;
//using namespace System::Windows::Forms;
#endif
#define FILE_NAME "tst_chunks.nc"

#define __MSXML_LIBRARY_DEFINED__
namespace Couplerlib {

	
#ifdef __cplusplus_cli
	class Coupler
	{
	public : gcroot<ref class CCouplerlib^> cpx;
		
#else
	class Coupler
	{
#endif
	private :	list<class xercesc::DOMDocument *> ModelDocs;
		class DOMNodeList *NL;
		vector<DOMNodeList *> NLArray;
		unsigned int maxcheck;
		bool usenetwork,oneway;
		void ctextd(xstring);
		void ctextd(string);
		void ctextd(wchar_t *);
		vector<int> gmodels,gtable;
		class xercesc::XercesDOMParser *parser;
		class xercesc::DOMDocument *Dictionary;
		class xercesc::DOMDocument *Specification,*GridSpecification;
		list<const XMLCh *> varnames;
		list<const XMLCh *> valuenames;
		vector<const XMLCh *> *specfilenames;
		vector<xstring> synonyms,synonymvals,convertunits,convertto,mapgroups,gntable;
		vector<xstring> convertmuldiv;
		vector<double> convertmultdivp;
		vector<class DictContext *> synonymcontext,convertcontext,mapcontext;
		//DictContext **synonymcontext, **convertcontext,**mapcontext;
		vector<vector<xstring> > mapgroupsto;
		vector<vector<double> > mapratio;
		vector<bool> mappreserveratio;
		vector<xstring> modelnames,systemnames,languagenames;
		vector<double> modelversion,systemversion,languageversion,convertratio; 
		vector<vector<xstring> > subinterf;
		//String **subinterf
		vector<vector<double> > dimm,dimb;
		vector<vector<int> > ifmeth;
		vector<vector<int> > dims;
		vector<vector<int> > dimsq;
		vector<vector<int> > nodims,ifdir,thrd;
		vector<vector<Poco::DateTime> > cstime, cetime;
		vector<vector<Poco::DateTime > > citime;
		vector<list<int> >checks; 
		vector<vector<vector<const xwchar_t *> > > orgsname,orgssymb,orgsdes,orgsconst,orgunits;
		vector<vector<vector<xstring> > > orgname;
		vector<vector<vector<int> > > orgtype,orgdir,curorg,masterpresratio;
		vector<vector<vector<double> > >orgbuf;
		vector<vector<vector<vector<double> > > >aorgbuf;
		vector<vector<vector<vector<int> > > > mastercon;
		vector<vector<vector<vector<double> > > > masterunits,masterunitspf,masterratio;
		vector<vector<vector<vector<int> > > > masterunitslink;
		vector<vector<vector<vector<double> > > > supvarvals;
		vector<vector<vector<vector<int> > > > supvarrefs;
		vector<vector<vector<vector<const xwchar_t *> > > > supvarorgnames;
		vector<vector<vector<const xwchar_t *> > > supvarnames;
		vector<vector<vector<double> > > supvardefault;
		vector<vector<vector<int> > > supvarnovalues;
		vector<vector<unsigned int> > supvarnovariables;

		list<const xwchar_t *> datalist,dataconst;
	list<int> dataflux;
	public :    int elements,dimfx,dimfy;
	public :int Initialize(string,string,xstring ); // Initialize List of Interfaces 
		int CreateDocument(); // Created By Model
		int LoadDocument();
		void SwitchCDF(bool useCDF);
		int SaveDocument(); //Interface for Model
		int GetModelDescriptionInterf(class DOMNode *des,class xercesc::DOMDocument *xdoc,int ifno);
		int GetDirectionInterf(class DOMNode *,int compno,int isfrom,int ifno);
		bool GetSpatialDims(int,double &,double &,double &,double &,int &,int &); 
		bool GetInfoFromnetCDF(char *,int &nodims,int &novars, bool &isthreed);
		bool GetIndex(int blockno,int idno,std::string vname);
		bool Coupler::OpennetCDFExtension(double timex);
		double GetStartTime(int);
		double GetEndTime(int);
		void SetStartDate(double);
		void SetEndDate(double);
		void SetDimNames(xstring *);
		double GetProgress(double,int);
		int parsedataitem(int fno,int intf,int it,class DOMNode *xl);
		int JoinEnds(int,int,int,int,int);
		int Scale(int,int,int,int);
		int GetIf(int modelno,int ifno,int modelxno, int ifxno,vector<double> &modeldat);
		int GetIf(int modelno,int ifno,int modelxno, int ifxno,int modelyno,int ifyno,vector<double> &modeldat);
		int GetIf(int modelno,int ifno,int modelxno, int ifxno,vector<vector<double> > &modeldat,int noels, bool noredim);
		int GetIf(int modelno,int ifno,int modelxno, int ifxno,int modelyno,int ifyno,vector<vector<double> >&modeldat, bool noredim);
		double GetFromBuffer(int modno,int ifno,int orgno);
		vector<double> & GetFromBufferA(int modno,int ifno,int orgno);
		double GetDataItem(int,int,int);
		void PutIf(int modelno,int ifno,vector<double> &,int items);
		int PutIf(int modelno,int ifno,vector<vector<double> >*,int items,int noelements);
		int CheckTextName(xstring frname,int index,vector<xstring >nametables,xstring message);
		list<int> CheckNames(xstring frname,int index,vector<xstring>nametables,xstring message);
		int CheckTextVersion(double minval,double maxval,int index,vector<double> versions,xstring message);
		int CheckNumAllowed(int numin,int index,vector<int> valuevector,xstring message);
		int CheckDim(int,xstring iname[],int dimno,int,double minb[],double maxb[],double minm[],double maxm[],int minl[],int maxl[],xstring message);
		list<int> CheckDirs(int modnm,int ifnn,int datadir);
		list<int> CheckAllowed(double minval,double maxval,int index,vector<double> versions,xstring message);
		list<int> CheckDate(Poco::DateTime minval,Poco::DateTime maxval,int index,vector<Poco::DateTime> times,xstring message);
		list<int> CheckInterval(double minval,double maxval,int index,vector<double > times,xstring message);
		list<int> LinkData(list<const xwchar_t *>,list<const xwchar_t *>, list<int>,int,int);
		list<int> PermittedInterfaces(vector<list<int> >,int);
		vector<int> *GetIfAddress(vector<int> &modelx,xstring intername,bool isinput,bool ispaired);
		list<int> GetRegroup(xstring,list<vector<xstring> > &,list<vector<double> >&, xstring,xstring);
		double ConvertUnits(xstring,xstring &,int &,xstring,xstring,xstring,xstring,double &,xstring &);
		vector<int> Getsynonyms(xstring,xstring,vector<xstring> &onames, vector<const xwchar_t *>);
		list<int> LinkSupp(vector<const xwchar_t * > &iorgnames,list<const xwchar_t * > &iorgconstit, int nno,int ifno,int varno,int noitems);
		bool GetSupplement(xstring varname, vector<double >svalues,int modno, int intno,int novalues);
		int ScreennetCDFDimensions(int dimreqno, char **dimtable,int blockno=0, int totblocks=1);
		void GetnetCDFScale(std::string dimname, double &dmin, double &dmax, double &dstep);
		bool OpenNetCDFOutput(char *iname,int nodims,int novars, int noglobatt, char **globatt, char **globattval,float *fv);
		bool CreateDimensions(int ndims, int *dimlen ,int *dimtype,double **dims, int *dimsattribno, char **dimsname, char ***dimsattribname,char ***dimsattribtext);
		bool CreateVariables(int ngroups, char **igname, int nattrib, char **iattnames,char ***iattvals);
		void StoreVariables(int ngroups,float **ivalues, int tindex);
		bool StoreDimVariables();
		bool CloseNetCDF();
		vector<std::string> GetnetCDFvarnames(int);
		vector<std::string> GetnetCDFvarattributes(char *,int);
		vector<double> *GetnetCDFvalue(double,int,int,bool,int);
		vector<double> *GetnetCDFCachedValue(double,int,int,bool);
		vector<double> *GetnetCDFDirectValue(double,int,int,bool,bool,int);
		bool EvalHabitat(double **values, int nodataelements);
		void GetHabitat(float *&habb,int nodataelements);
		char *MakeCDFFileTable(char *iname);
		void ReOpen(char *cfname);
		bool chkdims(int,int);
		int exxl, exxh,exyl,exyh;
		bool isbounded;
		std::vector<double>  *ExtractHabitat(std::vector<double> *dv,int blk,int varno,bool usecrt);
		int *Coupler::ColCriteria(int blk);
		class ProtocolSock *ps;
		xwchar_t ***dd;
		xwchar_t ***nmx;
		double **vmi;
		double **vmx;
	private : class DataSock *tds,*rds;
		      bool hasopenednetCDF,usenetCDF,usecrit,issliced;
			  int ncid,oncid,noallocatedblocks,nonetCDFdims,tmxvar,sumtra;
			  std::string *varname,*dimname,**varattname;
			  int **vardims,*vardimt,*dimlen,*vardimsf,*vardsize,nonetCDFvars,timedim,*varattnparts;
			  int **CDFiblocks,*CDFiblockcount,*dimallow,*dimdisallow;
			  int zdimmod,zdimdiv,dimz;
			  int *dimid, *varid;
			  char **netcdffile;
			  double *toffset;
			  double *txmin,*txmax;
			  int exfilelistno;
			  double *fillvalue;
			  xstring ddnames[3];
			  double scales[3];
			  int stindex[3],stdims[3];
			  double *vardscale;
			  int *vardoffset,*vardxtent;
			  std::string **varattval;
			  char *varatttext;
			  size_t **vari,**variq;
	          size_t **varx,**varxq;
		      bool *iscached;
		      int **cachesize;
			  double **divcache;
		      int ***cacheval,**cachesval;
			  double ***cachesca;
			  int *cachenoitems,*divnoitem;
			  bool *singlecache;
			  int icarfile;
			  int timecache[10];
			  bool *critflag;
			  bool **habitatc;
			  int *habitat;
			  int *crit;
			  int nocrit;
			  int sdims,sdimlen[10];
			  int rescaleallowblock;
			  double *rv2;
			  bool rvset;
			  float **dimvector;
	public : class MessageSock *outputmessage,*errormessage;
		class pipelink *pp1,*pp2;
		bool rezero;
		bool autorescale;
		int depfl;
		xstring errt, cont;
        xstring searchroot;
		void FinishTransmit();
		void EstablishTransmit(xstring newdest);
		int OrgReference(int mod,int intf,int seq);
		bool CheckInterface(); // {return CheckInterface(new std::vector<int>());};
		Coupler();
		Coupler(bool usesocks,bool isserver,bool isactive,bool usenetCDF=false);
		void CleanUpnetCDF(void);
		~Coupler();
	private : int SpecifyInterface(xstring); // User Specified Interface relationships
		
		int LoadDictionary(string); // Data Dictionary for translations
		int GetVariableValues(list<const xwchar_t *> &, list<const xwchar_t*> &);
		public :
		int GetVariableValues(list<xstring>,list<xstring>);
		bool SetCompressions(int nocomps, std::vector<std::string> comps, int comptype[],int codims[]);
		void filternodes(DOMNode *);
		int GetnetCDFtime(double tval);
		void setgrid(int nor,xwchar_t ***dd,double **vmi,double **vmx,xwchar_t ***nmx,xwchar_t *,int *boundx, bool boundi);
		int getgrid(int &nor,xwchar_t *** &dd,double ** &vmi,double **&vmx, xwchar_t *** &nmx, xwchar_t *,xwchar_t *&cult, int * &boundx, bool & boundi);
		bool CheckRescale(double timebase, int nodataelements,wchar_t **nnames,int alblock,double curtime,int *&habitat,int &nxdim,int &nydim);
		
		
	};

	
    //delegate void textedelegate(Object Inputtext);

}
