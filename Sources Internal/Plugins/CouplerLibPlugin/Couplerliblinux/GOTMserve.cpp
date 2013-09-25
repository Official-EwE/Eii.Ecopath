#include "StdAfx.h"
#include "GOTMserve.h"
#include "pipelink.h"

extern Coupler *cp;

using namespace Couplerlib;

#ifdef __cplusplus_cli
int main(array<System::String ^> ^args)
{
	cp=new Coupler(true, false,false);
	GOTMProcessing *GP=new GOTMProcessing(cp);
	GP->acceptprocessing(cp);
}
#endif


#ifdef __plusplus_cli
GOTMProcessing::GOTMProcessing(gcroot<Coupler ^>cpi)
#else
GOTMProcessing::GOTMProcessing(Coupler *cpi)
#endif
{
cp=cpi;
}


void GOTMProcessing::acceptprocessing(Coupler *cpi)
{
	bool iscon,timeoutstat,xmlopened;
	int statno;
	protmessage *returnmessage;
	enum statuscodes cancont;
	xstring xmllocation,runmessage;
    xstring conob,errob;
	vector<xstring> substrg;
#ifdef __cplusplus_cli
#endif
cp=cpi;

cp->ps->pollevent[Opencoupler]->WaitOne(-1,false);
timeoutstat=cp->ps->pollevent[Opencoupler]->WaitOne(10000,false);
cp->ps->pollevent[Opencoupler]->Reset();
cancont=Error;
if (timeoutstat)
{
	if(cp->ps->pollmessage->pr==Opencoupler)
	{
		hostname=cp->ps->pollmessage->message;
		statno=cp->ps->AddStation(hostname);
		if (statno>0)
		{
			cp->pp1->setdestin(hostname);
			cp->pp2->setdestin(hostname);
#ifdef __cplusplus_cli

			pipethreadc=new iThread(gcnew System::Threading::ThreadStart(cp->pp1->plj,&Pipelinkjoin::pipeserver));
			pipethreade=new iThread(gcnew System::Threading::ThreadStart(cp->pp2->plj,&Pipelinkjoin::pipeserver));
#endif
			pipethreadc->Start();
			pipethreade->Start();
			returnmessage=new protmessage(cp->ps->pollmessage->stationno,Ack_Opencoupler,Ok,L"Got Open Message OK");
			cp->ps->SndMessage(returnmessage, true, statno);
			cancont=Ok;
		}
	}
	else
	{
		hostname=cp->ps->pollmessage->message;
		statno=cp->ps->AddStation(hostname);
		if (statno>0)
		{
			returnmessage= new protmessage(cp->ps->pollmessage->stationno, Ack_Opencoupler, Error, L"Failed to get Message");
			cp->ps->SndMessage(returnmessage, true, statno);
		}
	}
}
else
{
	cancont=Error;
}

if (cancont==Ok)
{
	timeoutstat=cp->ps->pollevent[Establishxmllocation]->WaitOne(-1,false);
	cp->ps->pollevent[Establishxmllocation]->Reset();
	cancont=Error;
	if (timeoutstat)
	{
		if (cp->ps->pollmessage->pr==Establishxmllocation)
		{
			xmllocation = cp->ps->pollmessage->message;
			xmlopened=Gotmserver(xmllocation);
			if (xmlopened)
			{
				returnmessage= new protmessage(cp->ps->pollmessage->stationno, Ack_Establishxmllocation, Ok, L"Got Open Message OK");
				cp->ps->SndMessage(returnmessage, false, statno);
				cancont=Ok;
			}
			else
			{
				returnmessage=new protmessage(cp->ps->stationno,Ack_Establishxmllocation, Error, L"Could not Read xml Files");
				cp->ps->SndMessage(returnmessage,false, statno);
				cancont=Error;
			}
		}
	}
}
if (cancont==Ok)
{
	timeoutstat=cp->ps->pollevent[EditModel]->WaitOne(-1,false);
	cp->ps->pollevent[InitializeModel]->Reset();
	cancont=Error;
	if (timeoutstat)
	{
		if (cp->ps->pollmessage->pr==EditModel)
		{
			xmllocation=cp->ps->pollmessage->message;
			xmlopened=Editserver();
			if (xmlopened)
			{
				xstring timemessage(xstring(dstart)+xstring(L":")+xstring(dend)+xstring(L":")+converts(dt));
				returnmessage = new protmessage(cp->ps->pollmessage->stationno,Returntimestep, Ok, timemessage);
				cp->ps->SndMessage(returnmessage, false, statno);
				cancont=Ok;
			}
			else
			{
				returnmessage= new protmessage(cp->ps->pollmessage->stationno, Returntimestep,Error, L"0");
				cp->ps->SndMessage(returnmessage, false, statno);
				cancont=Error;
			}
		}
	}
}
if (cancont==Ok)
{
	timeoutstat=cp->ps->pollevent[InitializeModel]->WaitOne(-1,false);
	runmessage=cp->ps->pollmessage->message;
	substrg.push_back(runmessage.substr(0,runmessage.find(xstring(L":"))));
	slabsize=converti(substrg[0]);
	EwEGOTMtimeratio=converti(substrg[1]);
	if (timeoutstat)
	{
		Simulate();
	}
}
timeoutstat=cp->ps->pollevent[Modelfinalize]->WaitOne(-1,false);
if (cancont==Ok)
{
Display();
}
timeoutstat=cp->ps->pollevent[Modelterminated]->WaitOne(-1,false);
}

