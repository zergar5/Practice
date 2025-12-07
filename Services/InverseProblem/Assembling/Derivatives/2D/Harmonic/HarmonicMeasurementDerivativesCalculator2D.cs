using Application.DirectProblem._2D;
using Application.FEM._2D.Grid;
using Application.InverseProblem._2D;
using Application.InverseProblem.Parameters;
using Domain.Environment;
using Domain.Materials;
using Domain.Nodes;
using System.Numerics;
using Application.InverseProblem._2D.Harmonic;

namespace Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;

public class HarmonicMeasurementDerivativesCalculator2D : IHarmonicMeasurementDerivativesCalculator
{
    private readonly IInverseProblemContextProvider<HarmonicInverseProblem2DContext> _problemContextProvider;
    private readonly IHarmonicDirectProblem2DWithSources<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu> _directProblem;

    private readonly double _sigmaParameterDelta;
    private readonly double _boundParameterDelta;

    private GridBuilder2D.Grid2DParameters? _gridParameters;
    private MaterialWithSigmaMu[]? _materials;
    private double[,]? _measurementDerivatives;

    public HarmonicMeasurementDerivativesCalculator2D
    (
        IInverseProblemContextProvider<HarmonicInverseProblem2DContext> problemContextProvider,
        IHarmonicDirectProblem2DWithSources<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu> directProblem,
        double sigmaParameterDelta = 1e-1,
        double boundParameterDelta = 1e-2
    )
    {
        _problemContextProvider = problemContextProvider;
        _directProblem = directProblem;
        _sigmaParameterDelta = sigmaParameterDelta;
        _boundParameterDelta = boundParameterDelta;
    }

    public void Init()
    {
        var problemContext = _problemContextProvider.Get();
        var gridParameters = problemContext.GridParameters;

        _gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = gridParameters.XControlPoints.ToArray(),
            YControlPoints = gridParameters.YControlPoints.ToArray(),
            XSplitStrategies = gridParameters.XSplitStrategies,
            YSplitStrategies = gridParameters.YSplitStrategies,
            Areas = gridParameters.Areas,
        };

        var materials = problemContext.Materials;

        _materials = materials.Select(m => new MaterialWithSigmaMu
        {
            Id = m.Id,
            Sigma = m.Sigma
        }).ToArray();

        _directProblem.SetDefinedValueBoundaryCondition(problemContext.DefinedValueBoundaryConditions);

        _measurementDerivatives = new double[problemContext.Frequencies.Length, problemContext.ReceiverLines.Length];
    }

    public double[,] CalcDerivativesForParameter(Parameter parameter, double[,] measurements)
    {
        ResetDirectProblemParameters();

        var parameterValue = GetParameterValue(parameter);
        var delta = parameter.Type switch
        {
            ParameterType.Sigma => parameterValue * _sigmaParameterDelta,
            ParameterType.VerticalBound or ParameterType.HorizontalBound => _boundParameterDelta,
            _ => parameterValue
        };

        SetParameterValue(parameter, parameterValue + delta);

        switch (parameter.Type)
        {
            case ParameterType.Sigma:
                _directProblem.SetMaterials(_materials!);
                break;
            case ParameterType.VerticalBound or ParameterType.HorizontalBound:
                _directProblem.GenerateGrid(_gridParameters!);
                break;
            default:
                throw new NotSupportedException();
        }

        var problemContext = _problemContextProvider.Get();
        var frequencies = problemContext.Frequencies;
        var receiverLines = problemContext.ReceiverLines;
        var sources = problemContext.Sources;

        for (var i = 0; i < frequencies.Length; i++)
        {
            _directProblem.SetFrequency(frequencies[i]);

            for (var j = 0; j < receiverLines.Length; j++)
            {
                _directProblem.SetSources([sources[j]]);

                var solution = _directProblem.Solve();

                var fieldM = solution.Get(receiverLines[j].ReceiverM);
                var fieldN = solution.Get(receiverLines[j].ReceiverN);

                _measurementDerivatives![i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

                _measurementDerivatives[i, j] =
                    (_measurementDerivatives[i, j] - measurements[i, j]) / delta;

                Console.Write($"Parameter {parameter.Type} {parameter.Index} frequency {i} source {j}                                  \r");
            }
        }

        //SetParameterValue(parameter, parameterValue);

        return _measurementDerivatives!;
    }

    private double GetParameterValue(Parameter parameter)
    {
        var problemContext = _problemContextProvider.Get();

        return parameter.Type switch
        {
            ParameterType.Sigma => problemContext.Materials[parameter.Index].Sigma,
            ParameterType.VerticalBound => problemContext.GridParameters.XControlPoints[parameter.Index],
            ParameterType.HorizontalBound => problemContext.GridParameters.YControlPoints[parameter.Index],
            _ => throw new NotSupportedException()
        };
    }

    private void SetParameterValue(Parameter parameter, double value)
    {
        switch (parameter.Type)
        {
            case ParameterType.Sigma:
                _materials![parameter.Index].Sigma = value;
                break;
            case ParameterType.VerticalBound:
                _gridParameters!.XControlPoints[parameter.Index] = value;
                break;
            case ParameterType.HorizontalBound:
                _gridParameters!.YControlPoints[parameter.Index] = value;
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private void ResetDirectProblemParameters()
    {
        var problemContext = _problemContextProvider.Get();
        var gridParameters = problemContext.GridParameters;

        for (var i = 0; i < gridParameters.XControlPoints.Length; i++)
        {
            _gridParameters!.XControlPoints[i] = gridParameters.XControlPoints[i];
        }

        for (var i = 0; i < gridParameters.YControlPoints.Length; i++)
        {
            _gridParameters!.YControlPoints[i] = gridParameters.YControlPoints[i];
        }

        _directProblem.SetGrid(problemContext.Grid);

        var materials = problemContext.Materials;

        for (var i = 0; i < materials.Length; i++)
        {
            _materials![i].Sigma = materials[i].Sigma;
        }

        _directProblem.SetMaterials(materials);
    }
}