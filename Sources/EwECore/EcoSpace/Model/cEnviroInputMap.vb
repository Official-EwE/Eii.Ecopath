
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

    ''' <summary>
    ''' Max value of the map
    ''' </summary>
    ReadOnly Property Max() As Single

    ''' <summary>
    ''' Minimum value of the map
    ''' </summary>
    ReadOnly Property Min() As Single

    ''' <summary>
    ''' Mean value of the map
    ''' </summary>
    ReadOnly Property Mean() As Single

    ''' <summary>
    ''' Histogram of the map values
    ''' </summary>
    ''' <remarks>
    ''' Values in the Histogram will be normalized.
    ''' Re-computed on each call to Histogram.
    ''' </remarks>
    Function Histogram() As Drawing.PointF()


    ''' <summary>
    ''' Updates the map stats on the underlying data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Caluculates Min, Max and Mean</remarks>
    Function Update()


    ''' <summary>
    ''' Set the cMapResponseInteractionManager that this map uses
    ''' </summary>
    ''' <param name="theManager"></param>
    ''' <remarks></remarks>
    Sub setManager(ByVal theManager As cMapResponseInteractionManager)


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
    Private m_min As Single
    Private m_max As Single
    Private m_mean As Single
    Private m_binWidth As Single
    Private m_manager As cMapResponseInteractionManager

    ''' <inheritdocs cref="IEnviroInputMap.Init"/>
    Public Function Init(ByVal EnviroMediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean Implements IEnviroInputMap.Init
        Me.m_MedData = EnviroMediationData
        Me.m_spaceData = SpaceData

        ReDim Me.m_GrpToShape(Me.nGroups)

        Me.computeMinMax()

    End Function

    Private Sub computeMinMax()

        m_min = Single.MaxValue
        m_max = Single.MinValue

        For ir As Integer = 1 To Me.m_spaceData.InRow
            For ic As Integer = 1 To Me.m_spaceData.InCol
                Dim ob As Object = Me.m_map(ir, ic)
                m_min = Math.Min(CType(ob, Double), m_min)
                m_max = Math.Max(CType(ob, Double), m_max)
            Next
        Next

        Me.m_mean = (Me.m_min + m_max) * 0.5F

    End Sub


    ''' <summary>
    ''' Get/set the input map that the response function will use to look up its input value
    ''' </summary>
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
    Public Function ResponseFunction(ByVal igrp As Integer, ByVal iMapRow As Integer, ByVal iMapCol As Integer) As Single Implements IEnviroInputMap.ResponseFunction
        Dim iShp As Integer, MedX As Single ', ip As Long

        Try
            iShp = Me.ResponseIndexForGroup(igrp)
            'Response(shape) index of -9999 means there is no shape set for this Map/Group
            If iShp <= 0 Then
                'No shape has been set for this group
                'need to decide what the null response should be
                Return 0
            End If

            Dim obj As Object = Me.m_map(iMapRow, iMapCol)
            MedX = CType(obj, Single)

            Return Me.m_MedData.getEnviroResponse(iShp, MedX)

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

    ''' <summary>
    ''' Sets or gets the response(mediation) function index to use from the current cMediationDataStructures load during the Init(...)
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
                'Response index(shape index) of -9999 NULL_VALUE means there is not response set for this Map/Group
                Me.m_GrpToShape(GrpIndex) = ResponseShapeIndex

                'If the manager is nothing the response index was set during initialization
                'The manager is not initialized until an Ecospace scenarion is loaded
                If Not Me.m_manager Is Nothing Then
                    Me.m_manager.onChanged()
                End If


            End If
        End Set
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Histogram"/>
    Public Function Histogram() As Drawing.PointF() Implements IEnviroInputMap.Histogram
        Dim ipt As Integer, maxPts As Integer
        Dim nBins As Integer = 100
        Dim pts() As Drawing.PointF
        ReDim pts(nBins)
        Me.m_binWidth = Me.Max / nBins

        For ir As Integer = 1 To Me.m_spaceData.InRow
            For ic As Integer = 1 To Me.m_spaceData.InCol
                Dim cell As Single = CSng(CObj(Me.m_map(ir, ic)))
                ipt = Int(cell / m_binWidth)
                If ipt >= nBins Then ipt = nBins
                If ipt <= 0 Then ipt = 1
                pts(ipt).Y += 1
                maxPts = Math.Max(pts(ipt).Y, maxPts)
            Next
        Next

        'Normalize the histogram
        For i As Integer = 1 To nBins
            pts(i).X = m_binWidth * i
            pts(i).Y = pts(i).Y / maxPts
        Next

        Return pts

    End Function

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

    ''' <inheritdocs cref="IEnviroInputMap.Name"/>
    Public Property Name() As String Implements IEnviroInputMap.Name
        Get
            Return Me.m_name
        End Get
        Set(ByVal value As String)
            Me.m_name = value
        End Set
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Max"/>
    Public ReadOnly Property Max() As Single Implements IEnviroInputMap.Max
        Get
            Return Me.m_max
        End Get
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Mean"/>
    Public ReadOnly Property Mean() As Single Implements IEnviroInputMap.Mean
        Get
            Return Me.m_mean
        End Get
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Min"/>
    Public ReadOnly Property Min() As Single Implements IEnviroInputMap.Min
        Get
            Return Me.m_min
        End Get
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Update"/>
    Public Function Update() As Object Implements IEnviroInputMap.Update
        Dim bReturn As Boolean = False

        Try
            Me.computeMinMax()
            bReturn = True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Exception: " & ex.Message)
        End Try
        Return bReturn

    End Function

    ''' <inheritdocs cref="IEnviroInputMap.setManager"/>
    Public Sub setManager(ByVal theManager As cMapResponseInteractionManager) Implements IEnviroInputMap.setManager
        Me.m_manager = theManager
    End Sub

    Public Sub New()

    End Sub

    Public Sub New(ByVal theManager As cMapResponseInteractionManager, ByVal MapArray(,) As T, ByVal mapName As String)

        Me.setManager(theManager)
        Me.Name = mapName
        Me.m_map = MapArray

        'init to the data in the manager
        Me.Init(Me.m_manager.MediationData, Me.m_manager.SpaceData)

        Me.Update()

    End Sub

End Class
