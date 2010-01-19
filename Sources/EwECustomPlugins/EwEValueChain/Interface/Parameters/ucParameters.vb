#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plug-in credits/parameters/info form
''' </summary>
''' ===========================================================================
Public Class ucParameters
    Implements IDisposable

    ''' <summary>The core currently linked to.</summary>
    Private m_core As cCore = Nothing
    ''' <summary>The value chain data that htis page operates on.</summary>
    Private m_data As cData = Nothing
    ''' <summary>Smartibits</summary>
    Private m_fpBaseYear As cEwEFormatProvider = Nothing
    Private m_fpFMin As cEwEFormatProvider = Nothing
    Private m_fpFMax As cEwEFormatProvider = Nothing
    Private m_fpIncr As cEwEFormatProvider = Nothing

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="data">The data to paramterize.</param>
    ''' <param name="core">Core providing EwE data.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal data As cData, ByVal core As cCore)

        Me.InitializeComponent()

        Me.m_data = data
        Me.m_core = core

    End Sub

#End Region ' Constructor

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load me!
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Init check boxes
        Try
            Me.m_chkRunWithEcopath.Checked = Me.m_data.Parameters.RunWithEcopath
            Me.m_chkRunWithEcosim.Checked = Me.m_data.Parameters.RunWithEcosim
            Me.m_chkRunWithFishingPolicySearch.Checked = Me.m_data.Parameters.RunWithFishingPolicySearch
            Me.m_chkResultsByFleet.Checked = Me.m_data.Parameters.ResultsByFleet

            Me.m_fpFMin = New cEwEFormatProvider(Me.m_nudEffortMin, GetType(Single))
            Me.m_fpFMin.Value = Me.m_data.Parameters.EquilibriumEffortMin
            Me.m_fpFMax = New cEwEFormatProvider(Me.m_nudEffortMax, GetType(Single))
            Me.m_fpFMax.Value = Me.m_data.Parameters.EquilibriumEffortMax
            Me.m_fpIncr = New cEwEFormatProvider(Me.m_nudEffortIncr, GetType(Single))
            Me.m_fpIncr.Value = Me.m_data.Parameters.EquilibriumEffortIncrement

            For iFleet As Integer = 1 To Me.m_core.nFleets
                Me.m_clbFleets.Items.Add(Me.m_core.FleetInputs(iFleet).Name)
            Next iFleet

        Catch ex As Exception

        End Try

        ' Start listening to core state changes
        AddHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged
        ' Force core state dependent initialization
        Me.OnCoreStateChanged(Me.m_core.StateMonitor)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Unload me!
    ''' </summary>
    ''' <param name="bDisposing"></param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)

        ' Stop listening to core state changes
        RemoveHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged
        ' Unplug Ecosim controls
        Me.ConfigureEcosimControls(False)

        ' Default unloading
        Try
            If Disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(Disposing)
        End Try

    End Sub

#End Region ' Overrides

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; responds to core execution state changes.
    ''' </summary>
    ''' <param name="csm">Core state monitor that changes.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnCoreStateChanged(ByVal csm As cCoreStateMonitor)
        Me.ConfigureEcosimControls(csm.HasEcosimLoaded)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnRunWithEcopathCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_chkRunWithEcopath.CheckedChanged
        Me.m_data.Parameters.RunWithEcopath = Me.m_chkRunWithEcopath.Checked
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnRunWithEcosimCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
          Handles m_chkRunWithEcosim.CheckedChanged
        Me.m_data.Parameters.RunWithEcosim = Me.m_chkRunWithEcosim.Checked
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnRunWithFishingPolicySearchCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
         Handles m_chkRunWithFishingPolicySearch.CheckedChanged
        Me.m_data.Parameters.RunWithFishingPolicySearch = Me.m_chkRunWithFishingPolicySearch.Checked
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnResultsByFleetChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_chkResultsByFleet.CheckedChanged
        Me.m_data.Parameters.ResultsByFleet = Me.m_chkResultsByFleet.Checked
    End Sub

    Private Sub m_nudEffortMin_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_nudEffortMin.ValueChanged
        If (Me.m_data Is Nothing) Then Return
        Me.m_data.Parameters.EquilibriumEffortMin = CSng(Me.m_nudEffortMin.Value)
    End Sub

    Private Sub m_nudEffortMax_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_nudEffortMax.ValueChanged
        If (Me.m_data Is Nothing) Then Return
        Me.m_data.Parameters.EquilibriumEffortMax = CSng(Me.m_nudEffortMax.Value)
    End Sub

    Private Sub m_nudEffortIncr_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_nudEffortIncr.ValueChanged
        If (Me.m_data Is Nothing) Then Return
        Me.m_data.Parameters.EquilibriumEffortIncrement = CSng(Me.m_nudEffortIncr.Value)
    End Sub

    Private Sub m_clbFleets_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_clbFleets.SelectedIndexChanged
        Me.m_data.Parameters.EquilibriumFleetsToVary.Clear()
        For Each iFleet As Integer In Me.m_clbFleets.CheckedIndices
            Me.m_data.Parameters.EquilibriumFleetsToVary.Add(iFleet + 1)
        Next
    End Sub

#End Region ' Events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Configure Ecosim-dependent parameter controls.
    ''' </summary>
    ''' <param name="bConnect">
    ''' True to connect to Ecosim, False to disconnect.
    ''' </param>
    ''' -----------------------------------------------------------------------
    Private Sub ConfigureEcosimControls(ByVal bConnect As Boolean)

        If (bConnect) Then

            If (Me.m_fpBaseYear Is Nothing) Then
                ' Create Ecosim dependent format provider(s)
                Me.m_fpBaseYear = New cPropertyFormatProvider(Me.m_nudBaseYear, _
                                            Me.m_core.SearchObjective.ObjectiveParameters, eVarNameFlags.SearchBaseYear)
            End If

        Else

            If (Me.m_fpBaseYear IsNot Nothing) Then
                ' Release Ecosim dependent format provider(s)
                Me.m_fpBaseYear.Release()
                Me.m_fpBaseYear = Nothing
            End If

        End If

        ' Enable/disable Ecosim dependent controls
        If (Not Me.m_nudBaseYear.IsDisposed) Then
            Me.m_lblBaseYear.Enabled = bConnect
            Me.m_nudBaseYear.Enabled = bConnect
        End If

    End Sub

#End Region ' Internals

End Class
