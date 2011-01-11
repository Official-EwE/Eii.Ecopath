#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing the Ecosim results grids interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EcosimResults

#Region " Private variables "

        Private Enum eDisplayModeTypes As Byte
            NotSet = 0
            Groups
            Fleets
            Indices
        End Enum

        Private m_displayMode As eDisplayModeTypes

        ''' <summary>Results grid</summary>
        Private m_grid As EwEGrid = Nothing

        'format provides 
        Private m_fpStartSum As cEwEFormatProvider = Nothing
        Private m_fpEndSum As cEwEFormatProvider = Nothing
        Private m_fpNumSteps As cEwEFormatProvider = Nothing

        ''' <summary>core message handler.</summary>
        Private m_coreMessageHandler As cMessageHandler

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            'summary
            Me.m_fpStartSum = New cPropertyFormatProvider(Me.UIContext, Me.m_nudSumStart, Me.Core.EcoSimModelParameters, eVarNameFlags.EcosimSumStart)
            Me.m_fpEndSum = New cPropertyFormatProvider(Me.UIContext, Me.m_nudSumEnd, Me.Core.EcoSimModelParameters, eVarNameFlags.EcosimSumEnd)
            Me.m_fpNumSteps = New cPropertyFormatProvider(Me.UIContext, Me.udNumTimeSteps, Me.Core.EcoSimModelParameters, eVarNameFlags.EcosimSumNTimeSteps)

            Me.m_cmbFleets.Items.Clear()
            For i As Integer = 0 To Me.Core.nFleets 'includes the 'combined fleets' object
                Me.m_cmbFleets.Items.Add(Me.Core.EcosimFleetOutput(i).Name)
            Next
            Me.m_cmbFleets.SelectedIndex = 0

            Me.m_displayMode = eDisplayModeTypes.Fleets
            Me.m_rbGear.Checked = True
            Me.UpdateControls()

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            ' Clear last grid
            Me.m_grid.UIContext = Nothing

            Me.m_fpEndSum.Release()
            Me.m_fpNumSteps.Release()
            Me.m_fpStartSum.Release()

            ' Done
            MyBase.OnFormClosed(e)

        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbFleets.SelectedIndexChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_grid Is Nothing) Then Return

            If (TypeOf Me.m_grid Is EcosimResultsGridGroup) Then
                DirectCast(Me.m_grid, EcosimResultsGridGroup).SelectedFleetIndex = m_cmbFleets.SelectedIndex
            End If

        End Sub

        Private Sub rbGear_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbGear.CheckedChanged
            If m_rbGear.Checked Then
                Me.DisplayMode = eDisplayModeTypes.Fleets
            End If
        End Sub

        Private Sub rbGroup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbGroup.CheckedChanged
            If m_rbGroup.Checked Then
                Me.DisplayMode = eDisplayModeTypes.Groups
            End If
        End Sub

        Private Sub rbIndices_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbIndices.CheckedChanged
            If m_rbIndices.Checked Then
                Me.DisplayMode = eDisplayModeTypes.Indices
            End If
        End Sub

#End Region ' Events

#Region " Private stuff "

        Private Property DisplayMode() As eDisplayModeTypes
            Get
                Return Me.m_displayMode
            End Get
            Set(ByVal value As eDisplayModeTypes)

                If (Me.m_grid IsNot Nothing) Then
                    Me.m_grid.UIContext = Nothing
                    Me.m_plResultsGrid.Controls.Remove(Me.m_grid)
                    Me.m_grid = Nothing
                End If

                Me.m_displayMode = value

                Select Case Me.m_displayMode
                    Case eDisplayModeTypes.Groups
                        Me.m_grid = New EcosimResultsGridGroup()
                        Me.m_grid.UIContext = Me.UIContext
                        DirectCast(Me.m_grid, EcosimResultsGridGroup).SelectedFleetIndex = Me.m_cmbFleets.SelectedIndex
                    Case eDisplayModeTypes.Fleets
                        Me.m_grid = New EcosimResultsGridFleet()
                        Me.m_grid.UIContext = Me.UIContext
                    Case eDisplayModeTypes.Indices
                        Me.m_grid = New EcosimResultsGridIndices()
                        Me.m_grid.UIContext = Me.UIContext
                End Select

                If (Me.m_grid IsNot Nothing) Then
                    Me.m_grid.Dock = DockStyle.Fill
                    Me.m_plResultsGrid.Controls.Add(Me.m_grid)
                End If

                Me.UpdateControls()

            End Set
        End Property

        ''' <summary>
        ''' Message handler for core Ecosim Datachanged message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>This updates the grids with the results if the user changed the time periods</remarks>
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If msg.DataType = eDataTypes.EcoSimModelParameter Then
                For Each var As cVariableStatus In msg.Variables
                    If var.VarName = eVarNameFlags.EcosimSumEnd Or var.VarName = eVarNameFlags.EcosimSumStart Or var.VarName = eVarNameFlags.EcosimSumNTimeSteps Then
                        Me.m_grid.RefreshContent()
                        Exit Sub
                    End If
                Next
            End If
            MyBase.OnCoreMessage(msg)
        End Sub

        Private Sub UpdateControls()

            Dim bNeedsYearRange As Boolean = (Me.DisplayMode <> eDisplayModeTypes.Indices)

            Me.m_cmbFleets.Enabled = (Me.DisplayMode = eDisplayModeTypes.Groups)

            Me.m_lblBegin.Enabled = bNeedsYearRange
            Me.m_fpStartSum.Enabled = bNeedsYearRange
            Me.m_lblEnd.Enabled = bNeedsYearRange
            Me.m_fpEndSum.Enabled = bNeedsYearRange
            Me.m_lblNumTimeSteps.Enabled = bNeedsYearRange
            Me.m_fpNumSteps.Enabled = bNeedsYearRange

        End Sub

#End Region ' Private stuff

    End Class

End Namespace
