'==============================================================================
'
' $Log: cLayer.vb,v $
' Revision 1.1  2008/11/04 04:39:53  jeroens
' Moved
'
' Revision 1.3  2008/10/14 20:23:32  jeroens
' Forged basis for separate editors
'
' Revision 1.2  2008/10/10 20:08:29  jeroens
' Added ValueType
'
' Revision 1.1  2008/10/10 18:03:21  jeroens
' Renamed
'
' Revision 1.1  2008/09/26 07:31:58  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.Misc.Colours
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' <summary>
    ''' Administrative unit, maintains data for a single layer.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Ok, the basemap layer thing has gotten so complex that a bit of an explanation would not hurt.
    ''' </para>
    ''' <para>
    ''' The entire basemap chain consists of the following collaborating classes:
    ''' <list>
    ''' <item>
    ''' <description>One <see cref="cEcospaceBasemap">basemap</see> which defines the size and other aspects
    ''' of the map currently active in Ecospace. This class also provides access to individual data
    ''' <see cref="cEcospaceLayer">layers</see>.
    ''' </description></item>
    ''' <item><description>
    ''' <para>
    ''' Several <see cref="cEcospaceLayer">data layers</see> which each expose 
    ''' spatial array(s) of data. Basemap layers are two dimensional, allowing access to the array
    ''' via cell(row, col) interaction. Poking around in a basemap layer in fact modifies Ecospace
    ''' spatial cells of spatial data array that the the layer is connected to.
    ''' </para>
    ''' <para>
    ''' To obtain a layer from the basemap, call <see cref="cEcospaceBasemap.Layers">cEcospaceBasemap.Layers</see>
    ''' or one of the Layer-exposing properties of that class.
    ''' </para>
    ''' </description></item>
    ''' <item><description>
    ''' <para>
    ''' GUI <see cref="cLayer">Layers</see> combine one or more <see cref="cEcospaceLayer">core
    ''' basemap layers</see> as a single unit for display and interaction in the user interface. The GUI
    ''' Layer uses a <see cref="cLayerRenderer">layer renderer</see> to decide how this assembly
    ''' of core data is reflected.
    ''' </para>
    ''' <para>
    ''' To standardize the use of layers, the GUI provides a <see cref="cLayerFactory">factory</see>
    ''' class that delivers layers in a standardized way. To obtain one or more layers, call
    ''' <see cref="cLayerFactory.GetLayers">GetLayers</see>.
    ''' </para>
    ''' <para>
    ''' You can also create your own layers if need be, but try to use the factory whenever possible.
    ''' </para>
    ''' </description></item>
    ''' </list>
    ''' </para>
    ''' </remarks>
    Public Class cLayer

        ''' <summary>
        ''' Enumerated type to indicate layer changes.
        ''' </summary>
        Public Enum eChangeFlags As Integer
            ''' <summary>Value to indicate that a layers' map data has changed.</summary>
            Map = 1
            ''' <summary>Value to indicate that a layers' visual representation style has changed.</summary>
            VisualStyle = 2
            ''' <summary>Value to indicate that a layers' visible state has changed.</summary>
            Visibility = 4
            ''' <summary>Value to indicate that a layers' selected state has changed.</summary>
            Selected = 8
            ''' <summary>Value to indicate that one ore more of a layers' name, description (?) 
            ''' and other descriptive values have changed.</summary>
            Descriptive = 16
            ''' <summary>Value to indicate that a layers' editable state has changed.</summary>
            Editable = 32
            ''' <summary>All possible flags.</summary>
            All = &HFFFF

        End Enum

        Private m_core As cCore = Nothing

        Private m_strName As String = ""
        Private m_source As cCoreInputOutputBase = Nothing
        Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
        Private m_data As cEcospaceLayer = Nothing
        Private m_valueType As Type = GetType(Single)
        Private m_valueSet As Single = cCore.NULL_VALUE
        Private m_valueClear As Single = cCore.NULL_VALUE
        Private m_renderer As cLayerRenderer = Nothing
        Private m_editor As cLayerEditor = Nothing

        Private m_bSelected As Boolean = False
        Private m_bModified As Boolean = False

        ''' <summary>
        ''' The <see cref="cProperty">property</see> that provides the name of this layer.
        ''' As well, this property is used to simulate live map changes to sync different
        ''' layer instances linked to the same data.
        ''' </summary>
        ''' <remarks>
        ''' This is a hack solution. <see cref="cEcospaceLayer">Basemap layers</see> are not exposed as true
        ''' <see cref="EwECore.ValueWrapper.cValue">core value objects</see>. To provide layers with common GUI issues
        ''' such as remark feedback and broadcasted updates, as well as the ability to attach 
        ''' <see cref="cVisualStyle">Visual Styles</see> to layers, a hidden property is used.
        ''' </remarks>
        Private m_propName As cProperty = Nothing

        Public Event LayerChanged(ByVal layer As cLayer, ByVal updateType As eChangeFlags)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal data As cEcospaceLayer, _
                ByVal renderer As cLayerRenderer, _
                ByVal editor As cLayerEditor)
            Me.New(data, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, Nothing, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="source">
        ''' The core object that serves two purposes:
        ''' <list type="number">
        ''' <item><description>Provide the dynamic name for a layer</description></item>
        ''' <item><description>Provide the definition for distributing data changes</description></item>
        ''' </list>
        ''' </param>
        ''' <param name="varName">The name of the variable to associate data changes with</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal data As cEcospaceLayer, _
                ByVal renderer As cLayerRenderer, _
                ByVal editor As cLayerEditor, _
                ByVal source As cCoreInputOutputBase, _
                Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name)

            Me.New(data, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, source, varName)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="objValueSet"></param>
        ''' <param name="objValueClear"></param>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal data As cEcospaceLayer, _
                ByVal renderer As cLayerRenderer, _
                ByVal editor As cLayerEditor, _
                ByVal objValueSet As Single, _
                ByVal objValueClear As Single)

            Me.New(data, renderer, editor, objValueSet, objValueClear, Nothing, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="source">
        ''' The core object that serves two purposes:
        ''' <list type="number">
        ''' <item><description>Provide the dynamic name for a layer</description></item>
        ''' <item><description>Provide the definition for distributing data changes</description></item>
        ''' </list>
        ''' </param>
        ''' <param name="varName">The name of the variable to associate data changes with</param>
        ''' <param name="objValueSet"></param>
        ''' <param name="objValueClear"></param>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal data As cEcospaceLayer, _
                ByVal renderer As cLayerRenderer, _
                ByVal editor As cLayerEditor, _
                ByVal objValueSet As Single, _
                ByVal objValueClear As Single, _
                ByVal source As cCoreInputOutputBase, _
                Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name)

            Me.m_core = cCore.GetInstance()

            ' Sanity checks
            Debug.Assert(Not Object.ReferenceEquals(data, Nothing))
            Debug.Assert(Not Object.ReferenceEquals(renderer, Nothing))
            Debug.Assert(Not Object.ReferenceEquals(editor, Nothing))

            Me.m_strName = ""
            Me.m_source = source
            Me.m_varName = varName
            Me.m_data = data
            Me.m_renderer = renderer
            Me.m_editor = editor
            Me.m_valueSet = objValueSet
            Me.m_valueClear = objValueClear
            Me.m_propName = cPropertyManager.GetInstance().GetProperty(source, varName)

            If (m_propName IsNot Nothing) Then
                Me.m_valueType = m_propName.GetValueType()
                AddHandler Me.m_propName.PropertyChanged, AddressOf OnPropertyChanged
            End If

            ' Update editor
            Me.m_editor.Initialize(Me)
            ' Update representation
            Me.m_renderer.SetValueRange(Me.m_data.MinValue, Me.m_data.MaxValue)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Copy constructor.
        ''' </summary>
        ''' <param name="layer">The layer to copy.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal layer As cLayer)
            Me.New(layer.Data, layer.Renderer.Clone(), layer.Editor, layer.ValueSet, layer.ValueClear, layer.Source, layer.VarName)
            Me.Name = layer.Name
            Me.IsSelected = layer.IsSelected
        End Sub

        Private m_bInUpdate As Boolean = False

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Call this whenever properties and visual aspects of the layer have changed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Update(ByVal updateType As eChangeFlags)

            If Me.m_bInUpdate = True Then Return

            Me.m_bInUpdate = True

            ' Assess changes

            ' Map has changed via user drawing
            If ((updateType And eChangeFlags.Map) = eChangeFlags.Map) Then
                ' Update visuals
                Me.m_renderer.SetValueRange(Me.m_data.MinValue, Me.m_data.MaxValue)
                ' Inform the core
                Me.m_core.onChanged(Me.m_data)

                ' Fire off property change to make other copies of this layer respond. This is a hack.
                If Me.m_propName IsNot Nothing Then
                    Me.m_propName.FireChangeNotification(cProperty.eChangeFlags.Custom)
                End If

            End If

            If ((updateType And eChangeFlags.VisualStyle) = eChangeFlags.VisualStyle) Then
                Me.m_renderer.Update()
                Me.m_core.VisualStyleChanged(Me.m_renderer.VisualStyle)
            End If

            ' Inform the world last
            RaiseEvent LayerChanged(Me, updateType)

            Me.m_bInUpdate = False

        End Sub