void GOTMProcessing::Display()
{
	int a;
	PyObject *t=new PyObject();
	unsigned short int  *s;
	PyObject *simulator,*mpl,*meth,*outobj;
	PyEval_AcquireLock();
	a=PyRun_SimpleString("import PyQt4");
	simulator=PyImport_ImportModule("visualizer");
	mpl=PyImport_ImportModule("matplotlib");
	meth=PyObject_GetAttrString(mpl,"get_backend");
	outobj=PyObject_Call(meth,t,NULL);
	PyArg_ParseTuple(outobj,"u",s);
	meth=PyObject_GetAttrString(simulator,"visualizeResult");
	result=PyObject_Call(meth,t,NULL);
	PyEval_ReleaseLock();
}

bool GOTMProcessing::Editserver()
{
	PyObject *t=new PyObject();
	PyObject *scenariobuilder;
	PyObject *meth;
	PyObject *pydt,*pyst,*pyend,*pyastart,*pyaend;
	xstring dsstart,dsend;
	PyEval_AcquireLock();
	scenariobuilder=PyImport_ImportModule("scenariobuilder");
	meth=PyObject_GetAttrString(scenariobuilder,"editScenario");
	scenario=PyObject_Call(meth,t,NULL);
    PyArg_ParseTuple(FromScenario(scenario,"timeintegration/dt",pydt,"getasseconds"),"d",dt);
    PyArg_ParseTuple(FromScenario(scenario,"time/start",pyst,"toordinal"),"i",dstart.c_str());
	PyArg_ParseTuple(FromScenario(scenario,"time/stop",pyend,"toordinal"),"i",dend.c_str());
	PyArg_ParseTuple(FromScenario(scenario,"time/start",pyastart,"isoformat"),"s",dsstart.c_str());
    PyArg_ParseTuple(FromScenario(scenario,"time/stop",pyaend,"isoformat"),"s",dsend.c_str());
	TimeSpecify(dstart,dend,dt);
	PyEval_ReleaseLock();
	return(true);
}

PyObject *GOTMProcessing::FromScenario(PyObject *scen,char *var,PyObject *obj,char *op)
{
	PyObject *t=new PyObject();
	PyObject *meth,*OutObj;
	meth=PyObject_GetAttrString(scen,var);
	OutObj=PyObject_Call(meth,t,NULL);
	meth=PyObject_GetAttrString(OutObj,"getValue");
	OutObj=PyObject_Call(meth,t,NULL);
	meth=PyObject_GetAttrString(OutObj,op);
	obj=PyObject_Call(meth,t,NULL);
	return(obj);
}


