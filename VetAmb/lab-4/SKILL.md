# Lab 4 Implementation Skill Rules (VetAmb)

## Scope and domain safety
- Apply Lab 4 patterns only to VetAmb domain models: Clinic, Owner, Patient, Vet, Appointment, Service, MedicalRecord.
- Never introduce or copy classes/properties from PDF examples such as Quiz, Question, Client, or similar example-only structures.
- Preserve the existing Modern Glassmorphism UI style (translucent surfaces, blur/backdrop, current custom classes and visual language).

## 1) Form Models for Create/Edit (mandatory)
- All Create and Edit screens must use dedicated Form ViewModels (for example AppointmentFormModel) instead of binding EF entities directly.
- Controller POST actions must accept these Form Models, validate them, and map only allowed fields into entities.
- Protect internal/non-editable properties (Id, audit/soft-delete fields, navigation internals, computed/system fields) from over-posting.
- Keep mapping explicit and readable; do not use mass assignment that can accidentally include forbidden fields.

## 2) Soft Delete policy (mandatory)
- Soft delete must use nullable DateTime field: DeletedAt.
- Deletion operation sets DeletedAt = DateTime.UtcNow (or equivalent standardized timestamp) instead of Remove().
- All list/search/read queries must exclude rows where DeletedAt != null.
- Enforce filtering globally (preferred via EF query filters) so filtering is not forgotten in individual queries.
- Restore workflow, if needed later, must clear DeletedAt intentionally and explicitly.

## 3) Dual validation (mandatory)
- Client-side validation must trigger on blur (when each input loses focus), not only on submit.
- Server-side validation must always check ModelState.IsValid in POST actions.
- Never rely on client validation alone; server validation remains authoritative.
- Return validation messages in the same glassmorphism form layout style used across existing pages.

## 4) Custom date+time picker via Partial View (mandatory)
- Do not use the browser native date picker UI.
- Build and render date/time picker as a reusable Partial View component.
- Partial must support integration with Form Models and validation messages.
- JS behavior must be unobtrusive and reusable, with clear input/output contract.
- Visual design must continue existing translucent panel, border, blur, and spacing conventions.

## 5) AJAX autocomplete component (mandatory)
- Build a reusable autocomplete pattern that fetches options asynchronously from server endpoints via AJAX.
- Component must bind selected item key to a hidden input field (for POST binding), while user sees readable label text.
- Include keyboard navigation and clear no-result/loading states where feasible.
- Endpoint contracts should be simple and consistent (id + display text) for reuse across entities.
- Keep styling aligned with existing custom glassmorphism classes and interactions.

## 6) Localization startup contract (mandatory)
- Configure supported cultures: hr and en-US.
- Default/fallback culture must be hr.
- Request localization middleware must be placed exactly before endpoint routing middleware in Program.cs.

## 7) Implementation order discipline
- For each entity feature, implement in this order:
  1. Form Model
  2. Controller mapping + server validation
  3. Client blur validation wiring
  4. Partial component integration (date picker/autocomplete if needed)
  5. Query filtering/soft-delete behavior checks
- After each step, verify no visual regressions in existing UI classes and layout.

## 8) Compatibility constraints
- Keep architecture compatible with current VetAmb repositories, controllers, and DbContext conventions.
- Avoid breaking existing routes/views unless explicitly migrating them.
- Preserve existing CSS class naming and style tokens; extend styles minimally and consistently.
