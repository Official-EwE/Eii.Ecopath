# Eii.Ecopath.ScientificInterfaceShared

Foundation library with Ecopath with Ecosim Scientific User Interface elements

https://ecopath.org/

## Grid CSV files

Grid exports use comma-separated CSV with quoted fields when needed. Visible rows and columns are exported, including
headers and read-only cells; hidden rows and columns are omitted. Floating-point values use round-trip precision and
invariant formatting: a decimal point and no grouping separators, regardless of the computer's locale. Null-styled cells
are exported as empty fields.

Import detects comma, semicolon, or tab delimiters and supports quoted fields. The file must have the same visible row
and column counts as the grid; fixed header text must match. Only editable cells are updated. Numeric values must use a
decimal point without grouping separators: decimal-comma values are not supported, even when quoted. Convert such values
to decimal points before importing. Empty numeric fields are interpreted as the model's null value.

The importer validates the layout and editable values before applying them. A validation failure leaves the grid
unchanged, but a failure while applying values can leave partial changes; check the grid when warned. Other EwE CSV
features may use separate settings and formats.
