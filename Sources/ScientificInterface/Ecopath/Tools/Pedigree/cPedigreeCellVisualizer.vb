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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2

#End Region 'Imports

Namespace Ecopath.Tools

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Visualizer for rendering pedigree cells in the lovely
    ''' <see cref="gridPedigree">pedigree grid</see>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)>
    Public Class cPedigreeCellVisualizer
        Inherits cEwEGridVisualizerBase

        Private m_psg As cPedigreeStyleGuide = Nothing

        Public Sub New(ByVal psg As cPedigreeStyleGuide)
            Me.m_psg = psg
        End Sub

        ''' <summary>
        ''' Helper method, returns a pedigree level for a given cell.
        ''' </summary>
        ''' <param name="cell">The cell to obtain pedigree info for.</param>
        ''' <param name="pos">The position to obtain pedigree info for.</param>
        ''' <returns>A <see cref="cPedigreeLevel">pedigree level</see>, or
        ''' Nothing if something went wrong.</returns>
        Private Function GetLevel(ByVal cell As SourceGrid2.Cells.ICellVirtual,
                              ByVal pos As SourceGrid2.Position) As cPedigreeLevel

            Try

                ' Sanity checks
                If (cell Is Nothing) Then Return Nothing

                Dim value As Object = cell.GetValue(pos)
                If (value Is Nothing) Then Return Nothing

                ' Need an integer value representing a level index
                'If Not (TypeOf value Is Integer) Then Return Nothing

                ' Cannot find catch manager>

                Dim core As cCore = Me.Core(cell)
                Dim var As eVarNameFlags = core.PedigreeVariable(pos.Column - 1)
                Dim man As cPedigreeManager = core.GetPedigreeManager(var)
                Dim iCV As Integer = CInt(value)
                Dim iLevel As Integer = man.PedigreeGroupLevel(pos.Row)

                If (iCV > 0) Then Return Nothing
                If (iLevel > 0) Then
                    Return man.Level(iLevel)
                End If
                Return Nothing

            Catch ex As Exception
                ' Whoah
            End Try

            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw pedigree cell content background bits.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Background(
            ByVal cell As SourceGrid2.Cells.ICellVirtual,
            ByVal pos As SourceGrid2.Position,
            ByVal e As System.Windows.Forms.PaintEventArgs,
            ByVal rc As System.Drawing.Rectangle,
            ByVal status As SourceGrid2.DrawCellStatus)

            MyBase.DrawCell_Background(cell, pos, e, rc, status)

            Dim level As cPedigreeLevel = Me.GetLevel(cell, pos)
            If (level Is Nothing) Then
                Return
            End If

            Using br As New SolidBrush(Me.m_psg.BackgroundColor(Color.Transparent, level))
                e.Graphics.FillRectangle(br, New Rectangle(rc.Left + 4, rc.Top + 3, rc.Width - 8, rc.Height - 6))
            End Using

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw pedigree cell content text.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_ImageAndText(
            ByVal cell As SourceGrid2.Cells.ICellVirtual,
            ByVal pos As SourceGrid2.Position,
            ByVal e As System.Windows.Forms.PaintEventArgs,
            ByVal rc As System.Drawing.Rectangle,
            ByVal status As SourceGrid2.DrawCellStatus)

            Dim level As cPedigreeLevel = Me.GetLevel(cell, pos)

            Dim style As cStyleGuide.eStyleFlags = 0
            Dim clrFore As Color = Me.ForeColor
            Dim clrBack As Color = Nothing ' Not used here
            Dim rcBorder As RectangleBorder = Me.Border
            Dim fontCell As Font = Me.GetCellFont()
            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim fmt As StringFormat = Me.StringFormat
            Dim iValueAlt As Integer = cCore.NULL_VALUE
            Dim strText As String = ""

            Dim val As Object = cell.GetValue(pos)
            If (val IsNot Nothing) Then
                If (TypeOf val Is Integer) Then
                    iValueAlt = CInt(val)
                End If
            End If
            strText = Me.m_psg.DisplayText(level, iValueAlt)

            ' Rendering a cell with an associated property?
            If (TypeOf cell Is EwECellBase) Then
                ' #Yes: obtain cell style
                style = DirectCast(cell, EwECellBase).Style()
                If (sg IsNot Nothing) Then
                    ' Get SG colours for this style
                    sg.GetStyleColors(style, clrFore, clrBack)
                End If
            End If

            fmt.Alignment = StringAlignment.Center
            fmt.LineAlignment = StringAlignment.Center

            ' Render Image and Text in determined fore colour and text
            Utility.PaintImageAndText(e.Graphics, rc,
            Me.Image, Me.ImageAlignment, Me.ImageStretch,
            strText, fmt,
            Me.AlignTextToImage, Me.Border,
            clrFore, Me.GetCellFont())

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Borrow core reference from parent cell, if possible.
        ''' </summary>
        ''' <param name="cell">Cell to borrow core from.</param>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property Core(ByVal cell As SourceGrid2.Cells.ICellVirtual) As cCore
            Get
                If (TypeOf cell Is IUIElement) Then
                    Dim uic As cUIContext = DirectCast(cell, IUIElement).UIContext
                    If (uic IsNot Nothing) Then
                        Return uic.Core
                    End If
                End If
                Return Nothing
            End Get
        End Property

    End Class

End Namespace