#Region " Properties "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of this layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Name() As String
            Get
                ' If no hard-wired name provided, obtain the name from the attached property
                If String.IsNullOrEmpty(Me.m_strName) And (Me.m_propName IsNot Nothing) Then
                    Return CStr(Me.m_propName.GetValue())
                End If
                Return Me.m_strName
            End Get
            Set(ByVal value As String)
                Me.m_strName = value
                Me.Update(eChangeFlags.Descriptive)
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the source of this layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_source
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the variable of the source this layer applies to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the underlying core-exposed layer data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Data() As cEcospaceLayer
            Get
                Return Me.m_data
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the value that this layer interprets as relevant values.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueSet() As Single
            Get
                Return Me.m_valueSet
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the value that this layer interprets as clear values.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueClear() As Single
            Get
                Return Me.m_valueClear
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a given cell position has a value.
        ''' </summary>
        ''' <param name="ptCell"></param>
        ''' -----------------------------------------------------------------------
        Public Overridable ReadOnly Property HasValue(ByVal ptCell As Point) As Boolean
            Get
                If Object.ReferenceEquals(Me.m_valueSet, Nothing) Then Return False

                ' ToDo_JS: Build smartness to detect dimension of layer type
                Dim cellValue As Single = CSng(Me.Value(ptCell))

                If Me.m_valueSet.Equals(cCore.NULL_VALUE) Then
                    If Me.m_valueClear.Equals(cCore.NULL_VALUE) Then
                        Return (cellValue <> cCore.NULL_VALUE)
                    Else
                        Return (cellValue <> 0)
                    End If
                End If
                Return Me.m_valueSet.Equals(cellValue)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value in the underlying data layer.
        ''' </summary>
        ''' <param name="ptCell"></param>
        ''' -----------------------------------------------------------------------
        Public Property Value(ByVal ptCell As Point) As Object
            Get
                Return Me.m_data.Cell(ptCell.Y, ptCell.X)
            End Get
            Set(ByVal value As Object)
                Me.m_data.Cell(ptCell.Y, ptCell.X) = CSng(value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the data type of values in the underlying data layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueType() As Type
            Get
                Return Me.m_valueType
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the layer <see cref="cLayerRenderer">renderer</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Renderer() As cLayerRenderer
            Get
                Return Me.m_renderer
            End Get
            Set(ByVal value As cLayerRenderer)
                Me.m_renderer = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the layer <see cref="cLayerEditor">editor</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Editor() As cLayerEditor
            Get
                Return Me.m_editor
            End Get
            Set(ByVal value As cLayerEditor)
                Me.m_editor = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer is selected.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsSelected() As Boolean
            Get
                Return Me.m_bSelected
            End Get
            Set(ByVal value As Boolean)
                Me.m_bSelected = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer has been modified.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsModified() As Boolean
            Get
                Return Me.m_bModified
            End Get
            Set(ByVal value As Boolean)
                Me.m_bModified = value
                Me.m_data.Invalidate()
            End Set
        End Property

#End Region ' Properties

#Region " Events "

        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

            If Me.m_bInUpdate Then Return

            ' Translate property change flags into layer change flags
            Dim flag As cLayer.eChangeFlags = 0

            ' Has the name or remark changed?
            If (changeFlags And (cProperty.eChangeFlags.Value Or cProperty.eChangeFlags.Remarks)) > 0 Then
                ' Send out layer name change event
                flag = flag Or eChangeFlags.Descriptive
            End If

            ' Using the property hacK?
            If (changeFlags And cProperty.eChangeFlags.Custom) > 0 Then
                ' Not so sure!
                flag = flag Or (eChangeFlags.All And (Not eChangeFlags.Map))
            End If

            If (flag <> 0) Then Me.Update(flag)
        End Sub

#End Region ' Events

#Region " Overrides "

        Protected Overrides Sub Finalize()

            If Me.m_propName IsNot Nothing Then
                RemoveHandler Me.m_propName.PropertyChanged, AddressOf OnPropertyChanged
                Me.m_propName = Nothing
            End If

            MyBase.Finalize()
        End Sub

#End Region ' Overrides

    End Class ' Layer

End Namespace
