#Region " Imports "

Option Strict On

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Categories for sub-dividing groups.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eGroupCategoryTypes As Integer
    All
    PP
    DemPP
    PelPP
    Inv
    DemInv
    PelInv
    Fish
    DemFish
    SDemFish
    LDemFish
    PelFish
    SPelFish
    LPelFish
    MarMam
    Turtles
    Birds
    DemShark
    PelShark
    TopPred
    VertNFish
End Enum

''' ---------------------------------------------------------------------------
''' <summary>
''' Categories for sub-dividing fleets.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eFleetCategoryTypes As Integer
    All
    Trawl
    BottTrawl
    MidTrawl
    BeamTrawl
    PurSein
    Tuna
    DriftNet
    GillNet
    Artisanal
    Recr
End Enum
