Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#Region " Forcing Shape "

''' -----------------------------------------------------------------------
''' <summary>
''' Provides access to Forcing and EggProduction shapes, and a base class 
''' for Mediation functions.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cForcingFunction
    Inherits cShapeData

#Region " Protected data "

    Private m_bAllowValidation As Boolean = True

    Protected m_iIndex As Integer = 0
    Protected m_data As cEcosimDatastructures
    Protected m_manager As cBaseShapeManager

    Protected m_ID As Integer
    '   Protected m_Type As eDataTypes
    Protected m_nYears As Integer

    ' Parameters use to build a Curve
    'these are the variables is cEcoSimDatastructures.ShapeParameters
    Protected m_YZero As Single
    Protected m_YBase As Single
    Protected m_YEnd As Single
    Protected m_Steep As Single
    Protected m_ZScale As Single

    Protected m_ShapeFunctionType As eShapeFunctionType
    Protected m_ForcingApplicationType As eForcingApplicationTypes

    'this flag is used to stop updating during initialization
    'it is more of a safe guard 
    Protected m_bInInit As Boolean


    Protected m_bLockUpdates As Boolean

#End Region ' Protected data

#Region " Public fields/properties "

    'Public Property Title() As String
    '    Get
    '        Return Me.m_Xdata.Name
    '    End Get
    '    Set(ByVal strTitle As String)
    '        Me.m_Xdata.Name = strTitle
    '        Update()
    '    End Set
    'End Property

    Public Property YZero() As Single
        Get
            Return m_YZero
        End Get
        Set(ByVal value As Single)
            m_YZero = value
            Update()
        End Set
    End Property


    Public Property YBase() As Single
        Get
            Return m_YBase
        End Get
        Set(ByVal value As Single)
            m_YBase = value
            Update()
        End Set
    End Property

    Public Property YEnd() As Single
        Get
            Return m_YEnd
        End Get
        Set(ByVal value As Single)
            m_YEnd = value
            Update()
        End Set
    End Property

    Public Property Steep() As Single
        Get
            Return m_Steep
        End Get
        Set(ByVal value As Single)
            m_Steep = value
            Update()
        End Set
    End Property

    Public Property ShapeFunctionType() As eShapeFunctionType
        Get
            Return m_ShapeFunctionType
        End Get
        Set(ByVal value As eShapeFunctionType)
            m_ShapeFunctionType = value
            Update()
        End Set
    End Property

    Public Property ForcingApplicationType() As eForcingApplicationTypes
        Get
            Return Me.m_ForcingApplicationType
        End Get
        Set(ByVal value As eForcingApplicationTypes)
            Me.m_ForcingApplicationType = value
            Me.Update()
        End Set
    End Property

    Public ReadOnly Property ZScale() As Single
        Get
            Return m_ZScale
        End Get
    End Property

    ''' <summary>
    ''' Index of the shape in the list managers list of shape
    ''' </summary>
    ''' <remarks>This is a zero based index set when the shape is added to the manager (Construction of the shape) </remarks>
    Public Property ID() As Integer
        Get
            Return m_ID
        End Get
        Friend Set(ByVal value As Integer)
            m_ID = value
            '  Update()
        End Set
    End Property

    Public Property NYears() As Integer
        Get
            Return m_nYears
        End Get
        Friend Set(ByVal value As Integer)
            m_nYears = value
            Update()
        End Set
    End Property

    'Public Property AllowValidation() As Boolean
    '    Get
    '        Return Me.m_bAllowValidation
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Me.m_bAllowValidation = False
    '    End Set
    'End Property

#End Region ' Public fields/properties

