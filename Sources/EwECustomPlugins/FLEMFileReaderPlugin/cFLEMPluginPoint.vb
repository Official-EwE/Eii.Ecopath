
Imports EwEPlugin
Imports EwECore

Public Class cFLEMPluginPoint
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcospaceInitRunCompletedPlugin
    Implements EwEPlugin.IEcospaceBeginTimestepPlugin
    Implements EwEPlugin.IEcospaceRunCompletedPlugin

#Region "Public data"

    Public ForcePPSalinity As Boolean
    Public VaryHabCapWithCultch As Boolean

    'Default monthly forcing data file name
    Public ForceFile As String = "C:\Assessments\Florida oysters\AP666nutsalt.nuo"

    Public iHabCapModGroup As Integer = 6 'Clutch

    Public core As EwECore.cCore

#End Region

#Region "Private data"

    Private Nactive As Integer 'number of active (depth>0) cells in forcing data file for each month
    Private ForceData(,,) As Single 'forcing input data (rel pp, salinity in Flem file)
    Private CellRatio As Single 'ratio of ecospace to physical model cell length (km/km)
    Private NrowForce As Integer, NcolForce As Integer  'map dimensions for Flem physical model forcing data
    Private FileNumber As Integer


    Private orgHabCap(,,) As Single
    Private orgRelPP(,) As Single

#Region "Plugin stuff"

    Private frmInterface As frmFLEMReader
    Private Context As ScientificInterfaceShared.Controls.cUIContext
    Private bInitOK As Boolean
    Private PathData As cEcopathDataStructures
    Private SimData As cEcosimDatastructures
    Private SpaceData As cEcospaceDataStructures

#End Region

#End Region

