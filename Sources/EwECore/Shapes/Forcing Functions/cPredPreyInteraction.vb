Option Explicit On
Imports EwEUtils.Core

''' <summary>
''' Class to wrap the shape and function type modifiers for a pred/prey interaction
''' </summary>
''' <remarks>This will populate a list of five(MaxFunctions) shapes/functiontype pairs on construction. 
''' A user calls setShape(,,,) or getShape(,,,) to modify the shape or functiontype for this Pred Prey interaction. </remarks>
Public Class cPredPreyInteraction
    Implements ICoreInterface

#Region "Private Data"

#Region "Private class"

    'ToDo_jb cPredPreyInteraction needs to set the needs update

    ''' <summary>
    ''' Private class to hold the shape and function type for each possible modifier.
    ''' </summary>
    Private Class cShapeFunctionTypePair
        Public Shape As cForcingFunction = Nothing
        Public FunctionType As eForcingFunctionApplication
    End Class

#End Region

    Private m_pred As Integer
    Private m_prey As Integer
    Private m_bIsProd As Boolean
    Private m_manager As cPPIManager
    Private m_SFPairs As New List(Of cShapeFunctionTypePair)

    Private m_dbid As Integer

#End Region

#Region "Construction and Initialization"

    ''' <summary>
    ''' Create a new interaction.
    ''' </summary>
    ''' <param name="PredIndex"><see cref="cCoreGroupBase.Index">Predator index</see>.</param>
    ''' <param name="PreyIndex"><see cref="cCoreGroupBase.Index">Prey index</see>.</param>
    ''' <param name="PPIManager"><see cref="cPPIManager">Predator/prey interaction manager</see>.</param>
    Sub New(ByVal PredIndex As Integer, ByVal PreyIndex As Integer, ByVal PPIManager As cPPIManager)

        Me.m_dbid = cCore.NULL_VALUE '???

        Me.m_pred = PredIndex
        Me.m_prey = PreyIndex
        Me.m_manager = PPIManager

        'this logic comes from EwE5 frmAddFunction.Form_Load()
        If (Me.m_pred = Me.m_prey) And (Me.m_manager.getEcoPathData.PP(Me.m_prey) = 1) Then
            Me.m_bIsProd = True
        End If

        'initialize the list of shape/functiontype pairs with the number of function modifiers from Ecosim
        'Modifiers that are not used will have a NULL shape in the cShapeFunctionTypePair object
        For i As Integer = 1 To Me.m_manager.getEcoSimData.MaxFunctions
            Me.m_SFPairs.Add(New cShapeFunctionTypePair())
        Next

    End Sub

    ''' <summary>
    ''' Build the list of shapes used by this interaction from the underlying Ecosim data.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    Friend Function Load() As Boolean

        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData
        Dim SFPair As cShapeFunctionTypePair
        Dim bSucces As Boolean = True

        For i As Integer = 1 To esdata.MaxFunctions

            If esdata.FunctionNumber(m_prey, m_pred, i) = 0 Then Exit For

            'get the cShapeFunctionTypePair object for this index
            SFPair = m_SFPairs.Item(i - 1) 'Ecosim indexes are one based, m_SFPairs is zero based

            SFPair.FunctionType = DirectCast(esdata.FunctionType(m_prey, m_pred, i), eForcingFunctionApplication)

            'the other way to do this would be to search the PPImanager; it has a list of ALL shapes
            If esdata.IsMedFunction(m_prey, m_pred, i) Then
                SFPair.Shape = Me.getShapeFromEcosimIndex(Me.m_manager.getCore.MediationShapeManager, esdata.FunctionNumber(Me.m_prey, Me.m_pred, i))
            Else
                SFPair.Shape = Me.getShapeFromEcosimIndex(Me.m_manager.getCore.ForcingShapeManager, esdata.FunctionNumber(Me.m_prey, Me.m_pred, i))
            End If

        Next i
        Return bSucces

    End Function

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' Get the maximum number of shapes that can be assigned to a 
    ''' pred/prey interaction.
    ''' </summary>
    Public ReadOnly Property MaxNumShapes() As Integer
        Get
            Return m_manager.getEcoSimData.MaxFunctions
        End Get
    End Property

    ''' <summary>
    ''' Get the <see cref="cCoreGroupBase.Index">index</see> of the predator
    ''' for this interaction.
    ''' </summary>
    Public ReadOnly Property PredIndex() As Integer
        Get
            Return m_pred
        End Get
    End Property

    ''' <summary>
    ''' Get the <see cref="cCoreGroupBase.Index">index</see> of the prey for
    ''' this interaction.
    ''' </summary>
    Public ReadOnly Property PreyIndex() As Integer
        Get
            Return m_prey
        End Get
    End Property

    ''' <summary>
    ''' Get the number of shapes that are used by this predator/prey interaction.
    ''' </summary>
    ''' <remarks>The first shape that is Nothing marks the end of the series. 
    ''' No shapes after that will be used.</remarks>
    Public ReadOnly Property NAppliedShapes() As Integer

        Get
            Dim n As Integer
            'count the number of shapes that are used 
            'all shapes after the first null shape are not used
            For Each sfpair As cShapeFunctionTypePair In m_SFPairs
                If sfpair.Shape IsNot Nothing Then
                    n += 1
                Else
                    Exit For
                End If
            Next

            Return n

        End Get

    End Property

    ''' <summary>
    ''' Get whether this interaction denotes a production rate interaction.
    ''' </summary>
    Public ReadOnly Property isProdRate() As Boolean
        Get
            Return m_bIsProd
        End Get
    End Property

