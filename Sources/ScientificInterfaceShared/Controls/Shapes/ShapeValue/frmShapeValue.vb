#Region " Imports "

Option Strict On

Imports EwECore
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

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
        ''' <summary>String representation of <see cref="m_timeSeriesType">m_timeSeriesType</see>.</summary>
        Private m_strText As String = ""

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
            ' Store item text
            Me.m_strText = GetTimeSeriesStringResource(tst)
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
            Return Me.m_strText
        End Function

#Region " Clever bits "

        ''' <summary>
        ''' Return a localized string represtation for a given time series type enumerated value.
        ''' </summary>
        ''' <param name="tst"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetTimeSeriesStringResource(ByVal tst As eTimeSeriesType) As String

            ' Build name to find enum string representation in resources with
            ' 1. Get enum text
            Dim strText As String = [Enum].GetName(GetType(eTimeSeriesType), tst)
            ' 2. Apply enum text in string to resource name mask
            Dim strResourceName As String = String.Format("TS_{0}_NAME", strText.ToUpper())
            Try
                ' Cache string resource (this is processor intensive stuff - let's do it only once per instance of this class :P)
                Dim strTmp As String = My.Resources.ResourceManager.GetString(strResourceName)
                If Not String.IsNullOrEmpty(strTmp) Then strText = strTmp
            Catch e As Exception
                ' Resource string not found
                Debug.Assert(False, String.Format("Time series type {0}({1}) has no associated string resource {2}", _
                    CInt(tst), strText, strResourceName))
            End Try
            Return strText
        End Function

#End Region ' Clever bits

    End Class

#End Region ' Private helper classes

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_shape As cShapeData = Nothing
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
        Me.m_uic = uic
        Me.m_grid.UIContext = uic

        ' Store shape
        Me.m_shape = shape

        ' Determine interface mode
        If (shape Is Nothing) Then
            Me.m_editMode = eDialogEditModeType.AddTimeSeries
        Else
            Me.m_editMode = DirectCast(IIf(TypeOf shape Is cTimeSeries, eDialogEditModeType.EditTimeSeries, eDialogEditModeType.EditForcing), eDialogEditModeType)
        End If

        ' Determine display mode
        If TypeOf (shape) Is cMediationFunction Then
            Me.m_displayMode = frmShapeValue.eDisplayMode.Index
        ElseIf TypeOf (shape) Is cTimeSeries Then
            Me.m_displayMode = frmShapeValue.eDisplayMode.Yearly
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

        ' Kick off
        If Me.m_shape Is Nothing Then
            Me.NumPoints = cNUMROWS_EMTPY
        Else
            If Me.m_shape.IsSeasonal Then
                Me.NumPoints = cCore.N_MONTHS
            Else
                Me.NumPoints = Me.m_shape.XMax
            End If
        End If
        Me.FillDataGrid()
        Me.UpdateControls()

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnOK.Click

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

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnCancel.Click

        ' Done
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub cmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbType.SelectedIndexChanged

        Me.FillPoolCodeComboBox()
        Me.UpdateControls()

    End Sub

    Private Sub AnyTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles txtWeight.TextChanged, m_lblNumYears.TextChanged, txtName.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub cmbPoolCode_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbPoolCode.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

    Private Sub cmbViewAs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbViewAs.SelectedIndexChanged
        Me.NumPoints = CInt(IIf(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.XMax))
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
            Me.m_lblNumYears.Text = CStr(Me.m_iNumPoints)
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
        Dim bIsMediation As Boolean = (TypeOf (Me.m_shape) Is cMediationFunction)

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES
        txtName.Text = Me.m_shape.Name

        ' Hide seasonal flag for mediation functions
        Me.lblViewAs.Visible = Not bIsMediation
        Me.cmbViewAs.Visible = Not bIsMediation

        lblWeight.Visible = False
        txtWeight.Visible = False

        lblType.Visible = False
        cmbType.Visible = False

        lblPoolCode.Visible = False
        cmbPoolCode.Visible = False

        lbNoOfYears.Visible = False
        m_lblNumYears.Visible = False
        btnSetNoOfYears.Visible = False

        lblXBase.Visible = bIsMediation
        txtXBase.Visible = bIsMediation

        If bIsMediation Then
            Me.txtXBase.Text = CStr(DirectCast(Me.m_shape, cMediationFunction).XBaseIndex)
        End If

        Me.IsSeasonal = Me.m_shape.IsSeasonal

        Me.NumPoints = CInt(IIf(Me.IsSeasonal, cCore.N_MONTHS, Me.m_shape.XMax))
        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    Private Sub LoadTimeSeriesDataToGrid()

        Dim ts As cTimeSeries = DirectCast(Me.m_shape, cTimeSeries)
        'Dim l_Array As Single(,)

        'Set the plot title
        Me.Text = My.Resources.HEADER_VALUES
        txtName.Enabled = True
        txtName.Text = ts.Name

        lblWeight.Visible = True
        txtWeight.Visible = True
        txtWeight.Text = CStr(ts.WtType)

        lblType.Visible = True
        cmbType.Visible = True

        lblXBase.Visible = False
        txtXBase.Visible = False

        lblViewAs.Visible = False
        cmbViewAs.Visible = False

        Me.FillTSTypeCombo(ts)

        lblPoolCode.Visible = True
        cmbPoolCode.Visible = True
        Me.FillPoolCodeComboBox()

        btnOK.Visible = True
        btnCancel.Visible = True

        Me.m_grid.SetValues(Me.m_shape, Me.NumPoints, Me.m_displayMode)

    End Sub

    ''' <summary>
    ''' Load an empty grid for Time Series
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadEmptyGrid()

        Dim lstrTSNames As New List(Of String)
        Dim iNextTS As Integer = -1

        ' Get next TS sequential number
        For i As Integer = 1 To Me.m_uic.Core.nTimeSeries
            lstrTSNames.Add(Me.m_uic.Core.EcosimTimeSeries(i).Name)
        Next
        iNextTS = EwEUtils.Utilities.cStringUtils.GetNextNumber(lstrTSNames.ToArray(), My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES)

        'Set the plot title
        Me.Text = My.Resources.HEADER_ADD
        txtName.Enabled = True
        txtName.Text = String.Format(My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES, iNextTS)

        lblWeight.Visible = True
        txtWeight.Visible = True
        txtWeight.Text = "1.0"

        lblType.Visible = True
        cmbType.Visible = True
        Me.FillTSTypeCombo(Nothing)
        cmbType.Text = cmbType.Items(0).ToString

        lblPoolCode.Visible = True
        cmbPoolCode.Visible = True

        lblXBase.Visible = False
        txtXBase.Visible = False

        lblViewAs.Visible = False
        cmbViewAs.Visible = False

        Me.FillPoolCodeComboBox()
        cmbPoolCode.Text = cmbPoolCode.Items(0).ToString

        Me.m_grid.Clear(Me.NumPoints, (Me.m_editMode = eDialogEditModeType.AddTimeSeries Or Me.m_editMode = eDialogEditModeType.EditTimeSeries))

    End Sub

    Private Function OnUpdateTimeSeries() As Boolean

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditTimeSeries)

        Dim ts As cTimeSeries = Nothing
        Dim iPoolCode As Integer
        Dim fts As cFleetTimeSeries = Nothing
        Dim gts As cGroupTimeSeries = Nothing
        Dim bSucces As Boolean = True

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_TIMESERIES_UPDATING, TriState.True)

        'Get the time series
        ts = DirectCast(Me.m_shape, cTimeSeries)

        'Update the time series
        ts.Name = txtName.Text
        ' Parse value using UI number settings
        ts.WtType = Single.Parse(txtWeight.Text)
        ts.TimeSeriesType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = cmbPoolCode.SelectedIndex + 1

        'Assign the time series pool code to fleet index or group index
        Select Case cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                fts = CType(ts, cFleetTimeSeries)
                fts.FleetIndex = iPoolCode
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                gts = CType(ts, cGroupTimeSeries)
                gts.GroupIndex = iPoolCode
        End Select

        ' Update the shape
        Me.m_grid.ApplyValues(ts)

        ts.Update()
        bSucces = Me.m_uic.Core.UpdateTimeSeries()
        cApplicationStatusNotifier.SetStatusText("", TriState.False)

        Return bSucces
    End Function

    Private Function OnApplyForcing() As Boolean

        Debug.Assert(Me.m_editMode = eDialogEditModeType.EditForcing)

        Dim ff As cForcingFunction = Nothing

        'Get the time series
        ff = DirectCast(Me.m_shape, cForcingFunction)

        ' Update the forcing function
        ff.Name = Me.txtName.Text
        ff.IsSeasonal = Me.IsSeasonal

        If TypeOf (ff) Is cMediationFunction Then
            ' Parse value using UI number settings
            DirectCast(ff, cMediationFunction).XBaseIndex = Integer.Parse(Me.txtXBase.Text)
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

        cApplicationStatusNotifier.SetStatusText(String.Format(My.Resources.STATUS_TIMESERIES_ADDING, txtName.Text), TriState.True)

        strName = txtName.Text
        ' Parse value using UI number settings
        sWeight = Single.Parse(txtWeight.Text)
        tsType = Me.SelectedTimeSeriesType()

        ' Set the pool code
        iPoolCode = cmbPoolCode.SelectedIndex + 1
        iFirstYear = Me.m_grid.ValueStartRef
        asValues = Me.m_grid.Values(Me.m_iNumPoints)

        bSucces = Me.m_uic.Core.AddTimeSeries(strName, iPoolCode, tsType, sWeight, asValues, iDBID)

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of crucial controls based on the content in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        Dim bEnableOk As Boolean = True
        Try
            ' Need a name to 'OK'
            bEnableOk = Not String.IsNullOrEmpty(Me.txtName.Text)

            If (TypeOf (Me.m_shape) Is cMediationFunction) Then
                Dim sDummy As Single = 42.0!
                bEnableOk = bEnableOk And (Single.TryParse(Me.txtXBase.Text, sDummy) = True)
            End If

            ' Time series specific tests:
            If (Me.m_editMode = eDialogEditModeType.EditTimeSeries) Or _
               (Me.m_editMode = eDialogEditModeType.AddTimeSeries) Then
                ' TS need a valid weight factor
                ' Parse value using UI number settings
                bEnableOk = bEnableOk And (Single.Parse(Me.txtWeight.Text) >= 0)
                ' TS need a valid poolcode selection
                bEnableOk = bEnableOk And (Me.cmbPoolCode.SelectedIndex >= 0)
            End If

        Catch ex As Exception
            bEnableOk = False
        End Try

        Me.btnOK.Enabled = bEnableOk

    End Sub

    Private Sub FillTSTypeCombo(ByVal ts As cTimeSeries)

        Dim itemNew As cTSTComboBoxItem = Nothing
        Dim itemSelected As cTSTComboBoxItem = Nothing
        Dim bAdd As Boolean = True

        cmbType.DropDownStyle = ComboBoxStyle.DropDownList
        cmbType.Items.Clear()
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
                cmbType.Items.Add(itemNew)
                'Find selection
                If ts IsNot Nothing Then
                    If ts.TimeSeriesType = tst Then
                        itemSelected = itemNew
                    End If
                End If
            End If
        Next tst

        cmbType.Sorted = True
        cmbType.SelectedItem = itemSelected
    End Sub

    Private Sub FillPoolCodeComboBox()

        Dim fts As cFleetTimeSeries
        Dim gts As cGroupTimeSeries

        cmbPoolCode.DropDownStyle = ComboBoxStyle.DropDownList
        cmbPoolCode.Items.Clear()
        'Load pool code combo box based on the selected time series type
        Select Case cTimeSeriesFactory.TimeSeriesCategory(SelectedTimeSeriesType())
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                lblPoolCode.Text = My.Resources.LABEL_FLEET
                For i As Integer = 1 To Me.m_uic.Core.nFleets
                    cmbPoolCode.Items.Add(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, i, Me.m_uic.Core.FleetInputs(i).Name))
                Next
                If Me.m_shape IsNot Nothing Then
                    fts = CType(Me.m_shape, cFleetTimeSeries)
                    If ((fts.FleetIndex > 0 And fts.FleetIndex <= Me.m_uic.Core.nFleets)) Then
                        cmbPoolCode.SelectedIndex = fts.FleetIndex - 1
                    End If
                End If
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                lblPoolCode.Text = My.Resources.LABEL_GROUP
                For i As Integer = 1 To Me.m_uic.Core.nGroups
                    cmbPoolCode.Items.Add(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, i, Me.m_uic.Core.EcoPathGroupInputs(i).Name))
                Next
                If (Me.m_shape IsNot Nothing) Then
                    gts = CType(Me.m_shape, cGroupTimeSeries)
                    If ((gts.GroupIndex > 0 And gts.GroupIndex <= Me.m_uic.Core.nGroups)) Then
                        cmbPoolCode.SelectedIndex = gts.GroupIndex - 1
                    End If
                End If
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
        End Select
    End Sub

    Private Property SelectedTimeSeriesType() As eTimeSeriesType
        Get
            Dim item As cTSTComboBoxItem = DirectCast(Me.cmbType.SelectedItem, cTSTComboBoxItem)
            If item Is Nothing Then Return eTimeSeriesType.NotSet
            Return item.TimeSeriesType()
        End Get
        Set(ByVal t As eTimeSeriesType)
            For i As Integer = 0 To Me.cmbType.Items.Count - 1
                Dim item As cTSTComboBoxItem = DirectCast(Me.cmbType.Items(i), cTSTComboBoxItem)
                If item.TimeSeriesType = eTimeSeriesType.TimeForcing Then Me.cmbType.SelectedItem = item : Return
            Next
            Me.cmbType.SelectedItem = Nothing
        End Set
    End Property

    Private Property IsSeasonal() As Boolean
        Get
            Return Me.cmbViewAs.SelectedIndex = 1
        End Get
        Set(ByVal value As Boolean)
            Me.cmbViewAs.SelectedIndex = CInt(IIf(value, 1, 0))
        End Set
    End Property

#End Region 'Internal implementation

End Class
