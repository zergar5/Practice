using Domain.Environment;
using Domain.Nodes;

namespace Tests;

public class TestReceiverAndSourceConstructions
{
    public static (Source<Node2D>[] Sources, ReceiverLine<Node2D>[] Receivers) GetOneConstructions(double sourcePower)
    {
        var sources = new Source<Node2D>[1];
        var receiverLines = new ReceiverLine<Node2D>[sources.Length];

        for (var i = 0; i < sources.Length; i++)
        {
            sources[i] = new Source<Node2D>
            {
                Location = new Node2D
                {
                    X = 0.05,
                    Y = -2.5 - 1 * i
                },
                Power = sourcePower,
            };

            receiverLines[i] = new ReceiverLine<Node2D>
            {
                ReceiverM = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.5,
                },
                ReceiverN = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 1,
                }
            };
        }

        return (sources, receiverLines);
    }

    public static (Source<Node2D>[] Sources, ReceiverLine<Node2D>[] Receivers) GetTwoConstructions(double sourcePower)
    {
        var sources = new Source<Node2D>[2];
        var receiverLines = new ReceiverLine<Node2D>[sources.Length];

        for (var i = 0; i < sources.Length; i++)
        {
            sources[i] = new Source<Node2D>
            {
                Location = new Node2D
                {
                    X = 0.05,
                    Y = -2.5 - 0.5 * i
                },
                Power = sourcePower,
            };

            receiverLines[i] = new ReceiverLine<Node2D>
            {
                ReceiverM = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.25,
                },
                ReceiverN = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.5,
                }
            };
        }

        return (sources, receiverLines);
    }

    public static (Source<Node2D>[] Sources, ReceiverLine<Node2D>[] Receivers) GetFiveConstructions(double sourcePower)
    {
        var sources = new Source<Node2D>[5];
        var receiverLines = new ReceiverLine<Node2D>[sources.Length];

        for (var i = 0; i < sources.Length; i++)
        {
            sources[i] = new Source<Node2D>
            {
                Location = new Node2D
                {
                    X = 0.05,
                    Y = -2.5 - 0.2 * i
                },
                Power = sourcePower,
            };

            receiverLines[i] = new ReceiverLine<Node2D>
            {
                ReceiverM = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.1,
                },
                ReceiverN = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.2,
                }
            };
        }

        return (sources, receiverLines);
    }

    public static (Source<Node2D>[] Sources, ReceiverLine<Node2D>[] Receivers) GetTenConstructions(double sourcePower)
    {
        var sources = new Source<Node2D>[10];
        var receiverLines = new ReceiverLine<Node2D>[sources.Length];

        for (var i = 0; i < sources.Length; i++)
        {
            sources[i] = new Source<Node2D>
            {
                Location = new Node2D
                {
                    X = 0.05,
                    Y = -2.5 - 0.1 * i
                },
                Power = sourcePower,
            };

            receiverLines[i] = new ReceiverLine<Node2D>
            {
                ReceiverM = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.05,
                },
                ReceiverN = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.1,
                }
            };
        }

        return (sources, receiverLines);
    }

    public static (Source<Node2D>[] Sources, ReceiverLine<Node2D>[] Receivers) GetTwentyConstructions(double sourcePower)
    {
        var sources = new Source<Node2D>[20];
        var receiverLines = new ReceiverLine<Node2D>[sources.Length];

        for (var i = 0; i < sources.Length; i++)
        {
            sources[i] = new Source<Node2D>
            {
                Location = new Node2D
                {
                    X = 0.05,
                    Y = -0.1 - 0.1 * i
                },
                Power = sourcePower,
            };

            receiverLines[i] = new ReceiverLine<Node2D>
            {
                ReceiverM = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.05,
                },
                ReceiverN = new Node2D
                {
                    X = sources[i].Location.R(),
                    Y = sources[i].Location.Z() - 0.1,
                }
            };
        }

        return (sources, receiverLines);
    }
}