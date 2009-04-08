'==============================================================================
'
' $Log: GroupListBox.vb,v $
' Revision 1.3  2009/04/08 15:09:10  jeroens
' GroupListBox -> cGroupListBox
'
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
    ''' Listbox devived class meant for showing colour-coded Ecopath groups.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cGroupListBox
        Inherits ListBox

#Region " Helper classes "

#Region " LegendListBox.GroupItem "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' An item for a cGroupListBox
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Class cGroupItem

            ''' <summary>Group to show.</summary>
            Private m_group As cCoreGroupBase = Nothing
            ''' <summary>States if group must be displayed as relevant.</summary>
            Private m_bIsRelevant As Boolean = True

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Creates a new item for usage in the GroupListBox.
            ''' </summary>
            ''' <param name="group">Group to link to.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal group As cCoreGroupBase)
                Me.m_group = group
                Me.m_bIsRelevant = True
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Formats the item text for display in the GroupListBox.
            ''' </summary>
            ''' <returns>The formatted item text.</returns>
            ''' ---------------------------------------------------------------
            Public Overrides Function ToString() As String
                If Me.m_group IsNot Nothing Then Return Me.m_group.Name
                Return My.Resources.GENERIC_VALUE_ALL
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the group linked to the item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Group() As cCoreGroupBase
                Get
                    Return Me.m_group
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether a group should be displayed as relevant.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property IsRelevant() As Boolean
                Get
                    Return Me.m_bIsRelevant
                End Get
                Set(ByVal bIsRelevant As Boolean)
                    Me.m_bIsRelevant = bIsRelevant
                End Set
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
        ''' Creates a new cGroupListBox
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()

            ' Connect
            Me.m_core = cCore.GetInstance()
            Me.m_sg = StyleGuide.GetInstance()
            AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' This box draws its own items
            Me.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)

            If (Me.m_core IsNot Nothing) Then
                RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
                Me.m_sg = Nothing
                Me.m_core = Nothing
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to render an item
        ''' </summary>
        ''' <param name="e">Event parameters</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)

            If (e.Index >= Me.Items.Count Or e.Index < 0) Then Return

            Dim item As Object = Me.Items(e.Index)
            Dim gi As cGroupListBox.cGroupItem = Nothing
            Dim clrLegend As Color = Color.Transparent
            Dim clrText As Color = e.ForeColor

            ' Attempt to get item colour if it is a cGroupItem
            If (TypeOf item Is cGroupListBox.cGroupItem) Then
                ' Get item group
                gi = DirectCast(item, cGroupListBox.cGroupItem)
                ' Has a group attached?
                If (gi.Group IsNot Nothing) Then
                    ' #Yes: use dimmed colours
                    clrLegend = SystemColors.Control
                    clrText = SystemColors.ControlDark
                    ' Allowed to display and colour group?
                    If Me.m_sg.GroupVisible(gi.Group.Index) And gi.IsRelevant Then
                        ' #Yes: display group in full color
                        clrText = e.ForeColor
                        clrLegend = Me.m_sg.GroupColor(Me.m_core, gi.Group.Index)
                    End If
                End If
            End If

            ' TODO: Take current culture into consideration here. Right-to-left reading order cultures
            ' will need the colour box to be displayed on the right-hand side of the text.

            ' Render default background 
            e.DrawBackground()
            ' Render default text, bumped to the right by 22 pixels
            e.Graphics.DrawString(item.ToString(), e.Font, New SolidBrush(clrText), e.Bounds.X + 22, e.Bounds.Y)

            If (clrLegend.A > 0) Then
                ' Render colour box
                Using br As New SolidBrush(clrLegend)
                    e.Graphics.FillRectangle(br, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                End Using
                ' Render colour box border
                Using p As New Pen(clrText, 1)
                    e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                End Using
            End If

            ' Render default focus rectangle
            e.DrawFocusRectangle()

        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As StyleGuide.eChangeType)
            Me.Invalidate()
        End Sub

    End Class

End Namespace
