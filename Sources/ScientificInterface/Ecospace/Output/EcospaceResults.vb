'==============================================================================
'
' $Log: EcospaceResults.vb,v $
' Revision 1.2  2008/11/20 15:19:24  jeroens
' Renamed classes
'
' Revision 1.1  2008/09/26 07:32:01  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/06/25 00:26:22  sherman
' Moved Ecospace Results into Node... still has refresh issues
'
'==============================================================================

#Region "Imports directive"

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecospace

    Public Class cFormEcospaceResults

        ' The core reference
        Private m_Core As cCore = Nothing

        ' Results grid
        Private m_GridGear As cGridEcospaceResultsGear = Nothing
        Private m_GridGroup As cGridEcospaceResultsGroup = Nothing
        Private m_GridRegion As cGridEcospaceResultsRegion = Nothing

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ' Get the only core reference
            m_Core = cCore.GetInstance()

            'Initialize the results grid
            m_GridGear = New cGridEcospaceResultsGear
            m_GridGroup = New cGridEcospaceResultsGroup
            m_GridRegion = New cGridEcospaceResultsRegion

            ' Add the result grids. 
            plResultsGrid.Controls.Add(m_GridGear)
            plResultsGrid.Controls.Add(m_GridGroup)
            plResultsGrid.Controls.Add(m_GridRegion)

        End Sub

        Private Sub EcospaceResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            PopulateResults()
            Me.MessageSources = New eMessageSource() {eMessageSource.EcoSpace}
        End Sub

        Private Sub frmEcospaceResults_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            Me.MessageSources = Nothing
        End Sub

        ''' <summary> Repopulates the variables on demand. </summary>
        Private Sub PopulateResults()
            rbGear.Checked = True

            txbBegin.Text = CStr(m_Core.EcospaceModelParameters.StartSummaryTime)
            txbEnd.Text = CStr(m_Core.EcospaceModelParameters.EndSummaryTime)

            cbGears.Items.Clear()

            Dim efo As cEcospaceFleetSummary = Nothing
            For i As Integer = 0 To m_Core.nFleets
                efo = m_Core.EcospaceFleetSummary(i)
                cbGears.Items.Add(efo.Name)
            Next
            cbGears.SelectedIndex = 0

            cbRegions.Items.Clear()
            Dim ero As cEcospaceRegionSummary = Nothing
            For i As Integer = 0 To m_Core.nRegions
                ero = m_Core.EcospaceRegionSummary(i)
                cbRegions.Items.Add(ero.Name)
            Next
            cbRegions.SelectedIndex = 0
        End Sub

        Private Sub rbResults_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGear.CheckedChanged, rbRegion.CheckedChanged, rbGroup.CheckedChanged

            If rbGear.Checked Then
                'Display gear results
                m_GridGear.Visible = True : m_GridRegion.Visible = False : m_GridGroup.Visible = False

            ElseIf rbGroup.Checked Then
                'Display group results
                m_GridGear.Visible = False : m_GridRegion.Visible = False : m_GridGroup.Visible = True

            ElseIf rbRegion.Checked Then
                'Display region results
                m_GridGear.Visible = False : m_GridRegion.Visible = True : m_GridGroup.Visible = False

            End If

        End Sub

        Private Sub cbGears_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbGears.SelectedIndexChanged

            m_GridGroup.SelFleetIndex = cbGears.SelectedIndex ' + 1

        End Sub

        Private Sub cbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegions.SelectedIndexChanged

            m_GridRegion.SelRegionIndex = cbRegions.SelectedIndex ' + 1

        End Sub

        Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub
    End Class

End Namespace

