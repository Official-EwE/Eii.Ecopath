'==============================================================================
'
' $Log: ucIndexedItemListView.vb,v $
' Revision 1.1  2008/09/26 07:31:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/21 18:26:30  jeroens
' Structured Win32 info
'
' Revision 1.1  2008/06/01 23:45:10  jeroens
' Separated from Scientific Interface
'
' Revision 1.8  2008/05/20 14:56:28  jeroens
' Tweaked rendering a little further
'
' Revision 1.7  2008/01/06 18:25:54  jeroens
' - Removed obsolete logic
'
' Revision 1.6  2007/11/19 22:13:44  jeroens
' * Fixed bug 342
'
' Revision 1.5  2007/11/18 18:52:26  jeroens
' * Removed ref to unused MEASUREITEMSTRUCT
'
' Revision 1.4  2007/11/18 13:36:49  jeroens
' * Properly drawn label
'
' Revision 1.3  2007/10/15 00:48:26  jeroens
' * FF display ID, ICoreInputOutput display Index on numbered labels
'
' Revision 1.2  2007/10/14 16:35:20  jeroens
' + Prevented potential crash
'
' Revision 1.1  2007/10/14 01:46:13  jeroens
' * Initial version
'
'==============================================================================

Option Strict On

Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.ComponentModel
Imports EwECore
Imports EwEUtils.Win32Api.Win32
Imports EwEUtils.Win32Api.User32

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Owner-drawn list view that displays shape thumbnails.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Ownerdraw logic based on code "Owner-draw ListView control" by Ralph Arvesen,
    ''' http://blogs.vertigosoftware.com/ralph/archive/2004/08/09/478.aspx.
    ''' </para>
    ''' <para>
    ''' Item detection logic based on "WinForms ListView Find and GetSubItemRect" by
    ''' Stumpy842 (http://www.codeproject.com/script/Articles/list_articles.asp?userid=1297321),
    ''' http://www.codeproject.com/vb/net/LVFind.asp.
    ''' </para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class ucIndexedItemListView
        : Inherits ListView

        Public Sub New()
            MyBase.New()

            ' Peace, brother. Peace!
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

#Region " Ownerdraw bits "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Override the window proc and look for the custom draw messages.
        ''' </summary>
        ''' <param name="msg"><see cref="Message">Message</see> to process.</param>
        ''' -----------------------------------------------------------------------
        <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
        Protected Overrides Sub WndProc(ByRef msg As Message)

            ' Is listview item owner draw message?
            If (msg.Msg = OCM.OCM_NOTIFY) Then
                ' #Yes: get notification info
                Dim notifyHeader As NMHDR = CType(msg.GetLParam(GetType(NMHDR)), NMHDR)
                ' Is custom draw message?
                If ((notifyHeader.hwndFrom.Equals(Me.Handle)) And (notifyHeader.code = NM.NM_CUSTOMDRAW)) Then
                    ' #Yes: Process the message
                    If ProcessListCustomDraw(msg) Then
                        ' Done, abort
                        Return
                    End If
                End If
            End If

            MyBase.WndProc(msg)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' One step closer to detecting if a listview item should be drawn
        ''' </summary>
        ''' <param name="m"></param>
        ''' <returns>True if ownerdrawn.</returns>
        ''' -----------------------------------------------------------------------
        Private Function ProcessListCustomDraw(ByRef m As Message) As Boolean

            Dim bDrawSelf As Boolean = False

            ' Get custom draw information
            Dim customDraw As NMCUSTOMDRAW = CType(m.GetLParam(GetType(NMCUSTOMDRAW)), NMCUSTOMDRAW)

            ' Return different values in the message depending on the draw stage
            Select Case customDraw.dwDrawStage
                Case CDDS.CDDS_PREPAINT
                    m.Result = New System.IntPtr(CDRF.CDRF_NOTIFYITEMDRAW)

                Case CDDS.CDDS_ITEMPREPAINT
                    m.Result = New System.IntPtr(CDRF.CDRF_SKIPDEFAULT)

                    ' Is item visible?
                    If IsItemVisible(customDraw.dwItemSpec) Then
                        ' #Yes: draw the listview item
                        Dim g As Graphics = Graphics.FromHdc(customDraw.hdc)
                        Try
                            DrawIndexedItem(g, CInt(customDraw.dwItemSpec))
                            bDrawSelf = True
                        Finally
                            g.Dispose()
                        End Try
                    Else
                        bDrawSelf = True
                    End If

                Case Else
                    m.Result = New System.IntPtr(CDRF.CDRF_DODEFAULT)
            End Select

            Return bDrawSelf
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Check whether the listview item is visible or not.
        ''' </summary>
        ''' <param name="iItem">Index of the item to test.</param>
        ''' <returns>True if visible.</returns>
        ''' -----------------------------------------------------------------------
        Private Function IsItemVisible(ByVal iItem As Integer) As Boolean

            If ((iItem < 0) Or (iItem >= Me.Items.Count)) Then Return False
            Dim rc As Rectangle = Me.GetItemRect(iItem)
            Return ((Me.DisplayRectangle.Contains(rc.Left, rc.Top)) Or (Me.DisplayRectangle.Contains(rc.Right, rc.Bottom)))

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Draw the item.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="iItem"></param>
        ''' -----------------------------------------------------------------------
        Private Sub DrawIndexedItem(ByVal g As Graphics, ByVal iItem As Integer)

            Dim item As ListViewItem = Me.Items(iItem)
            Dim rc As Rectangle = Me.GetItemRect(iItem)
            Dim image As Image = Nothing
            Dim strText As String = item.Text
            Dim sf As New StringFormat

            ' Does item have a tag attached?
            If (item.Tag IsNot Nothing) Then
                ' #Yes: is this of type ICoreInterface?
                If (TypeOf item.Tag Is ICoreInterface) Then
                    ' #Yes: include label number in the item text
                    If (TypeOf item.Tag Is cForcingFunction) Then
                        strText = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (DirectCast(item.Tag, cForcingFunction).ID + 1), strText)
                    Else
                        strText = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, DirectCast(item.Tag, ICoreInterface).Index, strText)
                    End If
                End If
            End If

            ' Clear entire item rect
            g.FillRectangle(SystemBrushes.Window, rc)

            ' Ouch, this is VERY arbitrary and will probably come back to bite us...
            rc.X += 3
            rc.Y += 3
            rc.Width -= 6
            rc.Height -= 3

            ' Draw image
            If Me.LargeImageList IsNot Nothing Then
                If item.ImageIndex < Me.LargeImageList.Images.Count Then
                    image = Me.LargeImageList.Images(item.ImageIndex)

                    If image IsNot Nothing Then
                        g.DrawImage(image, rc.X + CInt((rc.Width - image.Width) / 2), rc.Y)
                        rc.Y += (image.Height + 2)
                        rc.Height -= (image.Height + 2)
                    End If
                End If
            End If

            ' Render label
            sf.Alignment = StringAlignment.Center

            Dim iNumCharsFitted As Integer
            Dim iNumLinesFilled As Integer
            Dim szfText As SizeF = g.MeasureString(strText, Me.Font, New SizeF(rc.Width, rc.Height), sf, iNumCharsFitted, iNumLinesFilled)

            rc.X += CInt((rc.Width - szfText.Width) / 2)
            rc.Width = CInt(szfText.Width)
            rc.Height = CInt(szfText.Height)

            If item.Selected Then
                g.FillRectangle(SystemBrushes.Highlight, rc)
                g.DrawString(strText, Me.Font, SystemBrushes.HighlightText, rc, sf)
            Else
                g.DrawString(strText, Me.Font, SystemBrushes.WindowText, rc, sf)
            End If

        End Sub

#End Region ' Ownerdraw bits

    End Class

End Namespace ' Controls
