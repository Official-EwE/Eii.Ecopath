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

Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEPlugin
Imports EwEUtils.Core
Imports System.Threading
Imports Couplerlib
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class GOTMplugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IAutolaunchPlugin
    Implements EwEPlugin.IUIContextPlugin
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
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcospacePlugin

    Private m_core As EwECore.cCore
    Private m_uic As cUIContext = Nothing
    ' The Ecospace model
    Private m_ecospace As cEcoSpace
    Private m_bInitOK As Boolean
    Private isnetworked As Boolean
    Private spinupdays As Integer
    Private longitude, latitude, longinterval, latinterval As Double
    Private latlen, longlen As Integer
    Private modelname, hosttono As String
    Private stationno As Integer
    Private m_PluginInterface As FormGotmPluggin
    Private cp As Couplerlib.CCouplerlib = Nothing
    Private contclick As Boolean = False
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
    Dim missmatch(365) As Single
    Public autorescale, isrescale, isrescale2 As Boolean
    Public rexdim, reydim As Integer

    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    ''' 
    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) _
        Implements EwEPlugin.ICorePlugin.CoreInitialized
        m_EcosimDS = DirectCast(objEcoSim, Ecosim.cEcoSimModel).EcosimData
        m_ecospace = DirectCast(objEcoSpace, cEcoSpace)
        autorescale = False
    End Sub

    Public Function SaveModel(ByVal dataSource As Object) As Boolean _
        Implements Global.EwEPlugin.IEcopathPlugin.SaveModel
        Return True
    End Function

    Public Function LoadModel(ByVal dataSource As Object) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.LoadModel
        ' NOP
    End Function

    Public Sub ModelLoaded(ByVal is3d As Boolean)
        Dim ng As Integer
        ng = m_core.nGroups
        'Dim longitude As Double = m_core.m_EcoSpaceData.lon1
        Dim nx(ng) As String
        Dim nu(ng) As String ' EwE must supply units 
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
        XmlSpecify(ng, nx, nu, nc, is3d)
    End Sub

    Public Sub Initialize(ByVal core As Object) Implements Global.EwEPlugin.IPlugin.Initialize
        ' Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        EwEstat = 0
        GOTMstat = 0
        isrescale = False
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
        wait(wx2, False)
        wx2 = False
        If isnetworked Then
            m_PluginInterface.gotmremoteserver()
        Else
            m_PluginInterface.gotmserver()
        End If
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

#If 0 Then
    ''' ===========================================================================
    ''' <summary>
    ''' Plug-in that should auto-launch when a consuming GUI is loaded
    ''' </summary>
    ''' ===========================================================================
    Public Interface IAutolaunchPlugin
        ' Inherits IGUIPlugin

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
#End If

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = uic

        If (uic IsNot Nothing) And (Me.m_PluginInterface Is Nothing) Then
            m_PluginInterface = New FormGotmPluggin(m_uic)
            m_PluginInterface.plugin = Me
            m_PluginInterface.GOTMfront()
            Me.thread = New Thread(AddressOf Me.displaythread)
            Me.thread.Start()
        End If

    End Sub

    Public Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        If contclick Then
            EcoModelRunInitialized(EcosimDatastructures, Nothing, False)
        End If
    End Sub

    Public Sub EcoModelRunInitialized(ByVal EcosimDatastructures As Object, ByVal EcospaceDatastructures As Object, ByVal UseEcospace As Boolean)
        Dim ds As cEcosimDatastructures
        Dim maxsize As Integer
        Dim initok As Boolean
        wait(wx1, True)
        ds = EcosimDatastructures
        maxsize = 0

        ifinputs = cp.GetIfAddress(imodellm, modelname, True, False) ' inputs
        ifoutputs = cp.GetIfAddress(omodellm, modelname, False, True) ' outputs
        ixinputs = cp.GetIfAddress(ixmodellm, modelname, True, True) ' inputs
        ixoutputs = cp.GetIfAddress(oxmodellm, modelname, False, False) ' outputs
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
        ' m_PluginInterface.Status(GOTMstat, EwEstat)
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
        ' Threading.WaitHandle.WaitAll(WH1)
    End Sub

    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        If contclick Then
            EcoModelInitialized(EcosimDatastructures, Nothing, False)
        End If
    End Sub

    Public Sub EcoModelInitialized(ByVal EcosimDatastructures As Object, ByVal EcospaceDatastructures As Object, ByVal UseEcospace As Boolean)
        Dim ds As cEcosimDatastructures
        Dim eds As cEcospaceDataStructures
        Dim a As Integer
        If GOTMstat > 0 Then
            ds = EcosimDatastructures
            'm_core = cCore.GetInstance()
            Dim ng As Integer
            ng = m_core.nGroups
            ' a = m_core.m_EwEModelUnitCurrency
            ds.NumYears = numyears
            nsy = ds.NumStepsPerYear
            If numyears > 0 Then
                ds.RedimTime()
            End If
            If UseEcospace Then
                eds = EcospaceDatastructures
                'eds.NumStep = 365
                'eds.TotalTime = 365
            End If
            EwEstat = 1
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            m_PluginInterface.Timedisplay(numyears)
        End If
    End Sub

    Public Sub XmlSpecify(ByVal nogroups As Integer, ByVal nxi As String(), ByVal nui As String(), ByVal nci As String(), ByVal is3di As Boolean)
        Dim Specification As Xml.XmlDocument = New Xml.XmlDocument
        Dim NL As Xml.XmlNodeList
        Dim Node As Xml.XmlNode

        Dim n, m As Integer
        Specification.Load(sdictname + "\EWEtemplate.xml")
        Dim Child, GChild, GGChild, GGGChild As Xml.XmlNode
        NL = Specification.GetElementsByTagName("ModelName")
        modelname = NL(0).InnerText
        NL = Specification.GetElementsByTagName("Interface")
        EcospaceSpecify(Specification, longitude, latitude, longinterval, latinterval, longlen, latlen, is3di)
        For n = 0 To NL.Count - 1
            Node = NL(n)
            For k = 0 To Node.ChildNodes.Count - 1
                If Node.ChildNodes(k).Name = "DataCollection" Then
                    For m = 0 To nogroups + 2
                        Child = Specification.CreateElement("Data")
                        GChild = Specification.CreateElement("Name")
                        If m < nogroups Then
                            GChild.InnerText = nxi(m)
                        ElseIf m = nogroups Then
                            GChild.InnerText = "Temperature"
                        ElseIf m = nogroups + 1 Then
                            GChild.InnerText = "Salinity"
                        Else
                            GChild.InnerText = "Depth"
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
                        ElseIf m = nogroups + 1 Then
                            GGGChild.InnerText = "Salinity"
                        Else
                            GGGChild.InnerText = "Depth"
                        End If
                        GGChild.AppendChild(GGGChild)
                        GGGChild = Specification.CreateElement("Symbol")
                        If m < nogroups Then
                            GGGChild.InnerText = nxi(m).Substring(0, 3)
                        ElseIf m = nogroups Then
                            GGGChild.InnerText = "Tmp"
                        ElseIf m = nogroups + 1 Then
                            GGGChild.InnerText = "Sal"
                        Else
                            GGGChild.InnerText = "Dep"
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
                        ElseIf m = nogroups + 1 Then
                            GGGChild.InnerText = "Mean Salinity"
                        Else
                            GGGChild.InnerText = "Bathymetric Depth"
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
                            ElseIf m = nogroups + 1 Then
                                GChild.InnerText = "psu"
                            Else
                                GChild.InnerText = "m"
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

    Public Sub EcospaceSpecify(ByRef Specification As Xml.XmlDocument, ByVal lonstart As Double, ByVal latstart As Double, ByVal longint As Double, ByVal latint As Double, ByVal longsz As Integer, ByVal latsz As Integer, ByVal is3d As Boolean)
        Dim NL, CL, GCL As Xml.XmlNodeList
        Dim Node, NewNode, NewChild As Xml.XmlNode
        Dim NodeName As String() = {"Longitude", "Latitude", "Depth"}
        Dim nodims As Integer
        NL = Specification.GetElementsByTagName("GridData")
        For n As Integer = 0 To NL.Count - 1
            Node = NL(n)
            CL = Node.ChildNodes
            If CL(0).Name = "GridFormRaster3D" Then
                nodims = 3
            End If
            If CL(0).Name = "GridFormRaster2D" Then
                nodims = 2
            End If
            If CL(0).Name = "GridFormNone" Then
                Node.RemoveChild(CL(0))
                If is3d Then
                    NewNode = Specification.CreateElement("GridFormRaster3D")
                    NewNode.InnerText = "3D"
                    nodims = 3
                Else
                    NewNode = Specification.CreateElement("GridFormRaster2D")
                    NewNode.InnerText = "2D"
                    nodims = 2
                End If
                Node.AppendChild(NewNode)
            End If
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
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString(0.0)
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Interval")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(longint)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(latint)
                End If
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString(1.0) 'Max depth all of sea
                End If
                NewNode.AppendChild(NewChild)
                NewChild = Specification.CreateElement("Length")
                If m = 1 Then
                    NewChild.InnerText = Convert.ToString(longsz)
                End If
                If m = 2 Then
                    NewChild.InnerText = Convert.ToString(latsz)
                End If
                If m = 3 Then
                    NewChild.InnerText = Convert.ToString(1)
                End If
                NewNode.AppendChild(NewChild)
                Node.AppendChild(NewNode)
            Next

        Next

    End Sub

    Public Sub EcosimSubTimeStepBegin(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepBegin

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
                        ' If (ds.TemperatureForceNo > 0) Then
                        'ds.tval(ds.TemperatureForceNo) = modd(m)
                        'End If
                    End If
                    If d = BiomassAtTimestep.Length + 1 Then
                        'If (ds.SalinityForceNo > 0) Then
                        'ds.tval(ds.SalinityForceNo) = modd(m)
                        'End If
                    End If
                    If d < BiomassAtTimestep.Length Then
                        BiomassAtTimestep(cp.OrgReference(imodellm(n), ifinputs(n), m) + 1) = modd(m)  ' biomass at timestep is 1 indexed
                    End If
                Next
            Next
            For n = 1 To ds.nGroups
                oldbiomass(n) = BiomassAtTimestep(n)
            Next
        End If
    End Sub

    Public Sub EcosimSubTimeStepEnd(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepEnd

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

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcosimBeginTimestepPlugin.EcosimBeginTimeStep
        ' NOP
    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep
        ' NOP
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
        ' Dim b As Integer = AppDomain.GetCurrentThreadId()
        While (Not waiter And Not isabort)
            a *= 1
            Threading.Thread.Sleep(1)

            System.Windows.Forms.Application.DoEvents()
        End While
        If (sinvoke) Then
            m_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            m_PluginInterface.Invoke(New FormGotmPluggin.InvokeDelegate(AddressOf m_PluginInterface.Progressbar))
            'm_PluginInterface.Invoke(New FormGotmPluggin.InvokeSDelegate(AddressOf m_PluginInterface.Progresstext)) No longer a part of this form
        End If
        waiter = False
    End Sub

    Public Function setstep(ByRef duration As Integer, ByRef stepsz As Double, ByVal gearratioi As Integer, ByVal isnet As Boolean, ByVal istation As Integer, ByVal connectname As String, ByVal dictname As String, ByVal specname As String, ByVal yearoff As Decimal, ByVal monthoff As Decimal) As Double
        ' Python.Runtime.PythonEngine.EndAllowThreads(tp)
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
        ' TRANSFER DATA WHILE THREAD IS SUSPENDED
        Threading.Thread.Sleep(1)
        WH3(0).Set() 'Ewe Thread starts
        wx3 = True
        If isended Then
            EwEstat = 3
            'm_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            If Not isabort Then
                'm_PluginInterface.Invoke(New InvokeStatusDelegate(AddressOf m_PluginInterface.Status))
            End If
        Else
            wait(wx2, False)
            ' Threading.WaitHandle.WaitAll(WH2) 'wait for ewe to end
        End If
        Return isended
    End Function

    Public Function Starting(ByVal cpi As CCouplerlib, ByVal gearratioi As Decimal, ByVal usesocket As Boolean) As Boolean
        ' now we check the xml files
        ' normall call the interface with the name of the interface file
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
        ' Threading.WaitHandle.WaitAll(WH2)
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
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False
        EwEstat = 0
        GOTMstat = 0
        contclick = True
        isended = False
        'Interface item has been clicked
        'Show the Ecotroph interface
        If m_bInitOK Then
            ' Test if form still exists

            If Me.m_PluginInterface IsNot Nothing And Me.m_PluginInterface.IsDisposed Then
                Me.m_PluginInterface = New FormGotmPluggin(m_uic)
            End If

            ' JS: EwE framework will show the form after it has properly been nested in the main UI

            '' Activate the form
            'Me.m_PluginInterface.Show()

            ' Pass form reference back to calling app
            f = Me.m_PluginInterface

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel
            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu
            End If
        Else
            ' Debug.Assert(False, "Plugin was not initialized properly.")
        End If

    End Sub

    Function Autolaunch() As Boolean Implements EwEPlugin.IAutolaunchPlugin.Autolaunch
        Return False
    End Function

#End Region 'GUI

    Public Sub EcosimPreDataInitialized(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreDataInitialized
        ' NOP
    End Sub

    Public Sub EcosimPreRunInitialized(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreRunInitialized
        If GOTMstat > 0 Then
            Me.m_EcosimDS.StepsPerMonth = 30 / gearratio
            Me.m_EcosimDS.bMultiThreaded = True
        End If
    End Sub

    Public Sub EcospaceInitialized(ByVal EcospaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized

        If contclick Then
            EcoModelInitialized(Me.m_EcosimDS, EcospaceDatastructures, True)
            Dim sscale As Double = EcospaceDatastructures.CellLength * (1.0 / (60.0 * 1.852))
            latlen = EcospaceDatastructures.InCol
            longlen = EcospaceDatastructures.InRow
            longitude = EcospaceDatastructures.lon1
            latitude = EcospaceDatastructures.lat1 - latlen * sscale
            EcospaceDatastructures.Totaltime = numyears
            EcospaceDatastructures.TimeStep = gearratio / 365.0
            m_core.EcospaceModelParameters.NumberOfTimeStepsPerYear = 365 / gearratio
            longinterval = sscale 'long(1) - EcospaceDatastructures.long(0)
            latinterval = sscale '(1) - EcospaceDatastructures.lat(0)
            For n = 0 To 365
                If (n > 100) And (n < 180) Then
                    missmatch(n) = 1.0
                Else
                    missmatch(n) = 0.0
                End If
            Next
            'm_PluginInterface.EcospaceSpecify(longitude, latitude, longinterval, latinterval, longlen, latlen)
            EwEstat = 1
            EcoModelRunInitialized(Me.m_EcosimDS, EcospaceDatastructures, True)

        End If
    End Sub

    Public Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep

        Dim noip, n, ycell As Integer
        Dim IParray As Integer()
        Dim modd As Double()()
        Dim xdim, ydim As Integer
        Dim watercellno As Integer
        Dim xmax, ymax As Integer
        Dim habitat2 As Single()
        Dim qc As cEcoSpace
        Dim etemp, esal, ezoop, ezoopi As Integer
        Dim firstxpoint, firstypoint As Boolean
        isrescale = False
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
            'ds.nEnvironmentalDriverLayers = 1
            'ds.EnvironmentalLayerMap(0, 0, 0) = 1.0
            etemp = -1
            esal = -1
            ezoop = -1
            For n = 0 To ds.nEnvironmentalDriverLayers
                If ds.EnvironmentalLayerName(n) = "Temperature" Then
                    etemp = n
                End If
                If ds.EnvironmentalLayerName(n) = "Salinity" Then
                    esal = n
                End If
                If ds.EnvironmentalLayerName(n) = "Zooplankton" Then
                    ezoop = n
                    For m = 0 To ds.NGroups
                        If ds.EcoPathData.GroupName(m) = "Herbivorous and Omnivorous zooplankton (copepods)" Then
                            ezoopi = m
                        End If
                    Next
                End If
            Next

            If isrescale Then
                ' JS 29Oct2013: Disabled resizing so we can leave Ecospace as-is in the release of EwE 6.4. To rethink for future versions
#If 0 Then
                ds.InCol = reydim
                ds.InRow = rexdim
                'ds.nEnvironmentalDriverLayers = 1
                'Preserve Habitat prefs
                Dim temphabs(ds.NGroups, ds.NoHabitats) As Single
                Array.Copy(ds.PrefHab, temphabs, ds.NGroups * ds.NoHabitats)
                ds.ReDimMapDims()
                Array.Copy(temphabs, ds.PrefHab, ds.NGroups * ds.NoHabitats)
                For n = 1 To ds.InRow
                    ' ds.Width(n) = 1.0
                Next
                ds.allocate(ds.PHabType, ds.InRow, ds.InCol, ds.NoHabitats)
                'ds.RedimHabitatVariables(False)
                ReDim ds.IFDweight(rexdim, reydim, ds.NGroups)
                ReDim ds.PredCell(rexdim, reydim, ds.NGroups)
                ReDim ds.EffortSpace(ds.nFleets, rexdim, reydim)
                ReDim m_core.m_spaceresults.m_sumEffortMap(rexdim, reydim)
                ds.redimTimeStepResults(ds.nTimeSteps)
                m_ecospace.redimForRun()
                m_ecospace.InitSpaceSolverThreads()
#End If
            End If
            ymax = ds.InCol
            xmax = ds.InRow
            Dim data As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
            Dim val As cValue = Nothing
            Dim meta As cVariableMetaData = Nothing
            For n = 0 To Me.imodellm.Count - 1
                If cp.oneway Then
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), modd, ymax * xmax, isrescale)
                Else
                    noip = cp.GetIf(imodellm(n), ifinputs(n), omodellm(n), ifoutputs(n), ixmodellm(n), ixinputs(n), modd(0))
                End If
                For m = 0 To noip - 1
                    d = cp.OrgReference(imodellm(n), ifinputs(n), m) + 1
                    If isrescale Then
                        If d = ds.NGroups + 3 Then
                            ReDim ds.jWaterCellIndex(xmax * ymax)
                            ReDim ds.iWaterCellIndex(xmax * ymax)
                            ReDim ds.iStartRow(xmax)
                            ReDim ds.iEndRow(xmax)
                            ReDim ds.jStartCol(ymax)
                            ReDim ds.jEndCol(ymax)
                            watercellno = 0
                            For xdim = 1 To xmax
                                firstypoint = True
                                For ydim = 1 To ymax
                                    ycell = (xmax - xdim) * ymax + (ydim - 1)
                                    ds.Depth(xdim, ydim) = -(modd(m)(ycell) > 0)
                                    ds.DepthA(xdim, ydim) = modd(m)(ycell)
                                    If (modd(m)(ycell) > 0) Then
                                        If (firstypoint) Then
                                            ds.iStartRow(xdim) = ydim
                                            firstypoint = False
                                        End If
                                        ds.iEndRow(xdim) = ydim
                                        watercellno += 1
                                        ds.iWaterCellIndex(watercellno) = xdim
                                        ds.jWaterCellIndex(watercellno) = ydim
                                    End If
                                Next
                            Next
                            For ydim = 1 To ymax
                                firstxpoint = True
                                For xdim = 1 To xmax
                                    ycell = (xmax - xdim) * ymax + (ydim - 1)
                                    If (modd(m)(ycell) > 0) Then
                                        If firstxpoint Then
                                            ds.jStartCol(ydim) = xdim
                                            firstxpoint = False
                                        End If
                                        ds.jEndCol(ydim) = xdim
                                    End If
                                Next
                            Next
                            ds.iTotalWaterCells = watercellno
                            ds.nWaterCells = watercellno
                            m_core.EcospaceBasemap.SetVariable(eVarNameFlags.InRow, xmax)
                            m_core.EcospaceBasemap.SetVariable(eVarNameFlags.InCol, ymax)
                            ' If (ds.TemperatureForceNo > 0) Then
                            'ds.tval(ds.TemperatureForceNo) = modd(m)
                            'End If
                        End If
                    End If
                    For xdim = 1 To xmax
                        For ydim = 1 To ymax
                            ycell = (xmax - xdim) * ymax + (ydim - 1)
                            ds.Bcell(xdim, ydim, (cp.OrgReference(imodellm(n), ifinputs(n), m) + 1)) = modd(m)(ycell)  ' biomass at timestep is 1 indexed
                        Next
                    Next
                    If d = ds.NGroups + 1 And etemp > -1 Then
                        For xdim = 1 To xmax
                            For ydim = 1 To ymax
                                ycell = (xmax - xdim) * ymax + (ydim - 1)
                                ds.EnvironmentalLayerMap(etemp, xdim, ydim) = modd(m)(ycell)
                            Next
                        Next
                    End If
                    If d = ds.NGroups + 2 And esal > -1 Then
                        For xdim = 1 To xmax
                            For ydim = 1 To ymax
                                ycell = (xmax - xdim) * ymax + (ydim - 1)
                                ds.EnvironmentalLayerMap(esal, xdim, ydim) = modd(m)(ycell)
                            Next
                        Next
                    End If
                    If d = ezoopi And ezoop > -1 Then
                        For xdim = 1 To xmax
                            For ydim = 1 To ymax
                                ycell = (xmax - xdim) * ymax + (ydim - 1)
                                ds.EnvironmentalLayerMap(ezoop, xdim, ydim) = 10.0 * modd(m)(ycell) / ds.EcoPathData.B(ezoopi) * missmatch(iTime Mod 360)
                            Next
                        Next
                    End If
                Next
            Next
            If isrescale Then
                ' JS 29Oct2013: Disabled resizing so we can leave Ecospace as-is in the release of EwE 6.4. To rethink for future versions
#If 0 Then
                If isrescale2 Then
                    cp.GetHabitat(habitat2, xmax * ymax)
                    'Dim tbm As cEcospaceLayerHabitat = New cEcospaceLayerHabitat(m_core, m_core.EcospaceBasemap, 0)
                    'm_core.EcospaceBasemap.SetVariable(eVarNameFlags.LayerHabitat, 1)
                    For xdim = 1 To xmax
                        For ydim = 1 To ymax
                            ycell = (xmax - xdim) * ymax + (ydim - 1)
                            For k = 0 To ds.NoHabitats
                                ds.PHabType(xdim, ydim, k) = 0.0
                                If habitat2(ycell) = k Then
                                    ds.PHabType(xdim, ydim, k) = 1.0
                                End If
                            Next
                            'm_core.EcospaceBasemap.LayerHabitat(1).Cell(xdim, ydim) = habitat2(ycell)
                            ' For oref = 1 To ds.NGroups
                            'ds.Bcell(xdim, ydim, oref) = ds.EcoPathData.B(oref)
                            'Next
                        Next
                    Next
                    ds.bHasCapacityChanged = True
                    m_ecospace.SetHabCap()
                    m_ecospace.initSpatialEquilibrium()
                    m_core.m_spaceresults.iTimeStep = 1
                    watercellno = ds.nWaterCells
                    isrescale2 = False
                    isrescale = False
                    autorescale = False
                End If
                ds.nWaterCells = 1
                m_ecospace.summarizeTimeStepData(1, 0)
                ds.nWaterCells = watercellno
#End If
            Else
                ds.bHasCapacityChanged = True
                m_ecospace.SetHabCap()
                'ds.nSumTimeSteps -= 1
                'ds.RedimHabitatVariables(True)
            End If
        End If

    End Sub

    Public Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

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
            If isrescale Then

            End If
        End If

    End Sub

    Public Sub LoadEcospaceScenario(ByVal dataSource As Object) _
        Implements EwEPlugin.IEcospacePlugin.LoadEcospaceScenario
        ' NOP
    End Sub

    Public Sub SaveEcospaceScenario(ByVal dataSource As Object) _
        Implements EwEPlugin.IEcospacePlugin.SaveEcospaceScenario
        ' NOP
    End Sub

    Public Sub EcospaceRunCompleted(ByVal EcoSpaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        isended = True
        If (isnetworked) Then
            cp.ps.SndMessage(New Cprotmessage(stationno, maprotocols.Finishtimestep, mastatuscodes.Waitingonresponse, "Model Over"), False, stationno)
            'cp.ps.SendMessage(New protmessage(stationno, protocols.Modelterminated, statuscodes.Ok, "Model finished"), False, stationno)
        End If
    End Sub

    Public Function CloseModel() As Boolean _
        Implements EwEPlugin.IEcopathPlugin.CloseModel

        isabort = True
        If contclick Then
            m_PluginInterface.ClosePipes()
            m_PluginInterface.StoreInRegistry()
            m_PluginInterface.isnotended = False
            isended = True
            If cp IsNot Nothing Then

                cp.CleanUp()
            End If
            Threading.Thread.Sleep(20)
        End If

    End Function

    Public Sub CloseEcospaceScenario() _
        Implements EwEPlugin.IEcospacePlugin.CloseEcospaceScenario
        ' NOP
    End Sub

End Class
