' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Drawing
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

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
