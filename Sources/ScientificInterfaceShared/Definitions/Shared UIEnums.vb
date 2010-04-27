
Option Explicit On
Option Strict On

Namespace Definitions

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type for identifing modifications to a list of items in the user interface, prior to
    ''' updating the list in a batch operation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum AddRemoveItemStatus As Integer
        ''' <summary>Item belongs to the original list.</summary>
        Original = 0
        ''' <summary>Item is flagged to be added to the list.</summary>
        Added
        ''' <summary>Item is flagged for removal from the list.</summary>
        Removed
        ''' <summary>Item does not belong to the list.</summary>
        Invalid
    End Enum

    Public Enum eMCRunDisplayInputValueTypes As Integer
        B = 0
        PB
        EE
        BA
        VU
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that states how line graphs will be rendered.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eSketchDrawModeTypes As Integer
        ''' <summary>The area under a graph will be filled.</summary>
        Fill = 0
        ''' <summary>A graph will be rendered as a line.</summary>
        Line
        ''' <summary>A graph will be rendered as individual points.</summary>
        Dots
        ''' <summary>A graph will be rendered as a line, connecting non-zero points only.</summary>
        LineSelective
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that states how graph tick marks will be scaled.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eAxisTickmarkDisplayModeTypes As Integer
        ''' <summary>Tick marks will be only be displayed for the range with values on an axis.</summary>
        Relative
        ''' <summary>Tick marks will display the full (absolute) range of values on an axis.</summary>
        Absolute
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that states how the values on a graph axis will be scaled.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eAxisAutoScaleModeTypes As Integer
        ''' <summary>A graph will automatically scale its axis to show the range of values on an axis.</summary>
        Auto
        ''' <summary>A graph will not scale its axis to show the range of values on an axis.</summary>
        Fixed
    End Enum

    Public Enum eRightClickAutoScaleModeTypes As Integer
        Auto
        Fixed
    End Enum

    Public Enum eApplyTargetTypes As Integer
        NotSet = 0
        Consumer
        PrimaryProducer
    End Enum

    Public Enum eApplyShapeTypes As Integer
        NotSet = 0
        Forcing
        Mediation
    End Enum

    Public Enum eTracerRunModeTypes As Integer
        Disabled = 0
        RunSim
        RunSpace
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type for identifying the broad categories of time shapes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eShapeCategoryTypes As Integer
        NotSet
        ''' <summary>Shape is a <see cref="EwECore.cForcingFunction">Forcing shape</see>.</summary>
        Forcing
        ''' <summary>Shape is a <see cref="EwECore.cMediationFunction">Mediation shape</see>.</summary>
        Mediation
        ''' <summary>Shape is a Egg production shape.</summary>
        EggProduction
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Type of data being plotted.
    ''' </summary>
    ''' <remarks>This can't be EwECore.eDataTypes because the data comes from the same Core objects. 
    ''' Can't use eVarNameFlags because there is more than one type of data in a plot.</remarks>
    ''' -----------------------------------------------------------------------
    Public Enum ePlotData As Integer
        NotSet = 0
        Biomass
        GroupCatch
        FleetValue
        Effort
        BioEst
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Line types for plots.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eLineType As Integer
        ''' <summary>Line type is not set.</summary>
        NotSet = 0
        ''' <summary>Line denotes model data.</summary>
        ModelData
        ''' <summary>Line denotes reference data.</summary>
        ReferenceData
    End Enum

End Namespace