#Region " Constructors and Initialization "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new Forcing Function Shape from the underlying EcoSim Data.
    ''' </summary>
    ''' <param name="esData"><see cref="cEcosimDatastructures">Ecosim data structure</see>
    ''' to create the forcing function from.</param>
    ''' <param name="Manager"></param>
    ''' <param name="DBID"></param>
    ''' <param name="DataType"></param>
    ''' <remarks>
    ''' This is used by the Manager to create forcing function from the 
    ''' underlying EcoSim data.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByRef esData As cEcosimDatastructures, ByRef Manager As cBaseShapeManager, ByVal DBID As Integer, ByVal DataType As eDataTypes)

        MyBase.New(esData.ForcePoints)

        m_bInInit = True
        m_data = esData

        m_datatype = DataType
        m_coreComponent = CoreComponent
        m_dbID = DBID

        m_manager = Manager 'keep a reference to the manager for this shape

        Load()

        m_bInInit = False

    End Sub

    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures for this shapes Database ID 
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overridable Function Load() As Boolean

        m_iEcoSimIndex = Array.IndexOf(m_data.ForcingDBIDs, m_dbID)
        Debug.Assert(m_iEcoSimIndex <> -1, "Failed to find index for Shape.")

        If m_iEcoSimIndex = -1 Then Return False
        m_bInInit = True
        Me.LockUpdates()

        'copy the data from zscale into an array that will be used to create a forcing data object
        Me.Init(m_data.ForcePoints)
        For ipt As Integer = 1 To m_data.ForcePoints
            Me.ShapeData(ipt) = m_data.zscale(ipt, m_iEcoSimIndex)
        Next ipt

        Me.Name = m_data.ForcingTitles(m_iEcoSimIndex)
        Me.m_ForcingApplicationType = m_data.ForcingApplicationType(m_iEcoSimIndex)

        m_nYears = m_data.NumYears

        'shape parameters
        m_ShapeFunctionType = m_data.ForcingShapeParams(m_iEcoSimIndex).ShapeFunctionType
        m_Steep = m_data.ForcingShapeParams(m_iEcoSimIndex).Steep
        m_YBase = m_data.ForcingShapeParams(m_iEcoSimIndex).YBase
        m_YEnd = m_data.ForcingShapeParams(m_iEcoSimIndex).YEnd
        m_YZero = m_data.ForcingShapeParams(m_iEcoSimIndex).YZero
        m_ZScale = m_data.ForcingShapeParams(m_iEcoSimIndex).ZScale

        Me.isSeasonal = m_data.isSeasonal(m_iEcoSimIndex)

        Me.UnlockUpdates()
        m_bInInit = False

        Return True

    End Function

#End Region ' Constructors and Initialization

#Region " Updating "

    ''' <summary>
    ''' Update the already existing underlying EcoSim data structures (m_data)
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>This gets called by the cForcingData when it has been edited to update the existing EcoSim data</remarks>
    Public Overrides Function Update() As Boolean

        Try

            Debug.Assert(m_data IsNot Nothing, Me.ToString & ".Update() underlying ecosim data has not been set.")

            'do not update during initialization
            If m_bInInit Then
                'update will be call be the Forcing Data object (m_xData) when it is populated it has no way of knowing who is changing its value
                'so it has to call update on its parent
                Return False
            End If


            'turn the Database ID into an Array index using the Ecosim Data structures database ID this value should be good
            m_iEcoSimIndex = Array.IndexOf(m_data.ForcingDBIDs, m_dbID)
            Debug.Assert(m_iEcoSimIndex >= 0, Me.ToString & ".Update() Failed to find index for Database ID " & m_dbID)
            If (m_iEcoSimIndex = cCore.NULL_VALUE) Or (m_iEcoSimIndex > m_data.ForcingShapes) Then
                cLog.Write(Me.ToString & ".Update() index out of bounds. Data not updated.")
                Return False
            End If

            'make sure the shape data is the same size as the EcoSim Shape data
            'this is a double check as the data size was checked when the forcing function was created by the Shape Manager
            'however it could have been changed by an interface at a later date
            Me.ResizeData(m_data.ForcePoints)

            'populate the raw shape data
            For ipt As Integer = 1 To Me.XMax
                m_data.zscale(ipt, m_iEcoSimIndex) = Me.ShapeData(ipt)
            Next ipt
            m_data.ForcingTitles(m_iEcoSimIndex) = Me.Name

            m_data.ForcingShapeType(m_iEcoSimIndex) = m_datatype
            m_data.ForcingApplicationType(m_iEcoSimIndex) = Me.m_ForcingApplicationType

            'shape parameters
            m_data.ForcingShapeParams(m_iEcoSimIndex).ShapeFunctionType = m_ShapeFunctionType
            m_data.ForcingShapeParams(m_iEcoSimIndex).Steep = m_Steep
            m_data.ForcingShapeParams(m_iEcoSimIndex).YBase = m_YBase
            m_data.ForcingShapeParams(m_iEcoSimIndex).YEnd = m_YEnd
            m_data.ForcingShapeParams(m_iEcoSimIndex).YZero = m_YZero
            m_data.ForcingShapeParams(m_iEcoSimIndex).ZScale = m_ZScale

            m_data.isSeasonal(m_iEcoSimIndex) = Me.IsSeasonal()

            ShapeChanged()
            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".update() Error: " & ex.Message)
            cLog.Write(Me.ToString & ".update() Error: " & ex.Message)
            Return False

        End Try

    End Function

    ''' <summary>
    ''' Tell the manager that a shape has changed
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub ShapeChanged()

        'tell the manager that a shape has changed it's data
        If Not Me.IsLockedUpdates Then Me.m_manager.ShapeChanged(Me)

    End Sub

#End Region ' Updating

End Class ' cForcingFunction

#End Region ' Forcing Shape 

#Region " Mediation Shape "

#Region " cMediatingGroup "

''' <summary>
''' Group and Weight of a Group that make up a Mediating Group for a Mediation function. There can be more then one cMediatingGroup for a Mediation Function
''' </summary>
''' <remarks>This is the Group(s) that provide the Biomass for the X axis of a mediation function</remarks>
Public Class cMediatingGroup

    Public iGroupIndex As Integer
    Public Weight As Single

    ''' <summary>
    ''' Build a new Mediation Group
    ''' </summary>
    ''' <param name="iGroup">Index to the EcoPath/EcoSIm group this is the iGroup</param>
    ''' <param name="theWeight">Weight that is applied to this group 0-1</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal iGroup As Integer, ByVal theWeight As Single)

        Debug.Assert(iGroup > 0 And iGroup <= cCore.GetInstance.nGroups, Me.ToString & ".iGroup out of bounds.")

        iGroupIndex = iGroup
        'weight does not have to one or zero it can be any value it 
        Weight = theWeight

    End Sub

    Public Sub New()
        iGroupIndex = 0
        Weight = 0
    End Sub

    Public Overrides Function ToString() As String
        Return "Group Index=" & iGroupIndex.ToString & " Weight=" & Weight.ToString
    End Function

