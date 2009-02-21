' =============================================================================
'
' $Log: SizeWeightPlot.vb,v $
' Revision 1.2  2009/02/21 00:23:07  jeroens
' Added headers
'
' =============================================================================

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

    Public Class SizeWeightPlot

        Private m_zgh As ZedGraphHelper = Nothing

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_zgh = New ZedGraphHelper(Me.zgcZedGraphCntl)

        End Sub

    End Class

End Namespace