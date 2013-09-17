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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.ComponentModel

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' Control that renders a vertical legend bar.
    ''' </summary>
    Public Class ucLegendBar
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_colors As New List(Of Color)

        Public Sub New()
            MyBase.New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Me.UIContext = Nothing
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        ''' <inheritdocs cref="IUIElement.UIContext"/>
        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged
                End If
                Me.m_uic = UIContext
                If (Me.m_uic IsNot Nothing) Then
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged
                End If
            End Set
        End Property

        Public Property Colors As List(Of Color)
            Get
                Return Me.m_colors
            End Get
            Set(value As List(Of Color))
                Me.m_colors.Clear()
                If (value IsNot Nothing) Then
                    Me.m_colors.AddRange(value)
                End If
            End Set
        End Property

        <Browsable(True), DefaultValue("High")> _
        Public Property LabelHigh As String = "High"

        <Browsable(True), DefaultValue("Low")> _
        Public Property LabelLow As String = "Low"

        <Browsable(True), _
         DefaultValue(80), _
         Description("Percentage that the bar occupies of the control")> _
        Public Property BarWidthPercentage As Integer = 80

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            Dim g As Graphics = e.Graphics
            Dim ft As Font = Nothing

            If (Me.UIContext IsNot Nothing) Then
                ft = Me.UIContext.StyleGuide.Font(Style.cStyleGuide.eApplicationFontType.Legend)
            Else
                ft = Me.Font
            End If

            Dim rcClient As Rectangle = Me.ClientRectangle

            Dim szHigh As SizeF = g.MeasureString(Me.LabelHigh, ft)
            Dim rcHigh As New Rectangle(Me.Padding.Left, Me.Padding.Top, _
                                        rcClient.Width - Me.Padding.Horizontal, CInt(szHigh.Height))

            Dim szLow As SizeF = g.MeasureString(Me.LabelLow, ft)
            Dim rcLow As New Rectangle(Me.Padding.Left, CInt(rcClient.Height - Me.Padding.Bottom - szLow.Height), _
                                       rcClient.Width - Me.Padding.Horizontal, CInt(szLow.Height))

            Dim iWidth As Integer = CInt(Math.Min((Me.Width - Me.Padding.Horizontal) * Me.BarWidthPercentage / 100, Me.Width - Me.Padding.Horizontal))
            Dim iHeight As Integer = CInt(rcClient.Height - 2 * Me.Padding.Vertical - rcHigh.Height - rcLow.Height)
            Dim rcBox As New Rectangle(CInt((Me.Width - iWidth) / 2), CInt(Me.Padding.Vertical + rcHigh.Height), iWidth, iHeight)

            ' Back
            Using br As New SolidBrush(Me.BackColor)
                g.FillRectangle(br, rcClient)
            End Using

            Me.DrawLabel(g, Me.LabelHigh, ft, rcHigh)
            Me.DrawLabel(g, Me.LabelLow, ft, rcLow)
            Me.DrawBox(g, rcBox)

            If (Me.UIContext IsNot Nothing) Then
                ft.Dispose()
            End If

        End Sub

        Private Sub DrawLabel(g As Graphics, strText As String, ft As Font, rc As Rectangle)

            Dim fmt As New StringFormat()
            fmt.FormatFlags = StringFormatFlags.NoWrap
            fmt.Alignment = StringAlignment.Center
            fmt.LineAlignment = StringAlignment.Center
            fmt.Trimming = StringTrimming.None
            g.DrawString(strText, ft, SystemBrushes.WindowText, rc, fmt)

        End Sub

        Private Sub DrawBox(g As Graphics, rcBox As Rectangle)

            If (Me.Colors Is Nothing) Then
                g.FillRectangle(SystemBrushes.GrayText, rcBox)
            Else
                Dim iNumCols As Integer = Math.Max(Me.Colors.Count, 1)
                Dim sHeight As Single = CSng(rcBox.Height / iNumCols)

                Dim brTmp As SolidBrush = Nothing
                For i As Integer = 1 To iNumCols
                    brTmp = New SolidBrush(Me.Colors(i - 1))
                    g.FillRectangle(brTmp, rcBox.X, rcBox.Y + rcBox.Height - sHeight * i, rcBox.Width, sHeight)
                    brTmp.Dispose()
                Next
            End If

        End Sub

#Region " Events "

        Private Sub OnStyleguideChanged(ct As Style.cStyleGuide.eChangeType)
            If ((ct And Style.cStyleGuide.eChangeType.Fonts) > 0) Then
                Me.Invalidate()
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace
