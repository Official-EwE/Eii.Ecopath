Imports EwECore
Imports EwEPlugin
Imports System.Threading
Imports Couplerlib


Public Class GOTMplugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IAutolaunchPlugin
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimBeginTimestepPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin
    Implements EwEPlugin.IEcosimSubTimestepsPlugin
    Implements EwEPlugin.IEcosimDataInitializedPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin
    Implements EwEPlugin.IEcospaceBeginTimestepPlugin
    Implements EwEPlugin.IEcospaceEndTimestepPlugin
    Implements EwEPlugin.IEcospaceRunCompletedPlugin
    Implements EwEPlugin.IEcopathClosedPlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcospacePlugin



    Private m_core As EwECore.cCore
    Private m_bInitOK As Boolean
    Private isnetworked As Boolean
    Private spinupdays As Integer
    Private longitude, latitude, longinterval, latinterval As Double
    Private latlen, longlen As Integer
    Private modelname, hosttono As String
    Private stationno As Integer
    Private m_PluginInterface As FormGotmPluggin
    Private cp As Couplerlib.CCouplerlib = Nothing
    Private numyears As Integer
    Public gearratio As Integer
    Public EwEstat, GOTMstat As Integer
    Private m_EcosimDS As cEcosimDatastructures
    Private m_EcospaceDS As cEcospaceDataStructures
    Private predscreen(), detrituspomscreen(), detritusdomscreen(), detritusfecscreen(), detritussedscreen(), pomscreen(), domscreen(), fecscreen(), sedscreen(), domfate(), pomfate(), fecfate(), sedfate(), oldbiomass() As Double
    Private nsy As Integer = 365
    Private isended, isabort As Boolean
    Private ifinputs As List(Of Integer)
    Private ifoutputs As List(Of Integer)
    Private imodellm As List(Of Integer)
    Private omodellm As List(Of Integer)
    Private ixinputs As List(Of Integer)
    Private ixoutputs As List(Of Integer)
    Private ixmodellm As List(Of Integer)
    Private oxmodellm As List(Of Integer)
    Private Shared WH1(1), WH2(1), WH3(1) As Threading.ManualResetEvent
    Public Shared wx1, wx2, wx3, wx4 As Boolean
    Private tp As IntPtr
    Private ifnoarray() As Integer
    Private thread As Thread
    Delegate Sub InvokeStatusDelegate()
    Dim svnames As List(Of String)
    Dim svvalues As List(Of String)
    Dim nostatev As Integer
    Dim sspecname As String
    Dim sdictname As String
    Dim hasrun As Integer
    Dim tmodel, salmodel As Integer




    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    ''' 
    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        m_EcosimDS = DirectCast(objEcoSim, Ecosim.cEcoSimModel).EcosimData
    End Sub

    Public Function SaveModel(ByVal dataSource As Object) As Boolean Implements Global.EwEPlugin.IEcopathPlugin.SaveModel

        Return True
    End Function

    Public Function LoadModel(ByVal dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel
        REM m_core = cCore.GetInstance()

    End Function

    

    Public Sub ModelLoaded()
        Dim ng As Integer
        ng = m_core.nGroups
        'Dim longitude As Double = m_core.m_EcoSpaceData.lon1
        Dim nx(ng) As String
        Dim nu(ng) As String REM EwE must supply units 
        Dim nc(ng) As String
        ReDim predscreen(ng + 1)
        ReDim detrituspomscreen(ng + 1)
        ReDim detritusdomscreen(ng + 1)
        ReDim detritusfecscreen(ng + 1)
        ReDim detritussedscreen(ng + 1)
        ReDim pomscreen(ng + 1)
        ReDim domscreen(ng + 1)
        ReDim fecscreen(ng + 1)
        ReDim sedscreen(ng + 1)
        ReDim pomfate(ng + 1)
        ReDim domfate(ng + 1)
        ReDim fecfate(ng + 1)
        ReDim sedfate(ng + 1)
        For n As Integer = 1 To ng
            nx(n - 1) = m_core.EcoPathGroupInputs(n).Name
            nu(n - 1) = "kg/km²"
            nc(n - 1) = "Consumer"
        Next
        ReDim oldbiomass(ng + 1)
        XmlSpecify(ng, nx, nu, nc)
    End Sub
    Public Sub Initialize(ByVal core As Object) Implements Global.EwEPlugin.IPlugin.Initialize
        REM Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        EwEstat = 0
        GOTMstat = 0
        hasrun = False
        isended = False
        isabort = False
        WH1(0) = New Threading.ManualResetEvent(False)
        WH2(0) = New Threading.ManualResetEvent(False)
        WH3(0) = New Threading.ManualResetEvent(False)
        wx1 = False
        wx2 = False
        wx3 = False
        Try
            If TypeOf core Is EwECore.cCore Then
                m_core = DirectCast(core, EwECore.cCore)
                Me.thread = New Thread(AddressOf Me.displaythread)
                Me.thread.Start()
                m_bInitOK = True
                System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            System.Diagnostics.Debug.Assert(False, ex.Message)
            Return
        End Try
    End Sub

    Public Sub displaythread()
        m_PluginInterface = New FormGotmPluggin(m_core)
        m_PluginInterface.plugin = Me
    End Sub

#Region "Plugin implementation"
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.EwE_GOTM2
        End Get
    End Property

    ''' <summary>
    ''' Text to be displayed for the plugin
    ''' </summary>
    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "EwE-GOTM Link Plug-in"
        End Get
    End Property


    ''' <summary>
    ''' Location where the menu item should go
    ''' </summary>
    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "GOTM"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "EwE to GOTM Link Plug-in"
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            'this will put the navigation item at the end of the tree as top level node 
            'Not the best place there should be a Plugins node and all plugins should go under it
            Return "ndTools"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "GOTM Linker Plugin"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Cefas"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:Jonathan.Beecham@cefas.co.uk"
        End Get
    End Property
#End Region 'Plugin implementation

#Region " GUI "

    ''' ===========================================================================
    ''' <summary>
    ''' Plug-in that should auto-launch when a consuming GUI is loaded
    ''' </summary>
    ''' ===========================================================================
    Public Interface IAutolaunchPlugin
        REM Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point to state whether auto-launch is active.
        ''' </summary>
        ''' <remarks>True if active.</remarks>
        ''' -----------------------------------------------------------------------
        Function Autolaunch() As Boolean

    End Interface
    Public Interface ICorePlugin
        Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object)
    End Interface

    Public Interface IGUIPlugin
        Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form)

    End Interface

    Public Interface IEcopathPlugin
        Sub SaveModel(ByVal dataSource As Object)
        Sub LoadModel(ByVal dataSource As Object)
    End Interface
    Public Interface IPlugin
        Sub Initialize(ByVal core As Object)

    End Interface
    Public Interface IEcosimRunInitializedPlugin
        Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object)
    End Interface
    Public Interface IEcosimInitializedPlugin
        Sub EcosimInitialized(ByRef EcosimDatastructures As Object)
    End Interface
    Public Interface IEcosimBeginTimestepPlugin
        Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer)
    End Interface
    Public Interface IEcosimEndTimestepPlugin
        Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object)
    End Interface
    Public Interface IEcosimRunCompletedPlugin
        Sub EcosimRunCompleted(ByRef EcosimDatastructures As Object)
    End Interface
    Public Interface IEcosimSubTimeStepPlugin
        Sub EcosimTimeStepBegin(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object)
        Sub EcosimTimeStepEnd(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object)

    End Interface

    Public Interface IMenuItemPlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the menu item location for this plugin.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property MenuItemLocation() As String

    End Interface

    Public Interface IEcopathClosedPlugin
        Function CloseModel() As Boolean
    End Interface

    Public Function CloseModel() As Boolean Implements EwEPlugin.IEcopathClosedPlugin.CloseModel
        isabort = True
        m_PluginInterface.ClosePipes()
        m_PluginInterface.StoreInRegistry()
        m_PluginInterface.isnotended = False
        isended = True
        If cp IsNot Nothing Then

            cp.CleanUp()
        End If
        Threading.Thread.Sleep(20)
    End Function

    Public Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        EcoModelRunInitialized(EcosimDatastructures, Nothing, False)
    End Sub

    Public Sub EcoModelRunInitialized(ByVal EcosimDatastructures As Object, ByVal EcospaceDatastructures As Object, ByVal UseEcospace As Boolean)
        Dim ds As cEcosimDatastructures
        Dim maxsize As Integer
        Dim initok As Boolean
        wait(wx1, True)
        ds = EcosimDatastructures
        maxsize = 0

        ifinputs = cp.GetIfAddress(imodellm, modelname, True, False) REM inputs
        ifoutputs = cp.GetIfAddress(omodellm, modelname, False, True) REM outputs
        ixinputs = cp.GetIfAddress(ixmodellm, modelname, True, True) REM inputs
        ixoutputs = cp.GetIfAddress(oxmodellm, modelname, False, False) REM outputs
        cp.GetSupplement("predscreen", predscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("detrituspomscreen", detrituspomscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("detritusdomscreen", detritusdomscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("detritusfecscreen", detritusfecscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("detritussedscreen", detritussedscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("pomscreen", pomscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("domscreen", domscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("fecscreen", fecscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("sedscreen", fecscreen, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("pomfate", pomfate, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("domfate", domfate, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("fecfate", fecfate, imodellm(0), ifinputs(0), ds.nGroups + 1)
        cp.GetSupplement("sedfate", fecfate, imodellm(0), ifinputs(0), ds.nGroups + 1)
        Threading.Thread.Sleep(10)
        EwEstat = 2
        'm_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
        REM m_PluginInterface.Status(GOTMstat, EwEstat)
        If (Not isnetworked) Then
            wx2 = True
            WH2(0).Set()
            wait(wx1, True)
        Else
            Dim initmessage As Cprotmessage
            initmessage = New Cprotmessage(stationno, maprotocols.Ack_Runmodel, mastatuscodes.Notdetermined, "Initialize")
            cp.ps.SndMessage(initmessage, False, stationno)
            ' cp.ps.pollevent(protocols.Ack_Initializemodel).WaitOne(-1)
        End If


        REM Threading.WaitHandle.WaitAll(WH1)

    End Sub
    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        EcoModelInitialized(EcosimDatastructures, Nothing, False)
    End Sub
    Public Sub EcoModelInitialized(ByVal EcosimDatastructures As Object, ByVal EcospaceDatastructures As Object, ByVal UseEcospace As Boolean)
        Dim ds As cEcosimDatastructures
        Dim a As Integer
        If GOTMstat > 0 Then
            ds = EcosimDatastructures
            'm_core = cCore.GetInstance()
            Dim ng As Integer
            ng = m_core.nGroups
            REM a = m_core.m_EwEModelUnitCurrency
            ds.NumYears = numyears
            nsy = ds.NumStepsPerYear
            If numyears > 0 Then
                ds.redimTime()
            End If
            EwEstat = 1
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            m_PluginInterface.Timedisplay(numyears)
        End If
    End Sub

    Public Sub XmlSpecify(ByVal nogroups As Integer, ByVal nxi As String(), ByVal nui As String(), ByVal nci As String())
        Dim Specification As Xml.XmlDocument = New Xml.XmlDocument
        Dim NL As Xml.XmlNodeList
        Dim Node As Xml.XmlNode

        Dim n, m As Integer
        Specification.Load(sdictname + "\EWEtemplate.xml")
        Dim Child, GChild, GGChild, GGGChild As Xml.XmlNode
        NL = Specification.GetElementsByTagName("ModelName")
        modelname = NL(0).InnerText
        NL = Specification.GetElementsByTagName("Interface")
        EcospaceSpecify(Specification, longitude, latitude, longinterval, latinterval, longlen, latlen)
        For n = 0 To NL.Count - 1
            Node = NL(n)
            For k = 0 To Node.ChildNodes.Count - 1
                If Node.ChildNodes(k).Name = "DataCollection" Then
                    For m = 0 To nogroups + 1
                        Child = Specification.CreateElement("Data")
                        GChild = Specification.CreateElement("Name")
                        If m < nogroups Then
                            GChild.InnerText = nxi(m)
                        ElseIf m = nogroups Then
                            GChild.InnerText = "Temperature"
                        Else
                            GChild.InnerText = "Salinity"
                        End If
                        Child.AppendChild(GChild)
                        GChild = Specification.CreateElement("DataItem")
                        GChild.InnerText = ""
                        If (m < nogroups) Then
                            GGChild = Specification.CreateElement(nci(m))
                        Else
                            GGChild = Specification.CreateElement("StateVariable")
                        End If
                        GGGChild = Specification.CreateElement("Name")
                        If m < nogroups Then
                            GGGChild.InnerText = nxi(m)
                        ElseIf m = nogroups Then
                            GGGChild.InnerText = "Temperature"
                        Else
                            GGGChild.InnerText = "Salinity"
                        End If
                        GGChild.AppendChild(GGGChild)
                        GGGChild = Specification.CreateElement("Symbol")
                        If m < nogroups Then
                            GGGChild.InnerText = nxi(m).Substring(0, 3)
                        ElseIf m = nogroups Then
                            GGGChild.InnerText = "Tmp"
                        Else
                            GGGChild.InnerText = "Sal"
                        End If
                        GGChild.AppendChild(GGGChild)
                        If (m < nogroups) Then
                            GGGChild = Specification.CreateElement("Constituent")
                            GGGChild.InnerText = "C"
                            GGChild.AppendChild(GGGChild)
                        End If
                        GGGChild = Specification.CreateElement("Description")
                        If m < nogroups Then
                            GGGChild.InnerText = nxi(m) + " EwE Group"
                        ElseIf m = nogroups Then
                            GGGChild.InnerText = "Mean Temperature"
                        Else
                            GGGChild.InnerText = "Mean Salinity"
                        End If
                        GGChild.AppendChild(GGGChild)
                        GChild.AppendChild(GGChild)
                        Child.AppendChild(GChild)
                        GChild = Specification.CreateElement("Flux")
                        If n = 0 Or m >= nogroups Then
                            GChild.InnerText = "State"
                        Else
                            GChild.InnerText = "Predation"
                        End If
                        Child.AppendChild(GChild)
                        GChild = Specification.CreateElement("Units")
                        If n = 0 And m < nogroups Then
                            GChild.InnerText = nui(m)
                        Else
                            If m < nogroups Then

                                GChild.InnerText = nui(m) + "/Interval"
                            ElseIf m = nogroups Then
                                GChild.InnerText = "C"
                            Else
                                GChild.InnerText = "psu"
                            End If
                        End If
                        Child.AppendChild(GChild)
                        Node.ChildNodes(k).AppendChild(Child)
                    Next
                End If
            Next
        Next
        Specification.Save(sdictname + "\EWE.xml")



    End Sub
    Public Sub EcospaceSpecify(ByRef Specification As Xml.XmlDocument, ByVal lonstart As Double, ByVal latstart As Double, ByVal longint As Double, ByVal latint As Double, ByVal longsz As Integer, ByVal latsz As Integer)
        Dim NL, CL, GCL As Xml.XmlNodeList
        Dim Node, NewNode, NewChild As Xml.XmlNode
        Dim NodeName As String() = {"Longitude", "Latitude", "Depth"}
        Dim nodims As Integer
        'Specification.Load(TestDataPath + "\GOTMtemplate.xml") REM change this
        NL = Specification.GetElementsByTagName("GridData")
        For n As Integer = 0 To NL.Count - 1
            Node = NL(n)
            CL = Node.ChildNodes
            If CL(0).Name = "GridFormRaster2D" Then

            End If
            If CL(0).Name = "GridFormNone" Then
                Node.RemoveChild(CL(0))
                NewNode = Specification.CreateElement("GridFormRaster2D")
                NewNode.InnerText = "2D"
                Node.AppendChild(NewNode)
            End If
            nodims = 2
            For m = 1 To CL.Count - 1
                Node.RemoveChild(CL(m))
            Next
            For m = 1 To nodims
                NewNode = Specification.CreateElement(NodeName(m - 1))
                NewChild = Specification.CreateElement("Minimum")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(lonstart)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(latstart)
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Interval")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(longint)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(latint)
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Length")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(longsz)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(latsz)
                End If
                NewNode.AppendChild(NewChild)
                Node.AppendChild(NewNode)
            Next

        Next

    End Sub

    Public Sub EcosimSubTimeStepBegin(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepBegin
        Dim noip, n As Integer
        Dim IParray As Integer()
        Dim modd As Double()
        Dim d As Integer
        If GOTMstat > 0 Then
            Dim ds As cEcosimDatastructures
            If Not isnetworked Then
                hasrun = m_PluginInterface.hasrun
            End If
            If Not hasrun Then
                If isnetworked Then
                    cp.ps.pollevent(maprotocols.Timestep, -1, False)
                    If cp.ps.pollmessage.sc = mastatuscodes.Waitingonresponse Then
                        hasrun = True
                    End If
                    cp.ps.resetpollevent(maprotocols.Timestep)
                Else
                    wait(wx3, True)
                End If

            Else
                d = -1
            End If
            ds = EcosimDatastructures
            For n = 0 To Me.imodellm.Count - 1
                cp.EstablishTransmit(hosttono)
                If cp.oneway Then
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), modd)
                Else
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), ixmodellm(n), ixinputs(n), modd)
                End If
                cp.FinishTransmit()
                For m = 0 To noip - 1
                    d = cp.OrgReference(imodellm(n), ifinputs(n), m) + 1
                    If d = BiomassAtTimestep.Length Then
                        If (ds.TemperatureForceNo > 0) Then
                            ds.tval(ds.TemperatureForceNo) = modd(m)
                        End If
                    End If
                    If d = BiomassAtTimestep.Length + 1 Then
                        If (ds.SalinityForceNo > 0) Then
                            ds.tval(ds.SalinityForceNo) = modd(m)
                        End If
                    End If
                    If d < BiomassAtTimestep.Length Then
                        BiomassAtTimestep(cp.OrgReference(imodellm(n), ifinputs(n), m) + 1) = modd(m)  REM biomass at timestep is 1 indexed
                    End If
                Next
            Next
            For n = 1 To ds.nGroups
                oldbiomass(n) = BiomassAtTimestep(n)
            Next
        End If
    End Sub
    Public Sub EcosimSubTimeStepEnd(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepEnd
        Dim n, m, k, ng, jlink, ilink, a As Integer
        Dim ds As cEcosimDatastructures
        Dim prex As Single()
        Dim modd As Double()
        Dim detdom, detpom, detfec, detsed As Double
        If GOTMstat > 0 Then
            ds = EcosimDatastructures
            ng = ds.nGroups
            modd = New Double(ng) {}
            For n = 0 To Me.oxmodellm.Count - 1
                detdom = 0.0
                detpom = 0.0
                For m = 0 To ds.nGroups
                    modd(m) = 0.0
                    detdom -= ds.GroupDetritus(m) * detritusdomscreen(m) 'detritus production rate from groups
                    detpom -= ds.GroupDetritus(m) * detrituspomscreen(m)
                    detfec -= ds.GroupDetritus(m) * detritusfecscreen(m)
                    detsed -= ds.GroupDetritus(m) * detritussedscreen(m)
                Next

                For m = 1 To ds.inlinks
                    jlink = ds.jlink(m)
                    ilink = ds.ilink(m)
                    modd(ilink - 1) += ds.MPred(m) * predscreen(jlink)
                    detpom -= ds.MPred(m) * pomscreen(jlink)
                    detdom -= ds.MPred(m) * domscreen(jlink)
                    detfec -= ds.MPred(m) * fecscreen(jlink)
                    detsed -= ds.MPred(m) * sedscreen(jlink)
                    If (ilink = 52) Then
                        a = 1
                    End If
                Next

                For m = 1 To ds.nGroups
                    modd(m - 1) += detpom * pomfate(m)
                    modd(m - 1) += detdom * domfate(m)
                    modd(m - 1) += detfec * fecfate(m)
                    modd(m - 1) += detsed * sedfate(m)
                    modd(m - 1) /= (360.0 * Convert.ToSingle(gearratio))
                Next
                For m = 1 To ds.nGroups
                    modd(m - 1) /= oldbiomass(m)
                    If modd(m - 1) > 0.1 Then
                        modd(m - 1) = 0.1
                    End If

                Next
                cp.PutIf(oxmodellm(n), ixoutputs(n), modd, -1)
            Next
            If (isnetworked) Then
                cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Finishtimestep, mastatuscodes.Ok, "Data Ready"), False, stationno)
            Else
                wx2 = True
            End If

        End If

    End Sub

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcosimBeginTimestepPlugin.EcosimBeginTimeStep


    End Sub
    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

    End Sub
    Public Sub EcosimEcosimRunCompleted(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunCompletedPlugin.EcosimRunCompleted
        isended = True
        If (isnetworked) Then
            cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Finishtimestep, mastatuscodes.Waitingonresponse, "Model Over"), False, stationno)
            isended = True
        End If

    End Sub
    Public Sub wait(ByRef waiter As Boolean, ByVal sinvoke As Boolean)
        Dim a As Integer
        REM Dim b As Integer = AppDomain.GetCurrentThreadId()
        While (Not waiter And Not isabort)
            a *= 1
            Threading.Thread.Sleep(1)

            System.Windows.Forms.Application.DoEvents()
        End While
        If (sinvoke) Then
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            m_PluginInterface.Invoke(New FormGotmPluggin.InvokeDelegate(AddressOf m_PluginInterface.Progressbar))
            m_PluginInterface.Invoke(New FormGotmPluggin.InvokeSDelegate(AddressOf m_PluginInterface.Progresstext))
        End If
        waiter = False
    End Sub

    Public Function setstep(ByRef duration As Integer, ByRef stepsz As Double, ByVal gearratioi As Integer, ByVal isnet As Boolean, ByVal istation As Integer, ByVal connectname As String, ByVal dictname As String, ByVal specname As String, ByVal yearoff As Decimal, ByVal monthoff As Decimal) As Double
        REM Python.Runtime.PythonEngine.EndAllowThreads(tp)
        isnetworked = isnet
        stationno = istation
        hosttono = connectname
        sdictname = dictname
        sspecname = specname
        Dim stpd As Integer = Int((24 * 60 * 60) / stepsz)
        Dim nodays As Integer = duration - yearoff * 365 - monthoff * 30
        Dim noyears As Integer = Int(nodays / 365)
        Dim noleapyears As Integer = Int(noyears + 3) / 4
        Dim calctimestep As Integer = stpd * 365 * noyears + noleapyears * stpd
        Dim retval As Double
        'gearratio = gearratioi
        numyears = noyears
        spinupdays = yearoff * 365 + monthoff * 30
        retval = ((calctimestep) / (nsy * numyears))
        Return (retval)
    End Function
    Public Function runstep() As Boolean
        REM TRANSFER DATA WHILE THREAD IS SUSPENDED
        Threading.Thread.Sleep(1)
        WH3(0).Set() 'Ewe Thread starts
        wx3 = True
        If isended Then
            EwEstat = 3
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            If Not isabort Then
                m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            End If
        Else
            wait(wx2, False)
            REM Threading.WaitHandle.WaitAll(WH2) 'wait for ewe to end
        End If
        Return isended
    End Function

    Public Function Starting(ByVal cpi As CCouplerlib, ByVal gearratioi As Decimal, ByVal usesocket As Boolean) As Boolean
        REM now we check the xml files
        REM normall call the interface with the name of the interface file
        Dim linkstat As Boolean
        cp = cpi
        'gearratio = gearratioi
        GOTMstat = 1 'Ready to go
        WH1(0).Set() 'trigger EwE thread
        Threading.Thread.Sleep(2000)
        cp.Initialize(sdictname, sdictname + "/meecedict.xml", sspecname)
        linkstat = cp.CheckInterface()
        wx1 = True
        If EwEstat = 1 Then
            wx3 = True
        End If
        If (Not usesocket) Then
            wait(wx2, False)
        End If
        REM Threading.WaitHandle.WaitAll(WH2)
        GOTMstat = 2 'EwE is now running so can continue
        wx1 = True
        Return linkstat
    End Function
    Public Sub isover()
        wx3 = True
        GOTMstat = 3
        If Not isabort Then
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
        End If
    End Sub


    ''' <summary>
    ''' Menu Item or Tree node clicked
    ''' </summary>
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False
        EwEstat = 0
        GOTMstat = 0
        isended = False
        'Interface item has been clicked
        'Show the Ecotroph interface
        If m_bInitOK Then
            ' Test if form still exists

            If Me.m_PluginInterface IsNot Nothing And Me.m_PluginInterface.IsDisposed Then
                Me.m_PluginInterface = New FormGotmPluggin(m_core)
            End If


            ' Activate the form
            Me.m_PluginInterface.Show()

            ' Pass form reference back to calling app
            f = Me.m_PluginInterface

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel

            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu

            End If
        Else
            REM Debug.Assert(False, "Plugin was not initialized properly.")
        End If
    End Sub

    Function Autolaunch() As Boolean Implements EwEPlugin.IAutolaunchPlugin.Autolaunch


        Return False

    End Function


