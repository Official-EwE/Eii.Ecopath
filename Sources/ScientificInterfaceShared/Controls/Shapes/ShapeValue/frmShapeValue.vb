' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

' JS 12/12/17: This dialog has gotten way too cluttered. Needs to be rethought and rebuilt from scratch
' JS 27/02/19: Perhaps merge the functionality here with the increasingly powerful "Change Shape" UI?

''' ---------------------------------------------------------------------------
''' <summary>
''' Shape value edit form.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmShapeValue

#Region " Private helper classes "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, used to hold a reference to a predefined <see cref="eTimeSeriesType">
    ''' Time Series type enumerated value</see>, and presents this value in a human-readable
    ''' form using the resource string table.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Class cTSTComboBoxItem

        ''' <summary>Time series type enumerated value to associate with the item.</summary>
        Private m_timeSeriesType As eTimeSeriesType = eTimeSeriesType.NotSet
        Private m_desc As New cTimeSeriesTypeFormatter()

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="tst"><see cref="eTimeSeriesType">Time series type enumerated value</see>
        ''' to associate with an instance of this class.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(tst As eTimeSeriesType)
            ' Store type flag
            Me.m_timeSeriesType = tst
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the time series type enumerated value.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Function TimeSeriesType() As eTimeSeriesType
            Return Me.m_timeSeriesType
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Overridden to deliver the combo box item text.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function ToString() As String
            Return Me.m_desc.ToString(Me.m_timeSeriesType, eDescriptorTypes.Name)
        End Function

    End Class

#End Region ' Private helper classes

#Region " Private vars "

    Private m_shape As cShapeData = Nothing
    Private m_handler As cShapeGUIHandler = Nothing
    Private m_iNumPoints As Integer = 0
    Private m_SketchPad As ucSketchPad = Nothing
    Private m_displayMode As eDisplayMode = eDisplayMode.Monthly
    Private m_editMode As eDialogEditModeType = eDialogEditModeType.EditTimeSeries

    Private Enum eDialogEditModeType
        'AddTimeSeries
        EditTimeSeries
        EditForcing
    End Enum

    Private Const cNUMROWS_EMTPY As Integer = 100

    Private m_fpWeight As cEwEFormatProvider = Nothing
    Private m_fpXBase As cEwEFormatProvider = Nothing
    Private m_fpXMin As cEwEFormatProvider = Nothing
    Private m_fpXMax As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New(uic As cUIContext, handler As cShapeGUIHandler)

        Me.InitializeComponent()

        ' Config
        Me.UIContext = uic
        Me.m_grid.UIContext = uic

        ' Store shape
        Me.m_shape = Nothing
        Me.m_handler = handler

        ' Determine interface and display modes
        Me.m_displayMode = frmShapeValue.eDisplayMode.Monthly

        If (TypeOf (Me.m_handler) Is cTimeSeriesShapeGUIHandler) Then
            Me.m_editMode = eDialogEditModeType.EditTimeSeries
            Dim ds As cTimeSeriesDataset = uic.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)
            Select Case ds.TimeSeriesInterval
                Case eTSDataSetInterval.Annual
                    Me.m_displayMode = frmShapeValue.eDisplayMode.Yearly
                Case eTSDataSetInterval.TimeStep
                    Me.m_displayMode = frmShapeValue.eDisplayMode.Monthly
                Case Else
                    Debug.Assert(False)
            End Select
        Else
            Me.m_editMode = eDialogEditModeType.EditForcing
            If (TypeOf (Me.m_handler) Is cMediationShapeGUIHandler) Then
                Me.m_displayMode = frmShapeValue.eDisplayMode.Index
            End If
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="uic">The UI context to connect to.</param>
    ''' <param name="shape">The shape to edit, if any. If left to Nothing, this
    ''' interface assumes that a new time series is being added.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(uic As cUIContext, shape As cShapeData)

        Me.New(uic, If(shape Is Nothing, cShapeGUIHandler.GetShapeUIHandler(eDataTypes.TimeSeriesDataset, uic), cShapeGUIHandler.GetShapeUIHandler(shape, uic)))
        Me.m_shape = shape

    End Sub

#End Region ' Construction

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, describes how the shape value interface will display 
    ''' shape data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eDisplayMode As Integer
        ''' <summary>Display values per year</summary>
        Yearly
        ''' <summary>Display values per year and month</summary>
        Monthly
        ''' <summary>Display values per index</summary>
        Index
    End Enum

#End Region ' Public interfaces

#Region " Events "

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        If Me.UIContext Is Nothing Then Return

        MyBase.OnLoad(e)

        ' Kick off
        Select Case Me.m_editMode

            Case eDialogEditModeType.EditTimeSeries

                Dim ds As cTimeSeriesDataset = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)
                Me.NumPoints = ds.NumPoints

            Case eDialogEditModeType.EditForcing
                If (Me.m_shape Is Nothing) Then
                    Me.NumPoints = cNUMROWS_EMTPY
                Else
                    Me.NumPoints = If(Me.m_shape.IsSeasonal, cCore.N_MONTHS, Me.m_shape.nPoints)
                End If

            Case Else
                Debug.Assert(False)

        End Select

        Me.m_fpWeight = New cEwEFormatProvider(Me.UIContext, Me.m_txtWeight, GetType(Single))
        Me.m_fpXBase = New cEwEFormatProvider(Me.UIContext, Me.m_txtXBase, GetType(Single))
        Me.m_fpXMin = New cEwEFormatProvider(Me.UIContext, Me.m_txtXMin, GetType(Single))
        Me.m_fpXMax = New cEwEFormatProvider(Me.UIContext, Me.m_txtXMax, GetType(Single))

        Me.FillDataGrid()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)

        Me.m_fpWeight.Release()
        Me.m_fpXBase.Release()
        Me.m_fpXMin.Release()
        Me.m_fpXMax.Release()

        MyBase.OnClosed(e)

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        Dim bSucces As Boolean = False

        Select Case Me.m_editMode
            Case eDialogEditModeType.EditTimeSeries
                bSucces = Me.OnUpdateTimeSeries()
            Case eDialogEditModeType.EditForcing
                bSucces = Me.OnApplyForcing()
        End Select

        If bSucces Then
            'Done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        ' Done
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnTypeSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbTSType.SelectedIndexChanged

        Me.FillPoolCodeComboBoxes()
        Me.UpdateControls()

    End Sub

    Private Sub AnyTextChanged(sender As Object, e As System.EventArgs) _
        Handles m_txtWeight.TextChanged, m_lblNumPoints.TextChanged, m_txtName.TextChanged, m_txtXBase.TextChanged, m_txtXMin.TextChanged, m_tbxUnits.TextChanged
        'Lazy update
        Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateControls))
    End Sub

    Private Sub OnPoolSelectionChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbPoolCode.SelectedIndexChanged, m_cmbPoolCodeSec.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

    Private Sub cmbViewAs_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbViewAs.SelectedIndexChanged
        Me.NumPoints = If(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.nPoints)
        If Not Me.m_bInUpdate Then
            Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)
        End If
    End Sub

#End Region ' Events

#Region " Internal implementation "

    Private Property NumPoints() As Integer
        Get
            Return Me.m_iNumPoints
        End Get
        Set(iNumpoints As Integer)
            Me.m_iNumPoints = iNumpoints
            Me.m_lblNumPoints.Text = CStr(Me.m_iNumPoints)
        End Set
    End Property

    Private m_bInUpdate As Boolean = False

    Private Sub FillDataGrid()

        Me.SuspendLayout()
        Me.m_bInUpdate = True
        Me.m_grid.SuspendLayoutGrid()

        Select Case Me.m_editMode
            Case eDialogEditModeType.EditForcing
                Me.LoadForcingDataToGrid()
            Case eDialogEditModeType.EditTimeSeries
                Me.LoadTimeSeriesDataToGrid()
        End Select

        Me.m_bInUpdate = False
        Me.m_grid.ResumeLayoutGrid()
        Me.ResumeLayout()

    End Sub

    Private Sub LoadForcingDataToGrid()

        Dim iOffset As Integer = 0
        Dim bIsMediation As Boolean = Me.m_handler.IsMediation()
        Dim bIsTimeSeries As Boolean = Me.m_handler.IsTimeSeries()
        Dim bIsResponse As Boolean = Me.m_handler.IsResponse()

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES

        Me.m_txtName.Enabled = True
        Me.m_txtName.Text = Me.m_shape.Name

        ' Hide seasonal flag for mediation functions and time series
        Me.m_lblViewAs.Visible = Not bIsMediation And Not bIsTimeSeries
        Me.m_cmbViewAs.Visible = Me.m_lblViewAs.Visible

        Me.m_lblWeight.Visible = False
        Me.m_txtWeight.Visible = False

        Me.m_lblTSType.Visible = False
        Me.m_cmbTSType.Visible = False

        Me.m_lblPoolCode.Visible = False
        Me.m_cmbPoolCode.Visible = False

        Me.m_lblPoolCodeSec.Visible = False
        Me.m_cmbPoolCodeSec.Visible = False

        Me.m_lblNoOfPoints.Visible = False
        Me.m_tlpNoOfYears.Visible = False

        Me.m_lblXBase.Visible = bIsMediation And Not bIsResponse ' Ugh
        Me.m_txtXBase.Visible = Me.m_lblXBase.Visible

        Me.m_lblXMin.Visible = bIsResponse
        Me.m_txtXMin.Visible = Me.m_lblXMin.Visible
        Me.m_lblUnits.Visible = Me.m_lblXMin.Visible
        Me.m_tbxUnits.Visible = Me.m_lblXMin.Visible

        If (bIsMediation) Then
            Me.m_fpXBase.Value = DirectCast(Me.m_shape, cMediationBaseFunction).XBaseIndex
        End If

        If (bIsResponse) Then
            Dim fn As cEnviroResponseFunction = DirectCast(Me.m_shape, cEnviroResponseFunction)
            Me.m_fpXMin.Value = fn.ResponseLeftLimit
            Me.m_fpXMax.Value = fn.ResponseRightLimit
            Me.m_tbxUnits.Text = fn.Units

            Dim sf As IShapeFunction = cShapeFunctionFactory.GetShapeFunction(fn, Me.Core.PluginManager)
            If (sf IsNot Nothing) Then
                Me.m_fpXMin.Enabled = Not sf.IsDistribution
                Me.m_fpXMax.Enabled = Not sf.IsDistribution
            End If
        End If

        Me.IsSeasonal = Me.m_shape.IsSeasonal

        Me.NumPoints = If(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.nPoints)
        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    Private Sub LoadTimeSeriesDataToGrid()

        Dim ts As cTimeSeries = DirectCast(Me.m_shape, cTimeSeries)

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES
        Me.m_txtName.Enabled = True

        Me.m_lblWeight.Visible = True
        Me.m_txtWeight.Visible = True

        Me.m_lblTSType.Visible = True
        Me.m_cmbTSType.Visible = True

        Me.m_lblXBase.Visible = False
        Me.m_txtXBase.Visible = False

        Me.m_lblViewAs.Visible = False
        Me.m_cmbViewAs.Visible = False

        If (ts IsNot Nothing) Then
            Me.m_txtName.Text = ts.Name
            Me.m_fpWeight.Value = ts.WtType
        End If

        Me.FillTSTypeCombo(ts)

        Me.m_lblPoolCode.Visible = True
        Me.m_cmbPoolCode.Visible = True

        Me.FillPoolCodeComboBoxes()

        Me.m_btnOK.Visible = True
        Me.m_btnCancel.Visible = True

        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    ''' <summary>
    ''' Load an empty grid for time Series
    ''' </summary>
    Private Sub LoadEmptyGrid()

        Dim lstrTSNames As New List(Of String)
        Dim iNextTS As Integer = -1

        ' Get next TS sequential number
        For i As Integer = 1 To Me.Core.nTimeSeries
            lstrTSNames.Add(Me.Core.EcosimTimeSeries(i).Name)
        Next
        iNextTS = EwEUtils.Utilities.cStringUtils.GetNextNumber(lstrTSNames.ToArray(), My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES)

        'Set the plot title
        Me.Text = My.Resources.HEADER_ADD
        Me.m_txtName.Enabled = True
        Me.m_txtName.Text = cStringUtils.Localize(My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES, iNextTS)

        Me.m_lblWeight.Visible = True
        Me.m_txtWeight.Visible = True
        Me.m_txtWeight.Text = "1.0"

        Me.m_lblTSType.Visible = True
        Me.m_cmbTSType.Visible = True
        Me.FillTSTypeCombo(Nothing)
        Me.m_cmbTSType.Text = Me.m_cmbTSType.Items(0).ToString

        Me.m_lblPoolCode.Visible = True
        Me.m_cmbPoolCode.Visible = True

        Me.m_lblPoolCodeSec.Visible = True
        Me.m_cmbPoolCodeSec.Visible = True

        Me.m_lblXBase.Visible = False
        Me.m_txtXBase.Visible = False
        Me.m_lblXMax.Visible = False
        Me.m_txtXMax.Visible = False
        Me.m_lblXMin.Visible = False
        Me.m_txtXMin.Visible = False
        Me.m_lblUnits.Visible = False
        Me.m_tbxUnits.Visible = False

        Me.m_lblViewAs.Visible = False
        Me.m_cmbViewAs.Visible = False

        Me.FillPoolCodeComboBoxes()

        Me.m_grid.Clear(Me.NumPoints, (Me.m_editMode = eDialogEditModeType.EditTimeSeries))

    End Sub

    Private Function OnUpdateTimeSeries() As Boolean

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditTimeSeries)

        Dim ts As cTimeSeries = Nothing
        Dim iPoolCode As Integer
        Dim fts As cFleetTimeSeries = Nothing
        Dim gts As cGroupTimeSeries = Nothing
        Dim bSucces As Boolean = True

        'Get the time series
        ts = DirectCast(Me.m_shape, cTimeSeries)

        'Update the time series
        ts.Name = Me.m_txtName.Text
        ' Parse value using UI number settings
        ts.WtType = CSng(Me.m_fpWeight.Value)
        ts.TimeSeriesType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = Me.m_cmbPoolCode.SelectedIndex + 1

        'Assign the time series pool code to fleet index or group index
        Select Case cTimeSeries.Category(ts.TimeSeriesType)
            Case eTimeSeriesCategoryType.Fleet
                fts = CType(ts, cFleetTimeSeries)
                fts.FleetIndex = iPoolCode
            Case eTimeSeriesCategoryType.Group
                gts = CType(ts, cGroupTimeSeries)
                gts.GroupIndex = iPoolCode
            Case eTimeSeriesCategoryType.FleetGroup
                fts = CType(ts, cFleetTimeSeries)
                fts.FleetIndex = iPoolCode
                fts.GroupIndex = iPoolCode
        End Select

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_TIMESERIES_UPDATING, ts.Name))

        If bSucces Then
            ' Update the shape
            Me.m_grid.ApplyValues(ts)
            ts.Update()
            bSucces = Me.Core.UpdateTimeSeries()
        End If

        cApplicationStatusNotifier.EndProgress(Me.Core)

        Return bSucces
    End Function

    Private Function OnApplyForcing() As Boolean

        Dim bIsMediation As Boolean = Me.m_handler.IsMediation()
        Dim bIsTimeSeries As Boolean = Me.m_handler.IsTimeSeries()
        Dim bIsResponse As Boolean = Me.m_handler.IsResponse()

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditForcing)

        Dim ff As cForcingFunction = Nothing

        'Get the time series
        ff = DirectCast(Me.m_shape, cForcingFunction)

        ff.LockUpdates()

        ' Update the forcing function
        ff.Name = Me.m_txtName.Text
        ff.IsSeasonal = Me.IsSeasonal

        If (bIsMediation And Not bIsResponse) Then
            ' Parse value using UI number settings
            DirectCast(ff, cMediationBaseFunction).XBaseIndex = CInt(Me.m_fpXBase.Value)
        End If

        If (bIsResponse) Then
            DirectCast(ff, cEnviroResponseFunction).ResponseLeftLimit = CSng(Me.m_fpXMin.Value)
            DirectCast(ff, cEnviroResponseFunction).ResponseRightLimit = CSng(Me.m_fpXMax.Value)
            DirectCast(ff, cEnviroResponseFunction).Units = CStr(Me.m_tbxUnits.Text)
        End If

        ' Update the shape
        Me.m_grid.ApplyValues(ff)

        ' ToDo: apply seasonal pattern

        ff.UnlockUpdates(False)
        Return ff.Update()

    End Function

    Private Function OnAddTimeSeries() As Boolean

        Dim iFirstYear As Integer = 1
        Dim strName As String
        Dim sWeight As Single
        Dim iPoolCode As Integer
        Dim iPoolCodeSec As Integer
        Dim tsType As eTimeSeriesType
        Dim iDBID As Integer = -1
        Dim asValues As Single() = Nothing
        Dim bSucces As Boolean = True

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_TIMESERIES_ADDING, Me.m_txtName.Text))

        strName = Me.m_txtName.Text
        ' Parse value using UI number settings
        sWeight = CSng(Me.m_fpWeight.Value)
        tsType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = Me.m_cmbPoolCode.SelectedIndex + 1
        iPoolCodeSec = Me.m_cmbPoolCodeSec.SelectedIndex + 1
        iFirstYear = Me.m_grid.ValueStartRef
        asValues = Me.m_grid.Values(Me.m_iNumPoints)

        bSucces = Me.Core.AddTimeSeries(strName, iPoolCode, iPoolCodeSec, tsType, sWeight, asValues, iDBID)

        cApplicationStatusNotifier.EndProgress(Me.Core)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of crucial controls based on the content in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        Dim bIsMediation As Boolean = Me.m_handler.IsMediation
        Dim bIsTimeSeries As Boolean = Me.m_handler.IsTimeSeries
        Dim bEnableOk As Boolean = True

        Try
            ' Need a name to 'OK'
            bEnableOk = Not String.IsNullOrEmpty(Me.m_txtName.Text)

            If (bIsMediation) Then
                Dim sDummy As Single = 42.0!
                bEnableOk = bEnableOk And (Single.TryParse(Me.m_txtXBase.Text, sDummy) = True)
            End If

            ' Time series specific tests:
            If (bIsTimeSeries) Then
                If Me.m_shape IsNot Nothing Then

                    ' TS need a valid weight factor
                    ' Parse value using UI number settings
                    bEnableOk = bEnableOk And (Single.Parse(Me.m_txtWeight.Text) >= 0)
                    ' TS need a valid poolcode selection
                    bEnableOk = bEnableOk And (Me.m_cmbPoolCode.SelectedIndex >= 0)
                    ' .. and perhaps a valid secondary pool code too
                    Dim ts As cTimeSeries = DirectCast(Me.m_shape, cTimeSeries)
                    If (cTimeSeries.Category(ts.TimeSeriesType) = eTimeSeriesCategoryType.FleetGroup) Then
                        bEnableOk = bEnableOk And (Me.m_cmbPoolCodeSec.SelectedIndex >= 0)
                    End If
                Else
                    bEnableOk = False
                End If
            End If

            Me.m_lblPoolCode.Visible = (cTimeSeries.Category(Me.SelectedTimeSeriesType()) <> eTimeSeriesCategoryType.NotSet)
            Me.m_cmbPoolCode.Visible = Me.m_lblPoolCode.Visible

            Me.m_lblPoolCodeSec.Visible = (cTimeSeries.Category(Me.SelectedTimeSeriesType()) = eTimeSeriesCategoryType.FleetGroup)
            Me.m_cmbPoolCodeSec.Visible = Me.m_lblPoolCodeSec.Visible

            Me.m_lblViewAs.Visible = Not bIsMediation And Not bIsTimeSeries
            Me.m_cmbViewAs.Visible = Me.m_lblViewAs.Visible

        Catch ex As Exception
            bEnableOk = False
            Debug.Assert(False, ex.Message)
        End Try

        Me.m_btnOK.Enabled = bEnableOk

    End Sub

    Private Sub FillTSTypeCombo(ts As cTimeSeries)

        Dim itemNew As cTSTComboBoxItem = Nothing
        Dim itemSelected As cTSTComboBoxItem = Nothing
        Dim t As eTimeSeriesType = eTimeSeriesType.NotSet

        Me.m_cmbTSType.Items.Clear()

        If (ts IsNot Nothing) Then
            t = ts.TimeSeriesType
        End If

        For Each tst As eTimeSeriesType In cTimeSeriesFactory.CompatibleTypes(t)
            itemNew = New cTSTComboBoxItem(tst)
            Me.m_cmbTSType.Items.Add(itemNew)
            'Find selection
            If (ts IsNot Nothing) Then
                If (ts.TimeSeriesType = tst) Then
                    itemSelected = itemNew
                End If
            End If
        Next tst

        Me.m_cmbTSType.Sorted = True
        Me.m_cmbTSType.SelectedItem = itemSelected

    End Sub

    Private Sub FillPoolCodeComboBoxes()

        Dim fts As cFleetTimeSeries
        Dim gts As cGroupTimeSeries
        Dim cat As eTimeSeriesCategoryType = cTimeSeries.Category(Me.SelectedTimeSeriesType())
        Dim fmt As New cCoreInterfaceFormatter()

        Me.m_cmbPoolCode.Items.Clear()
        Me.m_cmbPoolCodeSec.Items.Clear()

        'Load pool code combo box based on the selected time series type
        Select Case cat

            Case eTimeSeriesCategoryType.Fleet,
                 eTimeSeriesCategoryType.FleetGroup

                Me.m_lblPoolCode.Text = cStyleGuide.ToControlLabel(My.Resources.HEADER_FLEET)
                For i As Integer = 1 To Me.Core.nFleets
                    Me.m_cmbPoolCode.Items.Add(fmt.ToString(Me.Core.EcopathFleetInputs(i)))
                Next

                If (cat = eTimeSeriesCategoryType.FleetGroup) Then
                    Me.m_lblPoolCodeSec.Text = cStyleGuide.ToControlLabel(My.Resources.HEADER_GROUP)
                    For i As Integer = 1 To Me.Core.nGroups
                        Me.m_cmbPoolCodeSec.Items.Add(fmt.ToString(Me.Core.EcopathGroupInputs(i)))
                    Next
                End If

                If (Me.m_shape IsNot Nothing) Then
                    fts = DirectCast(Me.m_shape, cFleetTimeSeries)
                    If ((fts.FleetIndex > 0 And fts.FleetIndex <= Me.Core.nFleets)) Then
                        Me.m_cmbPoolCode.SelectedIndex = fts.FleetIndex - 1
                    End If
                    If ((fts.GroupIndex > 0 And fts.GroupIndex <= Me.Core.nGroups)) Then
                        Me.m_cmbPoolCodeSec.SelectedIndex = fts.GroupIndex - 1
                    End If
                End If

            Case eTimeSeriesCategoryType.Group

                Me.m_lblPoolCode.Text = cStyleGuide.ToControlLabel(My.Resources.HEADER_GROUP)
                For i As Integer = 1 To Me.Core.nGroups
                    Me.m_cmbPoolCode.Items.Add(fmt.ToString(Me.Core.EcopathGroupInputs(i)))
                Next

                If (Me.m_shape IsNot Nothing) Then
                    gts = DirectCast(Me.m_shape, cGroupTimeSeries)
                    If ((gts.GroupIndex > 0 And gts.GroupIndex <= Me.Core.nGroups)) Then
                        Me.m_cmbPoolCode.SelectedIndex = gts.GroupIndex - 1
                    End If
                End If

            Case eTimeSeriesCategoryType.NotSet
                ' Ignore

        End Select

    End Sub

    Private Property SelectedTimeSeriesType() As eTimeSeriesType
        Get
            Dim item As cTSTComboBoxItem = DirectCast(Me.m_cmbTSType.SelectedItem, cTSTComboBoxItem)
            If (item Is Nothing) Then Return eTimeSeriesType.NotSet
            Return item.TimeSeriesType()
        End Get
        Set(t As eTimeSeriesType)
            For i As Integer = 0 To Me.m_cmbTSType.Items.Count - 1
                Dim item As cTSTComboBoxItem = DirectCast(Me.m_cmbTSType.Items(i), cTSTComboBoxItem)
                If item.TimeSeriesType = eTimeSeriesType.TimeForcing Then Me.m_cmbTSType.SelectedItem = item : Return
            Next
            Me.m_cmbTSType.SelectedItem = Nothing
        End Set
    End Property

    Private Property IsSeasonal() As Boolean
        Get
            Return Me.m_cmbViewAs.SelectedIndex = 1
        End Get
        Set(value As Boolean)
            Me.m_cmbViewAs.SelectedIndex = If(value, 1, 0)
        End Set
    End Property

#End Region ' Internal implementation

End Class
