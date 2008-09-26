'==============================================================================
'
' $Log: cShapeData.vb,v $
' Revision 1.1  2008/09/26 07:30:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/06/06 15:56:08  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.8  2008/02/04 18:54:43  jeroens
' YMax no longer cached
'
' Revision 1.7  2008/01/21 04:06:37  jeroens
' Fixed shape max scale issues, once and for all
'
' Revision 1.6  2007/12/14 15:46:43  jeroens
' + Added SetValue
'
' Revision 1.5  2007/10/31 16:33:49  jeroens
' + Added quiet update unlock option
'
' Revision 1.4  2007/08/03 17:57:26  jeroens
' + Added ability to Refresh calculated Y max
'
' Revision 1.3  2007/07/17 02:15:38  jeroens
' * cTimeSeries now inherited from cShapeData
'
' Revision 1.2  2007/07/12 18:08:34  joeb
' Fixed iEcosim index bug
'
' Revision 1.1  2007/07/12 15:50:44  jeroens
' *** empty log message ***
'
' Revision 1.7  2007/07/05 21:16:25  jeroens
' + Added update locking
'
' Revision 1.6  2006/10/19 23:13:37  joeb
' Changed FishingRate and FishMort shape to live update for testing
'
' Revision 1.5  2006/10/17 21:58:12  joeb
' FishingRate updates FishRateNo
'
' Revision 1.4  2006/10/02 20:10:40  joeb
' Removed incremental updating of shapes. Shapes must now be update explicitly.
'
' Revision 1.3  2006/09/15 02:41:38  jeroens
' + Strict on
'
'==============================================================================

Option Strict On

Imports EwEUtils.Core

''' <summary>
''' Class to handle the data that makes up the shape of a forcing or mediation shape
''' </summary>
''' <remarks>This is used be all the Forcing or Mediation shapes</remarks>
Public MustInherit Class cShapeData
    Implements ICoreInterface

#Region " Private variables "

    'the core array index needs to be accessble by the derived classes
    Protected m_iEcoSimIndex As Integer = 0

    Protected m_datatype As eDataTypes = eDataTypes.NotSet
    Protected m_dbID As Integer = 0
    Private m_strName As String
    Private m_xdata() As Single
    'Private m_Ymax As Single
    Private m_Xmax As Integer
    Private m_bSeasonal As Boolean = False

    Public Event OnChanged(ByVal sd As cShapeData)

#End Region ' Private variables

#Region " Constructors "

    Sub New(ByVal NumberOfPoints As Integer)
        Init(NumberOfPoints)
    End Sub

    Sub New(ByVal ArrayOfData() As Single)
        Init(ArrayOfData)
    End Sub

#End Region ' Constructors

#Region " Capture "

    Private m_iLockCount As Integer = 0

    Public Sub LockUpdates()
        Me.m_iLockCount += 1
    End Sub

    Public Sub UnlockUpdates(Optional ByVal bUpdate As Boolean = True)
        Me.m_iLockCount -= 1
        If ((IsLockedUpdates() = False) And (bUpdate = True)) Then
            Me.Update()
        End If
    End Sub

    Public Function IsLockedUpdates() As Boolean
        Return Me.m_iLockCount <> 0
    End Function

#End Region ' Capture

#Region " Private methods "

    Protected Sub Init(ByVal NumberOfPoints As Integer)

        Debug.Assert(NumberOfPoints >= 0, "You can not initialize cForcingData with less than zero points.")
        m_Xmax = NumberOfPoints

        ReDim m_xdata(m_Xmax)
        Me.SetValue(1.0!)
        Me.Refresh()

    End Sub

    Protected Sub Init(ByVal ArrayOfData() As Single)

        Me.m_Xmax = ArrayOfData.GetUpperBound(0)
        Debug.Assert(m_Xmax > 0, "You can not initialize cForcingData with zero points.")

        Me.m_xdata = ArrayOfData
        'get Y max
        Me.Refresh()

    End Sub

    ''' <summary>
    ''' Determine the max Y value
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Refresh()

        'Me.m_Ymax = 0
        ''calculate Y max of m_xdata
        'For i As Integer = 1 To Me.m_Xmax
        '    Me.m_Ymax = Math.Max(Me.m_Ymax, Me.m_xdata(i))
        'Next

    End Sub

    ''' <summary>
    ''' Update the underlying EcoSim data by calling update on the CForcingFunction object that owns this data
    ''' </summary>
    ''' <remarks>This object does not know it's data is stored in the underlying EcoSim Data. 
    ''' That info is held by the CForcingFunction object that owns this data. This is because different shapes (Forcing or Mediation) store there data differently within the EcoSim data structures.
    ''' </remarks>
    Public MustOverride Function Update() As Boolean

    Public Sub SetValue(ByVal sValue As Single)
        For i As Integer = 0 To Me.m_Xmax
            Me.m_xdata(i) = sValue
        Next
        'm_Ymax = sValue
    End Sub

