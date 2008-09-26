'==============================================================================
'
' $Log: gridMPAOptimizations.vb,v $
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/08/26 18:51:42  jeroens
' Removed Mean column
'
' Revision 1.9  2008/08/18 15:50:07  jeroens
' Mean-aware
'
' Revision 1.8  2008/08/15 21:05:01  jeroens
' Grid interface more pronounced
' Added more states to better reflect what is happening
'
' Revision 1.7  2008/08/15 12:43:04  jeroens
' Ecoseed progress logged correctly
'
' Revision 1.6  2008/08/14 21:30:08  jeroens
' Enabled setting values
'
' Revision 1.5  2008/08/14 17:51:09  jeroens
' Undock! UNDOCK, thou fiend!
'
' Revision 1.4  2008/08/14 15:39:41  jeroens
' Working
'
' Revision 1.3  2008/08/02 03:04:19  jeroens
' Renamed resources
'
' Revision 1.2  2008/07/29 13:06:46  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.1  2008/06/04 15:32:43  jeroens
' Renamed
' Implemented series of changes in response to Email VC 03Jun08
'
' Revision 1.2  2008/06/02 00:01:32  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2008/04/07 03:38:16  jeroens
' Gettting there, slowly...
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports directive

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class gridMPAOptimizations
    : Inherits EwEGrid

    Public Enum eColumnTypes As Byte
        Variable
        Value
    End Enum

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim c As EwECell = Nothing

        Me.FixedColumnWidths = False

        Me.Redim(7, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Variable) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Value) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

        Me(1, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_NETECONOMICVALUE)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(1, eColumnTypes.Value) = c

        Me(2, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_SOCIAL_VALUE)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(2, eColumnTypes.Value) = c

        Me(3, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_MANDATED_REBUILDING)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(3, eColumnTypes.Value) = c

        Me(4, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_ECOSYSTEM_STRUCTURE)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(4, eColumnTypes.Value) = c

        Me(5, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_TOTAL)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(5, eColumnTypes.Value) = c

        Me(6, eColumnTypes.Variable) = New EwERowHeaderCell("% Area closed")
        c = New EwECell(0.0!, GetType(Single))
        c.Style = StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable
        Me(6, eColumnTypes.Value) = c

    End Sub

    Protected Overrides Sub FillData()

    End Sub

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

    Public Sub LogResult(ByVal sEconomicValue As Single, ByVal sSocialValue As Single, _
        ByVal sMandatedValue As Single, ByVal sEcologicalValue As Single, _
        ByVal sTotalWeighted As Single, ByVal sPercClosed As Single)

        Me(1, eColumnTypes.Value).Value = sEconomicValue
        Me(2, eColumnTypes.Value).Value = sSocialValue
        Me(3, eColumnTypes.Value).Value = sMandatedValue
        Me(4, eColumnTypes.Value).Value = sEcologicalValue
        Me(5, eColumnTypes.Value).Value = sTotalWeighted
        Me(6, eColumnTypes.Value).Value = sPercClosed

        Me.InvalidateCells()

    End Sub

End Class
