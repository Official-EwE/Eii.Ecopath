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
        Private m_core As cCore = Nothing

        Public Sub New(iHab As Integer, core As cCore)
            MyBase.New(Nothing)
            Me.m_iHab = iHab
            Me.m_core = core
        End Sub

        Protected Overrides Sub SetCellValue(ptSet As System.Drawing.Point, _
                                             value As Object, _
                                             e As System.Windows.Forms.MouseEventArgs, _
                                             ptClick As System.Drawing.Point)

            If (Me.Layer.ValueSet.Equals(value)) Then
                ' Hackerdihack: clear cell for all other habitat layers
                For i As Integer = 1 To Me.m_core.nHabitats
                    If (i <> Me.m_iHab) Then
                        Me.m_core.EcospaceBasemap.LayerHabitat(i).Cell(ptSet.Y, ptSet.X) = Me.Layer.ValueClear
                    End If
                Next
            End If
            MyBase.SetCellValue(ptSet, value, e, ptClick)
        End Sub

    End Class

End Namespace