#End Region ' Private methods

#Region " Properties "

    ''' <summary>
    ''' Data array 
    ''' </summary>
    ''' <value></value>
    ''' <remarks>This is WriteOnly so that you can not get a reference to the underlying array and change the data.</remarks>
    Public Property ShapeData() As Single()
        Get
            Return DirectCast(Me.m_xdata.Clone(), Single())
        End Get
        Set(ByVal value() As Single)
            Init(value)
            Update()
        End Set
    End Property

    Public Property ShapeData(ByVal iPoint As Integer) As Single
        Get
            Try
                Return m_xdata(iPoint)
            Catch ex As Exception
                cLog.Write(Me.ToString & ".ShapeData(" & iPoint.ToString & ") Error: " & ex.Message)
                Debug.Assert(False, Me.ToString & ".ShapeData(" & iPoint.ToString & ") Error: " & ex.Message)
            End Try
        End Get

        Set(ByVal value As Single)
            Try
                m_xdata(iPoint) = value

                ' Invalidate max
                'Me.m_Ymax = cCore.NULL_VALUE

                If Not Me.IsLockedUpdates() Then Me.Update()
            Catch ex As Exception
                cLog.Write(Me.ToString & ".ShapeData(" & iPoint.ToString & ") Error: " & ex.Message)
                Debug.Assert(False, Me.ToString & ".ShapeData(" & iPoint.ToString & ") Error: " & ex.Message)
            End Try
        End Set
    End Property

    ''' <summary>
    ''' Upper bound of the array. 
    ''' </summary>
    ''' <remarks>
    ''' This property cannot be used to resize the data. Call either 
    ''' <see cref="ResizeData">ResizeData</see> or build a new object
    ''' of the desired size.
    ''' </remarks>
    Public ReadOnly Property XMax() As Integer
        Get
            Return m_Xmax
        End Get
    End Property

    Public ReadOnly Property YMax(Optional ByVal bRefresh As Boolean = False) As Single
        Get
            Dim sYMax As Single = 0.0
            For i As Integer = 1 To Me.m_Xmax
                sYMax = Math.Max(sYMax, Me.m_xdata(i))
            Next
            'If ((bRefresh = True) Or (Me.m_Ymax = cCore.NULL_VALUE)) Then Me.Refresh()
            Return sYMax
        End Get
    End Property

    Public Property IsSeasonal() As Boolean
        Get
            Return Me.m_bSeasonal
        End Get

        Set(ByVal bSeasonal As Boolean)
            Me.m_bSeasonal = bSeasonal
            Me.Update()
        End Set
    End Property

#End Region ' Properties

#Region " Friend methods "

    ''' <summary>
    ''' Resize the existing data to a new number of points this will preserve any existing data and populate new points with a value of one (1). 
    ''' New points will have no affect on the model.
    ''' </summary>
    ''' <param name="newNumberOfPoints">New number of points</param>
    ''' <returns>True if successful. False otherwise</returns>
    ''' <remarks>This is called by the Forcing or Mediation shape that owns this data (m_owner.update()) 
    ''' when it needs to update it's data or when the Shape has been added to the Manager. 
    ''' If this object has not been assigned to a Shape then this will not be called and it can hold any amount of data.
    ''' </remarks>
    Friend Function ResizeData(ByVal newNumberOfPoints As Integer) As Boolean

        Try

            Debug.Assert(newNumberOfPoints >= 0, Me.ToString & ".ResizeData() Must be greater then zero points.")

            'Does the data need resizing
            If newNumberOfPoints = m_Xmax Then
                'No
                Return False
            End If

            ReDim Preserve m_xdata(newNumberOfPoints)
            For i As Integer = m_Xmax + 1 To newNumberOfPoints
                m_xdata(i) = 1 'give all the new points the value of one this means they will have no effect on the model
            Next i

            m_Xmax = newNumberOfPoints
            Refresh()

            Return True

        Catch ex As Exception
            cLog.Write(Me.ToString & ".ResizeData() Error: " & ex.Message)
            Return False
        End Try

    End Function

#End Region ' Friend methods

#Region " ICoreInterface implementation "

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return m_dbID
        End Get
        Friend Set(ByVal value As Integer)
            m_dbID = value
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return cValueID.getDataTypeID(m_datatype, Me.DBID)
    End Function

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return m_datatype 'datatype is set in the constructor of each class
        End Get
    End Property

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return m_iEcoSimIndex
        End Get
        Friend Set(ByVal value As Integer)
            m_iEcoSimIndex = value
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.m_strName
        End Get
        Set(ByVal strName As String)
            m_strName = strName
        End Set
    End Property

#End Region ' ICoreInterface implementation

End Class
