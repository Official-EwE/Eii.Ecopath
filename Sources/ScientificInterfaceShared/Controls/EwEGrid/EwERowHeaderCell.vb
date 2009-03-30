'==============================================================================
'
' $Log: EwERowHeaderCell.vb,v $
' Revision 1.1  2009/03/30 16:59:25  jeroens
' Split
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a EwERowHeaderCell to implement row headers. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EwERowHeaderCell
        : Inherits EwEHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwERowHeader()

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue)
            ' Set visualizer
            Me.VisualModel = g_visualizer
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New()
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New()
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

End Namespace
