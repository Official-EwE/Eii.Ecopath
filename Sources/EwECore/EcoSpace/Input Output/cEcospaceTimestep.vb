
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
    'Private m_biomap(,,) As Single
    'Private m_effort(,,) As Single
    'Private m_contaminants(,,) As Single

    'Private m_nRows As Integer
    'Private m_nCols As Integer

    Private m_biomass() As Single 'biomass by group
    Private m_relativebiomass() As Single 'biomass relative to start biomass by group
    Private m_biomassByRegion(,) As Single 'biomass by group region

    Private m_IBMMap(,,) As Boolean

    Private m_spaceData As cEcospaceDataStructures
    Private m_simData As cEcosimDatastructures
    Private m_stanzaData As cStanzaDatastructures

#End Region

#Region "Constructor & Initialization"

    Public Sub New(ByVal EcoSimData As cEcosimDatastructures, ByVal EcoSpaceData As cEcospaceDataStructures, ByVal StanzaData As cStanzaDatastructures)

        m_dbid = cCore.NULL_VALUE
        m_name = eDataTypes.EcospaceTimestepResults.ToString
        Me.m_simData = EcoSimData
        Me.m_spaceData = EcoSpaceData
        Me.m_stanzaData = StanzaData

        Debug.Assert(Me.m_simData IsNot Nothing, Me.ToString & ".New() Ecosim data cannot be null!")
        Debug.Assert(Me.m_spaceData IsNot Nothing, Me.ToString & ".New() Ecospace data cannot be null!")

        Try
            ReDim m_biomass(Me.m_simData.nGroups)
            ReDim m_relativebiomass(Me.m_simData.nGroups)
            ReDim m_ConMax(Me.m_simData.nGroups)
            ReDim m_biomassByRegion(Me.m_simData.nGroups, Me.m_spaceData.NoRegions)
            ReDim m_IBMMap(Me.m_spaceData.Inrow, Me.m_spaceData.InCol, Me.m_simData.nGroups)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".New() Error: " & ex.Message)
        End Try

    End Sub


    Friend Sub ComputeIBMMap()

        Try

            'clear out the data from the last timestep
            'this will set all the cells to false
            Array.Clear(m_IBMMap, 0, m_IBMMap.Length)

            Dim irow As Integer, jcol As Integer
            Dim iGrp As Integer
            For isp As Integer = 1 To Me.m_stanzaData.Nsplit
                For ist As Integer = 1 To Me.m_stanzaData.Nstanza(isp)
                    For iage As Integer = Me.m_stanzaData.Age1(isp, ist) To Me.m_stanzaData.Age2(isp, ist)
                        For ipkt As Integer = 1 To Me.m_stanzaData.Npackets
                            irow = Int(Me.m_stanzaData.iPacket(isp, iage, ipkt))
                            jcol = Int(Me.m_stanzaData.jPacket(isp, iage, ipkt))
                            iGrp = Me.m_stanzaData.EcopathCode(isp, ist)
                            m_IBMMap(irow, jcol, iGrp) = True
                        Next ipkt
                    Next iage
                Next ist
            Next isp

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ComputeIBMMap() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try

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
            Return Me.m_spaceData.Bcell
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
            Return Me.m_spaceData.EffortSpace
        End Get
    End Property


    ''' <summary>
    ''' Contaminant concentrations dimensioned by Row, Col and Group
    ''' </summary>
    ''' <value></value>
    ''' <returns>Matrix of contaminant concentrations at this timestep</returns>
    Public ReadOnly Property ContaminantMap() As Single(,,)
        Get
            Return Me.m_spaceData.Ccell
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
            Return Me.m_spaceData.Inrow
        End Get
    End Property

    Public ReadOnly Property inCols() As Integer
        Get
            Return Me.m_spaceData.InCol
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

    ''' <summary>
    ''' Number of Prey/Pred linkages
    ''' </summary>
    ''' <remarks>Number of links is set in cEcoSimModel.CalcEatenOfBy()</remarks>
    Public ReadOnly Property nPreyPredLinks() As Integer
        Get
            Return Me.m_simData.inlinks
        End Get
    End Property

    ''' <summary>
    ''' Gets the group index for the Prey of this Prey/Pred link
    ''' </summary>
    ''' <param name="iPreyPredIndex">Index of the Prey/Pred link (1 to nPreyPredLinks)</param>
    ''' <remarks> </remarks>
    Public ReadOnly Property iPreyIndex(ByVal iPreyPredIndex As Integer) As Integer

        Get
            Debug.Assert(iPreyPredIndex <= Me.m_simData.inlinks, Me.ToString & ".iPreyIndex(iPreyPredIndex) iPreyPredIndex out of bounds!")
            Try
                Return Me.m_simData.ilink(iPreyPredIndex)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

    End Property

    ''' <summary>
    ''' Gets the group index for the Predator of this Prey/Pred link
    ''' </summary>
    ''' <param name="iPreyPredIndex">Index of the Prey/Pred link (1 to nPreyPredLinks)</param>
    ''' <remarks> </remarks>
    Public ReadOnly Property iPredIndex(ByVal iPreyPredIndex As Integer) As Integer

        Get
            Debug.Assert(iPreyPredIndex <= Me.m_simData.inlinks, Me.ToString & ".iPredIndex(iPreyPredIndex) iPreyPredIndex out of bounds!")
            Try
                Return Me.m_simData.jlink(iPreyPredIndex)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Get

    End Property

    ''' <summary>
    ''' Mortality rate map due to predation by Row, Col, Prey/Pred linkage <see cref="nPreyPredLinks">nPreyPredLinks</see>
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>MortPredRate = [prey biomass eaten] / [prey biomass]</remarks>
    Public ReadOnly Property MortPredRate() As Single(,,)
        Get
            Return Me.m_spaceData.MPred
        End Get
    End Property

    ''' <summary>
    ''' Detritus by group
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Total biomass a group contributes to detritus</remarks>
    Public ReadOnly Property GroupDetritus() As Single(,,)
        Get
            Return Me.m_spaceData.GroupDetritus
        End Get
    End Property

    ''' <summary>
    ''' Map of IMB packets by Row, Col, Group
    ''' </summary>
    ''' <value>True if cell contains IBM packet(s). False otherwise</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IMBLocationsMap() As Boolean(,,)
        Get
            Return Me.m_IBMMap
        End Get
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