End Class

#End Region ' cMediatingGroup

#Region " cMediationFleet "

''' <summary>
''' Fleet and Weight of a Fleet that make up a Mediating Fleet for a Mediation function. There 
''' can be more then one cMediatingFleet for a Mediation Function
''' </summary>
''' <remarks>This defines the Fleet(s) that provide the Biomass for the X axis of a mediation function.</remarks>
Public Class cMediatingFleet

    Public iFleetIndex As Integer
    Public Weight As Single

    ''' <summary>
    ''' Build a new Mediation Fleet
    ''' </summary>
    ''' <param name="iFleet">Index to the EcoPath/EcoSim fleet.</param>
    ''' <param name="theWeight">Weight that is applied to this fleet [0-1]</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal iFleet As Integer, ByVal theWeight As Single)

        Debug.Assert(iFleet > 0 And iFleet <= cCore.GetInstance.nFleets, Me.ToString & ".iGroup out of bounds.")

        iFleetIndex = iFleet
        'weight does not have to one or zero it can be any value it 
        Weight = theWeight

    End Sub

    Public Sub New()
        iFleetIndex = 0
        Weight = 0
    End Sub

    Public Overrides Function ToString() As String
        Return "Fleet Index=" & iFleetIndex.ToString & " Weight=" & Weight.ToString
    End Function

End Class

#End Region ' cMediationFleet

#Region " cMediationFunction "

