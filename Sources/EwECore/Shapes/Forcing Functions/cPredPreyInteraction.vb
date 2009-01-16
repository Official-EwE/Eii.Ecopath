'==============================================================================
'
' $Log: cPredPreyInteraction.vb,v $
' Revision 1.2  2009/01/16 18:30:33  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/06/06 15:56:07  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.4  2008/01/21 20:27:53  joeb
' Remove m_bPendingUpdates it was stopping the data from updating even if a lock was not set
'
' Revision 1.3  2007/10/04 14:26:28  jeroens
' * Smoothened pending updates bahaviour
'
' Revision 1.2  2007/09/10 17:03:00  jeroens
' + Added update lock to improve performance
'
'==============================================================================

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
    ''' <remarks></remarks>
    Private Class cShapeFunctionTypePair
        Public Shape As cForcingFunction = Nothing
        Public FunctionType As eForcingFunctionApplication
    End Class

#End Region

    Private m_pred As Integer
    Private m_prey As Integer
    Private m_isProd As Boolean
    Private m_manager As cPPIManager
    Private m_SFPairs As New List(Of cShapeFunctionTypePair)

    Private m_dbid As Integer

#End Region

#Region "Construction and Initialization"

    Sub New(ByVal PredIndex As Integer, ByVal PreyIndex As Integer, ByRef PPIManager As cPPIManager)

        m_dbid = cCore.NULL_VALUE '???

        m_pred = PredIndex
        m_prey = PreyIndex
        m_manager = PPIManager

        'this logic comes from EwE5 frmAddFunction.Form_Load()
        If m_pred = m_prey And m_manager.getEcoPathData.PP(m_prey) = 1 Then
            m_isProd = True
        End If

        'initialize the list of shape/functiontype pairs with the number of function modifiers from Ecosim
        'Modifiers that are not used will have a NULL shape in the cShapeFunctionTypePair object
        For i As Integer = 1 To m_manager.getEcoSimData.MaxFunctions
            m_SFPairs.Add(New cShapeFunctionTypePair)
        Next

    End Sub

    ''' <summary>
    ''' Build the list of shapes used by this interaction from the underlying Ecosim data.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function Load() As Boolean
        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData
        Dim SFPair As cShapeFunctionTypePair

        For i As Integer = 1 To esdata.MaxFunctions

            If esdata.FunctionNumber(m_prey, m_pred, i) = 0 Then Exit For

            'get the cShapeFunctionTypePair object for this index
            SFPair = m_SFPairs.Item(i - 1) 'Ecosim indexes are one based m_SFPairs is zero based

            SFPair.FunctionType = DirectCast(esdata.FunctionType(m_prey, m_pred, i), eForcingFunctionApplication)

            'the other way to do this would be to search the PPImanager it has a list of ALL shapes
            If esdata.IsMedFunction(m_prey, m_pred, i) Then
                SFPair.Shape = Me.getShapeFromEcosimIndex(m_manager.getCore.MediationShapeManager, esdata.FunctionNumber(m_prey, m_pred, i))
            Else
                SFPair.Shape = Me.getShapeFromEcosimIndex(m_manager.getCore.ForcingShapeManager, esdata.FunctionNumber(m_prey, m_pred, i))
            End If

        Next i

    End Function

#End Region

#Region "Public Properties"


    ''' <summary>
    ''' Maximum number of shapes for this Pred Prey interaction
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MaxNumShapes() As Integer
        Get
            Return m_manager.getEcoSimData.MaxFunctions
        End Get
    End Property


    Public ReadOnly Property PredIndex() As Integer
        Get
            Return m_pred
        End Get
    End Property

    Public ReadOnly Property PreyIndex() As Integer
        Get
            Return m_prey
        End Get
    End Property

    ''' <summary>
    ''' Number of shapes that are used by this pred prey interaction.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The first shape that is Nothing marks the end of the series. No shapes after that will be used</remarks>
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

    Public ReadOnly Property isProdRate() As Boolean
        Get
            Return m_isProd
        End Get
    End Property

#End Region

#Region "Editing and Updating"

    ''' <summary>
    ''' Get the shape and FunctionType at ItemIndex
    ''' </summary>
    ''' <param name="ItemIndex">Index of the shape to set. There can be up to MaxNumShapes for a pred prey interaction</param>
    ''' <param name="Shape">A reference to the shape that is use for this pred prey. If shape in Nothing (Null) then no modifier will be used for this pred prey at this index</param>
    ''' <param name="FunctionType"></param>
    ''' <returns>True if there is a shape modifier defined at this index</returns>
    ''' <remarks></remarks>
    Public Function getShape(ByVal ItemIndex As Integer, ByRef Shape As cForcingFunction, ByRef FunctionType As eForcingFunctionApplication) As Boolean
        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData

        Try

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
    ''' Set the shape and functiontype for this index
    ''' </summary>
    ''' <param name="ItemIndex">Index of the shape to set. There can be up to MaxNumShapes for a pred prey interaction </param>
    ''' <param name="Shape">new Shape to use for this pred prey. If shape in Nothing (Null) then no modifier will be used for the pred prey</param>
    ''' <param name="FunctionType">Type of varaible to apply this modifier to</param>
    ''' <returns>True is the index was in bounds and the shape was set</returns>
    ''' <remarks>To clear an index set the shape to Nothing</remarks>
    Public Function setShape(ByVal ItemIndex As Integer, ByRef Shape As cForcingFunction, _
            Optional ByVal FunctionType As eForcingFunctionApplication = eForcingFunctionApplication.SearchRate) As Boolean

        Dim esdata As cEcosimDatastructures = m_manager.getEcoSimData

        Try

            If ItemIndex > esdata.MaxFunctions Or ItemIndex < 1 Then
                Shape = Nothing
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
            sfPair.Shape = Shape
            sfPair.FunctionType = FunctionType

            'update the ecosim data
            Me.Update()

            Return True

        Catch ex As Exception
            Debug.Assert(False, "Error: " & Me.ToString & ".setShape() " & ex.Message)
            Shape = Nothing
            Return False
        End Try

    End Function

    'jb Jan-18-2008 removed m_bPendingUpdates 
    'm_bPendingUpdates had to be True for Update() to work
    'if a shape is added when no lock is in place m_bPendingUpdates will be always be false
    'this blocks the Update() and the data can never be updated
    'Dim m_bPendingUpdates As Boolean = False
    Dim m_bLockUpdates As Boolean = False

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

        If LockUpdates Then
            '  m_bPendingUpdates = True
            Return
        End If

        '  If (m_bPendingUpdates = True) Then

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

            m_manager.getCore.onChanged(Me)
            'm_bPendingUpdates = False

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
        Debug.Assert(False, Me.ToString & ".Failed to find shape.")
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
        'this will generate an ID that identifies this object by its Pred/Prey pair
        'this is not guaranteed to be unique
        Return cValueID.Generate(Me, m_manager.getKey(m_pred, m_prey))
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
