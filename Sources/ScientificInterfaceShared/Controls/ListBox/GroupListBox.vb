'==============================================================================
'
' $Log: GroupListBox.vb,v $
' Revision 1.2  2009/03/23 18:44:46  jeroens
' Localized
'
' Revision 1.1  2009/03/19 16:54:10  jeroens
' Moved
'
' Revision 1.4  2008/12/15 15:37:28  jeroens
' no message
'
' Revision 1.3  2008/12/04 06:35:29  sherman
' Fixed Show/Hide refresh bug
' Fixed disposed bug
'
' Revision 1.2  2008/11/29 19:00:11  sherman
' Updated bugs and rescaling in RunEcosim plot
'
' Revision 1.1  2008/09/26 07:31:17  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Listbox devived class meant for providing a list of Ecopath groups.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class GroupListBox
        Inherits ListBox

#Region " Helper classes "

#Region " LegendListBox.GroupItem "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' An item for a LegendListBox
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Class GroupItem

            ''' <summary>Group to show</summary>
            Private m_group As cCoreGroupBase = Nothing

            ''' <summary>
            ''' Creates a new item for usage in the LegendListBox
            ''' </summary>
            ''' <param name="group">Group to link to</param>
            Public Sub New(ByVal group As cCoreGroupBase)
                Me.m_group = group
            End Sub

            ''' <summary>
            ''' Formats the item text for display in the list box
            ''' </summary>
            ''' <returns>The formatted item text</returns>
            Public Overrides Function ToString() As String
                If Me.m_group IsNot Nothing Then Return Me.m_group.Name
                Return My.Resources.GENERIC_VALUE_ALL
            End Function

            ''' <summary>
            ''' Gets the item text for the item
            ''' </summary>
            Public ReadOnly Property Group() As cCoreGroupBase
                Get
                    Return Me.m_group
                End Get
            End Property

        End Class

#End Region ' LegendListBox.Item

#End Region ' Helper classes

#Region " Privates "

        Private m_core As cCore = Nothing
        Private m_sg As StyleGuide = Nothing

#End Region ' Privates

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new LegendListBox
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            ' Connect
            Me.m_sg = StyleGuide.GetInstance()
            Me.m_core = cCore.GetInstance()

            ' This box draws its own items
            Me.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            AddHandler Me.DrawItem, AddressOf Me.DrawItemHandler

            AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

        Private Sub LegendListBox_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.DrawItem, AddressOf Me.DrawItemHandler
            RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.m_sg = Nothing
            Me.m_core = Nothing
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler for rendering a legend list box item
        ''' </summary>
        ''' <param name="sender">Event source</param>
        ''' <param name="e">Event parameters</param>
        ''' -----------------------------------------------------------------------
        Private Sub DrawItemHandler(ByVal sender As Object, ByVal e As DrawItemEventArgs) Handles Me.DrawItem

            If (e.Index >= Me.Items.Count Or e.Index < 0) Then Return

            ' Get the item
            Dim item As Object = Me.Items(e.Index)
            ' Set default item colour
            Dim clrLegend As Color = Color.Transparent
            Dim clrText As Color = e.ForeColor

            ' Attempt to get item colour if it is a LegendListBox.Item
            If (TypeOf item Is GroupListBox.GroupItem) Then
                ' Get group
                Dim gi As GroupListBox.GroupItem = DirectCast(item, GroupListBox.GroupItem)
                ' Extract the item colour
                If gi.Group IsNot Nothing Then
                    If Me.m_sg.GroupVisible(gi.Group.Index) Then
                        clrLegend = Me.m_sg.GroupColor(Me.m_core, gi.Group.Index)
                        clrText = e.ForeColor
                    Else
                        clrLegend = System.Drawing.SystemColors.InactiveCaption
                        clrText = System.Drawing.SystemColors.InactiveCaption
                    End If
                End If
            End If

            ' TODO: Take current culture into consideration here. Right-to-left reading order cultures
            ' will need the colour box to be displayed on the right-hand side of the text.

            ' Render default background 
            e.DrawBackground()
            ' Render default text, bumped to the right by 22 pixels
            e.Graphics.DrawString(item.ToString(), e.Font, New SolidBrush(clrText), e.Bounds.X + 22, e.Bounds.Y)

            If clrLegend.A > 0 Then
                ' Render colour box
                e.Graphics.FillRectangle(New SolidBrush(clrLegend), e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                e.Graphics.DrawRectangle(New Pen(Drawing.Color.Black, 1), e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
            End If
            ' Render default focus rectangle
            e.DrawFocusRectangle()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Hmmm...
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub InitializeComponent()
            Me.SuspendLayout()
            '
            'LegendListBox
            '
            Me.ResumeLayout(False)
        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As StyleGuide.eChangeType)
            'If ((ct And StyleGuide.eChangeType.Colours) = StyleGuide.eChangeType.Colours) Then
            Me.Invalidate()
            'End If
        End Sub

    End Class

End Namespace
