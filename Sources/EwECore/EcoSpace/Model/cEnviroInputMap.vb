
''' <summary>
''' Interface for defining Ecospace Environmental Input maps
''' </summary>
''' <remarks></remarks>
Public Interface IEnviroInputMap
    ''' <summary>
    ''' Name of the underlying Map
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Name() As String

    ''' <summary>
    ''' Return the value of the map as a function of the applied Response Function
    ''' </summary>
    ''' <param name="igrp">Index of the Group that this Response is for</param>
    ''' <param name="iRow">Row of the map</param>
    ''' <param name="iCol">Column of the map</param>
    Function ResponseFunction(ByVal igrp As Integer, ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Initialize the map with the cMediationDataStructures containing all the available response functions and cEcospaceDataStructures
    ''' </summary>
    ''' <param name="MediationData">cMediationDataStructures that contains the Response Function (mediation functions) that can be used by this Map</param>
    ''' <param name="SpaceData"></param>
    Function Init(ByVal MediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean

    ''' <summary>
    ''' Get or Set the index of the Response function applied to a Group
    ''' </summary>
    ''' <param name="GroupIndex">Index of the Group that the response function is applied to</param>
    ''' <value></value>
    ''' <returns>Index of a response function.</returns>
    ''' <remarks>
    ''' <code>
    ''' dim ResponseIndex as integer
    ''' dim GroupIndex as integer
    ''' GroupIndex = 1
    ''' 'Set the Response function index for GroupIndex
    '''  IEnviroInputMap.ResponseIndexForGroup(GroupIndex) = 2
    ''' 'Get the Response functon index for GroupIndex
    ''' ResponseIndex = IEnviroInputMap.ResponseIndexForGroup(GroupIndex) 
    ''' </code>
    ''' </remarks>
    Property ResponseIndexForGroup(ByVal GroupIndex As Integer) As Integer
End Interface



''' <summary>
''' Joins an input map(row,col) with a list(by group) of Environmental Response functions (mediation functions).
''' </summary>
''' <typeparam name="T">Type of map</typeparam>
''' <remarks>
''' Set the Map to the input map then tell it which response functions to use for which groups setShapeForGroup(igroup) = iResponseFunction
''' </remarks>
Public Class cEnviroInputMap(Of T)
    Implements IEnviroInputMap

    Private m_map(,) As T
    Private m_GrpToShape() As Integer
    Private m_MedData As cMediationDataStructures
    Private m_spaceData As cEcospaceDataStructures
    Private m_name As String


    Public Function Init(ByVal EnviroMediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean Implements IEnviroInputMap.Init
        Me.m_MedData = EnviroMediationData
        Me.m_spaceData = SpaceData

        ReDim Me.m_GrpToShape(Me.nGroups)

    End Function


    ''' <summary>
    ''' Set the input map that the response function will use to look up it's input value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Map() As T(,)
        Get
            Return Me.m_map
        End Get
        Set(ByVal value As T(,))
            Me.m_map = value
        End Set
    End Property


    ''' <summary>
    ''' Return a value for a cell in the input map base on the the response function for a group.
    ''' </summary>
    ''' <param name="igrp">Group index for the response function</param>
    ''' <param name="iMapRow">Row of the input map</param>
    ''' <param name="iMapCol">Col of the input map</param>
    ''' <returns>Y = F(x)</returns>
    ''' <remarks></remarks>
    Public Function ResponseFunction(ByVal igrp As Integer, ByVal iMapRow As Integer, ByVal iMapCol As Integer) As Single Implements IEnviroInputMap.ResponseFunction
        Dim iShp As Integer, MedX As Single ', ip As Long

        Try
            iShp = Me.ResponseIndexForGroup(igrp)
            'at this time I'm not sure if this is a error or not!
            Debug.Assert(iShp <> 0, Me.ToString & ".ResponseFunction() no function has been set for this group!")
            'no shape has been set for this group
            If iShp = 0 Then
                'need to decide what the null response should be
                Return 0
            End If

            MedX = 0.0000000001
            Dim obj As Object = Me.m_map(iMapRow, iMapCol)
            MedX = CType(obj, Single)

            Return Me.m_MedData.getMedValue(iShp, MedX)

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

    ''' <summary>
    ''' Sets or gets the response(mediation) function ndex to use from the current cMediationDataStructures load during the Init(...)
    ''' </summary>
    ''' <param name="GrpIndex">Group index for the response function.</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The Index of the ResponseFunction must exist in the underlying mediation data.</remarks>
    Public Property ResponseIndexForGroup(ByVal GrpIndex As Integer) As Integer Implements IEnviroInputMap.ResponseIndexForGroup
        Get
            Return Me.m_GrpToShape(GrpIndex)
        End Get

        Set(ByVal ResponseShapeIndex As Integer)
            If ResponseShapeIndex <= Me.m_MedData.MediationShapes And GrpIndex <= Me.nGroups Then
                Me.m_GrpToShape(GrpIndex) = ResponseShapeIndex
            End If
        End Set
    End Property


    Public ReadOnly Property nGroups() As Integer
        Get
            Return Me.m_spaceData.NGroups
        End Get
    End Property

    Public ReadOnly Property nFleets() As Integer
        Get
            Return Me.m_spaceData.nFleets
        End Get
    End Property


    Public Property Name() As String Implements IEnviroInputMap.Name
        Get
            Return Me.m_name
        End Get
        Set(ByVal value As String)
            Me.m_name = value
        End Set
    End Property

End Class
