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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

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
        Public Sub New(ByVal tst As eTimeSeriesType)
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
            Return Me.m_desc.GetDescriptor(Me.m_timeSeriesType, eDescriptorTypes.Name)
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
        AddTimeSeries
        AddForcing
        EditTimeSeries
        EditForcing
    End Enum

    Private Const cNUMROWS_EMTPY As Integer = 100

    Private m_fpWeight As cEwEFormatProvider = Nothing
    Private m_fpXBase As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="uic">The UI context to connect to.</param>
    ''' <param name="shape">The shape to edit, if any. If left to Nothing, this
    ''' interface assumes that a new time series is being added.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                   Optional ByVal shape As cShapeData = Nothing)

        Me.InitializeComponent()

        ' Config
        Me.UIContext = uic
        Me.m_grid.UIContext = uic

        ' Store shape
        Me.m_shape = shape
        Me.m_handler = cShapeGUIHandler.GetShapeUIHandler(shape)

        ' Determine interface mode
        If (shape Is Nothing) Then
            Me.m_editMode = eDialogEditModeType.AddTimeSeries
        Else
            Me.m_editMode = DirectCast(IIf(TypeOf shape Is cTimeSeries, eDialogEditModeType.EditTimeSeries, eDialogEditModeType.EditForcing), eDialogEditModeType)
        End If

        ' Determine display mode
        If TypeOf (shape) Is cMediationBaseFunction Then
            Me.m_displayMode = frmShapeValue.eDisplayMode.Index
        ElseIf TypeOf (shape) Is cTimeSeries Then
            Select Case (DirectCast(shape, cTimeSeries)).Interval
                Case eTSDataSetInterval.Annual
                    Me.m_displayMode = frmShapeValue.eDisplayMode.Yearly
                Case eTSDataSetInterval.Monthly
                    Me.m_displayMode = frmShapeValue.eDisplayMode.Monthly
                Case Else
                    Debug.Assert(False)
            End Select
        Else
            Me.m_displayMode = frmShapeValue.eDisplayMode.Monthly
        End If

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

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        If Me.UIContext Is Nothing Then Return

        MyBase.OnLoad(e)

        ' Kick off
        If Me.m_shape Is Nothing Then
            Me.NumPoints = cNUMROWS_EMTPY
        Else
            If Me.m_shape.IsSeasonal Then
                Me.NumPoints = cCore.N_MONTHS
            Else
                Me.NumPoints = Me.m_shape.nPoints
            End If
        End If

        Me.m_fpWeight = New cEwEFormatProvider(Me.UIContext, Me.m_txtWeight, GetType(Single))
        Me.m_fpXBase = New cEwEFormatProvider(Me.UIContext, Me.m_txtXBase, GetType(Single))

        Me.FillDataGrid()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOK.Click

        Dim bSucces As Boolean = False

        Select Case Me.m_editMode
            Case eDialogEditModeType.AddTimeSeries
                bSucces = Me.OnAddTimeSeries()
            Case eDialogEditModeType.EditTimeSeries
                bSucces = Me.OnUpdateTimeSeries()
            Case eDialogEditModeType.EditForcing
                bSucces = Me.OnApplyForcing()
            Case eDialogEditModeType.AddForcing
                ' Mode not supported yet (anymore?)
                Debug.Assert(False)
        End Select

        If bSucces Then
            'Done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCancel.Click

        ' Done
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnTypeSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbType.SelectedIndexChanged

        Me.FillPoolCodeComboBox()
        Me.UpdateControls()

    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_txtWeight.TextChanged, m_lblNumPoints.TextChanged, m_txtName.TextChanged
        'Lazy update
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

    Private Sub OnPoolSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbPoolCode.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

    Private Sub cmbViewAs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbViewAs.SelectedIndexChanged
        Me.NumPoints = CInt(IIf(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.nPoints))
        If Not Me.m_bInUpdate Then
            Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)
        End If
    End Sub

#End Region ' Events

