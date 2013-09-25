Imports EwECore
Imports System.IO
Imports System.Text
Imports Python.Runtime
Imports System.Threading
Imports System.Diagnostics
Imports Microsoft.Win32
Imports Couplerlib





Public Module GlobalCoupler
    Public cpglobal As CCouplerlib
End Module



Public Class FormGotmPluggin

    Public xx As CCouplerlib

    Private m_core As cCore
    Dim stationno As Integer
    'Dim fs As FileStream
    Private timebase As Double = 2436934.0
    Private TestDataPath As String
    Public hasrun, isnotended As Boolean
    Private CallType As Integer
    Private spinupdays As Integer
    Private FileString As String
    Private PathName As String
    Private FileName As String
    Private sockettext As String
    Private Connectname As String
    Private ncfchanged As Boolean
    Public Pycallback As Python.Runtime.PyObject
    Private StatusT As String() = {"Not Ready", "Waiting", "Running", "Finished", "Error"}
    Private scenario, result, netcdfmodule As PyObject
    Private thread, pipethreadc, pipethreade As Thread
    Private threadstate As IntPtr
    Private nprogress As Single
    Private biotext As String
    Private isedited, isclicked, isinitialized As Boolean
    Public plugin As GOTMplugin
    Dim cpool, npool, ppool, cflux, nflux, pflux, spool, sflux As Double
    Dim alongitude, alatitude, azed As Double()
    Private slabsize As Integer
    Public EwEGOTMtimeratio As Integer
    Public cp As CCouplerlib
    Delegate Sub stadelegate(ByVal ib As Integer, ByVal ic As Integer)
    Dim sta1 As stadelegate
    Public wx4, wx5, wx6 As Boolean
    Private usenetCDFfile As Boolean
    Dim Specification As Xml.XmlDocument
    Dim dictiname, Shortpathname As String
    Private maxbufferlen, currbufferlen As Integer
    Dim stbuf() As String
    Public habitatarray() As Integer
    Private isrescale, isrescale2 As Boolean
    Private xdim, ydim As Integer
    Dim isthreed As Boolean







    ''' <summary>
    ''' New constructor, is called everytime this object is created
    ''' </summary>
    Public Sub New(ByVal Core As cCore)
        Dim regkey1, regkey2 As RegistryKey
        isinitialized = False
        EwEGOTMtimeratio = 1
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        NumericUpDown1.Value = EwEGOTMtimeratio
        Status(0, 0)
        usenetCDFfile = False
        Try
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", False)
            regkey1 = regkey1.OpenSubKey("EwE", False)
            regkey1 = regkey1.OpenSubKey("CouplerLib", False)
            regkey2 = regkey1.OpenSubKey("NetCDF", False)
            TestDataPath = regkey1.GetValue("COUPLERPATH").ToString() + "\Testdata"
            'TestDataPath = "E:\ERSEMEwECoupler" + "\Testdata"
        Catch  'No Registry use Default
            TestDataPath = "E:\ERSEMEwECoupler" + "\Testdata"
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", True)
            regkey1 = regkey1.CreateSubKey("EwE")
            regkey1 = regkey1.CreateSubKey("CouplerLib")
            regkey1.SetValue("COUPLERPATH", "E:\ERSEMEwECoupler")
        End Try
        Try
            If regkey2.GetValue("File", False) <> Nothing Then
                Me.TextBox12.Text = regkey2.GetValue("File")
            End If
        Catch
            Me.TextBox12.Text = TestDataPath + "\Results.nc"
        End Try
        ncfchanged = False
        'OpenFileDialog1.InitialDirectory = System.Environment.GetEnvironmentVariable("COUPLERPATH") + "\Testdata"
        'Me.TextBox2.Text = TestDataPath
        OpenFileDialog1.InitialDirectory = TestDataPath
        OpenFileDialog1.Filter = "Python Files (*.py)|*.py|Python Compile Files (*.pyc)|.pyc|xml Interface specification(*.xml)|.xml|All files (*.*)|*.*"
        OpenFileDialog1.FileName = TestDataPath + "\GOTMEWELink4.xml"
        OpenFileDialog1.FilterIndex = 3
        OpenFileDialog1.RestoreDirectory = True
        TextBox1.Text = OpenFileDialog1.FileName()
        hasrun = False
        ' Add any initialization after the InitializeComponent() call.
        m_core = Core
        sta1 = New stadelegate(AddressOf Status)
        maxbufferlen = 20
        currbufferlen = 1
        ReDim stbuf(maxbufferlen + 1)
    End Sub

    Public Sub StoreInRegistry()
        Dim regkey1, regkey2 As RegistryKey

        If ncfchanged Then
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", True)
            regkey1 = regkey1.OpenSubKey("EwE", True)
            regkey1 = regkey1.OpenSubKey("CouplerLib", True)
            Try
                regkey2 = regkey1.OpenSubKey("NetCDF", True)
                regkey2.SetValue("File", TextBox12.Text)
            Catch
                regkey2 = regkey1.CreateSubKey("NetCDF")
                regkey2.SetValue("File", TextBox12.Text)
            End Try

        End If


    End Sub
    Public Sub Status()
        Invoke(sta1, plugin.GOTMstat, plugin.EwEstat)
    End Sub

    Public Sub Status(ByVal GS As Integer, ByVal ES As Integer)
        TextBox4.Text = StatusT(GS)
        TextBox3.Text = StatusT(ES)
        Update()
    End Sub

    Public Sub Timedisplay(ByVal nn As Integer)
        Dim ayears As String = "EwE will run for " + nn.ToString + " years"
        TextBox2.Text = ayears
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.OpenFileDialog1.ShowDialog()
        TextBox1.Text = OpenFileDialog1.FileName()


    End Sub


    Private Sub gotmremoteserver()
        FileString = OpenFileDialog1.FileName
        Dim outgoingmessage As Cprotmessage
        Dim gotmessage As Boolean
        Connectname = New String("LOWAPP25")
        stationno = cp.ps.AddStation(Connectname)
        cp.outputmessage.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), True)
        cp.errormessage.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), True)
        dictiname = Path.GetDirectoryName(TextBox1.Text)
        thread.Sleep(50)
        If (stationno > 0) Then
            cp.ps.Establishcomms(stationno, New String("HOST"))
            gotmessage = cp.ps.pollevent(maprotocols.Ack_Opencoupler, 10000, False)
            If gotmessage Then
                cp.ps.Setstagestatus(maprotocols.Ack_Opencoupler, cp.ps.pollmessage.sc)
                If cp.ps.pollmessage.sc = mastatuscodes.Ok Then
                    sockettext = cp.ps.pollmessage.getmessage
                    Me.Invoke(New FormGotmPluggin.InvokeSDelegate(AddressOf SocketProgressText))
                    outgoingmessage = New Cprotmessage(stationno, maprotocols.Establishxmllocation, mastatuscodes.Notdetermined, FileString)
                    cp.ps.SndMessage(outgoingmessage, False, 1)
                    gotmessage = cp.ps.pollevent(maprotocols.Ack_Establishxmllocation, -1, False)
                    cp.ps.Setstagestatus(maprotocols.Ack_Establishxmllocation, cp.ps.pollmessage.sc)
                    If gotmessage Then
                        sockettext = cp.ps.pollmessage.getmessage()
                    Else
                        sockettext = "INVALID XML ERROR"
                    End If
                Else
                    sockettext = "INVALID STATUS ERROR"
                End If
            Else
                sockettext = "NO REPLY ERROR"
            End If
        Else
            sockettext = "UNKNOWN HOST ERROR"
        End If
        Me.Invoke(New FormGotmPluggin.InvokeSDelegate(AddressOf SocketProgressText))


    End Sub




    Private Sub gotmserver()
        'Dim i As Integer
        'Dim a As Integer
        'Dim tr As Integer
        Dim novars As Integer
        Dim gotmdir As String
        Dim regkey1, regkey2 As RegistryKey
        Dim datapath, py2path, svtemp As String
        'Dim svnames As List(Of String)
        'Dim svvalues As List(Of String)
        Dim lock As System.IntPtr
        If Not isclicked Then
            isclicked = True
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", False)
            regkey1 = regkey1.OpenSubKey("EwE", False)
            regkey2 = regkey1.OpenSubKey("CouplerLib", False)
            TestDataPath = regkey2.GetValue("COUPLERPATH") + "\Testdata"
            'TestDataPath = System.Environment.GetEnvironmentVariable("COUPLERPATH") + "\Testdata"
            datapath = ""
            FileString = OpenFileDialog1.FileName
            PathName = "chdir(""" + Path.GetDirectoryName(FileString) + """)"
            Shortpathname = Path.GetDirectoryName(FileString)
            FileName = "execfile(""" + Path.GetFileName(FileString) + """)"
            For i = 0 To Shortpathname.Length - 1
                datapath += Shortpathname.Chars(i)
                If (Shortpathname.Chars(i) = "\") Then
                    datapath += "\"
                End If
            Next
            py2path = ";" + Shortpathname + "\xmlplot;"
            py2path += Shortpathname + "\core;"
            datapath += ";"
            cp.Initialize(Shortpathname, Shortpathname + "\meecedict.xml", OpenFileDialog1.FileName)
            Dim currentDomain As AppDomain = AppDomain.CurrentDomain
            currentDomain.SetData("COUPLERLIBRRCACHE", cp)
            'novars = cp.GetVariableValues(svnames, svvalues)
            If (Not hasrun) Then
                If Not usenetCDFfile Then
                    Dim rt As Integer
                    PythonEngine.Initialize()
                    threadstate = PythonEngine.BeginAllowThreads()
                    lock = PythonEngine.AcquireLock()

                    Dim sysmodule As PyObject, pyresult As PyObject
                    sysmodule = PythonEngine.ImportModule("sys")
                    gotmdir = regkey2.GetValue("GOTMDIR").ToString()
                    pyresult = sysmodule.GetAttr("path").InvokeMethod("append", New PyString(gotmdir + "\gui.py"))
                    'System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + System.Environment.GetEnvironmentVariable("GOTMDIR") + "\gui.py")
                    System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + gotmdir + "\gui.py")
                    'System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + "C:\Python26\DLLs")
                    System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH")) ' + ";" + gotmdir + " C:\Python26\DLLs")
                    svtemp = System.Environment.GetEnvironmentVariable("PATH")
                    Dim corescenario As PyObject
                    corescenario = PythonEngine.ImportModule("core.scenario")
                    corescenario.SetAttr("schemadir", New PyString(gotmdir + "\gui.py\Schemas\scenario"))
                    Me.scenario = corescenario.GetAttr("Scenario").InvokeMethod("fromSchemaName", New PyString("gotmgui-0.5.0"))
                    Me.scenario.InvokeMethod("loadAll", New PyString(TestDataPath + "\linuxOG11dj.gotmscenario"))
                    Dim scenariobuilder, newscenario As PyObject
                    scenariobuilder = PythonEngine.ImportModule("scenariobuilder")
                    newscenario = scenariobuilder.InvokeMethod("loadScenario")
                    netcdfmodule = PythonEngine.ImportModule("NetCDF")


                    REM If the user cancelled loading a new scenario, just keep the old one.
                    If (newscenario.IsTrue()) Then
                        scenario = newscenario
                    End If

                    corescenario.Dispose()
                    PythonEngine.ReleaseLock(lock)


                    REM lock = Python.Runtime.PythonEngine.AcquireLock()
                    REM Pyos = Python.Runtime.PythonEngine.ImportModule("os")
                    REM Pysys = Python.Runtime.PythonEngine.ImportModule("sys")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString("import PyQt4.Qt")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString("from os import chdir")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString("from sys import path")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString("from os import path as ospath")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString(pypath)
                    REM a = Python.Runtime.PythonEngine.RunSimpleString("path+=ospath.curdir")
                    REM a = Python.Runtime.PythonEngine.RunSimpleString(FileName)
                    REM hasrun = True
                Else
                    cp.SetEndDate(DateTimePicker2.Value())
                    cp.SetStartDate(DateTimePicker1.Value())
                    plugin.gearratio = NumericUpDown1.Value
                End If
            End If
            dictiname = Path.GetDirectoryName(TextBox1.Text)
            'fs = File.Create(TestDataPath + "\bfmoutputs.txt")
            wx4 = wx5 = wx6 = False
            isedited = False
            isnotended = True

            While isnotended
                Threading.Thread.Sleep(1)
                If wx4 Then
                    Edit()
                    wx4 = False
                End If
                If wx5 Then
                    Outersimulate()
                    wx5 = False
                End If
                If wx6 Then
                    ViewResults()
                    wx6 = False
                End If
            End While
            plugin.EwEstat = 0
            plugin.GOTMstat = 0
            'fs.Close()
        End If
        isclicked = False

    End Sub

    Delegate Sub Runroutine()

    Sub Edit()
        b3click(True)
        isedited = True
    End Sub



    Sub Outersimulate()
        If Not isedited Then
            b3click(False)
        End If
        plugin.ModelLoaded(CheckBox2.Checked)
        simulate()
    End Sub

    Sub ViewResults()
        plugin.wait(wx6, False)
        b5click()


    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        cp = New CCouplerlib(RadioButton2.Checked, True, False, RadioButton3.Checked)
        isinitialized = True
        Dim conob, errob As Object

        If (RadioButton2.Checked) Then
            Me.thread = New Thread(AddressOf Me.gotmremoteserver)

        Else

            conob = New String("ConsolePipe")
            errob = New String("ErrorPipe")
            cp.pp1.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), False, Nothing)
            cp.pp2.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), False, Nothing)
            Me.pipethreadc = New Thread(AddressOf cp.pp1.pipeserver)
            Me.pipethreade = New Thread(AddressOf cp.pp2.pipeserver)
            pipethreadc.Start()
            pipethreade.Start()
            Me.thread = New Thread(AddressOf Me.gotmserver)
        End If
        Threading.Thread.Sleep(2000)
        cp.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), True)
        cp.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), False)
        Me.thread.Start()


    End Sub



    Delegate Sub InvokeDelegate()
    Delegate Sub InvokeSDelegate()
    Delegate Sub InvokeConDelegate(ByVal ists As String())
    Delegate Sub InvokeErrDelegate()

    Private Sub simulate()
        Dim lock As IntPtr
        Dim a As Integer
        Dim isok As Boolean
        Dim interleave As Integer
        Dim eisended As Boolean
        Dim hasmore, canrun, usespatial As Boolean
        Dim realdays, extraday As Double
        Dim simulator, simulatorobject As PyObject
        Dim bioinfo As PyTuple
        Dim modelio, modelii, modelno, modelni, modeliox, modelnix, modeliix, modelnox As List(Of Integer)
        Dim names, units, abbrev, names2, units2, abbrev2 As String()
        Dim linkn, linkp, links, isc, isorg As Integer()
        Dim itest As Integer
        Dim lmname As String
        Dim ncratio, pcratio, scratio, pextran, pextrap, pextras, adjn, adjp, adjs, cadj As Double
        Dim noi, ref As Integer
        Dim depth, dp, adjpool, adjmult, temperature, salinity As Double
        Dim adepth, adp, aadjpool, aadjmult, atemperature, asalinity As Double()
        Dim bstart, bensplit As Integer
        Dim nobenthic, nopelagic As Integer
        Dim biovals, biovalsben As PyObject
        Dim vals, benvals, oldvals As Double()
        Dim avals, abenvals, aoldvals As Double()()
        Dim curtime As Double
        Dim nodims, novars As Integer
        Dim nodataelements, nospatialelements As Integer
        Dim nototdataelements As Integer
        Dim Pelflag, benflag As PyInt
        Dim valarray As Double()
        Dim ovalarray As Double()
        Dim unitsf, units2f As Integer()
        usespatial = CheckBox1.Checked()
        If usenetCDFfile Then

            isok = cp.GetInfoFromnetCDF(TextBox12.Text, nodims, novars, isthreed)
            If (isok) Then
                Dim pelagic(4) As String
                pelagic(0) = "lon"
                pelagic(1) = "lat"
                pelagic(2) = "time"
                If isthreed Then
                    pelagic(3) = "z"
                Else
                    pelagic(3) = "!z"
                End If
                Dim benthic(4) As String
                benthic(0) = "lon"
                benthic(1) = "lat"
                benthic(2) = "time"
                benthic(3) = "!z"
                Dim bathymet(4) As String
                bathymet(0) = "lon"
                bathymet(0) = "lat"
                Dim loninfo() As String = {"lon", "!lat", "!time", "!z"}
                Dim latinfo() As String = {"lat", "!lon", "!time", "!z"}
                Dim depthinfo() As String = {"z", "!lon", "!lat", "!time"}
                Dim bathyinfo() As String = {"lon", "lat", "!time", "!z"}
                cp.ScreennetCDFDimensions(4, loninfo, 2, 6)
                cp.ScreennetCDFDimensions(4, latinfo, 3, 6)
                cp.ScreennetCDFDimensions(4, depthinfo, 4, 6)
                cp.ScreennetCDFDimensions(4, bathyinfo, 5, 6)
                cp.GetIndex(2, 0, "lon")
                alongitude = cp.GetnetCDFvalue(0.0, 2, 0, True, False)
                cp.GetIndex(3, 0, "lat")
                alatitude = cp.GetnetCDFvalue(0.0, 3, 0, True, False)
                If isthreed Then
                    cp.GetIndex(4, 0, "z")
                    azed = cp.GetnetCDFvalue(0.0, 4, 0, True, False)
                End If
                nopelagic = cp.ScreennetCDFDimensions(4, pelagic, 0, 6)
                If isthreed Then
                    nobenthic = cp.ScreennetCDFDimensions(4, benthic, 1, 6)
                Else
                    nobenthic = 0
                End If
                abbrev = cp.GetnetCDFvarnames(0)
                names = cp.GetnetCDFvarattributes(0, "long_name")
                units = cp.GetnetCDFvarattributes(0, "units")
                ReDim unitsf(units.Length + 2)
                For n = 0 To units.Length - 1
                    If (isthreed And units(n) = "mg C/m^3") Then
                        unitsf(n) = 1
                    Else
                        unitsf(n) = 0
                    End If
                Next
                nodataelements = abbrev.Length
                If isthreed Then
                    abbrev2 = cp.GetnetCDFvarnames(1)
                    names2 = cp.GetnetCDFvarattributes(1, "long_name")
                    units2 = cp.GetnetCDFvarattributes(1, "units")
                    nobenthic = abbrev2.Length
                    ReDim units2f(units2.Length + 2)
                    For n = 0 To units.Length - 1
                    Next
                End If
                ReDim Preserve abbrev(nodataelements + nobenthic + 3)
                ReDim Preserve names(nodataelements + nobenthic + 3)
                ReDim Preserve units(nodataelements + nobenthic + 3)
                If isthreed Then
                    Array.Copy(abbrev2, 0, abbrev, nodataelements + 3, nobenthic)
                    Array.Copy(names2, 0, names, nodataelements + 3, nobenthic)
                    Array.Copy(units2, 0, units, nodataelements + 3, nobenthic)
                End If
            End If
        Else

            lock = PythonEngine.AcquireLock()




            REM  Create the simulator in Python.
            'a = PythonEngine.RunSimpleString("import os")
            'a = PythonEngine.RunSimpleString("os.chdir(""c:\\ewecefas\\gotm\\gui.py"")")
            'a = PythonEngine.RunSimpleString("import simulator")
            simulatorobject = PythonEngine.ImportModule("core.simulator")
            simulator = simulatorobject.InvokeMethod("Simulator", scenario)

            REM At this point GOTM has initialized and everything is known, including the names, units etc.
            REM  of the biological
            REM String xml = scenario.InvokeMethod("describe")
            'itest = simulator.InvokeMethod("getBioVariableInfo").AsManagedObject(Type.GetType("System.Integer"))
            bioinfo = New PyTuple(simulator.InvokeMethod("getBioVariableInfo"))
            abbrev = DirectCast(bioinfo(0).AsManagedObject(Type.GetType("System.String[]")), String())
            names = DirectCast(bioinfo(1).AsManagedObject(Type.GetType("System.String[]")), String())
            units = DirectCast(bioinfo(2).AsManagedObject(Type.GetType("System.String[]")), String())
            For i = 0 To abbrev.Length - 1
                If abbrev(i) = "Y1c" Then bstart = i
            Next
            nototdataelements = 172

        End If


        adjpool = 1.0
        REM nodataelements = DirectCast(bioinfo(3).AsManagedObject(Type.GetType("System.Integer")), Int32)

        npool = 0.0
        cpool = 0.0

        ppool = 0.0
        spool = 0.0
        If usenetCDFfile Then
            nodataelements = nopelagic + 3
            nototdataelements = nobenthic + nopelagic + 3
        Else

            Pelflag = New PyInt(1)
            benflag = New PyInt(0)
            biovals = simulator.InvokeMethod("getBioValues", Pelflag)
            biovalsben = simulator.InvokeMethod("getBioValues", benflag)
            vals = biovals.AsManagedObject(Type.GetType("System.Double[]"))
            benvals = biovalsben.AsManagedObject(Type.GetType("System.Double[]"))
            For i = 0 To benvals.Length
                abbrev(i + vals.Length) = abbrev(bstart + i)
                names(i + vals.Length) = names(bstart + i)
                units(i + vals.Length) = units(bstart + i)
            Next
            nodataelements = vals.Length
            nototdataelements += benvals.Length + 3
        End If
        ReDim Preserve abbrev(nototdataelements)
        ReDim Preserve names(nototdataelements)
        ReDim Preserve units(nototdataelements)
        ReDim linkn(nototdataelements)
        ReDim linkp(nototdataelements)
        ReDim links(nototdataelements)
        ReDim isc(nototdataelements)
        ReDim isorg(nototdataelements)
        abbrev(nodataelements - 3) = "ETW"
        names(nodataelements - 3) = "Temperature"
        units(nodataelements - 3) = "C"
        cp.GetIndex(0, nodataelements - 3, "SST")
        abbrev(nodataelements - 2) = "ESW"
        names(nodataelements - 2) = "Salinity"
        units(nodataelements - 2) = "psu"
        cp.GetIndex(0, nodataelements - 2, "SSS")
        abbrev(nodataelements - 1) = "SDp"
        names(nodataelements - 1) = "Depth"
        units(nodataelements - 1) = "m"
        cp.GetIndex(0, nodataelements - 1, "bathy")
        abbrev(nodataelements) = "NUL"
        names(nodataelements) = "NULL"
        units(nodataelements) = "0"
        REM this is strictly speaking a GOTM call and so goes in this file
        lmname = Me.specifyGOTMXML(names, units, abbrev, linkn, linkp, links, isc, isorg)
        Dim compnames() As String = {"z", "lon", "lat", "n_faces", "n_zfaces"}
        Dim comptypes() As Integer = {4, 3, 3, 2, 2}
        Dim comdims() As Integer = {2, 0, 1, -1, -1}
        plugin.GOTMstat = 1 'Ready to go
        hasmore = True
        canrun = plugin.Starting(cp, NumericUpDown1.Value, False)
        If usespatial Then
            cp.SetCompressions(5, compnames, comptypes, comdims)
            ReDim avals(nodataelements)
            ReDim abenvals(nototdataelements - nodataelements)
        Else
            ReDim vals(nodataelements)
            ReDim benvals(nototdataelements - nodataelements)
        End If
        If (canrun) Then
            curtime = cp.GetStartTime(0)
            modelio = cp.GetIfAddress(modelno, lmname, False, False)
            modeliix = cp.GetIfAddress(modelnox, lmname, True, True)
            modelii = cp.GetIfAddress(modelni, lmname, False, True)
            modeliox = cp.GetIfAddress(modelnix, lmname, True, False)
            interleave = 0
            realdays = 0.0
            While hasmore



                REM Run a new slab.
                If usenetCDFfile Then
                    hasmore = (curtime < cp.GetEndTime(0))
                Else
                    hasmore = simulator.InvokeMethod("runSlab", New PyInt(slabsize)).IsTrue()
                End If

                If (hasmore = False) Then
                    Dim ad As Integer = 1
                End If
                If Not usenetCDFfile Then
                    If (eisended And ((interleave Mod EwEGOTMtimeratio) = 0)) Then
                        hasmore = False
                    End If
                End If
                REM Send fraction complete to progress bar.
                If usenetCDFfile Then
                    nprogress = cp.GetProgress(curtime, 0)
                    If (usespatial) Then
                        If Not plugin.autorescale Then
                            isrescale = False
                        End If
                        If isrescale Then
                            xdim = cp.Getxdim()
                            ydim = cp.Getydim()
                            If plugin.isrescale Then
                                For n = 0 To nodataelements + 1
                                    'ReDim avals(n)(xdim * ydim)
                                Next
                                'ReDim adepth(xdim * ydim)
                                'ReDim atemperature(xdim * ydim)
                                'ReDim asalinity(xdim * ydim)
                                nospatialelements = xdim * ydim
                                plugin.isrescale2 = True
                                For n = 0 To nobenthic
                                    ' ReDim abenvals(n)(xdim * ydim)
                                Next
                            End If

                        End If
                        adepth = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, nodataelements - 1, False, 0)
                        'For n = 0 To nospatialelements - 1
                        'adepth(n) *= 1.0
                        'Next
                        atemperature = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, nodataelements - 3, False, 2)
                        'For n = 0 To nospatialelements - 1
                        'atemperature(n) /= 40.0
                        'Next
                        asalinity = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, nodataelements - 2, False, 1)
                        'For n = 0 To nospatialelements - 1
                        'asalinity(n) /= 40.0
                        'Next
                    Else
                        depth = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 5, nodataelements - 1, True, 0)(0) * -1.0
                        temperature = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, nodataelements - 3, False, 2)(0)
                        salinity = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, nodataelements - 2, False, 1)(0)
                    End If
                    For n = 0 To nodataelements - 4
                        If (usespatial) Then
                            avals(n) = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, n, False, 1)
                            If unitsf(n) = 1 Then
                                For j = 0 To nospatialelements - 1
                                    avals(n)(j) *= adepth(j)
                                Next
                            End If
                        Else
                            vals(n) = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, n, False, 1)(0)
                        End If

                    Next
                    For n = nodataelements To nototdataelements - 1
                        If usespatial Then
                            abenvals(n - nodataelements) = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, n, False, 0)

                        Else
                            benvals(n - nodataelements) = cp.GetnetCDFvalue((curtime - timebase) * 24.0 * 60.0 * 60.0, 0, n, False, 0)(0)
                        End If

                    Next
                    bensplit = nodataelements
                Else
                    nprogress = simulator.InvokeMethod("getProgress").AsManagedObject(Type.GetType("System.Single"))
                    depth = simulator.InvokeMethod("getDepth").AsManagedObject(Type.GetType("System.Double"))
                    temperature = simulator.InvokeMethod("getTemperature").AsManagedObject(Type.GetType("System.Double"))
                    salinity = simulator.InvokeMethod("getSalinity").AsManagedObject(Type.GetType("System.Double"))
                    REM Get a string describing the current depth-integrated bio values.
                    biovals = simulator.InvokeMethod("getBioValues", Pelflag)
                    biovalsben = simulator.InvokeMethod("getBioValues", benflag)
                    vals = biovals.AsManagedObject(Type.GetType("System.Double[]"))
                    bensplit = vals.Length
                    benvals = biovalsben.AsManagedObject(Type.GetType("System.Double[]"))
                    nodataelements = bensplit + benvals.Length
                End If
                If plugin.autorescale Then
                    isrescale = cp.CheckRescale(timebase, nodataelements, names, 0, curtime, habitatarray, xdim, ydim)
                    plugin.rexdim = cp.Getxdim()
                    plugin.reydim = cp.Getydim() + 1
                    If isrescale Then
                        plugin.isrescale = True
                    End If
                End If
                If (usespatial) Then
                    ReDim Preserve avals(nodataelements + 2)
                    'ReDim Preserve aoldvals(nodataelements + 2)
                    avals(nodataelements - 3) = atemperature
                    avals(nodataelements - 2) = asalinity
                    avals(nodataelements - 1) = adepth
                    For i = bensplit To avals.Length - 4
                        avals(i) = abenvals(i - bensplit)
                    Next
                    If (isrescale) Then
                        If plugin.isrescale2 Then
                            cp.EvalHabitat(avals, bensplit, xdim * ydim)
                        End If
                    End If
                    ' For n = 0 To nospatialelements


                    '                    Next
                Else
                    ReDim Preserve vals(nodataelements + 2)
                    ReDim Preserve oldvals(nodataelements + 2)
                    For i = bensplit To vals.Length - 4
                        vals(i) = benvals(i - bensplit)
                    Next
                    vals(nodataelements - 3) = temperature
                    vals(nodataelements - 2) = salinity
                    vals(nodataelements - 1) = depth


                End If

                If (((interleave Mod EwEGOTMtimeratio) = 0) And (interleave >= spinupdays) And (extraday < 1)) Or (Not hasmore) Then

                    For i As Integer = 0 To modelio.Count - 1
                        If (usespatial) Then
                            cp.PutIf(modelno(i), modelio(i), avals, nodataelements, nospatialelements)

                        Else
                            cp.PutIf(modelno(i), modelio(i), vals, nodataelements)
                        End If
                    Next

                    biotext = ""
                    Dim vtot As Single
                    Dim vcount As Single
                    If (usespatial) Then
                        For i As Integer = 0 To avals.Length - 4
                            vtot = 0.0
                            vcount = 0.0
                            For j As Integer = 0 To nospatialelements - 1
                                If adepth(j) > 0 Then
                                    vtot += avals(i)(j)
                                    vcount += 1.0
                                End If
                            Next
                            biotext = biotext + names(i) + " = " + (vtot / vcount).ToString() + " " + units(i) + vbCrLf
                        Next

                    Else
                        For i As Integer = 0 To vals.Length - 3
                            biotext = biotext + names(i) + " = " + vals(i).ToString() + " " + units(i) + vbCrLf
                        Next

                    End If

                End If
                Dim newvals As Double()
                Dim anewvals As Double()()
                If usespatial Then
                    anewvals = avals
                Else
                    newvals = vals
                End If

                If (usespatial) Then
                    ReDim aoldvals(avals.Length - 1)
                    For i = 0 To avals.Length - 4
                        ReDim aoldvals(i)(nospatialelements)
                        For n = 0 To nospatialelements - 1
                            aoldvals(i)(n) = avals(i)(n)
                        Next n
                    Next i


                Else
                    For i = 0 To vals.Length - 1
                        oldvals(i) = vals(i)
                    Next i

                End If
                If ((interleave Mod EwEGOTMtimeratio) = 0) And (interleave >= spinupdays) And (extraday < 1) Then
                    eisended = plugin.runstep()

                End If
                extraday = Convert.ToInt32(realdays - interleave)
                If extraday = 1 Then
                    Int(a = 1)
                End If

                If (interleave >= spinupdays) Then
                    If extraday < 1 Then
                        realdays += 365.25 / 360.0
                        If Not usenetCDFfile Then
                            For i As Integer = 0 To modelii.Count - 1
                                If ((interleave Mod EwEGOTMtimeratio) = 0) Then
                                    noi = cp.GetIf(modelnix(i), modeliox(i), modelni(i), modelii(i), modelnox(i), modelio(i), valarray)
                                End If
                                Dim d As Integer
                                For j = 0 To noi - 1

                                    d = cp.OrgReference(modelnix(i), modeliox(i), j)
                                    If isorg(d) = 1 Then
                                        adjmult = adjpool
                                    Else
                                        adjmult = 1.0
                                    End If
                                    If (valarray(j) < 0) Then
                                        newvals(cp.OrgReference(modelnix(i), modeliox(i), j)) += adjmult * (-valarray(j)) * newvals(cp.OrgReference(modelnix(i), modeliox(i), j))
                                    Else
                                        newvals(cp.OrgReference(modelnix(i), modeliox(i), j)) -= adjmult * (valarray(j)) * newvals(cp.OrgReference(modelnix(i), modeliox(i), j))

                                    End If
                                    If (newvals(cp.OrgReference(modelnix(i), modeliox(i), j)) < 0.0) Then
                                        d = -1
                                    End If
                                    REM newvals(j) = vals(j)

                                Next
                                cflux = nflux = pflux = 0.0
                                For j = 0 To oldvals.Length - 4
                                    If j > bensplit Then
                                        dp = 1.0
                                    Else
                                        dp = depth
                                    End If
                                    If (isc(j) = 1) Then
                                        cpool -= dp * (newvals(j) - oldvals(j))
                                        cflux += Math.Abs(dp * (newvals(j) - oldvals(j)))
                                        If (linkn(j) > -1) Then
                                            npool -= dp * (newvals(linkn(j)) - oldvals(linkn(j)))
                                            nflux += Math.Abs(dp * (newvals(linkn(j)) - oldvals(linkn(j))))
                                        End If
                                        If (linkp(j) > -1) Then
                                            ppool -= dp * (newvals(linkp(j)) - oldvals(linkp(j)))
                                            pflux += Math.Abs(dp * (newvals(linkp(j)) - oldvals(linkp(j))))
                                        End If
                                        If (links(j) > -1) Then
                                            spool -= dp * (newvals(links(j)) - oldvals(links(j)))
                                            sflux += Math.Abs(dp * (newvals(links(j)) - oldvals(links(j))))
                                        End If
                                    End If
                                Next
                                ncratio = 0.015
                                pcratio = 0.00167
                                scratio = 0.00167
                                pextran = npool - cpool * ncratio
                                pextrap = ppool - cpool * pcratio
                                pextras = spool - cpool * scratio
                                Dim adjntot As Double = 0.0
                                For j = 0 To oldvals.Length - 4
                                    If j > bensplit Then
                                        dp = 1.0
                                    Else
                                        dp = depth
                                    End If
                                    If (isc(j) = 1) Then
                                        If (linkn(j) > -1) And (isorg(j) = 1) Then
                                            adjn = pextran * Math.Abs(dp * (newvals(linkn(j)) - oldvals(linkn(j)))) / nflux
                                            newvals(linkn(j)) += adjn / dp
                                            npool -= adjn
                                            adjntot += adjn
                                        End If
                                        If (linkp(j) > -1) And (isorg(j) = 1) Then
                                            adjp = pextrap * Math.Abs(dp * (newvals(linkp(j)) - oldvals(linkp(j)))) / pflux
                                            newvals(linkp(j)) += adjp / dp
                                            ppool -= adjp
                                        End If
                                        If (links(j) > -1) And (isorg(j) = 1) Then
                                            adjs = pextras * Math.Abs(dp * (newvals(links(j)) - oldvals(links(j)))) / sflux
                                            newvals(links(j)) += adjs / dp
                                            spool -= adjs
                                        End If
                                    End If
                                Next
                                adjpool = 1 / (1 - cpool / 1000.0)

                                For j As Integer = 0 To newvals.Length - 1
                                    If Single.IsNaN(newvals(j)) Or newvals(j) < 0 Then
                                        Dim g As Integer = 1
                                    End If
                                Next

                                REM Hack to stop underflow problem when RE goes to zero with values ging via Python
                                If (newvals(7) < 0.01) Then
                                    newvals(7) = 0.01
                                End If
                                Dim pynewvals As PyObject()
                                pynewvals = New PyObject(bensplit - 1) {}
                                For j As Integer = 0 To bensplit - 1
                                    pynewvals(j) = New PyFloat(newvals(j))
                                Next
                                Dim cv As PyObject
                                simulator.InvokeMethod("setBioValues", New PyList(pynewvals))
                                ReDim pynewvals(newvals.Length - bensplit - 4)
                                For j As Integer = bensplit To newvals.Length - 4
                                    pynewvals(j - bensplit) = New PyFloat(newvals(j))
                                Next
                                simulator.InvokeMethod("setBioValuesBenthic", New PyList(pynewvals))
                            Next
                        End If
                    End If
                Else
                    realdays += 1.0
                End If
                curtime += 1.0
                interleave += 1
            End While
            plugin.isover()
            hasrun = True
            REM Clean up after the run and obtain the result.
            If usenetCDFfile Then
            Else
                result = simulator.InvokeMethod("finalize")

                Dim errmsg As String
                errmsg = result.GetAttr("errormessage").AsManagedObject(Type.GetType("System.String"))
                Dim corescenario As PyObject
                corescenario = PythonEngine.ImportModule("core.scenario")
                REM corescenario.SetAttr("Scenario", corescenario)
                REM corescenario.GetAttr("Scenario").InvokeMethod("saveAll", scenario, New PyString("E:\\gotm\\gui.py\\tempx"))
                REM errmsg = result.GetAttr("errormessage").AsManagedObject(Type.GetType("System.String"))
                'fs.Close()
            End If
        End If
        If usenetCDFfile Then
        Else

            PythonEngine.ReleaseLock(lock)
        End If

    End Sub
    Public Sub Progressbar()
        Me.ProgressBar1.Value = Int(Math.Round(nprogress * 100))
        Me.Update()
    End Sub

    Public Sub Progresstext()
        Me.TextBox5.Text = biotext
        If (biotext <> Nothing) Then
            'AddText(fs, biotext)
        End If
        TextBox7.Text = Convert.ToString(cpool)
        TextBox8.Text = Convert.ToString(npool)
        TextBox9.Text = Convert.ToString(ppool)
        TextBox10.Text = Convert.ToString(spool)
        Me.Update()
    End Sub

    Public Sub SocketProgressText()
        Me.TextBox5.Text = sockettext
        Me.Update()
    End Sub

    Public Sub ConsoleOutputText(ByVal inputtext As Object)
        Dim inputtexts As String
        If currbufferlen = maxbufferlen - 1 Then
            For n As Integer = 0 To maxbufferlen - 2 Step 1
                Me.stbuf(n) = Me.stbuf(n + 1)
            Next
        Else
            currbufferlen += 1
        End If
        inputtexts = inputtext.ToString
        inputtexts = inputtexts.Remove(inputtexts.Length - 1, 1)
        stbuf(currbufferlen - 2) = inputtext
        Me.Invoke(New FormGotmPluggin.InvokeConDelegate(AddressOf Conwrite), New Object() {stbuf})
        'Me.TextBox6.Lines

    End Sub

    Public Sub Conwrite(ByVal istr As String())
        TextBox6.Lines = istr
    End Sub

    Public Sub ErrorOutputText(ByVal inputtext As Object)
        Dim inputtexts As String
        If currbufferlen = maxbufferlen - 1 Then
            For n As Integer = 0 To maxbufferlen - 2 Step 1
                Me.stbuf(n) = Me.stbuf(n + 1)
            Next
        Else
            currbufferlen += 1
        End If
        inputtexts = inputtext.ToString
        inputtexts = inputtexts.Remove(inputtexts.Length - 1, 1)
        stbuf(currbufferlen - 2) = inputtexts
        Me.TextBox6.Lines = stbuf
    End Sub



    Private Sub TimeSpecify(ByVal sttime As String, ByVal endtime As String, ByVal inttime As Integer)
        Specification = New Xml.XmlDocument
        Dim NL, CL, GCL As Xml.XmlNodeList
        Dim Node As Xml.XmlNode
        Dim ts As TimeSpan

        'Specification.Load("C:\ewecefas\gotm\gui.py\gotmtemplate.xml")
        Specification.Load(TestDataPath + "\GOTMtemplate.xml")
        NL = Specification.GetElementsByTagName("TimeDimensionTemporal")
        For n As Integer = 0 To NL.Count - 1
            Node = NL(n)
            CL = Node.ChildNodes
            For m As Integer = 0 To CL.Count - 1
                If (CL(m).Name = "StartTime") Then
                    CL(m).InnerText = sttime
                End If
                If (CL(m).Name = "EndTime") Then
                    CL(m).InnerText = endtime
                End If
                If (CL(m).Name = "Interval") Then
                    GCL = CL(m).ChildNodes
                    If (GCL.Count = 1) Then
                        REM If (GCL(0).Name = "Duration") Then
                        REM ts = New TimeSpan(0, 0, inttime)
                        REM GCL(0).InnerText = ts.ToString
                        REM End If
                    End If
                End If
            Next
        Next
    End Sub

    Public Sub ClosePipes()
        If (isinitialized) Then

            cp.pp1.endpipe()
            cp.pp2.endpipe()
        End If


    End Sub



    Private Function specifyGOTMXML(ByVal ivariables As String(), ByVal iunits As String(), ByVal iabbrev As String(), ByRef ilkn As Integer(), ByRef ilkp As Integer(), ByRef ilks As Integer(), ByRef isc As Integer(), ByRef iisorg As Integer()) As String
        Dim NL, clx, gclx As Xml.XmlNodeList
        Dim cl, cl2, m, nodims As Integer
        Dim NewNode, NewChild As Xml.XmlNode
        Dim dt, ftype, tabrev As String
        Dim lonstart, latstart, longint, latint As Double
        Dim longsz, latsz As Integer
        Dim NodeName As String() = {"Longitude", "Latitude", "Depth"}
        Dim FgType As String() = {"C", "N", "Si", "O", "P", "C"}
        Dim Fgstr As String() = {"c", "n", "s", "o", "p"}
        Dim GroupType As String() = {"Phytoplankton", "Zooplankton", "Detritus", "Bacteria", "Consumer", "Detritus", "Bacteria", "Nutrient", "Nutrient", "Nutrient", "StateVariables", "StateVariables", "Other"}
        Dim Abbrevtype As String() = {"P", "Z", "R", "B", "Y", "Q", "H", "K", "N", "O", "D", "E"}
        Dim Node, Child, GChild, GGChild, GGGChild As Xml.XmlNode
        cp.SetDimNames(NodeName)
        NL = Specification.GetElementsByTagName("ModelName")
        Dim mname As String
        mname = NL(0).InnerText
        NL = Specification.GetElementsByTagName("Interface")
        For n = 0 To NL.Count - 1
            Node = NL(n).ChildNodes(4)  'Specification.GetElementsByTagName("GridData")
            clx = Node.ChildNodes
            If ((clx(0).Name = "GridFormNone" And isthreed) Or (clx(0).Name = "GridFormRaster3D")) Then
                Node.RemoveChild(clx(0))
                NewNode = Specification.CreateElement("GridFormRaster3D")
                NewNode.InnerText = "3D"
                Node.AppendChild(NewNode)
            End If
            If ((clx(0).Name = "GridFormNone" And Not isthreed) Or (clx(0).Name = "GridFormRaster2D")) Then
                Node.RemoveChild(clx(0))
                NewNode = Specification.CreateElement("GridFormRaster2D")
                NewNode.InnerText = "2D"
                Node.AppendChild(NewNode)
            End If
            If isthreed Then
                nodims = 3
            Else
                nodims = 2
            End If
            For m = 1 To clx.Count - 1
                Node.RemoveChild(clx(m))
            Next
            For m = 1 To nodims
                NewNode = Specification.CreateElement(NodeName(m - 1))
                NewChild = Specification.CreateElement("Minimum")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(alongitude(0))
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(alatitude(0))
                End If
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString(azed(0) / azed.Length())
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Interval")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(alongitude(1) - alongitude(0))
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(alatitude(1) - alatitude(0))
                End If
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString((azed(1) - azed(0)) / azed.Length())
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Length")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(alongitude.Length - 1)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(alatitude.Length - 1)
                End If
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString(azed.Length)
                End If
                NewNode.AppendChild(NewChild)
                Node.AppendChild(NewNode)
            Next

        Next
        NL = Specification.GetElementsByTagName("DataCollection")
        For n = 0 To NL.Count - 1
            Node = NL(n)
            For m = 0 To ivariables.Length - 1
                ilkn(m) = -1
                ilkp(m) = -1
            Next

            For m = 0 To ivariables.Length - 1
                cl = 12
                For k = 0 To Abbrevtype.Length - 1
                    If iabbrev(m).StartsWith(Abbrevtype(k)) Then
                        cl = k
                    End If
                Next
                Child = Specification.CreateElement("Data")
                GChild = Specification.CreateElement("Name")
                If (cl < 7) Then
                    If iabbrev(m).Substring(iabbrev(m).Length - 1, 1) = "D" Then
                        tabrev = iabbrev(m).Substring(0, iabbrev(m).Length - 2)
                    Else
                        tabrev = iabbrev(m).Substring(0, iabbrev(m).Length - 1)
                    End If
                    GChild.InnerText = tabrev
                Else
                    GChild.InnerText = iabbrev(m)
                End If
                    Child.AppendChild(GChild)
                    GChild = Specification.CreateElement("DataItem")
                    GChild.InnerText = ""
                    GGChild = Specification.CreateElement(GroupType(cl))
                    GGGChild = Specification.CreateElement("Name")
                    GGGChild.InnerText = ivariables(m)
                    GGChild.AppendChild(GGGChild)
                    REM If ivariables(m).Length > 3 Then
                    REM .InnerText = ivariables(m).Substring(0, 3)
                    REM Else
                    REM GGGChild.InnerText = ivariables(m)
                    REM End If
                If (cl < 7) Then
                    If iabbrev(m).Substring(iabbrev(m).Length - 1, 1) = "D" Then
                        ftype = iabbrev(m).Substring(iabbrev(m).Length - 2, 1)
                    Else
                        ftype = iabbrev(m).Substring(iabbrev(m).Length - 1, 1)
                    End If
                    cl2 = 5
                    For k = 0 To Fgstr.Length - 1
                        If Fgstr(k) = ftype Then
                            cl2 = k
                        End If
                    Next
                    GGGChild = Specification.CreateElement("Constituent")
                    GGGChild.InnerText = FgType(cl2)
                    GGChild.AppendChild(GGGChild)
                    If (cl2 = 0) Then
                        isc(m) = 1
                    Else
                        isc(m) = 0
                    End If
                    If (cl = 1) Then
                        iisorg(m) = 2
                    Else
                        If (cl = 2 Or cl = 5) Then
                            iisorg(m) = 1
                        Else
                            iisorg(m) = 0
                        End If
                    End If
                    If (cl2 = 1) Then
                        For k = 0 To ivariables.Length - 1
                            If ((iabbrev(k).Substring(0, iabbrev(k).Length - 1) = tabrev) And (iabbrev(k).Substring(iabbrev(k).Length - 1, 1) = "c")) Then
                                ilkn(k) = m
                            End If
                        Next
                    End If
                    If (cl2 = 4) Then
                        For k = 0 To ivariables.Length - 1
                            If ((iabbrev(k).Substring(0, iabbrev(k).Length - 1) = tabrev) And (iabbrev(k).Substring(iabbrev(k).Length - 1, 1) = "c")) Then
                                ilkp(k) = m
                            End If
                        Next
                    End If
                    If (cl2 = 2) Then
                        For k = 0 To ivariables.Length - 1
                            If ((iabbrev(k).Substring(0, iabbrev(k).Length - 1) = tabrev) And (iabbrev(k).Substring(iabbrev(k).Length - 1, 1) = "c")) Then
                                ilks(k) = m
                            End If
                        Next
                    End If
                Else
                    GGGChild = Specification.CreateElement("Constituent")
                    GGGChild.InnerText = "U"
                    GGChild.AppendChild(GGGChild)
                    tabrev = iabbrev(m)
                End If

                    GGGChild = Specification.CreateElement("Symbol")
                    GGGChild.InnerText = tabrev
                    GGChild.AppendChild(GGGChild)
                    GGGChild = Specification.CreateElement("Description")
                    GGGChild.InnerText = ivariables(m) + " ERSEM Group"
                    GGChild.AppendChild(GGGChild)
                    GChild.AppendChild(GGChild)
                    Child.AppendChild(GChild)
                    GChild = Specification.CreateElement("Flux")
                    If n = 0 Then
                        GChild.InnerText = "State"
                    Else
                        GChild.InnerText = "Predation"
                    End If
                    Child.AppendChild(GChild)
                    GChild = Specification.CreateElement("Units")
                    If n = 0 Then
                        GChild.InnerText = iunits(m)
                    Else
                        GChild.InnerText = iunits(m) + "/Interval"
                    End If
                    Child.AppendChild(GChild)
                    Node.AppendChild(Child)
            Next
        Next
        REM Specification.Save("c:\ewecefas\gotm\gui.py\gotm.xml")
        Specification.Save(TestDataPath + "\GOTM.xml")
        Return (mname)

    End Function
    Private Sub b3click(ByVal carryoutedit As Boolean)
        Dim lock As IntPtr
        Dim scenariobuilder As PyObject
        Dim dt As Double
        Dim dstart, dend As Double
        Dim pydt, pyst, pyend, pyastart, pyaend As PyObject
        Dim dsstart, dsend As String
        If Not usenetCDFfile Then

            lock = PythonEngine.AcquireLock()
            If carryoutedit Then
                scenariobuilder = PythonEngine.ImportModule("scenariobuilder")
                scenariobuilder.InvokeMethod("editScenario", scenario)
            End If
            REM Determine the number of GOTM time steps in a month.
            pydt = scenario("timeintegration/dt").InvokeMethod("getValue").InvokeMethod("getAsSeconds")
            pyst = scenario("time/start").InvokeMethod("getValue").InvokeMethod("toordinal")
            pyend = scenario("time/stop").InvokeMethod("getValue").InvokeMethod("toordinal")
            pyastart = scenario("time/start").InvokeMethod("getValue").InvokeMethod("isoformat")
            pyaend = scenario("time/stop").InvokeMethod("getValue").InvokeMethod("isoformat")
            dt = Convert.ToDouble(pydt.AsManagedObject(Type.GetType("System.Double")))
            dstart = Convert.ToDouble(pyst.AsManagedObject(Type.GetType("System.Double")))
            dend = Convert.ToDouble(pyend.AsManagedObject(Type.GetType("System.Double")))
            dsstart = Convert.ToString(pyastart.AsManagedObject(Type.GetType("System.String")))
            dsend = Convert.ToString(pyaend.AsManagedObject(Type.GetType("System.String")))
            spinupdays = NumericUpDown2.Value * 365 + NumericUpDown3.Value * 30
            slabsize = plugin.setstep(Int(dend - dstart), dt, EwEGOTMtimeratio, False, 0, "host", dictiname, TextBox1.Text, NumericUpDown2.Value, NumericUpDown3.Value)
            Me.TimeSpecify(dsstart, dsend, Int(dt))
            PythonEngine.ReleaseLock(lock)
        Else
            spinupdays = NumericUpDown2.Value * 365 + NumericUpDown3.Value * 30
            dt = 24 * 60 * 60
            cp.SetEndDate(DateTimePicker2.Value())
            cp.SetStartDate(DateTimePicker1.Value())
            dstart = cp.GetStartTime(0)
            dend = cp.GetEndTime(0)
            dsstart = DateTimePicker1.Value().ToString("o")
            dsend = DateTimePicker2.Value().ToString("o")
            'slabsize = plugin.setstep(1200, 1, EwEGOTMtimeratio, False, 0, "host", dictiname, TextBox1.Text, NumericUpDown2.Value, NumericUpDown3.Value)
            slabsize = plugin.setstep(Int(dend - dstart), dt, EwEGOTMtimeratio, False, 0, "host", dictiname, TextBox1.Text, NumericUpDown2.Value, NumericUpDown3.Value)
            Me.TimeSpecify(dsstart, dsend, Int(dt))
        End If
        Status()

    End Sub
    Private Sub remoteedit()
        Dim outgoingmessage As Cprotmessage
        Dim gotmessage As Boolean
        If (cp.ps.Getstagestatus(maprotocols.Ack_Establishxmllocation) = mastatuscodes.Ok) Then
            outgoingmessage = New Cprotmessage(stationno, maprotocols.EditModel, mastatuscodes.Notdetermined, Convert.ToString(spinupdays))
            cp.ps.SndMessage(outgoingmessage, False, 1)
            gotmessage = cp.ps.pollevent(maprotocols.Returntimestep, -1, False)
            cp.ps.Setstagestatus(maprotocols.Returntimestep, cp.ps.pollmessage.sc)
            If cp.ps.pollmessage.sc = mastatuscodes.Ok Then
                Dim timepart() As String = cp.ps.pollmessage.getmessage().Split(":")
                slabsize = plugin.setstep(Convert.ToInt32(timepart(1)) - Convert.ToInt32(timepart(0)), Convert.ToDouble(timepart(2)), EwEGOTMtimeratio, True, stationno, Connectname, dictiname, TextBox1.Text, Me.NumericUpDown2.Value, NumericUpDown3.Value)
                plugin.GOTMstat = 1 'Ready to go
                Status()
            End If

        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim editroutine As Runroutine
        If (RadioButton2.Checked) Then
            remoteedit()
        Else
            wx4 = True
        End If

    End Sub

    Private Sub b5click()
        Dim lock As IntPtr
        Dim a As Integer
        Dim s As String
        Dim simulator, mpl As PyObject
        lock = PythonEngine.AcquireLock()
        a = PythonEngine.RunSimpleString("import PyQt4")
        simulator = PythonEngine.ImportModule("visualizer")
        mpl = PythonEngine.ImportModule("matplotlib")
        s = mpl.InvokeMethod("get_backend").AsManagedObject(Type.GetType("System.String"))
        simulator.InvokeMethod("visualizeResult", result)
        PythonEngine.ReleaseLock(lock)
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If (RadioButton2.Checked) Then
            cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.InitializeModel, mastatuscodes.Ok, New String(Convert.ToString(slabsize) + ":" + Convert.ToString(EwEGOTMtimeratio))), False, stationno)
            cp.ps.pollevent(maprotocols.Runmodel, -1, False)
            plugin.ModelLoaded(CheckBox2.Checked)
            plugin.Starting(cp, NumericUpDown1.Value, True)
        Else
            wx5 = True
        End If

    End Sub




    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If (RadioButton2.Checked) Then
            cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Modelfinalize, mastatuscodes.Ok, "Display"), False, stationno)
        End If

        wx6 = True

    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        EwEGOTMtimeratio = NumericUpDown1.Value
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Shared Sub AddText(ByVal fs As FileStream, ByVal value As String)
        Dim info As Byte() = New UTF8Encoding(True).GetBytes(value)
        fs.Write(info, 0, info.Length)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        isnotended = False
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        If (RadioButton3.Checked) Then
            usenetCDFfile = RadioButton3.Checked
            TextBox12.Enabled = True
            Label12.Enabled = True
            If isinitialized Then
                cp.SwitchCDF(True)

            End If
        Else
            TextBox12.Enabled = True
            Label12.Enabled = True
            If isinitialized Then
                cp.SwitchCDF(False)

            End If
        End If
    End Sub

    Private Sub DateTimePicker2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker2.ValueChanged
        If isinitialized Then
            cp.SetEndDate(DateTimePicker2.Value())
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        If isinitialized Then
            cp.SetStartDate(DateTimePicker1.Value())
        End If
    End Sub

    Private Sub TextBox12_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox12.TextChanged
        ncfchanged = True
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim fh As FormHabitat = New FormHabitat
        plugin.autorescale = fh.SetLinks(cp, TextBox1.Text)
        fh.Visible = True
        b3click(False)
    End Sub
End Class