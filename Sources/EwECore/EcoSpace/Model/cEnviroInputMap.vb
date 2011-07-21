
''' <summary>
''' Interface for defining Ecospace Environmental Input maps
''' </summary>
''' <remarks></remarks>
Public Interface IEnviroInputMap
    Function ResponseFunction(ByVal igrp As Integer, ByVal iRow As Integer, ByVal iCol As Integer) As Single
    Function Init(ByVal MediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean
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
    ''' <param name="iRow">Row of the input map</param>
    ''' <param name="iCol">Col of the input map</param>
    ''' <returns>Y = F(x)</returns>
    ''' <remarks></remarks>
    Public Function ResponseFunction(ByVal igrp As Integer, ByVal iRow As Integer, ByVal iCol As Integer) As Single Implements IEnviroInputMap.ResponseFunction
        Dim iShp As Integer, MedX As Single, ip As Long

        Try
            iShp = Me.setResponseForGroup(igrp)
            'at this time I'm not sure if this is a error or not!
            Debug.Assert(iShp <> 0, Me.ToString & ".ResponseFunction() no function has been set for this group!")
            'no shape has been set for this group
            If iShp = 0 Then
                'need to decide what the null response should be
                Return 0
            End If

            MedX = 0.0000000001
            Dim obj As Object = Me.m_map(iRow, iCol)
            MedX = CType(obj, Single)

            '060328 CJW found that without the +0.01 below it could be unstable when slope
            'was large around Ecopath base point in mediation function, causing instability.
            'This solves it. VC.
            ip = Int(Me.m_MedData.IMedBase(iShp) * MedX / Me.m_MedData.MedXbase(iShp) + 0.01F)
            If ip < 1 Then ip = 1
            If ip > Me.m_MedData.NMedPoints Then ip = Me.m_MedData.NMedPoints
            Return Me.m_MedData.Medpoints(ip, iShp) / Me.m_MedData.MedYbase(iShp)

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

    ''' <summary>
    ''' Sets or gets the response(mediation) function to use from the current cMediationDataStructures load during the Init(...)
    ''' </summary>
    ''' <param name="GrpIndex">Group index for the response function.</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The Index of the ResponseFunction must exist in the underlying mediation data.</remarks>
    Public Property setResponseForGroup(ByVal GrpIndex As Integer) As Integer
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

End Class
