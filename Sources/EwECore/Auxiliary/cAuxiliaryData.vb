#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwECore.ValueWrapper

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
        Implements ICoreInterface

#Region " Private vars "

        ''' <summary>Unique database ID</summary>
        Private m_iDBID As Integer = 0
        ''' <summary>Remark text for this data.</summary>
        Private m_strRemark As String = ""
        ''' <summary>Visual style for this data.</summary>
        Private m_visualStyle As cVisualStyle = Nothing
        ''' <summary>Key to identify core variable this data refers to.</summary>
        Private m_key As cValueID = Nothing
        Private m_core As cCore = Nothing
        Private m_bAllowValidation As Boolean = False

#If USE_REFERENCES Then
        ''' <summary>List of <see cref="cReference">references</see> for this data.</summary>
        Private m_references As New List(Of cReference)
#End If

#End Region ' Private vars

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of cAuxiliaryData.
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="strValueID">Unique ID to assign to this cAuxillaryData instance.</param>
        ''' <remarks>
        ''' <para>This constructor should be used when defining cAuxilaryData for derived 
        ''' values and values from objects that do not originate from the EwE core.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Sub New(ByVal core As cCore, ByVal strValueID As String)
            Me.New(core, cValueID.FromString(strValueID))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of cAuxiliaryData that is soft-linked
        ''' to an <see cref="ICoreInterface">ICoreInterface</see>-derived object. 
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="key"></param>
        ''' -------------------------------------------------------------------
        Sub New(ByVal core As cCore, ByVal key As cValueID)
            MyBase.New()

            Me.m_key = key
            Me.m_core = core
            Me.AllowValidation = False

            Me.AllowValidation = True

        End Sub

#End Region ' Constructors

#Region " Public properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this object is allowed to report data changes to the core.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overloads Property AllowValidation() As Boolean
            Get
                Return Me.m_bAllowValidation
            End Get
            Set(ByVal value As Boolean)
                Me.m_bAllowValidation = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the key for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Key() As cValueID
            Get
                Return Me.m_key
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the unique ID assigned to this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ID() As String
            Get
                Return Me.m_key.ToString
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the remark text for this data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overloads Property Remark() As String
            Get
                Return Me.m_strRemark
            End Get
            Set(ByVal value As String)
                If (value <> m_strRemark) Then
                    Me.m_strRemark = value
                    Me.Update()
                End If
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

                If Object.ReferenceEquals(value, Me.VisualStyle) Then Return

                If (Me.m_visualStyle IsNot Nothing) Then
                    Me.m_visualStyle.Container = Nothing
                End If

                Me.m_visualStyle = value

                If (Me.m_visualStyle IsNot Nothing) Then
                    Me.m_visualStyle.Container = Me
                End If

                Me.Update()

            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether an instance holds any data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property IsEmpty() As Boolean
            Get
                Return String.IsNullOrEmpty(Me.Remark) And _
                       (Me.m_visualStyle Is Nothing)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update auxillary data changes to the core.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Update()

            If Me.AllowValidation Then
                ' Notify core, if provided
                If (Me.m_core IsNot Nothing) Then
                    Me.m_core.onChanged(Me, eMessageType.DataModified)
                End If
            End If

        End Sub

#End Region ' Public properties

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.CoreComponent" />
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property CoreComponent() As eCoreComponentType _
            Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.Core
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.CoreComponent" />
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property DataType() As eDataTypes _
            Implements ICoreInterface.DataType
            Get
                Return eDataTypes.Auxillary
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.CoreComponent" />
        ''' -----------------------------------------------------------------------
        Public Property DBID() As Integer Implements ICoreInterface.DBID
            Get
                Return Me.m_iDBID
            End Get
            Set(ByVal value As Integer)
                Me.m_iDBID = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.CoreComponent" />
        ''' -----------------------------------------------------------------------
        Public Function GetID() As String _
               Implements ICoreInterface.GetID
            Return Me.Key.ToString
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the ordinal number in the core storage structures for a core 
        ''' data entity.
        ''' </summary>
        ''' <remarks>
        ''' Since AuxillaryData are indexed via HashTables this property is not used.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Property Index() As Integer _
               Implements ICoreInterface.Index
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.Name" />
        ''' -----------------------------------------------------------------------
        Public Property Name() As String _
                Implements ICoreInterface.Name
            Get
                Return Me.Remark
            End Get
            Set(ByVal value As String)
                Me.Remark = value
            End Set
        End Property

    End Class

End Namespace
