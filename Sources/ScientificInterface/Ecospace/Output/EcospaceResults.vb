#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    Public Class cFormEcospaceResults

#Region " Private vars "

        ' Results grid
        Private m_GridGear As cGridEcospaceResultsGear = Nothing
        Private m_GridGroup As cGridEcospaceResultsGroup = Nothing
        Private m_GridRegion As cGridEcospaceResultsRegion = Nothing

        ' Summary
        Private m_fpSumStartTime As cEwEFormatProvider = Nothing
        Private m_fpSumEndTime As cEwEFormatProvider = Nothing
        Private m_fpSumLength As cEwEFormatProvider = Nothing

#End Region ' Private vars

        Public Sub New()

            Me.InitializeComponent()

        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim ecospaceModelParams As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()
            Dim pm As cPropertyManager = Me.PropertyManager

            Me.m_fpSumStartTime = New cPropertyFormatProvider(pm, Me.tbSumStartTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeStart)
            Me.m_fpSumEndTime = New cPropertyFormatProvider(pm, Me.tbSumEndTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeEnd)
            Me.m_fpSumLength = New cPropertyFormatProvider(pm, Me.udSumLength, ecospaceModelParams, eVarNameFlags.EcospaceNumberSummaryTimeSteps)

            'Initialize the results grid
            m_GridGear = New cGridEcospaceResultsGear
            m_GridGroup = New cGridEcospaceResultsGroup
            m_GridRegion = New cGridEcospaceResultsRegion

            ' Add the result grids. 
            plResultsGrid.Controls.Add(m_GridGear)
            plResultsGrid.Controls.Add(m_GridGroup)
            plResultsGrid.Controls.Add(m_GridRegion)

            m_GridGear.UIContext = Me.UIContext
            m_GridGroup.UIContext = Me.UIContext
            m_GridRegion.UIContext = Me.UIContext

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

            Me.PopulateResults()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Me.CoreComponents = Nothing
            Me.m_fpSumStartTime.Release()
            Me.m_fpSumEndTime.Release()
            Me.m_fpSumLength.Release()
            MyBase.OnFormClosed(e)
        End Sub

        ''' <summary> Repopulates the variables on demand. </summary>
        Private Sub PopulateResults()
            rbGear.Checked = True

            cbGears.Items.Clear()

            Dim efo As cEcospaceFleetOutput = Nothing
            For i As Integer = 0 To Me.Core.nFleets
                efo = Me.Core.EcospaceFleetOutput(i)
                cbGears.Items.Add(efo.Name)
            Next
            cbGears.SelectedIndex = 0

            cbRegions.Items.Clear()
            Dim ero As cEcospaceRegionOutput = Nothing
            For i As Integer = 0 To Me.Core.nRegions
                ero = Me.Core.EcospaceRegionOutput(i)
                cbRegions.Items.Add(ero.Name)
            Next
            cbRegions.SelectedIndex = 0

        End Sub

        Private Sub rbResults_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGear.CheckedChanged, rbGroup.CheckedChanged, rbRegion.CheckedChanged

            If rbGear.Checked Then
                'Display gear results
                m_GridGear.Visible = True : m_GridRegion.Visible = False : m_GridGroup.Visible = False
                Me.cbGears.Enabled = False
                Me.cbRegions.Enabled = False

            ElseIf rbGroup.Checked Then
                'Display group results
                m_GridGear.Visible = False : m_GridRegion.Visible = False : m_GridGroup.Visible = True
                Me.cbGears.Enabled = True
                Me.cbRegions.Enabled = False

            ElseIf rbRegion.Checked Then
                'Display region results
                m_GridGear.Visible = False : m_GridRegion.Visible = True : m_GridGroup.Visible = False
                Me.cbGears.Enabled = False
                Me.cbRegions.Enabled = True
            End If

        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbGears.SelectedIndexChanged
            'fleets are zero based so the zero index is ok
            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex
            m_GridGroup.RefreshContent()

        End Sub

        Private Sub cbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegions.SelectedIndexChanged

            'regions are zero based so the zero index is ok
            m_GridRegion.SelRegionIndex = cbRegions.SelectedIndex
            m_GridRegion.RefreshContent()

        End Sub

        'Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '    Me.DialogResult = System.Windows.Forms.DialogResult.OK
        '    Me.Close()
        'End Sub

        ''' <summary>
        ''' Message handler for core Ecosim Datachanged message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>This updates the grids with the results if the user changed the time periods</remarks>
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If msg.DataType = eDataTypes.EcospaceModelParameter Then
                For Each var As cVariableStatus In msg.Variables
                    If var.VarName = eVarNameFlags.EcospaceSummaryTimeStart Or var.VarName = eVarNameFlags.EcospaceSummaryTimeEnd Or var.VarName = eVarNameFlags.EcospaceNumberSummaryTimeSteps Then

                        If m_GridGroup.Visible Then m_GridGroup.RefreshContent()
                        If m_GridRegion.Visible Then m_GridRegion.RefreshContent()
                        If m_GridGear.Visible Then m_GridGear.RefreshContent()

                        Exit Sub
                    End If
                Next
            End If
            MyBase.OnCoreMessage(msg)
        End Sub

    End Class

End Namespace

