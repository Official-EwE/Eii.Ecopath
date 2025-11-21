Since version 6.6.1, this plug-in is part of the EwE Pro license

At the time of writing, all licensing logic is kept within this plug-in to 
prevent reverse engineering etc. This was a concern when relying on
a number of easily to reverse internal flags, but with the switch to
Treek's licensing library, the license logic should move to the EwECore
namespace.

How it works at the moment:

 - Licensing is handled in cDotSpatialUtils 
 - Compiler directive USE_LICENSE_LIB determines whether Treek is used
 - If Treek is used, a license file is needed that must be generated via 
   Treeks' licensing toolbox or via the server. For more information, see
   the internal\deployment\license folder.
 - If Treek is not used, the variables defined in cDotSpatialUtils.vb > 
   modLicense define the conditions of a license. It is imperative to 
   set these fields correctly when integrating this plug-in in code-only
   applications.
 - By default, DEBUG applications do NOT use Treeks' library.
 - As licensing is currently de-centralized, each plug-in can perform its
   own license checks by implementing ILifeSpanPlugin

In a future version, Treek's library should become part of the EwECore. Plugins
that need to check licenses will simply check if this license is active. 
Recompiling the core without license will stop plug-ins from working, and the
Treek library is reasonably tamper-proof.

Next work:
 - Move Treek to the core, and load any active license on startup
 - Instead of having the spatial temporal framework automatically prompting for 
   a license key, the EwE About box should be the place where users enter a 
   license key, remove a license key, or update to a new license key.