''' <summary>
''' A Mediation Function 'Is A' type of Forcing Function and Inherits its base functionality from cForcingFunction 
''' and extents it to include the Groups that make up the Mediating Biomass.
''' </summary>
''' <remarks></remarks>
Public Class cMediationFunction
    Inherits cForcingFunction


    Private m_iMedXBase As Integer

    Private m_groups As New List(Of cMediatingGroup)
    Private m_fleets As New List(Of cMediatingFleet)

#Region " Constructors "


    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef Manager As cBaseShapeManager, ByVal DBID As Integer, ByVal DataType As eDataTypes)
        'mediation data arrays from EcoSim
        'Public MedWeights(nGroups + nGear, MediationShapes) As Single 'defines biomass weights for med X
        'Public NMedXused() As Integer 'number of biomasses (mediation weights) in an iMediation
        'Public IMedUsed(,) As Integer 'groups used in med function X IMedUsed(nGroups + nGear, MediationShapes)
        'Public MedXbase() As Single 'ecopath base value of med function X
        'Public MedYbase() As Single 'value of med function at ecopath base X
        'Public MedIsUsed() As Boolean 'true if med function iMediation is used

        MyBase.New(EcoSimData, Manager, DBID, DataType)

        Try

            m_datatype = eDataTypes.Mediation
            m_coreComponent = eCoreComponentType.EcoSim

            m_bInInit = True
            m_data = EcoSimData
            m_dbID = DBID
            m_iEcoSimIndex = Array.IndexOf(m_data.MediationDBIDs, m_dbID)
            Dim iShape As Integer = m_iEcoSimIndex 'just for clarity

            m_manager = Manager 'keep a reference to the manager for this shape

            Dim grp As cMediatingGroup = Nothing
            Dim flt As cMediatingFleet = Nothing

            ' Groups: if this mediation shape has any weights applied to it then load the weight and group into an object
            For iGrp As Integer = 1 To m_data.nGroups
                If m_data.MedWeights(iGrp, iShape) > 0 Then
                    grp = New cMediatingGroup
                    grp.iGroupIndex = iGrp ' m_data.IMedUsed(iGrp, iShape)
                    grp.Weight = m_data.MedWeights(iGrp, iShape)
                    m_groups.Add(grp)
                End If
            Next

            ' Fleets: if this mediation shape has any weights applied to it then load the weight and fleet into an object
            For iFlt As Integer = 1 To m_data.nGear
                If m_data.MedWeights(m_data.nGroups + iFlt, iShape) > 0 Then
                    flt = New cMediatingFleet
                    flt.iFleetIndex = iFlt ' m_data.IMedUsed(iGrp, iShape)
                    flt.Weight = m_data.MedWeights(m_data.nGroups + iFlt, iShape)
                    m_fleets.Add(flt)
                End If
            Next

            Load()

            m_bInInit = False
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".New() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".New() Error: " & ex.Message, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        'copy the data from zscale into an array that will be used to create a forcing data object
        m_bInInit = True
        Me.LockUpdates()

        m_iEcoSimIndex = Array.IndexOf(m_data.MediationDBIDs, m_dbID)
        Debug.Assert(m_iEcoSimIndex > -1, "mediation shape database ID invalid.")
        If m_iEcoSimIndex < 0 Then Return False

        Me.ResizeData(m_data.NMedPoints)
        For ipt As Integer = 1 To m_data.NMedPoints
            Me.ShapeData(ipt) = m_data.Medpoints(ipt, m_iEcoSimIndex)
        Next ipt

        m_nYears = m_data.NumYears
        Me.Name = m_data.MediationTitles(m_iEcoSimIndex)
        Me.m_ForcingApplicationType = eForcingApplicationTypes.NotSet

        'shape parameters
        m_ShapeFunctionType = m_data.MediationShapeParams(m_iEcoSimIndex).ShapeFunctionType
        m_Steep = m_data.MediationShapeParams(m_iEcoSimIndex).Steep
        m_YBase = m_data.MediationShapeParams(m_iEcoSimIndex).YBase
        m_YEnd = m_data.MediationShapeParams(m_iEcoSimIndex).YEnd
        m_YZero = m_data.MediationShapeParams(m_iEcoSimIndex).YZero
        m_ZScale = m_data.MediationShapeParams(m_iEcoSimIndex).ZScale

        Me.UnlockUpdates()
        m_bInInit = False
        Return True

    End Function

#End Region ' Constructors

#Region "Properties"

    ''' <summary>
    ''' X Axis base index for biomass
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the vertical green line in the EwE 5 mediation interface</remarks>
    Public Property XBaseIndex() As Integer
        Get
            Try
                Return m_data.IMedBase(Array.IndexOf(m_data.MediationDBIDs, m_dbID))
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex)
            End Try

        End Get
        Set(ByVal value As Integer)
            Try
                m_data.IMedBase(Array.IndexOf(m_data.MediationDBIDs, m_dbID)) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex)
            End Try
        End Set
    End Property


    ''' <summary>
    ''' X Axis base value for sum of x biomass
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the vertical green line in the EwE 5 mediation interface</remarks>
    Public ReadOnly Property XBase() As Single
        Get
            Try
                Return m_data.MedXbase(Array.IndexOf(m_data.MediationDBIDs, m_dbID))
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex)
            End Try

        End Get

    End Property

#End Region

#Region " Updating "

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If m_bInInit Then
            Return False
        End If

        m_iEcoSimIndex = Array.IndexOf(m_data.MediationDBIDs, m_dbID)
        'can not update if there is not an index to the underlying data structures
        If (m_iEcoSimIndex = cCore.NULL_VALUE) Or (m_iEcoSimIndex > m_data.MediationShapes) Then
            cLog.Write(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        Me.ResizeData(m_data.NMedPoints)

        'populate the raw shape data
        For ipt As Integer = 1 To Me.XMax
            m_data.Medpoints(ipt, m_iEcoSimIndex) = Me.ShapeData(ipt)
        Next ipt

        m_data.MediationTitles(m_iEcoSimIndex) = Me.Name

        ' Forcing application type not applicable to mediation functions
        'm_data.ForcingApplicationType(m_iEcoSimIndex) = Me.m_ForcingApplicationType

        'shape parameters
        m_data.MediationShapeParams(m_iEcoSimIndex).ShapeFunctionType = m_ShapeFunctionType
        m_data.MediationShapeParams(m_iEcoSimIndex).Steep = m_Steep
        m_data.MediationShapeParams(m_iEcoSimIndex).YBase = m_YBase
        m_data.MediationShapeParams(m_iEcoSimIndex).YEnd = m_YEnd
        m_data.MediationShapeParams(m_iEcoSimIndex).YZero = m_YZero
        m_data.MediationShapeParams(m_iEcoSimIndex).ZScale = m_ZScale

        'there can not be both fleets and groups for the same shape
        Debug.Assert(Not (m_groups.Count > 0 And m_fleets.Count > 0))

        Dim nused As Integer
        For Each grp As cMediatingGroup In m_groups
            nused += 1
            m_data.IMedUsed(grp.iGroupIndex, m_iEcoSimIndex) = grp.iGroupIndex
            m_data.MedWeights(grp.iGroupIndex, m_iEcoSimIndex) = grp.Weight
        Next grp

        nused = 0
        For Each flt As cMediatingFleet In m_fleets
            nused += 1
            m_data.IMedUsed(m_data.nGroups + flt.iFleetIndex, m_iEcoSimIndex) = flt.iFleetIndex
            m_data.MedWeights(m_data.nGroups + flt.iFleetIndex, m_iEcoSimIndex) = flt.Weight
        Next flt

        m_data.NMedXused(m_iEcoSimIndex) = nused

        'tell the manager that a shape has changed it's data
        ShapeChanged()

        Return True

    End Function

    ''' <summary>
    ''' Clear all the data, in the underlying ecosim data, out of the MedWeights for this mediation shape.
    ''' </summary>
    ''' <remarks>
    ''' This is used if a mediating group is removed to clear the ecosim data before the group is removed from the list. 
    ''' This must be used in conjuction the Update() to restore the data
    ''' </remarks>
    Private Sub clearMedWeights()

        Try

            For Each grp As cMediatingGroup In m_groups
                m_data.IMedUsed(grp.iGroupIndex, m_iEcoSimIndex) = 0
                m_data.MedWeights(grp.iGroupIndex, m_iEcoSimIndex) = 0
            Next grp

            For Each flt As cMediatingFleet In m_fleets
                m_data.IMedUsed(m_data.nGroups + flt.iFleetIndex, m_iEcoSimIndex) = 0
                m_data.MedWeights(m_data.nGroups + flt.iFleetIndex, m_iEcoSimIndex) = 0
            Next flt

        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

