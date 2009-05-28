'==============================================================================
'
' $Log: gridMPAOptimizations.vb,v $
' Revision 1.6  2009/05/28 12:37:37  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.5  2008/12/15 15:55:36  jeroens
' no message
'
' Revision 1.4  2008/11/19 14:35:29  jeroens
' Resources!
'
' Revision 1.3  2008/11/14 00:29:11  jeroens
' Added more indicators
'
' Revision 1.2  2008/11/12 21:39:05  jeroens
' Revamping
'
' Revision 1.1  2008/09/26 07:32:03  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

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

        Me.Redim(9, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Variable) = New EwEColumnHeaderCell(My.Resources.HEADER_INDICATOR)
        Me(0, eColumnTypes.Value) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

        Me(1, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_NET_ECONOMIC_VALUE)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(1, eColumnTypes.Value) = c

        Me(2, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_SOCIAL_VALUE_EMPLOYMENT)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(2, eColumnTypes.Value) = c

        Me(3, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_MANDATED_REBUILDING)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(3, eColumnTypes.Value) = c

        Me(4, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_ECOSYSTEM_STRUCTURE)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(4, eColumnTypes.Value) = c

        Me(5, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_BIOMASS_DIVERSITY)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(5, eColumnTypes.Value) = c

        Me(6, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_BOUNDARYWEIGHT)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(6, eColumnTypes.Value) = c

        Me(7, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_TOTAL)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(7, eColumnTypes.Value) = c

        ' ToDo: globalize this
        Me(8, eColumnTypes.Variable) = New EwERowHeaderCell(My.Resources.HEADER_AREA_CLOSED)
        c = New EwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(8, eColumnTypes.Value) = c

    End Sub

    Protected Overrides Sub FillData()

    End Sub

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

    Public Sub LogResult(ByVal sEconomicValue As Single, ByVal sSocialValue As Single, _
        ByVal sMandatedValue As Single, ByVal sEcologicalValue As Single, _
        ByVal sBiomassDiversityValue As Single, ByVal sBoundaryWeightValue As Single, _
        ByVal sTotalWeighted As Single, ByVal sPercClosed As Single)

        Me(1, eColumnTypes.Value).Value = sEconomicValue
        Me(2, eColumnTypes.Value).Value = sSocialValue
        Me(3, eColumnTypes.Value).Value = sMandatedValue
        Me(4, eColumnTypes.Value).Value = sEcologicalValue
        Me(5, eColumnTypes.Value).Value = sBiomassDiversityValue
        Me(6, eColumnTypes.Value).Value = sBoundaryWeightValue
        Me(7, eColumnTypes.Value).Value = sTotalWeighted
        Me(8, eColumnTypes.Value).Value = sPercClosed

        Me.InvalidateCells()

    End Sub

End Class
