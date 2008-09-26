'==============================================================================
'
' $Log: ListBoxEX.vb,v $
' Revision 1.1  2008/09/26 07:31:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:09  jeroens
' Separated from Scientific Interface
'
' Revision 1.2  2006/09/30 13:27:27  jeroens
' Added header, strict ON
'
'==============================================================================

Option Strict On

Namespace Controls
    Public Class ListBoxEX
        Inherits System.Windows.Forms.ListBox
        Public Shadows Event Resize(ByVal sender As Object, ByVal e As EventArgs, ByVal i As Integer)
        Public Shadows Event DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        Public Shadows Event MeasureItem(ByVal sender As Object, ByVal e As MeasureItemEventArgs)

        Private WithEvents mListBox As System.Windows.Forms.ListBox
        Private IcoFolder As String = Application.StartupPath() + "\Ico\"
        Private Icons As New ArrayList

        Public Shadows Function Items(ByVal idx As Integer) As ItemEX
            Return CType(mListBox.Items(idx), ItemEX)
        End Function

        Public Class ItemEX
            Public IconIndex As Integer
            Public Text As String
            Public UserID As Integer
            Public User As String

            Public Sub New(ByVal _User As String, ByVal _Text As String, ByVal _IconIndex As Integer, ByVal _UserID As String)
                User = _User
                Text = _Text
                IconIndex = _IconIndex
                UserID = CInt(_UserID)
            End Sub
        End Class

        Public Property IconsFolder() As String
            Get
                Return IcoFolder
            End Get
            Set(ByVal Value As String)
                IcoFolder = Value
            End Set
        End Property

        Public Sub AddIcon(ByVal filename As String)
            Dim icona As Icon
            Dim fname As String

            fname = IconsFolder.ToString + filename
            icona = New Icon(fname)
            Icons.Add(icona)
        End Sub

        Public Sub AddIcon(ByVal iconName As Icon)
            Icons.Add(iconName)
        End Sub

        Public Sub AddIcon(ByVal imagename As Image)

            Dim bmp As Bitmap = New Bitmap(imagename)
            Dim icona As Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon())

            Icons.Add(icona)
        End Sub


        Public Sub New()
            MyBase.New()
            Me.DrawMode = Windows.Forms.DrawMode.OwnerDrawVariable
            mListBox = Me
        End Sub

        Public Sub Add(ByVal _User As String, ByVal _Text As String, Optional ByVal _iconindex As Integer = -1, Optional ByVal _UserID As Integer = -1)
            mListBox.Items.Add(New ItemEX(_User, _Text, _iconindex, CStr(_UserID)))
        End Sub

        Private Sub mListBox_Resize(ByVal sender As Object, ByVal e As System.EventArgs) _
          Handles mListBox.Resize
            RaiseEvent Resize(sender, e, 1)
        End Sub

        Private Sub DrawItemHandler(ByVal sender As Object, ByVal e As DrawItemEventArgs) Handles mListBox.DrawItem
            e.DrawBackground()
            e.DrawFocusRectangle()
            Dim titFont As New Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim subTitFont As New Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim titBrush As New SolidBrush(Color.DimGray)
            Dim subTitBrush As New SolidBrush(Color.Gray)
            Dim idx As Integer = e.Index
            e.Graphics.DrawString(DirectCast(mListBox.Items(idx), ItemEX).Text, titFont, titBrush, e.Bounds.Left + 30, e.Bounds.Top)
            e.Graphics.DrawString(DirectCast(mListBox.Items(idx), ItemEX).User, subTitFont, subTitBrush, e.Bounds.Left + 30, e.Bounds.Top + titFont.Height + 2)
            e.Graphics.DrawLine(New Pen(Color.LightGray), 0, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1)
            If Items(idx).IconIndex > -1 Then
                e.Graphics.DrawIcon(DirectCast(Icons(Items(idx).IconIndex), Icon), e.Bounds.Left + 1, e.Bounds.Top + 2)
            End If
        End Sub

        Private Sub MeasureItemHandler(ByVal sender As Object, ByVal e As MeasureItemEventArgs) Handles mListBox.MeasureItem
            e.ItemHeight = 40
            RaiseEvent MeasureItem(sender, e)
        End Sub
    End Class

End Namespace