void GOTMProcessing::Simulate() 
{
	int a,i,j;
	unsigned int ii,jj;
	PyObject *t=new PyObject();
	bool timeoutstat,eisended,hasmore,canrun;
	int nototdataelements,nodataelements;
	int interleave;
	double realdays,extraday;
	PyObject *simulator,*scenario,*bioinfo;
	PyObject *argsint=new PyObject();
	PyObject *OutObj[3],*Obj;
	vector<int> modelio, modelii, modelno, modelni, modeliox, modelnix, modeliix, modelnox;
	vector<int> linkn,linkp,links;
	vector<int> isorg,isc;
	double ncratio,pcratio,scratio,nextran,pextran,sextran,adjn,adjp,adjs,cadj;
	vector<xstring> *names, *units, *abbrev;
	xstring lmname,biotext,runmessage;
	int noi;
	double depth,dp,adjpool,adjmult,temperature,salinity;
	double npool,cpool,spool,ppool;
	int bstart,bensplit;
	int nprogress;
	PyObject *biovals,*biovalsben;
	PyObject *meth;
	vector<double> vals,oldvals,newvals,benvals,valarray;
	protmessage *returnmessage;
    bioinfo=new PyObject;
	PyEval_AcquireLock();

	a=PyRun_SimpleString("import os");
	simulator=PyImport_ImportModule("core.simulator");
	meth=PyObject_GetAttrString(simulator,"Simulator");
	scenario=PyObject_Call(meth,t,NULL);
	meth=PyObject_GetAttrString(scenario,"describe");
	bioinfo=PyObject_Call(meth,t,NULL);
	
	for (i=0;i<3;i++)
	{
    OutObj[i]=PyTuple_GetItem(bioinfo,i);
	}
	abbrev=new vector<xstring>(PyList_Size(OutObj[0]));
	names=new vector<xstring>(PyList_Size(OutObj[1]));
	units=new vector<xstring>(PyList_Size(OutObj[2]));
	for (j=0;j<PyList_Size(OutObj[0]);j++)
	{
	Obj=PyList_GetItem(OutObj[0],j);
	PyArg_ParseTuple(Obj,"s",((*abbrev)[j]).c_str());
    Obj=PyList_GetItem(OutObj[1],j);
	PyArg_ParseTuple(Obj,"s",(*names)[j].c_str());
	Obj=PyList_GetItem(OutObj[2],j);
	PyArg_ParseTuple(Obj,"s",(*units)[j].c_str());
	if ((*abbrev)[j]==xstring(L"Y1c"))
	{
		bstart=j;
 	}
	adjpool=1.0;
	nototdataelements=172;
    npool=cpool=ppool=spool=0.0;
	PyObject *benflag,*pelflag;
	benflag=PyTuple_New(1);
	pelflag=PyTuple_New(1);
	PyTuple_SetItem(benflag,0,PyInt_FromLong(0));
	PyTuple_SetItem(pelflag,1,PyInt_FromLong(1));
	abbrev->erase(abbrev->begin());
	names->erase(names->begin());
	units->erase(units->begin());
	meth=PyObject_GetAttrString(scenario,"getBioValues");
	biovals=PyObject_Call(meth,pelflag,NULL);
    meth=PyObject_GetAttrString(scenario,"getBioValues");
	biovalsben=PyObject_Call(meth,benflag,NULL);
    for (i=0;i<PyList_Size(biovals);i++)
	{
    double tval;
	Obj=PyList_GetItem(biovals,i);
	PyArg_ParseTuple(Obj,"d",tval);
	vals.push_back(tval);
	}
	for (i=0;i<PyList_Size(biovalsben);i++)
	{
    double tval;
	Obj=PyList_GetItem(biovalsben,i);
	PyArg_ParseTuple(Obj,"d",tval);
	benvals.push_back(tval);
	}
	for (ii=0;ii<benvals.size();ii++)
	{
		vals.push_back(benvals[ii]);
	}
	nodataelements=vals.size()+3;
	abbrev->push_back(L"ETW");
	names->push_back(L"Temperature");
	units->push_back(L"C");
	abbrev->push_back(L"ESW");
	names->push_back(L"Salinity");
	units->push_back(L"psu");
	abbrev->push_back(L"SDp");
	names->push_back(L"Depth");
	units->push_back(L"m");
	lmname=SpecifyGOTMXML(names,units,abbrev);
	hasmore=true;
	cp->ps->SndMessage(new protmessage(cp->ps->pollmessage->stationno,Runmodel,Ok,L"Running"), false, cp->ps->pollmessage->stationno);
	canrun=(cp->ps->pollmessage->sc==Ok);
	cp->ps->pollevent[Ack_Runmodel]->WaitOne(-1,false);
	cp->CheckInterface(ifnoarray);
	modelio=cp->GetIfAddress(modelno,lmname,false,false);
	modeliix=cp->GetIfAddress(modelno,lmname,true,true);
	modelii=cp->GetIfAddress(modelni,lmname,false,true);
	modeliox=cp->GetIfAddress(modelnix,lmname,true,false);
	interleave=0;
	realdays=0.0;
	if (canrun)
	{
		while(hasmore)
		{
			PyTuple_SetItem(argsint,0,PyInt_FromLong(long(slabsize)));
	PyTuple_SetItem(pelflag,1,PyInt_FromLong(1));
	meth=PyObject_GetAttrString(scenario,"runSlab");
	Obj=PyObject_Call(meth,argsint,NULL);
	int tint;
	double tval;
    PyArg_ParseTuple(Obj,"i",tint);
    hasmore=(tint==1);
    meth=PyObject_GetAttrString(scenario,"getProgress");
	Obj=PyObject_Call(meth,argsint,NULL);
    PyArg_ParseTuple(Obj,"d",nprogress);
	meth=PyObject_GetAttrString(scenario,"getDepth");
	Obj=PyObject_Call(meth,argsint,NULL);
    PyArg_ParseTuple(Obj,"d",depth);
	meth=PyObject_GetAttrString(scenario,"getTemperature");
	Obj=PyObject_Call(meth,argsint,NULL);
    PyArg_ParseTuple(Obj,"d",temperature);
	meth=PyObject_GetAttrString(scenario,"getSalinity");
	Obj=PyObject_Call(meth,argsint,NULL);
	PyArg_ParseTuple(Obj,"d",salinity);
	meth=PyObject_GetAttrString(scenario,"getBioValues");
	biovals=PyObject_Call(meth,pelflag,NULL);
    meth=PyObject_GetAttrString(scenario,"getBioValues");
	biovalsben=PyObject_Call(meth,benflag,NULL);
	vals.clear();
	benvals.clear();
	for (i=0;i<PyList_Size(biovals);i++)
	{
    double tval;
	Obj=PyList_GetItem(biovals,i);
	PyArg_ParseTuple(Obj,"d",tval);
	vals.push_back(tval);
	}
	bensplit=vals.size();
	for (i=0;i<PyList_Size(biovalsben);i++)
	{
    double tval;
	Obj=PyList_GetItem(biovalsben,i);
	PyArg_ParseTuple(Obj,"d",tval);
	benvals.push_back(tval);
	}
	nodataelements=bensplit+benvals.size()+3;
	for (ii=bensplit;ii<vals.size()-4;ii++)
	{
		vals.push_back(benvals[ii-bensplit]);
	}
	vals.push_back(temperature);
	vals.push_back(salinity);
	vals.push_back(depth);
	if (((interleave % EwEGOTMtimeratio)==0) && (interleave>=spinupdays) &&(extraday<1) ||(!hasmore))
	{
		for (ii=0;ii<modelio.size();ii++)
		{
			cp->PutIf(modelni[ii],modelio[ii],vals,nodataelements);
		}
		biotext=L"";
	}
	for (ii=0;ii<vals.size();ii++)
	{
		oldvals[ii]=vals[ii];
	}
	if (((interleave % EwEGOTMtimeratio)==0) && (interleave>=spinupdays) &&(extraday<1))
	{
		returnmessage=new protmessage(cp->ps->pollmessage->stationno,Timestep,Error,converts(nprogress)+xstring(L":")+biotext);
		cp->ps->SndMessage(returnmessage,false,0);
		cp->ps->pollevent[Finishtimestep]->WaitOne(-1,false);
		if(cp->ps->pollmessage->sc==Waitingonresponse)
		{
			hasmore=false;
		}
		cp->ps->pollevent[Finishtimestep]->Reset();
		runmessage=cp->ps->pollmessage->message;
	}
	extraday=double(realdays-interleave);
	if (interleave>=spinupdays)
	{
		if (extraday<1)
		{
			realdays+=365.25/360.0;
			for (ii=0; ii<modelni.size();ii++)
			{
				if ((interleave % EwEGOTMtimeratio) ==0)
				{
					cp->EstablishTransmit(hostname);
					noi=cp->GetIf(modelnix[ii],modeliox[ii],modelni[ii],modelii[ii],modelnox[ii],modelio[ii],valarray);
					cp->FinishTransmit();
				}
				int d;
				for (j=0;j<noi;j++)
				{
					d=cp->OrgReference(modelnix[ii],modeliox[ii],j);
					if (isorg[d]==1)
					{
						adjmult=adjpool;
					}
					else
					{
						adjmult=1.0;
					}
					if (valarray[j]<0)
					{
						newvals[cp->OrgReference(modelnix[i],modeliox[i],j)]+=(-valarray[j])*newvals[cp->OrgReference(modelnix[i],modeliox[i],j)];
					}
					else
					{
						newvals[cp->OrgReference(modelnix[i],modeliox[i],j)]-=(valarray[j])*newvals[cp->OrgReference(modelnix[i],modeliox[i],j)];
					}
					if (newvals[cp->OrgReference(modelnix[i],modeliox[i],j)]<0.0)
					{
						d=-1;
					}
				}
				cflux=nflux=pflux=0.0;
				for (ii=0;ii<oldvals.size()-3;ii++)
				{
					if (ii>(unsigned int)bensplit)
					{
						dp=1.0;
					}
					else
					{
						dp=depth;
					}
					if (isc[ii]==1)
					{
						cpool-=dp*(newvals[ii]-oldvals[ii]);
						cflux+=fabs(dp*(newvals[ii]-oldvals[ii]));
						if (linkn[ii]>-1)
						{
							npool-=dp*(newvals[linkn[ii]]-oldvals[linkn[ii]]);
							nflux+=fabs(dp*(newvals[linkn[ii]]-oldvals[linkn[ii]]));
						}
						if (linkp[ii]>-1)
						{
							ppool-=dp*(newvals[linkp[ii]]-oldvals[linkp[ii]]);
							pflux+=fabs(dp*(newvals[linkp[ii]]-oldvals[linkp[ii]]));
						}
						if (links[ii]>-1)
						{
							npool-=dp*(newvals[links[ii]]-oldvals[links[ii]]);
							sflux+=fabs(dp*(newvals[links[ii]]-oldvals[links[i]]));
						}
					}
				}
				ncratio=0.015;
				pcratio=0.00167;
				scratio=0.00167;
				nextran=npool-cpool*ncratio;
				pextran=ppool-cpool*pcratio;
				sextran=spool-cpool*scratio;
				double adjntot=0.0;
				double adjptot=0.0;
				double adjstot=0.0;
				for (ii=0;ii<oldvals.size()-3;ii++)
				{
					if (ii>(unsigned int)bensplit)
					{
						dp=1.0;
					}
					else
					{
						dp=depth;
					}
					if (isc[ii]==1)
					{
						if ((linkn[ii]>-1)&&(isorg[ii]==1))
						{
							adjn=nextran*fabs(dp*(newvals[linkn[ii]]-oldvals[linkn[ii]]))/nflux;
							newvals[linkn[ii]]+=adjn/dp;
							npool-=adjn ;
							adjntot+=adjn;
						}
						if ((linkp[ii]>-1)&&(isorg[ii]==1))
						{
							adjp=pextran*fabs(dp*(newvals[linkp[ii]]-oldvals[linkp[ii]]))/pflux;
							newvals[linkp[ii]]+=adjp/dp;
							ppool-=adjp ;
							adjptot+=adjp;
						}
						if ((links[ii]>-1)&&(isorg[ii]==1))
						{
							adjs=sextran*fabs(dp*(newvals[links[ii]]-oldvals[links[ii]]))/sflux;
							newvals[links[ii]]+=adjs/dp;
							spool-=adjs ;
							adjstot+=adjs;
						}
					}
				}
						adjpool=1/(1-cpool/1000.0);
						for (ii=0;ii<newvals.size();ii++)
						{
						}
						if (newvals[ii]<0.01)
						{
							newvals[ii]=0.01;
						}
					    //list<PyListObject *> *pynewvals;
						PyObject *pynewvals=PyList_New(vals.size());
						for (ii=0;ii<vals.size()-1;ii++)
						{
							Obj=PyFloat_FromDouble(newvals[ii]);
							PyList_SetItem(pynewvals,ii,Obj);
						}
						    PyObject *oup=PyTuple_New(1);
							PyTuple_SetItem(oup,0,pynewvals);
							meth=PyObject_GetAttrString(scenario,"setBioValues");
	                        PyObject_Call(meth,oup,NULL);
							PyObject *pynewbenvals=PyList_New(newvals.size()-bensplit-2);
							for (jj=bensplit;jj<newvals.size()-3;jj++);
							{
								Obj=PyFloat_FromDouble(newvals[jj]);
							PyList_SetItem(pynewvals,jj-bensplit,Obj);
							}
							PyTuple_SetItem(oup,0,pynewvals);
							meth=PyObject_GetAttrString(scenario,"setBioValuesBenthic");
	                        biovals=PyObject_Call(meth,oup,NULL);
					}
				}
				}
				else
				{
					meth=PyObject_GetAttrString(scenario,"setBioValues");
	                        PyObject_Call(meth,t,NULL);
				realdays+=1.0;
				}
				interleave+=1;
				}
				cp->ps->SndMessage(new protmessage(cp->ps->pollmessage->stationno,Timestep,Waitingonresponse,converts(nprogress)+xstring(L":")+biotext),false,cp->ps->pollmessage->stationno);
				hasrun=true;
				meth=PyObject_GetAttrString(scenario,"finalize");
	            result=PyObject_Call(meth,t,NULL);
				xstring errmsg;
				PyArg_ParseTuple(result,"u",errmsg.c_str());
				}
				PyEval_ReleaseLock();
				}
}

