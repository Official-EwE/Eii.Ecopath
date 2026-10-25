Imports System.ComponentModel
Imports System.Reflection

Namespace ValueChainMigrator.LegacyData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for implementing objects that can be stored in this type
    ''' of database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <Serializable()>
    Public MustInherit Class cOOPStorable

#Region " Privates "

        ''' <summary>Unique ID of an instance of cOOPStorable</summary>
        Private m_iDBID As Integer = cDBID_INVALID ' Key not assigned yet

        ''' <summary>Flag stating whether an cOOPStorabe instance is allowed to 
        ''' broadcast <see cref="OnChanged">OnChanged</see>events.</summary>
        Private m_bAllowEvents As Boolean = True

        ''' <summary>Flag preventing looped updates.</summary>
        Private m_bInUpdate As Boolean = False

#If DEBUG Then

        ''' <summary>Flag stating whether the object is deleted from the database.</summary>
        ''' <remarks>This flag is deliberately only available at debug time.</remarks>
        Private m_bDeleted As Boolean = False

#End If

#End Region ' Privates

#Region " Constructor "

        ''' <summary>Default ID for newly created cOOPStorable instances.</summary>
        Public Const cDBID_INVALID As Integer = 0

        Public Sub New()
            Me.m_iDBID = cDBID_INVALID
        End Sub

#End Region ' Constructor

#Region " Public properties "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Event to notify that instance unit has changed
        ''' </summary>
        ''' <param name="obj">The <see cref="cOOPStorable">instance</see>
        ''' that changed</param>
        ''' ---------------------------------------------------------------
        Public Event OnChanged(obj As cOOPStorable)

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' The unique ID of any object in the database. The database
        ''' manages this property exclusively although public read 
        ''' access is allowed.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(False)>
        Public Property DBID() As Integer
            Get
                Return Me.m_iDBID
            End Get
            Friend Set(value As Integer)
                Me.m_iDBID = value
            End Set
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this instance is allowed to send
        ''' <see cref="OnChanged">change events</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(False)>
        Public Property AllowEvents() As Boolean
            Get
                Return Me.m_bAllowEvents
            End Get
            Set(value As Boolean)
                Me.m_bAllowEvents = value
                If m_bAllowEvents Then Me.SetChanged()
            End Set
        End Property

#End Region ' Public properties

#Region " Public interfaces "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Copy the content of another instance of cOOPStorable.
        ''' </summary>
        ''' <param name="objSrc">The source ofject to copy content from.</param>
        ''' ---------------------------------------------------------------
        Public Overridable Sub CopyFrom(objSrc As cOOPStorable)

            Dim apiSrc As PropertyInfo() = Nothing
            Dim apiTgt As PropertyInfo() = Nothing
            Dim piSrc As PropertyInfo = Nothing
            Dim piTgt As PropertyInfo = Nothing

            If (objSrc Is Nothing) Then Return

            ' Copy all copyable properties
            apiSrc = objSrc.GetType().GetProperties()
            apiTgt = Me.GetType().GetProperties()
            For Each piSrc In apiSrc
                If String.Compare(piSrc.Name, "DBID") <> 0 Then
                    For Each piTgt In apiTgt
                        If piSrc.Name = piTgt.Name Then
                            Try
                                If piTgt.CanWrite() Then
                                    piTgt.SetValue(Me, piSrc.GetValue(objSrc, Nothing), Nothing)
                                End If
                            Catch ex As Exception
#If VERBOSE_LEVEL >= 2 Then
                                ' Ok, this did not work
                                Console.WriteLine("Woops: failed to copy prop {0} : {1}", piTgt.Name, ex.Message)
#End If
                            End Try
                        End If
                    Next
                End If
            Next
        End Sub

#End Region ' Public interfaces

#Region " Internals "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Helper method, states whether the content of an instance has
        ''' been modified externally.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(False)>
        Protected Sub SetChanged()
            If Me.m_bAllowEvents Then
                If (Me.m_bInUpdate = False) Then
                    ' Set deadlonk prevention lock
                    Me.m_bInUpdate = True
                    ' Raise event
                    RaiseEvent OnChanged(Me)
                    ' Release deadlonk prevention lock
                    Me.m_bInUpdate = False
                End If
            End If
        End Sub

#If DEBUG Then

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Helper method, states whether the object has been deleted from 
        ''' the database and thus cannot be saved.
        ''' </summary>
        ''' <remarks>
        ''' This method is only available at debug time.
        ''' </remarks>
        ''' ---------------------------------------------------------------
        <Browsable(False)>
        Friend Property Deleted() As Boolean
            Get
                Return Me.m_bDeleted
            End Get
            Set(value As Boolean)
                Me.m_bDeleted = value
            End Set
        End Property

#End If

#End Region ' Internals

    End Class

End Namespace