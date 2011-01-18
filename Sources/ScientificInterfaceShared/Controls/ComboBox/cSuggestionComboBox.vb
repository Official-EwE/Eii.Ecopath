#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports SAUPUtil.Misc.GeoCode

#End Region ' Imports

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Combo box for searching geocoded locations.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cSuggestionComboBox
        Inherits ComboBox

#Region " Helper classes "

        Private Class cGeoLocationItem
            Private m_item As cGeoCodeLocation

            Public Sub New(ByVal item As cGeoCodeLocation)
                Me.m_item = item
            End Sub

            Public Overrides Function ToString() As String
                Return Me.m_item.Description
            End Function

            ReadOnly Property Item() As cGeoCodeLocation
                Get
                    Return Me.m_item
                End Get
            End Property
        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private m_lookup As cGeoCodeLookup = Nothing
        Private m_bSearching As Boolean = False
        Private m_bSelecting As Boolean = False

#End Region ' Private vars

#Region " Public interfaces "

        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cGeoCodeLookup">Geo code looup engine</see> to use.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Provider() As cGeoCodeLookup
            Get
                Return Me.m_lookup
            End Get
            Set(ByVal value As cGeoCodeLookup)
                Me.m_lookup = value
            End Set
        End Property

        Public Shadows Property Text() As String
            Get
                Try
                    Return MyBase.Text
                Catch ex As Exception
                    Return ""
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    MyBase.Text = value
                Catch ex As Exception
                    ' NOP
                End Try
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the selected <see cref="cGeoCodeLocation">location</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Property SelectedItem() As cGeoCodeLocation
            Get
                Dim iIndex As Integer = -1
                Try
                    iIndex = Me.SelectedIndex
                Catch ex As Exception
                    ' Wow!
                End Try
                If iIndex = -1 Then Return Nothing
                Return DirectCast(Me.Items(iIndex), cGeoLocationItem).Item
            End Get
            Private Set(ByVal value As cGeoCodeLocation)
                ' NOP
            End Set
        End Property

        <Browsable(False)> _
        Public Shadows ReadOnly Property DropDownStyle() As ComboBoxStyle
            Get
                Return ComboBoxStyle.DropDown
            End Get
        End Property

#End Region ' Public interfaces

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Search for locations, and use this to populate the combo drop down.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub Search()

            ' Sanity checks
            If (Me.m_bSearching) Then Return
            If (Me.m_lookup Is Nothing) Then Return

            ' Start searching, setting this flag prevents search loops
            Me.m_bSearching = True

            ' Remember combo text and mouse pos. When combo items are added,
            ' which may happen below, the text in the combo will be erased.
            Dim strText As String = Me.Text
            Dim iStart As Integer = Me.SelectionStart
            Dim iLen As Integer = Me.SelectionLength

            ' Clear existing suggested locations
            Me.Items.Clear()

            Try
                ' Search!
                Dim locations As cGeoCodeLocation() = Me.m_lookup.FindLocations(strText)
                Dim location As cGeoCodeLocation = Nothing

                ' Has results?
                If (locations IsNot Nothing) Then
                    ' #Yes: populate suggestions drop down with locations.
                    For Each location In locations
                        Me.Items.Add(New cGeoLocationItem(location))
                    Next location
                End If

                ' Has suggested locations?
                If (Me.Items.Count > 0) Then
                    ' #Yes: Allocate enough room in dropdown to show all suggestions
                    Me.DropDownHeight = Me.ItemHeight * Me.Items.Count
                    ' Show suggestions
                    Me.DroppedDown = True
                    ' Restore combo text 
                    Me.Text = strText
                Else
                    ' #No: do not show suggestions 'cause there are none
                    Me.Items.Add(strText)
                    Me.DroppedDown = False
                End If

            Catch ex As Exception
                ' Something screwed up: swallow and keep moving
            End Try

            Try
                ' Restore cursor position
                Me.SelectionStart = iStart
                Me.SelectionLength = iLen
            Catch ex As Exception

            End Try

            ' Done searching
            Me.m_bSearching = False

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Mouse click handler, caught to prevent a new search from executing when 
        ''' a listbox item is being selected.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnClick(ByVal e As System.EventArgs)
            Me.m_bSelecting = True
            MyBase.OnClick(e)
            Me.m_bSelecting = False
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Combo text box change handler; used to trigger new searches.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
            ' Only search when not already in a search (which may affect the combo
            ' text) and not in the middle of a listbox selection process (which will
            ' also affect the combo text)
            If (Not Me.m_bSelecting) And (Not Me.m_bSearching) Then
                Me.Search()
            End If
            MyBase.OnTextChanged(e)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Listbox selection change handler; called to close the dropdown if it
        ''' happened to open up in response to a new search. Not sure if this needs 
        ''' to be called.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As System.EventArgs)
            MyBase.OnSelectedIndexChanged(e)
            Me.DroppedDown = False
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
