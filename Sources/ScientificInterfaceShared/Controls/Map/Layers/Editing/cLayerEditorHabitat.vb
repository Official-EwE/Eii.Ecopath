#Region " Imports "

Option Strict On

Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of habitat layers. Setting
    ''' a cell value in one habitat will clear the cell values in another.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorHabitat
        Inherits cLayerEditorTwoState

        Private m_iHab As Integer = 0

        Public Sub New()
            Me.New(0)
        End Sub

        Public Sub New(iHab As Integer)
            MyBase.New(Nothing)
            Me.m_iHab = iHab
        End Sub

        Protected Overrides Sub SetCellValue(ptSet As System.Drawing.Point, _
                                             value As Object, _
                                             e As System.Windows.Forms.MouseEventArgs, _
                                             ptClick As System.Drawing.Point)

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_iHab = 0) Then Return

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap

            If (Me.Layer.ValueSet.Equals(value)) Then
                ' Hackerdihack: clear cell for all other habitat layers
                For i As Integer = 1 To core.nHabitats
                    If (i <> Me.m_iHab) Then
                        bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X) = Me.Layer.ValueClear
                    End If
                Next
            End If
            MyBase.SetCellValue(ptSet, value, e, ptClick)
        End Sub

    End Class

End Namespace