#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Namespace Ecopath.Output

    Public Class PSDPlotByGroup
        Private m_core As cCore = cCore.GetInstance()

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            PopulateGroupBoxes()

        End Sub

        Private Sub PopulateGroupBoxes()
            llbGroups.SuspendLayout()

            llbGroups.Items.Clear()
            'llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(Nothing))
            For i As Integer = 1 To m_core.nLivingGroups
                llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
            Next

            llbGroups.ResumeLayout()
        End Sub

    End Class

End Namespace