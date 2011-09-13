#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwECore.ValueWrapper

#End Region ' Imports

''' <summary>
''' Joins an input map(row,col) with a list(by group) of Environmental Response functions (mediation functions).
''' </summary>
''' <typeparam name="T">Type of map</typeparam>
''' <remarks>
''' Set the Map to the input map then tell it which response functions to use for which groups setShapeForGroup(igroup) = iResponseFunction
''' </remarks>
Public Class cEnviroInputMap(Of T)
    Inherits cCoreInputOutputBase
    Implements IEnviroInputMap

#Region " Private vars "

    Private m_map(,) As T
    Private m_GrpToShape() As Integer
    Private m_MedData As cMediationDataStructures
    Private m_spaceData As cEcospaceDataStructures
    Private m_strName As String
    Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
    Private m_min As Single
    Private m_max As Single
    Private m_mean As Single
    Private m_binWidth As Single
    Private m_manager As cMapResponseInteractionManager

#End Region ' Private vars

    Friend Sub New(ByVal core As cCore, ByVal iDBID As Integer, ByVal iIndex As Integer, ByVal strName As String, _
                   ByVal varName As eVarNameFlags, ByVal theManager As cMapResponseInteractionManager, ByVal MapArray(,) As T)

        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData

        Me.AllowValidation = False

        Me.m_coreComponent = EwEUtils.Core.eCoreComponentType.EcoSpace
        Me.m_dataType = EwEUtils.Core.eDataTypes.EcospaceMapResponse
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        'VarName
        meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.VariableName, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        Me.Index = iIndex
        Me.DBID = iDBID
        Me.Name = strName
        Me.Variable = varName
        Me.m_map = MapArray
        Me.setManager(theManager)

        ' Init to the data in the manager
        Me.Init(Me.m_manager.MediationData, Me.m_manager.SpaceData)
        Me.Update()

        Me.ResetStatusFlags()
        Me.AllowValidation = True

    End Sub

    ''' <inheritdocs cref="IEnviroInputMap.Init"/>
    Friend Function Init(ByVal EnviroMediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean _
        Implements IEnviroInputMap.Init

        Me.m_MedData = EnviroMediationData
        Me.m_spaceData = SpaceData

        ReDim Me.m_GrpToShape(Me.nGroups)

        Me.computeMinMax()

    End Function

    Private Sub computeMinMax()

        m_min = Single.MaxValue
        m_max = Single.MinValue

        Try
            For ir As Integer = 1 To Me.m_spaceData.InRow
                For ic As Integer = 1 To Me.m_spaceData.InCol
                    Dim ob As Object = Me.m_map(ir, ic)
                    Me.m_min = Math.Min(CSng(ob), Me.m_min)
                    Me.m_max = Math.Max(CSng(ob), Me.m_max)
                Next
            Next
        Catch ex As Exception
            ' Argh
        End Try

        Me.m_mean = (Me.m_min + Me.m_max) * 0.5F

    End Sub

    ''' <inheritdocs cref="IEnviroInputMap.setManager"/>
    Friend Sub setManager(ByVal theManager As cMapResponseInteractionManager) Implements IEnviroInputMap.setManager
        Me.m_manager = theManager
    End Sub

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
                If (Not Me.m_manager Is Nothing) And (Me.AllowValidation = True) Then
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

        Try

            For ir As Integer = 1 To Me.m_spaceData.InRow
                For ic As Integer = 1 To Me.m_spaceData.InCol
                    Dim cell As Single = CSng(CObj(Me.m_map(ir, ic)))
                    ipt = CInt(cell / m_binWidth)
                    If ipt >= nBins Then ipt = nBins
                    If ipt <= 0 Then ipt = 1
                    pts(ipt).Y += 1
                    maxPts = CInt(Math.Max(pts(ipt).Y, maxPts))
                Next
            Next

            'Normalize the histogram
            For i As Integer = 1 To nBins
                pts(i).X = m_binWidth * i
                pts(i).Y = pts(i).Y / maxPts
            Next

        Catch ex As Exception

        End Try

        Return pts

    End Function

#Region " Properties "

    Public ReadOnly Property nGroups() As Integer
        Get
            ' ToDo: remove, obtain from core
            Return Me.m_spaceData.NGroups
        End Get
    End Property

    Public ReadOnly Property nFleets() As Integer
        Get
            ' ToDo: remove, obtain from core
            Return Me.m_spaceData.nFleets
        End Get
    End Property

    ''' <inheritdocs cref="IEnviroInputMap.Name"/>
    Public Shadows Property Name() As String _
        Implements IEnviroInputMap.Name
        Get
            Return MyBase.Name
        End Get
        Set(ByVal value As String)
            MyBase.Name = value
        End Set
    End Property

    Public Property Variable() As eVarNameFlags _
        Implements IEnviroInputMap.Variable
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.VariableName), eVarNameFlags)
        End Get
        Set(ByVal value As eVarNameFlags)
            Me.SetVariable(eVarNameFlags.VariableName, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the input map that the response function will use to look up its input value
    ''' </summary>
    Friend Property Map() As T(,)
        Get
            Return Me.m_map
        End Get
        Set(ByVal value As T(,))
            Me.m_map = value
        End Set
    End Property

#End Region ' Properties

    ''' <inheritdocs cref="IEnviroInputMap.Max"/>
    Public ReadOnly Property Max() As Single _
        Implements IEnviroInputMap.Max
        Get
            Return Me.m_max
        End Get
    End Property

    ''' ---------------------------------------
    ''' <inheritdocs cref="IEnviroInputMap.Mean"/>
    Public ReadOnly Property Mean() As Single _
        Implements IEnviroInputMap.Mean
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
    Public Function Update() As Boolean Implements IEnviroInputMap.Update
        Dim bReturn As Boolean = False
        Try
            Me.computeMinMax()
            bReturn = True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Exception: " & ex.Message)
        End Try
        Return bReturn

    End Function

End Class
