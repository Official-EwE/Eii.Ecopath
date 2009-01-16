
Imports EwEUtils.Core

''' <summary>
''' Results of the current Ecospace time step
''' </summary>
''' <remarks></remarks>
Public Class cEcospaceTimestep
    Implements ICoreInterface

#Region "Private data"

    Private m_dbid As Integer
    Private m_name As String

    Private m_iTime As Integer
    Private m_ts As Single
    Private m_ConMax() As Single
    Private m_biomap(,,) As Single
    Private m_effort(,,) As Single
    Private m_contaminants(,,) As Single

    Private m_nRows As Integer
    Private m_nCols As Integer


    Private m_biomass() As Single 'biomass by group
    Private m_relativebiomass() As Single 'biomass relative to start biomass by group


    Private m_biomassByRegion(,) As Single 'biomass by group region

#End Region

#Region "Constructor & Initialization"

    Public Sub New(ByVal nGrps As Integer, ByVal nRegions As Integer)

        m_dbid = cCore.NULL_VALUE
        m_name = eDataTypes.EcospaceTimestepResults
        Try
            ReDim m_biomass(nGrps)
            ReDim m_relativebiomass(nGrps)
            ReDim m_ConMax(nGrps)
            ReDim m_biomassByRegion(nGrps, nRegions)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".New() Error: " & ex.Message)
        End Try

    End Sub


    Friend Sub setMaps(ByRef BiomassMap(,,) As Single, ByRef EffortMap(,,) As Single, ByRef ContaminantMap(,,) As Single, ByVal nMapRows As Integer, ByVal nMapCols As Integer)
        m_biomap = BiomassMap
        m_effort = EffortMap
        m_contaminants = ContaminantMap

        m_nRows = nMapRows
        m_nCols = nMapCols
        ' Array.Copy(BiomassMap, m_biomap, BiomassMap.Length)
    End Sub

#End Region

#Region "Public Properties"

    Public Property iTimeStep() As Integer
        Get
            Return m_iTime
        End Get
        Set(ByVal value As Integer)
            m_iTime = value
        End Set
    End Property

    Public Property TimeStepinYears() As Single
        Get
            Return m_ts
        End Get
        Set(ByVal value As Single)
            m_ts = value
        End Set
    End Property

    ''' <summary>
    ''' Biomass map dimensioned by Row, Col, Group
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>BiomassMap(row,col,group) and FishingEffortMap(fleet,row,col) are both map variables but they are not indexed the same</remarks>
    Public ReadOnly Property BiomassMap() As Single(,,)
        Get
            Return m_biomap
        End Get
    End Property

    ''' <summary>
    ''' Fishing Effort dimensioned by Fleet, Row, Col
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>BiomassMap(row,col,group) and FishingEffortMap(fleet,row,col) are both map variables but they are not indexed the same</remarks>
    Public ReadOnly Property FishingEffortMap() As Single(,,)
        Get
            Return m_effort
        End Get
    End Property


    ''' <summary>
    ''' Contaminant concentrations dimensioned by Row, Col and Group
    ''' </summary>
    ''' <value></value>
    ''' <returns>Matrix of contaminant concentrations at this timestep</returns>
    Public ReadOnly Property ContaminantMap() As Single(,,)
        Get
            Return m_contaminants
        End Get
    End Property

    Public Property Biomass(ByVal iGroup As Single) As Single

        Get
            Try
                Return m_biomass(iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                m_biomass(iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

    Public Property ConcMax(ByVal iGroup As Single) As Single

        Get
            Try
                Return Me.m_ConMax(iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                Me.m_ConMax(iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

    Public Property RelativeBiomass(ByVal iGroup As Single) As Single

        Get
            Try
                Return m_relativebiomass(iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                m_relativebiomass(iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

    Public ReadOnly Property inRows() As Integer
        Get
            Return m_nRows
        End Get
    End Property

    Public ReadOnly Property inCols() As Integer
        Get
            Return m_nCols
        End Get
    End Property


    Public Property BiomassByRegion(ByVal iGroup As Integer, ByVal iRegion As Integer) As Single

        Get
            Try
                Return m_biomassByRegion(iGroup, iRegion)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                m_biomassByRegion(iGroup, iRegion) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property


#End Region

#Region " ICoreInterface implementation "

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return eDataTypes.EcospaceTimestepResults
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.EcoSpace
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return m_dbid
        End Get
        Set(ByVal value As Integer)
            m_dbid = value
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return cValueID.GenerateAbstract(Me.DataType, Me.DBID)
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return m_name
        End Get
        Set(ByVal value As String)
            m_name = value
        End Set
    End Property

#End Region

End Class


