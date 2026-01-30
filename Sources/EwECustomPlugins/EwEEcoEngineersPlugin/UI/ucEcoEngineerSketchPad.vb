' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Drawing
Imports ScientificInterfaceShared.Controls



''' <summary>
''' Special sketch pad for the complexity preview.
''' </summary>
Public Class ucEcoEngineerSketchPad
    Inherits ScientificInterfaceShared.Controls.ucForcingSketchPad

    ''' <summary>
    ''' We can make this value configurable by the user in the UI
    ''' </summary>
    Public Property MaxXValue As Single = 25000

    Public Sub New()
        Me.XAxisMaxValue = 1200
    End Sub

    Protected Overrides Sub GetXAxisLabels(iWidth As Integer, ByRef astrLabels() As String, ByRef sScale As Single)

        Dim lstrAxis As New List(Of String)

        If (Me.Shape IsNot Nothing) Then
            For sLabel As Single = 0 To Me.MaxXValue Step Me.MaxXValue / 5
                lstrAxis.Add(Me.UIContext.StyleGuide.FormatNumber(sLabel))
            Next
        End If
        astrLabels = lstrAxis.ToArray()

    End Sub

    Protected Overrides Function GetShapeTitle() As String
        If (Me.Shape Is Nothing) Then Return ""
        Return Me.Shape.Name
    End Function

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ucEcoEngineerSketchPad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.Name = "ucEcoEngineerSketchPad"
        Me.Size = New System.Drawing.Size(1153, 552)
        Me.ResumeLayout(False)

    End Sub
End Class