#End Region ' Updating

#Region " List Interfaces "

    'Public Function GetGroupEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    '    Return m_groups.GetEnumerator
    'End Function

    Public Function AddGroup(ByRef Group As cMediatingGroup) As Boolean
        m_groups.Add(Group)
        Update()
        Return True
    End Function

    Public Function AddGroup(ByVal iGroup As Integer, ByVal weight As Single) As Boolean

        'ToDo: data validation
        m_groups.Add(New cMediatingGroup(iGroup, weight))
        Update()
        Return True
    End Function

    Public Property Group(ByVal iGroup As Integer) As cMediatingGroup

        Get
            Return m_groups(iGroup)
        End Get

        Set(ByVal value As cMediatingGroup)
            m_groups.Item(iGroup) = value
            Update()
        End Set

    End Property

    Public ReadOnly Property CountGroup() As Integer
        Get
            Return m_groups.Count
        End Get
    End Property

    Public Function RemoveGroup(ByVal iGroup As Integer) As Boolean

        Try
            'clear the ecosim data
            clearMedWeights()
            'remove the group from the list
            m_groups.RemoveAt(iGroup)
            'update the ecosim data with the remaining group(s)
            Update()

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function RemoveGroup(ByRef group As cMediatingGroup) As Boolean

        Try
            'clear the ecosim data
            clearMedWeights()
            'remove the group from the list
            m_groups.Remove(group)
            'update the ecosim data with the remaining group(s)
            Update()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Sub Clear()

        Try
            'clear the ecosim data
            clearMedWeights()
            m_groups.Clear()
            m_fleets.Clear()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub


    Public Function AddFleet(ByRef Fleet As cMediatingFleet) As Boolean
        m_fleets.Add(Fleet)
        Update()
        Return True
    End Function

    Public Function AddFleet(ByVal iFleet As Integer, ByVal weight As Single) As Boolean

        'ToDo: data validation
        m_fleets.Add(New cMediatingFleet(iFleet, weight))
        Update()
        Return True
    End Function

    Public Property Fleet(ByVal iFleet As Integer) As cMediatingFleet

        Get
            Return m_fleets(iFleet)
        End Get

        Set(ByVal value As cMediatingFleet)
            m_fleets.Item(iFleet) = value
            Update()
        End Set

    End Property

    Public ReadOnly Property CountFleet() As Integer
        Get
            Return m_fleets.Count
        End Get
    End Property

    Public Function RemoveFleet(ByVal iFleet As Integer) As Boolean

        Try
            'clear the ecosim data
            clearMedWeights()
            'remove the fleet from the list
            m_fleets.RemoveAt(iFleet)
            'update the ecosim data with the remaining fleet(s)
            Update()

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function RemoveFleet(ByRef fleet As cMediatingFleet) As Boolean

        Try
            'clear the ecosim data
            clearMedWeights()
            'remove the fleet from the list
            m_fleets.Remove(fleet)
            'update the ecosim data with the remaining fleet(s)
            Update()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

