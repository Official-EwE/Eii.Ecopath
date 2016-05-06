' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' EwE Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' GOTMLink plug-in Copyright 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

#Region " Imports "

Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports Couplerlib
Imports EwECore
Imports Microsoft.Win32
Imports ScientificInterfaceShared.Controls
Imports System.Windows.Forms

#End Region ' Imports

Public Module GlobalCoupler
    Public cpglobal As CCouplerlib
End Module

Public Class FormGotmPluggin

    Public xx As CCouplerlib
    Private stationno As Integer
    'Dim fs As FileStream
    Private timebase As Double = 2436934.0
    Public hasrun, isnotended As Boolean
    Private CallType As Integer
    Private spinupdays As Integer
    Private FileString As String
    Private PathName As String
    Private FileName As String
    Private sockettext As String
    Private Connectname As String
    Private ncfchanged As Boolean
    Private StatusT As String() = {"Not Ready", "Waiting", "Running", "Finished", "Error"}
    Private thread, pipethreadc, pipethreade As Thread
    Private nprogress As Single
    Dim usespatial As Boolean
    Private isedited, isclicked, isinitialized As Boolean
    Public plugin As GOTMplugin

    Public EwEGOTMtimeratio As Integer
    Public cp As CCouplerlib
    Delegate Sub stadelegate(ByVal ib As Integer, ByVal ic As Integer)
    Private sta1 As stadelegate
    Private useextender As Integer
    Public wx4, wx5, wx6 As Boolean
    Private usenetCDFfile As Boolean
    Public Specification As Xml.XmlDocument
    Private dictiname, Shortpathname As String
    Private maxbufferlen, currbufferlen As Integer
    Private stbuf() As String
    Private isrescale, isrescale2 As Boolean
    Private xdim, ydim As Integer
    Private TestDataPath As String

    Friend extenders As List(Of PluginExtenderBase.PluginExtenderBase)

    Friend assembles As List(Of Assembly)
    Private curtime As Double
    Private slabsize As Integer
    Private modelno, modelnox, modelnix, modeliix, modeliox, modelni As List(Of Integer)
    Private modelio, modelii As List(Of Integer)
    Private regkey1, regkey2 As RegistryKey





    ''' <summary>
    ''' New constructor, is called everytime this object is created
    ''' </summary>
    Public Sub New(uic As cUIContext)

        Dim typ As Type()
        Dim consinfo As ConstructorInfo
        isinitialized = False
        EwEGOTMtimeratio = 1
        InitializeComponent()
        NumericUpDown1.Value = EwEGOTMtimeratio
        Status(0, 0)
        useextender = 0
        assembles = New List(Of Assembly)
        extenders = New List(Of PluginExtenderBase.PluginExtenderBase)
        assembles.Add(Assembly.LoadFrom("NetCDFServe.dll"))
        assembles.Add(Assembly.LoadFrom("GOTMserve.dll"))
        'extenders.Add(New GOTMserve.GOTMInterface)
        'extenders.Add(New NetCDFServe.NetCDFServe)
        usenetCDFfile = False
        Try
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", False)
            regkey1 = regkey1.OpenSubKey("EwE", False)
            regkey1 = regkey1.OpenSubKey("CouplerLib", False)
            TestDataPath = regkey1.GetValue("COUPLERPATH").ToString() + "\Testdata"
            'TestDataPath = "E:\ERSEMEwECoupler" + "\Testdata"
        Catch  'No Registry use Default
            TestDataPath = "E:\ERSEMEwECoupler" + "\Testdata"
            regkey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE", True)
            regkey1 = regkey1.CreateSubKey("EwE")
            regkey1 = regkey1.CreateSubKey("CouplerLib")
            regkey1.SetValue("COUPLERPATH", "E:\ERSEMEwECoupler")
        End Try
        For m = 0 To 1 'length of list of sub plugins
            typ = assembles(m).GetExportedTypes()
            For n = 0 To typ.Length - 1
                Dim retobject As ConstructorInfo
                Dim classname As String
                consinfo = typ(n).TypeInitializer()
                Dim pars As Type() = New Type() {Me.GetType()}
                Try
                    If (typ(n).BaseType.Name = "PluginExtenderBase") Then
                        retobject = typ(n).GetConstructor(pars) 'consinfo.Invoke(Nothing)
                        extenders.Add(retobject.Invoke(New Object() {Me}))
                        DomainUpDown1.Items.Add(typ(n).Name)
                    End If
                Catch
                    Dim a As Integer
                Finally
                End Try


            Next
        Next
        DomainUpDown1.SelectedIndex = 0
        ncfchanged = False
        'OpenFileDialog1.InitialDirectory = System.Environment.GetEnvironmentVariable("COUPLERPATH") + "\Testdata"
        'Me.TextBox2.Text = TestDataPath
        OpenFileDialog1.InitialDirectory = TestDataPath
        OpenFileDialog1.Filter = "Python Files (*.py)|*.py|Python Compile Files (*.pyc)|.pyc|xml Interface specification(*.xml)|.xml|All files (*.*)|*.*"
        OpenFileDialog1.FileName = TestDataPath + "\GOTMEWELink4.xml"
        OpenFileDialog1.FilterIndex = 3
        OpenFileDialog1.RestoreDirectory = True
        m_tbxLinkFile.Text = OpenFileDialog1.FileName()
        hasrun = False
        Me.UIContext = uic
        sta1 = New stadelegate(AddressOf Status)
        maxbufferlen = 20
        currbufferlen = 1
        ReDim stbuf(maxbufferlen + 1)
    End Sub


    Public Sub Status()
        If Me.InvokeRequired Then
            Invoke(sta1, plugin.GOTMstat, plugin.EwEstat)
        Else
            Me.BeginInvoke(New MethodInvoker(AddressOf Status), New Object() {plugin.GOTMstat, plugin.EwEstat})
        End If
    End Sub

    Public Sub Status(ByVal GS As Integer, ByVal ES As Integer)
        m_tbxGOTMStatus.Text = StatusT(GS)
        m_tbxEwEStatus.Text = StatusT(ES)
        Update()
    End Sub

    Public Sub Timedisplay(ByVal nn As Integer)
        Dim ayears As String = "EwE will run for " + nn.ToString + " years"
        TextBox2.Text = ayears
    End Sub


    Private Sub OnChooseLinkFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnChooseLinkFile.Click
        If Me.OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Me.m_tbxLinkFile.Text = OpenFileDialog1.FileName()
        End If
    End Sub


    Public Sub gotmremoteserver()
        FileString = OpenFileDialog1.FileName
        Dim outgoingmessage As Cprotmessage
        Dim gotmessage As Boolean
        Connectname = New String("LOWAPP25")
        stationno = cp.ps.AddStation(Connectname)
        cp.outputmessage.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), True)
        cp.errormessage.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), True)
        dictiname = Path.GetDirectoryName(m_tbxLinkFile.Text)
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


    Public Sub GOTMfront()
        extenders(DomainUpDown1.SelectedIndex).Load(regkey1, TestDataPath, OpenFileDialog1.FileName, cp, timebase)
    End Sub



    Public Sub gotmserver()
        'Dim i As Integer
        'Dim a As Integer
        'Dim tr As Integer
        Dim novars As Integer
        Dim regkey1, regkey2 As RegistryKey
        Dim datapath, py2path, svtemp As String
        'Dim svnames As List(Of String)
        'Dim svvalues As List(Of String)
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
                cp.SetEndDate(DateTimePicker2.Value())
                cp.SetStartDate(DateTimePicker1.Value())
                plugin.gearratio = NumericUpDown1.Value
            End If
            dictiname = Path.GetDirectoryName(m_tbxLinkFile.Text)
            'fs = File.Create(TestDataPath + "\bfmoutputs.txt")
            wx4 = wx5 = wx6 = False
            isedited = False
            isnotended = True

            While isnotended
                plugin.GOTMstat = 1
                Threading.Thread.Sleep(1)
                If wx4 Then
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

        cp = New CCouplerlib(RadioButton2.Checked, True, False, extenders(DomainUpDown1.SelectedIndex).UsesNetCDF())
        isinitialized = True
        Dim conob, errob As Object

        If (RadioButton2.Checked) Then
            'plugin.isnetworked = True
            'Me.thread = New Thread(AddressOf Me.gotmremoteserver)
            plugin.wx2 = True
        Else

            conob = New String("ConsolePipe")
            errob = New String("ErrorPipe")
            cp.pp1.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), False, Nothing)
            cp.pp2.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), False, Nothing)
            Me.pipethreadc = New Thread(AddressOf cp.pp1.pipeserver)
            Me.pipethreade = New Thread(AddressOf cp.pp2.pipeserver)
            pipethreadc.Start()
            pipethreade.Start()
        End If
        extenders(DomainUpDown1.SelectedIndex).Load(regkey1, Shortpathname, OpenFileDialog1.FileName, cp, timebase)
        Threading.Thread.Sleep(2000)
        cp.setdelegate(Me, New textedelegate(AddressOf ConsoleOutputText), True)
        cp.setdelegate(Me, New textedelegate(AddressOf ErrorOutputText), False)
        plugin.wx2 = True



    End Sub



    Delegate Sub InvokeDelegate()
    Delegate Sub InvokeSDelegate()
    Delegate Sub InvokeConDelegate(ByVal ists As String())
    Delegate Sub InvokeErrDelegate()

    Private Sub simulate()


        usespatial = CheckBox1.Checked()
        extenders(DomainUpDown1.SelectedIndex).simulate(Shortpathname, NumericUpDown1.Value, NumericUpDown2.Value * 365 + NumericUpDown3.Value * 30, Specification, usespatial)


    End Sub

    Public Sub Postvaluesa(ByRef vdataa As Double()(), ByVal nodataelements As Integer, ByVal nospatialelements As Integer)
        For i As Integer = 0 To modelio.Count - 1

            cp.PutIf(modelno(i), modelio(i), vdataa, nodataelements, nospatialelements)

        Next
    End Sub

    Public Sub postvalues(ByRef vdata As Double(), ByVal nodataelements As Integer)
        For i As Integer = 0 To modelio.Count - 1

            cp.PutIf(modelno(i), modelio(i), vdata, nodataelements)
        Next
    End Sub

    Public Function getvalues(ByRef vdata As Double()) As Integer
        Dim noi As Integer

        For i As Integer = 0 To modelii.Count - 1

            noi = cp.GetIf(modelnix(i), modeliox(i), modelni(i), modelii(i), modelnox(i), modelio(i), vdata)
            If (noi > 0) Then
                Return noi
            End If

        Next
    End Function


    Public Function Starting(ByVal lmname As String, ByVal usesocket As Boolean, ByRef icurtime As Double) As Boolean
        Dim retok As Boolean
        retok = plugin.Starting(cp, NumericUpDown1.Value, usesocket)
        icurtime = cp.GetStartTime(0)
        curtime = icurtime
        modelio = cp.GetIfAddress(modelno, lmname, False, False)
        modeliix = cp.GetIfAddress(modelnox, lmname, True, True)
        modelii = cp.GetIfAddress(modelni, lmname, False, True)
        modeliox = cp.GetIfAddress(modelnix, lmname, True, False)
        Return retok
    End Function

    Public Sub Progressbar()
        cApplicationStatusNotifier.UpdateProgress(Me.Core, "", nprogress)
        'Me.ProgressBar1.Value = Int(Math.Round(nprogress * 100))
        'Me.Update()
    End Sub

    Public Function orgrefs() As Integer()


    End Function


    Public Sub SocketProgressText()
        'Me.TextBox5.Text = sockettext
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



    Public Sub TimeSpecify(ByVal templatefilename As String, ByVal sttime As String, ByVal endtime As String, ByVal inttime As Integer)
        Specification = New Xml.XmlDocument
        Dim NL, CL, GCL As Xml.XmlNodeList
        Dim Node As Xml.XmlNode
        Dim ts As TimeSpan

        'Specification.Load("C:\ewecefas\gotm\gui.py\gotmtemplate.xml")
        Specification.Load(TestDataPath + "\" + templatefilename)
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




    Private Sub b3click(ByVal carryoutedit As Boolean)
        Dim dt As Double
        Dim dstart, dend As Double
        Dim dsstart, dsend As String

        spinupdays = NumericUpDown2.Value * 365 + NumericUpDown3.Value * 30
        cp.SetEndDate(DateTimePicker2.Value())
        cp.SetStartDate(DateTimePicker1.Value())
        dsstart = DateTimePicker1.Value().ToString("o")
        dsend = DateTimePicker2.Value().ToString("o")
        dt = 24 * 60 * 60
        'Me.TimeSpecify("GOTMTEMPLATE.xml", dsstart, dsend, Int(dt))
        'slabsize = plugin.setstep(Int(dend - dstart), dt, EwEGOTMtimeratio, False, 0, "host", dictiname, TextBox1.Text, NumericUpDown2.Value, NumericUpDown3.Value)
        Status()
        extenders(DomainUpDown1.SelectedIndex).Edit(carryoutedit, EwEGOTMtimeratio, spinupdays, Specification, cp)


    End Sub

    Public Function setstep(ByRef duration As Integer, ByRef StepSize As Double, ByVal gearratioi As Integer, ByVal isnet As Boolean, ByVal istation As Integer, ByVal connectname As String) As Integer
        Dim slabsize As Integer
        slabsize = plugin.setstep(duration, StepSize, gearratioi, isnet, istation, connectname, dictiname, m_tbxLinkFile.Text, NumericUpDown2.Value, NumericUpDown3.Value)
        Return (slabsize)
    End Function

    Private Sub remoteedit()
        Dim slabsize As Integer
        Dim outgoingmessage As Cprotmessage
        Dim gotmessage As Boolean
        If (cp.ps.Getstagestatus(maprotocols.Ack_Establishxmllocation) = mastatuscodes.Ok) Then
            outgoingmessage = New Cprotmessage(stationno, maprotocols.EditModel, mastatuscodes.Notdetermined, Convert.ToString(spinupdays))
            cp.ps.SndMessage(outgoingmessage, False, 1)
            gotmessage = cp.ps.pollevent(maprotocols.Returntimestep, -1, False)
            cp.ps.Setstagestatus(maprotocols.Returntimestep, cp.ps.pollmessage.sc)
            If cp.ps.pollmessage.sc = mastatuscodes.Ok Then
                Dim timepart() As String = cp.ps.pollmessage.getmessage().Split(":")
                slabsize = plugin.setstep(Convert.ToInt32(timepart(1)) - Convert.ToInt32(timepart(0)), Convert.ToDouble(timepart(2)), EwEGOTMtimeratio, True, stationno, Connectname, dictiname, m_tbxLinkFile.Text, Me.NumericUpDown2.Value, NumericUpDown3.Value)
                plugin.GOTMstat = 1 'Ready to go
                Status()
            End If

        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim editroutine As Runroutine
        extenders(DomainUpDown1.SelectedIndex).Edit(True, EwEGOTMtimeratio, spinupdays, Specification, cp)
        If (RadioButton2.Checked) Then
            remoteedit()
        Else
            wx4 = True
        End If

    End Sub

    Private Sub b5click()
        extenders(useextender).display()
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

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tbxLinkFile.TextChanged

    End Sub

    Private Shared Sub AddText(ByVal fs As FileStream, ByVal value As String)
        Dim info As Byte() = New UTF8Encoding(True).GetBytes(value)
        fs.Write(info, 0, info.Length)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        isnotended = False
    End Sub

    'Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '   If (RadioButton3.Checked) Then
    '      usenetCDFfile = RadioButton3.Checked
    '     TextBox12.Enabled = True
    '    Label12.Enabled = True
    '   If isinitialized Then
    '      cp.SwitchCDF(True)
    '
    '       End If
    '  Else
    '     TextBox12.Enabled = True
    '    Label12.Enabled = True
    '   If isinitialized Then
    '      cp.SwitchCDF(False)

    '            End If
    '       End If
    '  End Sub

    Private Sub DateTimePicker2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If isinitialized Then
            cp.SetEndDate(DateTimePicker2.Value())
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If isinitialized Then
            cp.SetStartDate(DateTimePicker1.Value())
        End If
    End Sub

    Public Sub StoreInRegistry()
        For n As Integer = 0 To extenders.Count - 1
            extenders(n).StoreInRegistry()
        Next
    End Sub

End Class