bool GOTMProcessing::Gotmserver(xstring fileurl)
{
	unsigned int i;
	PyObject *meth,*t,*obj;
	int a,tr,novars,initok;
	string pypath,py2path,scenpath;
	string svtemp;
	list <xstring> svnames,svvalues;
	pypath="";
	PathName=string("chdir(\"\"")+string("\"\")");
	PathName=string("chdir(\"\"") /*  iPath::GetDirectoryName(fileurl)*/+string("\"\")");
	Shortpathname="  "; //iPath::GetDirectoryName(fileurl);
	FileName=string("execfile(\"\"")/* iPath::GetFileName(fileurl)*/+string("\"\")");
	for (i=0;i<Shortpathname.length();i++)
	{
		pypath+=Shortpathname[i];
		if (Shortpathname[i]=='\\')
		{
			pypath+='\\';
		}
	}
	py2path=";"+Shortpathname+"\\xmlplot;";
    py2path+=Shortpathname+"\\core;";
	scenpath=Shortpathname+"\\bfm1.gotmscenario2";
	pypath+=";";
	initok+=cp->Initialize(Shortpathname,Shortpathname+"\\meecedict.xml",fileurl);
	novars=cp->GetVariableValues(svnames,svvalues);
	ienvironment::SetEnvironmentVariable("PYTHONPATH",ienvironment::GetEnvironmentVariable("PYTHONPATH")+";"+Shortpathname);
	ienvironment::SetEnvironmentVariable("PATH",ienvironment::GetEnvironmentVariable("PATH")+";"+Shortpathname);
	svtemp=ienvironment::GetEnvironmentVariable("PATH");
	if (!hasrun)
	{
		Py_Initialize();
		PyGILState_STATE gs=PyGILState_Ensure();
		PyObject *corescenario;
		corescenario=PyImport_ImportModule("core.scenario");
	meth=PyObject_GetAttrString(PyObject_GetAttrString(corescenario,"Scenario"),"fromSchemaName");
	PyObject *str=PyString_FromString("gotmgui-0.5.0");
	obj=PyObject_Call(meth,str,NULL);
	meth=PyObject_GetAttrString(corescenario,"loadAll");
    str=PyString_FromString(scenpath.c_str());
	obj=PyObject_Call(meth,str,NULL);
	PyObject *scenariobuilder,*newscenario;
	scenariobuilder=PyImport_ImportModule("scenariobuilder");
	meth=PyObject_GetAttrString(scenariobuilder,"loadScenario");
	newscenario=PyObject_Call(meth,t,NULL);
	if (newscenario!=NULL)
	{
		scenario=newscenario;
	}
		PyGILState_Release(gs);
}
return(true);
}


