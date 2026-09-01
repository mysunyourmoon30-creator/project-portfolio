# Connecting real hardware

The demo runs entirely on `SimulatedPlcDevice`/`SimulatedScaleReader` (see
`Innovation.Hardware`). Swapping in real hardware means implementing
`IPlcDevice`/`IScaleReader`/`IBarcodeSource` and changing one DI
registration - nothing else in the API or desktop app needs to change,
because neither ever references a concrete device type.

## PLC (Mitsubishi MX Component)

1. Install MX Component on the target machine and register `ActUtlTypeLib`
   (licensed, Windows-only - not available in the environment this clone was
   built in, hence `MxComponentPlcDevice` in `Innovation.Hardware.RealDevices`
   is a documented placeholder that throws `NotSupportedException`).
2. Add `<COMReference Include="ActUtlTypeLib" .../>` to
   `Innovation.Hardware.RealDevices.csproj`.
3. Configure the **logical station number** for the target PLC (set in MX
   Component's own configuration utility, not in this codebase).
4. Map `IPlcDevice`'s string addresses (e.g. `"D70"`) to `ActUtlType`'s
   `GetDevice`/`SetDevice` calls.
5. The address-to-step mapping lives in the `SEND_STEP_PARAMETER` table
   (seeded by `DemoDataSeeder`) - e.g. step 2 = Feeddoor Step = address
   `D70`-`D74` in the original system (Backend ROADMAP §8.2).

## Scale (RS-232)

`SerialScaleReader` in `Innovation.Hardware.RealDevices` is fully wired
against `System.IO.Ports.SerialPort` (no COM dependency), but the port name,
baud rate, and line format are placeholders - set them to match the actual
scale's configuration before use.

## Swapping the registration

In `Innovation.TotalWeight_PLC`'s composition root (Phase 4), the only
change needed is which concrete type is registered:

```csharp
// Demo:
services.AddSingleton<IPlcDevice>(new SimulatedPlcDevice());
// Real hardware:
services.AddSingleton<IPlcDevice>(new MxComponentPlcDevice());
```
