# Accessibility testing

This guide documents accessibility validation for desktop UI surfaces.

## Scope

- Dashboard window
- Settings window
- System tray interaction flow

## Current accessibility implementations

- `AutomationProperties.Name` on interactive controls
- Keyboard tab navigation order on dashboard controls
- High-contrast friendly default theme colors
- RTL flow-direction support based on UI culture

## Manual test checklist

1. Navigate dashboard and settings using keyboard only (Tab/Shift+Tab/Enter/Escape).
2. Verify screen reader announces meaningful control names.
3. Confirm text remains readable in Windows high-contrast mode.
4. Verify focus indicator is visible on actionable controls.
5. Validate no critical action depends only on color cues.

## WCAG mapping (desktop focus)

- **Perceivable**: labels and readable contrast
- **Operable**: keyboard accessibility
- **Understandable**: clear control naming and predictable behavior
- **Robust**: automation names compatible with assistive tools
