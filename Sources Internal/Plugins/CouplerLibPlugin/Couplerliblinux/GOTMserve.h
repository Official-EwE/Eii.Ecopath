#include "Couplerlib.h"
#include <python/Python.h>
#include <string>
#include <list>
#include <vector>
#include "CouplerLibEvent.h"
#include "ProtocolSock.h"
#ifdef __cplusplus_cli
#include <vcclr.h>
using namespace System;
#endif

using namespace std;
using namespace Couplerlib;

class GOTMProcessing
{
	string Shortpathname;
    bool hasrun;
	int Calltype;
	PyObject *scenario;
	PyObject *result;
	int *threadstate;
	Coupler *cp;
	string FileName,PathName;
	xstring hostname,FileString;
	vector<int> *ifnoarray;
	int slabsize,EwEGOTMtimeratio;
	class iThread *pipethreadc,*pipethreade;
	double cpool,npool,ppool,cflux,nflux,pflux,spool,sflux;
	int spinupdays;
	double dt;
	xstring dstart, dend;
	
	void Display();
	bool Editserver();
	PyObject *FromScenario(PyObject *scen,char *var,PyObject *obj,char *op);
	void TimeSpecify(xstring sttime,xstring endtime,double inttime);
	xstring SpecifyGOTMXML(vector<xstring> *ivariables, vector<xstring> *iunits, vector<xstring> *iabbrev);
	void Simulate();
	bool Gotmserver(xstring fileurl);
public :
	GOTMProcessing(Coupler *cp);
	void acceptprocessing(Coupler *cp);
};

