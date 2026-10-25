Namespace ValueChainMigrator.LegacyData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Class for querying if cOOPStorable instances are stored in the 
    ''' database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cOOPKey

#Region " Privates "

        ''' <summary>The ID for an object in the database</summary>
        Private ReadOnly m_iDBID As Integer = cOOPStorable.cDBID_INVALID
        ''' <summary>The runtime type that was used to write the
        ''' record in the database with this ID.</summary>
        Private ReadOnly m_tOriginating As Type = Nothing

#End Region ' Privates

#Region " Constructor "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Construct a new cOOPKey instance.
        ''' </summary>
        ''' <param name="t">The runtime type to generate the key for.</param>
        ''' <param name="iDBID">The unique ID that this key is generated for.</param>
        ''' ---------------------------------------------------------------
        Friend Sub New(t As Type, iDBID As Integer)

            ' Sanity checks
            Debug.Assert(t IsNot Nothing)
            Debug.Assert(iDBID <> cOOPStorable.cDBID_INVALID)

            Me.m_tOriginating = t
            Me.m_iDBID = iDBID

        End Sub

#End Region ' Constructor

#Region " Properties "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get the unique ID for the key.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property DBID() As Integer
            Get
                Return Me.m_iDBID
            End Get
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get the runtime type for the key.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property OriginatingType() As Type
            Get
                Return Me.m_tOriginating
            End Get
        End Property

#End Region ' Properties

    End Class

End Namespace
