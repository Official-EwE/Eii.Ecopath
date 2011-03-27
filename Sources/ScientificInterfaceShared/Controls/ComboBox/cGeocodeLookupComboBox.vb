#Region " Imports "

Option Strict On
Imports SAUPUtil.Misc.GeoCode
Imports System.Threading

#End Region ' Imports

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Multi-threaded combo box that attempts to find geocoded locations based
    ''' on the combo box text.
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class cGeocodeLookupComboBox
        Inherits ComboBox

#Region " Private variables "

        Private m_lookup As cGeoCodeLookup = Nothing
        Private m_searchThread As Thread = Nothing
        Private m_bIsSearching As Boolean = False

#End Region ' Private variables

#Region " Construction and destruction "

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub New()
            Me.m_lookup = New cGoogleMapsLookup()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            ' Abort any search
            Me.Search("")
            Me.m_lookup = Nothing
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction and destruction

#Region " Public interfacs "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the geocode location selected in the control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property SelectedLocation() As cGeoCodeLocation
            Get
                Return DirectCast(Me.SelectedItem, cGeoCodeLocation)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>Event to notify whether a search is in progress.</summary>
        ''' <param name="sender"></param>
        ''' <param name="bSearching"></param>
        ''' -----------------------------------------------------------------------
        Public Event OnSeaching(ByVal sender As cGeocodeLookupComboBox, ByVal bSearching As Boolean)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a search is in progress.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property IsSearching() As Boolean
            Get
                Return Me.m_bIsSearching
            End Get
        End Property

        Public Property LookupEngine() As cGeoCodeLookup
            Get
                Return Me.m_lookup
            End Get
            Set(ByVal value As cGeoCodeLookup)
                Me.Search("")
                Me.m_lookup = value
            End Set
        End Property

#End Region ' Public interfacs

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Locate a string in the list of available found locations.
        ''' </summary>
        ''' <param name="strText">The text to find.</param>
        ''' <returns>True if successful.</returns>
        ''' -----------------------------------------------------------------------
        Private Function Locate(ByVal strText As String) As Boolean
            Dim iIndex As Integer = Me.FindString(strText)

            If iIndex <> -1 Then
                Me.SelectedText = ""
                Me.SelectedIndex = iIndex
                Me.SelectionStart = strText.Length
                Me.SelectionLength = Me.Text.Length
                Return True
            Else
                Me.Text = strText
                Me.SelectionStart = Me.Text.Length
                Return False
            End If

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Keypress handler to initiate a geolocation search.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)

            MyBase.OnKeyPress(e)

            If Not Char.IsControl(e.KeyChar) Then
                If Me.SelectionLength = 0 Then
                    Me.Search(Me.Text)
                Else
                    Me.Search(Me.Text.Substring(0, Me.SelectionStart))
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Abort an active search, and initiate a new geolocation search if a 
        ''' search criterion is provided.
        ''' </summary>
        ''' <param name="strFindStr">Text to find geolocatios for.</param>
        ''' -----------------------------------------------------------------------
        Private Sub Search(ByVal strFindStr As String)

            ' Let's be easy on the designer
            If Me.DesignMode Then Return

            If Me.m_searchThread IsNot Nothing Then
                Me.m_searchThread.Abort()
                Me.FireSearchingEvent(False)
            End If

            If String.IsNullOrEmpty(strFindStr) Or (Me.m_lookup Is Nothing) Then Return

            Me.FireSearchingEvent(True)
            Me.m_searchThread = New Thread(AddressOf SearchThread)
            Me.m_searchThread.Start(strFindStr)

            ' Try to match text with present items
            Me.Locate(strFindStr)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Geocode lookup thread procedure.
        ''' </summary>
        ''' <param name="strFindStr"></param>
        ''' -----------------------------------------------------------------------
        Private Sub SearchThread(ByVal strFindStr As Object)

            Debug.Assert(Me.m_lookup IsNot Nothing)

            Dim aLocations As cGeoCodeLocation() = Me.m_lookup.FindPlaces(CStr(strFindStr))
            Me.BeginInvoke(New OnSearchResultsDelegate(AddressOf OnSearchResults), New Object() {aLocations})
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Delegate for passing geocode search results back to control thread.
        ''' </summary>
        ''' <param name="aLocations"></param>
        ''' -----------------------------------------------------------------------
        Private Delegate Sub OnSearchResultsDelegate(ByVal aLocations As cGeoCodeLocation())

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Apply geocode search results to combo box
        ''' </summary>
        ''' <param name="aLocations"></param>
        ''' -----------------------------------------------------------------------
        Private Sub OnSearchResults(ByVal aLocations As cGeoCodeLocation())

            ' Grab text before modifying dropdown items
            Dim strText As String = Me.Text

            ' Update items in critical section
            Monitor.Enter(Me)
            Me.Items.Clear()
            For Each Loc As cGeoCodeLocation In aLocations
                Me.Items.Add(Loc)
                Me.DroppedDown = True
            Next
            Monitor.Exit(Me)

            ' Restore text
            Me.Locate(strText)

            ' Notify world
            Me.FireSearchingEvent(False)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Notify the world that a search is in progress.
        ''' </summary>
        ''' <param name="bSearching">Flag inidicating if a search is active.</param>
        ''' -----------------------------------------------------------------------
        Private Sub FireSearchingEvent(ByVal bSearching As Boolean)
            Try
                RaiseEvent OnSeaching(Me, bSearching)
            Catch ex As Exception
                ' NOP
            End Try
        End Sub

#End Region ' Internals

    End Class

End Namespace
