#Region " Imports "

Option Strict On
Imports System
Imports System.Drawing

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing a collection of <see cref="Drawing">Drawing</see>-related utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDrawingUtils

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a <see cref="ContentAlignment">ContentAlignment</see> flag
        ''' into a <see cref="StringFormat">StringFormat</see> flag.
        ''' </summary>
        ''' <param name="ca">Content alignment flag to convert.</param>
        ''' <returns>A StringFormat value.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ContentAlignmentToStringFormat(ByVal ca As System.Drawing.ContentAlignment) As StringFormat
            Dim style As New StringFormat()

            Select Case ca
                Case ContentAlignment.BottomLeft
                    style.Alignment = StringAlignment.Near
                    style.LineAlignment = StringAlignment.Far
                Case ContentAlignment.BottomRight
                    style.Alignment = StringAlignment.Far
                    style.LineAlignment = StringAlignment.Far
                Case ContentAlignment.BottomCenter
                    style.Alignment = StringAlignment.Center
                    style.LineAlignment = StringAlignment.Far
                Case ContentAlignment.MiddleLeft
                    style.Alignment = StringAlignment.Near
                    style.LineAlignment = StringAlignment.Center
                Case ContentAlignment.MiddleRight
                    style.Alignment = StringAlignment.Far
                    style.LineAlignment = StringAlignment.Center
                Case ContentAlignment.MiddleCenter
                    style.Alignment = StringAlignment.Center
                    style.LineAlignment = StringAlignment.Center
                Case ContentAlignment.TopLeft
                    style.Alignment = StringAlignment.Near
                    style.LineAlignment = StringAlignment.Near
                Case ContentAlignment.TopRight
                    style.Alignment = StringAlignment.Far
                    style.LineAlignment = StringAlignment.Near
                Case ContentAlignment.TopCenter
                    style.Alignment = StringAlignment.Center
                    style.LineAlignment = StringAlignment.Near
            End Select
            Return style

        End Function

    End Class

End Namespace