#End Region

#Region "Editing and Updating"

    ''' <summary>
    ''' Get a shape modifier, consisting of a <see cref="cForcingFunction">forcing funtion</see> and 
    ''' a <see cref="eForcingFunctionApplication">Type of variable</see>, defined at a given index.
    ''' </summary>
    ''' <param name="ItemIndex">One-based index of the <see cref="cForcingFunction">shape</see> and 
    ''' <see cref="eForcingFunctionApplication">FunctionType</see> to retreive. There can 
    ''' be up to <see cref="MaxNumShapes">MaxNumShapes</see> for a pred prey interaction.</param>
    ''' <param name="Shape">A reference to the shape that is used for this pred/prey 
    ''' interaction.</param>
    ''' <param name="FunctionType"><see cref="eForcingFunctionApplication">Type of variable</see>
    ''' that this modifier applies to.</param>
    ''' <returns>True if there is a shape modifier defined at this index.</returns>
    Public Function getShape(ByVal ItemIndex As Integer, _
                             ByRef Shape As cForcingFunction, _
                             ByRef FunctionType As eForcingFunctionApplication) As Boolean

        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData

        Try

            ' Sanity checks
            Debug.Assert(ItemIndex > 0 And ItemIndex <= esdata.MaxFunctions, Me.ToString & ".getShape() ItemIndex out of bounds.")

            If ItemIndex > esdata.MaxFunctions Or ItemIndex < 1 Then
                Shape = Nothing
                Return False
            End If

            'm_SFPairs list is zero based
            'indexes in the interface are one based
            Dim iList As Integer = ItemIndex - 1

            Dim pair As cShapeFunctionTypePair = m_SFPairs.Item(iList)
            Shape = pair.Shape
            FunctionType = pair.FunctionType

            If Shape IsNot Nothing Then
                Return True
            Else
                'no shape defined for this index
                Return False
            End If

        Catch ex As Exception
            Debug.Assert(False, "Error: " & Me.ToString & ".getShape() " & ex.Message)
            Shape = Nothing
            Return False
        End Try

    End Function


    ''' <summary>
    ''' Set a shape modifier, consisting of a <see cref="cForcingFunction">forcing function</see> and 
    ''' <see cref="eForcingFunctionApplication">function type</see>, for a given index.
    ''' </summary>
    ''' <param name="ItemIndex">One-base index of the shape to set. There can be 
    ''' up to <see cref="MaxNumShapes">MaxNumShapes</see> for a pred/prey interaction.</param>
    ''' <param name="Shape"><see cref="cForcingFunction">Shape</see> to use for this 
    ''' pred/prey interaction index. If the shape is Nothing/Null then no modifier will be 
    ''' used for this pred/prey interaction index.</param>
    ''' <param name="FunctionType"><see cref="eForcingFunctionApplication">Type of variable</see>
    ''' to apply this modifier to.</param>
    ''' <returns>True is the index was in bounds and the shape was set</returns>
    ''' <remarks>To clear an index set the shape to Nothing</remarks>
    Public Function setShape(ByVal ItemIndex As Integer, _
                             ByVal shape As cForcingFunction, _
                             Optional ByVal FunctionType As eForcingFunctionApplication = eForcingFunctionApplication.SearchRate) As Boolean

        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData

        Try

            If (ItemIndex > esdata.MaxFunctions) Or (ItemIndex < 1) Then
                shape = Nothing
                Debug.Assert(False, Me.ToString & ".setShape() ShapeIndex out of bounds.")
                Return False
            End If

            'm_SFPairs list is zero based
            'indexes in the interface are one based
            Dim iList As Integer = ItemIndex - 1

            'set the shape object and the function type
            'in the already existing cShapeFunctionTypePair object from the m_SFPairs list
            'the cShapeFunctionTypePair objects were created when this interaction object was constructed
            Dim sfPair As cShapeFunctionTypePair = m_SFPairs.Item(iList)
            sfPair.Shape = shape
            sfPair.FunctionType = FunctionType

            'update the ecosim data
            Me.Update()

            Return True

        Catch ex As Exception
            Debug.Assert(False, "Error: " & Me.ToString & ".setShape() " & ex.Message)
            shape = Nothing
            Return False
        End Try

    End Function

    Dim m_bLockUpdates As Boolean = False

    ''' <summary>
    ''' Get/set whether updates should not be sent to the core. This functionality 
    ''' is particularly useful when making a series of changes to pred/prey interactions.
    ''' </summary>
    Public Property LockUpdates() As Boolean
        Get
            Return m_bLockUpdates
        End Get
        Set(ByVal value As Boolean)
            m_bLockUpdates = value
            Me.Update()
        End Set
    End Property

    ''' <summary>
    ''' Update the underlying Ecosim data with the values in this pred prey interaction
    ''' </summary>
    ''' <remarks>The update does not communicate the update with the core that is done by what/who ever called the method. 
    ''' This allows a manager to update all the data then tell the core. </remarks>
    Friend Sub Update()
        Dim ishp As Integer
        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData

        If LockUpdates Then Return

        Try

            'this only need to set FunctionNumber(), FunctionType() and IsMedFunction() 
            'Ecosim will set MedIsUsed() in InitializeMedFunctions() based on FunctionNumber()
            For Each sfPair As cShapeFunctionTypePair In m_SFPairs
                ishp += 1
                If sfPair.Shape IsNot Nothing Then
                    esdata.FunctionNumber(m_prey, m_pred, ishp) = sfPair.Shape.Index 'Index to data arrays in Ecosim zscale()
                    esdata.FunctionType(m_prey, m_pred, ishp) = sfPair.FunctionType
                    If TypeOf sfPair.Shape Is cMediationFunction Then
                        esdata.IsMedFunction(m_prey, m_pred, ishp) = True
                    Else
                        esdata.IsMedFunction(m_prey, m_pred, ishp) = False
                    End If
                Else
                    esdata.FunctionNumber(m_prey, m_pred, ishp) = 0
                    esdata.FunctionType(m_prey, m_pred, ishp) = 0
                    esdata.IsMedFunction(m_prey, m_pred, ishp) = False 'this probable doesn't matter
                End If

            Next

            Me.m_manager.getCore.onChanged(Me)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() " & ex.Message)
        End Try

        'End If

    End Sub

#End Region

#Region "Private Methods"

    Private Function getShapeFromEcosimIndex(ByRef theManager As cBaseShapeManager, ByVal iEcosimIndex As Integer) As cForcingFunction

        'HACK find a shape with the matching Ecosim index in the theManager
        For Each shape As cForcingFunction In theManager
            If shape.Index = iEcosimIndex Then
                Return shape
            End If
        Next
        'Debug.Assert(False, Me.ToString & ".Failed to find shape.")
        Return Nothing
    End Function

#End Region

#Region "ICoreInterface implementation"

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return eDataTypes.PredPreyInteraction
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.EcoSim
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
        Return cValueID.getDataTypeID(Me.DataType, CInt(m_pred * 1000 + m_prey))
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return "Predator/Prey interaction"
        End Get
        Set(ByVal value As String)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

#End Region

End Class
