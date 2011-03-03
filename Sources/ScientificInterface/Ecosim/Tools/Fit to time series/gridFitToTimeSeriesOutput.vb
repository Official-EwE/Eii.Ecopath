#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecosim

    ''' <summary>
    ''' Grid for displaying Fit to time series run results
    ''' </summary>
    <CLSCompliant(False)> _
    Public Class gridFitToTimeSeriesOutput
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            TimeStamp
            NoParams
            SS
            AIC
        End Enum

        Private Structure sOutput

            Public NumParams As Integer
            Public SS As Single
            Public AIC As Single

            Public Sub New(ByVal iNumParams As Integer, ByVal sSS As Single, ByVal sAIC As Single)
                Me.NumParams = iNumParams
                Me.SS = sSS
                Me.AIC = sAIC
            End Sub

        End Structure

        Private m_lData As New List(Of sOutput)

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1 + Me.m_lData.Count, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.TimeStamp) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.NoParams) = New EwEColumnHeaderCell(SharedResources.HEADER_NUMPARAMS)
            Me(0, eColumnTypes.SS) = New EwEColumnHeaderCell(SharedResources.HEADER_SS)
            Me(0, eColumnTypes.AIC) = New EwEColumnHeaderCell(SharedResources.HEADER_AIC)

            Me.FixedColumnWidths = True
            Me.FixedColumns = 1

        End Sub

        Protected Overrides Sub FillData()
            For i As Integer = 0 To Me.m_lData.Count - 1
                Dim out As sOutput = Me.m_lData(i)
                Me(i + 1, eColumnTypes.TimeStamp) = New EwERowHeaderCell(i + 1)
                Me(i + 1, eColumnTypes.NoParams) = New EwECell(out.NumParams, GetType(Integer), cStyleGuide.eStyleFlags.NotEditable)
                Me(i + 1, eColumnTypes.SS) = New EwECell(out.SS, GetType(Single), cStyleGuide.eStyleFlags.NotEditable)
                Me(i + 1, eColumnTypes.AIC) = New EwECell(out.AIC, GetType(Single), cStyleGuide.eStyleFlags.NotEditable)
            Next
        End Sub

        Public Sub AddFitToTimeSeriesOutput(ByVal iNumParams As Integer, ByVal sSS As Single, ByVal sAIC As Single)
            Me.m_lData.Add(New sOutput(iNumParams, sSS, sAIC))
            Me.RefreshContent()
        End Sub

        Public Sub ClearOutput()
            Me.m_lData.Clear()
            Me.RefreshContent()
        End Sub

    End Class

End Namespace