#End Region '  List Interfaces

End Class

#End Region ' cMediationFunction

#End Region ' Mediation Shape

#Region " Fishing Rate shape "

''' <summary>
''' A fish s
''' </summary>
''' <remarks></remarks>
Public Class cFishingRateShape
    Inherits cForcingFunction

    'Private m_ntimesteps As Integer

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef Manager As cBaseShapeManager, ByVal DBID As Integer, ByVal strFleetName As String)

        MyBase.New(EcoSimData, Manager, DBID, eDataTypes.FishingEffort)

        m_bInInit = True
        Me.DBID = DBID

        m_data = EcoSimData
        'this can be changed to use the database id see load

        Me.Name = strFleetName ' the fleetname is only stored in the Ecopath Data so it has to be passed into the constructor

        'create a new forcing data object this only happens when the shape is created
        'if the shape is being initialized from the EcoSim the forcing object must already exist

        Load()

        m_bInInit = False

    End Sub


    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        'copy the Fishing rate data into an array that will be used to create a forcing data object
        m_bInInit = True
        Dim m_ntimesteps As Integer = m_data.NTimes

        Debug.Assert(m_iEcoSimIndex > -1, Me.ToString & " database ID invalid.")
        If m_iEcoSimIndex = -1 Then Return False

        Me.ResizeData(m_ntimesteps)

        For ipt As Integer = 1 To m_ntimesteps
            Me.ShapeData(ipt) = m_data.FishRateGear(m_iEcoSimIndex, ipt) 'FishRateGear(NFleets,nTime)
        Next ipt

        m_nYears = m_data.NumYears

        m_bInInit = False

        Return True

    End Function

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If m_bInInit Then
            Return False
        End If

        'can not update if there is not an index to the underlying data structures
        If (m_iEcoSimIndex = cCore.NULL_VALUE) Or (m_iEcoSimIndex > m_data.nGear + 1) Then
            cLog.Write(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        'jb Sept-06 this can not happen for this type of object because all time dimensioning is handled by the core
        'and a new object will be created by the core with the new number of time steps changes when the time changes
        'm_Xdata.ResizeData(m_ntimesteps)

        Dim orgvalue As Single, newvalue As Single
        Dim bhaschanged As Boolean
        Dim isCombinedFleets As Boolean

        If m_iEcoSimIndex = m_data.nGear + 1 Then
            isCombinedFleets = True
        End If

        'we have to loop over all the time steps because we don't know what has changed
        For it As Integer = 1 To m_data.NTimes

            orgvalue = m_data.FishRateGear(m_iEcoSimIndex, it)
            newvalue = Me.ShapeData(it)

            'test the new value against the existing value 
            'if there is no change don't bother going through the entire update process 
            'this is to make it run a little faster in a live environment
            If newvalue <> orgvalue Then
                bhaschanged = True

                'update FishRateGear() with the new values 
                m_data.FishRateGear(m_iEcoSimIndex, it) = Me.ShapeData(it)

                'this shape is the combined fleets type so update all the fleets types with the new value
                If isCombinedFleets Then
                    For iFlt As Integer = 1 To m_data.nGear
                        m_data.FishRateGear(iFlt, it) = m_data.FishRateGear(m_iEcoSimIndex, it) 'dont worry about overwritting the fleet we just update
                    Next
                End If

                'FishRateGear() is a multiplier that is used to change the catch rate for all the groups caught by this fleet
                'It represents the fishing effort 1 is no change in effort from the existing value, zero would remove all fishing, two would double the catch rate
                'Now use FishRateGear/effort to update the catch rate for each group fished by this fleet
                For igrp As Integer = 1 To m_data.nGroups

                    If Not isCombinedFleets Then

                        m_data.FishRateNo(igrp, it) = m_data.FishRateNo(igrp, it) + m_data.FishMGear(m_iEcoSimIndex, igrp) * (m_data.FishRateGear(m_iEcoSimIndex, it) - orgvalue)

                    Else
                        'combined fleet this changes all the catch rates
                        m_data.FishRateNo(igrp, it) = 0
                        For iflt As Integer = 1 To m_data.nGear
                            m_data.FishRateNo(igrp, it) = m_data.FishRateNo(igrp, it) + m_data.FishMGear(iflt, igrp) * m_data.FishRateGear(iflt, it)
                        Next iflt

                    End If

                Next igrp
            End If 'newvalue <> orgvalue

        Next it

        If bhaschanged Then
            'tell the manager that a shape has changed it's data
            ShapeChanged()
        End If

        Return True

    End Function

End Class

#End Region ' Fishing Rate Shape

#Region " Fish Mortality Shape "

''' <summary>
''' A fish s
''' </summary>
''' <remarks></remarks>
Public Class cFishingMortShape
    Inherits cForcingFunction


    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef Manager As cBaseShapeManager, ByVal DBID As Integer, ByVal GroupName As String)

        MyBase.New(EcoSimData, Manager, DBID, eDataTypes.FishMort)
        m_bInInit = True

        'iEcoSimIndex is the array index in the underlying EcoSim data
        m_iEcoSimIndex = Array.IndexOf(m_data.FishRateNoDBID, Me.DBID)

        Me.Name = GroupName 'groupname is part of the Ecopath data so it can not be retrieved from the Ecosim data and must be passed in

        Load()

        m_bInInit = False

    End Sub


    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        m_bInInit = True

        m_iEcoSimIndex = Array.IndexOf(m_data.FishRateNoDBID, m_dbID)
        Debug.Assert(m_iEcoSimIndex <> -1, Me.ToString & ".Load() invalid database ID.")
        If m_iEcoSimIndex = -1 Then Return False

        Me.ResizeData(m_data.NTimes)
        For ipt As Integer = 1 To m_data.NTimes
            Me.ShapeData(ipt) = m_data.FishRateNo(m_iEcoSimIndex, ipt) 'FishRateNo(nGroups,nTime)
        Next ipt

        m_nYears = m_data.NumYears

        m_bInInit = False

        Return True

    End Function

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If m_bInInit Then
            Return False
        End If

        'can not update if there is not an index to the underlying data structures
        If (m_iEcoSimIndex = cCore.NULL_VALUE) Or (m_iEcoSimIndex > m_data.nGroups) Then
            cLog.Write(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        Me.ResizeData(m_data.NTimes)

        'populate the raw shape data
        For ipt As Integer = 1 To m_data.NTimes
            m_data.FishRateNo(m_iEcoSimIndex, ipt) = Me.ShapeData(ipt) 'FishRateNo(nGroups,nTime)
        Next ipt

        'tell the manager that a shape has changed it's data
        ShapeChanged()

        Return True

    End Function

End Class

#End Region ' Fish Mortality Shape