#Region "FLEM file reading and data forcing"


    ''' <summary>
    ''' Copy the salinity modifiers from the Ecosim into Ecospace spatial salinity modifiers
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitSalinityFromEcosim()

        If ForcePPSalinity = False Then Return

        'Turn on the Spatial fields
        SpaceData.SpatialFieldsInUse = True

        'In Ecosim the Salinity Modifiers are stored in the first index
        'Ecosim also contains fields for Temperature (in the second index) and a bunch of place holders that are empty
        SpaceData.nSpatialFields = 1

        'Redim SpatialFields the default size will be wrong
        ReDim SpaceData.SpatialField(SpaceData.InRow, SpaceData.InCol, SpaceData.NGroups)
        ReDim SpaceData.SpatialFieldOptimum(SpaceData.NGroups, SpaceData.nSpatialFields)
        ReDim SpaceData.SpatialFieldStdLeft(SpaceData.NGroups, SpaceData.nSpatialFields)
        ReDim SpaceData.SpatialFieldStdRight(SpaceData.NGroups, SpaceData.nSpatialFields)

        'Copy salinity modifiers from the Ecosim variables into the equivalent Ecospace variables
        For igrp As Integer = 1 To Me.SpaceData.NGroups
            'In Ecosim salintiy modifiers are stored in the first index
            SpaceData.SpatialFieldOptimum(igrp, 1) = Me.SimData.EnvResponseOpt(1, igrp)
            SpaceData.SpatialFieldStdLeft(igrp, 1) = Me.SimData.EnvResponseSdLeft(1, igrp)
            SpaceData.SpatialFieldStdRight(igrp, 1) = Me.SimData.EnvResponseSdRight(1, igrp)
        Next

    End Sub


    Private Sub InitFLEMFiles()
        'THE FOLLOWING CODE GOES BEFORE THE TIME LOOP IN FINDSPATIALEQUILIBRIUM
        Dim iForce As Integer, jForce As Integer 'indices for forcing map cells
        Dim CellLengthForce As Single 'cell length for forcing data (km)

        If ForcePPSalinity = False Then Return

        'Make a copy of the original HapCap and RelPP array so we can restore then after a run
        ReDim orgHabCap(SpaceData.InRow + 1, SpaceData.InCol + 1, SpaceData.NGroups)
        ReDim orgRelPP(SpaceData.InRow + 1, SpaceData.InCol + 1)
        Array.Copy(SpaceData.HabCap, Me.orgHabCap, Me.orgHabCap.Length)
        Array.Copy(SpaceData.RelPP, Me.orgRelPP, Me.orgRelPP.Length)

        FileNumber = FreeFile()

        If ForceFile <> "" Then
            FileOpen(FileNumber, ForceFile, OpenMode.Input)
            Input(FileNumber, Nactive)
            Input(FileNumber, NrowForce)
            Input(FileNumber, NcolForce)
            Input(FileNumber, CellLengthForce)
            ReDim ForceData(NrowForce + 1, NcolForce + 1, 2)
            For iForce = 1 To NrowForce + 1  'set default values for all the forcing array cells
                For jForce = 1 To NcolForce + 1
                    ForceData(iForce, jForce, 1) = 1  'default relative primary productivity
                    ForceData(iForce, jForce, 2) = 35  'default salinity
                Next
            Next
            CellRatio = SpaceData.CellLength / CellLengthForce
        End If

    End Sub


    Private Sub EcospaceTimeStep(ByVal iTime As Integer)
        'THE FOLLOWING CODE GOES INSIDE THE TIME LOOP IN FINDSPATIALEQUILIBRIUM NEAR THE TOP JUST AFTER ITT IS CALCULATED
        Dim iForce As Integer, jForce As Integer 'indices for forcing map cells
        Dim Bscale As Single

        If ForcePPSalinity Then
            If ForceFile <> "" Then   'read forcing data for this time step

                If EOF(FileNumber) Then  'close and reopn the forcefile to read data over again
                    FileClose(FileNumber)
                    FileOpen(FileNumber, ForceFile, OpenMode.Input)
                    LineInput(FileNumber) 'skip reading the map size information if this is second round
                End If

                For irec As Integer = 1 To Nactive 'read each of the forcing cell observations for this step
                    Input(FileNumber, iForce)
                    Input(FileNumber, jForce)
                    Input(FileNumber, ForceData(iForce, jForce, 1))
                    Input(FileNumber, ForceData(iForce, jForce, 2))
                Next

                'now have the forcing data for this month, put into forcing arrays for the ecospace map
                For i = 1 To SpaceData.InRow
                    For j = 1 To SpaceData.InCol
                        iForce = 1 + Int(CellRatio * i - 0.01)  'calculate forcing data cell row for this ecospace cell
                        jForce = 1 + Int(CellRatio * j - 0.01)  'calculate forcing data cell col for this ecospace cell
                        If iForce < 1 Then iForce = 1
                        If iForce > NrowForce Then iForce = NrowForce
                        If jForce < 1 Then jForce = 1
                        If jForce > NcolForce Then jForce = NcolForce
                        'Load salinity forcing into all the groups
                        'Apply a modifier to a group by changing its Salinity Tolerance Modifier in the Ecosim>Group info dialogue
                        For igrp As Integer = 1 To core.nGroups
                            SpaceData.SpatialField(i, j, igrp) = ForceData(iForce, jForce, 2)
                        Next
                        SpaceData.RelPP(i, j) = (ForceData(iForce, jForce, 1) - 0.5) ^ 0.5 'reduce the strong Flem nutrient effect here by using lower mean, power
                    Next
                Next

            End If ' ForceFile <> "" 
        End If

        '************modify habcap for oysters using culth biomass (group 6)
        If VaryHabCapWithCultch Then
            Bscale = SimData.StartBiomass(Me.iHabCapModGroup) * Me.SpaceData.nWaterCells / Me.SpaceData.TotHabCap(Me.iHabCapModGroup)
            For i = 1 To SpaceData.InRow
                For j = 1 To SpaceData.InCol
                    For ig As Integer = 1 To 4
                        SpaceData.HabCap(i, j, ig) = 0.8 * SpaceData.HabCap(i, j, ig) + 0.2 * SpaceData.Bcell(i, j, Me.iHabCapModGroup) / Bscale
                        If SpaceData.HabCap(i, j, ig) > 1 Then SpaceData.HabCap(i, j, ig) = 1
                    Next
                Next
            Next
        End If

        ''NOTE THERE NEEDS TO BE A CLOSEFILE(6) LINE AFTER THE END OF THE FINDSPATIALEQUILIBRIUM TIME LOOP
        'this is done in EcospaceRunCompleted
    End Sub




    ''' <summary>
    ''' Called when Ecospace has completed all its initialization and it about to start the time loop
    ''' </summary>
    ''' <param name="EcospaceDatastructures"></param>
    ''' <remarks></remarks>
    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Try

            InitSalinityFromEcosim()
            InitFLEMFiles()

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Called at the start of an Ecospace time step
    ''' </summary>
    ''' <param name="EcospaceDatastructures"></param>
    ''' <param name="iTime"></param>
    ''' <remarks></remarks>
    Public Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        Try

            EcospaceTimeStep(iTime)

        Catch ex As Exception

        End Try
    End Sub


    ''' <summary>
    ''' Called when a model has been loaded
    ''' </summary>
    ''' <param name="objEcoPath"></param>
    ''' <param name="objEcoSim"></param>
    ''' <param name="objEcoSpace"></param>
    ''' <remarks></remarks>
    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Try

            Dim EcoPathModel As Ecopath.cEcoPathModel = DirectCast(objEcoPath, Ecopath.cEcoPathModel)
            Dim EcoSimModel As Ecosim.cEcoSimModel = DirectCast(objEcoSim, Ecosim.cEcoSimModel)
            Dim EcoSpaceModel As cEcoSpace = DirectCast(objEcoSpace, cEcoSpace)

            Me.SimData = EcoSimModel.EcosimData
            Me.PathData = EcoPathModel.EcopathData
            Me.SpaceData = EcoSpaceModel.EcoSpaceData
            Me.ForcePPSalinity = False
            Me.VaryHabCapWithCultch = False

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Called once an Ecospace run has completed
    ''' </summary>
    ''' <param name="EcoSpaceDatastructures"></param>
    ''' <remarks></remarks>
    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        Try

            If Not ForcePPSalinity Then Return
            FileClose(FileNumber)

            'Restore the arrays we modified so Ecospace and initialize properly
            Array.Copy(Me.orgHabCap, SpaceData.HabCap, Me.orgHabCap.Length)
            Array.Copy(Me.orgRelPP, SpaceData.RelPP, Me.orgRelPP.Length)

        Catch ex As Exception

        End Try

    End Sub


#End Region

#Region "Plugin Requirements"

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "FLEM reader"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Toggle the FLEM file reader plugin"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        frmPlugin = Me.MainInterface

    End Sub


    Public ReadOnly Property NavigationTreeItemLocation As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceTools"
        End Get
    End Property

    Private Function MainInterface() As System.Windows.Forms.Form
        Dim bHasUI As Boolean = False

        If (Me.frmInterface IsNot Nothing) Then
            bHasUI = Not Me.frmInterface.IsDisposed
        End If

        If Not bHasUI Then
            Me.frmInterface = New frmFLEMReader()
            Me.frmInterface.UIContext = Me.Context
            Me.frmInterface.Text = Me.ControlText
            Me.frmInterface.Init(Me)
        End If

        Return Me.frmInterface

    End Function

    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.Context = DirectCast(uic, ScientificInterfaceShared.Controls.cUIContext)
        If Context IsNot Nothing Then
            core = Context.Core
        End If
    End Sub


    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                core = DirectCast(core, EwECore.cCore)

                ' m_myForm = New frmInvokeModels(Me)
                '  m_myForm.Show()

                bInitOK = True
                System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try

    End Sub

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Name">IPlugin.Name</see> implementation.</summary>
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "FLEM File Reader Plugin"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Description">IPlugin.Description</see> implementation.</summary>
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Forces RelPP and Salinity modifiers from a FLEM nutrient file."
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.</summary>
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.</summary>
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

#End Region 

End Class
