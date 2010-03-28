#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Auxiliary

    ''' =======================================================================
    ''' <summary>
    ''' <para>
    ''' This class represents all Auxiliary data that can be associated with
    ''' any value in the EwECore or an EwE user interface. This data is loose-typed;
    ''' each core and user interface value that requires Auxiliary data must define
    ''' a unique ID via which associated Auxillary data is stored and retreived.
    ''' </para>
    ''' <para>
    ''' When associated with <see cref="ICoreInterface">ICoreInterface</see>
    ''' -derived objects, cAuxillaryData offers the ability to maintain a
    ''' <see cref="ICoreInterface.DataType">data type</see> and 
    ''' <see cref="ICoreInterface.DBID">database ID</see> pair to uniquely
    ''' identify the object instance this data is associated with.
    ''' </para>
    ''' </summary>
    ''' =======================================================================
    Public Class cAuxiliaryData

#Region " Private vars "

        ''' <summary>Unique ID of the data.</summary>
        Private m_strID As String = ""
        ''' <summary><see cref="eDataTypes">data type</see> that soft-links this data 
        ''' to an <see cref="ICoreInterface">ICoreInterface</see> instance.</summary>
        Private m_dataType As eDataTypes = eDataTypes.NotSet
        ''' <summary>Database ID that soft-links this datato an 
        ''' <see cref="ICoreInterface">ICoreInterface</see> instance.</summary>
        Private m_iDBID As Integer = -1
        ''' <summary>Remark text for this data.</summary>
        Private m_strRemark As String = ""
        ''' <summary>Visual style for this data.</summary>
        Private m_visualStyle As cVisualStyle = Nothing
        ''' <summary>Pedigree for this data.</summary>
        Private m_iPedigree As Integer = 0

#If USE_REFERENCES Then
        ''' <summary>List of <see cref="cReference">references</see> for this data.</summary>
        Private m_references As New List(Of cReference)
#End If

#End Region ' Private vars

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of cAuxiliaryData that is 
        ''' NOT soft-linked to any core object.
        ''' </summary>
        ''' <param name="strID">Unique ID to assign to this cAuxillaryData instance.</param>
        ''' <remarks>
        ''' <para>This constructor should be used when defining cAuxilaryData for derived 
        ''' values and values from objects that do not originate from the EwE core.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strID As String)
            Me.New(strID, eDataTypes.NotSet, -1)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of cAuxiliaryData that is soft-linked
        ''' to an <see cref="ICoreInterface">ICoreInterface</see>-derived object. 
        ''' </summary>
        ''' <param name="strID">Unique ID to assign to this cAuxillaryData instance.</param>
        ''' <param name="obj"><see cref="ICoreInterface">ICoreInterface</see>-derived
        ''' instance to associate this data with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strID As String, ByVal obj As ICoreInterface)
            Me.New(strID, obj.DataType, obj.DBID)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of cAuxiliaryData that is soft-linked
        ''' to the <see cref="ICoreInterface">ICoreInterface</see>-derived object identified
        ''' by a <paramref name="dataType">data type</paramref> and a <paramref name="iDBID">database ID</paramref>.
        ''' </summary>
        ''' <param name="strID">Unique ID to assign to this cAuxillaryData instance.</param>
        ''' <param name="dataType">The <see cref="eDataTypes">data type</see> that identifies the
        ''' <see cref="ICoreInterface">ICoreInterface</see>-derived object to associate this data with.</param>
        ''' <param name="iDBID">The database ID that identifies the
        ''' <see cref="ICoreInterface">ICoreInterface</see>-derived object to associate this data with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strID As String, ByVal dataType As eDataTypes, ByVal iDBID As Integer)
            ' Sanity check
            Debug.Assert(Not String.IsNullOrEmpty(strID))

            Me.m_strID = strID
            Me.m_dataType = dataType
            Me.m_iDBID = iDBID
        End Sub

#End Region ' Constructors

#Region " Public properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the unique ID assigned to this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ID() As String
            Get
                Return Me.m_strID
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eDataTypes">data type</see> that, in conjunction 
        ''' with <see cref="DBID">DBID</see>, identifies the 
        ''' <see cref="ICoreInterface">ICoreInterface</see>-derived object that 
        ''' this data is associated with.
        ''' </summary>
        ''' <remarks>This property will return 
        ''' <see cref="eDataTypes.NotSet">eDataTypes.NotSet</see>.</remarks>
        ''' if this data is not associated with any object
        ''' -------------------------------------------------------------------
        Public ReadOnly Property DataType() As eDataTypes
            Get
                Return Me.m_dataType
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the database ID that, in conjunction with <see cref="DataType">DataType</see>, 
        ''' identifies the <see cref="ICoreInterface">ICoreInterface</see>
        ''' -derived object that this data is associated with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property DBID() As Integer
            Get
                Return Me.m_iDBID
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the remark text for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Remark() As String
            Get
                Return Me.m_strRemark
            End Get
            Set(ByVal value As String)
                Me.m_strRemark = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the pedigree level ID for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Pedigree() As Integer
            Get
                Return Me.m_iPedigree
            End Get
            Set(ByVal value As Integer)
                Me.m_iPedigree = value
            End Set
        End Property

#If USE_REFERENCES Then

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of <see cref="cReference">references</see> for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function References() As List(Of cReference)
            Return Me.m_references
        End Function

#End If

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the visual style for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property VisualStyle() As cVisualStyle
            Get
                Return Me.m_visualStyle
            End Get
            Set(ByVal value As cVisualStyle)
                Me.m_visualStyle = value
            End Set
        End Property

#End Region ' Public properties

    End Class

End Namespace
