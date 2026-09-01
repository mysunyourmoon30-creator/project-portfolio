# Innovation.TotalWeight_PLC — Portfolio Clone

A runnable, from-scratch rebuild of one vertical slice of the MES documented
in [`../README.md`](../README.md): WinForms desktop → ASP.NET Core API →
SQLite, on .NET 8, with simulated PLC/scale hardware. Built across Phases
0–6 per `../README.md` §8.

## Quick start

```bash
dotnet test TotalWeightPlc.slnx   # 67 tests across 8 test projects
```

Then see [`docs/DEMO_SCRIPT.md`](docs/DEMO_SCRIPT.md) to run the API and
desktop app together, or `.\run-demo.ps1` on Windows.

## Layout

- `src/Common/Innovation.Mvp.Core` — fixed MVP contracts (Phase 0)
- `src/Backend/*` — data layer, services, API (Phases 1–2)
- `src/Hardware/*` — `IPlcDevice`/`IScaleReader`/`IBarcodeSource` + simulators (Phase 3)
- `src/Desktop/Innovation.TotalWeight_PLC` — WinForms app (Phase 4)
- `tests/*` — unit, integration, and full-stack E2E tests (Phases 0–5)
- `docs/` — switching databases, connecting real hardware, coverage, demo script, retrospective

See [`docs/RETROSPECTIVE.md`](docs/RETROSPECTIVE.md) for the key architectural
decisions and one real bug the build process caught.