#Region " Internal implementation "

    Private Property NumPoints() As Integer
        Get
            Return m_iNumPoints
        End Get
        Set(ByVal iNumpoints As Integer)
            Me.m_iNumPoints = iNumpoints
            Me.m_lblNumPoints.Text = CStr(Me.m_iNumPoints)
        End Set
    End Property

    Private m_bInUpdate As Boolean = False

    Private Sub FillDataGrid()

        Me.SuspendLayout()
        Me.m_bInUpdate = True
        'Me.m_grid.Visible = False

        Select Case Me.m_editMode
            Case eDialogEditModeType.AddTimeSeries
                Me.LoadEmptyGrid()
            Case eDialogEditModeType.EditForcing
                Me.LoadForcingDataToGrid()
            Case eDialogEditModeType.EditTimeSeries
                Me.LoadTimeSeriesDataToGrid()
            Case eDialogEditModeType.AddForcing
                ' Mode not supported yet(/anymore?)
                Debug.Assert(False)
        End Select
        Me.UpdateControls()

        Me.m_bInUpdate = False
        'Me.m_grid.Visible = True
        Me.ResumeLayout()

    End Sub

    Private Sub LoadForcingDataToGrid()

        Dim iOffset As Integer = 0
        Dim bIsMediation As Boolean = (Me.m_shape.DataType = EwEUtils.Core.eDataTypes.Mediation)

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES
        m_txtName.Text = Me.m_shape.Name

        ' Hide seasonal flag for mediation functions
        Me.m_lblViewAs.Visible = Not bIsMediation
        Me.m_cmbViewAs.Visible = Not bIsMediation

        m_lblWeight.Visible = False
        m_txtWeight.Visible = False

        m_lblType.Visible = False
        m_cmbType.Visible = False

        m_lblPoolCode.Visible = False
        m_cmbPoolCode.Visible = False

        m_lblNoOfPoints.Visible = False
        m_tlpNoOfYears.Visible = False

        m_lblXBase.Visible = bIsMediation
        m_txtXBase.Visible = bIsMediation

        If bIsMediation Then
            Me.m_fpXBase.Value = DirectCast(Me.m_shape, cMediationBaseFunction).XBaseIndex
        End If

        Me.IsSeasonal = Me.m_shape.IsSeasonal

        Me.NumPoints = CInt(IIf(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.nPoints))
        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    Private Sub LoadTimeSeriesDataToGrid()

        Dim ts As cTimeSeries = DirectCast(Me.m_shape, cTimeSeries)
        'Dim l_Array As Single(,)

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES
        m_txtName.Enabled = True
        m_txtName.Text = ts.Name

        m_lblWeight.Visible = True
        m_txtWeight.Visible = True
        Me.m_fpWeight.Value = ts.WtType

        m_lblType.Visible = True
        m_cmbType.Visible = True

        m_lblXBase.Visible = False
        m_txtXBase.Visible = False

        m_lblViewAs.Visible = False
        m_cmbViewAs.Visible = False

        Me.FillTSTypeCombo(ts)

        m_lblPoolCode.Visible = True
        m_cmbPoolCode.Visible = True
        Me.FillPoolCodeComboBox()

        m_btnOK.Visible = True
        m_btnCancel.Visible = True

        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    ''' <summary>
    ''' Load an empty grid for Time Series
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
        m_txtName.Enabled = True
        m_txtName.Text = String.Format(My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES, iNextTS)

        m_lblWeight.Visible = True
        m_txtWeight.Visible = True
        m_txtWeight.Text = "1.0"

        m_lblType.Visible = True
        m_cmbType.Visible = True
        Me.FillTSTypeCombo(Nothing)
        m_cmbType.Text = m_cmbType.Items(0).ToString

        m_lblPoolCode.Visible = True
        m_cmbPoolCode.Visible = True

        m_lblXBase.Visible = False
        m_txtXBase.Visible = False

        m_lblViewAs.Visible = False
        m_cmbViewAs.Visible = False

        Me.FillPoolCodeComboBox()
        m_cmbPoolCode.Text = m_cmbPoolCode.Items(0).ToString

        Me.m_grid.Clear(Me.NumPoints, (Me.m_editMode = eDialogEditModeType.AddTimeSeries Or Me.m_editMode = eDialogEditModeType.EditTimeSeries))

    End Sub

    Private Function OnUpdateTimeSeries() As Boolean

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditTimeSeries)

        Dim ts As cTimeSeries = Nothing
        Dim iPoolCode As Integer
        Dim fts As cFleetTimeSeries = Nothing
        Dim gts As cGroupTimeSeries = Nothing
        Dim bSucces As Boolean = True

        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_TIMESERIES_UPDATING)

        'Get the time series
        ts = DirectCast(Me.m_shape, cTimeSeries)

        'Update the time series
        ts.Name = m_txtName.Text
        ' Parse value using UI number settings
        ts.WtType = CSng(Me.m_fpWeight.Value)
        ts.TimeSeriesType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = m_cmbPoolCode.SelectedIndex + 1

        'Assign the time series pool code to fleet index or group index
        Select Case cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)
            Case eTimeSeriesCategoryType.Fleet
                fts = CType(ts, cFleetTimeSeries)
                fts.FleetIndex = iPoolCode
            Case eTimeSeriesCategoryType.Group
                gts = CType(ts, cGroupTimeSeries)
                gts.GroupIndex = iPoolCode
        End Select

        ' Update the shape
        Me.m_grid.ApplyValues(ts)

        ts.Update()
        bSucces = Me.Core.UpdateTimeSeries()
        cApplicationStatusNotifier.EndProgress(Me.Core)

        Return bSucces
    End Function

    Private Function OnApplyForcing() As Boolean

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditForcing)

        Dim ff As cForcingFunction = Nothing

        'Get the time series
        ff = DirectCast(Me.m_shape, cForcingFunction)

        ' Update the forcing function
        ff.Name = Me.m_txtName.Text
        ff.IsSeasonal = Me.IsSeasonal

        If TypeOf (ff) Is cMediationBaseFunction Then
            ' Parse value using UI number settings
            DirectCast(ff, cMediationBaseFunction).XBaseIndex = CInt(Me.m_fpXBase.Value)
        End If

        ' Update the shape
        Me.m_grid.ApplyValues(ff)

        ' ToDo: apply seasonal pattern

        Return ff.Update()

    End Function

    Private Function OnAddTimeSeries() As Boolean

        Dim iFirstYear As Integer = 1
        Dim strName As String
        Dim sWeight As Single
        Dim iPoolCode As Integer
        Dim tsType As eTimeSeriesType
        Dim iDBID As Integer = -1
        Dim asValues As Single() = Nothing
        Dim bSucces As Boolean = True

        cApplicationStatusNotifier.StartProgress(Me.Core, String.Format(My.Resources.STATUS_TIMESERIES_ADDING, m_txtName.Text))

        strName = m_txtName.Text
        ' Parse value using UI number settings
        sWeight = CSng(Me.m_fpWeight.Value)
        tsType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = m_cmbPoolCode.SelectedIndex + 1
        iFirstYear = Me.m_grid.ValueStartRef
        asValues = Me.m_grid.Values(Me.m_iNumPoints)

        bSucces = Me.Core.AddTimeSeries(strName, iPoolCode, tsType, sWeight, asValues, iDBID)

        cApplicationStatusNotifier.EndProgress(Me.Core)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of crucial controls based on the content in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        Dim bIsMediation As Boolean = (Me.m_shape.DataType = EwEUtils.Core.eDataTypes.Mediation)
        Dim bEnableOk As Boolean = True
        Try
            ' Need a name to 'OK'
            bEnableOk = Not String.IsNullOrEmpty(Me.m_txtName.Text)

            If (bIsMediation) Then
                Dim sDummy As Single = 42.0!
                bEnableOk = bEnableOk And (Single.TryParse(Me.m_txtXBase.Text, sDummy) = True)
            End If

            ' Time series specific tests:
            If (Me.m_editMode = eDialogEditModeType.EditTimeSeries) Or _
               (Me.m_editMode = eDialogEditModeType.AddTimeSeries) Then
                ' TS need a valid weight factor
                ' Parse value using UI number settings
                bEnableOk = bEnableOk And (Single.Parse(Me.m_txtWeight.Text) >= 0)
                ' TS need a valid poolcode selection
                bEnableOk = bEnableOk And (Me.m_cmbPoolCode.SelectedIndex >= 0)
            End If

        Catch ex As Exception
            bEnableOk = False
            Debug.Assert(False, ex.Message)
        End Try

        Me.m_btnOK.Enabled = bEnableOk

    End Sub

    Private Sub FillTSTypeCombo(ByVal ts As cTimeSeries)

        Dim itemNew As cTSTComboBoxItem = Nothing
        Dim itemSelected As cTSTComboBoxItem = Nothing
        Dim bAdd As Boolean = True

        m_cmbType.DropDownStyle = ComboBoxStyle.DropDownList
        m_cmbType.Items.Clear()
        For Each tst As eTimeSeriesType In [Enum].GetValues(GetType(eTimeSeriesType))

            bAdd = True

            If ts IsNot Nothing Then
                ' Only allow types belonging to the same category as the current time series
                bAdd = cTimeSeriesFactory.TimeSeriesCategory(tst) = cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)
            End If

            Select Case tst
                Case eTimeSeriesType.NotSet, eTimeSeriesType.TimeForcing, eTimeSeriesType.EcotracerConcAbs, eTimeSeriesType.EcotracerConcRel
                    bAdd = False
                Case Else
                    ' Do not disallow
            End Select

            If bAdd Then
                itemNew = New cTSTComboBoxItem(tst)
                m_cmbType.Items.Add(itemNew)
                'Find selection
                If ts IsNot Nothing Then
                    If ts.TimeSeriesType = tst Then
                        itemSelected = itemNew
                    End If
                End If
            End If
        Next tst

        m_cmbType.Sorted = True
        m_cmbType.SelectedItem = itemSelected
    End Sub

    Private Sub FillPoolCodeComboBox()

        Dim fts As cFleetTimeSeries
        Dim gts As cGroupTimeSeries

        m_cmbPoolCode.DropDownStyle = ComboBoxStyle.DropDownList
        m_cmbPoolCode.Items.Clear()
        'Load pool code combo box based on the selected time series type
        Select Case cTimeSeriesFactory.TimeSeriesCategory(SelectedTimeSeriesType())
            Case eTimeSeriesCategoryType.Fleet
                m_lblPoolCode.Text = My.Resources.LABEL_FLEET
                For i As Integer = 1 To Me.Core.nFleets
                    m_cmbPoolCode.Items.Add(String.Format(My.Resources.GENERIC_LABEL_INDEXED, i, Me.Core.FleetInputs(i).Name))
                Next
                If Me.m_shape IsNot Nothing Then
                    fts = CType(Me.m_shape, cFleetTimeSeries)
                    If ((fts.FleetIndex > 0 And fts.FleetIndex <= Me.Core.nFleets)) Then
                        m_cmbPoolCode.SelectedIndex = fts.FleetIndex - 1
                    End If
                End If
            Case eTimeSeriesCategoryType.Group
                m_lblPoolCode.Text = My.Resources.LABEL_GROUP
                For i As Integer = 1 To Me.Core.nGroups
                    m_cmbPoolCode.Items.Add(String.Format(My.Resources.GENERIC_LABEL_INDEXED, i, Me.Core.EcoPathGroupInputs(i).Name))
                Next
                If (Me.m_shape IsNot Nothing) Then
                    gts = CType(Me.m_shape, cGroupTimeSeries)
                    If ((gts.GroupIndex > 0 And gts.GroupIndex <= Me.Core.nGroups)) Then
                        m_cmbPoolCode.SelectedIndex = gts.GroupIndex - 1
                    End If
                End If
            Case eTimeSeriesCategoryType.NotSet
        End Select
    End Sub

    Private Property SelectedTimeSeriesType() As eTimeSeriesType
        Get
            Dim item As cTSTComboBoxItem = DirectCast(Me.m_cmbType.SelectedItem, cTSTComboBoxItem)
            If item Is Nothing Then Return eTimeSeriesType.NotSet
            Return item.TimeSeriesType()
        End Get
        Set(ByVal t As eTimeSeriesType)
            For i As Integer = 0 To Me.m_cmbType.Items.Count - 1
                Dim item As cTSTComboBoxItem = DirectCast(Me.m_cmbType.Items(i), cTSTComboBoxItem)
                If item.TimeSeriesType = eTimeSeriesType.TimeForcing Then Me.m_cmbType.SelectedItem = item : Return
            Next
            Me.m_cmbType.SelectedItem = Nothing
        End Set
    End Property

    Private Property IsSeasonal() As Boolean
        Get
            Return Me.m_cmbViewAs.SelectedIndex = 1
        End Get
        Set(ByVal value As Boolean)
            Me.m_cmbViewAs.SelectedIndex = CInt(IIf(value, 1, 0))
        End Set
    End Property

#End Region ' Internal implementation

End Class