#End Region 'GUI


    Public Sub EcosimPreDataInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreDataInitialized

    End Sub

    Public Sub EcosimPreRunInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreRunInitialized
        If GOTMstat > 0 Then
            Me.m_EcosimDS.StepsPerMonth = 30 / gearratio
            Me.m_EcosimDS.bMultiThreaded = True
        End If
    End Sub

    Public Sub EcospaceInitialized(ByVal EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized
        EcoModelInitialized(Me.m_EcosimDS, EcospaceDatastructures, True)
        Dim sscale As Double = EcospaceDatastructures.CellLength * EcospaceDatastructures.KM_TO_DEGRESS
        longlen = EcospaceDatastructures.InCol
        latlen = EcospaceDatastructures.InRow
        longitude = EcospaceDatastructures.lon1
        latitude = EcospaceDatastructures.lat1 - latlen * sscale
        EcospaceDatastructures.TimeStep = gearratio / 365.0
        longinterval = sscale 'long(1) - EcospaceDatastructures.long(0)
        latinterval = sscale '(1) - EcospaceDatastructures.lat(0)
        'm_PluginInterface.EcospaceSpecify(longitude, latitude, longinterval, latinterval, longlen, latlen)
        EwEstat = 1
        EcoModelRunInitialized(Me.m_EcosimDS, EcospaceDatastructures, True)
    End Sub

    Public Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        Dim noip, n, ycell As Integer
        Dim IParray As Integer()
        Dim modd As Double()()
        Dim xdim, ydim As Integer
        Dim xmax, ymax As Integer
        If GOTMstat > 0 Then
            If EwEstat = 0 Then
                EwEstat = 1
            End If
            Dim ds As cEcospaceDataStructures
            If Not m_PluginInterface.hasrun Then
                If isnetworked Then

                    cp.ps.pollevent(maprotocols.Timestep, -1, False)
                Else
                    wait(wx3, True)
                End If
            End If
            Dim d As Integer
            ds = EcospaceDatastructures
            ymax = ds.InCol
            xmax = ds.InRow
            For n = 0 To Me.imodellm.Count - 1
                If cp.oneway Then
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), modd, ymax * xmax)
                Else
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), ixmodellm(n), ixinputs(n), modd(0))
                End If
                For m = 0 To noip - 1
                    d = cp.OrgReference(imodellm(n), ifinputs(n), m) + 1
                    For xdim = 1 To xmax
                        For ydim = 1 To ymax
                            ycell = (ymax - ydim) * xmax + (xdim - 1)
                            ds.Bcell(xdim, ydim, (cp.OrgReference(imodellm(n), ifinputs(n), m) + 1)) = modd(m)(ycell)  REM biomass at timestep is 1 indexed
                        Next
                    Next
                Next
            Next
        End If

    End Sub

    Public Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep
        Dim n, m, k, ng, jlink, ilink As Integer
        Dim ds As cEcospaceDataStructures
        Dim dsim As cEcosimDatastructures
        Dim prex As Single()
        Dim modd As Double()
        Dim detdom, detpom As Double
        If GOTMstat > 0 Then
            ds = EcospaceDatastructures
            ng = ds.NGroups
            modd = New Double(ng) {}
            For n = 0 To Me.oxmodellm.Count - 1
                detdom = 0.0
                detpom = 0.0
                For m = 0 To ds.NGroups
                    modd(m) = 0.0
                    detdom += ds.GroupDetritus(1, 1, m) * detritusdomscreen(m)  'detritus production rate from groups
                    detpom += ds.GroupDetritus(1, 1, m) * detrituspomscreen(m)
                Next

                For m = 1 To m_EcosimDS.inlinks
                    jlink = m_EcosimDS.jlink(m)
                    ilink = m_EcosimDS.ilink(m)
                    modd(ilink - 1) += ds.MPred(1, 1, m) * predscreen(jlink)
                    detpom += ds.MPred(1, 1, m) * pomscreen(jlink)
                    detdom += ds.MPred(1, 1, m) * domscreen(jlink)
                Next

                For m = 1 To ds.NGroups
                    modd(m - 1) += detpom * pomfate(m) ' / 5.0 'detritus is dry 
                    modd(m - 1) += detdom * domfate(m) '/ 5.0 ' Detritus asigned to different Detritus groups
                    modd(m - 1) /= (360.0 * Convert.ToSingle(gearratio))
                Next
                For m = 1 To ds.NGroups
                    modd(m - 1) /= ds.Bcell(1, 1, m)
                Next
                cp.PutIf(oxmodellm(n), ixoutputs(n), modd, -1)
            Next
            If (isnetworked) Then
                cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Finishtimestep, mastatuscodes.Ok, "Data Ready"), False, stationno)
            Else
                wx2 = True
            End If
        End If

    End Sub

    Public Sub LoadEcospaceScenario(ByVal dataSource As Object) Implements EwEPlugin.IEcospacePlugin.LoadEcospaceScenario

    End Sub

    Public Sub SaveEcospaceScenario(ByVal dataSource As Object) Implements EwEPlugin.IEcospacePlugin.SaveEcospaceScenario

    End Sub

    Public Sub EcospaceRunCompleted(ByVal EcoSpaceDatastructures As Object) Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        isended = True
        If (isnetworked) Then
            cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Finishtimestep, mastatuscodes.Waitingonresponse, "Model Over"), False, stationno)
            'cp.ps.SendMessage(New protmessage(stationno, protocols.Modelterminated, statuscodes.Ok, "Model finished"), False, stationno)
        End If
    End Sub
End Class
