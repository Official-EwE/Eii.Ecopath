'==============================================================================
'
' $Log: SuitabilityPlot.vb,v $
' Revision 1.1  2008/12/09 00:30:01  joeh
' Add node for the three Suitability curves (Electivity, Functional response and Suitability)
'
'
Namespace Ecosim

    Public Class SuitabilityPlot

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Dim SuitabilityPlot As New ucSuitabilityPlot() 'm_NetworkManager)

            SuitabilityPlot.Dock = DockStyle.Fill
            Me.Controls.Add(SuitabilityPlot)
        End Sub

    End Class

End Namespace

