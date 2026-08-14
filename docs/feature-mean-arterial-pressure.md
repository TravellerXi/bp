# Feature: Mean Arterial Pressure

## User story

> **As a** person checking my blood pressure
> **I want** to see my mean arterial pressure and whether it falls in a safe band
> **So that** I can tell whether my average perfusion pressure is adequate, not just which
> systolic/diastolic category I land in.

## Why this feature

The existing calculator reports only the NHS category. Two readings can share a category yet
differ materially in average pressure: 119/60 and 100/79 are both Ideal, but their mean
arterial pressures are 79.7 and 86.0 mmHg.

More usefully, the two measures can disagree. A reading of 89/60 is classified as **Ideal** by
category — neither reading is low enough to trigger the Low band — yet its MAP is 69.7 mmHg,
which falls **below** the 70 mmHg perfusion threshold. MAP is the value clinicians use when the
question is "is blood actually reaching the organs", so it adds information the existing output
cannot express rather than restating it.

## Acceptance criteria

1. MAP is calculated as `diastolic + (systolic − diastolic) / 3` and displayed to one decimal place.
2. MAP is classified into a band using inclusive lower limits, consistent with `BPCategory`:
   - `High` when MAP ≥ 100 mmHg
   - `Normal` when 70 ≤ MAP < 100 mmHg
   - `Low` when MAP < 70 mmHg
3. Both the value and the band appear on the results panel after a valid submission.
4. Invalid input continues to be rejected before any MAP is shown.
5. MAP always lies between the diastolic and systolic readings.

## Definition of done

| Gate | Evidence |
|---|---|
| Feature branch | `feature/mean-arterial-pressure`, merged to `develop` by pull request |
| Unit tests | `MeanArterialPressureTests` — calculation, band boundaries, degenerate case, invariant sweep |
| BDD | `Features/MeanArterialPressure.feature` — Scenario Outlines for value and band |
| E2E | `e2e/tests/bp-category.spec.ts` — `Mean arterial pressure` describe block, three browsers |
| CI | build, tests, coverage, SonarCloud and dependency scan all green on the pull request |

## Traceability

Addresses CA1 marking-scheme row **New feature (5 marks) — "appropriate feature, user story,
feature branch"**, and contributes evidence to **unit test** and **E2E tests** rows.