void GOTMProcessing::TimeSpecify(xstring sttime, xstring endtime, double inttime)
{
unsigned int n,m;
DOMNodeList *NL,*CL,*GCL;
DOMNode *Node;
XercesDOMParser *parser;
double timespan;
DOMDocument *Specification;
parser=new XercesDOMParser;
parser->parse((Shortpathname+string("GOTMtemplate.xml")).c_str());
Specification=parser->getDocument();
NL=Specification->getElementsByTagName(cxml(L"TimeDimensionTemporal"));
for (n=0;n<NL->getLength();n++)
{
	Node=NL->item(n);
	CL=Node->getChildNodes();
	for (m=0;m<CL->getLength();m++)
	{
		if (CL->item(m)->getNodeName()==xstring(L"StartTime"))
		{
			CL->item(m)->setNodeValue(sttime.c_str());
		}
		if (CL->item(m)->getNodeName()==xstring(L"EndTime"))
		{
			CL->item(m)->setNodeValue(endtime.c_str());
		}
		if (CL->item(m)->getNodeName()==xstring(L"Interval"))
		{
			GCL=CL->item(m)->getChildNodes();
		}
	}
}
}

xstring GOTMProcessing::SpecifyGOTMXML(vector<xstring> *ivariables,vector<xstring> *iunits,vector<xstring> *iabbrev)
{
	DOMNodeList *NL;
	XercesDOMParser *parser;
DOMDocument *Specification;
int cl,cl2;
unsigned int n,m,k;
xstring dt,ftype,tabrev,mname;
vector<xstring> *FgType=new vector<xstring>;
vector<xstring> *Fgstr=new vector<xstring>;
vector<xstring> *GroupType=new vector<xstring>;
vector<xstring> *AbbrevType=new vector<xstring>;
//FgType->assign(  {L"C",L"N"};
DOMNode *Node,*Child,*GChild,*GGChild,*GGGChild;
parser=new XercesDOMParser;
parser->parse((Shortpathname+string("GOTMtemplate.xml")).c_str());
Specification=parser->getDocument();
NL=Specification->getElementsByTagName(cxml(L"DataCollection"));
mname=xstring(NL->item(0)->getNodeName());
for (n=0;n<NL->getLength();n++)
{
	Node=NL->item(n);
	for (m=0;m<ivariables->size();m++)
	{
		cl=12;
		for (k=0;k<AbbrevType->size();k++)
		{
			if (*((iabbrev[m]).begin())==*((AbbrevType[k]).begin()))
			{
				cl=k;
			}
		}
		Child=Specification->createElement(cxml(L"Data"));
		GChild=Specification->createElement(cxml(L"Name"));
		if (cl<7)
		{
			GChild->setNodeValue(((*iabbrev)[m].substr(0,(*iabbrev)[m].size()-1)).c_str());
		}
		else
		{
			GChild->setNodeValue((*iabbrev)[m].c_str());
		}
		Child->appendChild(GChild);
		GChild=Specification->createElement(cxml(L"DataItem"));
		GChild->setNodeValue(cxml(L""));
		GGChild=Specification->createElement((*GroupType)[cl].c_str());
		GGGChild=Specification->createElement(cxml(L"Name"));
		GGGChild->setNodeValue((*ivariables)[m].c_str());
		GGChild->appendChild(GGGChild);
		if (cl<7)
		{
			ftype=(*iabbrev)[m].substr((*iabbrev)[m].size()-1,1);
			tabrev=(*iabbrev)[m].substr(0,(*iabbrev)[m].size()-1);
			cl2=5;
			for (k=0;k<Fgstr->size();k++)
			{
				if ((*Fgstr)[k]==ftype)
				{
					cl2=k;
				}
			}
			GGGChild=Specification->createElement(cxml(L"Constituent"));
			GGGChild->setNodeValue(((*FgType)[cl2]).c_str());
			GGChild->appendChild(GGGChild);
		}
		else
		{
			GGGChild=Specification->createElement(cxml(L"Constituent"));
			GGGChild->setNodeValue(cxml(L"U"));
			GGChild->appendChild(GGGChild);
			tabrev=(*iabbrev)[m];
		}
		GGGChild=Specification->createElement(cxml(L"Symbol"));
		GGGChild->setNodeValue(tabrev.c_str());
		GGChild->appendChild(GGGChild);
		GGGChild=Specification->createElement(cxml(L"Description"));
		GGGChild->setNodeValue(((*ivariables)[m]+xstring(cxml(L" EwE Group"))).c_str());
		GGChild->appendChild(GGChild);
		GChild->appendChild(GGChild);
		Child->appendChild(GGChild);
		GChild=Specification->createElement(cxml(L"Flux"));
		if (n==0)
		{
			GChild->setNodeValue(cxml(L"State"));
		}
		else
		{
			GChild->setNodeValue(cxml(L"Predation"));
		}
		Child->appendChild(GChild);
		GChild=Specification->createElement(cxml(L"Units"));
		if (n==0)
		{
			GChild->setNodeValue((*iunits)[m].c_str());
		}
		else
		{
			GChild->setNodeValue(((*iunits)[m]+xstring(L"/Interval")).c_str());
		}
		Child->appendChild(GChild);
		Node->appendChild(Child);
	}
}
//save
return(mname);

}
