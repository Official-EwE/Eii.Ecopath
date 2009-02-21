' =============================================================================
'
' $Log: PSDContributionPlot.vb,v $
' Revision 1.2  2009/02/21 00:23:06  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecopath.Output

    Public Class PSDContributionPlot

        Private m_zgh As ZedGraphHelper = Nothing

        Public Sub New()

            InitializeComponent()

        End Sub

    End Class

End Namespace