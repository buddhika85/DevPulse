// Action definition interface for generic table component
export interface TableAction {
  label: string; // button text
  color?: string; // Angular Material color (primary, accent, warn)
  icon?: string; // optional Material icon name (e.g., 'edit', 'delete')
  action: string; // action identifier emitted to parent
}
