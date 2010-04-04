#Region " Imports "

Option Strict On
Imports EwEUtils.Database

#End Region ' Imports

Namespace Database

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, provides information of an EwE5 model found in the 
    ''' source database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cEwE5ModelInfo

        Private m_strID As String = ""
        Private m_strName As String = ""
        Private m_strDescription As String = ""
        Private m_nScenarios As Integer = 0

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="strID">EwE5 modelName for this model.</param>
        ''' <param name="strName">EwE5 modelTitle for this model.</param>
        ''' <param name="strDescription">EwE5 model Remarks for this model.</param>
        ''' <param name="nScenarios">Number of EwE5 scenarios in this model.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal strID As String, ByVal strName As String, ByVal strDescription As String, ByVal nScenarios As Integer)
            Me.m_strID = strID
            Me.m_strName = strName
            Me.m_strDescription = strDescription
            Me.m_nScenarios = nScenarios
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Gets the internal model ID (EwE5 field modelName) for this model.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property ID() As String
            Get
                Return Me.m_strID
            End Get
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Gets the human readable model name (EwE5 field modelTitle) for this model.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property Name() As String
            Get
                Return Me.m_strName
            End Get
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Gets the model description (EwE5 field Remarks) for this model.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Gets the number of scenarios in this model.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public ReadOnly Property NumScenarios() As Integer
            Get
                Return Me.m_nScenarios
            End Get
        End Property

    End Class

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing an importer to convert an EwE5 document
    ''' into an EwE6 database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface IEwE5ModelImporter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connects the importer to an EwE5 source database. This database is
        ''' indicated as a file path, and is assumed to be an MS Access database.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Any database connection established via the Open method must be 
        ''' disconnected via the <see cref="Close">Close</see> method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function Open() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disconnects the importer from its EwE5 source document.
        ''' </summary>
        ''' <remarks>
        ''' Any database connection established via the <see cref="Open">Open</see>
        ''' method must be disconnected via the Close method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Sub Close()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the importer was connected to its source document 
        ''' via the <see cref="Open">Open</see> method.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsOpen() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Imports and converts a model in an EwE5 database into a new EwE6 database.
        ''' </summary>
        ''' <param name="strModelName">Name of the model in the EwE5 database to import.</param>
        ''' <param name="db">Database to import into.</param>
        ''' <param name="strLogfileName">File to log import progress to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function Import(ByVal strModelName As String, _
                        ByVal db As cEwEDatabase, _
                        ByVal strLogfileName As String) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a <see cref="cEwE5ModelInfo">descriptive list of models</see> 
        ''' that can be imported from an attached EwE5 document.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Function GetModels() As cEwE5ModelInfo()

    End Interface

End Namespace
