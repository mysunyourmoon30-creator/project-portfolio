# Test coverage report

Every test project already includes `coverlet.collector` (the default xUnit
template package). To generate a coverage report locally:

```bash
dotnet test TotalWeightPlc.slnx --collect:"XPlat Code Coverage"
```

This drops a Cobertura XML file per test project under
`tests/<Project>/TestResults/<guid>/coverage.cobertura.xml`. To turn that
into an HTML report, install the ReportGenerator tool once:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Then open `coveragereport/index.html`. Not wired into CI for this clone
(README §8.5 lists coverage reporting as cuttable) - run it manually when
you want the number.
