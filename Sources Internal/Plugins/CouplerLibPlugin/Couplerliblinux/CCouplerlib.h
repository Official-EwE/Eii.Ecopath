#pragma once

#ifdef __cplusplus_cli
#include "Couplerlib.h"
#include "Protocolsock.h"
#include "dispdelegates.h"
#include "pipelink.h"
#include "MessageSock.h"
using namespace System::Collections::Generic;


namespace Couplerlib 
{

public enum class maprotocols
{
Opencoupler,
Ack_Opencoupler,
Assignnodenumber,
Establishxmllocation,
Ack_Establishxmllocation,
InitializeModel,
Ack_InitializeModel,
EditModel,
Ack_EditModel,
Runmodel,
Ack_Runmodel,
Timestep,
Returntimestep,
Finishtimestep,
Dataexchange,
Dataready,
Userrequest,
Modelterminated,
Modelfinalize,
Ack_Modelfinalize,
Modelreset,
Ack_Modelreset,
Terminatemodel,
Modelexception,
Detatchcoupler,
Ack_Detatchcoupler,
Last_Protocolno
};
public enum class mastatuscodes
{
Notdetermined,
Ok,
Waitingonresponse,
Warning,
Error,
Fatal,
};


const wstring mastrtonat(System::String ^stin);


const std::string mastrtonatc(System::String ^stin);


public ref class Cprotmessage
{
public : 
	Cprotmessage(int statno,enum class maprotocols,enum class mastatuscodes, String ^filestr);
	enum class mastatuscodes sc;
	String ^getmessage();
	protmessage *pm;
};

public ref class CProtocolSock
{
public :
	ProtocolSock *ps;
	protocols pp;
	int AddStation(String ^st); 
	Cprotmessage ^currentmessage, ^pollmessage;
	void Setstagestatus(enum class maprotocols,enum class mastatuscodes);
	enum class mastatuscodes Getstagestatus(enum class maprotocols);
	void SndMessage(Cprotmessage ^,bool newcon, int entryn);
	void Establishcomms(int stationno, String ^stname);
	bool pollevent(protocols, int tmx, bool wait);
	void resetpollevent(protocols);
};

public ref class CMessageSock
{
    textedelegate ^ textd;
    Control ^ contd;
	bool cisconsol;
public : 
	MessageSock *ms;
	void ReceiveLoop();
	void InitiateSend(String ^dest);
	void SendLoop(wchar_t *);
	void setdelegate(Control ^cont,textedelegate ^td,bool isconsol );
};

public ref class Cpipelink
{
public :
	Cpipelink();
	Cpipelink(String ^);
	void pipeserver();
	void setusesocketd(bool);
	Cpipelink(String ^,String ^);
	void setdestin(String ^);
	void endpipe();
	void setdelegate(Control ^cont,textedelegate ^td,bool usesocket,CMessageSock  ^messock);  
	textedelegate ^ textd;
	Control ^ contd;
	String ^pipename,^destind;
	void  *hPipe; //HANDLE
	wchar_t *ccstore;
	bool pipeend,usesocketd,pipehasended;
	ref class CMessageSock ^messockd;
  
};


public ref class CCouplerlib
{
public:
	CCouplerlib(void);
	CCouplerlib(bool a,bool b,bool c,bool d);
	void CleanUp();
	void SwitchCDF(bool);
	Coupler *cp;
	bool oneway,hassethabitat;
#ifdef _Has_GDI
    Object ^contd;
    Object ^errord;
	textedelegate ^ctextd,^etextd;
	void citextd(xstring s);
	void citextd(std::string s);
	void setdelegate(Object ^cd,textedelegate ^,bool isconsole);
#endif
	List<int> ^GetIfAddress(List<int> ^%, String ^, bool, bool);
	bool GetInfoFromnetCDF(String ^, int %, int %, bool %);
	int ScreennetCDFDimensions(int dimreqno, array<String ^>^dimtable,int blockno, int totblocks);
	array<String ^>^GetnetCDFvarnames(int);
	array<String ^>^GetnetCDFvarattributes(int,String ^);
	array<double>^GetnetCDFvalue(double,int,int,bool,int);
	bool SetCompressions(int,array<String ^> ^, array<int> ^,array<int> ^);
	bool GetIndex(int blockno,int idno,String ^vname);
	double GetStartTime(int);
	double GetEndTime(int);
	double GetProgress(double,int);
	void SetDimNames(array<String ^> ^);
	void PutIf(int,int,array<const double> ^,int);
	void PutIf(int,int,array<array<const double> ^> ^,int,int);
	int GetIf(int,int,int,int,int,int,array<double> ^%);
	int GetIf(int,int,int,int,array<double> ^%);
	int GetIf(int,int,int,int,array<array<double>^>^%,int,bool);
	int getgrid(int %nor,array<DateTime ,2> ^%tmx,array<double,2> ^%vmin,array<double,2> ^%vmax,array<String ^,2>^ %nm, String ^fname,array<int> ^%bounds,bool %boundi);
	void setgrid(int %nor,array<DateTime ,2> ^tmx,array<double,2> ^vmin,array<double,2> ^vmax,array<String ^,2>^ nm, String ^fname,array<int>^bounds,bool boundi);
	bool CheckInterface();
	int Getxdim();
	int Getydim();
	void EstablishTransmit(String ^);
	void FinishTransmit();
	void SetStartDate(System::DateTime ^);
	void SetEndDate(System::DateTime ^);
	int OrgReference(int,int,int);
	void GetRedims(int %,int %);
	bool EvalHabitat(array<array<double>^ > ^,int,int);
	void GetHabitat(array<float>^%,int);
	void GetSupplement(String ^nm,array<double> ^,int,int,int);
	bool GetSpatialDims(int nopart, double %,double %, double %, double %,int %,int %);
	bool CheckRescale(double timebase, int nodataelements, array<String ^> ^names, int alblock,double curtime, array<int> ^%habitatarray, int %xdim, int %ydim);
	ref class CProtocolSock ^ps;
	void Initialize(String ^a,String ^b,String ^c) {cp->Initialize(mastrtonatc(a),mastrtonatc(b),mastrtonat(c));};
	private : ref class CDataSock ^tds,^rds;
	public : ref class CMessageSock ^outputmessage,^errormessage;
public : ref class Cpipelink ^pp1, ^pp2;
};


}
#endif